ALTER TABLE [dbo].[employee_proxies]
    ADD CONSTRAINT [FK_employee_proxies_employees1] FOREIGN KEY ([proxyid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

