ALTER TABLE [dbo].[audienceEmployees]
    ADD CONSTRAINT [FK_audienceEmployees_employees] FOREIGN KEY ([employeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

