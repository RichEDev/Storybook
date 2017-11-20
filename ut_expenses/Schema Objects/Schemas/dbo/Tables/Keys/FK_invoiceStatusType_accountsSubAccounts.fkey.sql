ALTER TABLE [dbo].[invoiceStatusType]
    ADD CONSTRAINT [FK_invoiceStatusType_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

