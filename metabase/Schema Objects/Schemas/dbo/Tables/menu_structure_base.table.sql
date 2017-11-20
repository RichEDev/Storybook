CREATE TABLE [dbo].[menu_structure_base] (
    [menuid]    INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [menu_name] NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [parentid]  INT            NULL
);

