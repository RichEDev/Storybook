CREATE TABLE [dbo].[codes_contractcategory] (
    [categoryId]          INT           IDENTITY (1, 1) NOT NULL,
    [subAccountId]        INT           NULL,
    [categoryDescription] NVARCHAR (50) NULL,
    [archived]            BIT           NOT NULL,
    [createdOn]           DATETIME      NULL,
    [createdBy]           INT           NULL,
    [modifiedOn]          DATETIME      NULL,
    [modifiedBy]          INT           NULL
);

