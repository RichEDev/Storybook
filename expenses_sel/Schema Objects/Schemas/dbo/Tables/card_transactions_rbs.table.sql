CREATE TABLE [dbo].[card_transactions_rbs] (
    [transactionid]       INT           NOT NULL,
    [cycle_date]          DATETIME      NULL,
    [trans_postdate]      DATETIME      NULL,
    [trans_type]          NVARCHAR (50) NULL,
    [bank_ref]            NVARCHAR (23) NULL,
    [mcc]                 NVARCHAR (4)  NULL,
    [exception_indicator] NVARCHAR (9)  NULL,
    [merchant_town]       NVARCHAR (13) NULL,
    [mcg]                 NVARCHAR (50) NULL
);

