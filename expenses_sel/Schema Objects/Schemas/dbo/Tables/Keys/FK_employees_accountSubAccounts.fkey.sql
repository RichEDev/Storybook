ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [FK_employees_accountSubAccounts] FOREIGN KEY ([defaultSubAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

