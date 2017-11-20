ALTER TABLE [dbo].[mobileDevices]
    ADD CONSTRAINT [FK_mobileDevices_employees] FOREIGN KEY ([employeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

