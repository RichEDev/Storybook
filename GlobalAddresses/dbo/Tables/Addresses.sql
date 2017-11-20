CREATE TABLE [dbo].[Addresses] (
    [AddressID]          INT            IDENTITY (1, 1) NOT NULL,
    [Postcode]           NVARCHAR (32)  NULL,
    [AddressName]        NVARCHAR (250) NULL,
    [Line1]              NVARCHAR (256) NULL,
    [Line2]              NVARCHAR (256) NULL,
    [Line3]              NVARCHAR (256) NULL,
    [City]               NVARCHAR (256) NULL,
    [County]             NVARCHAR (256) NULL,
    [Country]            INT            NULL,
    [LookupDate]         DATETIME       NULL,
    [ProviderIdentifier] NVARCHAR (50)  NULL,
    [Longitude]          NVARCHAR (20)  NULL,
    [Latitude]           NVARCHAR (20)  NULL,
    [CreatedOn]          DATETIME       NULL,
    [ModifiedOn]         DATETIME       NULL,
    [Udprn]              INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED ([AddressID] ASC)
);

