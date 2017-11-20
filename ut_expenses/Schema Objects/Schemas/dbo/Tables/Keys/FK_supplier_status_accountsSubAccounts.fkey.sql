ALTER TABLE [dbo].[supplier_status]
    ADD CONSTRAINT [FK_supplier_status_accountsSubAccounts] FOREIGN KEY ([subaccountid]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

