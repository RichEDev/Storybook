ALTER TABLE [dbo].[accessManagement]
    ADD CONSTRAINT [FK_accessmanagement_employees] FOREIGN KEY ([employeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

