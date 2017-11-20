CREATE TABLE [dbo].[employeeHomeAddresses](
	[EmployeeHomeAddressId] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[AddressId] [int] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedOn] [datetime] NULL,
	[ModifiedBy] [int] NULL,
CONSTRAINT [PK_employeeHomeAddresses] PRIMARY KEY CLUSTERED 
(
	[EmployeeHomeAddressId] ASC
)
)
GO

ALTER TABLE [dbo].[employeeHomeAddresses]  WITH CHECK ADD  CONSTRAINT [FK_employeeHomeAddresses_addresses] FOREIGN KEY([AddressId])
REFERENCES [dbo].[addresses] ([AddressID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[employeeHomeAddresses] CHECK CONSTRAINT [FK_employeeHomeAddresses_addresses]
GO

ALTER TABLE [dbo].[employeeHomeAddresses]  WITH CHECK ADD  CONSTRAINT [FK_employeeHomeAddresses_employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[employees] ([employeeid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[employeeHomeAddresses] CHECK CONSTRAINT [FK_employeeHomeAddresses_employees]
GO

