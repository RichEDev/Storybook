CREATE TABLE [dbo].[custom_entity_views] (
    [viewid]        INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [entityid]      INT              NOT NULL,
    [view_name]     NVARCHAR (100)   NOT NULL,
    [description]   NVARCHAR (4000)  NULL,
    [createdon]     DATETIME         NOT NULL,
    [createdby]     INT              NOT NULL,
    [modifiedon]    DATETIME         NULL,
    [modifiedby]    INT              NULL,
    [menuid]        INT              NULL,
    [allowadd]      BIT              NOT NULL,
    [add_formid]    INT              NULL,
    [allowedit]     BIT              NOT NULL,
    [edit_formid]   INT              NULL,
    [allowdelete]   BIT              NOT NULL,
    [allowapproval] BIT              NOT NULL,
    [SortColumn]    UNIQUEIDENTIFIER NULL,
    [SortOrder]     TINYINT          NOT NULL
);

