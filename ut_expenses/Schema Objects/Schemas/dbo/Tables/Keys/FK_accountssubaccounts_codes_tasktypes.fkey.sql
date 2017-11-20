ALTER TABLE [dbo].[codes_tasktypes]
    ADD CONSTRAINT [FK_accountssubaccounts_codes_tasktypes] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

