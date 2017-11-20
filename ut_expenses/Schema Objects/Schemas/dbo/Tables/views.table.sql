CREATE TABLE [dbo].[views] (
    [viewid]     INT              NOT NULL,
    [employeeid] INT              NOT NULL,
    [order]      INT              NOT NULL,
    [viewsid]    INT              IDENTITY (1, 1) NOT NULL,
    [createdon]  DATETIME         NOT NULL,
    [createdby]  INT              NULL,
    [fieldID]    UNIQUEIDENTIFIER NOT NULL
);

