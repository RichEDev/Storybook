	ALTER TABLE [dbo].[favourites]  WITH CHECK ADD  CONSTRAINT [FK_favourites_addresses] FOREIGN KEY([AddressID])
	REFERENCES [dbo].[addresses] ([AddressID])
	ON UPDATE CASCADE
	ON DELETE CASCADE