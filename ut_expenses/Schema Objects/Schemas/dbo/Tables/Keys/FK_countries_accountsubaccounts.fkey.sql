ALTER TABLE [dbo].[countries]
    ADD CONSTRAINT [FK_countries_accountsubaccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

