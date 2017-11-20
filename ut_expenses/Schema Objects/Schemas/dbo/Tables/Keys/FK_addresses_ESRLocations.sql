	ALTER TABLE [dbo].[addresses]  WITH CHECK ADD  CONSTRAINT [FK_addresses_ESRLocations] FOREIGN KEY([ESRLocationID])
	REFERENCES [dbo].[ESRLocations] ([ESRLocationID])