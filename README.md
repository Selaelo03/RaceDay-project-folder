# RaceDay

RaceDay is an event management and race results system designed to allow participants to register for events, select categories, and view their results, while organisers can create and manage events, categories, enrolments and results.

## Project Structure

- `docs/` – ERD, API endpoint plan, SQL Server database script and CI/CD evidence
- `.github/workflows/` – GitHub Actions workflow for project validation
- `README.md` – Project documentation

## Main Features

- User registration and authentication
- Participant and organiser profiles
- Event management
- Event category management
- Participant enrolments
- Race result management
- SQL Server database
- REST API endpoint planning
- Automated GitHub repository validation using GitHub Actions

## User Roles

### Participant

Participants can register for the RaceDay system, manage their profile, view available events, enrol in events by selecting a category, and view their race results.

### Organiser

Organisers can create and manage events and categories, view participant enrolments, and record race results.

## Technologies

- SQL Server
- REST API
- GitHub
- GitHub Actions
- diagrams.net (draw.io)
- Microsoft SQL Server Management Studio (SSMS)

## Documentation

The project documentation is available in the `docs/` folder.

### ERD

The Entity Relationship Diagram defines the database structure, entities, primary keys, foreign keys and relationships used by the RaceDay system.

**File:** `docs/RaceDay_ERD.drawio.png`

### API Endpoint Plan

The API Endpoint Plan documents the REST API routes, HTTP methods, descriptions, required roles, request bodies and expected responses.

**File:** `docs/API_Endpoint_Plan_RaceDay.pdf`

### SQL Database Script

The SQL Server database script creates the RaceDay database, tables, relationships, constraints and sample data.

**File:** `docs/RaceDay_Database.sql`

## Database

The RaceDay system uses Microsoft SQL Server.

The database contains the following main entities:

- User
- Participant
- Organiser
- Event
- Category
- Enrollment
- Result

The database script includes sample data for organisers, participants, events, categories, enrolments and race results.

## CI/CD Validation

The project uses GitHub Actions to automatically validate the required repository structure and documentation files.

A successful GitHub Actions build is shown below:

![Successful GitHub Actions Build](docs/github-actions-green.png)

## YouTube Walkthrough

The RaceDay POE Part 1 walkthrough demonstrates the project documentation, ERD, API Endpoint Plan, SQL database script, database execution, GitHub repository and successful GitHub Actions validation.

[Watch the RaceDay POE Part 1 Walkthrough](https://youtu.be/-ABR4bDZkoE)

## Repository

The complete RaceDay project is available on GitHub:

https://github.com/Selaelo03/RaceDay-project-folder