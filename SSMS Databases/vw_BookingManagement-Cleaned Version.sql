
-- ====================================================================================
-- 3. VIEW: vw_BookingManagement
-- ====================================================================================
DROP VIEW IF EXISTS vw_BookingManagement;
GO

CREATE VIEW vw_BookingManagement AS
SELECT 
    b.BookingID,

    -- Event Details
    e.EventID,
    e.Name AS EventName,
    e.StartDate AS EventDate,
    e.Notes AS EventNotes,
    e.EventTypeID,
    et.TypeName AS EventTypeName,

    -- Venue Details
    v.VenueID,
    v.Name AS VenueName,
    v.Location AS VenueLocation,
    v.Capacity AS VenueCapacity,
    v.ImageURL AS VenueImage,
    v.IsAvailable,

    -- Booking Details
    b.StartDate AS BookingStart,
    b.EndDate AS BookingEnd,
    DATEDIFF(HOUR, b.StartDate, b.EndDate) AS DurationHours,

    -- Derived Status
    CASE 
        WHEN b.StartDate < GETDATE() THEN 'Completed'
        WHEN b.StartDate <= DATEADD(DAY, 7, GETDATE()) THEN 'Upcoming (7 days)'
        ELSE 'Future Booking'
    END AS BookingStatus
FROM 
    Bookings b
    INNER JOIN Events e ON b.EventID = e.EventID
    INNER JOIN Venues v ON b.VenueID = v.VenueID
    LEFT JOIN EventTypes et ON e.EventTypeID = et.EventTypeID;
GO