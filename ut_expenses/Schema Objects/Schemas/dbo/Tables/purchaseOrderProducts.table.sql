CREATE TABLE [dbo].[purchaseOrderProducts] (
    [purchaseOrderProductID] INT             IDENTITY (1, 1) NOT NULL,
    [productID]              INT             NOT NULL,
    [purchaseOrderID]        INT             NOT NULL,
    [productUnitOfMeasureID] INT             NOT NULL,
    [productUnitPrice]       MONEY           NOT NULL,
    [productQuantity]        DECIMAL (18, 2) NOT NULL
);

