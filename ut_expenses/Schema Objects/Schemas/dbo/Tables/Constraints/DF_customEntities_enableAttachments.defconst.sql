ALTER TABLE [dbo].[customEntities]
    ADD CONSTRAINT [DF_customEntities_enableAttachments] DEFAULT ((0)) FOR [enableAttachments];

