-- Flyway migration: statsessionid
-- Generated from EF Core migration on 2025-08-06 22:41:24
-- Description: statsessionid

ALTER TABLE poll_stats ADD session_id character varying(255);
