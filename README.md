# Upstart Loan Management System

A full-stack web application for managing users and loans in a lending platform. Built with .NET 8 minimal APIs, React, and TypeScript.

## Features

- **User Management**: Create and manage user profiles with comprehensive personal and financial information
- **Loan Processing**: Create and track loan applications with detailed terms and conditions
- **Form-based Interface**: Simple, intuitive forms for data entry
- **API Integration**: Frontend communicates with backend through RESTful API endpoints
- **Responsive Design**: Mobile-friendly React interface

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
- Database (SQL Server/PostgreSQL/SQLite)

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
   - API will run on: https://localhost:7000
   - Swagger UI: https://localhost:7000/swagger

3. **Frontend Setup:**
   ```bash
   cd frontend
   npm install
   npm start
   ```
   - Frontend will run on: http://localhost:3000

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
- The frontend expects the backend API to run on `https://localhost:7000`
- You can change the API URL by setting the `REACT_APP_API_URL` environment variable
- Forms are designed to be simple and easily modifiable for learning purposes

### Backend Development
- Uses Entity Framework Core with database
- Minimal APIs for lightweight endpoints
- FluentValidation for request validation
- AutoMapper for object mapping

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

## Getting Started with the Forms

### Creating a User
1. Navigate to the "Create User" form
2. Fill in required fields (marked with *)
3. Optional fields can be left blank
4. Submit the form to create a user via the API
5. Note the User ID returned for creating loans

### Creating a Loan  
1. Switch to the "Create Loan" form
2. Enter the User ID from a previously created user
3. Fill in loan details (amount, interest rate, term, etc.)
4. Submit the form to create a loan via the API

The forms are intentionally simple and well-commented to make learning and modification easy!
