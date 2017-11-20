	ALTER TABLE [dbo].[favourites]  WITH CHECK ADD  CONSTRAINT [FK_favourites_employees] FOREIGN KEY([EmployeeID])
	REFERENCES [dbo].[employees] ([employeeid])
	ON UPDATE CASCADE
	ON DELETE CASCADE