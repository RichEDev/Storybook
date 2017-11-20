CREATE TABLE [dbo].[contract_productdetails] (
    [contractProductId]  INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [contractId]         INT      NULL,
    [productId]          INT      NULL,
    [productValue]       FLOAT    NULL,
    [currencyId]         INT      NULL,
    [salesTaxRate]       INT      NULL,
    [maintenanceValue]   FLOAT    NULL,
    [maintenancePercent] FLOAT    NULL,
    [unitCost]           FLOAT    NULL,
    [Quantity]           INT      NULL,
    [unitId]             INT      NULL,
    [projectedSaving]    FLOAT    NULL,
    [pricePaid]          FLOAT    NOT NULL,
    [archiveStatus]      INT      NULL,
    [archiveDate]        DATETIME NULL,
    [createdDate]        DATETIME NULL,
    [lastModified]       DATETIME NULL
);

