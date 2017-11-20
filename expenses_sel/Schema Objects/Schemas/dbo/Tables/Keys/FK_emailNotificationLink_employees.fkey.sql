ALTER TABLE [dbo].[emailNotificationLink]
    ADD CONSTRAINT [FK_emailNotificationLink_employees] FOREIGN KEY ([employeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

