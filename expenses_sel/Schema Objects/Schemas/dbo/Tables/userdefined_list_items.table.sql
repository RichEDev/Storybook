CREATE TABLE [dbo].[userdefined_list_items] (
    [userdefineid] INT           NOT NULL,
    [item]         NVARCHAR (50) NOT NULL,
    [order]        INT           NOT NULL,
    [valueid]      INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL
);

