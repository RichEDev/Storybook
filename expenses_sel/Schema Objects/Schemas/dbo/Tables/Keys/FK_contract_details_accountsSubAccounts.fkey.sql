ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [FK_contract_details_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

