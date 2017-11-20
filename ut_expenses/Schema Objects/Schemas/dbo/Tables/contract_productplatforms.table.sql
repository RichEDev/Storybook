CREATE TABLE [dbo].[contract_productplatforms] (
    [productPlatformId] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [contractId]        INT NULL,
    [contractProductId] INT NULL,
    [platformId]        INT NULL,
    [platformLparId]    INT NULL,
    [subAccountId]      INT NULL,
    [subLocationId]     INT NULL,
    [supplierId]        INT NULL,
    [calloffQuantity]   INT NULL,
    CONSTRAINT [PK_Contract_ProductPlatforms] PRIMARY KEY CLUSTERED ([productPlatformId] ASC)
);



