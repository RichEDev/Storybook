CREATE TABLE [dbo].[customEntityFormTabs] (
    [tabid]          INT            IDENTITY (1, 1)  NOT NULL,
    [formid]         INT            NOT NULL,
    [header_caption] NVARCHAR (100) NOT NULL,
    [order]          TINYINT        NOT NULL
);

