-- Flyway migration: nullable name
-- Generated from EF Core migration on 2025-08-06 20:21:39
-- Description: nullable name

ALTER TABLE users ALTER COLUMN first_name DROP NOT NULL;
ALTER TABLE users ALTER COLUMN last_name DROP NOT NULL;
