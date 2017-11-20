CREATE TABLE [dbo].[contract_producthistory] (
    [productHistoryId]  INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [dateChanged]       SMALLDATETIME  NULL,
    [employeeId]        INT            NULL,
    [contractProductId] INT            NULL,
    [productValue]      FLOAT (53)     NULL,
    [currentMaint]      FLOAT (53)     NULL,
    [nextYearMaint]     FLOAT (53)     NULL,
    [Comment]           NVARCHAR (255) NULL,
    CONSTRAINT [PK_Contract_Product_History] PRIMARY KEY CLUSTERED ([productHistoryId] ASC),
    CONSTRAINT [FK_contract_producthistory_contract_productdetails] FOREIGN KEY ([contractProductId]) REFERENCES [dbo].[contract_productdetails] ([contractProductId]) ON DELETE CASCADE,
    CONSTRAINT [FK_contract_producthistory_employees] FOREIGN KEY ([employeeId]) REFERENCES [dbo].[employees] ([employeeid])
);



