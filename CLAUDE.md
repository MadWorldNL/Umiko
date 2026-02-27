# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Build entire solution
dotnet build source/MadWorldNL.Umiko.slnx

# Run tests
dotnet test source/MadWorldNL.Umiko.slnx

# Run the full application (starts all services via Aspire)
dotnet run --project source/MadWorldNL.Umiko.Aspire

# Run individual projects
dotnet run --project source/MadWorldNL.Umiko.Controllers.Api
dotnet run --project source/MadWorldNL.Umiko.Controllers.Bus
dotnet run --project source/MadWorldNL.Umiko.Controllers.Web.Administrators
dotnet run --project source/MadWorldNL.Umiko.Controllers.Web.Users
```

## Architecture

Umiko is a .NET 10.0 distributed web application using Microsoft Aspire for orchestration.

### Project Dependency Graph

```
Aspire Host (Orchestrator)
├── Controllers.Api (REST API)
│   ├── Application.Functions (Business Logic)
│   │   └── Application.Domain (Entities)
│   │       └── Application.Frameworks (DDD Building Blocks)
│   ├── Infrastructures.Postgresql
│   │   └── Application.Domain
│   └── Infrastructures.RabbitMQ
│       └── Application.Domain
├── Controllers.Bus (Message Consumer)
│   ├── Application.Functions
│   ├── Infrastructures.Postgresql
│   └── Infrastructures.RabbitMQ
├── Controllers.Web.Administrators (Blazor WASM - Admin Portal)
└── Controllers.Web.Users (Blazor WASM - User Portal)
```

### Backing Services

- **PostgreSQL**: Relational database with pgAdmin, credentials managed via Aspire secret parameters
- **RabbitMQ**: Message broker with management plugin, credentials managed via Aspire secret parameters
- **Keycloak**: Identity and access management server with data volume persistence
- **MartenDB**: Event store running on the same PostgreSQL instance as EF Core, isolated in the `marten` schema. Registered in both API and Bus via `builder.AddNpgsqlDbContext<UmikoContext>("UmikoDb")` + `services.AddMarten(...).UseLightweightSessions()`. Used by `EventRepository<TAggregate, TId>` in `Infrastructures.Postgresql`.

### Layer Responsibilities

- **Aspire Host**: Orchestrates all services and infrastructure for local development
- **Controllers.Api**: REST API with OpenAPI support and OpenTelemetry instrumentation, handles HTTP requests
- **Controllers.Api.Contracts**: API contract definitions (DTOs/request-response models)
- **Controllers.Bus**: Background message processing service with OpenTelemetry instrumentation, consumes messages from RabbitMQ
- **Controllers.Web.Administrators/Users**: Two separate Blazor WebAssembly client apps (client-side rendering)
- **Application.Functions**: Business logic layer, shared by API and Bus
- **Application.Frameworks**: DDD building blocks (`DDD/`: Entity, AggregateRoot, ValueObject, IDomainEvent), functional types (`Functional/`: Option\<T\> with Some\<T\>/None\<T\>; Result\<T\> with Success\<T\>/Failure\<T\>, both with Match, plus `IsSuccess` and `Error` properties), and service bus abstractions (`ServiceBus/`: IQuery\<TResponse\>, IQueryHandler\<TQuery, TResponse\>, ICommand, ICommand\<TResponse\>, ICommandHandler\<TCommand\>, ICommandHandler\<TCommand, TResponse\>, LoggingQueryHandler, LoggingCommandHandler, IEvent, IEventHandler\<TEvent\>, LoggingEventHandler, IMessageBus) — no dependencies, foundational layer. `AggregateRoot<TId>` implements the apply pattern: `Apply(IDomainEvent)` raises a new event (mutates state via reflection + queues for dispatch), `Reconstitute(IEnumerable<IDomainEvent>)` replays history (mutates state only, nothing queued). Concrete aggregates implement `private void When(TEvent)` methods for state mutation; reflection looks up `When` by event type. `[UsedImplicitly]` suppresses the unused-method warning on `When` handlers.
- **Application.Domain**: Domain entities and business rules, depends on Frameworks. Status folder contains `IDatabaseStatusRepository`, `IMessageBusStatusRepository`, queries and results for both database and messaging connectivity checks. `Repositories/IEventRepository<TAggregate, TId>` defines `SaveAsync` and `LoadAsync` (returns `Option<TAggregate>`) for event-sourced aggregates.
- **Infrastructures.Postgresql**: PostgreSQL data access, depends on Domain. Implements `IDatabaseStatusRepository` via `DatabaseStatusRepository` using EF Core `CanConnectAsync`. Implements `IEventRepository<TAggregate, TId>` via `EventRepository<TAggregate, TId>` using Marten `IDocumentSession`: `SaveAsync` appends domain events to the stream and clears them; `LoadAsync` fetches the stream, creates the aggregate via `Activator.CreateInstance(nonPublic: true)`, then calls `Reconstitute`.
- **Infrastructures.RabbitMQ**: RabbitMQ messaging integration, depends on Domain. Implements `IMessageBus` via `RabbitMqMessageBus` (uses `IConnection` from `Aspire.RabbitMQ.Client`; `Send<TCommand>` uses Direct exchange, `Publish<TEvent>` uses Fanout exchange, both serialized as JSON with `Persistent = true`). Also implements `IMessageBusStatusRepository` via `MessageBusStatusRepository` (checks `connection.IsOpen`). Provides `CommandConsumer<TCommand>` (Direct exchange, binds named queue, resolves `ICommandHandler<TCommand>`) and `EventConsumer<TEvent>` (Fanout exchange, binds named queue, resolves `IEventHandler<TEvent>`) — both are `BackgroundService` implementations with activity tracing and ack/nack handling. Registered via `AddRabbitMqServices()` extension. Connection string key: `ConnectionStrings:UmikoBus`

### Key Configuration

- **Central package versions**: All NuGet versions managed in `source/Directory.Packages.props`
- **Build settings**: Strict mode enabled in `source/Directory.Build.props` (warnings as errors, nullable enabled)
- **Root namespace**: `MadWorldNL.Umiko`

### Development URLs (when running individual projects)

- API: `https://localhost:7115` or `http://localhost:5106`
- Bus: `https://localhost:7109` or `http://localhost:5130`
- Admin Portal: `https://localhost:7209` or `http://localhost:5214`
- User Portal: `https://localhost:7292` or `http://localhost:5293`

### Solution File

Uses the newer `.slnx` format: `source/MadWorldNL.Umiko.slnx`

### OpenTelemetry

Both `Controllers.Api` and `Controllers.Bus` include OpenTelemetry instrumentation configured via a `Configurations/OpenTelemetryExtensions.cs` extension method. This sets up:
- Logging export with formatted messages and scopes
- ASP.NET Core and Kestrel metrics
- ASP.NET Core and HTTP client tracing
- OTLP export when `OTEL_EXPORTER_OTLP_ENDPOINT` is configured
- Resource attribute `log_source = "application"` on all telemetry, used to distinguish app OTLP logs from Kubernetes pod stdout logs in Loki/Grafana

### Health Checks

- **API & Bus**: Use the built-in ASP.NET Core health checks middleware (`builder.Services.AddHealthChecks()` + `app.MapHealthChecks("/health")`), exposed at `/health`. All registered health checks (including the Aspire PostgreSQL check) are excluded via `Predicate = _ => false` to avoid database queries on every health probe — the endpoint returns `Healthy` without executing any checks. Database connectivity is available separately via the `/Status/Database` endpoint; RabbitMQ connectivity via `/Status/MessageBus`.
- **Web projects**: Serve a static `wwwroot/health.txt` file at `/health.txt`, plus a `/health` Blazor page using `EmptyLayout`
- **Aspire**: All four services have `.WithHttpHealthCheck()` configured in `AppHost.cs` — API and Bus use `/health`, web apps use `/health.txt`
- Backing services (PostgreSQL, RabbitMQ, Keycloak) have automatic health checks provided by their Aspire hosting packages

### Rate Limiting

Both `Controllers.Api` and `Controllers.Bus` use ASP.NET Core rate limiting middleware configured via `Configurations/RateLimiterExtensions.cs`:
- **Strategy**: Fixed window rate limiter, partitioned by client IP
- **Default limit**: 100 requests per minute per IP (configurable via `RateLimiter:PermitLimit`)
- **Partition key**: Reads `X-Forwarded-For` header first (for reverse proxy support), falls back to `RemoteIpAddress`
- **Health check exclusion**: `/health` endpoint has rate limiting disabled via `.DisableRateLimiting()`
- **Middleware order**: `UseRateLimiter()` runs before `UseForwardedHeaders()` so it can read the raw `X-Forwarded-For` header before the forwarded headers middleware consumes it
- **Rejection**: Returns HTTP 429 Too Many Requests

### API Documentation

Both `Controllers.Api` and `Controllers.Bus` use [Scalar](https://github.com/scalar/scalar) (`Scalar.AspNetCore`) to render interactive API reference documentation from OpenAPI specs. Available in development mode at `/scalar/v1`. The API registers a JWT Bearer security scheme via `BearerSecuritySchemeTransformer` (`IOpenApiDocumentTransformer`), which makes Scalar display an authentication input for entering a Bearer token.

### Authentication

`Controllers.Api` uses JWT Bearer authentication configured via `Configurations/AuthenticationExtensions.cs` and `Configurations/AuthenticationSettings.cs`:

- **Config section**: `Authentication` with keys `Authority` (Keycloak realm URL), `Audience` (client ID, e.g. `UmikoApi`), and `ValidateUser` (bool)
- **Aspire injection**: The main `AppHost.cs` injects `Authentication__Authority` dynamically using `ReferenceExpression.Create($"{keycloak.GetEndpoint("http")}/realms/Umiko")` so the API always points to the correct Keycloak instance
- **`ValidateUser = false`**: Disables all token validation (issuer, audience, lifetime, signature) — used in development without a running Keycloak. When false, a custom `SignatureValidator` is set that bypasses signature checks via `JsonWebTokenHandler.ReadJsonWebToken`
- **`BearerSecuritySchemeTransformer`**: An `IOpenApiDocumentTransformer` that adds the `Bearer` HTTP security scheme to `document.Components.SecuritySchemes` and appends a global `OpenApiSecurityRequirement` using `OpenApiSecuritySchemeReference`. Registered via `AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>())`
- **Secured endpoints**: Use `.RequireAuthorization()` on the route builder. Status and health endpoints are unauthenticated

### Aggregate Pattern

Aggregates in `Application.Domain` follow these conventions:

- **Factory method**: Use a `public static Create(...)` factory instead of a public constructor. The constructor is `private`.
- **Private parameterless constructor**: Required for EF Core reconstitution (e.g. `private CurriculumVitae()`).
- **Apply pattern**: The constructor calls `Apply(new SomeEvent { ... })` to raise a domain event. State is never set directly — it is always set inside a `private void When(TEvent)` handler.
- **`When` handlers**: Each event type gets a `[UsedImplicitly] private void When(TEvent)` method that mutates aggregate state. Called via reflection by `AggregateRoot.ApplyEvent`.
- **Value objects**: Properties that group related primitives (e.g. `FullName`) extend `ValueObject`, are `sealed`, immutable (`get`-only), and validate in the constructor.
- **Domain events**: Named in past tense (e.g. `CurriculumVitaeCreated`), implemented as `record` with `required init` properties including `Id`, `OccurredOn`, and any relevant state.
- **ID generation**: The API endpoint generates the aggregate `Id` server-side (`Guid.NewGuid()`) before sending the command, and returns it in the `202 Accepted` response body (`CreateCurriculumVitaeResponse`). The command carries the ID so the Bus uses the same value when creating the aggregate.

### Query, Command and Event Handler Pattern

`Application.Functions` uses `IQueryHandler`, `ICommandHandler`, and `IEventHandler` interfaces from `Application.Frameworks/ServiceBus/` for all business logic. Key conventions:

- **Query**: A record implementing `IQuery<TResponse>`, located in `Application.Domain` alongside its result type (e.g. `Status/GetDatabaseStatusQuery.cs`, `Status/GetDatabaseStatusResult.cs`)
- **Command**: A record implementing `ICommand` (no response) or `ICommand<TResponse>` (with response), located in `Application.Domain`
- **Event**: A record implementing `IEvent`, located in `Application.Domain`
- **Result**: A record holding the handler's output, defined in `Application.Domain` (e.g. `GetDatabaseStatusResult(bool IsConnected)`)
- **Handler**: A class in `Application.Functions` implementing `IQueryHandler<TQuery, TResponse>`, `ICommandHandler<TCommand>`/`ICommandHandler<TCommand, TResponse>`, or `IEventHandler<TEvent>`, returns `Task<Result<TResponse>>` — success wrapped in `Result<TResponse>.Success(value)`, failures in `Result<TResponse>.Failure(exception)`
- **Registration**: Concrete handlers registered by type, then all handlers automatically decorated with logging via open-generic `Decorate` in `FunctionsServiceCollectionExtensions`: `services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandler<,>))`, same for both `ICommandHandler` variants and `IEventHandler<>`
- **Controllers**: Inject the handler interface directly and use `Match` to map success/failure to HTTP responses

### Test AppHost (Aspire.Tests)

The `MadWorldNL.Umiko.Aspire.Tests` project is a simplified Aspire AppHost used by both Integration and E2E tests. Unlike the main AppHost, it excludes Keycloak and uses default credentials (no secret parameters) for PostgreSQL and RabbitMQ. It overrides `RateLimiter__PermitLimit` to `5` (instead of production default 100) for faster rate limiter integration tests. It also sets `Authentication__ValidateUser` to `false` so tests run without a live Keycloak instance — no token signature, issuer, or audience validation is performed. Tests reference it via `DistributedApplicationTestingBuilder.CreateAsync<Projects.Aspire_Tests>()`.
### Unit Tests

The `Application.Frameworks.UnitTests` project uses Reqnroll (BDD) with xUnit and Shouldly for unit-level tests of the Frameworks layer. Key patterns:

- **Reqnroll + xUnit + Shouldly**: Tests are written as Gherkin feature files (`.feature`) with C# step definitions using `[Binding]` and `[Scope(Feature = "...")]` attributes. Assertions use [Shouldly](https://docs.shouldly.org/)
- **Feature files**: Located in `Features/` mirroring the source structure (e.g. `Features/DDD/Entity.feature`, `Features/Functional/Option.feature`, `Features/Functional/Result.feature`)
- **Step definitions**: Located in `StepDefinitions/` with matching subfolder structure (e.g. `StepDefinitions/DDD/EntitySteps.cs`)
- **Test helpers**: Concrete test double classes in `Helpers/` used to instantiate abstract types (e.g. `TestEntity`, `TestAggregateRoot`, `TestValueObject`, `TestDomainEvent`)
- **Global usings**: Defined in `GlobalUsings.cs`

### Integration Tests

The `Controllers.IntegrationTests` project uses Reqnroll (BDD) with Aspire.Hosting.Testing to run integration tests against the full distributed application. Key patterns:

- **Reqnroll + xUnit + Shouldly**: Tests are written as Gherkin feature files (`.feature`) with C# step definitions using `[Binding]` and `[Scope(Feature = "...")]` attributes. Assertions use [Shouldly](https://docs.shouldly.org/) (e.g. `x.ShouldBe(expected)`, `x.ShouldNotBeNull()`)
- **Feature files**: Located in `Features/` (e.g. `Features/Api/StatusEndpoints/Ping.feature`)
- **Step definitions**: Located in `StepDefinitions/` (e.g. `StepDefinitions/Api/StatusEndpoints/PingSteps.cs`)
- **AspireHooks**: A `[Binding]` class using `[BeforeTestRun]`/`[AfterTestRun]` hooks to start and stop the Aspire app once per test run. Provides `CreateHttpClient(serviceName, ipAddress)` (with resilience handler) and `CreateRawHttpClient(serviceName, ipAddress)` (plain HttpClient, used by rate limiter tests to avoid retry on 429). Also provides `GenerateRandomIp()` to create unique IPs for test isolation via `X-Forwarded-For`. Both HTTP client methods automatically attach a fake JWT Bearer token (`Authorization: Bearer <token>`) generated from a minimal base64url-encoded header and payload — valid JWT format, no real signature, accepted because `ValidateUser=false` in the test AppHost
- **Rate limiter test isolation**: Each scenario gets a unique random IP via `AspireHooks.GenerateRandomIp()`, sent as `X-Forwarded-For` header. This ensures each scenario has its own rate limiter partition, isolated from health checks and other tests. Rate limiter tests use `CreateRawHttpClient` (HTTPS endpoint) to avoid both the resilience handler retrying 429s and the HTTP→HTTPS redirect consuming double permits
- **Async polling pattern**: For command-driven flows where the API returns `202 Accepted` and the Bus processes asynchronously, step definitions poll the GET endpoint in a loop with a short delay (`PollInterval = 500ms`) until it returns `200 OK`. A fresh `AspireHooks.GenerateRandomIp()` is generated **per request** inside the loop to avoid exhausting the per-IP rate limiter (5 req/min in tests). A `CancellationTokenSource(DefaultTimeout)` bounds the total wait.
- **Project references**: `IntegrationTests.csproj` references `Api.Contracts` to use request/response types (e.g. `CreateCurriculumVitaeRequest`, `CreateCurriculumVitaeResponse`) directly in step definitions
- **`*.feature.cs` gitignored**: Reqnroll auto-generates code-behind files alongside `.feature` files on build. These are excluded from source control via `.gitignore`. They are regenerated automatically on build.
- **Global usings**: Defined in `GlobalUsings.cs` (not in csproj)

### End-to-End Tests

The `Controllers.EndToEndTests` project uses Playwright with Reqnroll (BDD) and Aspire.Hosting.Testing for browser-based E2E tests. Key patterns:

- **Reqnroll + Playwright + Shouldly**: Tests are Gherkin feature files with step definitions that drive Playwright browser interactions. Assertions use [Shouldly](https://docs.shouldly.org/)
- **Feature files**: Located in `Features/` (e.g. `Features/WebAdministrators/Health.feature`, `Features/WebUsers/Health.feature`)
- **Step definitions**: Located in `StepDefinitions/` (e.g. `StepDefinitions/WebAdministrators/HealthSteps.cs`)
- **AspireHooks**: A `[Binding]` class using `[BeforeTestRun]`/`[AfterTestRun]` hooks to start the Aspire app and Playwright browser once per test run. Creates browser contexts with `IgnoreHTTPSErrors = true` (required because Chromium doesn't trust the ASP.NET Core dev certificate)
- **CI setup**: The `build.yml` workflow installs Playwright browsers via `playwright install chromium`

### Architecture Tests

The `ArchitectureTests` project uses [ArchUnitNET](https://github.com/TNG/ArchUnitNET) with Reqnroll (BDD) to enforce dependency rules:
- **Domain** can only depend on Frameworks
- **Infrastructure** (Postgresql, RabbitMQ) cannot depend on Functions, Api Contracts, or Controllers

Tests are written as Gherkin feature files (`Features/DomainDependencies.feature`, `Features/InfrastructureDependencies.feature`) with step definitions using `[Binding]` and `[Scope(Feature = "...")]` attributes. Each project exposes an `IMarker` interface (e.g. `IDomainMarker`, `IApiMarker`, `IApiContractsMarker`) used by tests to reference assemblies. Step definition classes inherit from `BaseArchitectureTests`, which loads all assemblies and defines layer providers.

### Containerization

The web projects (`Controllers.Web.Administrators` and `Controllers.Web.Users`) include Dockerfiles that build the Blazor WASM apps and serve them via Nginx.

### Helm Chart (Kubernetes Deployment)

The Helm chart is located at `deployment/umiko/` and deploys all application services to Kubernetes.

**Templates** (`deployment/umiko/templates/`):
- `namespace.yaml`: Creates the target namespace
- `application-database.yaml`: PostgreSQL StatefulSet with persistent storage, Secret for credentials, and ClusterIP Service (used by API and Bus)
- `authentication-database.yaml`: Dedicated PostgreSQL StatefulSet for Keycloak with persistent storage, Secret for credentials, and ClusterIP Service (uses `keycloak.postgres.*` values)
- `authenticatie-server.yml`: Keycloak StatefulSet (LoadBalancer Service + headless discovery Service), Secret for DB credentials, JGroups clustering via `keycloak-discovery` DNS
- `pgadmin.yaml`: pgAdmin Deployment and ClusterIP Service for database management, pre-configured with both the application database and authentication database as server entries
- `rabbitmq.yaml`: RabbitMQ StatefulSet with persistent storage, Secret for credentials, and ClusterIP Service
- `api.yaml`: REST API Deployment and ClusterIP Service, with `ConnectionStrings__UmikoDb` and conditional `OTEL_EXPORTER_OTLP_ENDPOINT` env vars
- `bus.yaml`: Message consumer Deployment and ClusterIP Service, with `ConnectionStrings__UmikoDb` and conditional `OTEL_EXPORTER_OTLP_ENDPOINT` env vars
- `web-administrators.yaml`: Admin portal Deployment and ClusterIP Service
- `web-users.yaml`: User portal Deployment and ClusterIP Service
- `ingress.yaml`: Traefik Ingress with subdomain-based routing and TLS, conditional Grafana route
- `cluster-issuer.yaml`: Optional Let's Encrypt ClusterIssuer for cert-manager (enabled via `clusterIssuer.enabled`)
- `otel-collector.yaml`: OpenTelemetry Collector Deployment (OTLP receiver, k8s_cluster metrics, k8s_events), DaemonSet for pod log collection via filelog receiver with `log_source = "k8s_pods"` resource attribute, ServiceAccount + RBAC (gated by `observability.enabled`)
- `prometheus.yaml`: Prometheus StatefulSet with remote write receiver (gated by `observability.enabled`)
- `tempo.yaml`: Grafana Tempo StatefulSet for trace storage (gated by `observability.enabled`)
- `loki.yaml`: Grafana Loki StatefulSet for log storage with OTLP ingestion (gated by `observability.enabled`)
- `grafana.yaml`: Grafana Deployment with auto-provisioned datasources and ConfigMap-based dashboard provisioning from `dashboards/` directory (gated by `observability.enabled`)
- `NOTES.txt`: Helm post-install notes with application URLs, health checks, backing services, and observability endpoints

**Ingress routing** (subdomain-based via Traefik):
- `<domain>` → web-users
- `admin.<domain>` → web-administrators
- `api.<domain>` → api
- `bus.<domain>` → bus
- `authentication.<domain>` → keycloak
- `grafana.<domain>` → grafana (when `observability.enabled`)

**Values files**:
- `values.yaml`: Base defaults (image names, ports, ingress class/domain/TLS secret, observability component configs)
- `values-development.yaml`: Development overrides (namespace `umiko-development`, `appTag`, domain `umiko.dev`, observability enabled)
- `values-production.yaml`: Production overrides (namespace `umiko-production`, `appTag`, domain `umiko.example.com`, observability enabled with larger storage)
- **`appTag`**: Single image tag used by all four application services (api, bus, web-users, web-administrators), set in environment values files

**TLS**:
- **Development**: Uses mkcert for locally-trusted certificates
- **Production**: Uses cert-manager with a Let's Encrypt ClusterIssuer (HTTP-01 solver via Traefik)
- The TLS secret name is configured via `ingress.tlsSecret` in values

**Health checks**: API and Bus use `/health`, web apps use `/health.txt`

### Observability Stack (Helm)

Gated behind `observability.enabled` (default `false`). When enabled, deploys a full OpenTelemetry-based observability stack:

| Component | Kind | Purpose |
|-----------|------|---------|
| OTel Collector | Deployment | Receives OTLP from API/Bus, collects k8s_cluster metrics and k8s_events, routes to backends |
| OTel Collector Logs | DaemonSet | Collects container stdout/stderr logs from `/var/log/pods` via filelog receiver |
| Prometheus | StatefulSet | Metrics storage (receives via remote write) |
| Tempo | StatefulSet | Trace storage (receives via OTLP HTTP) |
| Loki | StatefulSet | Log storage (receives via OTLP HTTP) |
| Grafana | Deployment | Visualization UI with auto-provisioned datasources and dashboard provisioning |

**Data flow**:
- API/Bus → OTLP/gRPC:4317 → OTel Collector → Tempo (traces), Prometheus (metrics), Loki (logs)
- All pods → stdout/stderr → node filesystem → OTel Collector Logs DaemonSet → Loki
- Kubernetes → k8s_cluster receiver → Prometheus (pod/node metrics)
- Kubernetes → k8s_events receiver → Loki (cluster events)

**Log source labelling**: Logs can be distinguished by the `log_source` Loki label:
- `log_source = "application"` — structured logs sent directly from the .NET apps via OTLP
- `log_source = "k8s_pods"` — raw stdout/stderr captured from all pods by the DaemonSet
- Loki is configured to index `log_source` as a stream label via `limits_config.otlp_config.resource_attributes.attributes_config`

**RBAC**: The OTel Collector uses a ServiceAccount with ClusterRole permissions to read pods, nodes, deployments, statefulsets, events, resourcequotas, and horizontalpodautoscalers.

**Grafana Dashboards**: Dashboard JSON files in `deployment/umiko/dashboards/` are automatically loaded into Grafana via ConfigMap-based provisioning. To add a dashboard: export it from Grafana UI, save the JSON file to the `dashboards/` directory, and redeploy with Helm. The logging dashboard (`logging.json`) includes `Application`, `Log Level`, and `Log Source` filter variables.

### Database Migrations

EF Core migrations use `UmikoContext` in the `Infrastructures.Postgresql` project. Commands should be run from the `source/MadWorldNL.Umiko.Controllers.Api` directory. See `documentation/database.md` for full details.

### CI/CD

GitHub Actions workflows in `.github/workflows/`:

- **`build.yml`** (Build & Test Application): Runs on push/PR to `main` — installs dev certificates (linux-dev-certs), restores, builds (Release), and tests the solution
- **`publish-containers.yml`**: Triggered on git tags (`v*`) — publishes multi-arch container images (x64/arm64) for Api, Bus, Web.Administrators, and Web.Users to GitHub Container Registry
- **`sonarqube.yml`** (SonarQube): Runs on push/PR to `main` — builds and analyzes code with SonarQube Cloud on Windows runner
- **`claude-code-review.yml`**: Automated code review on pull requests using Claude Code
- **`claude.yml`**: Responds to `@claude` mentions in comments, PR reviews, and issues

### Community Files

- `CONTRIBUTING.md`: Contribution guidelines
- `SECURITY.md`: Security vulnerability reporting policy
- `CODE_OF_CONDUCT.md`: Code of conduct
- `.github/pull_request_template.md`: PR template
- `.github/ISSUE_TEMPLATE/`: Bug report and feature request templates
- `documentation/`: Developer guides:
  - `documentation/authentication-server.md`: Keycloak setup for local development, including audience attribute configuration for JWT validation
  - `documentation/kubernetes.md`: Local (Docker Desktop) and production (MicroK8s) Kubernetes setup, Traefik, TLS, Helm deploy
  - `documentation/database.md`: EF Core migration commands using `UmikoContext` and `Infrastructures.Postgresql`
  - `documentation/dns.md`: DNS A record configuration for production domains
  - `documentation/setup-server.md`: Server setup guide (prerequisite for production deployment)
  - `documentation/ubuntu-dev-environment.md`: Ubuntu development environment setup
  - `documentation/versions.md`: Version tracking