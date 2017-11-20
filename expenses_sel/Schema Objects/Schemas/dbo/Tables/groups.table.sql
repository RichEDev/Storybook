CREATE TABLE [dbo].[groups] (
    [groupid]               INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [groupname]             NVARCHAR (50)   NOT NULL,
    [description]           NVARCHAR (4000) NULL,
    [CreatedOn]             DATETIME        NULL,
    [CreatedBy]             INT             NULL,
    [ModifiedOn]            DATETIME        NULL,
    [ModifiedBy]            INT             NULL,
    [oneClickAuthorisation] BIT             NOT NULL
);

