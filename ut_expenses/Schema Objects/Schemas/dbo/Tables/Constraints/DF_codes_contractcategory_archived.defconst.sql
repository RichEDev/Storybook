ALTER TABLE [dbo].[codes_contractcategory]
    ADD CONSTRAINT [DF_codes_contractcategory_archived] DEFAULT ((0)) FOR [archived];

