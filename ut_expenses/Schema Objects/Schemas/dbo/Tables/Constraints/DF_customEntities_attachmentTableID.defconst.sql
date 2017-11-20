ALTER TABLE [dbo].[customEntities]
    ADD CONSTRAINT [DF_customEntities_attachmentTableID] DEFAULT (newid()) FOR [attachmentTableID];

