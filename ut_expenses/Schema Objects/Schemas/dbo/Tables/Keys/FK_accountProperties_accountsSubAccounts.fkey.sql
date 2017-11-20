ALTER TABLE [dbo].[accountProperties]
    ADD CONSTRAINT [FK_accountProperties_accountsSubAccounts] FOREIGN KEY ([subAccountID]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

