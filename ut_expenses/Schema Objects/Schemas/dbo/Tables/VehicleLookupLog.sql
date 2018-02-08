CREATE TABLE [dbo].[VehicleLookupLog]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EmployeeId] INT NOT NULL, 
    [DelegateId] INT NULL, 
    [Registration] NVARCHAR(100) NOT NULL, 
    [Code] NVARCHAR(100) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [DateTimeStamp] DATETIME NOT NULL
)
