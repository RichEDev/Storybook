ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [FK_reports_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

