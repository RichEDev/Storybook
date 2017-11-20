CREATE TABLE [dbo].[EmployeeWorkAddresses] (
 [EmployeeWorkAddressId] [int] IDENTITY(1,1) NOT NULL,
 [EmployeeId] [int] NOT NULL,
 [AddressId] [int] NOT NULL,
 [StartDate] [datetime] NULL,
 [EndDate] [datetime] NULL,
 [Active] [bit] NOT NULL,
 [Temporary] [bit] NOT NULL,
 [CreatedOn] [datetime] NULL,
 [CreatedBy] [int] NULL,
 [ModifiedOn] [datetime] NULL,
 [ModifiedBy] [int] NULL,
 [ESRAssignmentLocationId] [int] NULL
 
 CONSTRAINT [PK_employeeWorkAddresses] PRIMARY KEY CLUSTERED 
 (
	 [EmployeeWorkAddressId] ASC
 ), 
    [Rotational] BIT NULL, 
    [PrimaryRotational] BIT NULL
 )

GO

ALTER TABLE [dbo].[employeeWorkAddresses]  WITH CHECK ADD  CONSTRAINT [FK_employeeWorkAddresses_addresses] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Addresses] ([AddressId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[employeeWorkAddresses] CHECK CONSTRAINT [FK_employeeWorkAddresses_addresses]
GO

ALTER TABLE [dbo].[employeeWorkAddresses]  WITH CHECK ADD  CONSTRAINT [FK_employeeWorkAddresses_employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[employees] ([employeeid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[employeeWorkAddresses] CHECK CONSTRAINT [FK_employeeWorkAddresses_employees]
GO