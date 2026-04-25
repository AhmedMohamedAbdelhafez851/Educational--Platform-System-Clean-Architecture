# Online Exam System

This is an online exam system (educational platform) built using ASP.NET Core MVC and structured using Clean Architecture.

## Features

* User Authentication & Authorization (Admin / User)
* Exam Creation and Management
* Multiple Choice Questions
* Exam Submission
* Result Review
* Exam Scheduling and Time Control
* Question Bank Management

---

## Technologies

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* ASP.NET Identity
* Clean Architecture
* CQRS
* FluentValidation
* Repository Pattern
* Unit of Work
* Bootstrap
* Logging

---

## Run

1. Clone the project
2. Update connection string in `appsettings.json`
3. Run migrations
4. Start the application

```bash id="run03"
dotnet ef database update
dotnet run
```
