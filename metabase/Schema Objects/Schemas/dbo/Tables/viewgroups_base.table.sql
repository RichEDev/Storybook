CREATE TABLE [dbo].[viewgroups_base] (
    [viewgroupid_old] INT              NULL,
    [groupname]       NVARCHAR (50)    COLLATE Latin1_General_CI_AS NOT NULL,
    [parentid_old]    INT              NULL,
    [level]           INT              NOT NULL,
    [amendedon]       DATETIME         NULL,
    [viewgroupid]     UNIQUEIDENTIFIER NOT NULL,
    [parentid]        UNIQUEIDENTIFIER NULL,
    [alias]           NVARCHAR (150)   COLLATE Latin1_General_CI_AS NULL
);

