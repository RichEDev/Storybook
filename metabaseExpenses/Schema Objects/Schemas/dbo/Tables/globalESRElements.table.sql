﻿CREATE TABLE [dbo].[globalESRElements] (
    [globalESRElementID] INT            IDENTITY (55, 1) NOT FOR REPLICATION NOT NULL,
    [ESRElementName]     NVARCHAR (250) COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK_globalESRElements] PRIMARY KEY CLUSTERED ([globalESRElementID] ASC)
);



