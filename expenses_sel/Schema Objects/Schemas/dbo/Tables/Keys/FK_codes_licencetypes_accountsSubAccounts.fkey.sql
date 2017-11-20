ALTER TABLE [dbo].[codes_licencetypes]
    ADD CONSTRAINT [FK_codes_licencetypes_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

