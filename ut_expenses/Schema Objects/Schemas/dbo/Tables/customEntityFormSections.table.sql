CREATE TABLE [dbo].[customEntityFormSections] (
    [sectionid]      INT            IDENTITY (1, 1)  NOT NULL,
    [formid]         INT            NOT NULL,
    [header_caption] NVARCHAR (100) NOT NULL,
    [tabid]          INT            NULL,
    [order]          TINYINT        NOT NULL
);

