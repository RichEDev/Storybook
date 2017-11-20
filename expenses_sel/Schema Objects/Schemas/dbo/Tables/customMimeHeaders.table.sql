CREATE TABLE [dbo].[customMimeHeaders] (
    [fileExtension] NVARCHAR (50)    NOT NULL,
    [mimeHeader]    NVARCHAR (150)   NOT NULL,
    [description]   NVARCHAR (500)   NULL,
    [createdOn]     DATETIME         NULL,
    [createdBy]     INT              NULL,
    [modifiedOn]    DATETIME         NULL,
    [modifiedBy]    INT              NULL,
    [customMimeID]  UNIQUEIDENTIFIER NOT NULL
);

