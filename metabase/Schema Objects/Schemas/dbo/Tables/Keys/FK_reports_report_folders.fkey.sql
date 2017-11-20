ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [FK_reports_report_folders] FOREIGN KEY ([folderid]) REFERENCES [dbo].[report_folders] ([folderid]) ON DELETE SET NULL ON UPDATE NO ACTION;

