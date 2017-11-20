CREATE TABLE [dbo].[item_roles] (
    [itemroleid]  INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [rolename]    NVARCHAR (50)   NOT NULL,
    [description] NVARCHAR (4000) NULL,
    [CreatedOn]   DATETIME        NULL,
    [CreatedBy]   INT             NULL,
    [ModifiedOn]  DATETIME        NULL,
    [ModifiedBy]  INT             NULL
);

