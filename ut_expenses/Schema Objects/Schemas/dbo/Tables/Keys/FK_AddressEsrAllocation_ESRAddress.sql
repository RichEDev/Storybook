ALTER TABLE [dbo].[AddressEsrAllocation]
	ADD CONSTRAINT [FK_AddressEsrAllocation_ESRAddress]
	FOREIGN KEY (ESRAddressID)
	REFERENCES [ESRAddresses] (ESRAddressID)
