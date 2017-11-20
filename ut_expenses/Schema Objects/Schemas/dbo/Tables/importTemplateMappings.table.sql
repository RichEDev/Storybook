CREATE TABLE [dbo].[importTemplateMappings] (
    [templateMappingID] INT              IDENTITY (1, 1)  NOT NULL,
    [templateID]        INT              NOT NULL,
    [fieldID]           UNIQUEIDENTIFIER NOT NULL,
    [destinationField]  NVARCHAR (250)   NULL,
    [columnRef]         INT              NULL,
    [importElementType] TINYINT          NOT NULL,
    [mandatory]         BIT              NOT NULL,
    [dataType]          TINYINT          NOT NULL,
	[lookupTable]		UNIQUEIDENTIFIER NULL,
	[matchField]		UNIQUEIDENTIFIER NULL,
	[overridePrimaryKey] BIT			 NULL,
	[importField]		BIT				 NULL
);

