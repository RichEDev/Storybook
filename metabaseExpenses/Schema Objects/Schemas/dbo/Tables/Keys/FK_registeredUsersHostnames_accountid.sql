ALTER TABLE [dbo].[registeredUsersHostnames]
	ADD CONSTRAINT [FK_registeredUsersHostnames_accountid]
	FOREIGN KEY (accountid)
	REFERENCES [registeredusers] (accountid)
