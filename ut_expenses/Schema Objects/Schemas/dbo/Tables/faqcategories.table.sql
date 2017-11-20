CREATE TABLE [dbo].[faqcategories] (
    [faqcategoryid] INT           IDENTITY (1, 1)  NOT NULL,
    [category]      NVARCHAR (50) NOT NULL,
    [CreatedOn]     DATETIME      NULL,
    [CreatedBy]     INT           NULL,
    [ModifiedOn]    DATETIME      NULL,
    [ModifiedBy]    INT           NULL
);

