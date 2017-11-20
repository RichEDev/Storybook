CREATE TABLE [dbo].[customEntityAttributeSummaryColumns] (
    [columnid]          INT              IDENTITY (1, 1) NOT NULL,
    [attributeid]       INT              NOT NULL,
    [alternate_header]  NVARCHAR (150)   NULL,
    [width]             INT              NULL,
    [order]             TINYINT          NULL,
    [filterVal]         NVARCHAR (100)   NULL,
    [default_sort]      BIT              CONSTRAINT [DF_customEntityAttributeSummaryColumns_default_sort] DEFAULT ((0)) NOT NULL,
    [displayFieldId]    UNIQUEIDENTIFIER NULL,
    [joinViaID]         INT              NULL,
    [columnAttributeId] INT              NOT NULL,
    CONSTRAINT [PK_customEntityAttributeSummaryColumns_columnid] PRIMARY KEY CLUSTERED ([columnid] ASC),
    CONSTRAINT [FK_customEntityAttributeSummaryColumns_attributeid] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[customEntityAttributes] ([attributeid]),
    CONSTRAINT [FK_customEntityAttributeSummaryColumns_joinVia] FOREIGN KEY ([joinViaID]) REFERENCES [dbo].[joinVia] ([joinViaID])
);



