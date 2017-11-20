ALTER TABLE [dbo].[employee_proxies]
    ADD CONSTRAINT [FK_employee_proxies_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

