# ASP.NET Quiz Portal

##  Overview
The **Quiz Portal** is a web-based application developed using the **ASP.NET Framework**.  
It allows administrators to create, manage, and delete quizzes while enabling teachers to create quizes and students to participate and view results.  

This project is intentionally designed with **known security flaws** to serve as a **practice environment for web vulnerability assessment and penetration testing** in a controlled setting.

---

##  Features
- **User Authentication** – Login and session handling for both administrators and students.  
- **Quiz Management** – Create, edit, and delete quizzes with multiple questions.  
- **Role-Based Access Control** – Separate functionalities for Admins, Teacher and Students.  
- **Quiz Participation** – Students can take quizzes and view scores.
- **Quiz Creation** – Teachers can create quizzes and view scores.
- **Score Tracking** – Records and displays quiz results.

---

## 🛠 Technology Stack
- **Frontend:** ASP.NET Web Forms, HTML, CSS  
- **Backend:** ASP.NET Framework with VB.NET / C#  
- **Database:** Microsoft SQL Server  
- **Tools Used:** Visual Studio, SQL Server Management Studio (SSMS)

---

##  Known Vulnerabilities
This application is **deliberately insecure** and may contain:  
- SQL Injection  
- Cross-Site Scripting (XSS)  
- Weak authentication or session management  
- Misconfigured access controls  

> ⚠️ **Disclaimer:** This project is intended for **educational purposes only**. Do **not** deploy it on public-facing servers.

---

##  Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/YourUsername/aspnet-quiz-portal.git
````

### 2. Set Up the Database

1. Open `Database/QuizPortal.sql` in SQL Server Management Studio (SSMS).
2. Execute the script to create the database and tables.
3. Update the connection string in `Web.config` and all .aspx.vb files:

```xml
<connectionStrings>
  <add name="DefaultConnection" connectionString="Data Source=YOUR_SERVER\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;" providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 3. Open the Project

* Open the `.sln` solution file in **Visual Studio**.
* Run the project using **IIS Express** or your preferred web server.

---

##  Learning Use-Cases

* Practice **web vulnerability scanning** with tools like **OWASP ZAP**, **Burp Suite**, or **sqlmap**.
* Perform manual testing of SQL injection, XSS, and authentication flaws.
* Understand secure coding practices by fixing the vulnerabilities in a safe environment.

---

##  Folder Structure

```
QuizPortal/
├── Database/           # SQL scripts for database setup
│   └── QuizPortal.sql
├── Login.aspx          # Login page
├── ManageQuizzes.aspx  # Admin panel for quiz management
├── Web.config          # Connection string & app settings
├── StyleSheet.css      # Styles
├── bin/                # Compiled binaries (ignored in Git)
└── obj/                # Build files (ignored in Git)
```

---


##  Author

Developed by **Ehmaan Shafqat**
📧 Contact: emanshafqat9611@gmail.com

```

---

