ALTER TABLE [dbo].[codes_contracttype]
    ADD CONSTRAINT [FK_codes_contracttype_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

