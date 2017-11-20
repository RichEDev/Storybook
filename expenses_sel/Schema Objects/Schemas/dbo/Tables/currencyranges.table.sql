CREATE TABLE [dbo].[currencyranges] (
    [currencyrangeid] INT      IDENTITY (1, 1) NOT NULL,
    [currencyid]      INT      NOT NULL,
    [startdate]       DATETIME NOT NULL,
    [enddate]         DATETIME NOT NULL,
    [exchangerate]    FLOAT    NULL,
    [createdon]       DATETIME NULL,
    [createdby]       INT      NULL,
    [modifiedon]      DATETIME NULL,
    [modifiedby]      INT      NULL,
    [subAccountId]    INT      NULL
);

