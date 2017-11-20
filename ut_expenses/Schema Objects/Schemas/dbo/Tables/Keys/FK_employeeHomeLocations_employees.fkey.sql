ALTER TABLE [dbo].[employeeHomeLocations]
    ADD CONSTRAINT [FK_employeeHomeLocations_employees] FOREIGN KEY ([employeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

