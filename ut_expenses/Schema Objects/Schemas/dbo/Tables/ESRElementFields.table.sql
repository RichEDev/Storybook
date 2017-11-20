CREATE TABLE [dbo].[ESRElementFields] (
    [elementFieldID]       INT              IDENTITY (1, 1)  NOT NULL,
    [elementID]            INT              NOT NULL,
    [globalElementFieldID] INT              NOT NULL,
    [aggregate]            TINYINT          NULL,
    [order]                TINYINT          NOT NULL,
    [reportColumnID]       UNIQUEIDENTIFIER NOT NULL
);

