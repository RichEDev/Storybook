CREATE TABLE [dbo].[budgetholders] (
    [budgetholderid] INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [budgetholder]   NVARCHAR (50)   NOT NULL,
    [description]    NVARCHAR (4000) NULL,
    [employeeid]     INT             NOT NULL,
    [CreatedOn]      DATETIME        NULL,
    [CreatedBy]      INT             NULL,
    [ModifiedOn]     DATETIME        NULL,
    [ModifiedBy]     INT             NULL
);

