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

### Layer Responsibilities

- **Aspire Host**: Orchestrates all services and infrastructure for local development
- **Controllers.Api**: REST API with OpenAPI support and OpenTelemetry instrumentation, handles HTTP requests
- **Controllers.Api.Contracts**: API contract definitions (DTOs/request-response models)
- **Controllers.Bus**: Background message processing service with OpenTelemetry instrumentation, consumes messages from RabbitMQ
- **Controllers.Web.Administrators/Users**: Two separate Blazor WebAssembly client apps (client-side rendering)
- **Application.Functions**: Business logic layer, shared by API and Bus
- **Application.Frameworks**: DDD building blocks (Entity, AggregateRoot, ValueObject, IDomainEvent) — no dependencies, foundational layer
- **Application.Domain**: Domain entities and business rules, depends on Frameworks
- **Infrastructures.Postgresql**: PostgreSQL data access, depends on Domain
- **Infrastructures.RabbitMQ**: RabbitMQ messaging integration, depends on Domain

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

### Health Checks

- **API & Bus**: Use the built-in ASP.NET Core health checks middleware (`builder.Services.AddHealthChecks()` + `app.MapHealthChecks("/health")`), exposed at `/health`. All registered health checks (including the Aspire PostgreSQL check) are excluded via `Predicate = _ => false` to avoid database queries on every health probe — the endpoint returns `Healthy` without executing any checks. Database connectivity is available separately via the `/Status/Database` endpoint.
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

Both `Controllers.Api` and `Controllers.Bus` use [Scalar](https://github.com/scalar/scalar) (`Scalar.AspNetCore`) to render interactive API reference documentation from OpenAPI specs. Available in development mode at `/scalar/v1`.

### Test AppHost (Aspire.Tests)

The `MadWorldNL.Umiko.Aspire.Tests` project is a simplified Aspire AppHost used by both Integration and E2E tests. Unlike the main AppHost, it excludes Keycloak and uses default credentials (no secret parameters) for PostgreSQL and RabbitMQ. It overrides `RateLimiter__PermitLimit` to `5` (instead of production default 100) for faster rate limiter integration tests. Tests reference it via `DistributedApplicationTestingBuilder.CreateAsync<Projects.Aspire_Tests>()`.

### Integration Tests

The `Controllers.IntegrationTests` project uses Reqnroll (BDD) with Aspire.Hosting.Testing to run integration tests against the full distributed application. Key patterns:

- **Reqnroll + xUnit**: Tests are written as Gherkin feature files (`.feature`) with C# step definitions using `[Binding]` and `[Scope(Feature = "...")]` attributes
- **Feature files**: Located in `Features/` (e.g. `Features/Api/StatusEndpoints/Ping.feature`)
- **Step definitions**: Located in `StepDefinitions/` (e.g. `StepDefinitions/Api/StatusEndpoints/PingSteps.cs`)
- **AspireHooks**: A `[Binding]` class using `[BeforeTestRun]`/`[AfterTestRun]` hooks to start and stop the Aspire app once per test run. Provides `CreateHttpClient(serviceName, ipAddress)` (with resilience handler) and `CreateRawHttpClient(serviceName, ipAddress)` (plain HttpClient, used by rate limiter tests to avoid retry on 429). Also provides `GenerateRandomIp()` to create unique IPs for test isolation via `X-Forwarded-For`
- **Rate limiter test isolation**: Each scenario gets a unique random IP via `AspireHooks.GenerateRandomIp()`, sent as `X-Forwarded-For` header. This ensures each scenario has its own rate limiter partition, isolated from health checks and other tests. Rate limiter tests use `CreateRawHttpClient` (HTTPS endpoint) to avoid both the resilience handler retrying 429s and the HTTP→HTTPS redirect consuming double permits
- **Global usings**: Defined in `GlobalUsings.cs` (not in csproj)

### End-to-End Tests

The `Controllers.EndToEndTests` project uses Playwright with Reqnroll (BDD) and Aspire.Hosting.Testing for browser-based E2E tests. Key patterns:

- **Reqnroll + Playwright**: Tests are Gherkin feature files with step definitions that drive Playwright browser interactions
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
- `postgres.yaml`: PostgreSQL StatefulSet with persistent storage, Secret for credentials, and ClusterIP Service
- `rabbitmq.yaml`: RabbitMQ StatefulSet with persistent storage, Secret for credentials, and ClusterIP Service
- `api.yaml`: REST API Deployment and ClusterIP Service, with `ConnectionStrings__UmikoDb` and conditional `OTEL_EXPORTER_OTLP_ENDPOINT` env vars
- `bus.yaml`: Message consumer Deployment and ClusterIP Service, with `ConnectionStrings__UmikoDb` and conditional `OTEL_EXPORTER_OTLP_ENDPOINT` env vars
- `web-administrators.yaml`: Admin portal Deployment and ClusterIP Service
- `web-users.yaml`: User portal Deployment and ClusterIP Service
- `ingress.yaml`: Traefik Ingress with subdomain-based routing and TLS, conditional Grafana route
- `cluster-issuer.yaml`: Optional Let's Encrypt ClusterIssuer for cert-manager (enabled via `clusterIssuer.enabled`)
- `otel-collector.yaml`: OpenTelemetry Collector Deployment (OTLP receiver, k8s_cluster metrics, k8s_events), DaemonSet for pod log collection via filelog receiver, ServiceAccount + RBAC (gated by `observability.enabled`)
- `prometheus.yaml`: Prometheus StatefulSet with remote write receiver (gated by `observability.enabled`)
- `tempo.yaml`: Grafana Tempo StatefulSet for trace storage (gated by `observability.enabled`)
- `loki.yaml`: Grafana Loki StatefulSet for log storage with OTLP ingestion (gated by `observability.enabled`)
- `grafana.yaml`: Grafana Deployment with auto-provisioned datasources for Prometheus, Tempo, and Loki (gated by `observability.enabled`)

**Ingress routing** (subdomain-based via Traefik):
- `<domain>` → web-users
- `admin.<domain>` → web-administrators
- `api.<domain>` → api
- `bus.<domain>` → bus
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

**RBAC**: The OTel Collector uses a ServiceAccount with ClusterRole permissions to read pods, nodes, deployments, statefulsets, events, resourcequotas, and horizontalpodautoscalers.

**Grafana Dashboards**: Dashboard JSON files in `deployment/umiko/dashboards/` are automatically loaded into Grafana via ConfigMap-based provisioning. To add a dashboard: export it from Grafana UI, save the JSON file to the `dashboards/` directory, and redeploy with Helm.

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
  - `documentation/kubernetes.md`: Local (Docker Desktop) and production (MicroK8s) Kubernetes setup, Traefik, TLS, Helm deploy
  - `documentation/database.md`: EF Core migration commands using `UmikoContext` and `Infrastructures.Postgresql`
  - `documentation/dns.md`: DNS A record configuration for production domains
  - `documentation/setup-server.md`: Server setup guide (prerequisite for production deployment)