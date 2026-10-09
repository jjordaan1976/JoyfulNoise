-- Removes the demo family the recorder creates (Thandi Mokoena and her children),
-- so the demo can be recorded again from a clean state. Matches on the
-- @example.com emails used in Program.cs; nothing else is touched.
--
--   sqlcmd -S . -d JoyfullMusicSchool -E -i tools/DemoRecorder/remove-demo-family.sql

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;   -- sqlcmd defaults this off, which the filtered indexes reject

DECLARE @AccountHolders TABLE (AccountHolderID INT PRIMARY KEY);
INSERT INTO @AccountHolders
SELECT AccountHolderID FROM AccountHolder WHERE Email = 'thandi.mokoena@example.com';

DECLARE @Students TABLE (StudentID INT PRIMARY KEY);
INSERT INTO @Students
SELECT StudentID FROM Student WHERE AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);

DECLARE @Bundles TABLE (BundleID INT PRIMARY KEY);
INSERT INTO @Bundles
SELECT BundleID FROM LessonBundle WHERE StudentID IN (SELECT StudentID FROM @Students);

BEGIN TRANSACTION;

DELETE pa FROM PaymentAllocation pa
JOIN Payment p ON p.PaymentID = pa.PaymentID
WHERE p.AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);

DELETE FROM Payment WHERE AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);
DELETE FROM Invoice WHERE AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);
DELETE FROM Lesson        WHERE BundleID IN (SELECT BundleID FROM @Bundles);
DELETE FROM BundleQuarter WHERE BundleID IN (SELECT BundleID FROM @Bundles);
DELETE FROM LessonBundle  WHERE BundleID IN (SELECT BundleID FROM @Bundles);
DELETE FROM ScheduledSlot WHERE StudentID IN (SELECT StudentID FROM @Students);
DELETE FROM ExtraLesson   WHERE StudentID IN (SELECT StudentID FROM @Students);
DELETE FROM [User]
WHERE StudentID IN (SELECT StudentID FROM @Students)
   OR AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);
DELETE FROM Student WHERE StudentID IN (SELECT StudentID FROM @Students);
DELETE FROM AccountHolder WHERE AccountHolderID IN (SELECT AccountHolderID FROM @AccountHolders);

COMMIT;
PRINT 'Demo family removed.';
