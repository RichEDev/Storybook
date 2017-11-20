CREATE TABLE [dbo].[filter_rules] (
    [filterid]          INT      IDENTITY (1, 1)  NOT NULL,
    [parent]            TINYINT  NOT NULL,
    [child]             TINYINT  NOT NULL,
    [createdon]         DATETIME NULL,
    [createdby]         INT      NULL,
    [paruserdefineid]   INT      NULL,
    [childuserdefineid] INT      NULL,
    [enabled]           BIT      NOT NULL
);

