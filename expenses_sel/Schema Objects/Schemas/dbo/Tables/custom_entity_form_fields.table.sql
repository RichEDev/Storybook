CREATE TABLE [dbo].[custom_entity_form_fields] (
    [formid]      INT     NOT NULL,
    [attributeid] INT     NOT NULL,
    [readonly]    BIT     NOT NULL,
    [sectionid]   INT     NULL,
    [row]         TINYINT NOT NULL,
    [column]      TINYINT NOT NULL
);

