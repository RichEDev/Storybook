CREATE TABLE [dbo].[ESRElements] (
    [elementID]       INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [globalElementID] INT NOT NULL,
    [NHSTrustID]      INT NOT NULL
);

