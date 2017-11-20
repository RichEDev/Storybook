ALTER TABLE [dbo].[employees]  WITH CHECK ADD  CONSTRAINT [FK_employees_AuthoriserLevelDetails] FOREIGN KEY([AuthoriserLevelDetailId])
REFERENCES [dbo].[AuthoriserLevelDetails] ([AuthoriserLevelDetailId])
GO

ALTER TABLE [dbo].[employees] CHECK CONSTRAINT [FK_employees_AuthoriserLevelDetails]