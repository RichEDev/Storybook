CREATE TABLE [dbo].[card_transactions_amex] (
    [transactionid]      INT           NOT NULL,
    [record_type]        NVARCHAR (1)  NULL,
    [surname]            NVARCHAR (30) NULL,
    [initials]           NVARCHAR (8)  NULL,
    [employee_id]        NVARCHAR (15) NULL,
    [corp_id]            NVARCHAR (10) NULL,
    [control_number]     NVARCHAR (8)  NULL,
    [billing_date]       DATETIME      NULL,
    [charge_description] NVARCHAR (70) NULL,
    [charge_code]        NVARCHAR (5)  NULL,
    [charge_type]        NVARCHAR (50) NULL,
    [sequence_number]    NVARCHAR (15) NULL
);

