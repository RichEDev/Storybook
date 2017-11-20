ALTER TABLE [dbo].[reportsLog]
	ADD CONSTRAINT [FK_reportsLog_employees]
	FOREIGN KEY (employeeId)
	REFERENCES [employees] (employeeid)
	ON DELETE SET NULL
	ON UPDATE NO ACTION
