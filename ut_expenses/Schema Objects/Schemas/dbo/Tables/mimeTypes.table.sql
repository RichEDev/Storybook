CREATE TABLE [dbo].[mimeTypes] (
    [mimeID]       INT              IDENTITY (1, 1) NOT NULL,
    [subAccountID] INT              NULL,
    [archived]     BIT              NOT NULL,
    [createdOn]    DATETIME         NULL,
    [createdBy]    INT              NULL,
    [modifiedOn]   DATETIME         NULL,
    [modifiedBy]   INT              NULL,
    [globalMimeID] UNIQUEIDENTIFIER NOT NULL
);

