### Feature: Brand Theme (Joyful Noise logo palette)

#### Change History
> **Policy Note:** Do not retroactively backfill change history for historical edits made prior to this section's introduction. Record entries only for changes made from [Current Date / Effective Date] forward. Focus records on the *reason for the change*, not trivial author details.

| Date | Version | Reason for Change | Impacted Components |
| :--- | :--- | :--- | :--- |
| 2026-10-09 | v1.0 | Re-theme all three apps from the old gold look to the Joyful Noise logo palette, use Inter, show the logo, and put every colour in one shared place so the apps cannot drift apart. | `Tutor.Client.Theme` (new), `App.razor`, `index.html`, layouts, login pages and seven pages in the three apps, `Tutor.Client.Tests`, `UI_SKILL.md` |

---

#### 1. Objectives & Context
- **Feature Objectives:** The management app, Student Portal and Account Holder Portal look like the Joyful Noise brand and like each other, and their colours stay readable.
- **Scope:**
  - **In-Scope:** The palette sampled from the logo; the Inter font; the logo on the login cards and a small wave mark in the app bar; one shared stylesheet and one shared MudBlazor theme; one set of status chip colours for all three apps; automated contrast and consistency tests; the colour section of `UI_SKILL.md`.
  - **Out-of-Scope:** Dark mode. A vector (SVG) or higher-resolution logo. Changing the app name from "Tutor". The date format difference between `UI_SKILL.md` (`yyyy/MM/dd`) and the pages (`2026 Oct 09`). A leftover Google sign-in script in `Tutor.Web/wwwroot/index.html` (unused since Gmail login was removed).
- **Related Features & Systems:** `features_description_authentication.md` (the login pages); `UI_SKILL.md` section 2; approved mockups in `docs/ux-previews/rebrand/`.

#### 2. Dependencies
- **People / Stakeholders:**
  - **Feature Owner / PO:** Johan Jordaan
  - **Domain Expert / Sign-off:** To be confirmed
- **Services & APIs:**
  - **Internal Services:** None.
  - **External Services:** Google Fonts (Inter), loaded from each app's `index.html`.
- **Packages & Libraries:**
  - **NuGet / Third-Party Dependencies:** MudBlazor 7.15.0, xUnit, Moq.

#### 3. Business Rules
- **[BR-01]:** Anything filled with the brand pink carries a **black** label. White on pink is only 2.9:1.
- **[BR-02]:** Pink used as text, an icon, a text or outlined button, or the active tab uses the **deep pink** (`#c03276`) so it stays readable.
- **[BR-03]:** The logo spectrum (mint, sage, amber, coral, pink, orchid) is for identity and categories only: dashboard tiles (mint, amber, coral, pink) and quarters (Q1 mint, Q2 amber, Q3 coral, Q4 pink).
- **[BR-04]:** Status is always green, amber or red (`#2b7a2f`, `#a65700`, `#c62828`), never a spectrum colour. Every text colour reaches 4.5:1 on white, the page background and the grey inner panels.
- **[BR-05]:** MudBlazor's Secondary colour stays a neutral grey (`#5f5f5f`), because the apps use it for every muted caption and label.
- **[BR-06]:** Colours are defined only in `Tutor.Client.Theme`. No app defines its own.
- **[BR-07]:** Status chips look the same in all three apps. As a result, the Student Portal's Cancelled and Forfeited chips now match the management app (Cancelled amber, Forfeited red); they were previously reversed.

#### 4. Technical Scope & Guidance
- **Technical Strategy & Patterns:** A small Razor class library, `Tutor.Client.Theme`, holds `BrandColors.cs` (the palette), `TutorTheme.cs` (the MudBlazor theme used by each `App.razor`), `wwwroot/theme.css` (CSS tokens, MudBlazor overrides, shared classes) and the logo images. Pages refer to colours with `var(--c-*)` tokens instead of hex values. A global rule maps MudBlazor's `Color.Primary` text, icons, text/outlined buttons and active tabs to the deep pink, so the roughly 90 existing `Color.Primary` uses did not need editing.
- **Validators:** None.
- **Repositories & DAOs:** None. There is no API, database or behaviour change.
- **Blazor Services:** None created. The theme is static data, so it is guarded by tests in `Tutor.Client.Tests`: `ThemeContrastTests` (every text-on-fill pairing meets 4.5:1), `ThemeChipContrastTests` (all nine status chips), `ThemeParityTests` (the C# palette equals the CSS tokens; Inter is the font). Markup and CSS are presentation and are checked in the browser.
- **Known follow-ups (not in this change):** the date format difference; a vector logo; the unused Google sign-in script.

#### 5. Key Functional & Behavioural Requirements
- [ ] The login screen of all three apps shows the full logo, a pink **Send OTP** button with a black label, and a plain light background (no purple gradient).
- [ ] The app bar is white with the wave mark and the thin colour strip along its bottom edge, in all three apps.
- [ ] Management dashboard: the four tiles have mint, amber, coral and pink left edges; the active navigation item has a pink tint; icons are deep pink.
- [ ] New Bundle dialog: **Save Bundle** is pink with a black label; in Prorata from July the quarter cards are Q3 coral and Q4 pink.
- [ ] Student page and bundle detail: quarter cards are coloured by quarter number.
- [ ] Invoices: every status chip is readable; the row **Paid** button and **Record Payment** are legible; outstanding amounts are dark amber.
- [ ] Account statement: aging cards have green, amber, light red and red edges; the "Total outstanding" banner is readable.
- [ ] Student Portal: the next-lesson card has a pink left edge; the countdown badge is pink with black text.
- [ ] All text is set in Inter (check in the browser's developer tools).
- [ ] After the first deployment, do a hard refresh (Ctrl+F5): browsers can keep the old `index.html`, which shows the old look until refreshed.

---

#### 6. User Guide
> **Audience:** Application users. This section is served by the in-app Help chat.
> **Constraint:** Plain UI language only — no class, method, table, or field names.

- **Where to find it:** Every screen of the management app, the Student Portal and the Account Holder Portal.
- **What changed:** The apps now use the colours of the Joyful Noise logo: black text on a light background, with pink for main buttons and highlights. The logo appears on the sign-in screen, and a small version of it sits in the top bar of every screen. The text style is now Inter.
- **How to tell what a colour means:** Green means paid or complete, amber means pending or outstanding, and red means overdue or an error. The rainbow colours (mint, amber, coral, pink) only tell things apart, for example the four quarters of a bundle. They never mean "good" or "bad".
- **What the screen enforces:** Pink buttons always have black writing so they stay easy to read.
- **Common questions:**
  - *The screen still looks gold after the update.* Refresh your browser page with Ctrl+F5 once.
  - *Why is "Outstanding" in a dark orange?* It is darker than before so that it is easy to read on the grey panel.
  - *Why are the Cancelled and Forfeited labels different colours in the Student Portal?* They now match the management app: Cancelled is amber and Forfeited is red.
