ALTER TABLE [dbo].[reports_export_options]
    ADD CONSTRAINT [DF_reports_export_options_csvheader] DEFAULT (0) FOR [csvheader];

