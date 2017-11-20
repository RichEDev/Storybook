CREATE UNIQUE NONCLUSTERED INDEX [IX_CompanyEsrAllocation_company]
	ON [dbo].[CompanyEsrAllocation]
	(companyid, ESRLocationID, ESRAddressID)
