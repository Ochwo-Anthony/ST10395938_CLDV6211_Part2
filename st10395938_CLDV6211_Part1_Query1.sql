--CREATE 'VENUE' TABLE
CREATE TABLE Venues (
    VenueId INT IDENTITY(1,1) PRIMARY KEY,
    VenueName VARCHAR(255) NOT NULL,
    VenueLocation VARCHAR(255) NOT NULL,
    VenueCapacity INT NOT NULL,
    ImageUrl VARCHAR(500)
);

--CREATE 'EVENT' TABLE
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    EventName VARCHAR(255) NOT NULL,
    EventDate DATE NOT NULL,
    EventDescription TEXT,
    VenueId INT,
	CONSTRAINT FK_Events_Venues FOREIGN KEY (VenueId) REFERENCES Venues(VenueId)
);

--CREATE 'BOOKING' TABLE
CREATE TABLE Bookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT,
    VenueId INT,
    BookingDate DATE NOT NULL,
    CONSTRAINT FK_Bookings_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_Bookings_Venues FOREIGN KEY (VenueId) REFERENCES Venues(VenueId)
);

-- SAMPLE DATA 'VENUE'
INSERT INTO Venues (VenueName, VenueLocation, VenueCapacity, ImageUrl) VALUES
('Grand Hall', 'Pretoria', 500, 'https://unsplash.com/photos/a-large-hall-with-a-clock-on-the-ceiling-Am6Vpx1RyUY'),
('Skyline Conference Center', 'Johannesburg', 300, 'https://unsplash.com/photos/a-room-filled-with-chairs-and-a-projector-screen-onxFGIkDvcI'),
('Oceanview Pavilion', 'Cape Town', 200, 'https://unsplash.com/photos/a-sandy-area-with-palm-trees-and-benches-hMdO_dYy0K0');

-- SAMPLE DATA 'EVENT'
INSERT INTO Events (EventName, EventDate, EventDescription, VenueId) VALUES
('Tech Conference 2025', '2025-06-15', 'A conference on emerging tech trends.', 1),
('Music Fest 2025', '2025-07-22', 'A festival featuring live bands.', 2),
('Business Summit', '2025-08-10', 'Annual business networking event.', 3);

-- SAMPLE DATA 'BOOKING'
INSERT INTO Bookings (EventId, VenueId, BookingDate) VALUES
(1, 1, '2025-06-01'),
(2, 2, '2025-07-05'),
(3, 3, '2025-07-20');

SELECT * FROM Venues
SELECT * FROM Events
SELECT * FROM Bookings