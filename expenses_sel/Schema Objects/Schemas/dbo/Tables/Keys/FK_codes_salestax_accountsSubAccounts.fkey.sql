ALTER TABLE [dbo].[codes_salestax]
    ADD CONSTRAINT [FK_codes_salestax_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

