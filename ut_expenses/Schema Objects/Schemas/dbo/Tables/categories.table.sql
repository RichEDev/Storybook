CREATE TABLE [dbo].[categories] (
    [categoryid]  INT             IDENTITY (1, 1) NOT NULL,
    [category]    NVARCHAR (50)   NOT NULL,
    [description] NVARCHAR (4000) NULL,
    [CreatedOn]   DATETIME        NULL,
    [CreatedBy]   INT             NULL,
    [ModifiedOn]  DATETIME        NULL,
    [ModifiedBy]  INT             NULL,
    CONSTRAINT [PK_categories] PRIMARY KEY NONCLUSTERED ([categoryid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_categories_categories] FOREIGN KEY ([categoryid]) REFERENCES [dbo].[categories] ([categoryid])
);



