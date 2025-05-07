
## Vehicle Service Booking System
- A user can create an account and book a car or bike service
- A user can select service type and provide vehicle details
- A mechanic can view incoming service requests and accept or reject them
- A mechanic can update the service status to show progress or completion


## Proejct Structure 
```bash
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