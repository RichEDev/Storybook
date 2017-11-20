CREATE TABLE [dbo].[document_mergesources] (
    [mergesourceid]  INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [mergeprojectid] INT              NULL,
    [oldreportid]    INT              NULL,
    [createddate]    DATETIME         NOT NULL,
    [createdby]      INT              NOT NULL,
    [reportid]       UNIQUEIDENTIFIER NULL
);

