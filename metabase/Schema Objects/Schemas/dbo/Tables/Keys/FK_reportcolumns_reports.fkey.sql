ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [FK_reportcolumns_reports] FOREIGN KEY ([reportID]) REFERENCES [dbo].[reports] ([reportid]) ON DELETE CASCADE ON UPDATE NO ACTION;

