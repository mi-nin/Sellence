# Sellence Microservice Platform

A modern microservice platform built with .NET 8 and React, featuring clean architecture and CQRS patterns.

## 🏗️ Architecture Overview

- **Microservices**: Separated Admin and Client services for scalability
- **CQRS**: Command Query Responsibility Segregation using Mediator pattern
- **Clean Architecture**: Clear separation of concerns with domain-driven design
- **Frontend**: Blazor Server (Admin) + Next.js (Client) for optimal user experience

## 🚀 Getting Started

### Prerequisites

Before running the application, ensure you have:

- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **SQL Server** or **Docker** for database

### 🔧 Development Setup

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd sellence
   ```

2. **Restore NuGet packages**

   ```bash
   dotnet restore
   ```

3. **Configure database connection**

   - Update connection strings in `appsettings.json` files
   - Ensure SQL Server is running

4. **Run database migrations**

   ```bash
   dotnet ef database update
   ```

5. **Install frontend dependencies**

   ```bash
   cd frontend/sellence-client
   npm install
   cd ../..
   ```

6. **Start all services**

   Open multiple terminal windows and run:

   **Admin API:**

   ```bash
   dotnet run --project services/admin/Sellence.Admin.Api
   ```

   **Client API:**

   ```bash
   dotnet run --project services/client/Sellence.Client.Api
   ```

   **Admin Frontend:**

   ```bash
   dotnet run --project frontend/sellence-admin
   ```

   **Client Frontend:**

   ```bash
   cd frontend/sellence-client
   npm run dev
   ```

### 🐳 Docker Setup

For easier development setup, use Docker Compose:

```bash
docker-compose up -d
```

This will start all services with their dependencies.

## 📁 Project Structure

```
sellence/
├── services/              # Microservices
│   ├── admin/            # Admin service
│   └── client/           # Client service
├── frontend/             # Frontend applications
│   ├── sellence-admin/   # Blazor admin panel
│   └── sellence-client/  # Next.js client app
├── data/                 # Data layer & migrations
├── shared/               # Shared libraries & models
├── tests/                # Unit & integration tests
├── docker-compose.yml    # Docker configuration
└── README.md
```

## 🌐 API Endpoints

Once running, access the services at:

| Service         | URL                   | Description               |
| --------------- | --------------------- | ------------------------- |
| Admin API       | http://localhost:5001 | Administrative operations |
| Client API      | http://localhost:5002 | Client-facing operations  |
| Admin Frontend  | http://localhost:5003 | Admin dashboard           |
| Client Frontend | http://localhost:3000 | Customer interface        |

## 📋 Available Scripts

### Backend (.NET)

- `dotnet run` - Start development server
- `dotnet test` - Run unit tests
- `dotnet build` - Build the project
- `dotnet ef database update` - Apply database migrations

### Frontend (Next.js)

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run start` - Start production server
- `npm run lint` - Run ESLint

## 🛠️ Development

### Code Structure

- **Domain Layer**: Business logic and entities
- **Application Layer**: Use cases and business rules
- **Infrastructure Layer**: External concerns (database, APIs)
- **Presentation Layer**: Controllers and UI components

### Testing

Run all tests:

```bash
dotnet test
```

### Database Migrations

Create new migration:

```bash
dotnet ef migrations add MigrationName
```

Apply migrations:

```bash
dotnet ef database update
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:

- Create an issue in the repository
- Check the documentation in the `/docs` folder
- Contact the development team

---

Built with ❤️ using .NET 8 and Next.js
