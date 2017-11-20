ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [DF_custom_entities_enableCurrencies] DEFAULT ((0)) FOR [enableCurrencies];

