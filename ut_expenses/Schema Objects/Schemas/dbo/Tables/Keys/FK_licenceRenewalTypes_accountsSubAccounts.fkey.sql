ALTER TABLE [dbo].[licenceRenewalTypes]
    ADD CONSTRAINT [FK_licenceRenewalTypes_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

