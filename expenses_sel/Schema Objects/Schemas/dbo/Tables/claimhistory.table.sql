CREATE TABLE [dbo].[claimhistory] (
    [claimhistoryid] INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [claimid]        INT             NOT NULL,
    [datestamp]      DATETIME        NULL,
    [comment]        NVARCHAR (4000) NOT NULL,
    [stage]          TINYINT         NOT NULL,
    [refnum]         NVARCHAR (50)   NULL,
    [employeeid]     INT             NULL,
    [createdon]      DATETIME        NULL,
    [createdby]      INT             NULL,
    [modifiedon]     DATETIME        NULL,
    [modifiedby]     INT             NULL,
    [CacheExpiry]    DATETIME        NULL
);

