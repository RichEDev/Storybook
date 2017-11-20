ALTER TABLE [dbo].[email_schedule]
    ADD CONSTRAINT [FK_email_schedule_accountSubAccounts] FOREIGN KEY ([runSubAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

