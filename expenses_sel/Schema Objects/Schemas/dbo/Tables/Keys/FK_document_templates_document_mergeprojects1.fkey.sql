ALTER TABLE [dbo].[document_templates]
    ADD CONSTRAINT [FK_document_templates_document_mergeprojects1] FOREIGN KEY ([mergeprojectid]) REFERENCES [dbo].[document_mergeprojects] ([mergeprojectid]) ON DELETE CASCADE ON UPDATE NO ACTION;

