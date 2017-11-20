CREATE TABLE [dbo].[AddressEsrAllocation]
(
	[AddressEsrAllocationId] int IDENTITY (1, 1) NOT NULL,
	[AddressId] INT NOT NULL, 
    [ESRLocationID] BIGINT NULL, 
    [ESRAddressID] BIGINT NULL
)
