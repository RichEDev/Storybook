ALTER TABLE [dbo].[monthly_exchangerates]
    ADD CONSTRAINT [FK_monthly_exchangerates_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

