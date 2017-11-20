CREATE TABLE [dbo].[teams] (
    [teamid]       INT             IDENTITY (1, 1) NOT NULL,
    [teamname]     NVARCHAR (50)   NOT NULL,
    [description]  NVARCHAR (4000) NULL,
    [CreatedOn]    DATETIME        NULL,
    [CreatedBy]    INT             NULL,
    [ModifiedOn]   DATETIME        NULL,
    [ModifiedBy]   INT             NULL,
    [teamleaderid] INT             NULL,
    [subAccountId] INT             NULL
);

