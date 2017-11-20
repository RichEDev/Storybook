ALTER TABLE [dbo].[customEntityAttachmentFields]
    ADD CONSTRAINT [DF_custom_entity_attachment_fields_fieldid] DEFAULT (newid()) FOR [fieldid];

