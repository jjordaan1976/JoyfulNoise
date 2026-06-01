## Blazor UI Style Guide

> **Purpose:** Drop this file into your project context when asking Claude to build, edit, or review any Blazor page or component for the Stride Sales application. It encodes the exact patterns, tokens, and conventions used throughout the codebase so Claude can generate consistent, on-brand UI without guessing.

---

## 1. Tech Stack

| Layer | Technology |
|---|---|
| Framework | Blazor WebAssembly (.NET 9) |
| Component library | **MudBlazor** |
| Icons | **Material Icons** (via `<span class="material-icons">icon_name</span>`) |
| Auth | Microsoft Entra ID / MSAL (`[Authorize]` on every page) |
| HTTP | Injected `HttpClient` via typed service classes |
| Culture | `en-ZA` — affects date picker formatting |

---

## 2. Colour Tokens

All colours are CSS custom properties. **Never hardcode hex values for UI colour.** Always reference these tokens.

### Semantic tokens (always use these)

```css
/* Backgrounds */
--c-bg            /* page background: #f0f2f5 */
--c-bg-card       /* card/panel surface */
--c-bg-raised     /* slightly elevated surface (tag pills, etc.) */
--c-surface       /* white inner surface */

/* Text */
--c-text          /* primary body text: #3a3a3a */
--c-text-muted    /* secondary / label text: #78797a */
--c-text-dim      /* de-emphasised / placeholder text */

/* Structure */
--c-border        /* dividers, card outlines: #dfe1e3 */

/* Brand / Semantic */
--c-accent        /* gold primary brand: #c9a84c */
--c-success       /* green: confirmed / complete states */
--c-warn          /* amber: caution / no price set */
--c-danger        /* red: errors / rejected */

/* Border radius */
--radius-sm       /* tight corners (pills, tags) */
--radius-md       /* medium (input-style containers) */
--radius-lg       /* cards and panels */
--radius          /* default (same as radius-lg) */
```

### Status chip tokens

Used for lead/proposal/task status badges (small coloured pills):

```css
--chip-qualified       /* bg */   --chip-qualified-text
--chip-escalated       /* bg */   --chip-escalated-text
--chip-rejected        /* bg */   --chip-rejected-text
--chip-scoring         /* bg */   --chip-scoring-text
```

### MudBlazor theme (defined in `MainLayout.razor`)

```csharp
PaletteLight = new PaletteLight
{
    Primary              = "#c9a84c",   // gold — buttons, progress, checkboxes
    PrimaryContrastText  = "#ffffff",
    Secondary            = "#78797a",
    Background           = "#f0f2f5",
    Surface              = "#ffffff",
    TextPrimary          = "#3a3a3a",
    TextSecondary        = "#78797a",
    Divider              = "#dfe1e3",
    TableLines           = "#dfe1e3",
};
```

---

## 3. Typography Scale

| Use | Size | Weight | Colour |
|---|---|---|---|
| Section label / eyebrow | `11px` | `700` | `--c-text-muted` |
| Section label style | `text-transform:uppercase; letter-spacing:.06em` | | |
| Body / table cell | `13px` | normal | `--c-text` |
| Secondary / metadata | `12px` | normal | `--c-text-muted` |
| Micro / timestamps | `10–11px` | normal | `--c-text-muted` |
| Card title | `11px` | `700` | `--c-text-muted` (uppercase) |
| Stat value | `2rem+` | `700–800` | semantic colour |
| Currency value | `13–15px` | `600–700` | `--c-text` |

---

## 4. Date & Number Formatting

- **All dates:** `yyyy/MM/dd` (South African format — do NOT use `dd/MM/yyyy` or `MM/dd/yyyy`)
- **Timestamps:** `yyyy/MM/dd HH:mm`
- **Month-only:** `yyyy/MM`
- **Currency:** prefix with `R` (not `R `) — e.g. `R 1 234.56` → use `.ToString("N2")` in C#
- **All `MudDatePicker`s must include:** `DateFormat="yyyy/MM/dd"`
- **Percentages:** `{value:F0}%`
- **Null/missing values:** render as `—` (em dash), never `null`, `""`, or `0`

---

## 5. Shell Layout

```
┌─────────────────────────────────────────────┐
│  stride-topbar  (logo + LoginDisplay)        │
├──────────────┬──────────────────────────────┤
│  stride-nav  │  stride-body                 │
│  (sidebar)   │  (@Body — page content)      │
└──────────────┴──────────────────────────────┘
```

CSS class names:
- `.hc-shell` — outer grid container
- `.hc-topbar` — top bar (`header`)
- `.hc-nav` — left sidebar (`nav`)
- `.hc-body` — main content area (`main`)
- `.hc-nav__section` — section divider label in sidebar
- `.hc-nav__item` — nav link item (with `.active` modifier)
- `.hc-nav__icon` — material icon inside nav item

All pages begin with a `<PageTitle>` tag: `<PageTitle>Page Name — Stride Sales</PageTitle>`

---

## 6. Page Header Pattern

Every page starts with this header block:

```razor
<div class="page-header">
    <div class="page-header__title">Page Title</div>
    <div class="page-header__sub">Short descriptive subtitle</div>
</div>
```

For pages with actions (e.g. a Create button), wrap title and button in a flex row:

```razor
<div class="page-header">
    <div style="display:flex;align-items:center;justify-content:space-between">
        <div>
            <div class="page-header__title">Page Title</div>
            <div class="page-header__sub">Subtitle</div>
        </div>
        <MudButton Variant="Variant.Filled" OnClick="OpenCreate">Create</MudButton>
    </div>
</div>
```

For detail pages with a back link:

```razor
<div class="page-header">
    <div style="font-size:12px;color:var(--c-text-muted);margin-bottom:4px;cursor:pointer"
         @onclick="@(() => Nav.NavigateTo("/leads"))">
        ← Back to Leads
    </div>
    <div class="page-header__title">Detail Page Title</div>
    <div class="page-header__sub">@_entity.Name</div>
</div>
```

---

## 7. Loading, Empty & Error States

### Loading spinner

```razor
<div class="loading-pulse">
    <MudProgressCircular Indeterminate Size="Size.Small" Color="Color.Default" />
    <span>Loading…</span>
</div>
```

### Empty state (no data)

```razor
<div class="empty-state">
    <div class="empty-state__icon material-icons">history</div>
    <div class="empty-state__title">Nothing here yet</div>
    <div class="empty-state__sub">Descriptive message about what to do next</div>
</div>
```

### State guard pattern (every page with async data)

```razor
@if (_loading)
{
    <div class="loading-pulse">...</div>
}
else if (_items is null || !_items.Any())
{
    <div class="empty-state">...</div>
}
else
{
    <!-- actual content -->
}
```

---

## 8. Card / Panel Pattern

Cards are the core layout unit. Every card uses this structure:

```razor
<div style="background:var(--c-bg-card);border:1px solid var(--c-border);
            border-radius:var(--radius-lg);padding:20px 24px">
    <!-- content -->
</div>
```

For titled cards (`.detail-card`):

```razor
<div class="detail-card">
    <div class="detail-card__title">Section Title</div>
    <!-- rows of content -->
</div>
```

CSS for `detail-card`:
```css
.detail-card {
    background: var(--c-surface);
    border: 1px solid var(--c-border);
    border-radius: var(--radius);
    padding: 16px;
}
.detail-card__title {
    font-size: 11px;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: .06em;
    color: var(--c-text-muted);
    margin-bottom: 12px;
}
```

### Detail row (key-value pair inside a card)

```razor
<div class="detail-row">
    <span class="detail-label">Label</span>
    <span class="detail-value">Value</span>
</div>
```

CSS:
```css
.detail-row {
    display: flex; gap: 12px; padding: 6px 0;
    border-bottom: 1px solid var(--c-border);
    font-size: 13px; align-items: flex-start;
}
.detail-row:last-child { border-bottom: none; }
.detail-label { flex: 0 0 160px; color: var(--c-text-muted); font-size: 12px; }
.detail-value { flex: 1; color: var(--c-text); word-break: break-word; }
```

---

## 9. MudBlazor Component Conventions

### Tables

```razor
<MudTable Items="_items" Dense Hover Elevation="0"
          Style="background:var(--c-bg-card);border:1px solid var(--c-border);
                 border-radius:var(--radius-lg);overflow:hidden">
    <HeaderContent>
        <MudTh>Column</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd>@context.Property</MudTd>
    </RowTemplate>
</MudTable>
```

- Always use `Dense` and `Hover`.
- Always use `Elevation="0"` (shadow comes from the border/card, not MudBlazor's shadow).
- Wrap in the card style (`background:var(--c-bg-card)...`) so the table sits flush inside it.
- Null/missing cells: render `@(context.Value ?? "—")`.
- Currency cells: add `Class="currency"` and prefix with `R`.
- Date cells: `@context.Date.ToString("yyyy/MM/dd")`, style with `font-size:12px;color:var(--c-text-muted)`.

### Dialogs

- **Never use `IsVisible`.** Use `@ref` and call `.ShowAsync()` / `.CloseAsync()` explicitly.
- Always declare `DialogOptions`:

```csharp
private readonly DialogOptions _dialogOpts = new()
    { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
```

- Expose a public `OpenAsync(...)` method on dialog components.
- Raise `[Parameter] EventCallback OnCreated` (or `OnSaved`) when an operation completes successfully.
- Inside dialogs, use `MudGrid` with `Spacing="2"` for form layout.
- Dialog actions pattern:

```razor
<DialogActions>
    @if (_isEdit)
    {
        <MudButton Color="Color.Error" OnClick="Delete" Disabled="_saving">Delete</MudButton>
        <div style="flex:1"></div>
    }
    <MudButton OnClick="@(() => _dialog!.CloseAsync())">Cancel</MudButton>
    <MudButton Variant="Variant.Filled" OnClick="Save" Disabled="_saving">
        @(_saving ? "Saving…" : "Save")
    </MudButton>
</DialogActions>
```

### Inputs — always use `Variant.Outlined`

```razor
<MudTextField  @bind-Value="_model.Name"    Label="Name"    Variant="Variant.Outlined" />
<MudNumericField @bind-Value="_model.Count" Label="Count"   Variant="Variant.Outlined" Min="0" />
<MudSelect     @bind-Value="_model.Status"  Label="Status"  Variant="Variant.Outlined">
    <MudSelectItem Value="@("Active")">Active</MudSelectItem>
</MudSelect>
<MudDatePicker @bind-Date="_model.Date"     Label="Date"    Variant="Variant.Outlined"
               DateFormat="yyyy/MM/dd" Clearable />
<MudTextArea   @bind-Value="_model.Notes"   Label="Notes"   Variant="Variant.Outlined"
               Lines="3" AutoGrow />
```

### Two-way binding callbacks

**Always** use `:after` — never `ValueChanged` on the same component as `@bind-Value`:

```razor
<!-- CORRECT -->
<MudNumericField @bind-Value="_headcount" @bind-Value:after="OnHeadcountChanged" ... />

<!-- WRONG — compile error -->
<MudNumericField @bind-Value="_headcount" ValueChanged="OnHeadcountChanged" ... />
```

The `:after` callback is parameterless; read the already-updated field directly.

### Snackbar feedback

```csharp
// Success
Snackbar.Add("Lead created.", Severity.Success);

// Error
Snackbar.Add(r?.ReturnMessage ?? "Failed to create lead.", Severity.Error);

// Warning
Snackbar.Add("Please select an effective date.", Severity.Warning);
```

---

## 10. Status Chip Components

Two reusable components exist for status badges:

### `<StatusChip Status="@status" />`

Renders a coloured pill for proposal/lead status. Uses `--chip-*` tokens.

### `<RecommendationChip Score="@score" Max="@max" />`

Score-to-colour mapping (also used in `LeadScoreHelpers.cs`):
```
≥ 80%  → --c-success (green)
≥ 60%  → --c-accent  (gold)
≥ 40%  → --c-warn    (amber)
< 40%  → --c-danger  (red)
```

### Inline status pill (when a named component isn't used)

```razor
<span style="font-size:10px;font-weight:700;padding:2px 8px;border-radius:4px;
             background:var(--chip-qualified);color:var(--chip-qualified-text)">
    @status
</span>
```

### Score pill (`.score-pill`)

```razor
<span class="score-pill"
      style="background:@LeadScoreHelpers.ScorePillBg(Score, Max);
             color:@LeadScoreHelpers.ScorePillFg(Score, Max)">
    @Score / @Max
</span>
```

---

## 11. Section Label / Eyebrow Pattern

Used throughout for grouping sections inside cards or pages:

```razor
<div style="font-size:11px;font-weight:700;color:var(--c-text-muted);
            text-transform:uppercase;letter-spacing:.06em;margin-bottom:8px">
    Section Name
</div>
```

---

## 12. Stat / KPI Cards

Used on the Dashboard:

```razor
<div class="stat-card">
    <div class="stat-card__label">Metric Name</div>
    <div class="stat-card__value">
        42<small style="font-size:1rem;color:var(--c-text-muted)">unit</small>
    </div>
</div>
```

---

## 13. Page `@code` Structure

Follow this order inside `@code` blocks:

```csharp
// 1. Injected services (match @inject declarations)
// 2. Parameters ([Parameter] properties)
// 3. Private state fields (bools like _loading, _saving first; then data fields)
// 4. Computed properties (private string X => ...)
// 5. Lifecycle (OnInitializedAsync, OnParametersSetAsync)
// 6. Data loading methods (LoadXxx)
// 7. Event handlers (OpenXxx, CloseXxx, OnXxxChanged)
// 8. Submit/action methods (Create, Update, Delete, Save)
// 9. Helpers / formatters (FmtCcy, etc.)
```

### Standard page init pattern

```csharp
private bool _loading = true;
private List<MyModel> _items = [];

protected override async Task OnInitializedAsync()
{
    _loading = true;
    var r = await MySvc.GetAllAsync();
    _items = r?.ReturnCode == 0 ? r.Data?.ToList() ?? [] : [];
    _loading = false;
}
```

### Parallel data loading (use when page needs multiple independent calls)

```csharp
protected override async Task OnInitializedAsync()
{
    _loading = true;
    await Task.WhenAll(LoadLeads(), LoadUsers(), LoadDevices());
    _loading = false;
}
```

### Save/submit pattern

```csharp
private bool _saving = false;

private async Task Save()
{
    _saving = true;
    var r = await MySvc.CreateAsync(_form);
    _saving = false;

    if (r?.ReturnCode == 0)
    {
        Snackbar.Add("Created.", Severity.Success);
        await _dialog!.CloseAsync();
        await OnCreated.InvokeAsync();
    }
    else Snackbar.Add(r?.ReturnMessage ?? "Failed.", Severity.Error);
}
```

---

## 14. Service Layer Pattern

Every page injects a typed service. Services follow this shape:

```csharp
// GET (returns data directly, empty on failure)
public async Task<IEnumerable<MyModel>> GetAllAsync()
{
    try
    {
        var r = await http.GetFromJsonAsync<ResponseBase<IEnumerable<MyModel>>>("MyEntity/GetAll");
        return r?.ReturnCode == 0 && r.Data is not null ? r.Data : [];
    }
    catch (Exception ex) { Console.Error.WriteLine($"MyService.GetAllAsync: {ex.Message}"); return []; }
}

// POST/PUT (returns ResponseBase for status checking)
public async Task<ResponseBase<int>?> CreateAsync(MyModel model)
{
    try
    {
        var resp = await http.PostAsJsonAsync("MyEntity/Create", model);
        return await resp.Content.ReadFromJsonAsync<ResponseBase<int>>();
    }
    catch (Exception ex) { Console.Error.WriteLine($"MyService.CreateAsync: {ex.Message}"); return null; }
}
```

Pages always check: `if (r?.ReturnCode == 0)` before using `r.Data`.

---

## 15. Authorisation on Pages

Every page must have:

```razor
@page "/my-route"
@attribute [Authorize]
```

For function-level guards (using `FunctionAuthorisationAttribute` or `RequireFunctionAttribute`), those are applied on the **API controller**, not the Blazor page. The page uses `CurrentUserContext` to check permissions in the UI:

```csharp
@inject CurrentUserContext CurrentUser

// In code:
if (!CurrentUser.HasFunction("FunctionKey")) { /* hide button */ }
```

---

## 16. Navigation

```csharp
@inject NavigationManager Nav

Nav.NavigateTo("/leads");
Nav.NavigateTo($"/leads/{leadId}");

// Back link
<div style="font-size:12px;color:var(--c-text-muted);cursor:pointer"
     @onclick="@(() => Nav.NavigateTo("/leads"))">← Back to Leads</div>
```

---

## 17. Common Reusable Components

| Component | Usage |
|---|---|
| `<StatusChip Status="@s" />` | Coloured proposal/lead/task status pill |
| `<RecommendationChip Score="@n" Max="@m" />` | Score percentage chip |
| `<ScoreBadge Score="@n" />` | Numeric score indicator (gold/red) |
| `<LeadScoreSummaryCard Title="@t" Score="@n" Max="@m" CompletedAt="@dt" />` | Score card with progress bar |
| `<LeadScoreRow Label="@l" Score="@n" Max="@m" />` | Single key-score row |
| `<LeadCompletionBadge Done="@bool" Score="@n" Max="@m" />` | Tab completion indicator |
| `<LeadBoolRow Label="@l" Value="@bool" />` | Label + yes/no row |
| `<LeadQaBlock Label="@l" Answer="@a" Score="@n" />` | Q&A display block |
| `<NavItem Href="@h" Icon="@i" Label="@l" ExactMatch />` | Sidebar nav link |

---

## 18. Key Don'ts

- **Don't** use `IsVisible` on dialogs — use `@ref` + `.ShowAsync()`.
- **Don't** use `ValueChanged` alongside `@bind-Value` — use `@bind-Value:after`.
- **Don't** hardcode hex colours — use `var(--c-*)` tokens.
- **Don't** use any date format other than `yyyy/MM/dd` and `yyyy/MM/dd HH:mm`.
- **Don't** render null/missing as `""` or `0` — use `"—"` (em dash).
- **Don't** access `HttpContext` directly in Blazor pages — use injected services.
- **Don't** put business logic in pages — pages call services, services call the API.
- **Don't** use `MudElevation` other than `Elevation="0"` on tables.
- **Don't** style MudBlazor inputs with anything other than `Variant.Outlined`.

UI/UX & Blazor Development Principles
1. Visual Hierarchy & Density
Information Density: Prioritize high-density layouts. Use Dense="true" on MudTable, MudList, and MudTreeView.

Input Precision: Use Variant="Variant.Outlined" and Margin="Margin.Dense" for all MudTextField, MudSelect, and MudNumericField components to match a professional SaaS aesthetic.

Dialog Economy: Constrain MudDialog width using MaxWidth. Avoid full-width dialogs for simple forms. Group related numeric inputs horizontally using MudGrid to reduce vertical scrolling.

2. Functional Design Patterns
Relationship Mapping: When displaying "Source to Target" relationships (e.g., Substitution Rules), use visual cues like MudStack with directional icons (ArrowForward) rather than just text columns.

Action Clarity: Use MudMenu (kebab/three-dot menu) for row-level actions to reduce visual noise in tables. Primary actions (Save/Update) must be Color="Color.Primary", while destructive actions (Delete) must be Color="Color.Error" and require a MudMessageBox confirmation.

Semantic Status: Use MudChip with semantic colors to represent status (e.g., Color.Success for Active, Color.Error for Expired, Color.Default for Permanent/Never).

3. Component Architecture (Blazor/MudBlazor)
Clean Razor Files: Keep logic out of .razor files. Use code-behind files or injected services.

Validation: Use FluentValidation for complex form logic. Ensure "at least one" or "conditional" requirements are handled in the validator, not via manual if statements in the UI.

Performance: Use Virtualize="true" for MudSelect or MudAutocomplete when data sets exceed 50 items.

DRY Dialogs: If multiple dialogs share 80% of the same fields (e.g., Employee Rules), refactor into a single parameterised component.

4. Direct Communication Style
Labels over Help Text: Use clear, technical labels. Avoid excessive gray sub-text; use Adornment icons with Tooltips for context where space is at a premium.

Empty States: Never show an empty table without a MudAlert or a descriptive "No records found" state.