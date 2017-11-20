ALTER TABLE [dbo].[card_providers]
    ADD CONSTRAINT [DF_card_providers_purchasecard] DEFAULT ((0)) FOR [purchasecard];

