CREATE TABLE [dbo].[static_exchangerates] (
    [currencyid]   INT      NOT NULL,
    [tocurrencyid] INT      NOT NULL,
    [exchangerate] FLOAT    NOT NULL,
    [createdon]    DATETIME NULL,
    [createdby]    INT      NULL,
    [subAccountId] INT      NULL
);

