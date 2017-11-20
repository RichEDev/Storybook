ALTER TABLE [dbo].[financial_status]
    ADD CONSTRAINT [FK_financial_status_accountsSubAccounts] FOREIGN KEY ([subaccountid]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

