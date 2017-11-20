ALTER TABLE [dbo].[reports_export_options]
    ADD CONSTRAINT [FK_reports_export_options_reports] FOREIGN KEY ([reportID]) REFERENCES [dbo].[reports] ([reportID]) ON DELETE CASCADE ON UPDATE NO ACTION;

