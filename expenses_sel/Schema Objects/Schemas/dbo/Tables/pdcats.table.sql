CREATE TABLE [dbo].[pdcats] (
    [pdcatid]    INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [pdname]     NVARCHAR (50) NOT NULL,
    [CreatedOn]  DATETIME      NULL,
    [CreatedBy]  INT           NULL,
    [ModifiedOn] DATETIME      NULL,
    [ModifiedBy] INT           NULL
);

