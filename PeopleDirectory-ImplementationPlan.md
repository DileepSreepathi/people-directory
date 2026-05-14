# People Directory Application — Implementation Plan

## 1. Technology Stack

| Layer | Technology |
|---|---|
| Backend API | .NET 8 Web API (C#) |
| Frontend | React 18 + TypeScript + Vite |
| Database | SQL Server 2022 |
| ORM | Entity Framework Core 8 (Code-First) |
| Authentication | ASP.NET Core Identity + JWT Bearer Tokens |
| Email | SMTP via MailKit / SmtpClient |
| Containerisation | Docker + Docker Compose |
| Patterns | Repository, Dependency Injection, SOLID, Clean Architecture |

---

## 2. Solution Structure

```
PeopleDirectory/
├── docker-compose.yml
├── docker-compose.override.yml
│
├── src/
│   ├── PeopleDirectory.Domain/            # Entities, Enums, Interfaces
│   ├── PeopleDirectory.Application/       # DTOs, Services, Validators
│   ├── PeopleDirectory.Infrastructure/    # EF DbContext, Repositories, Email
│   ├── PeopleDirectory.API/               # Controllers, Middleware, Program.cs
│   └── PeopleDirectory.React/             # React SPA (Vite + TypeScript)
│
├── tests/
│   ├── PeopleDirectory.UnitTests/
│   └── PeopleDirectory.IntegrationTests/
│
└── README.md
```

---

## 3. Database Design (Code-First Entities)

### 3.1 Person Table

| Column | Type | Notes |
|---|---|---|
| Id | int (PK, Identity) | |
| FirstName | nvarchar(100) | Indexed for search |
| LastName | nvarchar(100) | Indexed for search |
| Email | nvarchar(255) | Unique |
| MobileNumber | nvarchar(20) | |
| Gender | nvarchar(10) | Male / Female / Other |
| ProfilePictureUrl | nvarchar(500) | Relative path to uploaded image |
| DateOfBirth | date | Bonus field |
| CityId | int (FK) | References City table |
| AddressLine | nvarchar(300) | Optional street address |
| CreatedAt | datetime2 | |
| UpdatedAt | datetime2 | |
| IsActive | bit | Soft-delete flag |

### 3.2 Country Table

| Column | Type |
|---|---|
| Id | int (PK, Identity) |
| Name | nvarchar(100) |
| Code | nvarchar(5) |

### 3.3 City Table

| Column | Type |
|---|---|
| Id | int (PK, Identity) |
| Name | nvarchar(100) |
| CountryId | int (FK) |

### 3.4 AdminUser Table (ASP.NET Identity)

Managed by ASP.NET Core Identity — stores credentials, roles, and claims in the standard Identity tables (AspNetUsers, AspNetRoles, etc.).

### 3.5 AuditLog Table

| Column | Type | Notes |
|---|---|---|
| Id | int (PK, Identity) |
| PersonId | int (FK) | |
| Action | nvarchar(20) | Created / Updated / Deleted |
| ChangesJson | nvarchar(max) | JSON diff of old vs new values |
| PerformedBy | nvarchar(100) | Admin username |
| PerformedAt | datetime2 | |

### 3.6 Seed Data

Pre-populate the Country and City tables via EF Core migrations with a reasonable set of countries and their major cities so the cascading dropdowns work out of the box. Seed at least 20 sample Person records for demo purposes.

---

## 4. Backend API Implementation

### 4.1 Project Setup

1. Create a new .NET 8 solution with the four class library / web projects listed in Section 2.
2. Configure `appsettings.json` with SQL Server connection string, JWT settings, and SMTP credentials.
3. Register all services in `Program.cs` using the built-in DI container.

### 4.2 Domain Layer (`PeopleDirectory.Domain`)

Define entity classes for Person, Country, City, and AuditLog. Define repository interfaces: `IPersonRepository`, `ICountryRepository`, `ICityRepository`, `IAuditLogRepository`. Keep this layer free of any framework dependencies.

### 4.3 Application Layer (`PeopleDirectory.Application`)

Define DTOs: `PersonSummaryDto` (for grid), `PersonDetailDto` (for detail view), `PersonCreateDto`, `PersonUpdateDto`, `SearchResultDto`, `CountryDto`, `CityDto`. Define service interfaces and implementations: `IPersonService`, `ILocationService`, `IEmailService`. Add FluentValidation validators for create/update DTOs. Implement AutoMapper profiles for Entity-to-DTO mapping.

### 4.4 Infrastructure Layer (`PeopleDirectory.Infrastructure`)

Implement `AppDbContext` inheriting from `IdentityDbContext` with `DbSet<Person>`, `DbSet<Country>`, `DbSet<City>`, `DbSet<AuditLog>`. Implement the generic repository pattern with a base `Repository<T>` class and specific repositories. Implement `EmailService` using MailKit to send SMTP emails on record create/update. Add EF Core migration configuration with seed data.

### 4.5 API Layer (`PeopleDirectory.API`)

#### Controllers

**PeopleController** (public, no auth required):

| Endpoint | Method | Purpose |
|---|---|---|
| `/api/people/search?query={text}` | GET | Predictive type-ahead — returns top 10 matches by FirstName or LastName (LIKE '%text%') |
| `/api/people?query={text}&country={id}&city={id}&gender={g}&page={n}&size={s}` | GET | Full search with filters + pagination |
| `/api/people/{id}` | GET | Full person detail |

**AdminPeopleController** (requires JWT auth + Admin role):

| Endpoint | Method | Purpose |
|---|---|---|
| `/api/admin/people` | GET | List all people (paged) |
| `/api/admin/people/{id}` | GET | Get person for editing |
| `/api/admin/people` | POST | Create new person (multipart for image upload) |
| `/api/admin/people/{id}` | PUT | Update person |
| `/api/admin/people/{id}` | DELETE | Soft-delete person |

**LocationController** (public):

| Endpoint | Method | Purpose |
|---|---|---|
| `/api/locations/countries` | GET | All countries for dropdown |
| `/api/locations/countries/{id}/cities` | GET | Cities by country (cascading dropdown) |

**AuthController**:

| Endpoint | Method | Purpose |
|---|---|---|
| `/api/auth/login` | POST | Validate credentials, return JWT |
| `/api/auth/refresh` | POST | Refresh expired token |

#### Middleware & Cross-Cutting

Add global exception handling middleware. Add Serilog for structured logging. Enable CORS for the React dev server origin. Add Swagger/OpenAPI documentation via Swashbuckle.

---

## 5. Frontend (React) Implementation

### 5.1 Project Setup

Scaffold with Vite + React + TypeScript. Install key packages: `axios`, `react-router-dom`, `react-query` (TanStack Query), `react-select` (for dropdowns), `react-hook-form` + `zod` (form validation), and a UI library such as Ant Design or Material UI.

### 5.2 Folder Structure

```
src/
├── api/             # Axios instance, API service functions
├── components/      # Shared UI components (SearchBar, PersonCard, Filters, etc.)
├── features/
│   ├── search/      # Client search page, type-ahead, result grid
│   ├── detail/      # Person detail page
│   └── admin/       # Admin login, CRUD pages
├── hooks/           # Custom hooks (useDebounce, useAuth, etc.)
├── context/         # AuthContext for JWT token management
├── types/           # TypeScript interfaces matching API DTOs
├── routes/          # Route definitions, ProtectedRoute wrapper
└── App.tsx
```

### 5.3 Client Section Pages

**Search Page (`/`)**

The landing page features a prominent search input with debounced type-ahead (300ms delay). As the user types, call `/api/people/search?query=...` and show a dropdown of matching names. On selecting a suggestion (or pressing Enter), navigate to the results grid. The grid shows summary cards/rows with: Name, City, Country, Email. Sidebar or toolbar filters for Country (dropdown), City (cascading dropdown), and Gender (radio/toggle) — all filter the results via query parameters to the API. Implement pagination or infinite scroll.

**Person Detail Page (`/people/:id`)**

Fetches `/api/people/{id}` and displays the full profile: profile picture, name, surname, email, mobile, gender, address (city + country), date of birth, and any other fields. Include a "Back to results" button that preserves the previous search state.

### 5.4 Admin Section Pages

**Login Page (`/admin/login`)**

Simple email + password form. On success, store the JWT in memory (React context) and a refresh token in an httpOnly cookie (or secure storage). Redirect to the admin dashboard.

**Admin Dashboard (`/admin`)**

A paginated table of all people with Edit/Delete action buttons and a "Add New Person" button.

**Create / Edit Person Form (`/admin/people/new` | `/admin/people/:id/edit`)**

React Hook Form with fields for every Person attribute. The Country dropdown loads all countries; on selection, the City dropdown fetches `/api/locations/countries/{id}/cities` dynamically. Profile picture upload via a file input with image preview. Validation feedback inline using zod schema.

**Protected Routes**

Wrap all `/admin/*` routes in a `<ProtectedRoute>` component that checks for a valid JWT in AuthContext and redirects to login if absent.

---

## 6. Email Notification

When a person record is created or updated via the Admin section, the backend `IEmailService` sends an HTML email containing: the action performed (Created / Updated), the affected person's name, a summary of changed fields with old and new values (for updates), and a timestamp. The email is sent asynchronously (fire-and-forget with error logging) so it doesn't block the API response. Configure SMTP settings (host, port, credentials) via `appsettings.json` and environment variables for Docker.

---

## 7. Docker Setup

### 7.1 Dockerfile — API

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish src/PeopleDirectory.API -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PeopleDirectory.API.dll"]
```

### 7.2 Dockerfile — React

```dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY src/PeopleDirectory.React/package*.json ./
RUN npm ci
COPY src/PeopleDirectory.React/ .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
```

### 7.3 docker-compose.yml

```yaml
version: "3.8"
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "YourStrong!Passw0rd"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

  api:
    build:
      context: .
      dockerfile: src/PeopleDirectory.API/Dockerfile
    environment:
      ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=PeopleDirectoryDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
      Jwt__Secret: "your-256-bit-secret-key-here"
      Email__SmtpHost: "smtp.example.com"
      Email__SmtpPort: "587"
    ports:
      - "5000:8080"
    depends_on:
      - sqlserver

  web:
    build:
      context: .
      dockerfile: src/PeopleDirectory.React/Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - api

volumes:
  sqldata:
```

---

## 8. Step-by-Step Implementation Order

### Phase 1 — Foundation (Day 1–2)

1. Create the .NET 8 solution and all projects. Wire up project references.
2. Define domain entities (Person, Country, City, AuditLog) in the Domain project.
3. Create `AppDbContext` with entity configurations and seed data.
4. Generate the initial EF Core migration and verify it creates the schema on SQL Server.
5. Set up Docker Compose with SQL Server and confirm the API boots and applies migrations on startup.

### Phase 2 — Backend API (Day 2–3)

6. Implement the generic repository and specific repositories in Infrastructure.
7. Implement `PersonService`, `LocationService` in the Application layer.
8. Build the public `PeopleController` with search (type-ahead) and detail endpoints.
9. Build the `LocationController` (countries list + cities-by-country).
10. Add Swagger and test all endpoints via the Swagger UI.

### Phase 3 — Admin Backend (Day 3–4)

11. Configure ASP.NET Core Identity and seed a default admin user.
12. Implement `AuthController` with JWT login/refresh.
13. Build `AdminPeopleController` with full CRUD + image upload.
14. Implement change-tracking logic that compares old vs new values and writes to AuditLog.
15. Implement `EmailService` and trigger it on create/update. Test with a tool like MailHog in Docker.

### Phase 4 — React Client Section (Day 4–5)

16. Scaffold the React project with Vite + TypeScript. Set up routing and Axios instance.
17. Build the Search page with debounced type-ahead input calling the search API.
18. Build the results grid/table with filter sidebar (Country, City, Gender).
19. Build the Person Detail page displaying the full profile.

### Phase 5 — React Admin Section (Day 5–6)

20. Build the Admin Login page and AuthContext (JWT handling).
21. Build the Admin Dashboard with a paginated people table.
22. Build the Create/Edit form with cascading Country → City dropdowns and image upload.
23. Wire up delete (soft-delete) with a confirmation dialog.
24. Add ProtectedRoute component to guard admin routes.

### Phase 6 — Polish & Deployment (Day 6–7)

25. Write unit tests for services and repositories (xUnit + Moq).
26. Write integration tests for API endpoints (WebApplicationFactory).
27. Add the React Dockerfile and Nginx config. Verify the full Docker Compose stack runs end-to-end.
28. Final UI polish — responsive layout, loading states, error toasts, empty states.
29. Write the project README with setup instructions.

---

## 9. Bonus / Impress Items

These are optional additions that go beyond the base requirements and demonstrate depth:

- **Debounce + Highlight**: Highlight the matched portion of the name in the type-ahead dropdown results.
- **Pagination with Total Count**: Show "Showing 1–10 of 42 results" with proper server-side pagination.
- **Dark Mode Toggle**: A simple theme switcher using CSS variables or the UI library's theming.
- **Export to CSV**: A button on the search results grid that downloads the current filtered results as CSV.
- **Profile Picture Crop**: Use a library like `react-easy-crop` to let admins crop the profile image before upload.
- **SignalR Real-Time**: Push a toast notification to any open admin browser tab when another admin edits a record.
- **Rate Limiting**: Apply rate limiting on the type-ahead endpoint to prevent abuse.
- **Health Checks**: Add `/health` and `/ready` endpoints for container orchestration readiness probes.
- **Fluent Validation Error Mapping**: Return structured `ProblemDetails` (RFC 7807) responses with per-field errors that the React form consumes directly.

---

## 10. Key Design Decisions

**Why Clean Architecture over simple MVC?** The assessment asks for Repository pattern, DI, and SOLID. Clean Architecture naturally enforces these by separating Domain, Application, Infrastructure, and API layers, making the codebase testable and maintainable.

**Why JWT over Cookie-based sessions?** Since the frontend is a React SPA served from a different origin (in development), token-based auth avoids CORS cookie issues and aligns well with the REST API approach.

**Why Code-First EF Core?** Explicitly required by the assessment. Code-First gives full control over the schema via C# entity classes and migrations, avoids the EDMX designer, and works seamlessly in Docker containers where there's no Visual Studio tooling.

**Why debounce on the client instead of server-side throttling alone?** Debouncing (300ms) on the React side drastically reduces the number of API calls during rapid typing, improving both UX and server load. Server-side rate limiting serves as a safety net.
