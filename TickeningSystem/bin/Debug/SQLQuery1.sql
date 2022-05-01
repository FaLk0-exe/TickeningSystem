CREATE TABLE [User](
Id int primary key identity(1,1) not null,
[Login] varchar(50) not null,
[Password] varchar(50) not null,
[Role] varchar(5) not null
);
CREATE TABLE Movie(
Id int primary key identity(1,1) not null,
[Name] nvarchar(max) COLLATE Cyrillic_General_CI_AS null,
[Producer] nvarchar(100) COLLATE Cyrillic_General_CI_AS null,
[Description] nvarchar(max) COLLATE Cyrillic_General_CI_AS null,
Duration int not null
);
Create table Room(
Id int primary key identity(1,1) not null,
[Name] nvarchar(30) not null,
SeatsCount int not null
);

Create table Screening(
Id int primary key identity(1,1) not null,
RoomId int not null,
MovieId int not null,
ScreeningStart timestamp not null,
FOREIGN KEY (RoomId) REFERENCES Room(Id),
FOREIGN KEY (MovieId) REFERENCES Movie(Id)
);

Create table Seat(
Id int primary key identity(1,1) not null,
[Row] int not null,
[Column] int not null,
RoomId int not null,
FOREIGN KEY (RoomId) REFERENCES Room(Id)
);

CREATE TABLE [Status](
Id int primary key identity(1,1) not null,
[Name] NVARCHAR(50) COLLATE Cyrillic_General_CI_AS null
);

CREATE TABLE SeatStatus(
Id int primary key identity(1,1) not null,
StatusId int not null,
SeatId int not null,
ScreeningId int not null,
Price decimal not null
);