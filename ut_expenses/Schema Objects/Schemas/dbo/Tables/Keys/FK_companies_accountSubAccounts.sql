ALTER TABLE [dbo].[companies]
	ADD CONSTRAINT [FK_companies_accountSubAccounts]
	FOREIGN KEY ([subAccountID])
	REFERENCES [dbo].[accountsSubAccounts] ([subAccountID])
