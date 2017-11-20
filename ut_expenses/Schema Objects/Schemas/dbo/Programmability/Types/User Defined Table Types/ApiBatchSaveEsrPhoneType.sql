CREATE TYPE [dbo].[ApiBatchSaveEsrPhoneType] AS TABLE (
    [ESRPhoneId]         BIGINT         NULL,
    [ESRPersonId]        BIGINT         NULL,
    [PhoneType]          NVARCHAR (500) NULL,
    [PhoneNumber]        NVARCHAR (500) NULL,
    [EffectiveStartDate] DATETIME       NULL,
    [EffectiveEndDate]   DATETIME       NULL,
    [ESRLastUpdate]      DATETIME       NULL);

