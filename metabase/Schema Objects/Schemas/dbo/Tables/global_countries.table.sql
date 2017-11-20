CREATE TABLE [dbo].[global_countries] (
    [globalcountryid]     INT            IDENTITY (1, 1) NOT NULL,
    [country]             NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [countrycode]         NVARCHAR (2)   COLLATE Latin1_General_CI_AS NOT NULL,
    [createdon]           DATETIME       NULL,
    [modifiedon]          DATETIME       NULL,
    [postcodeRegexFormat] NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [alpha3CountryCode]   NVARCHAR (3)   NULL,
    [numeric3Code]        INT            NOT NULL
);

