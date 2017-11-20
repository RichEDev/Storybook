ALTER TABLE [dbo].[card_providers]
    ADD CONSTRAINT [DF_card_providers_creditcard] DEFAULT ((0)) FOR [creditcard];

