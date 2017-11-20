ALTER TABLE [dbo].[reportcriteria]
    ADD CONSTRAINT [FK_reportcriteria_reportcriteria] FOREIGN KEY ([reportID]) REFERENCES [dbo].[reports] ([reportID]) ON DELETE CASCADE ON UPDATE NO ACTION;

