-- Flyway migration: cookies
-- Generated from EF Core migration on 2025-08-06 21:54:10
-- Description: cookies
ALTER TABLE polls ALTER COLUMN user_id DROP NOT NULL;
ALTER TABLE polls ADD session_id character varying(255);