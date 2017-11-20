ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_useforlookup] DEFAULT ((0)) FOR [useforlookup];

