CREATE TABLE [dbo].[globalESRElementFields] (
    [globalESRElementFieldID] INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [globalESRElementID]      INT            NOT NULL,
    [ESRElementFieldName]     NVARCHAR (250) COLLATE Latin1_General_CI_AS NOT NULL,
    [isMandatory]             BIT            NOT NULL,
    [isControlColumn]         BIT            NOT NULL
);

