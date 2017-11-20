CREATE TABLE [dbo].[codes_salestax] (
    [salesTaxId]   INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [subAccountId] INT             NULL,
    [description]  NVARCHAR (50)   NULL,
    [salesTax]     DECIMAL (18, 2) NULL,
    [archived]     BIT             NOT NULL,
    [createdOn]    DATETIME        NULL,
    [createdBy]    INT             NULL,
    [modifiedOn]   DATETIME        NULL,
    [modifiedBy]   INT             NULL
);

