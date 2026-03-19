-- Migration: Support extra-lesson invoices on the Invoice table
-- Run once against the MusicSchool database.

-- 1. Make BundleID nullable (bundle invoices keep their value; extra-lesson invoices will be NULL).
ALTER TABLE Invoice
    ALTER COLUMN BundleID INT NULL;

-- 2. Add ExtraLessonID as a nullable FK to ExtraLesson.
ALTER TABLE Invoice
    ADD ExtraLessonID INT NULL
        CONSTRAINT FK_Invoice_ExtraLesson
        REFERENCES ExtraLesson(ExtraLessonID);

-- 3. (Optional) Add a check constraint so every invoice row belongs to either a bundle OR an extra lesson, never both or neither.
ALTER TABLE Invoice
    ADD CONSTRAINT CHK_Invoice_BundleOrExtraLesson
        CHECK (
            (BundleID IS NOT NULL AND ExtraLessonID IS NULL)
            OR
            (BundleID IS NULL AND ExtraLessonID IS NOT NULL)
        );
