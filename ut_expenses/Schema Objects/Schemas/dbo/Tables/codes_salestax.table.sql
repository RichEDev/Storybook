CREATE TABLE [dbo].[codes_salestax] (
    [salesTaxId]   INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [subAccountId] INT             NULL,
    [description]  NVARCHAR (50)   NULL,
    [salesTax]     DECIMAL (18, 2) NULL,
    [archived]     BIT             CONSTRAINT [DF_codes_salestax_archived] DEFAULT ((0)) NOT NULL,
    [createdOn]    DATETIME        NULL,
    [createdBy]    INT             NULL,
    [modifiedOn]   DATETIME        NULL,
    [modifiedBy]   INT             NULL,
    CONSTRAINT [PK_Codes_SalesTax] PRIMARY KEY CLUSTERED ([salesTaxId] ASC),
    CONSTRAINT [FK_codes_salestax_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]) ON DELETE CASCADE
);



