ALTER TABLE [dbo].[savedExpensesFlagsApproverJustifications]  WITH CHECK ADD  CONSTRAINT [FK_savedExpensesFlagsApproverJustifications_employees] FOREIGN KEY([delegateID])
REFERENCES [dbo].[employees] ([employeeid])
GO

ALTER TABLE [dbo].[savedExpensesFlagsApproverJustifications] CHECK CONSTRAINT [FK_savedExpensesFlagsApproverJustifications_employees]
GO