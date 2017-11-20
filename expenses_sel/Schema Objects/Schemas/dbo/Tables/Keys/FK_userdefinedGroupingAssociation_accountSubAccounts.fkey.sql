ALTER TABLE [dbo].[userdefinedGroupingAssociation]
    ADD CONSTRAINT [FK_userdefinedGroupingAssociation_accountSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

