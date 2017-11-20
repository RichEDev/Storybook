ALTER TABLE [dbo].[CompanyEsrAllocation]
	ADD CONSTRAINT [FK_CompanyEsrAllocation_ESRLocation]
	FOREIGN KEY (ESRLocationID)
	REFERENCES [ESRLocations] (ESRLocationID)
