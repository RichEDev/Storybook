ALTER TABLE [dbo].[column_templates]
    ADD CONSTRAINT [FK_column_templates_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

