ALTER TABLE [dbo].[card_transactions_barclaycard]
    ADD CONSTRAINT [FK_card_transactions_barclaycard_card_transactions] FOREIGN KEY ([transactionid]) REFERENCES [dbo].[card_transactions] ([transactionid]) ON DELETE CASCADE ON UPDATE NO ACTION;

