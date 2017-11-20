CREATE TABLE [dbo].[DocumentGroupingConfigurations] (
    [GroupingId]     INT            IDENTITY (1, 1) NOT NULL,
    [MergeProjectId] INT            NOT NULL,
    [Label]          NVARCHAR (255) NOT NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedOn]      DATETIME       CONSTRAINT [DF_DocumentGroupingConfigurations_CreatedOn] DEFAULT (getdate()) NULL,
    [ModifiedBy]     INT            NULL,
    [ModifiedOn]     DATETIME       NULL,
    [Archived]       BIT            CONSTRAINT [DF_DocumentGroupingConfigurations_Archived] DEFAULT ((0)) NULL,
	[Description]    nvarchar(250)  NOT NULL DEFAULT (''),
    CONSTRAINT [PK_DocumentGroupingConfigurations] PRIMARY KEY CLUSTERED ([GroupingId] ASC, [MergeProjectId] ASC),
	CONSTRAINT [FK_DocumentGroupingConfigurations_document_mergeprojects] FOREIGN KEY([MergeProjectId]) REFERENCES [dbo].[document_mergeprojects] ([mergeprojectid])
);