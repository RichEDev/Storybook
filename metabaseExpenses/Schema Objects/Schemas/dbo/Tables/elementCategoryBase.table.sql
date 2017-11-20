CREATE TABLE [dbo].[elementCategoryBase] (
    [categoryID]   INT             IDENTITY (22, 1) NOT FOR REPLICATION NOT NULL,
    [categoryName] NVARCHAR (100)  COLLATE Latin1_General_CI_AS NOT NULL,
    [description]  NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [moduleID]     INT             NOT NULL,
    CONSTRAINT [PK_module_category_base] PRIMARY KEY CLUSTERED ([categoryID] ASC),
    CONSTRAINT [FK_elementCategoryBase_moduleBase] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID]) NOT FOR REPLICATION
);



