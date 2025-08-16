# 🍽️ Restaurant Management System

## 📖 Overview
A **desktop application** for managing a restaurant with **online ordering**.  
Built with **WPF (C#)**, following the **MVVM pattern**, and backed by a **SQL Server 2022** database.  
The app supports both **customers** and **employees**, each with separate functionalities.

---

## 🚀 Features

### 🔵 Customer Features
- Browse restaurant menu with **categories, images, prices, allergens, and availability**  
- Advanced search & filtering by **name or allergens**  
- Account **creation, login, and profile management**  
- Place **food orders** with multiple items and quantities  
- Track **order status** in real-time: registered, preparing, out for delivery, delivered, canceled  
- View and manage **order history**, including cancellation of active orders  

### 🟠 Employee Features
- Full **CRUD operations** for **categories, dishes, menus, and allergens**  
- Manage **orders**: update statuses, view details, and customer information  
- **Generate reports** for inventory and orders  
- Monitor **low-stock dishes**  

### 🟡 Business Logic
- **Configurable discounts** based on order value or frequency  
- **Delivery charges** for small orders (configurable)  
- **Automatic stock updates** after orders are placed  
- Support for **menu combinations** with dynamic price calculations  

---

## 🛠️ Technologies Used
- **C# / WPF** (MVVM, Data Binding)  
- **SQL Server 2022** (stored procedures, normalized schema)  
- **ADO.NET**  
- **XAML** for UI design
