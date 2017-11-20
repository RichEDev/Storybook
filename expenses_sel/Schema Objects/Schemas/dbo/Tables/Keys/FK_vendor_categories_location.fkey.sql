ALTER TABLE [dbo].[supplier_categories]
    ADD CONSTRAINT [FK_vendor_categories_location] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

