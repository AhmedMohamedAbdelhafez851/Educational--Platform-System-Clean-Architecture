# 📚 Online Exam System - Educational Platform

[![Live Demo](https://img.shields.io/badge/demo-live-green)](http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## 🌐 Live Demo

**Access the live system:**  
👉 [http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login](http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login)

---

## 📖 Table of Contents

- [System Overview](#system-overview)
- [Key Features](#key-features)
- [User Roles & Access](#user-roles--access)
- [Step-by-Step User Guides](#step-by-step-user-guides)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Installation Guide](#installation-guide)
- [Deployment](#deployment)
- [Support](#support)

---

## 🎯 System Overview

The **Online Exam System** is a comprehensive, production-ready educational platform that allows teachers to create exams, generate invitation links, and analyze student performance — all without students needing to create accounts.

**Built for:** Schools, academies, training centers, and universities  
**Languages:** English & Arabic (Full RTL Support)  
**Access:** Web-based, responsive on all devices

---

## ✨ Key Features

### 📝 For Teachers & Admins

| Feature | Description |
|---------|-------------|
| **Exam Creation** | Create exams with custom titles, time limits, and total scores |
| **Question Management** | Add multiple-choice questions with 4 answer options |
| **Invitation Links** | Generate unique 6-character codes to share with students |
| **Real-time Analytics** | View student performance, scores, and attendance rates |
| **Score Distribution Chart** | Visual breakdown of student scores (0-20%, 20-40%, etc.) |
| **Most Difficult Questions** | Identify questions with lowest correct answer rates |
| **Student Ranking** | See top performers and students needing attention |
| **Attendance Tracking** | Know exactly who attended and who didn't |
| **Exam Reports** | Detailed performance reports per exam |

### 👨‍🎓 For Students

| Feature | Description |
|---------|-------------|
| **No Registration Required** | Take exams using invitation codes (no account needed) |
| **Timer Interface** | Real-time countdown with color warnings (green → orange → red) |
| **Question Navigator** | Quick jump between questions with status indicators |
| **Unsure Button** | Mark questions you're uncertain about (highlighted in orange) |
| **Instant Results** | See score and correct/incorrect answers immediately |
| **Answer Review** | Detailed breakdown of each question with correct answers |
| **Responsive Design** | Works perfectly on mobile, tablet, and desktop |

### 👑 For Admins

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
| **Anti-Cheating** | Tab switching detection (after 10 violations, auto-submit) |
| **Fullscreen Mode** | Recommended for exam integrity |
| **Responsive UI** | Works on all screen sizes |

---

## 👥 User Roles & Access

| Role | Default Email | Default Password | Access Level |
|------|---------------|-----------------|--------------|
| **Super Admin** | superadmin@examify.com | SuperAdmin@123 | Full system control |
| **Admin** | admin@site.com | Admin@123 | Admin dashboard + analytics |
| **Teacher** | teacher@examify.com | Teacher@123 | Create exams, manage questions, view analytics |
| **Student** | student@examify.com | Student@123 | Take exams only (account option) |

> **Note:** Students can also take exams **without an account** using invitation codes from teachers.

---

## 📋 Step-by-Step User Guides

### Guide 1: How to Log In (For Teachers & Admins)

1. Go to: [http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login](http://ahmedaabdelhafez-001-site1.atempurl.com/Account/Login)
2. Enter your email and password
3. Click "Login"
4. You will be redirected to your role-based dashboard

### Guide 2: How to Create an Exam (For Teachers)

1. After logging in, click **"امتحان جديد"** (Create Exam)
2. Fill in:
   - **Exam Title** (e.g., "Mathematics Final Exam")
   - **Time (Minutes)** (e.g., 60 minutes)
   - **Total Degree** (e.g., 100 points)
3. Click **"إنشاء الامتحان"** (Create Exam)
4. Your exam now appears in the exams list

### Guide 3: How to Add Questions to an Exam

1. Find your exam in the list
2. Click the **"ثلاث نقاط"** (three dots ⋮) menu
3. Select **"إدارة الأسئلة"** (Manage Questions)
4. Click **"إضافة سؤال"** (Add Question)
5. Enter:
   - **Question Title** (e.g., "What is 2+2?")
   - **4 Answer Choices** (e.g., "1, 2, 3, 4")
   - Select which choice is **correct** by clicking the radio button
6. Click **"حفظ"** (Save)
7. Repeat for more questions

### Guide 4: How to Generate an Invitation Link

1. Go to Exams list
2. Click the **"ثلاث نقاط"** (three dots ⋮) menu
3. Select **"روابط الدعوة"** (Invitation Links)
4. Click **"Generate New Link"**
5. Set options (optional):
   - **Expiration Date** (leave empty for no expiry)
   - **Max Attempts** (default is 1)
6. Click **"Generate Link"**
7. Copy the generated link or code (e.g., `ABC123`)
8. Share the link or code with your students

### Guide 5: How a Student Takes an Exam

**Option A: Using the Link (Recommended)**

1. Student clicks the invitation link (e.g., `https://yourdomain.com/UserExam/Join?code=ABC123`)
2. Student enters:
   - **Full Name** (required)
   - **Email** (optional)
   - **Student ID** (optional)
3. Click **"Start Exam"**

**Option B: Using the Code Only**

1. Student goes to: `http://ahmedaabdelhafez-001-site1.atempurl.com/UserExam/Join`
2. Enters the 6-character invitation code (e.g., `ABC123`)
3. Enters name and optional details
4. Click **"Start Exam"**

### Guide 6: How to Take the Exam

Once the exam starts:

1. **Timer** shows remaining time (turns orange at 30%, red at 10%)
2. **Answer questions** by clicking on your choice
3. **Mark unsure questions** by clicking the "غير متأكد" (Unsure) button
4. **Navigate** using the question navigator on the right
5. The navigator shows:
   - ✅ **Green** = Answered
   - ❓ **Orange** = Unsure
   - 🟢 **Green + Orange** = Answered but unsure
   - 🔵 **Blue** = Current question
6. Click **"Submit Exam"** when finished
7. Confirm submission

### Guide 7: How to View Exam Results (Student)

After submission:

1. Results page shows immediately:
   - **Final Score** (percentage)
   - **Pass/Fail** status (Pass = 50% or higher)
   - **Correct Answers** count
   - **Incorrect Answers** count
2. Scroll down to see **detailed answer review**:
   - Each question shows your answer and the correct answer
   - Green = Correct, Red = Incorrect
3. Click **"Print Result"** to save or print

### Guide 8: How to View Analytics (Teacher)

1. Go to Exams list
2. Click the **"ثلاث نقاط"** (three dots ⋮) menu
3. Select **"عرض التحليلات"** (View Analytics)
4. The analytics dashboard shows:
   - **Total Students** who took the exam
   - **Average Score**
   - **Pass Rate**
   - **Attendance Rate**
   - **Score Distribution Chart** (bar chart)
   - **Most Difficult Questions** (lowest correct percentages)
   - **Top Performing Students** (highest scores)
   - **Students Needing Attention** (lowest scores)
   - **Complete Attendance List** (who attended vs who didn't)

### Guide 9: How to Enable/Disable Invitation Links

1. Go to Invitations page for an exam
2. Find the invitation link you want to modify
3. Click the **Enable/Disable** button
4. When disabled, students cannot use that link anymore

### Guide 10: How to Change Language

1. Look for the language switcher in the top navigation bar
2. Click to toggle between:
   - 🇺🇸 **English**
   - 🇪🇬 **العربية (Arabic)**
3. The entire interface switches instantly

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
| **Frontend** | Bootstrap 5, jQuery | 5.3 / 3.6 |
| **Tables** | DataTables | 1.13.4 |
| **Charts** | Chart.js | 4.4.0 |
| **Icons** | Bootstrap Icons | 1.11.0 |

---

## 🏗️ Architecture

### Clean Architecture Layers
