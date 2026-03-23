In my design, 
Here is the **updated merged README** with your additional rule incorporated cleanly and consistently:

---

# Stride.Sales

**Sales Lifecycle Automation & Weighted Qualification Engine**
Architecture: Four-layer .NET 9 Web API
**Controller → Repository → DataAccessObject (Aggregate + Single-table)**

---

## Architecture Rules (Enforced)

### Controllers

* Handle **HTTP concerns only** (routing, status codes, request/response mapping).
* Call the **Repository** and wrap the result in `ResponseBase<T>`.
* **No business logic**.
* **No try/catch for business flow**.
* If a repository throws:

  * Controller returns a failed `ResponseBase`
  * Exception must be logged (via injected logging or middleware strategy).

---

### Repositories

* Act as the **primary orchestration layer**.
* Responsible for:

  * Business logic
  * Validation
  * Coordinating data access
* Interact with **DataAccessObjects (DAOs)**.
* No transaction orchestration
* May throw exceptions (preferred over silent failure, per updated rule).

---

### Aggregate DataAccessObjects (Aggregate DAOs)

* Responsible for:

  * **Transaction orchestration**
  * **Aggregated reads (JOIN queries across multiple tables)**
* Aggregated reads:

  * Must map results to **flattened, purpose-built models**
  * Models should be tailored to the specific query/use-case
  * **Do NOT return raw table-shaped entities or relational graphs**
* For any transactional operation:

  * Must accept and use:

    * `IDbConnection`
    * `IDbTransaction`
  * Must **NOT create or open connections internally**
* Coordinate multiple DAO calls within a transaction boundary.

---

### Single-table DataAccessObjects (DAOs)

* Responsible for:

  * Single-table reads and writes only
* No business logic
* No transaction orchestration
* Use provided `IDbConnection` (and transaction if supplied)

---

## Transaction Rules

* Transactions are controlled at the **Aggregate DAO level**
* When operating inside a transaction:

  * BOTH `IDbConnection` and `IDbTransaction` **must be passed explicitly**
  * DAOs must **only use the provided connection**, never instantiate their own

---

## Database Conventions

* **SQL Server + Dapper** (No EF Core)
* Always use **explicit column lists** — never `SELECT *`
* Updates:

  * Use `COALESCE(@Value, ColumnName)` for optional fields
* Status fields:

  * `NVARCHAR`
  * Backed by `const string` values in static classes
* Timestamps:

  * `DATETIME` stored as **UTC**
* Nullable reference types:

  * **Enabled across all projects**

---

## Error Handling Strategy

* Repositories are the **primary boundary for business errors**
* Exceptions:

  * May be thrown by repositories
  * Must be logged
* Controllers:

  * Convert results (or exceptions) into `ResponseBase<T>`
  * Never expose raw exceptions

---

## Testing Standards

### Repository Tests

* All **public repository methods MUST be tested**
* Frameworks:

  * **xUnit**
  * **Moq**
* Naming convention:

  * `{RepositoryName}Tests`

---

### Aggregate DAO Tests

* All methods MUST be tested
* Frameworks:

  * **xUnit**
  * **Moq**
* Naming convention:

  * `{AggregateDaoName}Tests`

---

## Summary of Key Principles

* **Strict separation of concerns**
* **Repositories = brain of the system**
* **Aggregate DAOs = joins + transactions + flattened read models**
* **Single-table DAOs = pure data access**
* **No hidden connection management**
* **Explicit transactions only**
* **Test coverage is mandatory at orchestration layers**

## Blazor Binding Rule ##
* When a component needs both two-way binding and a post-change callback, use @bind-Value with @bind-Value:after, never @bind-Value with ValueChanged.
* The :after callback is parameterless — the bound field is already updated when it fires.
* @bind-Value + ValueChanged on the same component is a compile error. Use @bind-Value + @bind-Value:after instead; read the bound field directly in the callback.

