ALTER TABLE [dbo].[currencies]
    ADD CONSTRAINT [DF_currencies_positiveFormat] DEFAULT ((0)) FOR [positiveFormat];

