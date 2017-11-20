CREATE TABLE [dbo].[custom_menu_structure] (
    [menuid]    INT            IDENTITY (10000, 1) NOT FOR REPLICATION NOT NULL,
    [menu_name] NVARCHAR (100) NOT NULL,
    [parentid]  INT            NULL
);

