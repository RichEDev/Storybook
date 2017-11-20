ALTER TABLE [dbo].[reportcriteria]
    ADD CONSTRAINT [FK_reportcriteria_reports] FOREIGN KEY ([reportid]) REFERENCES [dbo].[reports] ([reportid]) ON DELETE CASCADE ON UPDATE NO ACTION;

