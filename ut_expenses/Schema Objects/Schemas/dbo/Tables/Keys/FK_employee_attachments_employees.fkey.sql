ALTER TABLE [dbo].[employee_attachments]
    ADD CONSTRAINT [FK_employee_attachments_employees] FOREIGN KEY ([id]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

