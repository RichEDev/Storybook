ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [DF_fields_useforlookup] DEFAULT ((0)) FOR [useforlookup];

