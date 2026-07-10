
Architecture: Four-layer .NET 9 Web API + Blazor WebAssembly client

```
Controller → Repository → Aggregate DAO → Single-table DAO
```

---

## Solution Structure

| Project | Purpose |
|---|---|
| `*.Api` | ASP.NET Core Web API — HTTP entry points, auth, DI wiring |
| `*.Web` | Blazor WebAssembly client (MudBlazor UI) |
| `*.Models` | Domain models, transfer models, static constants |
| `*.Interfaces` | All interfaces (repositories, DAOs, services) |
| `*.Repositories` | Orchestration layer — business logic lives here |
| `*.DataAccess` | Dapper-based SQL Server data access |
| `*.Repositories.Tests` | xUnit + Moq tests for repositories |
| `*.DataAccess.Tests` | xUnit + Moq tests for aggregate DAOs |

---

## Architecture Rules (Enforced)

### Layer 1 — Controllers

* Handle **HTTP concerns only**: routing, status codes, request/response mapping.
* Every action method is a **single-line call** to `Execute()` on `BaseController`.
* `BaseController.Execute<T>()` is the sole place exceptions are caught, logged, and wrapped in `ResponseBase<T>`.
* **No business logic. No try/catch inside action methods.**
* Inject repositories and `ILogger<T>` only.

```csharp
[HttpPost("Create")]
public Task<ResponseBase<int>> Create([FromBody] Lead lead)
    => Execute(
        () => _leadRepository.CreateLeadAsync(lead),
        _logger,
        "Error creating lead");
```

---

### Layer 2 — Repositories

* The **brain of the system** — all business logic and validation lives here.
* Orchestrate one or more DAO calls to fulfil a use-case.
* **Throw exceptions on failure** — never return silent nulls or booleans for errors.
* Do **not** manage transactions; delegate to Aggregate DAOs for transactional operations.
* May inject: single-table DAOs, aggregate DAOs, other repositories, `ILogger<T>`, `ICurrentUserService`.
* Prefer **C# for computations** (e.g. score aggregation, status decisions) over complex SQL expressions.

---

### Layer 3 — Aggregate DAOs

* Responsible for:
  * **Transactions** — open connection, begin transaction, coordinate DAO calls, commit or roll back.
  * **JOIN reads** — queries that span multiple tables, mapped to **flat, purpose-built read models**.
* Do **not** return raw entity graphs. Map to dedicated, flat models tailored to the use-case.
* Prefer **C# for computations** over `CASE WHEN` expressions in SQL.



### Layer 4 — Single-table DAOs (SQL)

* Single-table reads and writes **only**.
* Pure Dapper CRUD — no business logic, no joins, no transactions.
* For any transactional method, you MUST accept IDbConnection and IDbTransaction as parameters. NEVER use an injected class-level 
* _db field inside these methods. You must execute the query directly against the passed-in connection parameter."

```csharp

public async Task SomeTransactionalOpAsync(
    SomeModel model,
    IDbConnection conn,
    IDbTransaction tx)
```
**CRITICAL:** When writing transactional methods in DAOs, you must execute against the passed-in parameters, not a class-level field.

❌ **BAD (DO NOT DO THIS - Uses class-level `_db`):**
```csharp
public async Task InsertAsync(AuditLog entry, IDbConnection conn, IDbTransaction tx)
{
    // WRONG: Ignores the 'conn' parameter and uses injected '_db'
    await _db.ExecuteAsync(InsertSql, entry, tx); 
}
```

---

### Layer 4 — External API DAOs (HTTP)

When data lives in an external system (e.g. an HR platform), the DAO uses `HttpClient` instead of Dapper. The same interface-and-implementation pattern applies; the calling repository and controller are unaware of whether the DAO is SQL or HTTP.

**Rules:**
* Register as a **typed `HttpClient`** via `AddHttpClient<IDao, Dao>()` in `Program.cs`.
* Base URL is stored in `appsettings.json` under `ExternalApis:<ServiceName>`.
* In development, point the URL at the `*.WireMock` project (runs on a fixed local port).
* DAOs deserialise the response directly to models — no business logic.
* No transactions, no `IDbConnection` parameters.

```csharp
// Program.cs
builder.Services.AddHttpClient<IEmployeeDao, EmployeeDao>(client =>
    client.BaseAddress = new Uri(builder.Configuration["ExternalApis:EmployeeApi"]!));

// appsettings.json
"ExternalApis": {
  "EmployeeApi": "http://localhost:9090/"
}
```

The `*.WireMock` console project uses **WireMock.Net** to stub the external API with realistic seed data. Run it alongside the API during development. It accepts an optional port argument: `dotnet run -- 9090`.

---

## Transaction Rules

* Transactions are **opened and committed at the Aggregate DAO level**.
* The calling aggregate DAO opens the connection, begins the transaction, passes both to each single-table DAO it calls, then commits.
* Single-table DAOs must accept `IDbConnection` and optional `IDbTransaction` — they must use only what is provided.

```csharp
// Pattern inside an Aggregate DAO
if (_db.State != ConnectionState.Open) _db.Open();
using var tx = _db.BeginTransaction();
await _leadDao.InsertAsync(lead, _db, tx);
await _auditDao.InsertAsync(audit, _db, tx);
tx.Commit();
```

---

## Response Envelope

All API responses are wrapped in `ResponseBase<T>`:

```csharp
public class ResponseBase<T>
{
    public int    ReturnCode    { get; set; }  // 0 = success, -1 = failure
    public string ReturnMessage { get; set; }
    public T?     Data          { get; set; }

    public static ResponseBase<T> Success(T data, string message = "Success") { ... }
    public static ResponseBase<T> Failure(string message = "An error occurred") { ... }
}
```

* `ReturnCode == 0` means success.
* On failure, `ReturnMessage` carries the exception message (no stack trace).
* Controllers never expose raw exceptions to clients.

---

## Database Conventions

* **SQL Server + Dapper** — no EF Core anywhere in the solution.
* Always use **explicit column lists** — never `SELECT *`.
* Define SQL as `public static readonly string` fields on the DAO class.
* Updates use `COALESCE(@Value, ColumnName)` for optional (nullable) fields.
* Status fields are `NVARCHAR` in SQL, backed by `const string` values in static C# classes (e.g. `LeadStatus`, `ProposalStatus`).
* All timestamps are stored as **UTC `DATETIME`**.
* **Nullable reference types are enabled** across all projects.

## Primary Key & Identity Constraints (Enforced)
* Mandatory Key Structure: Every table in the database must have a single primary key column named exactly Id.
* Data Type: The Id field must be of type int.
* Identity Configuration: The field must use identity specification starting at 0 with an increment of 1: IDENTITY(0,1).
* No Compound Keys: Composite or multi-field primary keys are strictly forbidden. All tables must rely solely on the single-column auto-incrementing integer Id field as their primary key.

### Database Migrations

Every schema change **must** be recorded as a numbered SQL script in the `Database/` folder at the solution root:

```
Database/
  001_EmployeeAttributes.sql
  002_AddSomeTable.sql
  ...
```

* Scripts are numbered sequentially and never modified once merged.
* Each script must be idempotent where possible (use `IF NOT EXISTS` guards).
* Scripts are applied manually to each environment in order — there is no automated migration runner.



### Program.cs wiring


### Interface (in `*.Interfaces`)

```csharp
public interface ICurrentUserService
{
    string? ObjectId    { get; }   // Entra "oid" claim — stable unique user ID
    string? Email       { get; }   // "preferred_username" / "upn" / "email"
    string? DisplayName { get; }   // "name" claim
    string? TenantId    { get; }   // "tid" claim
    bool    IsAuthenticated { get; }

    /// Throws InvalidOperationException if not authenticated.
    string RequireObjectId();
}
```


### Usage in a repository

```csharp
public class LeadRepository : ILeadRepository
{
    private readonly ILeadDataAccessObject  _leadDao;
    private readonly ICurrentUserService    _currentUser;

    public LeadRepository(ILeadDataAccessObject leadDao, ICurrentUserService currentUser)
    {
        _leadDao     = leadDao;
        _currentUser = currentUser;
    }

    public async Task<int> CreateLeadAsync(Lead lead)
    {
        lead.CreatedByObjectId = _currentUser.RequireObjectId();
        return await _leadDao.InsertAsync(lead);
    }
}
```

---

## Error Handling Strategy

| Layer | Responsibility |
|---|---|
| Single-table DAO | Throws on DB errors (Dapper propagates) |
| Aggregate DAO | Rolls back transaction on exception; re-throws |
| Repository | Throws `InvalidOperationException` or domain exceptions for business rule violations |
| Controller | `BaseController.Execute<T>()` catches all exceptions, logs them, returns `ResponseBase.Failure(ex.Message)` |

Repositories should **throw** rather than return silent failures — this preserves the exception message that ultimately reaches the client via `ResponseBase`.

---

## Dependency Injection Registration

All registrations in `Program.cs` use **Scoped** lifetime (one instance per HTTP request):

```
IDbConnection          → SqlConnection         (per-request DB connection)
ICurrentUserService    → CurrentUserService     (per-request identity)
ISomeDao               → SomeDao               (all DAOs)
ISomeRepository        → SomeRepository        (all repositories)
IProposalPdfService    → ProposalPdfService     (scoped services)
```

Registration order in `Program.cs`:

1. Dapper type handlers (global, once at startup)
2. Database connection
3. DAOs
4. Repositories
5. Other services (PDF, etc.)
6. MVC controllers

---

## Testing Standards

### Repository Tests

* All **public repository methods must be tested**.
* Frameworks: **xUnit** + **Moq**.
* Mock all DAO dependencies — repositories must be testable without a database.
* Naming convention: `{RepositoryName}Tests` (e.g. `LeadRepositoryTests`).

Testing Standards: Data Access Layer (DAO & Aggregates)
You are strictly forbidden from writing tests that assert string contents (e.g., checking if a raw SQL string contains a specific word or column name). This provides false coverage. Implement the following two tiers of testing for the data access layer using standard xUnit assertions only (do not use FluentAssertions):

### Base Data Access Objects (SQL Verification)
Goal: Verify SQL correctness, table existence, and schema validity.
Pattern: Execute queries against a real database instance using connection strings provided by a test fixture.
Scope: Use SET FMTONLY ON; or execute with default parameters against empty tables. The test passes if it executes without throwing a SqlException. If tables or columns are renamed or dropped, these tests must fail.
Assertion Style: Use native xUnit execution blocks. For example:

C#
// Ensure query executes without throwing SqlException
var exception = await Record.ExceptionAsync(() => db.ExecuteAsync(MyDao.SomeSql, new { Id = 0 }));
Assert.Null(exception);

### Aggregated Data Access Objects (Transaction & Coordination)
Goal: Verify that units of work safely orchestrate transactions, loop logic, and status mappings.
Pattern: Mock all underlying base DAOs injected into the Aggregate DAO constructor using Moq.
Transaction Invariance:
Every mutation method utilizing an explicit transaction must have a success test verifying Commit() is called on the transaction mock.
Every mutation method must have a failure test where an underlying dependency throws an exception. Use Assert.ThrowsAsync<T> to verify the exception is rethrown, and verify that Rollback() is called on the transaction mock exactly once.
Connection & Transaction Passing: Assert that the active IDbConnection and IDbTransaction instances are passed down into the underlying DAO mock calls to maintain atomic operations within the connection scope.

### Coverage Reports

Run `run-coverage.bat` from the solution root to generate per-project HTML coverage reports:

```
run-coverage.bat
```

* Uses **coverlet** (XPlat Code Coverage) + **ReportGenerator** (local dotnet tool).
* Each test project gets its own `coveragereport/index.html` alongside it.
* The batch script opens all reports automatically on completion.
* `TestResults/`, `coveragereport/`, and `*.cobertura.xml` are excluded from git (see `.gitignore`).


## UI Guide

All Blazor client work follows **`UI_SKILL.md`** (in the solution root). That document is the authoritative reference for:

* CSS token system (`--c-bg`, `--c-accent`, `--c-text-*`, etc.)
* `hc-shell` CSS grid layout (220 px nav / 52 px topbar / 1fr body)
* Page-level component classes (`page-header`, `detail-card`, `stat-card`, `empty-state`, `loading-pulse`)
* MudBlazor theme configuration and component usage patterns

When building or reviewing any UI component, consult `UI_SKILL.md` first.

---

## Blazor (Web Client) Conventions

* **Two-way binding with callbacks**: use `@bind-Value` with `@bind-Value:after`, never `@bind-Value` + `ValueChanged` on the same component (compile error).
* The `:after` callback is parameterless — the bound field is already updated when it fires.
* **Dialogs**: do not use `IsVisible`. Use `@ref` on a component that wraps `MudDialog` and call `await _dialog.ShowAsync()` / `await _dialog.CloseAsync()` explicitly.
* **Dialog pattern**: expose a `public async Task OpenAsync(...)` method; raise an `[Parameter] EventCallback OnCreated` when the operation completes successfully.
* **Parallel data loading**: use `Task.WhenAll` for independent API calls in `OnInitializedAsync`.

---

## Date Formatting

* All dates displayed in the UI use `yyyy/MM/dd`.
* All `MudDatePicker` components include `DateFormat="yyyy/MM/dd"`.
* Timestamps (date + time) use `yyyy/MM/dd HH:mm`.
* The app culture is set globally to `en-ZA` in `*.Web/Program.cs` — required for MudDatePicker formatting. Do not remove it.

------

## Section: Type Safety & Domain Constraints
* Rule: No Magic Strings for State.
* State Management: All entity states, statuses, and categories must be implemented as Enumerations (enums).
* Implementation: Properties named Status, Type, or Category must never be string types in the domain model.
* Conversion: Use EF Core Value Converters to persist Enums. Do not use integer-based logic in business services; always refer to the Enum member name.
* Validation: Use Enum.IsDefined or FluentValidation IsInEnum() to ensure incoming API data matches valid domain states.

---

## Summary of Key Principles

* **Controllers = thin HTTP adapters** — one line per action, no logic.
* **Repositories = system brain** — all business rules, validation, orchestration.
* **Aggregate DAOs = joins + transactions** — flat read models, explicit connection/transaction passing.
* **Single-table DAOs = pure data access** — no logic, no connection management.

* **Throw on failure** — silent returns mask errors; exceptions preserve context.
* **Prefer C# over SQL** for any computation or conditional logic.
* **Test coverage is mandatory** at repository and aggregate DAO layers.


---

## Web UI Architecture & Testing Standards (Blazor / Razor Components)

### Code-Behind Requirement

* All non-trivial Razor pages/components must use a separate code-behind class (`.razor.cs`).
* Razor markup files are responsible only for presentation and binding.
* Business rules, decision logic, state transitions, validation, orchestration, and UI behaviour must reside in the code-behind class.
* The code-behind class becomes the primary unit under test.
* Avoid placing logic directly inside `@code { }` blocks except for trivial UI-only concerns.

Example:

```
CustomerPage.razor
CustomerPage.razor.cs
```

### Page Model Responsibility

The code-behind class is the behavioural model for the page.

Responsibilities include:

* Loading data from APIs.
* Managing page state.
* Executing commands.
* Validation and decision logic.
* Coordinating user interactions.
* Determining UI visibility and enabled/disabled states.

The Razor file should bind to properties and invoke methods exposed by the code-behind model.

### Web Test Project

Every solution must contain a dedicated web testing project:

```
*.Web.Tests
```

Frameworks:

* xUnit
* Moq
* ASP.NET Core Test Host (`WebApplicationFactory`)
* Optional: bUnit for component rendering tests where required

Purpose:

* Execute real HTTP requests against actual API endpoints.
* Validate end-to-end request processing through Controllers, Repositories, and application services.
* Replace DAO implementations with mocks so that tests remain deterministic and database-independent.

### API Integration Test Rules

Tests must:

* Host the actual API application in memory.
* Call real HTTP endpoints using `HttpClient`.
* Exercise real controller routing.
* Exercise real repository logic.
* Exercise real validation paths.
* Mock all DAO dependencies.

The objective is to verify application behaviour while avoiding database dependencies.

Example execution path:

```
Test
  -> HttpClient
      -> Controller
          -> Repository
              -> Mock DAO
```

### Decision Coverage Requirement

Every decision point that influences page behaviour must have test coverage.

Examples:

* Success path.
* Failure path.
* Validation failures.
* Conditional visibility.
* Conditional enable/disable behaviour.
* Empty data scenarios.
* Permission-based behaviour.
* Status-based behaviour.
* Error handling paths.

If a page contains five independent behavioural decisions, there must be tests covering all possible outcomes of those decisions.

### Minimum Coverage Standard

For each page model:

* Every public method must be tested.
* Every conditional branch must be tested.
* Every error path must be tested.
* Every state transition must be tested.

Coverage should focus on behavioural correctness rather than line-count metrics.

### Mocking Rules

Web tests must mock:

* Single-table DAOs
* Aggregate DAOs
* External API DAOs

Web tests must NOT mock:

* Controllers
* Repositories
* Application services under test
* Authentication abstractions (unless explicitly testing authentication scenarios)

The goal is to validate as much real application behaviour as possible while isolating infrastructure concerns.

### Architectural Principle

The UI layer must be designed as testable software, not as markup containing embedded logic.

A developer should be able to:

1. Instantiate a page model.
2. Provide mocked dependencies.
3. Execute behaviour.
4. Assert state changes.
5. Verify all decision paths.

without requiring a browser, database, or external service.
