ALTER TABLE [dbo].[teams]
    ADD CONSTRAINT [FK_teams_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

