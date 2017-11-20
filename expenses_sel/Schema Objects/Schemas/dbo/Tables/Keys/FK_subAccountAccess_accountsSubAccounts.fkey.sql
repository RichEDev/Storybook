ALTER TABLE [dbo].[subAccountAccess]
    ADD CONSTRAINT [FK_subAccountAccess_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

