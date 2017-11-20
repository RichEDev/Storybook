ALTER TABLE [dbo].[codes_invoicefrequencytype]
    ADD CONSTRAINT [FK_codes_invoicefrequencytype_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

