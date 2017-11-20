ALTER TABLE [dbo].[accessRoles]
	ADD CONSTRAINT [DF_access_roles_allowMobileAccess]
	DEFAULT ((0))
	FOR [allowMobileAccess]
