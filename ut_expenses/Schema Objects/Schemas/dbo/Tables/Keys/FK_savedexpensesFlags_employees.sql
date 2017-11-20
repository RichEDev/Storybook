ALTER TABLE [dbo].[savedexpensesFlags]  WITH CHECK ADD  CONSTRAINT [FK_savedexpensesFlags_employees] FOREIGN KEY([claimantJustificationDelegateID])
REFERENCES [dbo].[employees] ([employeeid])
GO

ALTER TABLE [dbo].[savedexpensesFlags] CHECK CONSTRAINT [FK_savedexpensesFlags_employees]
GO