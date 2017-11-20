ALTER TABLE [dbo].[tasks]
    ADD CONSTRAINT [FK_accountssubaccount_tasks] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

