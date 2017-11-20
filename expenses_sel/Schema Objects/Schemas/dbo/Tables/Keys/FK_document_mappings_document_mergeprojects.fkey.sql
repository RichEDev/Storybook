ALTER TABLE [dbo].[document_mappings]
    ADD CONSTRAINT [FK_document_mappings_document_mergeprojects] FOREIGN KEY ([mergeprojectid]) REFERENCES [dbo].[document_mergeprojects] ([mergeprojectid]) ON DELETE CASCADE ON UPDATE NO ACTION;

