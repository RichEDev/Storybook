ALTER TABLE [dbo].[codes_termtype]
    ADD CONSTRAINT [FK_codes_termtype_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

