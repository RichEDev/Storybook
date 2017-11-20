ALTER TABLE [dbo].[CompanyEsrAllocation]
	ADD CONSTRAINT [FK_CompanyEsrAllocation_ESRAddress]
	FOREIGN KEY (ESRAddressID)
	REFERENCES [ESRAddresses] (ESRAddressID)
