CREATE TABLE [dbo].[purchaseOrderAudiences] (
    [id]              INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [purchaseOrderID] INT NOT NULL,
    [audienceID]      INT NOT NULL
);

