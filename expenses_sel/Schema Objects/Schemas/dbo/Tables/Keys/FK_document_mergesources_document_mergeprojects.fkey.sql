ALTER TABLE [dbo].[document_mergesources]
    ADD CONSTRAINT [FK_document_mergesources_document_mergeprojects] FOREIGN KEY ([mergeprojectid]) REFERENCES [dbo].[document_mergeprojects] ([mergeprojectid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

