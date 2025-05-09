# FixMate - Vehicle Service Booking System

FixMate is a comprehensive vehicle service booking system that connects vehicle owners with service providers. The system is built using Clean Architecture principles and follows best practices for maintainability and scalability.

## Features

- User registration and authentication
- Vehicle management
- Service request creation and tracking
- Service provider management
- Real-time service status updates
- Rating and review system

## Prerequisites

- .NET 7.0 SDK or later
- SQL Server (LocalDB for development)
- Visual Studio 2022 or later (recommended)

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/fixmate.git
   cd fixmate
   ```

2. Update the connection string in `appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FixMateDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. Update the JWT settings in `appsettings.Development.json`:
   ```json
   "Jwt": {
     "Key": "your-development-secret-key-here-min-16-chars",
     "Issuer": "https://localhost:5001",
     "Audience": "https://localhost:5001"
   }
   ```

4. Run the database migrations:
   ```bash
   dotnet ef database update --project FixMate.Infrastructure --startup-project FixMate.Web
   ```

5. Run the application:
   ```bash
   dotnet run --project FixMate.Web
   ```

The API will be available at `https://localhost:5001` and `http://localhost:5000`.

## API Documentation

Once the application is running, you can access the Swagger documentation at:
- https://localhost:5001/swagger
- http://localhost:5000/swagger

## Project Structure

The solution follows Clean Architecture principles and is organized into the following projects:

- `FixMate.Domain`: Contains enterprise business rules and entities
- `FixMate.Application`: Contains business rules and interfaces
- `FixMate.Infrastructure`: Contains implementations of interfaces and external concerns
- `FixMate.Web`: Contains the API controllers and configuration

## Testing

Run the unit tests:
```bash
dotnet test
```

## Deployment

1. Update the production settings in `appsettings.Production.json`:
   - Set the correct database connection string
   - Update the JWT settings with secure values
   - Configure logging paths

2. Build the application:
   ```bash
   dotnet publish -c Release
   ```

3. Deploy the published files to your hosting environment.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 