CREATE TABLE [dbo].[invoiceLineItems] (
    [invoiceLineItemID] INT             IDENTITY (1, 1) NOT NULL,
    [invoiceID]         INT             NOT NULL,
    [productID]         INT             NOT NULL,
    [unitOfMeasureID]   INT             NOT NULL,
    [salesTaxID]        INT             NOT NULL,
    [unitPrice]         MONEY           NOT NULL,
    [quantity]          DECIMAL (18, 2) NOT NULL
);

