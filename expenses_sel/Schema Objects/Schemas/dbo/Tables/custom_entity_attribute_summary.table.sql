CREATE TABLE [dbo].[custom_entity_attribute_summary] (
    [summary_attributeid] INT     IDENTITY (1, 1) NOT NULL,
    [attributeid]         INT     NOT NULL,
    [otm_attributeid]     INT     NOT NULL,
    [order]               TINYINT NULL
);

