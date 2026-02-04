# Today's Feature Implementation - Logging with Serilog in ASP.NET Core 10 Web API

A modern ASP.NET Core 10 Web API project demonstrating best practices for logging implementation using both **Serilog (Static Logging)** and **Built-in ILogger (Dependency Injection)**.

## 🎯 Project Overview

This project is an Employee Management API that showcases advanced logging techniques with:
- Filtering, sorting, and pagination support
- Comprehensive error handling
- Structured logging for better observability
- Two different logging approaches for comparison

## 📋 Features

- **CRUD Operations** for Employee Management
- **Advanced Filtering** by Fname and Lname
- **Sorting** by Id, Fname, or Lname (ascending/descending)
- **Pagination** with customizable page size
- **AutoMapper** for DTO mapping
- **Entity Framework Core** with SQL Server
- **Swagger/OpenAPI** documentation

## 🔍 Logging Implementation

### 1. Static Logging with Serilog

**Location:** `Program.cs`

The project uses **Serilog** as a static logger with advanced configuration:
```
Log.Logger = new LoggerConfiguration() .ReadFrom.Configuration(configuration) .Enrich.FromLogContext() .Enrich.WithMachineName() .Enrich.WithEnvironmentUserName() .CreateLogger();
```


**Key Features:**
- ✅ Configuration loaded from `serilogsettings.json`
- ✅ Enriched with machine name and user context
- ✅ Application lifecycle logging (startup/shutdown)
- ✅ Fatal error logging in global exception handler

**Usage Example:**
```
Log.Information("Web server started"); Log.Fatal(ex, "Application failed to start"); Log.CloseAndFlush(); // Ensures logs are written before shutdown
```


**Advantages:**
- No dependency injection required
- Available throughout the application
- Easy to use in startup/shutdown scenarios
- Consistent static access pattern

---

### 2. Built-in ILogger with Dependency Injection

**Location:** `Controllers/EmployeeAPIController.cs`

The project demonstrates the **recommended approach** using ASP.NET Core's built-in `ILogger<T>`:
```
private readonly ILogger<EmployeeAPIController> logger;
public EmployeeAPIController(IEmployeeService employeeService, ILogger<EmployeeAPIController> logger) { this.employeeService = employeeService; this.logger = logger; }
```



**Logging Levels Used:**

| Level | Usage | Example |
|-------|-------|---------|
| `LogInformation` | Successful operations | `logger.LogInformation("Successfully fetched {Count} employees", result?.TotalCount ?? 0);` |
| `LogWarning` | Business logic issues | `logger.LogWarning("Employee with Id: {EmployeeId} not found", id);` |
| `LogError` | Exception handling | `logger.LogError(ex, "Error occurred while fetching employees");` |

**Structured Logging Examples:**

```
// GET - Detailed request logging logger.LogInformation("Fetching employees with PageNumber: {PageNumber}, PageSize: {PageSize}, FilterOn: {FilterOn}, FilterQuery: {FilterQuery}, SortBy: {SortBy}, IsAscending: {IsAscending}", pageNumber, pageSize, filterOn, filterQuery, sortBy, isAscending);
// POST - Entity creation logging logger.LogInformation("Adding employee with Name: {FirstName} {LastName}", emp.Fname, emp.Lname); logger.LogInformation("Employee added successfully with Id: {EmployeeId}", result.Id);
// PUT - Update tracking logger.LogInformation("Updating employee with Id: {EmployeeId}", id);
// DELETE - Deletion audit logger.LogInformation("Attempting to delete employee with Id: {EmployeeId}", id); logger.LogInformation("Employee with Id: {EmployeeId} deleted successfully", id);
// Error with context logger.LogError(ex, "Error occurred while updating employee with Id: {EmployeeId}", id);

```

**Advantages:**
- ✅ Testable and mockable
- ✅ Category-based logging (`ILogger<EmployeeAPIController>`)
- ✅ Structured logging with named parameters
- ✅ Better performance with log level checks
- ✅ Integrates seamlessly with DI container

---

## 🔄 Migration from Static to DI Logging

The codebase shows the **evolution from static Serilog to ILogger**:

**Commented Out (Old Approach):**
```
//Log.Information("Fetching employees with pagination"); //Log.Fatal(ex, "Error occurred while fetching employees");
```

**Current Implementation (Recommended):**
```
logger.LogInformation("Fetching employees with PageNumber: {PageNumber}...", pageNumber); logger.LogError(ex, "Error occurred while fetching employees with PageNumber: {PageNumber}", pageNumber);
```


**Why the Change?**
1. Better unit testing support
2. More detailed context in logs
3. Type-safe structured logging
4. Better separation of concerns
5. Follows ASP.NET Core best practices

---

## 📊 Logging Strategy

### Application Lifecycle (Static Logging)
- Application startup/shutdown
- Fatal errors that prevent app initialization
- Global error handling

### Request/Response Flow (ILogger DI)
- API endpoint requests
- Business logic operations
- Entity operations (CRUD)
- Validation warnings
- Operation-specific errors

---

## ⚙️ Configuration

### serilogsettings.json
Configure Serilog outputs, log levels, and enrichers:
```
{ "Serilog": { "MinimumLevel": "Information", "WriteTo": [ { "Name": "File", "Args": { "path": "Logs/log-.txt", "rollingInterval": "Day" } } ] } }
```

---

## 🎓 Learning Outcomes

This project demonstrates:

1. **Dual Logging Approaches**: Understanding when to use static vs DI logging
2. **Structured Logging**: Using named parameters for better log analysis
3. **Log Levels**: Appropriate use of Information, Warning, and Error levels
4. **Contextual Logging**: Adding relevant data to each log entry
5. **Error Handling**: Comprehensive exception logging with context
6. **Best Practices**: Following ASP.NET Core recommended patterns

---

Made with ❤️ while mastering Advanced ASP.NET Core Patterns

**Keep Building Enterprise-Grade APIs! 🚀**