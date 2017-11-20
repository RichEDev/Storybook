CREATE TABLE [ApiMethodLog] (
   ApiLogId int NOT NULL constraint PK_ApiLog Primary Key IDENTITY(1,1),
   EmployeeId int constraint FK_ApiMethodLog_Employees Foreign Key REFERENCES employees NOT NULL, 
   UserName nvarchar(200) NOT NULL, 
   Uri nvarchar(MAX) NOT NULL, 
   Result nvarchar(MAX) NOT NULL)
