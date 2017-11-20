CREATE TABLE [dbo].[supplier_categories] (
    [categoryid]   INT           IDENTITY (1, 1) NOT NULL,
    [description]  NVARCHAR (50) NULL,
    [createdon]    DATETIME      NULL,
    [createdby]    INT           NULL,
    [modifiedon]   DATETIME      NULL,
    [modifiedby]   INT           NULL,
    [subAccountId] INT           NULL,
    [archived]     BIT           NOT NULL
);

