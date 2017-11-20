CREATE TABLE [dbo].[countrysubcats] (
    [countrysubcatid] INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [countryid]       INT      NOT NULL,
    [subcatid]        INT      NOT NULL,
    [vat]             FLOAT    NOT NULL,
    [vatpercent]      FLOAT    NOT NULL,
    [createdon]       DATETIME NOT NULL,
    [createdby]       INT      NULL
);

