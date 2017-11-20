CREATE TABLE [dbo].[DocumentSourceSortingColumns] (
    [DocumentSourceSortingColumnId] INT            IDENTITY (1, 1) NOT NULL,
    [DocumentSourceSortingId]       INT            NOT NULL,
    [ColumnName]                    NVARCHAR (255) NOT NULL,
    [SequenceOrder]                 INT            CONSTRAINT [DF_DocumentSourceSortingColumns_SequenceOrder] DEFAULT ((1)) NOT NULL,
    [DocumentSortTypeId]            INT            CONSTRAINT [DF_DocumentSourceSortingColumns_DocumentSortTypeId] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_DocumentSourceSortingColumns] PRIMARY KEY CLUSTERED ([DocumentSourceSortingColumnId] ASC),
    CONSTRAINT [FK_DocumentSourceSortingColumns_DocumentSourceSorting] FOREIGN KEY ([DocumentSourceSortingId]) REFERENCES [dbo].[DocumentSourceSorting] ([DocumentSourceSortingId]) ON DELETE CASCADE,
	CONSTRAINT [FK_DocumentSourceSortingColumns_DocumentSortType] FOREIGN KEY([DocumentSortTypeId]) REFERENCES [dbo].[DocumentSortType] ([DocumentSortTypeId])
);



