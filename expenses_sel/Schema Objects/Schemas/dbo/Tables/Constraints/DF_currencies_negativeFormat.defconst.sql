ALTER TABLE [dbo].[currencies]
    ADD CONSTRAINT [DF_currencies_negativeFormat] DEFAULT ((0)) FOR [negativeFormat];

