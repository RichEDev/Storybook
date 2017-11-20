ALTER TABLE [dbo].[range_exchangerates]
    ADD CONSTRAINT [FK_range_exchangerates_accountssubaccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

