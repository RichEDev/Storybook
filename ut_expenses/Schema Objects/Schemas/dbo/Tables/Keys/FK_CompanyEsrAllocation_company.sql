ALTER TABLE [dbo].[CompanyEsrAllocation]
	ADD CONSTRAINT [FK_CompanyEsrAllocation_company]
	FOREIGN KEY (companyid)
	REFERENCES [companies] (companyid)
