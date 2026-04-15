# 🏦 Loan Processing System

A comprehensive, full-stack banking and loan management application built with **ASP.NET Core 9.0 MVC**. This system streamlines the loan application lifecycle, automates client account generation, and provides robust administrative oversight using Role-Based Access Control (RBAC).

## ✨ Key Features

### 👤 Client Portal
* **Automated Onboarding:** Forced profile completion routing with image upload capabilities for identity verification.
* **Smart Bank Accounts:** Auto-generation of unique 10-digit account numbers upon account creation.
* **Loan Applications:** Seamless loan request workflow with support for TIN document uploads, specific loan types, and multiple guarantor tracking.
* **Real-Time Transactions:** Secure deposit and withdrawal processing with automated validation for insufficient funds.

### 🛡️ Administrative & Employee Workspace
* **Analytics Dashboard:** High-level metrics tracking total clients, branches, and aggregated data on dispersed loans (Sum, Max, Min, Average).
* **Advanced Employee Management:** Transactional CRUD operations for staff onboarding, utilizing SQL Stored Procedures to handle complex relational data (e.g., educational qualifications) atomically.
* **Loan Processing Engine:** Dedicated workflow for authorized employees to review, approve, or reject pending loan requests with administrative remarks.
* **System Configuration:** Dynamic management of Branch locations, Job Designations, Account Categories, and Loan Types (including interest rates).
* **Identity Management:** Admin-level control over user roles via ASP.NET Core Identity.

---

## 🗄️ Database Architecture & Relationships

This project utilizes **Entity Framework Core (Code-First)** heavily customized with the Fluent API to ensure strict data integrity and realistic ID generation.

* **Custom ID Seeding:** Primary keys are seeded to mimic enterprise banking systems (e.g., Clients start at `100,000`, Employees at `200,000`, Loans at `300,000`).
* **Identity Extension:** The default `ApplicationUser` has 1-to-1 relationships mapped to either a `Client` or `Employee` profile.
* **Financial Precision:** All currency fields (`Balance`, `DesiredLoanAmount`, `AnnualIncome`) are strictly enforced at `18,2` decimal precision.
* **Relational Mapping:** * `LoanRequest` contains 1-to-Many relationships with `Guarantors`.
  * `Employee` contains 1-to-Many relationships with `EducationalQualification`.
  * `Client` contains a 1-to-1 relationship with `EmploymentDetail`.

---

## 🛠️ Technical Stack

* **Framework:** ASP.NET Core 9.0 MVC
* **Authentication & Authorization:** Microsoft Identity (RBAC with Admin, Employee, and Client roles)
* **ORM:** Entity Framework Core
* **Database:** Microsoft SQL Server
* **Advanced Data Handling:** Custom SQL Stored Procedures (`sp_InsertEmployee`, `sp_UpdateEmployee`, `sp_InsertQualification`, `sp_DeleteEmployee`) for optimized, multi-table transactions.
* **Frontend:** Razor Views, Bootstrap, HTML5/CSS3.

---

## 🚀 Getting Started

### Prerequisites
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
* SQL Server (LocalDB or Express)
* Visual Studio 2022 or Visual Studio Code


