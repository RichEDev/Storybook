ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_IsForeignKey] DEFAULT ((0)) FOR [IsForeignKey];