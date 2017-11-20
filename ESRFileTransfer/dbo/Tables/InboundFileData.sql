CREATE TABLE [dbo].[InboundFileData] (
    [DataID]            INT             IDENTITY (1, 1) NOT NULL,
    [FileData]          VARBINARY (MAX) NOT NULL,
    [FileName]          NVARCHAR (250)  NOT NULL,
    [NHSTrustID]        INT             NOT NULL,
    [FinancialExportID] INT             NOT NULL,
    [Status]            TINYINT         CONSTRAINT [DF_InboundFileData_Status] DEFAULT ((0)) NOT NULL,
    [ExportHistoryID]   INT             NOT NULL,
    CONSTRAINT [PK_InboundFileData] PRIMARY KEY CLUSTERED ([DataID] ASC)
);

