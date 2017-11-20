ALTER TABLE [dbo].[corporate_cards]
    ADD CONSTRAINT [DF_corporate_cards_blockcash] DEFAULT ((0)) FOR [blockcash];

