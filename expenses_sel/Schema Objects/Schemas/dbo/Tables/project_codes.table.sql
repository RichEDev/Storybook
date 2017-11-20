CREATE TABLE [dbo].[project_codes] (
    [projectcodeid] INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [projectcode]   NVARCHAR (50)   NOT NULL,
    [description]   NVARCHAR (2000) NULL,
    [archived]      BIT             NOT NULL,
    [rechargeable]  BIT             NOT NULL,
    [CreatedOn]     DATETIME        NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedOn]    DATETIME        NULL,
    [ModifiedBy]    INT             NULL
);

