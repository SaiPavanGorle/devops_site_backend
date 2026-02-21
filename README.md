# DevOps Release Portal API (.NET 8)

ASP.NET Core Web API for a **DevOps Release Portal** with:

- ASP.NET Core Identity (email/password) backed by EF Core + SQL Server.
- JWT authentication for SPA clients.
- Role-based authorization with roles: `Developer`, `DevOps`, `Tester`, `Manager`, `BA`.
- Swagger enabled in Development.
- Serilog logging to console + rolling files in `Logs/`.
- CORS with allowed frontend origin from configuration.
- Health endpoint: `GET /health` => `{ "status": "ok" }`.

## Project structure

- `Controllers/` - API endpoints.
- `Data/` - `ApplicationDbContext`, Identity user model, and startup seeding.
- `Auth/` - JWT configuration + DTOs.
- `Services/` - JWT token generation service.
- `Program.cs` - central startup and middleware setup.

## Prerequisites

- .NET 8 SDK
- SQL Server (or SQL Server Express / LocalDB)

## Configuration

Update `appsettings.json` (or environment-specific config):

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key` (use a long, random secret in production)
- `Jwt:Issuer`
- `Jwt:Audience`
- `Cors:AllowedOrigin` (default: `http://localhost:5173`)

## EF Core migrations setup

From repository root:

```bash
# Install EF CLI (once)
dotnet tool install --global dotnet-ef

# Add initial migration
dotnet ef migrations add InitialIdentitySetup

# Apply migration
dotnet ef database update
```

If multiple projects/solutions are used later, include `--project` and `--startup-project` arguments.

## Run locally

```bash
dotnet restore
dotnet build
dotnet run
```

In Development, Swagger is available at `/swagger`.

## Authentication flow (SPA)

1. Call `POST /api/auth/login` with JSON:
   ```json
   {
     "email": "devops@example.com",
     "password": "DevOps@123"
   }
   ```
2. Receive JWT token.
3. Send token in `Authorization: Bearer <token>` for protected endpoints.

## Roles and seeding

### Always seeded on startup
- Roles: `Developer`, `DevOps`, `Tester`, `Manager`, `BA`

### Seeded only in Development
- `devops@example.com / DevOps@123` with role `DevOps`
- `dev@example.com / Dev@123` with role `Developer`

### Disable development user seeding for production

Set this config value to false (e.g., in `appsettings.Production.json` or env vars):

```json
{
  "Seed": {
    "EnableDevelopmentUsers": false
  }
}
```

Production also skips these users automatically unless environment is `Development`.

## IIS publish (Windows Server)

1. Install **.NET 8 Hosting Bundle** on the IIS server.
2. Publish application:

   ```bash
   dotnet publish -c Release -o ./publish
   ```

3. In IIS:
   - Create a new Site (or Application) pointing to `publish/`.
   - Use an app pool with **No Managed Code**.
   - Ensure the app pool identity has write permission to `Logs/` folder.
4. Set production configuration:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ConnectionStrings__DefaultConnection=...`
   - `Jwt__Key=...`
   - `Cors__AllowedOrigin=...`
5. Run DB migration on deployment target:

   ```bash
   dotnet ef database update
   ```

6. Verify:
   - `GET /health` returns `{ "status": "ok" }`.
   - Login endpoint issues JWT.

## Logging

Serilog writes:
- Console logs.
- Rolling daily logs: `Logs/log-YYYYMMDD.txt`.

Rotate/retention is configured in code (`retainedFileCountLimit: 14`).
