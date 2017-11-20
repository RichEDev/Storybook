CREATE TABLE [dbo].[custom_entity_view_filters] (
    [viewid]    INT              NOT NULL,
    [fieldid]   UNIQUEIDENTIFIER NOT NULL,
    [condition] TINYINT          NOT NULL,
    [value]     NVARCHAR (150)   NOT NULL,
    [order]     TINYINT          NOT NULL
);

