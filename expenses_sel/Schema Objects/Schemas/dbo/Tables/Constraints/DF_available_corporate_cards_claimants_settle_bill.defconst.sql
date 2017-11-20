ALTER TABLE [dbo].[corporate_cards]
    ADD CONSTRAINT [DF_available_corporate_cards_claimants_settle_bill] DEFAULT ((0)) FOR [claimants_settle_bill];

