-- =============================================================================
-- Migration: Relax CK_Bundle_TotalLessons to allow prorated bundles.
--
-- CK_Bundle_TotalLessons required TotalLessons to be divisible by 4, which was
-- true while every bundle covered a full year. Since the Full / Prorata option
-- (LessonBundleRepository.AddBundleAsync), a prorated bundle stores only the
-- lessons for the quarters left in the year: a 36-lesson bundle started in Q4
-- is saved with TotalLessons = 9, which the old constraint rejected
-- ("The INSERT statement conflicted with the CHECK constraint").
--
-- The "divisible by 4" rule still applies to the full-year size the teacher
-- selects, and is enforced in the repository before prorating.
--
-- Idempotent: safe to run more than once.
-- =============================================================================

IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Bundle_TotalLessons')
BEGIN
    ALTER TABLE [dbo].[LessonBundle] DROP CONSTRAINT [CK_Bundle_TotalLessons];
END
GO

ALTER TABLE [dbo].[LessonBundle]
    ADD CONSTRAINT [CK_Bundle_TotalLessons] CHECK ([TotalLessons] > 0);
GO
