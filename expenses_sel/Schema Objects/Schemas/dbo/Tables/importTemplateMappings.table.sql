CREATE TABLE [dbo].[importTemplateMappings] (
    [templateMappingID] INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [templateID]        INT              NOT NULL,
    [fieldID]           UNIQUEIDENTIFIER NOT NULL,
    [destinationField]  NVARCHAR (250)   NULL,
    [columnRef]         INT              NULL,
    [importElementType] TINYINT          NOT NULL,
    [mandatory]         BIT              NOT NULL,
    [dataType]          TINYINT          NOT NULL
);

