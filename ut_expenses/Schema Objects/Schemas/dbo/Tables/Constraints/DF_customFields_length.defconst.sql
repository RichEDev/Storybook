ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_length] DEFAULT ((0)) FOR [length];

