-- =============================================================================
-- Migration: Drop the MagicLink table.
--
-- MagicLink (created in 002) was an early design for portal access links. It was
-- superseded by OTP login and has no application code reading or writing it.
-- 002 is left in place because merged migrations are never modified; this
-- script retires the table for every environment that applied it.
--
-- Idempotent: safe to run more than once, and a no-op where 002 was never applied.
-- =============================================================================

IF OBJECT_ID('dbo.MagicLink', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[MagicLink];   -- its indexes and constraints are dropped with it
END
GO
