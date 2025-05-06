<h1 align="center">🔧 FixMate</h1>

<p align="center">
  <b>Your all-in-one vehicle service booking platform.</b><br/>
  Built with <strong>ASP.NET Core</strong>, powered by <strong>Clean Architecture</strong>, and ready for <strong>Docker & Cloud</strong>.
</p>

<div align="center">
  <img src="https://img.shields.io/badge/.NET%208-blueviolet?logo=dotnet&style=flat-square" />
  <img src="https://img.shields.io/badge/Architecture-Clean-informational?style=flat-square" />
  <img src="https://img.shields.io/badge/Deployed%20with-Docker-blue?logo=docker&style=flat-square" />
  <img src="https://img.shields.io/github/license/abdullahsherdy/FixMate?style=flat-square" />
</div>

---

## 📦 Overview

**FixMate** is a vehicle service booking system where:
- 🚗 **Customers** can register, manage vehicles, and book car/bike services.
- 🧰 **Mechanics** can view, accept/reject, and update service requests.
- 🛠️ Built using **Clean Architecture** with proper layering and separation of concerns.
- 🐳 Fully **containerized with Docker** and ready for **cloud deployment**.

---

## 🧱 Tech Stack

- **ASP.NET Core 8.0**
- **Entity Framework Core**
- **Clean Architecture**
- **Docker**
- **Swagger / OpenAPI**
- (Optional) PostgreSQL / SQL Server
- (Optional) JWT Auth, SignalR, Email Service

---

## 📂 Project Structure

```bash
FixMate/
│
├── src/
│   ├── FixMate.Domain/         # Domain models, entities, enums
│   ├── FixMate.Application/    # Use cases, interfaces, DTOs
│   ├── FixMate.Infrastructure/ # EF Core, DB, external services
│   └── FixMate.Web/         # API controllers, middleware, DI
│
├── tests/                      # Unit & integration tests
│
├── Dockerfile
├── docker-compose.yml
└── README.md
```
