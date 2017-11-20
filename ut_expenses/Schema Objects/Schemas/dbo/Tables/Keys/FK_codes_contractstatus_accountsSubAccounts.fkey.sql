ALTER TABLE [dbo].[codes_contractstatus]
    ADD CONSTRAINT [FK_codes_contractstatus_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

