# Asset Management Application

A Blazor Web App (Server) for managing employees, assets, assignments, and a dashboard with reporting. Uses EF Core for CRUD and Dapper for reporting queries.

## Tech Stack
- ASP.NET Blazor (Server render mode)
- EF Core (SQL Server)
- Dapper (reports)
- ASP.NET Identity (single admin user)

## Prerequisites
- .NET 9 SDK
- SQL Server (Express OK)

## Setup
1. Update connection string in `AssetManagement.Web/appsettings.json` if needed.
2. Apply EF migrations:
   ```bash
   dotnet ef database update -s AssetManagement.Web/AssetManagement.Web.csproj -p AssetManagement.Infrastructure/AssetManagement.Infrastructure.csproj
   ```
3. Run the app:
   ```bash
   dotnet run --project AssetManagement.Web
   ```

## Admin Credentials
- Email: `admin@demo.local`
- Password: `Admin!12345`

## Demo Data
On first run, sample employees, assets, and one assignment are seeded.

## Features
- Employees: add/edit/delete/list
- Assets: add/edit/delete/list, filter by type/status/spare
- Assignments: assign available assets, return, history
- Dashboard: totals and assets by type (Dapper)
- Reports: warranty expiry list and CSV export at `/export/warranty?days=30`

## Project Structure
- `AssetManagement.Domain`: entities and enums
- `AssetManagement.Application`: service interfaces, shared models
- `AssetManagement.Infrastructure`: EF Core DbContext, services, Dapper queries
- `AssetManagement.Web`: Blazor UI, Identity, DI, endpoints

## Notes
- Single-user login only; no registration UI. Credentials configured in `appsettings.json` under `Admin`.
- Update the connection string for your SQL Server instance.
