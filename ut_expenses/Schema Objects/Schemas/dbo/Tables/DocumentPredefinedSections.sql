CREATE TABLE [dbo].[DocumentPredefinedSections] (
    [PredefinedSectionId] INT            IDENTITY (1, 1) NOT NULL,
    [MergeProjectId]      INT            NOT NULL,
    [GroupingId]          INT            NOT NULL,
    [SectionName]         NVARCHAR (250) NOT NULL,
    [SequenceOrder]       INT            CONSTRAINT [DF_DocumentPredefinedSections_SequenceOrder] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_DocumentPredefinedSections_1] PRIMARY KEY CLUSTERED ([PredefinedSectionId] ASC),
    CONSTRAINT [FK_DocumentPredefinedSections_DocumentGroupingConfigurations] FOREIGN KEY ([GroupingId], [MergeProjectId]) REFERENCES [dbo].[DocumentGroupingConfigurations] ([GroupingId], [MergeProjectId]) ON DELETE CASCADE
);



