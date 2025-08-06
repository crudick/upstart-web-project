# Flyway Database Migrations

This directory contains the Flyway configuration and migration files for the OpenStation database.

## Directory Structure

```
flyway/openstation/
├── flyway.conf              # Local development configuration
├── flyway-docker.conf       # Docker environment configuration
├── migrations/              # SQL migration files
│   └── V1__InitialCreate.sql
└── README.md
```

## Migration Files

Migration files follow Flyway naming conventions:
- `V{version}__{description}.sql` for versioned migrations
- `U{version}__{description}.sql` for undo migrations (if needed)

## Usage

### With Docker Compose (Recommended)

Migrations run automatically when you start the application:

```bash
docker-compose up --build
```

The Flyway service will:
1. Wait for PostgreSQL to be healthy
2. Apply all pending migrations
3. Complete before the backend starts

### Manual Migration (Local Development)

If you have Flyway installed locally:

```bash
cd flyway/openstation
flyway migrate
```

### Using the EF to Flyway Conversion Script

To create new migrations from Entity Framework changes:

```bash
cd scripts
./ef-to-flyway.sh -n "YourMigrationName"
```

This will:
1. Generate an EF migration
2. Convert it to a Flyway SQL file
3. Place it in the migrations directory
4. Optionally remove the EF migration files

## Configuration

- **flyway.conf**: Used for local development (connects to localhost:5432)
- **flyway-docker.conf**: Used in Docker environment (connects to postgres:5432)

## Database Connection

- **Host**: postgres (Docker) / localhost (local)
- **Port**: 5432
- **Database**: OpenStationDb
- **User**: postgres
- **Password**: postgres

## Migration History

Flyway tracks applied migrations in the `flyway_schema_history` table. You can view the migration history by querying this table or using `flyway info`.
