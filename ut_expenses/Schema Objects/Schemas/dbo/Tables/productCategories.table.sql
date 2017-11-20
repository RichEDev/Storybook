CREATE TABLE [dbo].[productCategories] (
    [categoryId]   INT          IDENTITY (1, 1) NOT NULL,
    [subAccountId] INT          NOT NULL,
    [Description]  VARCHAR (50) NULL,
    [archived]     BIT          NOT NULL,
    [createdon]    DATETIME     NOT NULL,
    [createdby]    INT          NOT NULL,
    [modifiedon]   DATETIME     NULL,
    [modifiedby]   INT          NULL
);

