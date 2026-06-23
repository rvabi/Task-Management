CREATE DATABASE TaskManagementDb;
GO

USE TaskManagementDb;
GO

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE Tasks (
    TaskId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Todo',
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UserId INT NOT NULL,
    CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId),
    CONSTRAINT CK_Tasks_Status CHECK (Status IN ('Todo', 'In Progress', 'Done'))
);
GO

INSERT INTO Users (UserName, Email)
VALUES
('Abimanju', 'Abi@gmail.com'),
('Kirushigan ', 'Kirush@gmail.com'),
('Kamshigan', 'Kamshi@gmail.com');
GO

INSERT INTO Tasks (Title, Description, Status, UserId)
VALUES
('Create database', 'Create Users and Tasks tables', 'Done', 1),
('Build user API', 'Create user endpoints', 'In Progress', 1),
('Create frontend', 'Build HTML pages', 'Todo', 2),
('Test API', 'Use Postman to test endpoints', 'Todo', 3);
GO

SELECT * FROM Users;
SELECT * FROM Tasks;
GO