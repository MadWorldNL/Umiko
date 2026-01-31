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
- **Application.Domain**: Domain entities and business rules
- **Infrastructures.Postgresql**: PostgreSQL data access, depends on Domain
- **Infrastructures.RabbitMQ**: RabbitMQ messaging integration, depends on Domain

### Key Configuration

- **Central package versions**: All NuGet versions managed in `source/Directory.Packages.props`
- **Build settings**: Strict mode enabled in `source/Directory.Build.props` (warnings as errors, nullable enabled)
- **Root namespace**: `MadWorldNL.Umiko`

### Development URLs (when running individual projects)

- API: `https://localhost:7115` or `http://localhost:5106`
- Admin Portal: `https://localhost:7209` or `http://localhost:5214`

### Solution File

Uses the newer `.slnx` format: `source/MadWorldNL.Umiko.slnx`

### OpenTelemetry

Both `Controllers.Api` and `Controllers.Bus` include OpenTelemetry instrumentation configured via a `Configurations/OpenTelemetryExtensions.cs` extension method. This sets up:
- Logging export with formatted messages and scopes
- ASP.NET Core and Kestrel metrics
- ASP.NET Core and HTTP client tracing
- OTLP export when `OTEL_EXPORTER_OTLP_ENDPOINT` is configured

### Architecture Tests

The `ArchitectureTests` project uses [ArchUnitNET](https://github.com/TNG/ArchUnitNET) (xUnit) to enforce dependency rules:
- **Domain** cannot depend on any other project
- **Infrastructure** (Postgresql, RabbitMQ) cannot depend on Functions, Api Contracts, or Controllers

Each project exposes an `IMarker` interface (e.g. `IDomainMarker`, `IApiMarker`, `IApiContractsMarker`) used by tests to reference assemblies. Test classes inherit from `BaseArchitectureTests`, which loads all assemblies and defines layer providers.

### CI/CD

GitHub Actions workflow (`.github/workflows/build.yml`) runs on push/PR to `main`:
- Restores, builds (Release), and tests the solution

### Community Files

- `CONTRIBUTING.md`: Contribution guidelines
- `SECURITY.md`: Security vulnerability reporting policy
- `CODE_OF_CONDUCT.md`: Code of conduct
- `.github/pull_request_template.md`: PR template
- `.github/ISSUE_TEMPLATE/`: Bug report and feature request templates
- `documentation/`: Developer guides (e.g. Ubuntu dev environment setup)