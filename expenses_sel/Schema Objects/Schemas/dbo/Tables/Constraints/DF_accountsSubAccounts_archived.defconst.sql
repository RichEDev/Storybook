ALTER TABLE [dbo].[accountsSubAccounts]
    ADD CONSTRAINT [DF_accountsSubAccounts_archived] DEFAULT ((0)) FOR [archived];

