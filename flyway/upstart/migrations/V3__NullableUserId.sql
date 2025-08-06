-- Flyway migration: NullableUserId
-- Generated from EF Core migration on 2025-08-06 15:46:51
-- Description: NullableUserId

ALTER TABLE poll_stats ALTER COLUMN user_id DROP NOT NULL;
