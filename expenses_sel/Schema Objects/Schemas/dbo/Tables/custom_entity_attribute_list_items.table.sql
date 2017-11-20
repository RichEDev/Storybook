CREATE TABLE [dbo].[custom_entity_attribute_list_items] (
    [attributeid] INT           NOT NULL,
    [item]        NVARCHAR (50) NOT NULL,
    [order]       INT           NOT NULL,
    [valueid]     INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL
);

