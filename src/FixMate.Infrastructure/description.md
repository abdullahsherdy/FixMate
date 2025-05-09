
## Vehicle Service Booking System
- A user can create an account and book a car or bike service
- A user can select service type and provide vehicle details
- A mechanic can view incoming service requests and accept or reject them
- A mechanic can update the service status to show progress or completion



## Proejct Structure 
```bash
Arch : Clean + CQRS 
CQRS depend on 
    - Command Query Responsibility Segregation
    - Domain Driven Design
MediatR is a library that helps you implement the CQRS pattern in your application. 
It allows you to send commands and queries to different handlers, making your code more organized and maintainable.
its flow
    ```plaintext
        Controller → Send(Command) → MediatR → Finds Handler → Executes Logic → Ret
    ```
FixMate/
│
├── src/
│   ├── FixMate.Domain/         # Domain models, entities, enums, ValueObjects
│   ├── FixMate.Application/    # Use cases, interfaces, DTOs
|   |		└── FixMate.Application.Tests/ # Unit tests for application layer 
│   ├── FixMate.Infrastructure/ # EF Core, DB, external services
│   └── FixMate.Web/         # API controllers, middleware, DI
│
├── tests/                      # Unit & integration tests
│
├── Dockerfile
├── docker-compose.yml  # Docker Compose file for local development, incase of multiple services like DB which not included in the repo or locally 
└── README.md
```

```bash
this layer arch is CQRS 
CQRS stands for:
    Command Query Responsibility Segregation 
It means:
Commands: Do something (create, update, delete).
Queries: Ask for something (read data).
In traditional systems, a single method might:
    Get data 🧾
    Modify data ✏️
    Save it 🧷
But CQRS separates those responsibilities. Each side has one job only.

FixMate.Application/
│
├── Common/
│   ├── Behaviors/
│   │   └── ValidationBehavior.cs
│   └── Exceptions/
│       └── ValidationException.cs
│
├── Interfaces/
│   ├── Persistence/
│   │   ├── IUserRepository.cs
│   │   ├── IMechanicRepository.cs
│   │   ├── IVehicleRepository.cs
│   │   └── IServiceRequestRepository.cs
│   ├── Services/
│   │   └── IPasswordHasher.cs
│   └── Auth/
│       └── ICurrentUserService.cs
│
├── Features/
│   ├── Auth/
│   │   └── Commands/
│   │       └── LoginCommand.cs / LoginHandler.cs
│   │
│   ├── Users/
│   │   ├── Commands/
│   │   │   └── RegisterUserCommand.cs / RegisterUserHandler.cs
│   │   ├── DTOs/
│   │   │   └── UserDto.cs
│   │   └── Validators/
│   │       └── RegisterUserValidator.cs
│   │
│   ├── Mechanics/
│   │   ├── Commands/
│   │   │   └── UpdateServiceStatusCommand.cs
│   │   └── Queries/
│   │       └── GetServiceRequestsQuery.cs
│   │ 
│   ├── Vehicles/
│   │   ├── Commands/
│   │   │   └── AddVehicleCommand.cs
│   │   └── DTOs/
│   │       └── VehicleDto.cs
│   │
│   └── ServiceRequests/
│       ├── Commands/
│       │   └── CreateServiceRequestCommand.cs
│       └── DTOs/
│           └── ServiceRequestDto.cs
└── FixMate.Application.csproj
```