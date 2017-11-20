ALTER TABLE [dbo].[AccessRoles]
	ADD CONSTRAINT [DF_access_roles_allowApiAccess]
	DEFAULT ((0))
	FOR [allowApiAccess]
