CREATE TABLE [dbo].[currencymonths] (
    [currencymonthid] INT      IDENTITY (1, 1) NOT NULL,
    [currencyid]      INT      NOT NULL,
    [exchangerate]    FLOAT    NULL,
    [month]           TINYINT  NOT NULL,
    [year]            SMALLINT NOT NULL,
    [createdon]       DATETIME NULL,
    [createdby]       INT      NULL,
    [modifiedon]      DATETIME NULL,
    [modifiedby]      INT      NULL,
    [subAccountId]    INT      NULL
);

