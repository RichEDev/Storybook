ALTER TABLE [dbo].[currencies]
    ADD CONSTRAINT [DF_currencies_archived] DEFAULT ((0)) FOR [archived];

