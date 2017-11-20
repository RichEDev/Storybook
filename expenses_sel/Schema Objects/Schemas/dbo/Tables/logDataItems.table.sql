CREATE TABLE [dbo].[logDataItems] (
    [logDataID]    INT             IDENTITY (1, 1) NOT NULL,
    [logID]        INT             NOT NULL,
    [logReasonID]  INT             NULL,
    [logDataItem]  NVARCHAR (4000) NOT NULL,
    [createdon]    DATETIME        NULL,
    [logElementID] INT             NULL
);

