ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [DF_custom_entities_enableAttachments] DEFAULT ((0)) FOR [enableAttachments];

