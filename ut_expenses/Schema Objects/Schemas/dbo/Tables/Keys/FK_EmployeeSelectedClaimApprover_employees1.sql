ALTER TABLE [dbo].[EmployeeSelectedClaimApprover]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeSelectedClaimApprover_employees1] FOREIGN KEY([ApproverId])
REFERENCES [dbo].[employees] ([employeeid])
