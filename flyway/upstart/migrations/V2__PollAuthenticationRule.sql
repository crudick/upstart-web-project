-- Flyway migration: PollAuthenticationRule
-- Generated from EF Core migration on 2025-08-06 15:34:01
-- Description: PollAuthenticationRule

ALTER TABLE polls ADD requires_authentication boolean NOT NULL DEFAULT FALSE;
