ALTER TABLE [dbo].[codes_contracttype]
    ADD CONSTRAINT [DF_codes_contracttype_archived] DEFAULT ((0)) FOR [archived];

