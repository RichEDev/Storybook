ALTER TABLE [dbo].[customEntityAttachmentFields]
    ADD CONSTRAINT [FK_customEntityAttachmentFields_customEntities] FOREIGN KEY ([tableid]) REFERENCES [dbo].[customEntities] ([attachmentTableID]) ON DELETE CASCADE ON UPDATE NO ACTION;

