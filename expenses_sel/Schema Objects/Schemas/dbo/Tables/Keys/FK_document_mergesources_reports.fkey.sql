ALTER TABLE [dbo].[document_mergesources]
    ADD CONSTRAINT [FK_document_mergesources_reports] FOREIGN KEY ([reportid]) REFERENCES [dbo].[reports] ([reportID]) ON DELETE CASCADE ON UPDATE NO ACTION;

