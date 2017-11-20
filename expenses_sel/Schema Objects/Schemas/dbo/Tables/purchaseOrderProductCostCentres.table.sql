CREATE TABLE [dbo].[purchaseOrderProductCostCentres] (
    [purchaseOrderProductCostCentreID] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [purchaseOrderProductID]           INT NOT NULL,
    [departmentID]                     INT NULL,
    [costCodeID]                       INT NULL,
    [projectCodeID]                    INT NULL,
    [percentUsed]                      INT NOT NULL
);

