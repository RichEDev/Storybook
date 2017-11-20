ALTER TABLE [dbo].[currencyranges]
    ADD CONSTRAINT [FK_currencyranges_accountssubaccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

