CREATE TABLE [dbo].[DocumentSortingConfigurationColumns] (
    [SortingColumnId]  INT            IDENTITY (1, 1) NOT NULL,
    [GroupingId] INT            NOT NULL, 
    [MergeProjectId] INT NOT NULL,
	[SortingColumn]    NVARCHAR (255) NOT NULL,
	[DocumentSortTypeId] INT NOT NULL,
    [SequenceOrder]    INT            NOT NULL,
  
    CONSTRAINT [PK_DocumentSortingConfigurationColumns] PRIMARY KEY CLUSTERED ([SortingColumnId] ASC),
    CONSTRAINT [FK_DocumentSortingConfigurationColumns_DocumentGroupingConfigurations] FOREIGN KEY([GroupingId], [MergeProjectId]) REFERENCES [dbo].[DocumentGroupingConfigurations] ([GroupingId], [MergeProjectId]),
    CONSTRAINT [FK_DocumentSortingConfigurationColumns_DocumentSortType] FOREIGN KEY([DocumentSortTypeId]) REFERENCES [dbo].[DocumentSortType] ([DocumentSortTypeId])
  	);





