ALTER TABLE [dbo].[EmployeeContextualHelp] 
ADD CONSTRAINT [FK_EmployeeContextualHelp_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[employees] ([employeeid])
ON DELETE CASCADE
ON UPDATE NO ACTION
