CREATE TABLE [dbo].[card_transactions_premierinn] (
    [transactionid]          INT            NOT NULL,
    [Invoice_date]           DATETIME       NULL,
    [Invoice_Number]         NVARCHAR (50)  NULL,
    [Guest_name]             NVARCHAR (50)  NULL,
    [Location]               NVARCHAR (50)  NULL,
    [Card_name]              NVARCHAR (250) NULL,
    [Purchase_order_number]  NVARCHAR (50)  NULL,
    [Customer_own_reference] NVARCHAR (50)  NULL,
    [Product]                NVARCHAR (50)  NULL,
    [Quantity]               INT            NULL,
    [Net]                    MONEY          NULL,
    [vat]                    MONEY          NULL
);

