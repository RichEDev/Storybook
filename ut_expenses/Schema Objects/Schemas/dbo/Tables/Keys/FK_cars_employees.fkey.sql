ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [FK_cars_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

