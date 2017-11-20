ALTER TABLE [dbo].[AccessRoles]
	ADD CONSTRAINT [DF_access_roles_allowWebsiteAccess]
	DEFAULT ((0))
	FOR [allowWebsiteAccess]
