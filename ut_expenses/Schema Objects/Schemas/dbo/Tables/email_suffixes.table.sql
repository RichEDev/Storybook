CREATE TABLE [dbo].[email_suffixes] (
    [suffixid]   INT            IDENTITY (1, 1)  NOT NULL,
    [suffix]     NVARCHAR (400) NOT NULL,
    [CreatedOn]  DATETIME       NULL,
    [CreatedBy]  INT            NULL,
    [ModifiedOn] DATETIME       NULL,
    [ModifiedBy] INT            NULL
);

