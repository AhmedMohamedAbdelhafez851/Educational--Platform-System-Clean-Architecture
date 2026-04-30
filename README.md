# Online Exam System

An online examination platform built with **ASP.NET Core MVC**, designed using **Clean Architecture** principles to ensure scalability, maintainability, and separation of concerns.

The system enables administrators to create and manage exams, while users can take exams and receive immediate feedback on their performance.

---

## 🔗 Live Demo

You can try the system here:  
http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login

---

## 📌 Features

- User authentication and role-based authorization (Admin / User)
- Create, edit, and manage exams
- Add and manage multiple-choice questions
- Question bank for reusable questions
- Exam scheduling with time control
- Submit exams and calculate results automatically
- Review answers with correct and incorrect responses

---

## 🏗️ Architecture

The project follows **Clean Architecture** with clear separation between layers:

- **Presentation (MVC)** → Handles UI and user interaction  
- **Application** → Contains business logic (CQRS, validation)  
- **Domain** → Core entities and rules  
- **Infrastructure** → Database access and external services  

This structure improves testability, flexibility, and long-term maintainability.

---

## 🛠️ Technologies

- ASP.NET Core MVC  
- Entity Framework Core  
- SQL Server  
- ASP.NET Identity  
- Clean Architecture  
- CQRS Pattern  
- FluentValidation  
- Repository Pattern & Unit of Work  
- Bootstrap  
- Logging  

---

## ⚙️ Running the Project

1. Clone the repository  
02. Update the connection string in `appsettings.json`  
3. Apply database migrations  
4. Run the application  

```bash
dotnet ef database update
dotnet run
```

---

## 📂 Project Structure

```
/src
 ├── Web
 ├── Application
 ├── Domain
 ├── Infrastructure
```

---

## 📈 Notes

- The project is structured to support future extensions such as APIs or mobile clients  
- Designed with best practices commonly used in enterprise .NET applications  

---

## 📄 License

This project is open-source and available under the MIT License.
