ALTER TABLE [dbo].[customEntityAttachmentFields]
    ADD CONSTRAINT [FK_custom_entity_attachment_fields_custom_entities] FOREIGN KEY ([tableid]) REFERENCES [dbo].[custom_entities] ([attachmentTableID]) ON DELETE CASCADE ON UPDATE NO ACTION;

