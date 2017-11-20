ALTER TABLE [dbo].[reports_export_options]
    ADD CONSTRAINT [FK_reports_export_options_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

