ALTER TABLE [dbo].[document_merge_association]
    ADD CONSTRAINT [FK_document_merge_association_document_templates] FOREIGN KEY ([documentid]) REFERENCES [dbo].[document_templates] ([documentid]) ON DELETE CASCADE ON UPDATE NO ACTION;

