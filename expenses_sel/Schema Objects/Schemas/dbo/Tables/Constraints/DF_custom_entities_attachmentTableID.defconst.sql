ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [DF_custom_entities_attachmentTableID] DEFAULT (newid()) FOR [attachmentTableID];

