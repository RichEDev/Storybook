	ALTER TABLE [dbo].[addressLabels] ADD CONSTRAINT [FK_addressLabels_addresses] FOREIGN KEY([AddressID])
	REFERENCES [dbo].[addresses] ([AddressID])
	ON UPDATE CASCADE
	ON DELETE CASCADE