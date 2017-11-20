CREATE TABLE [dbo].[custom_viewgroups] (
    [entity_name] NVARCHAR (250)   NULL,
    [level]       INT              NULL,
    [amendedon]   DATETIME         NULL,
    [viewgroupid] UNIQUEIDENTIFIER NOT NULL,
    [parentid]    UNIQUEIDENTIFIER NULL,
    [alias]       NVARCHAR (150)   NULL
);

