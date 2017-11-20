CREATE TABLE [dbo].[contract_productdetails] (
    [contractProductId]  INT        IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [contractId]         INT        NULL,
    [productId]          INT        NULL,
    [productValue]       FLOAT (53) NULL,
    [currencyId]         INT        NULL,
    [salesTaxRate]       INT        NULL,
    [maintenanceValue]   FLOAT (53) NULL,
    [maintenancePercent] FLOAT (53) NULL,
    [unitCost]           FLOAT (53) NULL,
    [Quantity]           INT        NULL,
    [unitId]             INT        NULL,
    [projectedSaving]    FLOAT (53) NULL,
    [pricePaid]          FLOAT (53) CONSTRAINT [DF_Contract_ProductDetails_PricePaid] DEFAULT ((0)) NOT NULL,
    [archiveStatus]      INT        CONSTRAINT [DF_Contract_ProductDetails_ArchivedStatus] DEFAULT ((0)) NULL,
    [archiveDate]        DATETIME   NULL,
    [createdDate]        DATETIME   NULL,
    [lastModified]       DATETIME   NULL,
    CONSTRAINT [PK_Contract_ProductDetails] PRIMARY KEY CLUSTERED ([contractProductId] ASC),
    CONSTRAINT [FK_contract_productdetails_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]),
    CONSTRAINT [FK_contract_productdetails_product_details] FOREIGN KEY ([productId]) REFERENCES [dbo].[productDetails] ([productId])
);



