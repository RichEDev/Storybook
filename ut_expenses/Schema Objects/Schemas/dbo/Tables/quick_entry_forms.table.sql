CREATE TABLE [dbo].[quick_entry_forms] (
    [quickentryid] INT             IDENTITY (1, 1)  NOT NULL,
    [name]         NVARCHAR (100)  NOT NULL,
    [description]  NVARCHAR (1000) NULL,
    [genmonth]     BIT             NOT NULL,
    [numrows]      INT             NOT NULL,
    [CreatedOn]    DATETIME        NULL,
    [CreatedBy]    INT             NULL,
    [ModifiedOn]   DATETIME        NULL,
    [ModifiedBy]   INT             NULL
);

