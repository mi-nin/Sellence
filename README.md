# Sellence Microservice Platform

## Architecture Overview
- **Microservices**: Separated Admin and Client services
- **CQRS**: Using Mediator library for command/query separation
- **Clean Architecture**: Clear separation of concerns
- **Frontend**: Blazor Server (Admin) + Next.js (Client)

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+
- SQL Server or Docker

### Development Setup
1. Clone the repository
2. Run dotnet restore to restore NuGet packages
3. Update connection strings in appsettings.json
4. Run database migrations: dotnet ef database update
5. Start services:
   - Admin API: dotnet run --project services/admin/Sellence.Admin.Api
   - Client API: dotnet run --project services/client/Sellence.Client.Api
   - Admin Frontend: dotnet run --project frontend/sellence-admin
   - Client Frontend: cd frontend/sellence-client && npm run dev

### Docker Setup
\\\ash
docker-compose up -d
\\\

## Project Structure
\\\
sellence/
├── services/          # Microservices
├── frontend/          # Frontend applications
├── data/             # Data layer
├── shared/           # Shared libraries
└── docker-compose.yml
\\\

## API Endpoints
- Admin API: http://localhost:5001
- Client API: http://localhost:5002
- Admin Frontend: http://localhost:5003
- Client Frontend: http://localhost:3000
