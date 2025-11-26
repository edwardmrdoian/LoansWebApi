# LoanApi
Api for Loan Managment

# Loans Web API (ASP.NET Core 8)

A clean, modular, production-ready Web API for managing **Users**, **Loans**, and **Accountant operations**.  
The project follows best practices from:

- **Clean Code — Robert C. Martin**
- **Dependency Injection in .NET — Mark Seemann**
- **Ultimate ASP.NET Core Web API — Code Maze**

This API includes **JWT authentication**, **role-based authorization**, **NLog logging**, **FluentValidation**, **global exception handling**, **clean architecture**, and **unit + integration tests**.

---

## 🚀 Features

### 👤 User Features
- User registration (password hashing with BCrypt)
- User login (JWT token)
- Get user by ID
- Users can:
  - Create loans  
  - Update/delete their own loans (only when status = *InProcess*)
  - View their own loans only
- Blocked users cannot request new loans

### 🧾 Loan Features
- Loan fields:  
  `LoanType`, `Amount`, `Currency`, `PeriodMonths`, `Status`, `UserId`
- New loans default to: `Status = InProcess`
- Users can manage only their loans
- Accountants can:
  - Get all loans
  - Get loans of specific users
  - Update or delete any loan
  - Approve/Reject loan status
  - Block/Unblock users

### 🧮 Accountant Features
- Stored in database manually (no registration)
- Role-based authorization: `Role = Accountant`
- Has full access to all loans

---

## 🏗️ Architecture

LoansWebApi/
│
├── Loans.Api/ → Controllers, middleware, Program.cs
├── Loans.Application/ → Services, DTOs, validators, logic
├── Loans.Domain/ → Entities, enums, error models
├── Loans.Infrastructure/ → EF Core, repositories, context, migrations
│
├── Loans.Tests.Unit/ → xUnit unit tests
└── Loans.Tests.Integration/ → API integration tests


### Principles Used

- **SOLID**
- **Clean Architecture**
- **CQRS-style service separation**
- **Repository pattern**
- **Dependency Injection everywhere**

---

## 🔐 Authentication & Authorization

### JSON Web Tokens (JWT)
- Issued on login
- Stored in `Authorization: Bearer <token>`
- Contains:
  - User ID
  - Username
  - Role

### Roles:
- **User**
- **Accountant**

Example:
```csharp
[Authorize(Roles = "User")]
[Authorize(Roles = "Accountant")]

| Component     | Technology                        |
| ------------- | --------------------------------- |
| Backend       | ASP.NET Core 8 Web API            |
| Database      | SQL Server + EF Core 8            |
| Auth          | JWT Bearer Authentication         |
| Logging       | NLog (File + Database Logging)    |
| Validation    | FluentValidation                  |
| Documentation | Swagger / OpenAPI                 |
| Tests         | xUnit, Moq, WebApplicationFactory |


| Location               | Description   |
| ---------------------- | ------------- |
| `/logs/yyyy-mm-dd.log` | File logs     |
| `Logs` table           | Database logs |
Configured in nlog.config.

❗ Global Error Handling

All errors are handled by custom middleware.

Standard response:

{
  "errorCode": "USER_NOT_FOUND",
  "message": "User not found",
  "statusCode": 404,
  "details": null,
  "timestamp": "2025-01-01T12:00:00Z"
}


Supports:

Validation errors

Not found errors

Unauthorized

Forbidden

Internal server errors

🧪 Tests
Unit Tests (Loans.Tests.Unit)

Services

Validators

Repositories (mocked)

Controllers (light)

Integration Tests (Loans.Tests.Integration)

Uses SQLite InMemory

Uses WebApplicationFactory

Covers:

Auth flow

Loan lifecycle

User restrictions

Role-based access

Error schema tests

Run all tests:

dotnet test

🛠️ Run the Project
1️⃣ Apply migrations
dotnet ef database update --project Loans.Infrastructure

2️⃣ Run API
dotnet run --project Loans.Api

3️⃣ Open Swagger
https://localhost:{port}/swagger

🔧 Configuration (appsettings.json)
"ConnectionStrings": {
  "sqlConnection": "Server=.;Database=LoansDb;Trusted_Connection=True;"
},
"Jwt": {
  "Key": "SUPER_SECRET_KEY_12345",
  "Issuer": "LoansApi",
  "Audience": "LoansApiUsers",
  "ExpiresInMinutes": 1440
}

📘 API Endpoints Summary
Auth
POST /api/auth/register
POST /api/auth/login

Users
GET /api/users/{id}
PUT /api/users/{id}/block   (Accountant)
PUT /api/users/{id}/unblock (Accountant)

Loans
POST   /api/loans                (User)
GET    /api/loans/{id}           (User/Accountant)
GET    /api/loans                (Accountant)
GET    /api/loans/user/{userId}  (Accountant)
PUT    /api/loans/{id}           (User/Accountant)
PUT    /api/loans/{id}/status    (Accountant)
DELETE /api/loans/{id}           (User/Accountant)

📬 Contact

If you want CI/CD, architecture diagrams, Postman collection, or advanced test coverage — just ask.

GitHub: https://github.com/edwardmrdoian/

Project: LoansWebApi
Developed by Edward Mrd 🚀
