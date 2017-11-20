CREATE TABLE [dbo].[card_transactions] (
    [transactionid]      INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [statementid]        INT             NOT NULL,
    [reference]          NVARCHAR (50)   NULL,
    [card_number]        NVARCHAR (20)   NOT NULL,
    [corporatecardid]    INT             NULL,
    [transaction_date]   DATETIME        NULL,
    [description]        NVARCHAR (500)  NULL,
    [transaction_amount] MONEY           NOT NULL,
    [original_amount]    MONEY           NULL,
    [currency_code]      NVARCHAR (250)  NOT NULL,
    [globalcurrencyid]   INT             NOT NULL,
    [exchangerate]       DECIMAL (18, 5) NULL,
    [country_code]       NVARCHAR (250)  NULL,
    [globalcountryid]    INT             NULL,
    [createdon]          DATETIME        NOT NULL,
    [createdby]          INT             NULL,
    [modifiedon]         DATETIME        NULL,
    [modifiedby]         INT             NULL
);

