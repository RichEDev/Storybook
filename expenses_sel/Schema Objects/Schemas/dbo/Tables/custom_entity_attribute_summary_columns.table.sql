CREATE TABLE [dbo].[custom_entity_attribute_summary_columns] (
    [columnid]         INT            IDENTITY (1, 1) NOT NULL,
    [attributeid]      INT            NOT NULL,
    [column_fieldname] NVARCHAR (100) NOT NULL,
    [alternate_header] NVARCHAR (150) NULL,
    [width]            INT            NULL,
    [order]            TINYINT        NULL,
    [filterVal]        NVARCHAR (100) NULL,
    [default_sort]     BIT            NOT NULL
);

