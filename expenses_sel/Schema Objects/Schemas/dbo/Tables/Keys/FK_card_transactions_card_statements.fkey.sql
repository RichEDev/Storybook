ALTER TABLE [dbo].[card_transactions]
    ADD CONSTRAINT [FK_card_transactions_card_statements] FOREIGN KEY ([statementid]) REFERENCES [dbo].[card_statements_base] ([statementid]) ON DELETE CASCADE ON UPDATE NO ACTION;

