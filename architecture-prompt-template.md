# .NET API Architecture Prompt Template

## Project Overview
Build a .NET [VERSION] Web API for [PROJECT NAME].

[Brief description of what the system does.]

---

## Solution Structure

Generate the following projects in a single solution:

| Project | Type | Purpose |
|---|---|---|
| `[AppName].Api` | ASP.NET Core Web API | Controllers, startup, DI registration |
| `[AppName].Models` | Class Library | Domain models, status constants, transfer models, detail/view models |
| `[AppName].Interfaces` | Class Library | Repository and service interfaces |
| `[AppName].Repositories` | Class Library | Repository and service implementations (Dapper + SQL Server) |

---

## Architecture Pattern

Use a strict four-layer architecture. Every data operation flows top-to-bottom through all four layers. No layer may skip another.

```
Controller
    └── Repository          (orchestration, error handling)
            └── AggregateService    (transactions, complex multi-table reads via JOIN queries)
            └── Service             (single-table CRUD, raw Dapper SQL)
```

### Layer responsibilities

**Controller** (`[AppName].Api/Controllers/`)
- Thin. No business logic.
- Accepts HTTP requests, calls the repository, wraps result in `ResponseBase<T>`, returns it.
- All responses use `ResponseBase<T>` with `ReturnCode` (0 = success, -1 = failure) and `ReturnMessage`.
- Inherits from `BaseController : ControllerBase` which is decorated with `[ApiController]`.

**Repository** (`[AppName].Repositories/[Entity]Repository.cs`)
- Catches exceptions, logs them via `ILogger<T>`, returns null/false on failure (never throws to the controller).

- Delegates all reads to either the AggregateService (for joined/detail reads) or the Service (for flat reads).

**AggregateService** (`[AppName].Repositories/[Entity]AggregateService.cs`)
- Orchestrates multi-step operations and owns transactions.
- For write operations that touch multiple tables, opens a connection, begins a transaction, calls services within it, commits or rolls back.
- Handles complex SELECT queries that JOIN multiple tables.
- Returns detail/view model classes (e.g. `LessonDetail`) that are flat projections of the join.
- Queries are declared as `public static readonly string` constants so they can be referenced in tests.
- Never writes to the database.

**Service** (`[AppName].Repositories/[Entity]Service.cs`)
- Handles all single-table SQL: SELECT, INSERT, UPDATE.
- Takes `IDbConnection` via constructor injection.
- INSERT methods that need the new ID use `SELECT CAST(SCOPE_IDENTITY() AS int)`.
- UPDATE methods return `rowsAffected > 0`.
- For operations that must run inside a transaction, accepts `IDbTransaction tx` as a parameter.
- Never opens its own connection or transaction.

---

## Models Project

### Domain models (`[AppName].Models/[Entity].cs`)
- One file per entity.
- Include a companion `static class [Entity]Status` with `const string` values for any status fields.
- Nullable reference types enabled. Use `string? Field` for optional strings, `int?` for optional FKs.
- Include `CreatedAt` on all entities.
- Include `Notes` (`string?`) on any entity where a teacher/user might want to annotate a record.

### Detail/view models (`[AppName].Models/[Entity]Detail.cs`)
- Flat classes that Dapper maps directly from JOIN query results.
- Named with the `Detail` suffix.
- Include computed properties (e.g. `FullName`, `LessonsRemaining`) as expression-bodied `get` properties.
- No navigation properties — keep them flat.

### Transfer models (`[AppName].Models/TransferModels/`)
- `ResponseBase<T>` with `int ReturnCode`, `string ReturnMessage`, `T Data`.
- `RequestBase<T>` for any complex POST bodies.
- Specific request classes (e.g. `AddBundleRequest`) when a single endpoint needs more than one entity.

---

## Interfaces Project

### Repository interfaces (`IXxxRepository.cs`)
- Mirror the public methods of the Repository implementation exactly.
- XML summary comments on any non-obvious methods.

### Service interfaces (`IXxxService.cs`)
- Mirror the public methods of the Service implementation.
- Overloaded `InsertAsync` signatures: one without transaction (standalone callers) and one with `IDbTransaction tx` (transactional callers).
- Optional parameters (e.g. `string? note = null`) use default values on the interface.

---

## Database conventions

- SQL Server with Dapper (no EF Core).
- All SELECT queries explicitly list column names — never use `SELECT *`.
- `COALESCE(@Value, ColumnName)` pattern for optional UPDATE fields so that passing `null` is a no-op.
- Filtered unique indexes (`WHERE ColumnName IS NOT NULL`) for nullable unique columns.
- Status columns are `NVARCHAR` storing the string constant values from the model's status class.
- Timestamps are `DATETIME` (UTC). `TimeOnly` columns map to SQL `TIME` via a custom `TimeOnlyTypeHandler`.
- Register the `TimeOnlyTypeHandler` at startup: `SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler())`.

---

## API conventions

- Route prefix matches controller name: `[Route("Invoice")]`, `[Route("Lesson")]`, etc.
- GET endpoints use `[HttpGet]` with `[FromQuery]` parameters.
- POST endpoints use `[HttpPost]` with `[FromBody]`.
- PUT endpoints use `[HttpPut]`. Prefer `[FromQuery]` for simple status/flag updates, `[FromBody]` for full entity updates.
- Optional fields (e.g. `note`) on PUT endpoints are `[FromQuery] string? note = null`.
- URL-encode query string values in the client when they may contain special characters: `Uri.EscapeDataString(value)`.

---

## Startup / DI registration (`[AppName].Api/Startup.cs`)

Register in this order:
1. `TimeOnlyTypeHandler`
2. `IDbConnection` → `SqlConnection` (scoped)
3. All services (scoped)
4. All repositories (scoped)
5. `AddControllers()`
6. `AddOpenApi()` + Scalar UI for development

Use Serilog for structured logging with Console and rolling File sinks.

---

## Blazor WebAssembly front-end 

### Client services (`Services/Services.cs`)
- One service class per API controller, e.g. `LessonService`, `ExtraLessonService`.
- All methods are `async Task<T>` wrapped in try/catch returning a safe default on failure.
- GET calls use `GetFromJsonAsync<ResponseBase<T>>`.
- POST calls use `PostAsJsonAsync` then read `ReadFromJsonAsync<ResponseBase<T>>`.
- PUT calls use `PutAsync` with a manually constructed URL string. Append optional query params with a null check before appending.
- Never throw from a service method — log to `Console.Error` and return default.

### UI framework
- MudBlazor for all UI components.
- `MudDialog` for all create/edit/confirm flows — never navigate to a separate page for a form.
- `MudSnackbar` for all success/error feedback.
- `StatusChip` shared component that maps status strings to CSS classes.
- `ResponseBase<T>.ReturnCode != 0` check for API-level business errors (distinct from HTTP errors).

---
### Unit Testing ([AppName].Tests)
Adhere strictly to TDD principles using xUnit and Moq to ensure structural integrity and correct orchestration.

Repository Testing
Focus: Tests must target the [AppName].Repositories/[Entity]Repository.cs classes.

Mocking: Inject mocked dependencies (ILogger<T>, IService, IAggregateService) using Moq. No database interfaces (IDbConnection, IDbTransaction) should be mocked here, as the Repository is database-agnostic.

Validation criteria:

Verify that the Repository calls the correct underlying Service/AggregateService methods.

Verify exception handling: use Moq to configure a Service to throw an exception, then assert that the Repository catches it, logs the error via the mocked ILogger, and returns the expected failure state (null/false).

Verify that dependencies are called with the correct mapped parameters.
---

### Integration Testing ([AppName].Integration.Tests)
Service and aggregated Service Testing into the Database
Focus: Tests must target the [AppName].Repositories/[Entity]Service.cs classes.
Execute EACH SQl Statement With to VALIDATE the Systax, against an actual Database (So all queries has to be publicly declared)

## Entities to generate

Replace the placeholders below with your actual domain entities.

```
[Entity1]
  Fields: [list fields, types, nullable?, e.g. "Name NVARCHAR(100) NOT NULL"]
  Status values: [e.g. Active, Inactive] or NONE
  Relationships: [e.g. belongs to Entity2 via Entity2ID]
  Notes field: YES / NO

[Entity2]
  Fields: ...
  Status values: ...
  Relationships: ...
  Notes field: YES / NO
```

---

## Cross-cutting requirements

- [ ] Nullable reference types enabled on all projects.
- [ ] `COALESCE` pattern on all optional UPDATE fields.
- [ ] All status values defined as `const string` in a companion static class in the model file.
- [ ] `Notes` field on entities where annotation makes sense; always included in all SELECT queries and mapped on all read models.
- [ ] Filtered unique indexes on nullable unique columns.
- [ ] `InstallmentNumber` or sequence fields: drop check constraints before adding uniqueness constraints if the business rule allows multiples per parent.
- [ ] Transactions for any operation that writes to more than one table.
- [ ] Repository catches all exceptions, logs with structured parameters (`{EntityID}`), returns null/false.
- [ ] Controllers never contain business logic or exception handling — only call repository and wrap result.
- [ ] Client service methods URL-encode any string query parameters.
- [ ] Optional note/reason fields default to `null` at every layer (controller → repository → service → SQL).

---

## Example file listing (for a single entity `Lesson`)

```
MusicSchool.Models/
  Lesson.cs                          // Lesson, LessonStatus, CancelledBy
  LessonDetail.cs                    // LessonDetail (flat join projection)

MusicSchool.Interfaces/
  ILessonRepository.cs
  ILessonService.cs
  ILessonAggregateService.cs

MusicSchool.Repositories/
  LessonRepository.cs                // orchestration + transactions
  LessonService.cs                   // single-table Dapper SQL
  LessonAggregateService.cs          // JOIN queries → LessonDetail

MusicSchool.Api/Controllers/
  LessonController.cs
```
