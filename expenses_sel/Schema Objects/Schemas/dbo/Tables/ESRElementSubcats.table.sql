CREATE TABLE [dbo].[ESRElementSubcats] (
    [elementSubcatID] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [elementID]       INT NOT NULL,
    [subcatID]        INT NOT NULL
);

