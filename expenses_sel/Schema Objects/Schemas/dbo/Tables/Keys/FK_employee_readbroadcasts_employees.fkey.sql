ALTER TABLE [dbo].[employee_readbroadcasts]
    ADD CONSTRAINT [FK_employee_readbroadcasts_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

