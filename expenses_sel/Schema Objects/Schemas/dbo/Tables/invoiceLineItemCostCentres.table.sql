CREATE TABLE [dbo].[invoiceLineItemCostCentres] (
    [invoiceLineItemCostCentreId] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [invoiceLineItemId]           INT NOT NULL,
    [departmentId]                INT NULL,
    [costCodeId]                  INT NULL,
    [projectCodeId]               INT NULL,
    [percentUsed]                 INT NOT NULL
);

