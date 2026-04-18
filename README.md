# 🛒 Bulky - ASP.NET MVC E-Commerce Project

Bulky is a simple e-commerce web application built using **ASP.NET Core MVC** and **Entity Framework Core**.  
The project demonstrates CRUD operations, authentication, role management, and basic online shopping functionality.

---

##  Features

-  Product Management (Add / Edit / Delete / View)
-  Category Management
-  User Authentication & Authorization
-  Role-based access (Admin / Customer / Company)
-  Shopping cart functionality
-  Product images upload & display
-  Database integration using Entity Framework Core
-  Order management system (basic structure)

---

##  Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server / LocalDB
- C#
- HTML / CSS / Bootstrap
- JavaScript / jQuery

---
### 🛠️ Tech Stack

| Category | Technology |
| :--- | :--- |
| **Frontend** | ASP.NET Core MVC, Razor Pages, HTML, CSS, JavaScript |
| **Backend** | C#, ASP.NET Core 7 |
| **Database** | SQL Server, Entity Framework Core |
| **Authentication** | ASP.NET Core Identity |
| **Payment** | Stripe |
| **Email** | IEmailSender (Identity UI) |
| **Architecture** | Repository Pattern, Unit of Work, Dependency Injection |
| **Tools** | Visual Studio 2026 |

---

## 📸 Screenshots
<img width="1893" height="242" alt="Screenshot 2026-04-13 191557" src="https://github.com/user-attachments/assets/1606f242-9df7-4446-b3a8-3ef7c8c41ef5" />
<img width="1888" height="912" alt="Screenshot 2026-04-13 190909" src="https://github.com/user-attachments/assets/c76f0974-ac41-42cc-838a-9b57ab032463" />
<img width="1887" height="913" alt="Screenshot 2026-04-13 191050" src="https://github.com/user-attachments/assets/1eaf220e-3251-4e8c-8e9e-b20002c6cd66" />
<img width="1908" height="911" alt="Screenshot 2026-04-13 191034" src="https://github.com/user-attachments/assets/ebe2a05b-418c-456c-87dc-c3628e023a72" />
<img width="1904" height="907" alt="Screenshot 2026-04-13 191012" src="https://github.com/user-attachments/assets/1fdddad8-470b-454e-a6d1-11bc8d757a3e" />
<img width="1913" height="911" alt="Screenshot 2026-04-13 191000" src="https://github.com/user-attachments/assets/7936b52c-5529-4393-be7f-b48a20271d09" />
<img width="1916" height="852" alt="Screenshot 2026-04-13 190948" src="https://github.com/user-attachments/assets/f0dfc106-8a5f-4d61-b862-e87d8ca223db" />
<img width="1919" height="906" alt="Screenshot 2026-04-13 190931" src="https://github.com/user-attachments/assets/d5c1ad6c-ed62-49cb-8975-b6c9c631faae" />


##  Setup Instructions

<h3>This project uses SQL Server LocalDB by default to ensure a smooth setup for developers. The default server name used in this project is (localdb)\MSSQLLocalDB.</h3>

Connection String Setup
To run the project on your local machine, please follow these steps:

1- Clone the Repository

2-Download Database File (Bulky_2026-04-13.bak) and Restore it in your SSMS 

3-Open the appsettings.json file.

4-Locate the ConnectionStrings section.

5-Verify or update the server name to match your local SQL Server instance

## Configure Stripe Payment:

-  Step 1 — Add Stripe keys to appsettings.json
-  Open: appsettings.json
-  Add or update this section:
```json
{
  "Stripe": {
    "SecretKey": "your_secret_key",
    "PublishableKey": "your_publishable_key"
  }
}
```
👉 You get these keys from your Stripe Dashboard → Developers → API Keys.
- Step 2 — Add Stripe to Program.cs

- Open:
```
BulkyBookWeb/Program.cs
```
- Add:
```
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
```

<h2>This sets Stripe up globally so payment sessions can be created.</h2>

