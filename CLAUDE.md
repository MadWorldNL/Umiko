# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Build entire solution
dotnet build source/MadWorldNL.Umiko.slnx

# Run the full application (starts all services via Aspire)
dotnet run --project source/MadWorldNL.Umiko.Aspire

# Run individual projects
dotnet run --project source/MadWorldNL.Umiko.Controllers.Api
dotnet run --project source/MadWorldNL.Umiko.Controllers.Web.Administrators
dotnet run --project source/MadWorldNL.Umiko.Controllers.Web.Users
```

## Architecture

Umiko is a .NET 10.0 distributed web application using Microsoft Aspire for orchestration.

### Project Dependency Graph

```
Aspire Host (Orchestrator)
├── Controllers.Api (REST API)
│   └── Application.Functions (Business Logic)
│       └── Application.Domain (Entities)
├── Controllers.Web.Administrators (Blazor WASM - Admin Portal)
└── Controllers.Web.Users (Blazor WASM - User Portal)
```

### Layer Responsibilities

- **Aspire Host**: Orchestrates and runs all services together for local development
- **Controllers.Api**: REST API with OpenAPI support, handles HTTP requests
- **Controllers.Web.Administrators/Users**: Two separate Blazor WebAssembly client apps (client-side rendering)
- **Application.Functions**: Business logic layer, shared by API
- **Application.Domain**: Domain entities and business rules

### Key Configuration

- **Central package versions**: All NuGet versions managed in `source/Directory.Packages.props`
- **Build settings**: Strict mode enabled in `source/Directory.Build.props` (warnings as errors, nullable enabled)
- **Root namespace**: `MadWorldNL.Umiko`

### Development URLs (when running individual projects)

- API: `https://localhost:7115` or `http://localhost:5106`
- Admin Portal: `https://localhost:7209` or `http://localhost:5214`

### Solution File

Uses the newer `.slnx` format: `source/MadWorldNL.Umiko.slnx`