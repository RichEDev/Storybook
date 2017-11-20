ALTER TABLE [dbo].[currencies]
    ADD CONSTRAINT [FK_currencies_accountssubaccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

