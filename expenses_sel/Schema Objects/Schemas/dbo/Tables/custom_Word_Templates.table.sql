CREATE TABLE [dbo].[custom_Word_Templates] (
    [id]         INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [CreatedOn]  DATETIME        NULL,
    [CreatedBy]  INT             NULL,
    [ModifiedOn] DATETIME        NULL,
    [ModifiedBy] INT             NULL,
    [att1260]    INT             NULL,
    [att1261]    NVARCHAR (4000) NULL,
    [att1262]    NVARCHAR (4000) NULL
);

