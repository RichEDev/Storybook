ALTER TABLE [dbo].[document_merge_association]
    ADD CONSTRAINT [FK_document_merge_association_customEntities] FOREIGN KEY ([entityid]) REFERENCES [dbo].[customEntities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

