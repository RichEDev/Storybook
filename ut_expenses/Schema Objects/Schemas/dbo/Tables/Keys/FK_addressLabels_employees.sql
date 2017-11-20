	ALTER TABLE [dbo].[addressLabels]  WITH CHECK ADD  CONSTRAINT [FK_addressLabels_employees] FOREIGN KEY([EmployeeID])
	REFERENCES [dbo].[employees] ([employeeid])
	ON UPDATE CASCADE
	ON DELETE CASCADE