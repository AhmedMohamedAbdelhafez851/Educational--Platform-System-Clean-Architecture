# 📚 Online Exam System

[![Live Demo](https://img.shields.io/badge/demo-live-green)](http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Arabic Support](https://img.shields.io/badge/lang-العربية-red)](http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login?culture=ar-EG)

## 🌐 Live Demo

**Access the live system:**  
👉 [[http://ahmedaabdelhafez-001-site1.atempur](http://hafez222-001-site1.ftempurl.com/)l.com/Account/Login](http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login)

**Test Credentials:**

| Role | Email | Password |
|------|-------|----------|
| Super Admin | superadmin@examify.com | SuperAdmin@123 |
| Admin | admin@site.com | Admin@123 |
| Teacher | teacher@examify.com | Teacher@123 |
| Student | student@examify.com | Student@123 |

> **Note:** Students can also take exams **without an account** using invitation codes from teachers.

---

## 📸 Screenshots

[![View Screenshots](https://img.shields.io/badge/📸-View_Screenshots-blue)](https://drive.google.com/drive/folders/1-5Ycqs2d_sFRvkHbtmSffQqoyoa8KeA7)

> **Click the link above** to view all system screenshots including:
> - Login & Authentication pages
> - Teacher Dashboard & Exam Management
> - Question Management Interface
> - Student Exam Taking Experience
> - Analytics & Reports Dashboard
> - Arabic RTL Interface

---

## ✨ Key Features

### 👨‍🏫 Teachers & Admins
| Feature | Description |
|---------|-------------|
| **Exam Creation** | Create exams with custom titles, time limits, and total scores |
| **Question Management** | Add multiple-choice questions with 4 answer options |
| **Invitation Links** | Generate unique 6-character codes to share with students |
| **Analytics Dashboard** | View student performance, scores, and attendance rates |
| **Score Distribution** | Visual breakdown of student scores with Chart.js |
| **Difficult Questions** | Identify questions with lowest correct answer rates |
| **Student Ranking** | See top performers and students needing attention |
| **Attendance Tracking** | Know exactly who attended vs who didn't |

### 👨‍🎓 Students
| Feature | Description |
|---------|-------------|
| **No Registration** | Take exams using invitation codes (no account needed) |
| **Timer Interface** | Real-time countdown with color warnings (green → orange → red) |
| **Question Navigator** | Quick jump between questions with status indicators |
| **Unsure Button** | Mark questions you're uncertain about (highlighted in orange) |
| **Instant Results** | See score and correct/incorrect answers immediately |
| **Answer Review** | Detailed breakdown of each question with correct answers |
| **Responsive Design** | Works perfectly on mobile, tablet, and desktop |

### 👑 System Admins
| Feature | Description |
|---------|-------------|
| **User Management** | Create and manage teachers, students, and admins |
| **Role-Based Access** | SuperAdmin, Admin, Teacher, Student roles |
| **System Dashboard** | Overview of total exams, questions, and active users |
| **Audit Logging** | Track all system actions for security |

### 🔧 Technical Highlights
| Feature | Description |
|---------|-------------|
| **Auto-Grading** | Exams are graded automatically upon submission |
| **Anonymous Taking** | Students can take exams without creating accounts |
| **Caching** | Frequently accessed data is cached for speed |
| **Multi-Language** | Full English and Arabic support with RTL layout |
| **Anti-Cheating** | Tab switching detection (auto-submit after 10 violations) |
| **Fullscreen Mode** | Recommended for exam integrity |

---

## 🛠️ Technology Stack

| Category | Technology | Version |
|----------|------------|---------|
| **Backend Framework** | ASP.NET Core MVC | 8.0 |
| **ORM** | Entity Framework Core | 8.0 |
| **Architecture** | Clean Architecture + CQRS | - |
| **Mediator** | MediatR | 12.1.1 |
| **Validation** | FluentValidation | 11.9.0 |
| **Logging** | Serilog | Latest |
| **Database** | SQL Server | 2022+ |
| **Frontend** | Bootstrap 5 | 5.3 |
| **JavaScript** | jQuery | 3.6.0 |
| **Tables** | DataTables | 1.13.4 |
| **Charts** | Chart.js | 4.4.0 |
| **Icons** | Bootstrap Icons | 1.11.0 |

---

## 🏗️ Architecture

### Clean Architecture Layers
