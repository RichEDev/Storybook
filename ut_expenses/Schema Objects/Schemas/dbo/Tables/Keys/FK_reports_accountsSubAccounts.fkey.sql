ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [FK_reports_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

