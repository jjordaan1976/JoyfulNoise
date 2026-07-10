-- =============================================================================
-- Migration: Add User table and backfill users for existing
--            Teachers, Students and AccountHolders.
--
-- A User is a login identity. One row per role an email address holds —
-- the same email can be both a Student and an AccountHolder.
-- Roles: Teacher (Tutor.Web), Student (StudentPortal), AccountHolder (AccountHolderPortal)
--
-- Idempotent: safe to run more than once. The backfill section can also be
-- re-run on its own at any time to create users for rows added outside the app.
-- =============================================================================

-- ── User table ───────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'User' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[User]
    (
        UserID          INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Email           NVARCHAR(150)  NOT NULL,
        DisplayName     NVARCHAR(200)  NOT NULL,
        Role            NVARCHAR(20)   NOT NULL,   -- Teacher | Student | AccountHolder
        TeacherID       INT            NULL REFERENCES Teacher(TeacherID),
        StudentID       INT            NULL REFERENCES Student(StudentID),
        AccountHolderID INT            NULL REFERENCES AccountHolder(AccountHolderID),
        IsActive        BIT            NOT NULL DEFAULT 1,
        CreatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT UQ_User_Email_Role UNIQUE (Email, Role),
        CONSTRAINT CK_User_Role CHECK (Role IN ('Teacher', 'Student', 'AccountHolder'))
    );

    CREATE INDEX IX_User_Email ON [dbo].[User] (Email);
END
GO

-- ── Backfill: one user per existing Teacher ──────────────────────────────────
INSERT INTO [dbo].[User] (Email, DisplayName, Role, TeacherID, IsActive)
SELECT t.Email, t.Name, 'Teacher', t.TeacherID, t.IsActive
FROM Teacher t
WHERE t.Email IS NOT NULL AND LTRIM(RTRIM(t.Email)) <> ''
  AND NOT EXISTS (SELECT 1 FROM [dbo].[User] u
                  WHERE u.Email = t.Email AND u.Role = 'Teacher');
GO

-- ── Student.Email ────────────────────────────────────────────────────────────
-- The Student model and DAO already use Email, but the column was never added
-- to the table. Nullable: young students may not have their own email address.
IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID('dbo.Student') AND name = 'Email')
BEGIN
    ALTER TABLE [dbo].[Student] ADD Email NVARCHAR(150) NULL;
END
GO

-- ── Backfill: one user per existing Student (skip students without an email) ─
INSERT INTO [dbo].[User] (Email, DisplayName, Role, StudentID, IsActive)
SELECT s.Email, s.FirstName + ' ' + s.LastName, 'Student', s.StudentID, s.IsActive
FROM Student s
WHERE s.Email IS NOT NULL AND LTRIM(RTRIM(s.Email)) <> ''
  AND NOT EXISTS (SELECT 1 FROM [dbo].[User] u
                  WHERE u.Email = s.Email AND u.Role = 'Student');
GO

-- ── Backfill: one user per existing AccountHolder ────────────────────────────
INSERT INTO [dbo].[User] (Email, DisplayName, Role, AccountHolderID, IsActive)
SELECT a.Email, a.FirstName + ' ' + a.LastName, 'AccountHolder', a.AccountHolderID, a.IsActive
FROM AccountHolder a
WHERE a.Email IS NOT NULL AND LTRIM(RTRIM(a.Email)) <> ''
  AND NOT EXISTS (SELECT 1 FROM [dbo].[User] u
                  WHERE u.Email = a.Email AND u.Role = 'AccountHolder');
GO
