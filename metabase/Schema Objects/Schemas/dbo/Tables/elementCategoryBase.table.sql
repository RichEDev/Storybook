CREATE TABLE [dbo].[elementCategoryBase] (
    [categoryID]   INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [categoryName] NVARCHAR (100)  COLLATE Latin1_General_CI_AS NOT NULL,
    [description]  NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [moduleID]     INT             NOT NULL
);

