CREATE TABLE [dbo].[invoiceProductDetails] (
    [invoiceProductID]     INT   IDENTITY (1, 1) NOT NULL,
    [contractID]           INT   NULL,
    [invoiceID]            INT   NULL,
    [productID]            INT   NULL,
    [productInvoiceAmount] FLOAT NULL,
    [codeType]             INT   NULL,
    [codeNumber]           INT   NULL
);

