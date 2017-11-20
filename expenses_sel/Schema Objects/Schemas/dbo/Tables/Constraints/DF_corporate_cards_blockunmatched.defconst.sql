ALTER TABLE [dbo].[corporate_cards]
    ADD CONSTRAINT [DF_corporate_cards_blockunmatched] DEFAULT ((0)) FOR [blockunmatched];

