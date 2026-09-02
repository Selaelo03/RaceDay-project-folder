USE master;
GO

CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO


-- =============================================
-- 1. USER TABLE
-- =============================================

CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO


-- =============================================
-- 2. PARTICIPANT TABLE
-- =============================================

CREATE TABLE Participant (
    ParticipantID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    DateOfBirth DATE NOT NULL,
    PhoneNumber VARCHAR(20) NOT NULL,

    CONSTRAINT FK_Participant_User
        FOREIGN KEY (UserID)
        REFERENCES [User](UserID)
);
GO


-- =============================================
-- 3. ORGANISER TABLE
-- =============================================

CREATE TABLE Organiser (
    OrganiserID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    OrganisationName VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20) NOT NULL,

    CONSTRAINT FK_Organiser_User
        FOREIGN KEY (UserID)
        REFERENCES [User](UserID)
);
GO


-- =============================================
-- 4. EVENT TABLE
-- =============================================

CREATE TABLE [Event] (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    EventName VARCHAR(100) NOT NULL,
    Description VARCHAR(500),
    EventDate DATE NOT NULL,
    Location VARCHAR(200) NOT NULL,
    Distance DECIMAL(10,2) NOT NULL,
    EventType VARCHAR(50) NOT NULL,

    CONSTRAINT FK_Event_Organiser
        FOREIGN KEY (OrganiserID)
        REFERENCES Organiser(OrganiserID)
);
GO


-- =============================================
-- 5. CATEGORY TABLE
-- =============================================

CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName VARCHAR(100) NOT NULL,
    CategoryType VARCHAR(50) NOT NULL,
    MinimumAge INT,
    MaximumAge INT,
    Distance DECIMAL(10,2),

    CONSTRAINT FK_Category_Event
        FOREIGN KEY (EventID)
        REFERENCES [Event](EventID)
);
GO


-- =============================================
-- 6. ENROLLMENT TABLE
-- =============================================

CREATE TABLE Enrollment (
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrollmentDate DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Enrollment_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES Participant(ParticipantID),

    CONSTRAINT FK_Enrollment_Event
        FOREIGN KEY (EventID)
        REFERENCES [Event](EventID),

    CONSTRAINT FK_Enrollment_Category
        FOREIGN KEY (CategoryID)
        REFERENCES Category(CategoryID),

    CONSTRAINT UQ_Enrollment_Participant_Event
        UNIQUE (ParticipantID, EventID)
);
GO


-- =============================================
-- 7. RESULT TABLE
-- =============================================

CREATE TABLE Result (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentID INT NOT NULL,
    FinishTime TIME NOT NULL,
    FinishingPosition INT NOT NULL,

    CONSTRAINT FK_Result_Enrollment
        FOREIGN KEY (EnrollmentID)
        REFERENCES Enrollment(EnrollmentID),

    CONSTRAINT UQ_Result_Enrollment
        UNIQUE (EnrollmentID)
);
GO


-- =============================================
-- SAMPLE DATA
-- =============================================


-- =============================================
-- 8. TWO ORGANISERS
-- =============================================

INSERT INTO [User]
    (FirstName, LastName, Email, PasswordHash, Role)
VALUES
    ('John', 'Smith', 'john.smith@raceday.com',
     'hashed_password_123', 'Organiser'),

    ('Sarah', 'Williams', 'sarah.williams@raceday.com',
     'hashed_password_456', 'Organiser');
GO


INSERT INTO Organiser
    (UserID, OrganisationName, PhoneNumber)
VALUES
    (1, 'RaceDay Events', '0115551234'),

    (2, 'Running South Africa', '0115555678');
GO


-- =============================================
-- 9. TWO PARTICIPANTS
-- =============================================

INSERT INTO [User]
    (FirstName, LastName, Email, PasswordHash, Role)
VALUES
    ('David', 'Mokoena', 'david.mokoena@email.com',
     'hashed_password_789', 'Participant'),

    ('Lebo', 'Nkosi', 'lebo.nkosi@email.com',
     'hashed_password_101', 'Participant');
GO


INSERT INTO Participant
    (UserID, DateOfBirth, PhoneNumber)
VALUES
    (3, '2000-05-15', '0821112233'),

    (4, '1998-09-20', '0834445566');
GO


-- =============================================
-- 10. THREE EVENTS
-- =============================================

INSERT INTO [Event]
    (OrganiserID, EventName, Description, EventDate,
     Location, Distance, EventType)
VALUES
    (1, 'Johannesburg City Run',
     'Annual city running event',
     '2026-10-10',
     'Johannesburg',
     10.00,
     'Road Race'),

    (1, 'Pretoria Marathon',
     'Annual Pretoria marathon event',
     '2026-11-15',
     'Pretoria',
     42.20,
     'Marathon'),

    (2, 'Cape Town Fun Run',
     'Community fun running event',
     '2026-12-05',
     'Cape Town',
     5.00,
     'Fun Run');
GO


-- =============================================
-- 11. CATEGORIES FOR EACH EVENT
-- =============================================

INSERT INTO Category
    (EventID, CategoryName, CategoryType,
     MinimumAge, MaximumAge, Distance)
VALUES
    (1, 'Open 10K', 'Age', 18, 40, 10.00),

    (1, 'Masters 10K', 'Age', 41, 99, 10.00),

    (2, 'Open Marathon', 'Age', 18, 40, 42.20),

    (2, 'Masters Marathon', 'Age', 41, 99, 42.20),

    (3, 'Junior 5K', 'Age', 13, 17, 5.00),

    (3, 'Adult 5K', 'Age', 18, 99, 5.00);
GO


-- =============================================
-- 12. ENROLLMENTS
-- =============================================

INSERT INTO Enrollment
    (ParticipantID, EventID, CategoryID)
VALUES
    (1, 1, 1),

    (2, 1, 2),

    (1, 2, 3),

    (2, 3, 6);
GO


-- =============================================
-- 13. RESULTS
-- =============================================

INSERT INTO Result
    (EnrollmentID, FinishTime, FinishingPosition)
VALUES
    (1, '00:52:35', 1),

    (2, '00:58:42', 2),

    (3, '03:45:20', 1),

    (4, '00:28:15', 1);
GO