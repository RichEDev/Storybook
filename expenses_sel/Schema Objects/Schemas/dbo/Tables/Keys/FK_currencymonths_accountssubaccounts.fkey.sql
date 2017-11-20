ALTER TABLE [dbo].[currencymonths]
    ADD CONSTRAINT [FK_currencymonths_accountssubaccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

