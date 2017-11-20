ALTER TABLE [dbo].[codes_contractcategory]
    ADD CONSTRAINT [FK_codes_contractcategory_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

