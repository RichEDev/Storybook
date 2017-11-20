ALTER TABLE [dbo].[report_folders]
    ADD CONSTRAINT [DF_report_folders_personal] DEFAULT (1) FOR [personal];

