ALTER TABLE [dbo].[scheduled_reports]
    ADD CONSTRAINT [FK_scheduled_reports_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

