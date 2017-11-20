CREATE TABLE [dbo].[purchaseOrderRecurringScheduleMonths] (
    [purchaseOrderId]               INT     NOT NULL,
    [month]                         TINYINT NOT NULL,
    [purchaseOrderScheduleMonthsID] INT     IDENTITY (1, 1) NOT NULL
);

