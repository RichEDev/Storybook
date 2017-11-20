CREATE TABLE [dbo].[customEntityAttributeListItems] (
    [attributeid] INT            NOT NULL,
    [item]        NVARCHAR (150) NOT NULL,
    [order]       INT            NOT NULL,
    [valueid]     INT            IDENTITY (1, 1)  NOT NULL,
	[archived]    BIT			 NOT NULL 
);

