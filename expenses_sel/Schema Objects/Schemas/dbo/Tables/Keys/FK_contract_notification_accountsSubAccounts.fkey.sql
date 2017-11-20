ALTER TABLE [dbo].[contract_notification]
    ADD CONSTRAINT [FK_contract_notification_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

