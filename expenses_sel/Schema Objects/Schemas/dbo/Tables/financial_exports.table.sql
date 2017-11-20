﻿CREATE TABLE [dbo].[financial_exports] (
    [financialexportid] INT              IDENTITY (1, 1) NOT NULL,
    [applicationtype]   TINYINT          NOT NULL,
    [automated]         BIT              NOT NULL,
    [CreatedBy]         INT              NOT NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    [ModifiedOn]        DATETIME         NULL,
    [ModifiedBy]        INT              NULL,
    [curexportnum]      INT              NOT NULL,
    [lastexportdate]    DATETIME         NULL,
    [exporttype]        TINYINT          NULL,
    [reportID]          UNIQUEIDENTIFIER NOT NULL,
    [oldreportid]       INT              NULL,
    [NHSTrustID]        INT              NULL
);

