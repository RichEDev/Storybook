CREATE TABLE [dbo].[globalESRElementFields] (
    [globalESRElementFieldID] INT            IDENTITY (387, 1) NOT FOR REPLICATION NOT NULL,
    [globalESRElementID]      INT            NOT NULL,
    [ESRElementFieldName]     NVARCHAR (250) COLLATE Latin1_General_CI_AS NOT NULL,
    [isMandatory]             BIT            CONSTRAINT [DF_globalESRElementFields_isMandatory] DEFAULT ((0)) NOT NULL,
    [isControlColumn]         BIT            CONSTRAINT [DF_globalESRElementFields_isControlColumn] DEFAULT ((0)) NOT NULL,
	[isSummaryColumn]         BIT            CONSTRAINT [DF_globalESRElementFields_isSummaryColumn] DEFAULT ((0)) NOT NULL,
	[isRounded]               BIT            CONSTRAINT [DF_globalESRElementFields_isRounded]       DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_globalESRElementFields] PRIMARY KEY CLUSTERED ([globalESRElementFieldID] ASC),
    CONSTRAINT [FK_globalESRElementFields_globalESRElements] FOREIGN KEY ([globalESRElementID]) REFERENCES [dbo].[globalESRElements] ([globalESRElementID]) ON DELETE CASCADE ON UPDATE CASCADE
);



