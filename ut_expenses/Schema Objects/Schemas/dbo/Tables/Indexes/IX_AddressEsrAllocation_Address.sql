CREATE UNIQUE NONCLUSTERED INDEX [IX_AddressEsrAllocation_address]
	ON [dbo].[AddressEsrAllocation]
	(AddressID, ESRLocationID, ESRAddressID)
