## Blazor UI Style Guide

> **Purpose:** Drop this file into your project context when asking Claude to build, edit, or review any Blazor page or component for the Tutor application. It encodes the exact patterns, tokens, and conventions used across the codebase so Claude can generate consistent, on-brand UI without guessing.

---

## 1. Tech Stack

| Layer | Technology |
|---|---|
| Framework | Blazor WebAssembly (.NET 9) |
| Component library | **MudBlazor** |
| Font | **Inter** (loaded from Google Fonts in each app's `index.html`; set as the font family of every MudBlazor typography group in `TutorTheme`) |
| Icons | **Material Icons** (via `<span class="material-icons">icon_name</span>`) |
| Auth | Microsoft Entra ID / MSAL (`[Authorize]` on every page) |
| HTTP | Injected `HttpClient` via typed service classes |
| Culture | `en-ZA` — affects date picker formatting |

---

## 2. Colour Tokens

All colours are CSS custom properties. **Never hardcode hex values for UI colour.** Always reference these tokens.

### Brand palette (taken from the Joyful Noise logo)

The logo is **pure black type on white** with a **six-colour pastel wave** running mint → sage → amber → coral → pink → orchid. Black and white are the base of the UI; the pastels are the accent. Hex values were sampled from the logo artwork.

| Logo element | Token | Hex |
|---|---|---|
| Wordmark, "Music Room", piano keys | `--c-text` | `#000000` |
| Logo background | `--c-bg-card` / `--c-surface` | `#ffffff` |
| Wave — mint | `--c-brand-mint` | `#93d4c7` |
| Wave — sage | `--c-brand-sage` | `#b9cda4` |
| Wave — amber | `--c-brand-amber` | `#f6b669` |
| Wave — coral | `--c-brand-coral` | `#f79b83` |
| Wave — pink (**main brand colour**) | `--c-brand-pink` = `--c-accent` | `#ef6ba3` |
| Wave — orchid | `--c-brand-orchid` | `#aa77b8` |

### Semantic tokens (always use these)

```css
/* Backgrounds */
--c-bg            /* page background: #f7f7f7 */
--c-bg-card       /* card/panel surface: #ffffff */
--c-bg-raised     /* slightly elevated surface (tag pills, etc.): #f0f0f0 */
--c-surface       /* white inner surface: #ffffff */

/* Text */
--c-text          /* primary body text: #000000 (the logo's black) */
--c-text-muted    /* secondary / label text: #5f5f5f */
--c-text-dim      /* de-emphasised / placeholder text: #8a8a8a (placeholders only, not for readable text) */

/* Structure */
--c-border        /* dividers, card outlines: #e4e4e4 */

/* Brand */
--c-accent        /* logo pink, the primary brand colour: #ef6ba3 — always carries black text/icons */
--c-accent-deep   /* pink darkened for text, icons and links: #c03276 (5.3:1 on white, 4.7:1 on the grey panel) — derived, not in the logo */
--c-brand-mint    /* #93d4c7 */
--c-brand-sage    /* #b9cda4 */
--c-brand-amber   /* #f6b669 */
--c-brand-coral   /* #f79b83 */
--c-brand-pink    /* #ef6ba3 (same as --c-accent) */
--c-brand-orchid  /* #aa77b8 */
--c-brand-gradient /* linear-gradient(90deg, mint, sage, amber, coral, pink, orchid) — the logo wave */

/* Semantic status (not brand colours — see rules below). Each is 4.5:1 or better on white, the page and the grey panel */
--c-success       /* green: confirmed / complete states: #2b7a2f */
--c-warn          /* amber: caution / outstanding / no price set: #a65700 */
--c-danger        /* red: errors / overdue / rejected: #c62828 */

/* Border radius */
--radius-sm       /* tight corners (pills, tags) */
--radius-md       /* medium (input-style containers) */
--radius-lg       /* cards and panels */
--radius          /* default (same as radius-lg) */
```

### Brand colour rules

1. **Pastels are fills, never text.** Put black (`--c-text`) on pastel fills: black on pink is 7.3:1, on amber 11.8:1, on mint 12.5:1, on orchid 6.0:1. White on any of them fails contrast (white on pink is only 2.9:1, on orchid 3.5:1). For pink text or links on a white surface or the page background use `--c-accent-deep`.
2. **Primary actions are pink**: `--c-accent` fill with a black label. One primary button per view. Secondary actions are outlined with `--c-text-muted`. MudBlazor's Secondary stays a **neutral grey** because the apps use `Color.Secondary` for muted captions and labels; a coloured Secondary would recolour every caption. Orchid is a category colour only.
3. **The spectrum is for identity and categories**: the logo strip, avatars, lesson-type tags, chart series, quarter markers. Use `--c-brand-gradient` at most once per screen (for example a 4px strip under the app bar, or on the login card).
4. **Never use the spectrum for status.** Success, warning and error stay `--c-success` / `--c-warn` / `--c-danger` (green / amber / red), so a pink or amber chip never reads as an alert. The graded score tiers in section 10 are the only exception.
5. **Contrast minimum**: text and icons need 4.5:1 against the surface they sit on (3:1 for text at 18px bold or larger).

### Status chip tokens

Status chips (pastel background + coloured text) are defined once in `theme.css` as `--chip-<status>-bg` and `--chip-<status>-fg` for: `scheduled`, `completed`, `forfeited`, `cancelled`, `rescheduled`, `pending`, `paid`, `overdue`, `void`. The `<StatusChip>` components pick the matching `.status-<status>` class. Each pair must stay at 4.5:1 or better (enforced by `ThemeChipContrastTests`).

### Where the theme lives (single source of truth)

All three apps (`Tutor.Web`, `Tutor.StudentPortal`, `Tutor.AccountHolderPortal`) share the `Tutor.Client.Theme` project. **Never define a colour inside an app.**

| File | Purpose |
|---|---|
| `BrandColors.cs` | The palette as constants |
| `TutorTheme.cs` | The MudBlazor theme (`TutorTheme.Light`), used by each `App.razor`, plus `ThemeAssets` logo paths |
| `wwwroot/theme.css` | The CSS tokens above, MudBlazor overrides, shared classes (`stats-card`, `quarter-card`, `aging-card*`, `countdown-badge`, `status-*`) |
| `wwwroot/images/` | `logo-full.png` (login cards), `logo-wave.png` (app bar mark) |

Each app's `index.html` links `_content/Tutor.Client.Theme/theme.css`. Tests in `Tutor.Client.Tests` keep the C# palette and the CSS tokens identical and enforce the contrast rules, so changing a colour means changing it in `BrandColors.cs` and `theme.css` together.

Quarter and stat-card colours follow the spectrum: Q1 mint, Q2 amber, Q3 coral, Q4 pink (`quarter-card--1..4`); dashboard tiles mint, amber, coral, pink (`stats-card`, `stats-card--2..4`).

### MudBlazor theme (`TutorTheme.Light`)

```csharp
PaletteLight = new PaletteLight
{
    Primary               = "#ef6ba3",   // logo pink — buttons, progress, checkboxes
    PrimaryContrastText   = "#000000",   // black on pink is 7.3:1; white on pink fails (2.9:1)
    Secondary             = "#5f5f5f",   // neutral grey — used for muted captions/labels (Color.Secondary)
    SecondaryContrastText = "#ffffff",
    Background            = "#f7f7f7",
    Surface               = "#ffffff",
    AppbarBackground      = "#ffffff",   // the logo sits on white
    AppbarText            = "#000000",
    TextPrimary           = "#000000",
    TextSecondary         = "#5f5f5f",
    Divider               = "#e4e4e4",
    TableLines            = "#e4e4e4",
    Success               = "#2b7a2f",
    Warning               = "#a65700",
    Error                 = "#c62828",
};

// Typography: Inter, applied to every MudBlazor typography group (Default, H1-H6, Subtitle1/2,
// Body1/2, Button, Caption, Overline) with FontFamily = ["Inter", "Segoe UI", "system-ui", "Roboto", "sans-serif"].
// Inter is loaded from Google Fonts in each app's index.html.
```

Because pink is too light to read as text, `theme.css` also remaps any MudBlazor `Color.Primary` used as **text, an icon, a text or outlined button, or the active tab** to `--c-accent-deep`. Filled buttons keep the pink fill with a black label.

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
│  hc-topbar  (logo + LoginDisplay)            │
├──────────────┬──────────────────────────────┤
│  hc-nav      │  hc-body                     │
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

All pages begin with a `<PageTitle>` tag: `<PageTitle>Page Name — Tutor</PageTitle>`

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
≥ 60%  → --c-brand-mint (teal)
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
| `<ScoreBadge Score="@n" />` | Numeric score indicator (mint/red) |
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

---

## UX & UI Architecture Guidelines

### Core Philosophy: Progressive Disclosure
- **Design for the 95% Path:** The default UI state must present only the mandatory elements required to complete the primary intent (e.g., Select Cost Centre → Execute Action).
- **Hide 5% Complexity via M3 Patterns:** Secondary or edge-case actions (e.g., historical overrides, ad-hoc adjustments) must be accessible within 1-2 interactions but never occupy primary screen real estate.
- **Zero Cognition Overhead:** Default selections must be smart and context-aware (e.g., pre-selecting the active pay period by default).

---

### Material Design 3 (M3) Component Standards

1. **Primary Actions & Main Views:**
   - Use M3 `FilledButton` for the main call to action (e.g., "Run Payroll").
   - Use M3 `Card` (Outlined or Elevated) to group primary controls.
   - Defaults must be pre-populated (e.g., current period pre-selected).

2. **Secondary Controls (5% Features):**
   - **Inline Progressive Disclosure:** Use M3 `ExpansionPanel` or collapsible sections titled "Advanced Options" or "Adjustments" for inline edge cases.
   - **Transient Context:** Use M3 `BottomSheet` (mobile) or `Dialog` (desktop) for transactional edge-case captures (e.g., "Add Ad-Hoc Payment").
   - **Overrides:** Use M3 `FilterChip` or inline `SegmentedButton` to switch contexts (e.g., changing pay period from "Current" to "Historical") rather than displaying persistent date inputs.

3. **Information Hierarchy & Visual Density:**
   - **Primary Surface:** High emphasis text, clear primary CTA, key status metrics.
   - **Secondary Surface:** Low-emphasis icons, subtle text buttons ("Change period...", "Add ad-hoc item"), and muted borders.
   - Do not display empty states or zero-value fields for edge cases unless explicitly opened by the user.

---

### Specific Domain Pattern: "Action with Overrides"
When generating views or components for operational workflows (e.g., Payroll, Invoicing, Data Processing), adhere to this exact structural hierarchy:

---

## UX Preview Mockups (used in Workflow Step 2)

Before any UI is built, Claude produces a static HTML mockup per screen or dialog for the developer to approve (see `claude.md`, Step 2).

- Self-contained single HTML file per screen, dummy data, no dependencies on the solution.
- Define the colour tokens from section 2 as CSS variables and use only `var(--c-*)`; no other hex values.
- Reproduce the shell (`hc-shell`, `hc-topbar`, `hc-nav`, `hc-body`), `page-header`, `detail-card`, dense tables, chips, and the loading/empty states exactly as described in this guide, using Inter and the typography scale in section 3.
- Follow the date, number, and null (`—`) formats in section 4.
- Include a simple state switcher (for example buttons or a dropdown) covering loading, empty, populated, error, and validation states, plus open/closed for dialogs.
- Show dialog widths, input density (`Variant.Outlined`, `Margin.Dense`), and row-level action menus as they will be built.
- Save to `docs/ux-previews/<feature>/`. Reference only: never referenced by the app and never treated as production code.
- The mockup only approximates MudBlazor; final visual verification happens on the real build in Workflow Step 7.

---

## Build Checklist (used in Workflow Step 3)

Claude self-checks UI work against this list while building. The independent review in Step 5 is limited to the Architectural Guide and does not cover UI.

- [ ] UI matches the mockups approved in Step 2; any deviation was raised with the developer
- [ ] Page has `@page`, `@attribute [Authorize]`, and `<PageTitle>`
- [ ] Standard page header; loading, empty, and error states handled
- [ ] No hardcoded hex colours; `var(--c-*)` tokens only
- [ ] Dates `yyyy/MM/dd`, timestamps `yyyy/MM/dd HH:mm`, null shown as `—`
- [ ] Tables `Dense Hover Elevation="0"`; inputs `Variant.Outlined`
- [ ] Dialogs use `@ref` + `ShowAsync/CloseAsync`; no `IsVisible`
- [ ] `@bind-Value:after`, never `ValueChanged` alongside `@bind-Value`
- [ ] Delete actions use `Color.Error` with confirmation
- [ ] No business logic in pages; services call the API
- [ ] Blazor services TDD'd where they do not call external APIs
