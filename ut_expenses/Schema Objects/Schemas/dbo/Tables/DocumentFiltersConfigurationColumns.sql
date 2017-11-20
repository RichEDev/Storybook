CREATE TABLE [dbo].[DocumentFiltersConfigurationColumns] (
    [FilterColumnId]  INT            IDENTITY (1, 1) NOT NULL,
    [GroupingId] INT            NOT NULL,
	[MergeProjectId] INT        NOT NULL,
    [FilterColumn]    NVARCHAR (255) NOT NULL,
    [SequenceOrder]    INT            NOT NULL,
    [Condition]        TINYINT        NOT NULL,
    [ValueOne]         NVARCHAR (150) NOT NULL,
    [ValueTwo]         NVARCHAR (150) CONSTRAINT [DF_Table_1_valueTwo] DEFAULT ('') NOT NULL,
    [TypeText] NVARCHAR(150) NOT NULL, 
    [FieldType] NVARCHAR(2) NOT NULL, 
    CONSTRAINT [PK_DocumentFiltersConfigurationColumns] PRIMARY KEY CLUSTERED ([FilterColumnId] ASC),
    CONSTRAINT [FK_DocumentFiltersConfigurationColumns_DocumentGroupingConfigurations] FOREIGN KEY([GroupingId], [MergeProjectId]) REFERENCES [dbo].[DocumentGroupingConfigurations] ([GroupingId], [MergeProjectId]) ON DELETE CASCADE,
   
    );





