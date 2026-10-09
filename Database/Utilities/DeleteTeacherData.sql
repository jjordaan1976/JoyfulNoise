-- =============================================================================
-- Utility: Delete ALL data belonging to one teacher (the "company"/tenant root).
--
-- Removes, in FK-safe order, everything under the teacher:
--   PaymentAllocation → Payment → Invoice → Lesson → BundleQuarter →
--   LessonBundle → ScheduledSlot → ExtraLesson → [User] →
--   Student → AccountHolder → (optionally) the Teacher row itself.
--
-- SAFETY:
--   • Runs in a single transaction.
--   • Prints row counts per table.
--   • Does NOTHING unless @Commit = 1 — run once with @Commit = 0 (default)
--     to preview the counts, then set @Commit = 1 to actually delete.
--
-- THIS IS IRREVERSIBLE ONCE COMMITTED. Take a backup first.
-- =============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;   -- any error rolls the whole transaction back

DECLARE @TeacherID     INT = 0;   -- <<< SET THE TEACHER ID HERE
DECLARE @DeleteTeacher BIT = 0;   -- 1 = also delete the Teacher row itself
DECLARE @Commit        BIT = 0;   -- 1 = actually delete; 0 = preview only (rolls back)

IF @TeacherID <= 0
BEGIN
    RAISERROR('Set @TeacherID to the teacher whose data must be deleted.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM Teacher WHERE TeacherID = @TeacherID)
BEGIN
    RAISERROR('TeacherID %d does not exist.', 16, 1, @TeacherID);
    RETURN;
END

DECLARE @TeacherName NVARCHAR(100) = (SELECT Name FROM Teacher WHERE TeacherID = @TeacherID);
PRINT CONCAT('Deleting data for TeacherID ', @TeacherID, ' (', @TeacherName, ')',
             CASE WHEN @Commit = 1 THEN ' — COMMIT MODE' ELSE ' — PREVIEW ONLY (will roll back)' END);

BEGIN TRANSACTION;

-- Scope tables: every account holder, student and bundle under the teacher.
DECLARE @AccountHolders TABLE (AccountHolderID INT PRIMARY KEY);
INSERT INTO @AccountHolders
SELECT AccountHolderID FROM AccountHolder WHERE TeacherID = @TeacherID;

DECLARE @Students TABLE (StudentID INT PRIMARY KEY);
INSERT INTO @Students
SELECT StudentID FROM Student
WHERE AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);

DECLARE @Bundles TABLE (BundleID INT PRIMARY KEY);
INSERT INTO @Bundles
SELECT BundleID FROM LessonBundle WHERE TeacherID = @TeacherID;

-- 1. PaymentAllocation (child of Payment and Invoice)
DELETE pa
FROM PaymentAllocation pa
JOIN Payment p ON p.PaymentID = pa.PaymentID
WHERE p.AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);
PRINT CONCAT('PaymentAllocation: ', @@ROWCOUNT);

-- 2. Payment
DELETE FROM Payment
WHERE AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);
PRINT CONCAT('Payment: ', @@ROWCOUNT);

-- 3. Invoice (bundle-, extra-lesson-, and directly account-holder-linked)
DELETE FROM Invoice
WHERE AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);
PRINT CONCAT('Invoice: ', @@ROWCOUNT);

-- 4. Lesson (via the teacher's bundles)
DELETE FROM Lesson
WHERE BundleID IN (SELECT BundleID FROM @Bundles);
PRINT CONCAT('Lesson: ', @@ROWCOUNT);

-- 5. BundleQuarter
DELETE FROM BundleQuarter
WHERE BundleID IN (SELECT BundleID FROM @Bundles);
PRINT CONCAT('BundleQuarter: ', @@ROWCOUNT);

-- 6. LessonBundle
DELETE FROM LessonBundle WHERE TeacherID = @TeacherID;
PRINT CONCAT('LessonBundle: ', @@ROWCOUNT);

-- 7. ScheduledSlot
DELETE FROM ScheduledSlot WHERE TeacherID = @TeacherID;
PRINT CONCAT('ScheduledSlot: ', @@ROWCOUNT);

-- 8. ExtraLesson
DELETE FROM ExtraLesson WHERE TeacherID = @TeacherID;
PRINT CONCAT('ExtraLesson: ', @@ROWCOUNT);

-- 9. Login users for the teacher's students, account holders — and the teacher
--     itself only when the Teacher row is also being deleted.
IF OBJECT_ID('dbo.[User]') IS NOT NULL
BEGIN
    DELETE FROM [User]
    WHERE StudentID       IN (SELECT StudentID FROM @Students)
       OR AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders)
       OR (@DeleteTeacher = 1 AND TeacherID = @TeacherID);
    PRINT CONCAT('[User]: ', @@ROWCOUNT);
END

-- 10. Student
DELETE FROM Student
WHERE StudentID IN (SELECT StudentID FROM @Students);
PRINT CONCAT('Student: ', @@ROWCOUNT);

-- 11. AccountHolder
DELETE FROM AccountHolder WHERE TeacherID = @TeacherID;
PRINT CONCAT('AccountHolder: ', @@ROWCOUNT);

-- 12. Teacher (optional)
IF @DeleteTeacher = 1
BEGIN
    DELETE FROM Teacher WHERE TeacherID = @TeacherID;
    PRINT CONCAT('Teacher: ', @@ROWCOUNT);
END

IF @Commit = 1
BEGIN
    COMMIT TRANSACTION;
    PRINT 'COMMITTED.';
END
ELSE
BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'PREVIEW ONLY — all deletes rolled back. Set @Commit = 1 to apply.';
END
