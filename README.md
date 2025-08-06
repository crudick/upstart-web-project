# UpPoll - Modern Polling Platform

A full-stack web application for creating, sharing, and analyzing polls. Built with .NET 8 minimal APIs, React, TypeScript, and modern UI components.

## What is UpPoll?

UpPoll is a modern polling platform that allows users to create engaging polls, share them with their audience, and view real-time results. Whether you're gathering feedback, making group decisions, or conducting market research, UpPoll provides the tools you need.

## Features

### For Unauthenticated Users
- **Browse Public Polls**: View and participate in publicly available polls
- **Vote on Polls**: Cast votes on any public poll without needing an account
- **View Results**: See real-time poll results and analytics
- **Responsive Design**: Access polls from any device

### For Authenticated Users
- **Create Polls**: Design custom polls with multiple choice questions
- **Manage Polls**: Edit, delete, or update your existing polls
- **Privacy Controls**: Make polls public or private
- **Dashboard Analytics**: View detailed statistics on poll performance
- **User Management**: Secure account registration and authentication
- **Poll History**: Track all your created polls in one place

## Architecture

### Backend (.NET 8)
- **Upstart.Api**: Minimal API with endpoint routing
- **Upstart.Application**: Application layer with service interfaces
- **Upstart.Domain**: Domain entities and business logic
- **Upstart.Persistence**: Entity Framework Core with database entities

### Frontend (React + TypeScript)
- **React 18**: Modern React with hooks and functional components
- **TypeScript**: Type-safe development
- **Tailwind CSS**: Utility-first CSS framework
- **Framer Motion**: Smooth animations and transitions
- **Heroicons**: Beautiful SVG icons
- **Context API**: State management for authentication

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user account
- `POST /api/auth/login` - Authenticate and login user

### Users
- `POST /api/users` - Create a new user
- `GET /api/users/{id}` - Get user details

### Polls
- `GET /api/polls` - Get all public polls
- `POST /api/polls` - Create a new poll (authenticated)
- `GET /api/polls/{guid}` - Get specific poll details
- `PUT /api/polls/{guid}` - Update poll (authenticated, owner only)
- `DELETE /api/polls/{guid}` - Delete poll (authenticated, owner only)

### Poll Answers
- `POST /api/polls/{pollId}/answers` - Submit a poll response
- `GET /api/polls/{pollId}/stats` - Get poll statistics

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
   - Browse public polls or create an account to start creating your own polls
   - Use the dashboard to manage your polls and view analytics

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
│   │   ├── components/    # React components
│   │   │   ├── auth/      # Authentication forms
│   │   │   ├── layout/    # Layout components
│   │   │   ├── pages/     # Page components
│   │   │   ├── poll/      # Poll-specific components
│   │   │   └── ui/        # Reusable UI components
│   │   ├── contexts/      # React context providers
│   │   ├── services/      # API client functions
│   │   ├── types/         # TypeScript type definitions
│   │   └── App.tsx        # Main application component
│   └── public/            # Static assets
├── docker-compose.yml     # Docker services configuration
└── README.md
```

## Data Models

### User Data Model
```typescript
interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  createdAt: string;
}

interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phoneNumber?: string;
}
```

### Poll Data Model
```typescript
interface Poll {
  id: string;
  guid: string;
  title: string;
  description?: string;
  isPublic: boolean;
  allowMultipleVotes: boolean;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
  answers: PollAnswer[];
}

interface PollAnswer {
  id: string;
  text: string;
  pollId: string;
  voteCount: number;
}
```

### Poll Statistics
```typescript
interface PollStat {
  pollAnswerId: string;
  voteCount: number;
  percentage: number;
}
```

## Development Notes

### Frontend Configuration
- The frontend expects the backend API to run on `https://localhost:5166`
- You can change the API URL by setting the `REACT_APP_API_URL` environment variable
- Uses Tailwind CSS for styling with custom design system
- Responsive design optimized for mobile and desktop

### Backend Development
- Uses Entity Framework Core with PostgreSQL database
- Minimal APIs for lightweight endpoints
- FluentValidation for request validation
- AutoMapper for object mapping
- Serilog for structured logging with Seq integration
- JWT-based authentication for secure API access

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

### Database Configuration

- **Database**: UpstartDb
- **Host**: localhost (local) / postgres (Docker)
- **Port**: 5432
- **User**: postgres
- **Password**: postgres

## Authentication & Security

### JWT Authentication
- Secure token-based authentication
- Tokens expire after 24 hours
- Refresh token mechanism for seamless user experience

### Password Security
- Passwords are hashed using secure algorithms
- Minimum password requirements enforced
- Account lockout protection against brute force attacks

### API Security
- CORS configured for frontend domain
- Request validation and sanitization
- Rate limiting on sensitive endpoints

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

### Log Structure

The application captures:

- **HTTP Requests**: Method, path, status code, response time, client IP, user agent
- **User Operations**: Registration, login, poll creation, voting events
- **Poll Analytics**: Vote submissions, poll views, engagement metrics
- **System Events**: Application startup, shutdown, errors
- **Performance**: Request duration, database operations

## UI Components & Design System

### Design Principles
- **Modern & Clean**: Minimalist design focused on usability
- **Responsive**: Works seamlessly across all device sizes
- **Accessible**: WCAG-compliant components with proper ARIA labels
- **Consistent**: Unified color palette and typography system

### Custom Components
- **Button**: Multiple variants (primary, secondary, outline, ghost)
- **Input**: Form inputs with icons and validation states
- **Card**: Content containers with consistent styling
- **Poll Components**: Specialized components for poll display and interaction

### Color System
- **Primary**: Upstart brand colors for CTAs and highlights
- **Secondary**: Supporting colors for less prominent actions
- **Neutral**: Grays for text, borders, and backgrounds
- **Semantic**: Success, warning, and error states

## Deployment

### Production Considerations
- Environment-specific configuration files
- Database connection string security
- HTTPS enforcement
- Static file caching and CDN integration
- Performance monitoring and alerting

### Docker Support
- Multi-stage Docker builds for optimized production images
- Docker Compose for local development environment
- Health checks for service reliability

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.