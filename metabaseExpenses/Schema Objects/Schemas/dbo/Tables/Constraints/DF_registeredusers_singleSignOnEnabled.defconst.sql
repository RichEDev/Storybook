ALTER TABLE [dbo].[registeredusers]
	ADD CONSTRAINT [DF_registeredusers_singleSignOnEnabled]
	DEFAULT ((0))
	FOR [SingleSignOnEnabled]
