CREATE TABLE [dbo].[audiences] (
    [audienceID]   INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [audienceName] NVARCHAR (250) NOT NULL,
    [description]  NVARCHAR (MAX) NULL,
    [createdOn]    DATETIME       NOT NULL,
    [createdBy]    INT            NOT NULL,
    [modifiedOn]   DATETIME       NULL,
    [modifiedBy]   INT            NULL
);

