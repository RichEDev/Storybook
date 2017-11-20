ALTER TABLE [dbo].[EmployeeSelectedClaimApprover]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeSelectedClaimApprover_employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[employees] ([employeeid])
ON DELETE CASCADE