CREATE TABLE [dbo].[custom_entity_form_tabs] (
    [tabid]          INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [formid]         INT            NOT NULL,
    [header_caption] NVARCHAR (100) NOT NULL,
    [order]          TINYINT        NOT NULL
);

