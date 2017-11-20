CREATE TABLE [dbo].[template_columns] (
    [templateid]   INT              NOT NULL,
    [columntype]   TINYINT          NOT NULL,
    [groupby]      BIT              NOT NULL,
    [sort]         TINYINT          NOT NULL,
    [order]        INT              NOT NULL,
    [funcsum]      BIT              NOT NULL,
    [funcmax]      BIT              NOT NULL,
    [funcmin]      BIT              NOT NULL,
    [funcavg]      BIT              NOT NULL,
    [funccount]    BIT              NOT NULL,
    [literalname]  NVARCHAR (50)    NULL,
    [literalvalue] NVARCHAR (1000)  NULL,
    [runtime]      BIT              NOT NULL,
    [hidden]       BIT              NOT NULL,
    [fieldID]      UNIQUEIDENTIFIER NULL
);

