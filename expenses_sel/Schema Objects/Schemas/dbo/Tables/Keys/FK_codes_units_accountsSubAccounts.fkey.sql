ALTER TABLE [dbo].[codes_units]
    ADD CONSTRAINT [FK_codes_units_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

