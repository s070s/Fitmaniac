# Fitmaniac

Fitmaniac is a multi-project fitness platform built on .NET 10. The repository contains a REST API, a Blazor web client, a .NET MAUI mobile client, and the supporting application, domain, infrastructure, and shared libraries behind them.

The current solution is organized as a cleanly separated backend plus two frontends:

- `src/Fitmaniac.Api`: ASP.NET Core minimal API with SignalR, JWT auth, EF Core, SQL Server, automatic migrations, and seeders.
- `src/Fitmaniac.Web`: Blazor Server web app that authenticates with cookies and calls the API through typed HTTP consumers.
- `mobile/Fitmaniac.MAUI`: .NET MAUI client for mobile/desktop scenarios using bearer tokens against the same API.
- `src/Fitmaniac.Application`: service contracts, abstractions, mapping, and common application models.
- `src/Fitmaniac.Domain`: entities, enums, and core domain rules.
- `src/Fitmaniac.Infrastructure`: EF Core persistence, Identity, JWT wiring, seeders, and service implementations.
- `src/Fitmaniac.Shared`: DTOs, constants, resources, and embedded JSON seed data shared across clients.

## Solution Layout

```text
Fitmaniac/
|- Fitmaniac.slnx
|- Directory.Build.props
|- global.json
|- SCAFFOLDING_PLAN.md
|- mobile/
|  \- Fitmaniac.MAUI/
\- src/
   |- Fitmaniac.Api/
   |- Fitmaniac.Application/
   |- Fitmaniac.Domain/
   |- Fitmaniac.Infrastructure/
   |- Fitmaniac.Shared/
   \- Fitmaniac.Web/
```

`Fitmaniac.slnx` includes all seven projects. `Directory.Build.props` sets the common target framework to `net10.0`, enables nullable reference types, and turns on implicit usings.

## Current Feature Surface

The implementation currently includes these main areas:

- Authentication and account flows: register, login, refresh, logout, forgot password, reset password, change password.
- User and profile data.
- Trainer discovery and client subscription to trainers.
- Workouts and workout details.
- Weekly program access.
- Goals, measurements, and progress summaries.
- Subscription and billing endpoints.
- Real-time and REST-backed chat via SignalR.
- Admin user and trainer management pages in the web app.

The web app currently exposes pages such as dashboard, workouts, workout detail, trainers, goals, measurements, progress, subscription, profile, chat, admin users, and admin trainers. The MAUI app currently includes login, registration, home, workouts, workout detail, progress, and profile screens.

## Tech Stack

- .NET SDK `10.0.100` via `global.json`
- ASP.NET Core minimal APIs
- Blazor Server
- .NET MAUI
- Entity Framework Core 10 with SQL Server
- ASP.NET Core Identity with integer keys
- JWT bearer authentication
- SignalR
- Swagger / OpenAPI in development

## Prerequisites

To run the repository locally, install:

- .NET SDK `10.0.100`
- SQL Server LocalDB or a reachable SQL Server instance
- MAUI workloads if you want to build or run the mobile app

On Windows, install MAUI workloads with:

```powershell
dotnet workload install maui
```

## Configuration

### API

The API uses these defaults:

- Connection string: `Server=(localdb)\MSSQLLocalDB;Database=FitmaniacDb;Trusted_Connection=True;TrustServerCertificate=True;`
- HTTPS URL: `https://localhost:7301`
- HTTP URL: `http://localhost:5301`

Development settings in `src/Fitmaniac.Api/appsettings.Development.json` currently provide:

- `Jwt:Key`: `dev-secret-key-must-be-at-least-32-characters-long!`
- Seeded admin password: `Admin123!`
- Sample data enabled: `true`

The seeded admin user is configured as:

- Email: `admin@fitmaniac.local`
- Username: `admin`
- Password: `Admin123!`

For non-development environments, set a real JWT signing key and admin seed credentials before startup.

### Web

The Blazor web app defaults to:

- `ApiBaseUrl`: `https://localhost:7301`
- `SelfBaseUrl`: `https://localhost:7090`

It uses cookie auth for the browser and forwards authenticated API requests through typed consumers and an SSR handler.

### MAUI

The MAUI app defaults to:

- `ApiBaseUrl`: `https://10.0.2.2:7301`

`10.0.2.2` is the Android emulator loopback address for the host machine. If you run the app on Windows, iOS, a physical device, or another emulator, update `mobile/Fitmaniac.MAUI/appsettings.json` to an address reachable from that target.

## Running The Solution

Start the API first, then the web app or MAUI client.

### 1. Restore and build

```powershell
dotnet restore Fitmaniac.slnx
dotnet build Fitmaniac.slnx
```

### 2. Run the API

```powershell
dotnet run --project src/Fitmaniac.Api
```

What happens on startup:

- EF Core migrations are applied automatically.
- Database seeders run automatically.
- Swagger UI is available in development.
- SignalR chat hub is hosted at `/hubs/chat`.

Useful development URLs:

- API root: `https://localhost:7301/`
- Swagger UI: `https://localhost:7301/swagger`

### 3. Run the web app

```powershell
dotnet run --project src/Fitmaniac.Web
```

Then open the configured web URL for that project in your browser.

### 4. Run the MAUI app

Build or run the MAUI project from Visual Studio, or use the CLI for a specific target framework.

Example for Windows:

```powershell
dotnet build mobile/Fitmaniac.MAUI/Fitmaniac.MAUI.csproj -f net10.0-windows10.0.19041
```

Example for Android:

```powershell
dotnet build mobile/Fitmaniac.MAUI/Fitmaniac.MAUI.csproj -f net10.0-android
```

## Authentication Model

The solution uses different auth flows per client:

- API: JWT bearer authentication.
- Web: cookie auth in the browser, then server-side calls to the API through typed consumers.
- MAUI: access tokens stored client-side and attached through `AuthHttpHandler`.

The API also supports SignalR authentication for `/hubs/chat` by accepting bearer tokens in the `access_token` query string for hub connections.

## Development Notes

- The API applies migrations on startup, so the configured SQL Server must be reachable before launch.
- CORS allowed origins currently include `http://localhost:5173` and `https://localhost:7090`.
- Swagger and OpenAPI are only mapped in development.
- The repository includes `SCAFFOLDING_PLAN.md`, which describes the broader target architecture. The implemented codebase currently represents a subset of that plan.

## Project Roles

### `Fitmaniac.Domain`

Contains domain entities such as users, trainers, clients, workouts, programs, goals, measurements, chat conversations, and subscriptions.

### `Fitmaniac.Application`

Contains service interfaces, shared result models, pagination helpers, abstractions, and mapping contracts.

### `Fitmaniac.Infrastructure`

Contains EF Core persistence, Identity configuration, JWT configuration, seeders, storage services, validation, and the concrete application service implementations.

### `Fitmaniac.Shared`

Contains DTOs, constants, localization resources, and embedded JSON data used by the rest of the solution.

### `Fitmaniac.Api`

Hosts the REST API, configures CORS, authentication, authorization policies, rate limiting, Swagger, static files, automatic migrations, seeders, and the SignalR chat hub.

### `Fitmaniac.Web`

Hosts the Blazor Server UI and calls the API through typed consumers. It uses ASP.NET Core cookie authentication and SSR-friendly API forwarding.

### `Fitmaniac.MAUI`

Hosts the MAUI client application and communicates with the API through a typed API client plus an auth delegating handler.

## Useful Commands

```powershell
dotnet restore Fitmaniac.slnx
dotnet build Fitmaniac.slnx
dotnet run --project src/Fitmaniac.Api
dotnet run --project src/Fitmaniac.Web
dotnet ef migrations list --project src/Fitmaniac.Infrastructure --startup-project src/Fitmaniac.Api
```

## Status

This repository already has a working multi-project foundation with implemented API endpoints, a Blazor web client, and a MAUI client. It also contains a more ambitious scaffold plan for additional pages, services, and platform polish that have not all been implemented yet.