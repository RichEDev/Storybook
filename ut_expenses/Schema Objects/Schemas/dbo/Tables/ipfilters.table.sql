CREATE TABLE [dbo].[ipfilters] (
    [ipFilterID]  INT             IDENTITY (1, 1) NOT NULL,
    [ipAddress]   NVARCHAR (50)   NOT NULL,
    [description] NVARCHAR (4000) NULL,
    [active]      BIT             NOT NULL,
    [createdOn]   DATETIME        NULL,
    [createdBy]   INT             NULL,
    [modifiedOn]  DATETIME        NULL,
    [modifiedBy]  INT             NULL
);

