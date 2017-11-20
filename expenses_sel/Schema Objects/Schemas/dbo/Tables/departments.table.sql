CREATE TABLE [dbo].[departments] (
    [departmentid] INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [department]   NVARCHAR (50)   NOT NULL,
    [description]  NVARCHAR (4000) NULL,
    [archived]     BIT             NOT NULL,
    [CreatedOn]    DATETIME        NULL,
    [CreatedBy]    INT             NULL,
    [ModifiedOn]   DATETIME        NULL,
    [ModifiedBy]   INT             NULL
);

