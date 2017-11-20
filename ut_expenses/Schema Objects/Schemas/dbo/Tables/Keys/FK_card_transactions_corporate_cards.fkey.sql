ALTER TABLE [dbo].[card_transactions]
    ADD CONSTRAINT [FK_card_transactions_corporate_cards] FOREIGN KEY ([corporatecardid]) REFERENCES [dbo].[employee_corporate_cards] ([corporatecardid]) ON DELETE SET NULL ON UPDATE NO ACTION;

