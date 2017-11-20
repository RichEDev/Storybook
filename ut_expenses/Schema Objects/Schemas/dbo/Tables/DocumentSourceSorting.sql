CREATE TABLE [dbo].[DocumentSourceSorting] (
    [DocumentSourceSortingId] INT            IDENTITY (1, 1) NOT NULL,
    [MergeProjectId]          INT            NOT NULL,
    [GroupingId]              INT            NOT NULL,
    [SourceName]              NVARCHAR (255) NOT NULL,
    [DocumentSortTypeId] INT NOT NULL, 
    [SequenceOrder] INT NOT NULL, 
    CONSTRAINT [PK_DocumentReportSourceSorting] PRIMARY KEY CLUSTERED ([DocumentSourceSortingId] ASC),
   );



