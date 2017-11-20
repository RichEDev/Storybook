ALTER TABLE [dbo].[supplier_details]
    ADD CONSTRAINT [FK_supplier_details_accountsSubAccounts] FOREIGN KEY ([subaccountid]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

