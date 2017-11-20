ALTER TABLE [dbo].[productDetails]
    ADD CONSTRAINT [FK_productDetails_subAccountId] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

