ALTER TABLE [dbo].[auditLog]
    ADD CONSTRAINT [FK_auditlog_accountssubaccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

