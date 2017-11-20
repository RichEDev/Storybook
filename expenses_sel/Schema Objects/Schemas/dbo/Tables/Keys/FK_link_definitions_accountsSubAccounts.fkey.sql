ALTER TABLE [dbo].[link_definitions]
    ADD CONSTRAINT [FK_link_definitions_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

