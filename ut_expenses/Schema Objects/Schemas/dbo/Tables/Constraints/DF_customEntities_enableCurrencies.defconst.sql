ALTER TABLE [dbo].[customEntities]
    ADD CONSTRAINT [DF_customEntities_enableCurrencies] DEFAULT ((0)) FOR [enableCurrencies];

