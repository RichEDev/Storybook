ALTER TABLE [dbo].[registeredUsers]
	ADD CONSTRAINT [FK_registeredUsers_dbserver]
	FOREIGN KEY (dbserver)
	REFERENCES [databases] (databaseID)
