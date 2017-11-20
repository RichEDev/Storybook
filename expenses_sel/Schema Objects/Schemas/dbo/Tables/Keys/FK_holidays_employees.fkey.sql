ALTER TABLE [dbo].[holidays]
    ADD CONSTRAINT [FK_holidays_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

