CREATE TABLE [dbo].[locales] (
    [localeID]   INT            IDENTITY (153, 1) NOT FOR REPLICATION NOT NULL,
    [localeName] NVARCHAR (250) COLLATE Latin1_General_CI_AS NOT NULL,
    [localeCode] NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [active]     BIT            CONSTRAINT [DF_locales_active] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_locales] PRIMARY KEY CLUSTERED ([localeID] ASC),
    CONSTRAINT [IX_locales] UNIQUE NONCLUSTERED ([localeName] ASC)
);



