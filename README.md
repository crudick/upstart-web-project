# Upstart Web Project

A full-stack web application for managing users and loans. Built with .NET 8 minimal APIs, React, and TypeScript.

## Architecture

### Backend (.NET 8)
- **Upstart.Api**: Minimal API with endpoint routing
- **Upstart.Application**: Application layer with service interfaces
- **Upstart.Domain**: Domain entities and business logic
- **Upstart.Persistence**: Entity Framework Core with database entities

### Frontend (React + TypeScript)
- **Create React App**: Standard React development setup
- **TypeScript**: Type-safe development
- **Fetch API**: HTTP client for API communication
- **CSS**: Custom styling for responsive design

## API Endpoints

- `POST /api/users` - Create a new user
- `POST /api/loans` - Create a new loan

## Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Database (PostgreSQL)

### Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd upstart-web-project
   ```

2. **Backend Setup:**
   ```bash
   cd backend/Upstart
   dotnet restore
   dotnet run --project Upstart.Api
   ```
   - API will run on: https://localhost:5166
   - Swagger UI: https://localhost:5166/swagger

3. **Frontend Setup:**
   ```bash
   cd frontend
   npm install
   npm start
   ```
   - Frontend will run on: http://localhost:3000 or http://localhost:3001

### Running the Application

1. **Start the Backend:**
   ```bash
   cd backend/Upstart
   dotnet run --project Upstart.Api
   ```

2. **Start the Frontend:**
   ```bash
   cd frontend
   npm start
   ```

3. **Access the Application:**
   - Open your browser to http://localhost:3000
   - Use the navigation buttons to switch between "Create User" and "Create Loan" forms
   - Fill out the forms and submit to create users and loans via the API

## Project Structure

```
upstart-web-project/
├── backend/
│   └── Upstart/
│       ├── Upstart.Api/           # Web API project
│       ├── Upstart.Application/   # Application layer
│       ├── Upstart.Domain/        # Domain entities
│       └── Upstart.Persistence/   # Data access layer
├── frontend/
│   ├── src/
│   │   ├── components/    # React form components
│   │   │   ├── CreateUserForm.tsx
│   │   │   └── CreateLoanForm.tsx
│   │   ├── services/      # API client functions
│   │   │   └── api.ts
│   │   ├── App.tsx        # Main application component
│   │   └── App.css        # Application styles
│   └── public/            # Static assets
├── docker-compose.yml     # Docker services configuration
└── README.md
```

## Data Models

### User Data Model
```typescript
interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  dateOfBirth?: string;
  socialSecurityNumber?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  annualIncome?: number;
  employmentStatus?: string;
  creditScore?: number;
}
```

### Loan Data Model
```typescript
interface CreateLoanRequest {
  userId: number;
  loanAmount: number;
  interestRate: number;
  termMonths: number;
  monthlyPayment: number;
  loanPurpose: string;
  loanStatus: string;
  applicationDate: string;
  approvalDate?: string;
  disbursementDate?: string;
  maturityDate?: string;
  outstandingBalance: number;
  totalPaymentsMade: number;
  nextPaymentDueDate?: string;
  paymentFrequency: string;
  lateFees: number;
  originationFee: number;
  apr?: number;
  loanOfficerNotes?: string;
}
```

## Development Notes

### Frontend Configuration
- The frontend expects the backend API to run on `https://localhost:5166`
- You can change the API URL by setting the `REACT_APP_API_URL` environment variable
- Forms are designed to be simple and easily modifiable for learning purposes

### Backend Development
- Uses Entity Framework Core with database
- Minimal APIs for lightweight endpoints
- FluentValidation for request validation
- AutoMapper for object mapping
- Serilog for structured logging with Seq integration

## Database Migrations

This project uses Flyway for database schema management and migrations. Flyway provides version control for your database schema.

### Migration Directory Structure

```
flyway/upstart/
├── flyway.conf              # Local development configuration
├── flyway-docker.conf       # Docker environment configuration
├── migrations/              # SQL migration files
│   └── V1__InitialCreate.sql
└── README.md
```

### Running Migrations

#### Option 1: Docker Compose (Recommended)

The easiest way to run migrations is using Docker Compose, which will automatically:
1. Start PostgreSQL
2. Wait for the database to be healthy
3. Apply all pending migrations
4. Make the database ready for the application

```bash
# Start PostgreSQL and run migrations
docker-compose up postgres flyway

# Or start everything (database, migrations, and any other services)
docker-compose up --build
```

#### Option 2: Manual Migration (Local Development)

If you have Flyway installed locally and PostgreSQL running:

```bash
# Navigate to the Flyway configuration directory
cd flyway/upstart

# Run pending migrations
flyway migrate

# Check migration status
flyway info

# View migration history
flyway info
```

#### Option 3: Using Flyway Docker Image

If you have Docker but want to run migrations manually:

```bash
# Run migrations using Flyway Docker image
docker run --rm \
  --network host \
  -v $(pwd)/flyway/upstart:/flyway/conf \
  -v $(pwd)/flyway/upstart/migrations:/flyway/sql \
  flyway/flyway:latest \
  -configFiles=/flyway/conf/flyway.conf migrate
```

### Database Configuration

- **Database**: UpstartDb
- **Host**: localhost (local) / postgres (Docker)
- **Port**: 5432
- **User**: postgres
- **Password**: postgres

### Migration Files

Migration files follow Flyway naming conventions:
- `V{version}__{description}.sql` for versioned migrations
- Example: `V1__InitialCreate.sql`, `V2__AddUserTable.sql`

### Creating New Migrations

#### From Entity Framework Changes

Use the provided script to convert EF migrations to Flyway:

```bash
cd scripts
./ef-to-flyway.sh -n "YourMigrationName"
```

This script will:
1. Generate an Entity Framework migration
2. Convert it to a Flyway SQL file
3. Place it in the `flyway/upstart/migrations/` directory
4. Optionally remove the EF migration files

#### Manual SQL Migrations

Create new migration files directly in `flyway/upstart/migrations/`:

```sql
-- V2__AddLoanTable.sql
CREATE TABLE loans (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    amount DECIMAL(15,2) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);
```

### Migration Best Practices

1. **Always backup** your database before running migrations in production
2. **Test migrations** in a development environment first
3. **Use descriptive names** for migration files
4. **Keep migrations small** and focused on one change
5. **Never modify** existing migration files after they've been applied
6. **Use transactions** when possible for atomic changes

### Troubleshooting

#### View Migration History
```bash
cd flyway/upstart
flyway info
```

#### Repair Migration History (if needed)
```bash
flyway repair
```

#### Clean Database (Development Only - DESTRUCTIVE)
```bash
flyway clean  # Removes all database objects
```

## Logging with Serilog, Seq, and Datadog

This project uses Serilog for structured logging with:
- **Seq** for local development log aggregation and analysis
- **Datadog** for production log management and monitoring

### Seq Dashboard

Seq provides a web-based interface for searching, filtering, and analyzing logs with rich querying capabilities.

- **Seq URL**: http://localhost:5341
- **Default Login**: No authentication required for local development

### Running Seq

#### Option 1: Docker Compose (Recommended)

Seq starts automatically with the full application stack:

```bash
# Start everything including Seq
docker-compose up --build

# Or start just the logging stack
docker-compose up postgres seq
```

#### Option 2: Standalone Seq

Run Seq independently:

```bash
# Run Seq in Docker
docker run -d \
  --name seq \
  -e ACCEPT_EULA=Y \
  -p 5341:80 \
  -v seq-data:/data \
  datalust/seq:latest
```

### Log Configuration

The application is configured to send logs to both console and Seq:

- **Console**: Formatted output for development debugging
- **Seq**: Structured logs for analysis and searching
- **Log Levels**: Information and above (Debug in Development)

### Log Structure

The application captures:

- **HTTP Requests**: Method, path, status code, response time, client IP, user agent
- **User Operations**: User creation, validation errors, business logic events
- **System Events**: Application startup, shutdown, errors
- **Performance**: Request duration, database operations

### Example Log Queries in Seq

Once you have logs flowing to Seq, you can use these queries:

```sql
-- All user creation events
RequestPath like '/api/users' and RequestMethod = 'POST'

-- Failed requests (4xx/5xx status codes)
StatusCode >= 400

-- Slow requests (over 1 second)
Elapsed > 1000

-- User validation failures
@mt like '%validation failed%'

-- Requests from a specific IP
ClientIP = '127.0.0.1'
```

### Log Enrichment

Logs are automatically enriched with:

- **Environment**: Development, Production, etc.
- **Machine Name**: Host machine identifier
- **Thread ID**: Thread processing the request
- **Process ID**: Application process identifier
- **Request Context**: HTTP method, path, status, timing

### Configuration Files

#### appsettings.json
```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      }
    ]
  }
}
```

#### appsettings.Development.json
- Debug level logging enabled
- Enhanced console output
- Development-specific Seq configuration

### Troubleshooting Logging

#### Seq Not Receiving Logs
1. Verify Seq is running: http://localhost:5341
2. Check application logs for Serilog connection errors
3. Verify firewall settings for port 5341

#### Console Logs Only
- Check Seq URL configuration in appsettings
- Verify Seq container is healthy: `docker ps`
- Check Docker network connectivity

#### Performance Impact
- Seq logging is asynchronous and shouldn't impact performance
- Log levels can be adjusted per namespace in appsettings
- Consider log retention policies for production

### Frontend Development
- TypeScript for type safety
- Component-based architecture with React hooks
- Custom CSS for styling (no external UI libraries)
- Fetch API for HTTP requests

### Form Features
- **User Form**: Comprehensive user profile creation with personal and financial information
- **Loan Form**: Detailed loan application with terms, dates, and financial details
- **Validation**: Required fields marked with asterisks
- **Feedback**: Success/error messages displayed after form submission
- **Navigation**: Toggle between forms using navigation buttons

