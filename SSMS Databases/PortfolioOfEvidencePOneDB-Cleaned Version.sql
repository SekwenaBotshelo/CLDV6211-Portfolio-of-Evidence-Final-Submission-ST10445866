-- ====================================================================================
-- 1. DATABASE AND TABLES
-- ====================================================================================
CREATE DATABASE PortfolioOfEvidencePOneDB;
GO

USE PortfolioOfEvidencePOneDB;
GO

-- Drop and recreate tables if needed (optional safety for re-runs)
DROP TABLE IF EXISTS Bookings;
DROP TABLE IF EXISTS Events;
DROP TABLE IF EXISTS Venues;
DROP TABLE IF EXISTS EventTypes;
GO

-- Table: Venues
CREATE TABLE Venues (
    VenueID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
    ImageURL NVARCHAR(255),
    IsAvailable BIT NOT NULL DEFAULT 1
);
GO

-- Table: Events
CREATE TABLE Events (
    EventID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Notes NVARCHAR(500),
    EventTypeID INT NULL
);
GO

-- Table: EventTypes (Lookup)
CREATE TABLE EventTypes (
    EventTypeID INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL
);
GO

-- Insert predefined Event Types
INSERT INTO EventTypes (TypeName)
VALUES 
    ('Conference'),
    ('Wedding'),
    ('Concert'),
    ('Workshop'),
    ('Birthday Party'),
    ('Other');
GO

-- Add FK from Events to EventTypes
ALTER TABLE Events
ADD CONSTRAINT FK_Events_EventTypes
    FOREIGN KEY (EventTypeID) REFERENCES EventTypes(EventTypeID)
    ON DELETE SET NULL;
GO

-- Table: Bookings
CREATE TABLE Bookings (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    VenueID INT NOT NULL,
    EventID INT NOT NULL UNIQUE,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    CONSTRAINT CHK_Bookings_ValidDates CHECK (StartDate < EndDate),
    CONSTRAINT FK_Bookings_Venue FOREIGN KEY (VenueID) REFERENCES Venues(VenueID) ON DELETE NO ACTION ON UPDATE CASCADE,
    CONSTRAINT FK_Bookings_Event FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE NO ACTION ON UPDATE CASCADE
);
GO

-- ====================================================================================
-- 2. INSERT SAMPLE DATA
-- ====================================================================================

-- Venues
INSERT INTO Venues (Name, Location, Capacity, ImageURL)
VALUES 
('Grand Hall', '123 City Center, Cape Town', 500, 'https://example.com/images/grandhall.jpg'),
('Beachside Pavilion', '77 Ocean Drive, Durban', 300, 'https://example.com/images/beachside.jpg'),
('Skyline Rooftop', '99 High Street, Johannesburg', 150, 'https://example.com/images/skyline.jpg');
GO

-- Events
INSERT INTO Events (Name, StartDate, EndDate, Notes)
VALUES 
('Corporate Year-End Function', '2025-06-20', '2025-06-20', 'Private corporate event for Acme Inc.'),
('Wedding Celebration', '2025-07-15', '2025-07-15', 'Couple wedding reception'),
('Product Launch Party', '2025-08-05', '2025-08-05', 'Launch event for new mobile device');
GO

-- Assign Event Types
UPDATE Events SET EventTypeID = 1 WHERE Name = 'Corporate Year-End Function'; -- Conference
UPDATE Events SET EventTypeID = 2 WHERE Name = 'Wedding Celebration';         -- Wedding
UPDATE Events SET EventTypeID = 3 WHERE Name = 'Product Launch Party';        -- Concert
GO

-- Bookings
INSERT INTO Bookings (VenueID, EventID, StartDate, EndDate)
VALUES 
(1, 1, '2025-06-20 14:00:00', '2025-06-20 23:00:00'),
(2, 2, '2025-07-15 10:00:00', '2025-07-15 22:00:00'),
(3, 3, '2025-08-05 18:00:00', '2025-08-05 23:00:00');
GO

-- ====================================================================================
-- 4. STORED PROCEDURE: sp_SearchBookings
-- ====================================================================================
DROP PROCEDURE IF EXISTS sp_SearchBookings;
GO

CREATE PROCEDURE sp_SearchBookings
    @SearchTerm NVARCHAR(100) = NULL,
    @BookingID INT = NULL,
    @EventName NVARCHAR(100) = NULL,
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL,
    @EventTypeID INT = NULL,
    @VenueAvailableOnly BIT = NULL
AS
BEGIN
    SELECT * FROM vw_BookingManagement
    WHERE 
        (@SearchTerm IS NULL OR 
         EventName LIKE '%' + @SearchTerm + '%' OR 
         VenueName LIKE '%' + @SearchTerm + '%')
        AND (@BookingID IS NULL OR BookingID = @BookingID)
        AND (@EventName IS NULL OR EventName LIKE '%' + @EventName + '%')
        AND (@DateFrom IS NULL OR BookingStart >= @DateFrom)
        AND (@DateTo IS NULL OR BookingEnd <= @DateTo)
        AND (@EventTypeID IS NULL OR EventTypeID = @EventTypeID)
        AND (@VenueAvailableOnly IS NULL OR IsAvailable = @VenueAvailableOnly)
    ORDER BY BookingStart DESC;
END;
GO
