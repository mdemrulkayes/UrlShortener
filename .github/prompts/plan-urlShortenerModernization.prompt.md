## Plan: URL Shortener — Full Modernization & Feature Expansion

**TL;DR**: Migrate from .NET 9 to .NET 10, restructure the API to Vertical Slice architecture using FastEndpoints, add ASP.NET Core Identity with JWT authentication to both API and Blazor Server frontend, migrate CI from Azure Pipelines to GitHub Actions, and add several new features (analytics, expiration, custom aliases, QR codes, URL management). All endpoints and pages will be protected, with a public redirect endpoint as the sole exception.

> **Note**: Your appsettings.json has a SQL Server–style connection string (`Server=localhost,1401;...`) but the project references `Npgsql.EntityFrameworkCore.PostgreSQL`. You'll need to update the connection string to PostgreSQL format (e.g., `Host=localhost;Port=5432;Database=urlshortener;Username=urlshortener;Password=urlshortener`).

---

### Step 1: Migrate to .NET 10

1. Update `TargetFramework` from `net9.0` to `net10.0` in all three `.csproj` files:
   - `src/API/UrlShortener.Api/UrlShortener.Api.csproj`
   - `src/WEB/UrlShortener/UrlShortener.csproj`
   - `tests/UrlShortener.UnitTest/UrlShortener.UnitTest.csproj`

2. Upgrade all NuGet packages to their .NET 10-compatible versions:
   - **API**: `Microsoft.AspNetCore.OpenApi`, `Microsoft.EntityFrameworkCore.*` → `10.0.x`, `FluentValidation.AspNetCore` → latest, `Scalar.AspNetCore` → latest, `Npgsql.EntityFrameworkCore.PostgreSQL` → `10.0.x`
   - **WEB**: `MudBlazor` → latest .NET 10-compatible version, `Refit` / `Refit.HttpClientFactory` → latest
   - **Tests**: `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `coverlet.collector`, `Shouldly` → latest

3. Add new packages needed for later steps:
   - **API**: `FastEndpoints`, `FastEndpoints.Security`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.AspNetCore.Authentication.JwtBearer`
   - **WEB**: `Microsoft.AspNetCore.Components.Authorization`, `Blazored.LocalStorage`

4. Review `Program.cs` (API) and `Program.cs` (WEB) for any breaking API changes in .NET 10 (e.g., `MapOpenApi`, `MapStaticAssets`, `ImportMap` — these should remain stable but verify after upgrade).

5. Remove Docker-related properties from `.csproj` files (`DockerDefaultTargetOS`, `DockerfileContext`, `DockerComposeProjectPath`) since Docker is not being used.

---

### Step 2: Feature Additions (New Capabilities)

Based on the current minimal feature set (shorten URL, redirect, list all URLs), here are the features to add:

| Feature                | Description                                                                                               |
| ---------------------- | --------------------------------------------------------------------------------------------------------- |
| **Click Analytics**    | Track click count, last accessed time, referrer, and user-agent per short URL. Add a `ClickEvent` entity. |
| **URL Expiration**     | Optional `ExpiresAt` field on `ShortenedUrl`. Expired URLs return 410 Gone.                               |
| **Custom Aliases**     | Allow users to specify a custom short code instead of auto-generating. Validate uniqueness.               |
| **QR Code Generation** | Generate QR code image for each shortened URL (use `QRCoder` NuGet package).                              |
| **URL Management**     | Edit (update long URL), delete, and activate/deactivate short URLs.                                       |
| **User-scoped URLs**   | Each URL is owned by the authenticated user. Users only see their own URLs.                               |
| **Dashboard Stats**    | Home page shows total URLs, total clicks, top 5 URLs by clicks for the logged-in user.                    |

**New/modified entities:**

- Extend `ShortenedUrl` in `Models/ShortenedUrl.cs`: add `CustomAlias?`, `ExpiresAt?`, `IsActive`, `ClickCount`, `UserId` (FK to Identity user)
- New entity `ClickEvent`: `Id`, `ShortenedUrlId`, `ClickedAt`, `Referrer?`, `UserAgent?`, `IpAddress?`

**Implementation will happen inside the Vertical Slice restructure (Step 4).**

---

### Step 3: Migrate Azure Pipelines to GitHub Actions

1. Create `.github/workflows/ci.yml` with:
   - Trigger on `push` and `pull_request` to `master`
   - Job: `build-and-test` on `ubuntu-latest`
   - Steps: checkout, setup .NET 10 SDK, restore, build (Release), run tests, publish artifacts

2. Delete `azure-pipelines.yml`

---

### Step 4: Restructure API to Vertical Slice Architecture with FastEndpoints

**Current flat structure:**

```
UrlShortener.Api/
  Db/, Dtos/, Models/, Services/, Program.cs, Utils.cs
```

**New Vertical Slice structure:**

```
UrlShortener.Api/
  Program.cs
  Common/
    Utils.cs
    BaseModels/
  Data/
    ApplicationDbContext.cs
    Configurations/
      ShortenedUrlConfiguration.cs
      ClickEventConfiguration.cs
    Migrations/
  Features/
    Auth/
      Register/
        RegisterEndpoint.cs
        RegisterRequest.cs
        RegisterResponse.cs
      Login/
        LoginEndpoint.cs
        LoginRequest.cs
        LoginResponse.cs
      CurrentUser/
        CurrentUserEndpoint.cs
    Urls/
      ShortenUrl/
        ShortenUrlEndpoint.cs
        ShortenUrlRequest.cs
        ShortenUrlResponse.cs
        ShortenUrlValidator.cs
      RedirectUrl/
        RedirectUrlEndpoint.cs
      ListUrls/
        ListUrlsEndpoint.cs
        ListUrlsResponse.cs
      GetUrl/
        GetUrlEndpoint.cs
        GetUrlResponse.cs
      UpdateUrl/
        UpdateUrlEndpoint.cs
        UpdateUrlRequest.cs
        UpdateUrlValidator.cs
      DeleteUrl/
        DeleteUrlEndpoint.cs
      ToggleUrl/
        ToggleUrlEndpoint.cs
      GetQrCode/
        GetQrCodeEndpoint.cs
    Analytics/
      GetUrlStats/
        GetUrlStatsEndpoint.cs
        GetUrlStatsResponse.cs
      GetDashboard/
        GetDashboardEndpoint.cs
        GetDashboardResponse.cs
  Models/
    ShortenedUrl.cs
    ClickEvent.cs
    ApplicationUser.cs
```

**Key changes:**

- Each feature/use-case is a self-contained folder with its endpoint, request/response DTOs, and validator
- `ShortenedUrlService.cs` gets decomposed — each endpoint handles its own DB logic directly (thin endpoints), or a small internal handler if complex
- `Program.cs` becomes minimal: registers `FastEndpoints`, Identity, JWT, DbContext, CORS — all endpoint mapping is handled by FastEndpoints auto-discovery
- The old `Services/`, `Dtos/` folders are removed
- `FluentValidation.AspNetCore` package can be removed since FastEndpoints has built-in FluentValidation integration

---

### Step 5: Organize Minimal API Endpoints via FastEndpoints

Each endpoint inherits from `Endpoint<TRequest, TResponse>` or `EndpointWithoutRequest<TResponse>`. Example structure for `ShortenUrlEndpoint`:

- Defines route, verb, auth policies, and roles in `Configure()`
- Contains request handling logic in `HandleAsync()`
- Validator is auto-discovered by FastEndpoints when placed in the same namespace/assembly

FastEndpoints auto-discovers all endpoint classes at startup. `Program.cs` simply calls:

- `builder.Services.AddFastEndpoints()`
- `app.UseFastEndpoints()`

Group prefixes will be set via `Group` property or route prefixes: `/api/auth/...`, `/api/urls/...`, `/api/analytics/...`. The redirect endpoint (`GET /{shortCode}`) stays at root level without `/api` prefix since it's the public-facing redirect.

---

### Step 6: Implement Authentication & Authorization in API

1. **Create `ApplicationUser`** class extending `IdentityUser` in `Models/ApplicationUser.cs` (can add `FullName`, `CreatedAt` etc.)

2. **Update `ApplicationDbContext`** in `Data/ApplicationDbContext.cs` to inherit from `IdentityDbContext<ApplicationUser>` instead of `DbContext`

3. **Configure Identity + JWT in `Program.cs`**:
   - `builder.Services.AddIdentity<ApplicationUser, IdentityRole>()` with `.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()`
   - Configure JWT Bearer authentication with `builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` using settings from `appsettings.json`
   - Add JWT config section to `appsettings.json`: `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpirationInMinutes`

4. **Auth Endpoints** (inside `Features/Auth/`):
   - `POST /api/auth/register` — register user with email/password, returns success
   - `POST /api/auth/login` — validate credentials, return JWT access token + refresh token
   - `GET /api/auth/me` — return current user info from token claims

5. **Add `UserId` FK** to `ShortenedUrl` model, create EF migration

6. **Protect all endpoints** with `[Authorize]` via FastEndpoints config. Only two endpoints are anonymous:
   - `GET /{shortCode}` (redirect — public)
   - `POST /api/auth/register` and `POST /api/auth/login` (auth endpoints)

7. All URL CRUD endpoints filter by `UserId` from the JWT claims so users only manage their own URLs.

---

### Step 7: Implement Authentication & Authorization in Frontend

1. **Add auth services** in `Program.cs` (WEB):
   - Register `AuthenticationStateProvider` (custom implementation that reads JWT from `Blazored.LocalStorage`)
   - Add `builder.Services.AddAuthorizationCore()`
   - Add `builder.Services.AddCascadingAuthenticationState()`

2. **Create custom `JwtAuthenticationStateProvider`** that:
   - Reads JWT from local storage
   - Parses claims from JWT to build `ClaimsPrincipal`
   - Exposes `NotifyUserAuthentication()` / `NotifyUserLogout()` methods

3. **Update Refit HTTP client** (`ApiServices/IUrlShortenerService.cs`):
   - Add `IAuthService` interface with `Login()`, `Register()`, `Logout()`, `GetCurrentUser()` methods
   - Add a `DelegatingHandler` that injects JWT from local storage into `Authorization: Bearer` header on every request

4. **Create auth pages** under `Components/Pages/`:
   - `Login.razor` — email/password form, calls login API, stores token, redirects to `/urls`
   - `Register.razor` — registration form, calls register API, redirects to login

5. **Update layout** in `Components/Layout/MainLayout.razor`:
   - Show Login/Register links when unauthenticated
   - Show user email + Logout button when authenticated
   - Wrap navigation in `<AuthorizeView>`

6. **Update `_Imports.razor`**: add `@using Microsoft.AspNetCore.Components.Authorization`

7. **Update `App.razor`**: ensure `<CascadingAuthenticationState>` wraps the component tree

---

### Step 8: Protect All Routes

**API (already covered in Step 6):**

- FastEndpoints default policy requires authentication on all endpoints
- Explicit `AllowAnonymous()` only on: `GET /{shortCode}`, `POST /api/auth/register`, `POST /api/auth/login`

**Frontend:**

- Update `Routes.razor`: replace `<RouteView>` with `<AuthorizeRouteView>` and add `<NotAuthorized>` template that redirects to `/login`
- Add `@attribute [Authorize]` to:
  - `Home.razor` (will become the dashboard)
  - `UrlShortener.razor`
- Login and Register pages remain `@attribute [AllowAnonymous]`

---

### Step 9: Update Unit Tests

- Update `ShortenedUrlServiceTests.cs` — tests will need refactoring since `ShortenedUrlService` is being replaced by individual FastEndpoints. Consider integration tests using `WebApplicationFactory<Program>` for endpoint testing.
- Add tests for auth flow (register, login, protected endpoint access)
- Keep `UtilsTestCases.cs` — `Utils.GenerateShortCode` logic stays

---

### Step 10: Database Migration

- Generate new EF Core migration after all model changes (Identity tables + `ClickEvent` + `ShortenedUrl` schema changes)
- Remove or regenerate existing migrations in `Migrations/` since the schema is changing significantly
- Fix the PostgreSQL connection string format in `appsettings.json`

---

### Verification

- `dotnet build UrlShortener.sln` — all 3 projects compile against .NET 10
- `dotnet test` — all unit tests pass
- Manual testing: register → login → create short URL → list URLs → redirect → view analytics → logout → verify protected routes redirect to login
- GitHub Actions: push to `master`, confirm CI workflow runs successfully
- Verify JWT expiration, token refresh, and unauthorized access returns 401

### Decisions

- **Blazor Server**: Keeping Interactive Server mode (not converting to WASM)
- **FastEndpoints**: Preferred over Carter or manual MapGroup for endpoint organization with built-in validation
- **JWT Bearer**: Stateless token auth for API, stored in browser local storage on the frontend
- **PostgreSQL**: Keeping Npgsql, connection string needs correction to PG format
- **Docker excluded**: Removing Docker-related properties from `.csproj`; Dockerfiles and docker-compose left for manual update later
