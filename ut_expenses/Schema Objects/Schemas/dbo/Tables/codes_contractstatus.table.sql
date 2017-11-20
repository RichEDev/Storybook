CREATE TABLE [dbo].[codes_contractstatus] (
    [statusId]          INT           IDENTITY (1, 1) NOT NULL,
    [subAccountId]      INT           NULL,
    [statusDescription] NVARCHAR (50) NOT NULL,
    [createdOn]         DATETIME      NULL,
    [createdBy]         INT           NULL,
    [modifiedOn]        DATETIME      NULL,
    [modifiedBy]        INT           NULL,
    [archived]          BIT           NOT NULL,
    [isArchive]         BIT           NOT NULL
);

