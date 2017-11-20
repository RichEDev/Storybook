CREATE TABLE [dbo].[purchaseOrderDeliveries] (
    [deliveryID]        INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [locationID]        INT           NOT NULL,
    [purchaseOrderID]   INT           NOT NULL,
    [deliveryDate]      DATETIME      NOT NULL,
    [deliveryReference] NVARCHAR (50) NULL,
    [createdOn]         DATETIME      NOT NULL,
    [createdBy]         INT           NOT NULL,
    [modifiedOn]        DATETIME      NULL,
    [modifiedBy]        INT           NULL
);

