### Feature: Authentication & Sign-in

#### Change History
> **Policy Note:** Do not retroactively backfill change history for historical edits made prior to this section's introduction. Record entries only for changes made from [Current Date / Effective Date] forward. Focus records on the *reason for the change*, not trivial author details.

| Date | Version | Reason for Change | Impacted Components |
| :--- | :--- | :--- | :--- |
| 2026-10-09 | v1.0 | Initial specification of one-time-password sign-in as built, plus a temporary switch that turns authentication off and treats everyone as one configured user, so development is not blocked by login. Authorisation is deliberately deferred to just before go-live. | `AuthRepository`, `BypassAuthenticationHandler`, `Startup`, the three `JwtAuthenticationStateProvider` classes, all four `appsettings.json` files, `Tutor.Client.Tests` |

---

#### 1. Objectives & Context
- **Feature Objectives:** Identify who is using the system. Teachers use the management app, students use the Student Portal, account holders use the Account Holder Portal. Until go-live, development must not be blocked by the sign-in step.
- **Scope:**
  - **In-Scope:** One-time-password (OTP) sign-in by email; a signed token carrying the user's identity (email, display name, role, and teacher / student / account-holder IDs); a token sent with every API call; a `User` record per teacher, student and account holder; a configuration switch that disables authentication and uses a fixed bypass user.
  - **Out-of-Scope:** Authorisation (restricting teacher-only API endpoints to teachers) — **planned for just before go-live**. Real email delivery (OTPs are printed to the API console). Sign-out and token refresh. OTP hardening (attempt limits, persistent storage, cryptographic random numbers).
- **Related Features & Systems:** User table and automatic user creation (`Database/003_AddUsers.sql`); `ICurrentUserService` (reads the signed-in user's IDs on the API); the three Blazor apps (`Tutor.Web`, `Tutor.StudentPortal`, `Tutor.AccountHolderPortal`).

#### 2. Dependencies
- **People / Stakeholders:**
  - **Feature Owner / PO:** Johan Jordaan
  - **Domain Expert / Sign-off:** To be confirmed
- **Services & APIs:**
  - **Internal Services:** `IAuthRepository`, `IUserRepository`, `IJwtTokenService`, `IOtpService`, `IEmailService` (currently `ConsoleEmailService`, which prints to the API console).
  - **External Services:** None active. `BrevoEmailService` exists but is not registered.
- **Packages & Libraries:**
  - **NuGet / Third-Party Dependencies:** `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.Components.Authorization`, xUnit, Moq.

#### 3. Business Rules
- **[BR-01]:** A person can sign in only if a user is registered for their email address.
- **[BR-02]:** An OTP is valid for 10 minutes and can be used once.
- **[BR-03]:** One email address can hold several roles (for example teacher and account holder). The signed token carries the IDs for every role that email holds.
- **[BR-04]:** Authentication is **on** unless `Auth:Enabled` is explicitly set to `false`. A missing setting never disables it.
- **[BR-05]:** While `Auth:Enabled` is `false`, every request is treated as the user whose email is `Auth:BypassEmail`, with the same claims a real sign-in would have produced. The API refuses to start if `Auth:BypassEmail` is empty.
- **[BR-06]:** The API and the three apps each read their own `Auth:Enabled`. They **must be set to the same value**, otherwise the apps and API disagree about who is signed in.
- **[BR-07]:** The bypass user has only the roles registered for that email. At present `adelejordaan@gmail.com` is a teacher only, so the Student and Account Holder portals have no student or account holder identity.

#### 4. Technical Scope & Guidance
- **Technical Strategy & Patterns:** The bypass swaps the API authentication scheme (`BypassAuthenticationHandler`) instead of removing `[Authorize]`, so controllers and `ICurrentUserService` behave identically with authentication on or off, and turning it back on is a configuration change. The bypass identity is built by `AuthRepository.GetUserClaimsAsync`, the same method that builds real token claims, so there is a single definition of what a user's claims are. On the apps, `JwtAuthenticationStateProvider` returns a fixed signed-in user when `Auth:Enabled` is `false`.
- **Validators:** None.
- **Repositories & DAOs:** `AuthRepository` (`RequestOtpAsync`, `VerifyOtpAsync`, new `GetUserClaimsAsync`), `UserRepository`, `UserDataAccessObject`. `MagicLink` table retired by `Database/004_DropMagicLinkTable.sql`.
- **Blazor Services:** `JwtAuthenticationStateProvider` in each app. It does not call an external API, so it is TDD'd (`Tutor.Client.Tests`, 6 tests per app).
- **Known gaps (tracked, not in this change):** no role-based authorisation; OTP store is in memory with unlimited guesses and a non-cryptographic generator; no sign-out; the client does not check token expiry; the JWT secret is in `appsettings.json`.

#### 5. Key Functional & Behavioural Requirements
- [ ] With `Auth:Enabled` = `false`, opening any page of the management app goes straight to the page with data loaded and no login screen.
- [ ] An API call with no `Authorization` header to a teacher endpoint (for example `Teacher/GetAllActive`) returns data.
- [ ] The API start-up log shows the warning "AUTHENTICATION IS DISABLED".
- [ ] The Student Portal and Account Holder Portal open without a login screen. They show "not found" unless the bypass email is also registered as a student or account holder (BR-07).
- [ ] With `Auth:Enabled` = `true` on the API **and** all three apps, unauthenticated visitors are redirected to the login screen, the OTP is printed in the API console, and entering it signs the user in.
- [ ] With `Auth:Enabled` = `false` and an empty `Auth:BypassEmail`, the API fails at start-up with a clear message.
- [ ] Removing `Auth:Enabled` from the settings leaves authentication switched on.

---

#### 6. User Guide
> **Audience:** Application users. This section is served by the in-app Help chat.
> **Constraint:** Plain UI language only — no class, method, table, or field names.

- **Where to find it:** The login screen appears when you open any of the apps and are not signed in.
- **How to sign in (when sign-in is switched on):**
  1. Open the app. You are taken to the login screen.
  2. Type your email address and choose **Send OTP**.
  3. Enter the one-time password you were sent and choose **Verify OTP**.
  4. You are taken to the app's home page.
- **How sign-in works at the moment:** Sign-in is temporarily switched off while the system is being built. You are taken straight into each app and do not see a login screen. The one-time password is not emailed; it is shown in the system's technical console for the person setting the system up.
- **What the screen enforces:** The one-time password expires after 10 minutes and works once. Only email addresses registered with the school can sign in. If you also have a student or account holder record, the same email gives you access to each of them.
- **Common questions:**
  - *Why am I not asked to sign in?* Sign-in is switched off during setup. It will be switched on before the system goes live.
  - *My one-time password was rejected.* It may have expired or already been used. Choose **Back** and request a new one.
  - *It says no user is registered for my email.* Ask your teacher to check that your email address is on your record.
  - *The Student or Account Holder portal says "not found".* While sign-in is switched off the portals use the setup user, who is a teacher. They will show your own details once sign-in is switched on.
