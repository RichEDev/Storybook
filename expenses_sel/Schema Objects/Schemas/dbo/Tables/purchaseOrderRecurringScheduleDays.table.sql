CREATE TABLE [dbo].[purchaseOrderRecurringScheduleDays] (
    [purchaseOrderId]             INT     NOT NULL,
    [day]                         TINYINT NOT NULL,
    [purchaseOrderScheduleDaysID] INT     IDENTITY (1, 1) NOT NULL
);

