ALTER TABLE [dbo].[report_folders]
    ADD CONSTRAINT [DF_report_folders_folderID] DEFAULT (newid()) FOR [folderID];

