ALTER TABLE [dbo].[corporate_cards]
    ADD CONSTRAINT [DF_corporate_cards_singleclaim] DEFAULT ((0)) FOR [singleclaim];

