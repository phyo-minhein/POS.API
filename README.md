# POS — Point of Sale

A portfolio point-of-sale application built with ASP.NET Core, Entity Framework Core, SQL Server, and Blazor. The project is being developed as end-to-end vertical slices so that each business feature includes its API behavior and user interface.

## Current status

The repository currently includes:

- Category, product, and sale API controllers
- Entity Framework Core database-first models for four domain tables
- Service classes and asynchronous database operations
- Swagger/OpenAPI documentation
- A Blazor Web App using Interactive Server components
- A POS navigation shell with dashboard, checkout, products, categories, sales history, and login routes

Authentication, inventory-aware checkout, reporting, logging, automated tests, and the completed feature pages are planned work.

## Technology

- .NET 10
- ASP.NET Core Web API
- Blazor Web App with Interactive Server components
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- Bootstrap

The repository pins the .NET SDK through `global.json` for consistent local and CI builds.

## Repository structure

```text
POS/
├── POS.API/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   ├── ViewModels/
│   └── Program.cs
├── POS.Frontend/
│   └── POS.Frontend/
│       ├── Clients/
│       ├── Components/
│       │   ├── Layout/
│       │   └── Pages/
│       ├── Models/
│       └── Program.cs
├── POS.slnx
└── global.json
```

The frontend is organized for a typed-client structure: Razor components will depend on focused client classes, and those clients will communicate with the API through `HttpClient`.

## Database model

The existing database-first domain contains:

- `Category`
- `Product`
- `Sale`
- `SaleDetail`

`Sale` and `SaleDetail` represent an invoice header and its line items. `Product` belongs to an optional category and can appear in sale details.

## Prerequisites

Install or provide:

- The .NET 10 SDK specified by `global.json`
- SQL Server
- A local `POS` database containing the current domain tables
- Visual Studio with .NET 10 tooling, or a terminal/editor capable of running .NET projects

Check the selected SDK:

```powershell
dotnet --version
```

## Local configuration

Files named `appsettings.json` are intentionally ignored by Git. Create `POS.API/appsettings.json` locally with the following structure:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=POS;User Id=<YOUR_USER>;Password=<YOUR_PASSWORD>;TrustServerCertificate=True;"
  }
}
```

Replace the placeholders with local development credentials. Never commit real passwords, tokens, or production connection strings.

The frontend currently requires no private configuration. API base-address configuration will be introduced with the first real typed feature client.

## Build

From the repository root:

```powershell
dotnet restore POS.slnx
dotnet build POS.slnx
```

The expected result is a successful build with no warnings or errors.

## Run locally

Start the API in one terminal:

```powershell
dotnet run --project .\POS.API\POS.API.csproj --launch-profile http
```

Start the frontend in another terminal:

```powershell
dotnet run --project .\POS.Frontend\POS.Frontend\POS.Frontend.csproj --launch-profile http
```

Development addresses:

| Application | Address |
| --- | --- |
| Blazor frontend | `http://localhost:5140` |
| API | `http://localhost:5120` |
| Swagger UI | `http://localhost:5120/swagger` |

Both applications must be running when a frontend feature communicates with the API.

## Current API areas

The current API exposes CRUD-style endpoints under:

- `/api/categories`
- `/api/product`
- `/api/sale`

Swagger displays the exact operations and request/response schemas available in the current build.

The target contract for the Category vertical slice is documented in [`docs/api/categories.md`](docs/api/categories.md). It is implemented incrementally and may be ahead of the current controller while the slice is in progress.

## Development approach

Features are implemented as vertical slices:

1. Define the user story and API contract.
2. Implement database and API behavior.
3. Verify the API and important failure cases.
4. Add a typed frontend client.
5. Build the Blazor workflow.
6. Test the complete browser-to-database path.

EF Core already provides repository and unit-of-work behavior, so the project avoids a generic repository layer unless a concrete requirement justifies one.

## Planned features

- Correct API status codes and consistent error responses
- Input validation and duplicate prevention
- Category and product management UI
- Product search, filtering, and pagination
- ASP.NET Core Identity authentication
- Admin and Cashier authorization
- Inventory-aware transactional checkout
- Sales history and printable invoice details
- Reporting dashboard
- Structured logging and centralized exception handling
- Automated API and Blazor component tests
- CI and reproducible deployment

## Security

- Keep local settings and secrets out of source control.
- Do not log passwords, access tokens, or connection strings.
- Validate authorization in the API, not only in the UI.
- Rotate any credential that is accidentally exposed before publishing or deploying the application.

## Project stage

This project is under active development and is not yet intended for production use.
