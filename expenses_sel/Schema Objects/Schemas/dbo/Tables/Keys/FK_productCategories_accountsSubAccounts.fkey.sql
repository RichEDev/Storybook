ALTER TABLE [dbo].[productCategories]
    ADD CONSTRAINT [FK_productCategories_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

