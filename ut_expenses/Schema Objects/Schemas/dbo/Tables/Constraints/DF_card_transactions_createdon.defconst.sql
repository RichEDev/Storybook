ALTER TABLE [dbo].[card_transactions]
    ADD CONSTRAINT [DF_card_transactions_createdon] DEFAULT (getdate()) FOR [createdon];

