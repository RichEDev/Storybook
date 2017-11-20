ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_normalview] DEFAULT ((1)) FOR [normalview];

