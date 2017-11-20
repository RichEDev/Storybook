CREATE TABLE [dbo].[OutboundFileData] (
    [DataID]         INT             IDENTITY (1, 1) NOT NULL,
    [FileData]       VARBINARY (MAX) NULL,
    [NHSTrustID]     INT             NOT NULL,
    [FileName]       NVARCHAR (250)  NOT NULL,
    [FileCreatedOn]  DATETIME        NULL,
    [FileModifiedOn] DATETIME        NULL,
    [Status]         TINYINT         CONSTRAINT [DF_OutboundFileData_Status] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_OutboundFileData] PRIMARY KEY CLUSTERED ([DataID] ASC)
);

