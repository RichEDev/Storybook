ALTER TABLE [dbo].[codes_inflatormetrics]
    ADD CONSTRAINT [FK_codes_inflatormetrics_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

