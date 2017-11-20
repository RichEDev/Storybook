	ALTER TABLE [dbo].[organisations]  WITH CHECK ADD  CONSTRAINT [FK_organisations_addresses] FOREIGN KEY([PrimaryAddressID])
	REFERENCES [dbo].[addresses] ([AddressID])