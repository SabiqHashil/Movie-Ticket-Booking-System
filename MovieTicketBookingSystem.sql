CREATE DATABASE MovieTicketBookingSystem;
GO

USE MovieTicketBookingSystem;
GO

CREATE TABLE Users (
	UserID INT IDENTITY(1,1) PRIMARY KEY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	DateOfBirth DATE NOT NULL,
	Gender VARCHAR(10) NOT NULL CHECK (Gender IN ('Male', 'Female', 'Other')),
	PhoneNumber VARCHAR(10) NOT NULL,
	EmailAddress VARCHAR(255) NOT NULL UNIQUE,
	Address VARCHAR(255) NOT NULL,
	State VARCHAR(50) NOT NULL,
	City VARCHAR(50) NOT NULL,
	Username VARCHAR(50) NOT NULL UNIQUE,
	Password VARCHAR(255) NOT NULL,
	Role VARCHAR(20) NOT NULL CHECK (Role IN ('User', 'Admin')),
	CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME DEFAULT GETDATE()
);
GO



SELECT * FROM Users;

-- Signup Procedure
ALTER PROCEDURE sp_SignUp
    @FirstName VARCHAR(100),
    @LastName VARCHAR(100),
    @DateOfBirth DATE,
    @Gender VARCHAR(10),
    @PhoneNumber VARCHAR(10),
    @Email VARCHAR(100),
    @Address VARCHAR(250),
    @State VARCHAR(50),
    @City VARCHAR(50),
    @Username VARCHAR(50),
    @Password VARCHAR(255)
AS
BEGIN
    INSERT INTO Users (FirstName, LastName, DateOfBirth, Gender, PhoneNumber, EmailAddress, Address, State, City, Username, Password)
    VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @PhoneNumber, @Email, @Address, @State, @City, @Username, @Password)
END


EXEC sp_Login @Username = 'sbqhashil';
-- Login Procedure
ALTER PROCEDURE sp_Login
    @Username VARCHAR(50),
	@Password VARCHAR(255)
AS
BEGIN
    SELECT UserID, FirstName, LastName, Username
    FROM Users
    WHERE Username = @Username
END

-- Movie Table
CREATE TABLE Movies (
    MovieID INT PRIMARY KEY,
    MovieName VARCHAR(255) NOT NULL,
    Genre VARCHAR(100),
    Duration INT,
    Language VARCHAR(50),
    Description TEXT,
    Image VARCHAR(MAX)
);


--Insert Movie procedure
CREATE PROCEDURE sp_InsertMovie
    @MovieName NVARCHAR(255),
    @Genre NVARCHAR(100),
    @Duration INT,
    @Language NVARCHAR(50),
    @Description TEXT,
    @Image NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Movies (MovieName, Genre, Duration, Language, Description, Image)
    VALUES (@MovieName, @Genre, @Duration, @Language, @Description, @Image)
END

--Get Movies procedure
ALTER PROCEDURE sp_GetMovies
AS
BEGIN
	SELECT MovieID, MovieName, Genre, Duration, Language, Description, Image
	FROM Movies
	ORDER BY MovieID DESC 
END

EXEC sp_GetMovies

--Update Movie Procedure
ALTER PROCEDURE sp_UpdateMovie
    @MovieID INT,
    @MovieName NVARCHAR(MAX),
    @Genre NVARCHAR(MAX),
    @Duration INT,
    @Language NVARCHAR(MAX),
    @Description NVARCHAR(MAX),
    @Image NVARCHAR(MAX)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Movies WHERE MovieID = @MovieID)
    BEGIN
        UPDATE Movies
        SET
            MovieName = @MovieName,
            Genre = @Genre,
            Duration = @Duration,
            Language = @Language,
            Description = @Description,
            Image = @Image
        WHERE
            MovieID = @MovieID;

        PRINT 'Movie updated successfully.';
    END
    ELSE
    BEGIN
        PRINT 'Movie not found.';
    END
END

--Get Movie By ID
CREATE PROCEDURE sp_GetMovieById
    @MovieID INT
AS
BEGIN
    SELECT 
        MovieID,
        MovieName,
        Genre,
        Duration,
        Language,
        Description,
        Image
    FROM 
        Movies
    WHERE 
        MovieID = @MovieID;
END;


--Delete Movie procedure
ALTER PROCEDURE sp_DeleteMovie
    @MovieID INT
AS
BEGIN
    DELETE FROM Movies WHERE MovieID = @MovieID;
END;

-- Get All Movies procedure
ALTER PROCEDURE GetAllMovies
AS
BEGIN
    SELECT MovieID, MovieName, Genre, Duration, Language, Description, Image
    FROM Movies
	ORDER BY MovieID DESC
END

EXEC GetAllMovies

-- GetMovieByID
CREATE PROCEDURE GetMovieById
    @MovieID INT
AS
BEGIN
    SELECT * FROM Movies WHERE MovieID = @MovieID
END


--Create table Theater
CREATE TABLE Theater (
    TheaterID INT IDENTITY(1,1) PRIMARY KEY,
    TheaterName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    ClassA_Rate DECIMAL(10, 2) NOT NULL,
    ClassA_SeatCount INT NOT NULL,
    ClassB_Rate DECIMAL(10, 2) NOT NULL,
    ClassB_SeatCount INT NOT NULL,
    ClassC_Rate DECIMAL(10, 2) NOT NULL,
    ClassC_SeatCount INT NOT NULL
);

-- View all theaters procedure
ALTER PROCEDURE GetAllTheaters
AS
BEGIN
    SELECT 
        TheaterID,
        TheaterName,
        Location,
        ClassA_Rate,
        ClassA_SeatCount,
        ClassB_Rate,
        ClassB_SeatCount,
        ClassC_Rate,
        ClassC_SeatCount
    FROM Theater
	ORDER BY TheaterName ASC;
END;

--Add Theater procedure
ALTER PROCEDURE AddTheater
    @TheaterName NVARCHAR(100),
    @Location NVARCHAR(200),
    @ClassA_Rate DECIMAL(10, 2),
    @ClassA_SeatCount INT,
    @ClassB_Rate DECIMAL(10, 2),
    @ClassB_SeatCount INT,
    @ClassC_Rate DECIMAL(10, 2),
    @ClassC_SeatCount INT
AS
BEGIN
    INSERT INTO Theater (TheaterName, Location, ClassA_Rate, ClassA_SeatCount, ClassB_Rate, ClassB_SeatCount, ClassC_Rate, ClassC_SeatCount)
    VALUES (@TheaterName, @Location, @ClassA_Rate, @ClassA_SeatCount, @ClassB_Rate, @ClassB_SeatCount, @ClassC_Rate, @ClassC_SeatCount);

    SELECT SCOPE_IDENTITY() AS NewTheaterID;
END;


-- Get Theater By ID
ALTER PROCEDURE GetTheaterById
    @TheaterID INT
AS
BEGIN
    SELECT 
        TheaterID,
        TheaterName,
        Location,
        ClassA_Rate,
        ClassA_SeatCount,
        ClassB_Rate,
        ClassB_SeatCount,
        ClassC_Rate,
        ClassC_SeatCount
    FROM Theater
    WHERE TheaterID = @TheaterID;
END;

-- Update Theater procedure
ALTER PROCEDURE sp_UpdateTheater
    @TheaterID INT,
    @TheaterName NVARCHAR(100),
    @Location NVARCHAR(200),
    @ClassA_Rate DECIMAL(10, 2),
    @ClassA_SeatCount INT,
    @ClassB_Rate DECIMAL(10, 2),
    @ClassB_SeatCount INT,
    @ClassC_Rate DECIMAL(10, 2),
    @ClassC_SeatCount INT
AS
BEGIN
    UPDATE Theater
    SET 
        TheaterName = @TheaterName,
        Location = @Location,
        ClassA_Rate = @ClassA_Rate,
        ClassA_SeatCount = @ClassA_SeatCount,
        ClassB_Rate = @ClassB_Rate,
        ClassB_SeatCount = @ClassB_SeatCount,
        ClassC_Rate = @ClassC_Rate,
        ClassC_SeatCount = @ClassC_SeatCount
    WHERE TheaterID = @TheaterID;

    SELECT SCOPE_IDENTITY() AS UpdatedTheaterID;
END;


--Delete Theater procedure
CREATE PROCEDURE sp_DeleteTheater
    @TheaterID INT
AS
BEGIN
    DELETE FROM Theaters WHERE TheaterID = @TheaterID;
END;


--Schedule Table
CREATE TABLE Schedule (
    ScheduleID INT IDENTITY(1,1) PRIMARY KEY,
    MovieID INT NOT NULL,
    TheaterID INT NOT NULL,
    Class CHAR(1) NOT NULL CHECK (Class IN ('A', 'B', 'C')),
    ShowTime TIME NOT NULL,
    Date DATE NOT NULL,
    FOREIGN KEY (TheaterID) REFERENCES Theater(TheaterID)
);


--View All Schedules procedure
ALTER PROCEDURE ViewAllSchedules
AS
BEGIN
    SELECT 
        S.ScheduleID,
        M.MovieName,
        M.Image AS MovieImage,
        T.TheaterName,
        T.Location,
        S.Class,
		        -- Total seat count based on class
        CASE 
            WHEN S.Class = 'A' THEN T.ClassA_SeatCount
            WHEN S.Class = 'B' THEN T.ClassB_SeatCount
            WHEN S.Class = 'C' THEN T.ClassC_SeatCount
        END AS TotalSeatCount,
		        -- Remaining seat count after booked seats
        CASE 
            WHEN S.Class = 'A' THEN T.ClassA_SeatCount - ISNULL(( 
                SELECT SUM(TK.NumberOfSeats)
                FROM Tickets TK
                WHERE TK.ScheduleID = S.ScheduleID AND TK.SeatClass = 'A'
            ), 0)
            WHEN S.Class = 'B' THEN T.ClassB_SeatCount - ISNULL(( 
                SELECT SUM(TK.NumberOfSeats)
                FROM Tickets TK
                WHERE TK.ScheduleID = S.ScheduleID AND TK.SeatClass = 'B'
            ), 0)
            WHEN S.Class = 'C' THEN T.ClassC_SeatCount - ISNULL(( 
                SELECT SUM(TK.NumberOfSeats)
                FROM Tickets TK
                WHERE TK.ScheduleID = S.ScheduleID AND TK.SeatClass = 'C'
            ), 0)
        END AS RemainingSeats,
        S.ShowTime,
        S.Date,
		        -- Ticket rate based on class
        CASE 
            WHEN S.Class = 'A' THEN T.ClassA_Rate
            WHEN S.Class = 'B' THEN T.ClassB_Rate
            WHEN S.Class = 'C' THEN T.ClassC_Rate
        END AS TicketRate
    FROM 
        Schedule S
    INNER JOIN 
        Movies M ON S.MovieID = M.MovieID
    INNER JOIN 
        Theater T ON S.TheaterID = T.TheaterID
    ORDER BY
        S.ScheduleID DESC; -- (newest first)
END;


EXEC ViewAllSchedules

--Insert Schedule
CREATE PROCEDURE InsertSchedule
	@MovieID INT,
	@TheaterID INT,
	@Class CHAR(1),
	@ShowTime TIME,
	@Date DATE
AS
BEGIN
	INSERT INTO Schedule (MovieID, TheaterID, Class, ShowTime, Date)
	VALUES (@MovieID, @TheaterID, @Class, @ShowTime, @Date);

	SELECT SCOPE_IDENTITY() AS ScheduleID;
END;


--Get Scheduled By ID
ALTER PROCEDURE GetScheduleByID
    @ScheduleID INT
AS
BEGIN
   SET NOCOUNT ON;

    SELECT 
        S.ScheduleID, 
        M.MovieName, 
        T.TheaterName, 
        T.Location, 
        S.Class, 
        T.ClassA_Rate AS ClassRate, 
        S.ShowTime, 
        S.Date, 
        (T.ClassA_SeatCount + T.ClassB_SeatCount + T.ClassC_SeatCount) AS TotalSeatCount,
        (T.ClassA_SeatCount + T.ClassB_SeatCount + T.ClassC_SeatCount) - (SELECT COUNT(*) FROM Tickets WHERE ScheduleID = S.ScheduleID) AS RemainingSeats
    FROM 
        Schedule S
    INNER JOIN 
        Theater T ON S.TheaterID = T.TheaterID
    INNER JOIN 
        Movies M ON S.MovieID = M.MovieID
    WHERE 
        S.ScheduleID = @ScheduleID
    AND 
        S.Date >= GETDATE();  
END;

-- Update Schedule
ALTER PROCEDURE UpdateSchedule
    @ScheduleID INT,
    @MovieID INT,
    @TheaterID INT,
    @Class CHAR(1),
    @ShowTime TIME,
    @Date DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Update the Schedule table with the provided parameters
    UPDATE Schedule
    SET 
        MovieID = @MovieID,
        TheaterID = @TheaterID,
        Class = @Class,
        ShowTime = @ShowTime,
        Date = @Date
    WHERE 
        ScheduleID = @ScheduleID;

    -- Return the updated Schedule details
    SELECT 
        S.ScheduleID, 
        M.MovieName, 
        T.TheaterName, 
        T.Location, 
        S.Class, 
        T.ClassA_Rate AS ClassRate, 
        S.ShowTime, 
        S.Date, 
        (T.ClassA_SeatCount + T.ClassB_SeatCount + T.ClassC_SeatCount) AS TotalSeatCount,
        (T.ClassA_SeatCount + T.ClassB_SeatCount + T.ClassC_SeatCount) - (SELECT COUNT(*) FROM Tickets WHERE ScheduleID = S.ScheduleID) AS RemainingSeats
    FROM 
        Schedule S
    INNER JOIN 
        Theater T ON S.TheaterID = T.TheaterID
    INNER JOIN 
        Movies M ON S.MovieID = M.MovieID
    WHERE 
        S.ScheduleID = @ScheduleID;
END;



--Delete Schedule
ALTER PROCEDURE DeleteSchedule
    @ScheduleID INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Schedule
    WHERE ScheduleID = @ScheduleID;

    IF @@ROWCOUNT = 0
    BEGIN
        RETURN 0;
    END
    ELSE
    BEGIN
        RETURN 1;
    END
END


SELECT * FROM Schedule

SELECT * FROM Movies


-- UserShow procedure
ALTER PROCEDURE sp_UserShow
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        M.Image AS MovieImage,
        S.MovieID,
        M.MovieName,
        MAX(S.Date) AS Date  
    FROM 
        Schedule S
    INNER JOIN 
        Movies M ON S.MovieID = M.MovieID
    WHERE 
        S.Date >= GETDATE()
    GROUP BY 
        S.MovieID, M.Image, M.MovieName  
    ORDER BY
        MAX(S.Date) DESC; 
END;


EXEC sp_UserShow

--Movie Details
CREATE PROCEDURE sp_GetMovieDetails
    @MovieID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        M.MovieID,
        M.MovieName,
        M.Genre,
        M.Duration,
        M.Language,
        M.Description,
        M.Image
    FROM 
        Movies M
    WHERE 
        M.MovieID = @MovieID;
END;

--Table of Tickets
CREATE TABLE Tickets (
    TicketID INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleID INT NOT NULL, 
    UserID INT NOT NULL,     
    SeatClass CHAR(1) NOT NULL CHECK (SeatClass IN ('A', 'B', 'C')), 
    NumberOfSeats INT NOT NULL, 
    TotalPrice DECIMAL(10, 2) NOT NULL, 
    BookingDate DATETIME DEFAULT GETDATE(),  
	TicketNumber NVARCHAR(50) NULL,
    FOREIGN KEY (ScheduleID) REFERENCES Schedule(ScheduleID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Scheduled Movie Details for Booking procedure
ALTER PROCEDURE sp_MovieBooking
    @MovieID INT,
    @ScheduleID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Debugging: Log input parameters
    PRINT 'Input @MovieID: ' + CAST(@MovieID AS VARCHAR(10));
    PRINT 'Input @ScheduleID: ' + ISNULL(CAST(@ScheduleID AS VARCHAR(10)), 'NULL');

    -- Fetch Scheduled Movie Details for Booking
    SELECT 
        S.ScheduleID, 
        S.MovieID,
        M.MovieName,
        T.TheaterName, 
        T.Location, 
        S.Class, 
        CASE 
            WHEN S.Class = 'A' THEN T.ClassA_Rate
            WHEN S.Class = 'B' THEN T.ClassB_Rate
            WHEN S.Class = 'C' THEN T.ClassC_Rate
        END AS ClassRate,
        S.ShowTime, 
        S.Date,
        CASE 
            WHEN S.Class = 'A' THEN T.ClassA_SeatCount - ISNULL((
                SELECT SUM(TK.NumberOfSeats)
                FROM Tickets TK
                WHERE TK.ScheduleID = S.ScheduleID AND TK.SeatClass = 'A'
            ), 0)
            WHEN S.Class = 'B' THEN T.ClassB_SeatCount - ISNULL((
                SELECT SUM(TK.NumberOfSeats)
                FROM Tickets TK
                WHERE TK.ScheduleID = S.ScheduleID AND TK.SeatClass = 'B'
            ), 0)
            WHEN S.Class = 'C' THEN T.ClassC_SeatCount - ISNULL((
                SELECT SUM(TK.NumberOfSeats)
                FROM Tickets TK
                WHERE TK.ScheduleID = S.ScheduleID AND TK.SeatClass = 'C'
            ), 0)
        END AS RemainingSeats
    FROM 
        Schedule S
    INNER JOIN 
        Theater T ON S.TheaterID = T.TheaterID
    INNER JOIN 
        Movies M ON S.MovieID = M.MovieID
    WHERE 
        M.MovieID = @MovieID
        AND (@ScheduleID IS NULL OR S.ScheduleID = @ScheduleID)
        AND S.Date >= GETDATE()
    ORDER BY 
        S.Date, S.ShowTime;
END;


EXEC sp_MovieBooking @MovieID = 12, @ScheduleID = NULL;

SELECT * FROM Schedule

-- Get Payment Details procedure
ALTER PROCEDURE sp_GetPaymentDetails
    @ScheduleID INT,
    @NumberOfSeats INT
AS
BEGIN
    SET NOCOUNT ON;

	 -- Validate input parameters
    IF @ScheduleID <= 0 OR @NumberOfSeats <= 0
    BEGIN
        RAISERROR ('Invalid ScheduleID or NumberOfSeats.', 16, 1);
        RETURN;
    END

    SELECT 
        S.ScheduleID,
        M.MovieName,
        T.TheaterName,
        T.Location,
        S.Class AS SeatClass,
        CASE 
            WHEN S.Class = 'A' THEN T.ClassA_Rate
            WHEN S.Class = 'B' THEN T.ClassB_Rate
            WHEN S.Class = 'C' THEN T.ClassC_Rate
        END AS ClassRate,
        @NumberOfSeats AS NumberOfSeats,
        @NumberOfSeats * CASE 
            WHEN S.Class = 'A' THEN T.ClassA_Rate
            WHEN S.Class = 'B' THEN T.ClassB_Rate
            WHEN S.Class = 'C' THEN T.ClassC_Rate
        END + (@NumberOfSeats * 12) AS TotalPriceWithGST
    FROM 
        Schedule S
    INNER JOIN 
        Theater T ON S.TheaterID = T.TheaterID
    INNER JOIN 
        Movies M ON S.MovieID = M.MovieID
    WHERE 
        S.ScheduleID = @ScheduleID
		AND (S.Class IN ('A', 'B', 'C')); -- Ensure only valid seat classes are retrieved
	  -- Error handling if no schedule is found
    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR ('No schedule found for the provided ScheduleID.', 16, 1);
    END
END;

EXEC sp_GetPaymentDetails @ScheduleID = 78, @NumberOfSeats = 5

select * from Schedule



--Insert Ticket sp
ALTER PROCEDURE sp_InsertTicket
    @ScheduleID INT,
    @UserID INT,
    @SeatClass CHAR(1),
    @NumberOfSeats INT,
    @TotalPrice DECIMAL(10, 2),
    @TicketNumber NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	-- Validate foreign key relationships
        IF NOT EXISTS (SELECT 1 FROM Schedule WHERE ScheduleID = @ScheduleID)
            RAISERROR ('Invalid ScheduleID.', 16, 1);

		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID)
            RAISERROR ('Invalid UserID.', 16, 1);

		 -- Validate seat class
        IF @SeatClass NOT IN ('A', 'B', 'C')
            RAISERROR ('Invalid SeatClass. Must be A, B, or C.', 16, 1);

    -- Generate a random ticket number
    SET @TicketNumber = CONCAT('TKT-', CAST(ABS(CHECKSUM(NEWID())) AS NVARCHAR(50)));

    INSERT INTO Tickets (ScheduleID, UserID, SeatClass, NumberOfSeats, TotalPrice, BookingDate, TicketNumber)
    VALUES (@ScheduleID, @UserID, @SeatClass, @NumberOfSeats, @TotalPrice, GETDATE(), @TicketNumber);

    -- Return the ticket number
    SELECT @TicketNumber AS TicketNumber;
END;
GO

select * from Users
select * from Schedule
select * from Tickets



--User Tickets sp
ALTER PROCEDURE sp_GetUserTickets
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        T.TicketID,
        T.TicketNumber,
        T.SeatClass,
        T.NumberOfSeats,
        T.TotalPrice,
        T.BookingDate,
        M.MovieName,
        M.Genre,
        M.Language,
        M.Duration,
        Th.TheaterName,
        Th.Location,
        S.ShowTime,
        S.Date
    FROM Tickets T
    INNER JOIN Schedule S ON T.ScheduleID = S.ScheduleID
    INNER JOIN Movies M ON S.MovieID = M.MovieID
    INNER JOIN Theater Th ON S.TheaterID = Th.TheaterID
    WHERE T.UserID = @UserID
	ORDER BY T.BookingDate DESC;
END;
GO

exec sp_GetUserTickets 14

--Get All Booked Tickets for ADMIN sp
ALTER PROCEDURE sp_GetAllBookedTickets
AS
BEGIN
    SELECT 
        T.TicketID,
        U.UserID,
        CONCAT(U.FirstName, ' ', U.LastName) AS UserName,
        T.TicketNumber,
        T.SeatClass,
        CASE T.SeatClass
            WHEN 'A' THEN TH.ClassA_Rate
            WHEN 'B' THEN TH.ClassB_Rate
            WHEN 'C' THEN TH.ClassC_Rate
        END AS ClassRate,
        T.NumberOfSeats,
        T.TotalPrice,
        T.BookingDate,
        M.MovieName,
        M.Language,
        TH.TheaterName,
        TH.Location,
        S.ShowTime,
        S.Date
    FROM Tickets T
    INNER JOIN Users U ON T.UserID = U.UserID
    INNER JOIN Schedule S ON T.ScheduleID = S.ScheduleID
    INNER JOIN Movies M ON S.MovieID = M.MovieID
    INNER JOIN Theater TH ON S.TheaterID = TH.TheaterID
    ORDER BY T.BookingDate DESC;
END;
GO

EXEC sp_GetAllBookedTickets