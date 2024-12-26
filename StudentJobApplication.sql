CREATE DATABASE StudentJobPortel
GO
USE StudentJobPortel
GO


CREATE TABLE Students (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(15),
	DOB Date,
    Photo NVARCHAR(MAX),
    Resume NVARCHAR(MAX)
);



SELECT * FROM Students;

INSERT INTO Students (Name, Email, Phone, Photo, Resume, DOB)
VALUES
('John Doe', 'john.doe@example.com', '1234567890', 'photo1.jpg', 'resume1.pdf', '1995-05-15'),
('Jane Smith', 'jane.smith@example.com', '9876543210', 'photo2.jpg', 'resume2.pdf', '1998-08-22'),
('Michael Brown', 'michael.brown@example.com', '5678901234', 'photo3.jpg', 'resume3.pdf', '1992-12-03'),
('Emily Davis', 'emily.davis@example.com', '3456789012', 'photo4.jpg', 'resume4.pdf', '2000-03-17');


SELECT * FROM dbo.Students



