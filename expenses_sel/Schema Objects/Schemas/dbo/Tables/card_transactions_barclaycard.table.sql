CREATE TABLE [dbo].[card_transactions_barclaycard] (
    [transactionid]    INT           NOT NULL,
    [company_number]   NVARCHAR (8)  NULL,
    [cardholder_name]  NVARCHAR (20) NULL,
    [spend_category]   NVARCHAR (25) NULL,
    [transaction_type] NVARCHAR (1)  NULL,
    [transaction_code] NVARCHAR (3)  NULL,
    [visa_mcc]         NVARCHAR (7)  NULL
);

