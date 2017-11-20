CREATE TYPE [dbo].[ApiBatchSaveEsrAddressType] AS TABLE (
    [ESRAddressId]       BIGINT         NULL,
    [ESRPersonId]        BIGINT         NULL,
    [AddressType]        NVARCHAR (500) NULL,
    [AddressStyle]       NVARCHAR (500) NULL,
    [PrimaryFlag]        NVARCHAR (500) NULL,
    [AddressLine1]       NVARCHAR (500) NULL,
    [AddressLine2]       NVARCHAR (500) NULL,
    [AddressLine3]       NVARCHAR (500) NULL,
    [AddressTown]        NVARCHAR (500) NULL,
    [AddressCounty]      NVARCHAR (500) NULL,
    [AddressPostcode]    NVARCHAR (500) NULL,
    [AddressCountry]     NVARCHAR (500) NULL,
    [EffectiveStartDate] DATETIME       NULL,
    [EffectiveEndDate]   DATETIME       NULL,
    [ESRLastUpdate]      DATETIME       NULL);

