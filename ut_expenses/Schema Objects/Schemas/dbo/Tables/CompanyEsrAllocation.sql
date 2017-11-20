CREATE TABLE [dbo].[CompanyEsrAllocation]
(
	[CompanyEsrAllocationID] int IDENTITY (1, 1) NOT NULL,
	[companyid] INT NOT NULL, 
    [ESRLocationID] BIGINT NULL, 
    [ESRAddressID] BIGINT NULL
)
