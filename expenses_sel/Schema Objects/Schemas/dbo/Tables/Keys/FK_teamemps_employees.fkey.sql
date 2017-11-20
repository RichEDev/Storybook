ALTER TABLE [dbo].[teamemps]
    ADD CONSTRAINT [FK_teamemps_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

