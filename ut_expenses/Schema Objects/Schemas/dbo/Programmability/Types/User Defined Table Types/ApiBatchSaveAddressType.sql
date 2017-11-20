CREATE TYPE [dbo].[ApiBatchSaveAddressType] AS TABLE (
    [AddressID]            INT            NULL,
    [AddressName]          NVARCHAR (500) NULL,
    [Archived]             BIT            NULL,
    [Line1]                NVARCHAR (500) NULL,
    [Line2]                NVARCHAR (500) NULL,
    [Line3]                NVARCHAR (500) NULL,
    [City]                 NVARCHAR (500) NULL,
    [County]               NVARCHAR (500) NULL,
    [Country]              INT            NULL,
    [Postcode]             NVARCHAR (500) NULL,
    [Latitude]             NVARCHAR (500) NULL,
    [Longitude]            NVARCHAR (500) NULL,
    [CreationMethod]       INT            NULL,
    [GlobalIdentifier]     NVARCHAR (500) NULL,
    [Udprn]                INT            NULL,
    [LookupDate]           DATETIME       NULL,
    [SubAccountID]         INT            NULL,
    [AccountWideFavourite] BIT            NULL,
    [ESRAddressID]         BIGINT         NULL,
    [ESRLocationID]        BIGINT         NULL,
    [CreatedOn]            DATETIME       NULL,
    [CreatedBy]            INT            NULL,
    [ModifiedOn]           DATETIME       NULL,
    [ModifiedBy]           INT            NULL);



