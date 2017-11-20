ALTER TABLE [dbo].[document_merge_association]
    ADD CONSTRAINT [FK_document_merge_association_custom_entities] FOREIGN KEY ([entityid]) REFERENCES [dbo].[custom_entities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

