
### CQRS Implementation

- **Commands** - Create, Update, Delete operations with validation
- **Queries** - Read operations with caching for performance
- **MediatR** - Pipeline behaviors for logging, validation, and auditing
- **FluentValidation** - Automatic validation via pipeline behavior

### Design Patterns Used

| Pattern | Implementation |
|---------|----------------|
| Repository Pattern | Generic `Repository<T>` for data access |
| Unit of Work | `UnitOfWork` for transaction management |
| CQRS | MediatR for command/query separation |
| Dependency Injection | Built-in DI container |
| Options Pattern | Configuration management |
| Middleware Pipeline | Custom exception and logging middleware |

---

## 🛠️ Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core MVC | 8.0 | Web Framework |
| Entity Framework Core | 8.0 | ORM |
| MediatR | 12.1.1 | CQRS Pattern |
| FluentValidation | 11.9.0 | Input Validation |
| Serilog | Latest | Structured Logging |
| Bootstrap | 5.3 | UI Framework |
| DataTables | 1.13.4 | Advanced Tables |
| Chart.js | 4.4.0 | Analytics Charts |
| SQL Server | 2022+ | Database |
| jQuery | 3.6.0 | JavaScript Library |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or LocalDB)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) / VS Code / JetBrains Rider
- Git

### Installation

#### 1. Clone the repository

```bash
git clone https://github.com/yourusername/OnlineExamSystem.git
cd OnlineExamSystem
