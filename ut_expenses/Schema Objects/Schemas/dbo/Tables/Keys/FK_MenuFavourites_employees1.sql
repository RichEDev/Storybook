ALTER TABLE [dbo].[employeeMenuFavourites]
	ADD CONSTRAINT [FK_MenuFavourites_employees1]
	FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[employees] ([employeeid])