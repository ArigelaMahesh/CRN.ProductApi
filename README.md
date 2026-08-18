# CRN Product API

A RESTful Backend API solution for managing Products and Items, developed as part of the CRN Technical Assessment.

The application is built using ASP.NET Core Web API with .NET 8, Entity Framework Core, SQL Server, JWT authentication, refresh tokens, FluentValidation, Swagger/OpenAPI, repository and service layer patterns.

---

## 1. Project Overview

The CRN Product API provides RESTful endpoints to perform CRUD operations on Products and manage their relationship with Items.

The application follows a layered architecture to improve:

- Maintainability
- Scalability
- Separation of concerns
- Testability
- Security

---

## 2. Technology Stack

| Technology | Usage |
|---|---|
| .NET 8 | Application framework |
| C# | Programming language |
| ASP.NET Core Web API | REST API |
| SQL Server | Database |
| Entity Framework Core | ORM / Data Access |
| JWT | Authentication |
| Refresh Token | Token renewal |
| FluentValidation | Request validation |
| Swagger / OpenAPI | API documentation |
| xUnit | Unit testing |
| Moq | Mocking |
| WebApplicationFactory | Integration testing |
| Docker | Containerization |

---

## 3. Architecture

The application follows a layered architecture.

```text
CRN.ProductApi
│
├── src
│   ├── API
│   │   ├── Controllers
│   │   ├── Middleware
│   │   └── Program.cs
│   │
│   ├── Application
│   │   ├── DTOs
│   │   ├── Interfaces
│   │   ├── Services
│   │   └── Validators
│   │
│   ├── Domain
│   │   └── Entities
│   │
│   └── Infrastructure
│       ├── Data
│       ├── Repositories
│       └── Authentication
│
└── tests
