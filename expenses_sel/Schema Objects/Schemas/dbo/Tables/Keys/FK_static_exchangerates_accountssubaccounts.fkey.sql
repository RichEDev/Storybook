ALTER TABLE [dbo].[static_exchangerates]
    ADD CONSTRAINT [FK_static_exchangerates_accountssubaccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

