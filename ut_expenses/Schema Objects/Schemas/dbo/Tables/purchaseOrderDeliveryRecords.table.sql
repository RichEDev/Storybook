CREATE TABLE [dbo].[purchaseOrderDeliveryRecords] (
    [purchaseOrderDeliveryRecordID] INT             IDENTITY (1, 1)  NOT NULL,
    [purchaseOrderProductID]        INT             NOT NULL,
    [deliveryID]                    INT             NOT NULL,
    [deliveredQuantity]             DECIMAL (18, 2) NOT NULL,
    [returnedQuantity]              DECIMAL (18, 2) NULL
);

