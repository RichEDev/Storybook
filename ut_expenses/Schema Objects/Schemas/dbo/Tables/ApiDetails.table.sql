CREATE TABLE [ApiDetails] (
			ApiDetailsId int NOT NULL constraint PK_ApiDetails Primary Key IDENTITY(1,1),
			EmployeeId int NOT NULL constraint FK_ApiDetails_Employees Foreign Key REFERENCES employees, 
			CertificateInfo nvarchar(max) NOT NULL, 
			GenerationTime bigint NOT NULL, 
			ExpiryTime DateTime NOT NULL)