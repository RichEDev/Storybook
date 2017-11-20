ALTER TABLE [dbo].[savings]
    ADD CONSTRAINT [FK_savings_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

