CREATE TABLE [dbo].[menu_structure_base] (
    [menuid]    INT            IDENTITY (8, 1) NOT FOR REPLICATION NOT NULL,
    [menu_name] NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [parentid]  INT            NULL,
    CONSTRAINT [PK_menu_structure] PRIMARY KEY CLUSTERED ([menuid] ASC),
    CONSTRAINT [FK_menu_structure_menu_structure] FOREIGN KEY ([parentid]) REFERENCES [dbo].[menu_structure_base] ([menuid])
);



