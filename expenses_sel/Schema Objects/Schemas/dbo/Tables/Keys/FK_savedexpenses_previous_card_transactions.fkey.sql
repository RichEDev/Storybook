ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_card_transactions] FOREIGN KEY ([transactionid]) REFERENCES [dbo].[card_transactions] ([transactionid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

