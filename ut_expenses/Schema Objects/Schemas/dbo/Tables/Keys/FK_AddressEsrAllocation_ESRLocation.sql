ALTER TABLE [dbo].[AddressEsrAllocation]
	ADD CONSTRAINT [FK_AddressEsrAllocation_ESRLocation]
	FOREIGN KEY (ESRLocationID)
	REFERENCES [ESRLocations] (ESRLocationID)
