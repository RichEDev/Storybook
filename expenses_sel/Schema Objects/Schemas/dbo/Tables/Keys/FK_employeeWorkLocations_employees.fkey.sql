ALTER TABLE [dbo].[employeeWorkLocations]
    ADD CONSTRAINT [FK_employeeWorkLocations_employees] FOREIGN KEY ([employeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

