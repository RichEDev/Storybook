CREATE TABLE [dbo].[DocumentGroupingConfigurationColumns] (
    [GroupingColumnId] INT            IDENTITY (1, 1) NOT NULL,
    [GroupingId]       INT            NOT NULL,
    [MergeProjectId]   INT            NOT NULL,
    [GroupingColumn]   NVARCHAR (255) NOT NULL,
    [SequenceOrder]    INT            CONSTRAINT [DF_DocumentGroupingConfigurationColumns_SequenceOrder] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_DocumentGroupingConfigurationColumns] PRIMARY KEY CLUSTERED ([GroupingColumnId] ASC),
    CONSTRAINT [FK_DocumentGroupingConfigurationColumns_DocumentGroupingConfigurations] FOREIGN KEY ([GroupingId], [MergeProjectId]) REFERENCES [dbo].[DocumentGroupingConfigurations] ([GroupingId], [MergeProjectId]) ON DELETE CASCADE
);





