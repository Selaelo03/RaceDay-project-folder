# RaceDay – Running Event Management System

## Project Overview

RaceDay is a web-based running event management system developed to support the organisation and management of running events.

The system is designed to provide functionality for participants and organisers, including user registration, authentication, event management, event categories, participant enrolments and race results.

The project was developed in stages as part of the Portfolio of Evidence (POE), covering planning and database design, API development, and the final web application.

## Project Objectives

The main objectives of RaceDay are to:

- Provide a system for managing running events.
- Support participant registration and authentication.
- Allow users to access and manage their profiles.
- Support event and event-category management.
- Allow participants to enrol in events.
- Support the recording and management of race results.
- Provide a web-based interface for interacting with the system.
- Apply software development and testing practices to improve system reliability.

---

## Repository Structure

The repository is organised into the following main folders and projects:

```text
RaceDay-project-folder/
│
├── .github/
│   └── workflows/
│       └── raceday-part2.yml
│
├── database/
│
├── docs/
│   ├── RaceDay_Database.sql
│   ├── RaceDay_ERD.drawio.png
│   └── API_Endpoint_Plan_RaceDay.pdf
│
├── RaceDay/
│   ├── RaceDay.slnx
│   │
│   ├── RaceDay.API/
│   │
│   ├── RaceDay.API.Tests/
│   │
│   └── RaceDay.Web/
│
└── README.md
```

### Main Components

| Component | Description |
|---|---|
| `.github/workflows` | Contains the GitHub Actions workflow configuration. |
| `database` | Contains database-related project resources. |
| `docs` | Contains project documentation, database design and API planning documents. |
| `RaceDay.API` | Contains the backend Web API. |
| `RaceDay.API.Tests` | Contains automated tests for selected application components. |
| `RaceDay.Web` | Contains the web application and user interface. |
| `RaceDay.slnx` | The solution file used to open the RaceDay projects in Visual Studio. |

---

# PART 1 – PLANNING AND DATABASE DESIGN

## 1. Introduction

Part 1 focused on planning and designing the RaceDay system before implementation.

The purpose of this phase was to identify the system requirements, establish the database structure and plan the API endpoints required for the application.

## 2. Database Design

The database was designed to support the main activities of a running event management system.

The design considers the relationships between users, events, categories, enrolments and results.

The database design documentation includes:

- Database SQL script.
- Entity Relationship Diagram (ERD).
- Database structure and relationships.

### Database

The application uses Microsoft SQL Server as its database management system.

**Database name:** `RaceDayDB`

### Database Documentation

The following documents are available in the `docs` folder:

- `RaceDay_Database.sql` – Database SQL script.
- `RaceDay_ERD.drawio.png` – Entity Relationship Diagram.
- `API_Endpoint_Plan_RaceDay.pdf` – Planned API endpoint documentation.

## 3. API Planning

The API endpoint plan was prepared to identify the operations required by the RaceDay system.

The planned functionality includes user management, event management, categories, enrolments and results.

This planning provided the foundation for the backend implementation in Part 2.

### Part 1 Demonstration Video

[RaceDay Part 1 – Planning and Database Design](https://youtu.be/-ABR4bDZkoE)

---

# PART 2 – BACKEND API DEVELOPMENT

## 1. Introduction

Part 2 focused on developing the backend functionality of the RaceDay system using ASP.NET Core Web API.

The API provides the services required by the application and communicates with the SQL Server database.

## 2. Technologies Used

The following technologies and tools were used:

| Technology | Purpose |
|---|---|
| C# | Main programming language. |
| .NET 10 | Development platform. |
| ASP.NET Core Web API | Backend API development. |
| Entity Framework Core | Database access and management. |
| Microsoft SQL Server | Database management system. |
| JWT | Authentication and access control. |
| Swagger | API documentation and testing. |
| Visual Studio 2026 | Development environment. |
| xUnit | Automated testing framework. |
| GitHub | Source code repository and version control. |
| GitHub Actions | Automated repository workflow. |

## 3. Backend Functionality

The backend was developed to support the main RaceDay system operations.

The implemented API functionality covers:

### User Management

- Participant registration.
- User login and authentication.
- User profile retrieval.
- User profile updates.
- Role-based access restrictions.

Public registration is restricted to the Participant role. Organiser registration is not available through the public registration process.

### Event Management

The API provides event-related functionality for retrieving and managing event information.

### Category Management

The system includes event categories and related category functionality.

### Enrolment Management

The API supports participant enrolment operations and the management of enrolment records.

### Results Management

The system includes functionality for managing race results and finishing positions.

## 4. Authentication and Authorisation

JWT authentication was implemented to support secure access to protected API functionality.

Users can authenticate through the login process and use the resulting authentication token when accessing protected endpoints.

Role restrictions are applied to control access to specific operations.

## 5. API Documentation

Swagger was used to document and test the available API endpoints.

It provides an interactive interface where API requests can be submitted and their responses inspected.

## 6. Testing and Verification

The API functionality was tested during development.

The following operations were successfully verified during the development process:

- Participant registration.
- User login.
- Authenticated API requests.
- Retrieving a user profile.
- Updating a user profile.
- Event enrolment operations.

### Part 2 Demonstration Video

[RaceDay Part 2 – RESTful API Demonstration](https://youtu.be/6zH9VTIa4e8)

---

# PART 3 – FINAL WEB APPLICATION

## 1. Introduction

Part 3 focused on completing and presenting the final RaceDay solution.

The existing project was extended to include the RaceDay.Web application alongside the backend API and automated testing project.

The final solution brings together the web interface, backend services and database integration.

## 2. Web Application

The `RaceDay.Web` project provides the user-facing part of the RaceDay system.

It is developed using ASP.NET Core MVC and provides the presentation layer through which users interact with the application.

The web project is separated from the API so that the user interface and backend functionality can be maintained as separate components.

## 3. Application Architecture

The final solution consists of three main projects:

### RaceDay.API

Responsible for backend operations, API endpoints, authentication and communication with the database.

### RaceDay.API.Tests

Contains automated tests used to verify selected application logic.

### RaceDay.Web

Provides the web interface and communicates with the backend services.

## 4. Database Integration

Entity Framework Core is used to support communication between the backend application and Microsoft SQL Server.

The application uses an application database context to manage database entities and their relationships.

The database used during development is `RaceDayDB`.

## 5. Security

JWT authentication supports secure access to protected API endpoints.

The system applies role-based restrictions to selected operations.

Public registration is limited to participants, while organiser registration is restricted.

## 6. Automated Testing

The `RaceDay.API.Tests` project contains automated tests written using xUnit.

The final automated test run completed with five successful tests.

The tests cover selected application logic, including:

- Event properties and updates.
- Category age limits.
- Enrolment references.
- Result finishing positions.

Automated testing helps verify that the tested components behave as expected.

## 7. Build Verification

The complete RaceDay solution was built using the .NET development environment.

The final build completed successfully without build errors.

The automated test run also completed successfully, with five tests passing.

## 8. Development Environment

The final solution was developed and verified using:

- Visual Studio 2026.
- .NET 10.
- C#.
- ASP.NET Core MVC.
- ASP.NET Core Web API.
- Entity Framework Core.
- Microsoft SQL Server.
- Swagger.
- xUnit.

## 9. Containerisation

Docker containerisation was not completed and is not included as an implemented feature in this submission.

The application was developed and verified using the .NET development environment.

### Part 3 Demonstration Video

[RaceDay Part 3 – Final POE Demonstration](https://youtu.be/CRuiwZqnej4)

---

# BUILDING AND RUNNING THE PROJECT

## Requirements

The following software is required to work with the project:

- Visual Studio 2026.
- .NET 10 SDK.
- Microsoft SQL Server.
- SQL Server Management Studio (SSMS), if required for database management.

## 1. Clone the Repository

Clone the repository from GitHub:

```bash
git clone https://github.com/Selaelo03/RaceDay-project-folder.git
```

## 2. Open the Solution

Navigate into the `RaceDay` folder and open:

```text
RaceDay.slnx
```

Open the solution using Visual Studio 2026.

## 3. Configure the Database

Ensure that Microsoft SQL Server is running.

Check the connection string in the API project's configuration and update it to match the local SQL Server instance.

The application uses the database:

```text
RaceDayDB
```

Apply the required Entity Framework Core migrations if the database has not already been created.

## 4. Restore Dependencies

From the solution directory, run:

```bash
dotnet restore RaceDay.slnx
```

## 5. Build the Solution

Run:

```bash
dotnet build RaceDay.slnx
```

## 6. Run Automated Tests

Run:

```bash
dotnet test RaceDay.slnx
```

The completed test run should report five successful tests.

## 7. Run the Application

Open the solution in Visual Studio 2026.

Configure `RaceDay.API` and `RaceDay.Web` as multiple startup projects, setting both to **Start**.

Run the solution using the Visual Studio Start button or F5.

The API and web application should launch in their respective browser or application windows.

Swagger can be accessed using the API's configured Swagger URL when the API is running.

---

# GITHUB REPOSITORY

**Repository:**  
https://github.com/Selaelo03/RaceDay-project-folder

**Final submission branch:**  
`part3-final-submission`

The repository contains the project source code, documentation, database design resources, API implementation, web application, automated tests and GitHub Actions workflow.

---

# PROJECT DEMONSTRATION VIDEOS

The following videos provide demonstrations of the RaceDay project across all three POE parts.

| Part | Description | Video |
|---|---|---|
| Part 1 | Planning and Database Design | https://youtu.be/-ABR4bDZkoE |
| Part 2 | RESTful API Development | https://youtu.be/6zH9VTIa4e8 |
| Part 3 | Final Web Application Demonstration | https://youtu.be/CRuiwZqnej4 |

---

# CONCLUSION

The RaceDay project demonstrates the development of a running event management system through three project phases.

Part 1 established the project requirements, database design and API plan.

Part 2 focused on implementing the backend API, database integration, authentication and core system operations.

Part 3 brought the components together through the final web application, automated testing and build verification.

The completed solution demonstrates practical application of software development, database design, API development, authentication, web application development and software testing.

**Project:** RaceDay – Running Event Management System  
**Repository:** https://github.com/Selaelo03/RaceDay-project-folder  
**Final Branch:** `part3-final-submission`