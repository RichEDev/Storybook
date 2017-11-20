ALTER TABLE [dbo].[codes_rechargeentity]
    ADD CONSTRAINT [FK_codes_rechargeentity_accountsSubAccount] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE ON UPDATE NO ACTION;

