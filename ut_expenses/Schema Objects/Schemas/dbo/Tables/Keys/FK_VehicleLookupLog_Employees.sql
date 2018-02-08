ALTER TABLE [dbo].[VehicleLookupLog]
    ADD CONSTRAINT [FK_VehicleLookupLog_employees] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;