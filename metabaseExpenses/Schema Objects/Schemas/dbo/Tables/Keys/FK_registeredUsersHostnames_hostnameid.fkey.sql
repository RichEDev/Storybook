ALTER TABLE [dbo].[registeredUsersHostnames]
	ADD CONSTRAINT [FK_registeredUsersHostnames_hostnameid]
	FOREIGN KEY (hostnameID)
	REFERENCES [hostnames] (hostnameID)
