# Development Workflow (Mandatory)

Every feature or change follows these eight steps, in order. Steps marked **AI** are carried out by Claude; steps marked **HUMAN** are carried out by the developer. Claude must not skip a step, reorder steps, or perform a HUMAN step on the developer's behalf.

| # | Step | Owner | Summary |
|---|---|---|---|
| 1 | Prompt the model | AI | Developer describes the feature or change required |
| 2 | Review the plan | HUMAN | Resolve questions and confirm the proposed approach |
| 3 | Build with TDD | AI | Code to the Architectural Guide, tests first |
| 4 | Document the feature | AI | Create/update feature documentation as part of delivery |
| 5 | Independent model review | AI | Check alignment with the Architectural Guide |
| 6 | Address the gaps | AI | Correct every issue identified in review |
| 7 | Test the system | HUMAN | Developer validates behaviour end to end |
| 8 | Review, commit & PR | HUMAN | Developer reviews the code, commits and opens a PR |

## Guides That Drive the Workflow

| Guide | Used in step(s) |
|---|---|
| `Architectural_Guide.md` — layering, transactions, DB conventions, error handling, testing standards | 2, 3, 5 |
| `UI_SKILL.md` — Blazor/MudBlazor style, tokens, formatting | 2, 3 |
| This file — TDD rules, feature documentation template, Definition of Done | 3, 4, 6 |

Where guides conflict, the Architectural Guide wins for server-side code and `UI_SKILL.md` wins for UI.

---

## Step 1 — Prompt the model (AI)

- Claude reads the developer's request and the three guides before doing anything else.
- Claude explores the existing code relevant to the request (similar features, existing DAOs, repositories, pages) so the plan matches established patterns.

## Step 2 — Review the plan (HUMAN gate)

**Claude must not write any test or production code until the developer approves the plan.**

Claude produces a plan and presents it for review. The plan must contain:

1. **Understanding** — the request restated in one or two sentences.
2. **Open questions** — every ambiguity, assumption, or missing business rule. Ask rather than guess. Claude stops and waits for answers.
3. **Proposed approach**, structured by layer:
   - Controllers / endpoints (route, verb, request/response models)
   - Repositories (use-cases, business rules, validation)
   - Aggregate DAOs (transactions, JOIN read models)
   - Single-table DAOs (SQL operations)
   - Validators
   - Database migration script(s) (next number in `Database/`)
   - Blazor pages, dialogs, services, components (reuse of existing components noted)
4. **Test plan** — the failing tests Claude intends to write first, grouped by component, and which components are excluded from TDD and why (see TDD Scope).
5. **Impact** — existing files, tables, or features affected.
6. **Feature document** — the `features_description_<feature>.md` to be created or updated.
7. **UX preview** (required for any feature with UI; omit only if there is no UI) — see below.

### UX preview (features with UI)

The developer approves how each screen looks and behaves **before** any code is written.

- **Screen inventory** — list every screen, dialog, and state: loading, empty, error, populated, and validation failures.
- **Static HTML mockup** — for each screen or dialog, Claude builds a throwaway, self-contained HTML mockup with dummy data, following `UI_SKILL.md` (CSS tokens, shell layout, page header, cards, dense tables, chips, empty/loading states, date and number formats). States are switchable in the mockup (for example with simple toggles) so each can be reviewed.
- **Reference only** — mockups are never production code and are never wired into the solution. Save them to `docs/ux-previews/<feature>/` and show them to the developer.
- **Approved with the plan** — the developer approves the mockups together with the plan. UX changes are treated as plan revisions: Claude updates the mockup and the plan, then confirms again.
- **Built to match** — in Step 3 the real Razor pages are built to match the approved mockups. Any deviation goes back to the developer rather than being decided silently.
- **Not a substitute for Step 7** — the mockup only approximates MudBlazor rendering, so final visual checks happen on the real build.

Claude then asks the developer to confirm. Only an explicit approval of the plan **and** the UX preview moves to Step 3. If the developer changes either, Claude updates it and confirms again.

## Step 3 — Build with TDD (AI)

- Follow the TDD Process below (red → green → refactor) for every component in TDD scope.
- Code strictly to `Architectural_Guide.md` and, for UI, `UI_SKILL.md`; UI must match the mockups approved in Step 2.
- Build in the layer order: Models/Interfaces → Single-table DAOs → Aggregate DAOs → Repositories → Controllers → Blazor services → Blazor UI.
- Add any schema change as the next numbered script in `Database/`. Never modify an already-merged script.
- Run the test suite regularly; all tests must be green before leaving this step.
- If implementation reveals the approved plan is wrong or incomplete, Claude stops, explains, and returns to Step 2 for the affected part. It does not silently change approach.

## Step 4 — Document the feature (AI)

- Create or update `features_description_<feature>.md` using the template in this file.
- Complete every section, including the Change History entry (reason for change, impacted components) and the plain-language User Guide.
- Documentation is written as part of delivery, not deferred.

## Step 5 — Independent model review (AI)

**The purpose of Step 5 is to check that the change is aligned with `Architectural_Guide.md`.**

The review must be independent: it is performed by a **separate reviewer** (a fresh subagent with no memory of the build, ideally a different model) rather than by the context that wrote the code. Claude should launch the reviewer, give it the guides, the approved plan, the list of changed files, and the feature document, and ask it to report only. The reviewer does **not** modify code.

**Scope of the review is limited to alignment with `Architectural_Guide.md`.** The reviewer checks every changed file against the Architecture Rules, Transaction Rules, Database Conventions, Error Handling Strategy, DI Registration and Testing Standards in that guide. It reports findings with file/line references, the guide rule violated, and severity (blocker / should-fix / nit).

The reviewer does **not** review UI style, feature documentation, plan conformance, or general code quality, and does not suggest changes outside the Architectural Guide. Those are covered by Step 3 (UI guide, TDD) and Step 4 (documentation), and by the developer in Steps 7 and 8.

## Step 6 — Address the gaps (AI)

- Claude fixes **every** issue the reviewer raised (blockers and should-fix are mandatory; nits are fixed unless there is a stated reason not to).
- Any fix to behaviour follows TDD (failing test first).
- Claude updates the feature document where the fixes change it, then re-runs the full test suite.
- If a finding is disputed or cannot be fixed, Claude lists it explicitly for the developer rather than dropping it.
- If blockers were found, Claude repeats Step 5 on the changed areas.

## Step 7 — Test the system (HUMAN)

- Claude hands over with a short summary: what was built, files changed, how to run it, migration scripts to apply (in order), and a manual test checklist drawn from the feature's Key Functional & Behavioural Requirements.
- Claude does **not** declare the work complete. The developer validates the behaviour end to end and reports back. Defects found go back to Step 6 (with a failing test first).

## Step 8 — Review, commit & PR (HUMAN)

- The developer reviews the code, commits, and opens the pull request.
- **Claude must not run `git commit`, `git push`, or open a PR** unless the developer explicitly asks for it in that conversation.
- On request, Claude may draft the commit message and PR description from the feature document.

---

# Test-Driven Development (TDD) Guide

## Scope

The following components **must be developed using TDD**:

### Required TDD Coverage
- **Validators**
- **Repositories**
- **AggregatedDataAccessObjects**

### Conditional TDD Coverage
- **Blazor Services**
  - MUST be TDD’d **if they do NOT call external APIs**
  - Services that call APIs are excluded from mandatory TDD (unless explicitly required)

---

## TDD Process

All development must strictly follow this cycle:

### 1. Write a Failing Test
- Create a unit test **before writing any implementation code**
- Use:
  - **xUnit**
  - **Moq** for mocking dependencies
- Only use **standard xUnit assertions**
- The test must fail initially

---

### 2. Make the Test Pass
- Write the **minimum amount of code required** to make the test pass
- Avoid over-engineering or premature optimisation

---

### 3. Refactor
- Improve:
  - **Readability**
  - **Performance**
- Ensure:
  - All tests remain **green**
  - No behavioural changes are introduced

---

## Rules & Principles

- **No production code without a failing test**
- **Keep tests simple and focused**
- **Mock external dependencies only**
- **Avoid testing implementation details — test behaviour**
- **Refactoring is mandatory, not optional**

---

## Tooling Standards

- Testing Framework: **xUnit**
- Mocking Library: **Moq**
- Assertions: **xUnit built-in assertions only**

---

## Do feature documenation following the below template

### Feature: [Feature Name]

#### Change History
> **Policy Note:** Do not retroactively backfill change history for historical edits made prior to this section's introduction. Record entries only for changes made from [Current Date / Effective Date] forward. Focus records on the *reason for the change*, not trivial author details.

| Date | Version | Reason for Change | Impacted Components |
| :--- | :--- | :--- | :--- |
| YYYY-MM-DD | v1.0 | Initial implementation specification | All |
| YYYY-MM-DD | v1.1 | Updated validation rule to handle edge case X | Validator Service |

---

#### 1. Objectives & Context
- **Feature Objectives:** What specific business outcome or user problem does this feature solve?
- **Scope:** 
  - **In-Scope:** [Explicitly defined boundaries]
  - **Out-of-Scope:** [Explicitly excluded elements]
- **Related Features & Systems:** [Links or references to existing systems/features]

#### 2. Dependencies
- **People / Stakeholders:**
  - **Feature Owner / PO:** [Name]
  - **Domain Expert / Sign-off:** [Name]
- **Services & APIs:**
  - **Internal Services:** [e.g., Auth Service, Legacy Stored Proc `sp_GetCustomerData`]
  - **External Services:** [e.g., Payment Gateway API, SendGrid]
- **Packages & Libraries:**
  - **NuGet / Third-Party Dependencies:** [e.g., FluentValidation v11.x, MediatR]

#### 3. Business Rules
*Domain rules and invariants that must be enforced regardless of interface or delivery mechanism.*
- **[BR-01]:** [e.g., A customer cannot place an order if account balance is over 90 days past due.]
- **[BR-02]:** [e.g., Discount percentages cannot exceed 15% without explicit approval.]

#### 4. Technical Scope & Guidance
- **Technical Strategy & Patterns:** [Notes on design patterns applied, e.g., Strangler Fig, CQRS, TDD boundary decisions]
- **Validators:** List of validator classes created/modified.
- **Repositories & DAOs:** Data access mechanisms and database components affected.
- **Blazor Services:** Services created/modified (note external API boundaries to justify TDD inclusion/exclusion).

#### 5. Key Functional & Behavioural Requirements
- [ ] Requirement 1 (e.g., "System must throw a validation error if email already exists prior to registration")
- [ ] Requirement 2 (e.g., "Must fetch user metadata via `UserAggregatedDAO` within a single round-trip")

---

#### 6. User Guide
> **Audience:** Application users. This section is served by the in-app Help chat.
> **Constraint:** Plain UI language only — no class, method, table, or field names.

- **Where to find it:** [Navigation path, e.g., "Pipeline → Proposals → New Proposal"]
- **How to [Common Task 1]:** Numbered steps using exact on-screen labels (buttons, fields, menus as the user sees them).
- **How to [Common Task 2]:** …
- **What the screen enforces:** Business rules as user-facing behavior (e.g., "The expected order month cannot be in the past").
- **Common questions:** 2–4 Q&A pairs for operational queries.

---

## Definition of Done

A task is only complete when:
- The plan, and the UX mockups where the feature has UI, were approved by the developer (Step 2) before any code was written
- The corresponding `features_description_<feature>.md` file has been created or updated to reflect the work piece
- All required components are developed using TDD
- An independent Architectural Guide review (Step 5) was run and every finding was fixed or explicitly escalated (Step 6)
- The full test suite is green
- The developer has been given the handover summary and manual test checklist (Step 7)
- Commit and PR are left to the developer (Step 8)
- Tests:
  - Are written first
  - Are passing
  - Cover expected behaviour
- Code has been refactored for clarity and performance
- No unnecessary complexity has been introduced