ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_genlist] DEFAULT ((0)) FOR [genlist];

