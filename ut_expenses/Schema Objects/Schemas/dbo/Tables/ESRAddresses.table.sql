CREATE TABLE [dbo].[ESRAddresses] (
    [ESRAddressId]       BIGINT         NOT NULL,
    [ESRPersonId]        BIGINT         NULL,
    [AddressType]        NVARCHAR (30)  NULL,
    [AddressStyle]       NVARCHAR (30)  NOT NULL,
    [PrimaryFlag]        NVARCHAR (30)  NOT NULL,
    [AddressLine1]       NVARCHAR (240) NULL,
    [AddressLine2]       NVARCHAR (240) NULL,
    [AddressLine3]       NVARCHAR (240) NULL,
    [AddressTown]        NVARCHAR (30)  NULL,
    [AddressCounty]      NVARCHAR (70)  NULL,
    [AddressPostcode]    NVARCHAR (30)  NULL,
    [AddressCountry]     NVARCHAR (60)  NULL,
    [EffectiveStartDate] DATETIME       NOT NULL,
    [EffectiveEndDate]   DATETIME       NULL,
    [ESRLastUpdate]      DATETIME       NULL,
    CONSTRAINT [PK_ESRAddresses] PRIMARY KEY CLUSTERED ([ESRAddressId] ASC)
);

