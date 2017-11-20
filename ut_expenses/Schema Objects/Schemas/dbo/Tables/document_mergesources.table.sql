CREATE TABLE [dbo].[document_mergesources] (
    [mergesourceid]  INT              IDENTITY (1, 1) NOT NULL,
    [mergeprojectid] INT              NULL,
    [oldreportid]    INT              NULL,
    [createddate]    DATETIME         NOT NULL,
    [createdby]      INT              NOT NULL,
    [reportid]       UNIQUEIDENTIFIER NULL,
    [groupingsource] BIT              CONSTRAINT [DF_document_mergesources_groupingsource] DEFAULT ((0)) NOT NULL,
    [sortingsource]  BIT              CONSTRAINT [DF_document_mergesources_sortingsource] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_document_mergesources] PRIMARY KEY CLUSTERED ([mergesourceid] ASC),
    CONSTRAINT [FK_document_mergesources_document_mergeprojects] FOREIGN KEY ([mergeprojectid]) REFERENCES [dbo].[document_mergeprojects] ([mergeprojectid]) ON DELETE CASCADE,
    CONSTRAINT [FK_document_mergesources_reports] FOREIGN KEY ([reportid]) REFERENCES [dbo].[reports] ([reportID]) ON DELETE CASCADE
);





