/* Admin Data in database */

insert into Users(username,password,firstname,lastname,gender,email,address,pincode,state,city,phoneno) 
values 
('admin','admin@123','Prathamesh','Dhande','Male','admin123@gmail.com','Boisar',202332,'Maharashtra','Boisar','9970205298')
go

/* Insert Data in Roles */
insert into Roles(rolename) values 
('Admin'),
('User'),
('Librarian')
go

/* Giving the access to the role of admin */
insert into RoleMapping(roleid,userid) values (1,1)
go

/* Dummy Users into the database */
-- Insert record 1
INSERT INTO Users (username, [password], firstname, lastname, gender, email, [address], pincode, [state], city, phoneno)
VALUES ('john_doe', 'password123', 'John', 'Doe', 'Male', 'john.doe@example.com', '1234 Elm Street', 123456, 'California', 'Los Angeles', '1234567890');
go

-- Insert record 2
INSERT INTO Users (username, [password], firstname, lastname, gender, email, [address], pincode, [state], city, phoneno)
VALUES ('jane_smith', 'password456', 'Jane', 'Smith', 'Female', 'jane.smith@example.com', '5678 Oak Avenue', 789012, 'New York', 'New York City', '0987654321');
go

-- Insert record 3
INSERT INTO Users (username, [password], firstname, lastname, gender, email, [address], pincode, [state], city, phoneno)
VALUES ('mark_jones', 'password789', 'Mark', 'Jones', 'Male', 'mark.jones@example.com', '9101 Pine Road', 345678, 'Texas', 'Houston', '1122334455');
go

-- inserting the dummy records as user in database
insert into RoleMapping(roleid,userid) values 
(2,2),(2,3),(2,4)
go

-- using Stored Procedure of spRegisteruser
declare @issuccess int
exec spRegisterUser 'usersdfd','user1','user','user1','Male','user1@gmail.com','dd','33','Mah','Vashi','1234567890',@issuccess out
select @issuccess
go

-- inserting the library setting
insert into Settings(maxissueDays,dailyfineamount,maxbookperuser) values
(10,50,5)
go

-- inserting the category records
INSERT INTO Category (CategoryName)
VALUES 
('Fiction'),
('Non-Fiction'),
('Science'),
('Biography'),
('Fantasy'),
('History'),
('Technology'),
('Art'),
('Health'),
('Philosophy');
go

-- single insert query for adding one book in the table
declare @coverimage nvarchar(max) ='~/Content/BookImages/361d6860-e5dd-44f2-a2c1-7ebb179a4134.jpg'
declare @issuccess bit
exec spCreateUpdateBook
    @title = 'The Art of Programming',
    @isbn = '978-0-306-40615-7',
    @categoryid = 2,
    @authorname = 'Donald Knuth',
    @coverimage = @coverimage,
    @publicationyear = 2020,
    @originalQty = 10,
    @availableQty = 5,
    @createdby = 1,
    @toinsert = 1,
	@updatedby = 1,
	@bookid=1,
    @issuccess=@issuccess output
select @issuccess 'Success'
go



-- Book 1
declare @issuccess int
EXEC spCreateUpdateBook 
    @title = 'Book Title 1',
    @isbn = '978-3-16-148410-0',
    @categoryid = 1,
    @authorname = 'Author One',
    @coverimage = '~\content\BookImages\361d6860-e5dd-44f2-a2c1-7ebb179a4134.jpg',
    @publicationyear = 2021,
    @originalQty = 10,
    @availableQty = 10,
    @updatedby = 1,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

-- Check success
SELECT @issuccess AS IsSuccess;

-- Book 2
EXEC spCreateUpdateBook 
    @title = 'Book Title 2',
    @isbn = '978-1-23-456789-7',
    @categoryid = 2,
    @authorname = 'Author Two',
    @coverimage = '~\content\BookImages\667d51d8-ca0e-4359-bd9d-82efb7cf693a.png',
    @publicationyear = 2020,
    @originalQty = 5,
    @availableQty = 5,
    @updatedby = 1,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

SELECT @issuccess AS IsSuccess;


-- Book 3
EXEC spCreateUpdateBook 
    @title = 'Book Title 3',
    @isbn = '978-3-16-148410-1',
    @categoryid = 3,
    @authorname = 'Author Three',
    @coverimage = '~\content\BookImages\book2.jpg',
    @publicationyear = 2019,
    @originalQty = 8,
    @availableQty = 8,
    @updatedby = 1,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

SELECT @issuccess AS IsSuccess;


-- Book 4
EXEC spCreateUpdateBook 
    @title = 'Book Title 4',
    @isbn = '978-3-16-148410-2',
    @categoryid = 4,
    @authorname = 'Author Four',
    @coverimage = '~\content\BookImages\d9af3167-8c30-4d59-ba73-4fd8824c1336.png',
    @publicationyear = 2018,
    @originalQty = 7,
    @availableQty = 7,
    @updatedby = 1,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

SELECT @issuccess AS IsSuccess;


-- Book 5
EXEC spCreateUpdateBook 
    @title = 'Book Title 5',
    @isbn = '978-3-16-148410-3',
    @categoryid = 5,
    @authorname = 'Author Five',
    @coverimage = '~\content\BookImages\e7c028a1-6e85-404e-aa07-11f011d9d560.png',
    @publicationyear = 2022,
    @originalQty = 12,
    @availableQty = 12,
    @updatedby = 1,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

SELECT @issuccess AS IsSuccess;


-- Book 6
EXEC spCreateUpdateBook 
    @title = 'Book Title 6',
    @isbn = '978-3-16-148410-4',
    @categoryid = 6,
    @authorname = 'Author Six',
    @coverimage = '~\content\BookImages\0c77063f-620d-4c1b-a065-9858bb21cd36.png',
    @publicationyear = 2023,
    @originalQty = 15,
    @availableQty = 15,
    @updatedby = 1,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

SELECT @issuccess AS IsSuccess;
DECLARE @issuccess BIT;
EXEC spCreateUpdateBook 
    @title = 'The Lost Chronicles',
    @isbn = '978-3-16-148410-0',
    @categoryid = 1,
    @authorname = 'Isabella Drake',
    @coverimage = '~/Content/BookImages/05cf86b9-2072-4f21-a0fd-1164c907e829.png',
    @publicationyear = 2021,
    @originalQty = 10,
    @availableQty = 10,
    @updatedby = 1,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

EXEC spCreateUpdateBook 
    @title = 'Echoes of Eternity',
    @isbn = '978-1-40-289462-6',
    @categoryid = 2,
    @authorname = 'Liam Montgomery',
    @coverimage = '~/Content/BookImages/29cb8056-57a0-4c8e-ab14-6e7b050031c5.png',
    @publicationyear = 2020,
    @originalQty = 15,
    @availableQty = 14,
    @updatedby = 2,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

EXEC spCreateUpdateBook 
    @title = 'Shadows of the Moon',
    @isbn = '978-0-14-312234-3',
    @categoryid = 2,
    @authorname = 'Aurora West',
    @coverimage = '~/Content/BookImages/33d6ad47-61c9-466a-ad7f-746470e9da93.png',
    @publicationyear = 2019,
    @originalQty = 8,
    @availableQty = 7,
    @updatedby = 3,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

EXEC spCreateUpdateBook 
    @title = 'The Final Horizon',
    @isbn = '978-0-452-28784-2',
    @categoryid = 1,
    @authorname = 'Ethan Carter',
    @coverimage = '~/Content/BookImages/258aa8b8-d9d7-498d-98a2-9f821dab5ab1.jpg',
    @publicationyear = 2022,
    @originalQty = 20,
    @availableQty = 18,
    @updatedby = 2,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

EXEC spCreateUpdateBook 
    @title = 'Whispers in the Wind',
    @isbn = '978-0-399-17062-6',
    @categoryid = 3,
    @authorname = 'Samantha Greene',
    @coverimage = '~/Content/BookImages/361d6860-e5dd-44f2-a2c1-7ebb179a4134.jpg',
    @publicationyear = 2018,
    @originalQty = 12,
    @availableQty = 9,
    @updatedby = 3,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;

EXEC spCreateUpdateBook 
    @title = 'The Celestial Gateway',
    @isbn = '978-1-250-30824-9',
    @categoryid = 2,
    @authorname = 'Oliver Banks',
    @coverimage = '~/Content/BookImages/667d51d8-ca0e-4359-bd9d-82efb7cf693a.png',
    @publicationyear = 2023,
    @originalQty = 25,
    @availableQty = 23,
    @updatedby = 1,
    @createdby = 1,
    @toinsert = 1,
    @issuccess = @issuccess OUTPUT;


INSERT INTO Issues (UserId, BookId, DueDate, Status, DailyFineAmount)
VALUES (1, 1, DATEADD(day, 14, GETDATE()), 'Issued', 5);

INSERT INTO Issues (UserId, BookId, IssueDate, DueDate, Status, DailyFineAmount)
VALUES (9, 17, '2024-05-04', '2024-05-14', 'Issued', 6);

INSERT INTO Issues (UserId, BookId, IssueDate, DueDate, Status, DailyFineAmount)
VALUES (4, 27, '2024-06-12', '2024-06-20', 'Issued', 10);
