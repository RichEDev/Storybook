ALTER TABLE [dbo].[AddressEsrAllocation]
	ADD CONSTRAINT [FK_AddressEsrAllocation_Address]
	FOREIGN KEY (AddressId)
	REFERENCES [Addresses] (AddressId)
