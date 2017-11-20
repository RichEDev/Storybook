ALTER TABLE [dbo].[report_folders]
    ADD CONSTRAINT [DF_report_folders_folderid] DEFAULT (newid()) FOR [folderid];

