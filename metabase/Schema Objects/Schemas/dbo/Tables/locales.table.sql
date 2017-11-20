CREATE TABLE [dbo].[locales] (
    [localeID]   INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [localeName] NVARCHAR (250) COLLATE Latin1_General_CI_AS NOT NULL,
    [localeCode] NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [active]     BIT            NOT NULL
);

