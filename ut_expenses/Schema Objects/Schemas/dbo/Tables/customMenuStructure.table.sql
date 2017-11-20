CREATE TABLE [dbo].[customMenuStructure] (
    [menuid]    INT            IDENTITY (10000, 1)  NOT NULL,
    [menu_name] NVARCHAR (100) NOT NULL,
    [parentid]  INT            NULL
);

