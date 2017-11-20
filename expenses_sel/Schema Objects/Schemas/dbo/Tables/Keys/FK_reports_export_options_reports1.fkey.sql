ALTER TABLE [dbo].[reports_export_options]
    ADD CONSTRAINT [FK_reports_export_options_reports1] FOREIGN KEY ([footerid]) REFERENCES [dbo].[reports] ([reportID]) ON DELETE NO ACTION ON UPDATE SET NULL;

