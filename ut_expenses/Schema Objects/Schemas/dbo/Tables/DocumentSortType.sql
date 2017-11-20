CREATE TABLE [dbo].[DocumentSortType] (
    [DocumentSortTypeId] INT           NOT NULL,
    [DocumentSortType]   NVARCHAR (50) CONSTRAINT [DF_DocumentSortType_DocumentSortType] DEFAULT (N'Asc') NOT NULL,
    CONSTRAINT [PK_DocumentSortType] PRIMARY KEY CLUSTERED ([DocumentSortTypeId] ASC)
);

