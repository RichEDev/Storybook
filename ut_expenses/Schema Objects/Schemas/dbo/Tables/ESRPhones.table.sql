CREATE TABLE [dbo].[ESRPhones] (
    [ESRPhoneId]         BIGINT        NOT NULL,
    [ESRPersonId]        BIGINT        NOT NULL,
    [PhoneType]          NVARCHAR (30) NOT NULL,
    [PhoneNumber]        NVARCHAR (60) NOT NULL,
    [EffectiveStartDate] DATETIME      NOT NULL,
    [EffectiveEndDate]   DATETIME      NULL,
    [ESRLastUpdate]      DATETIME      NULL,
    CONSTRAINT [PK_ESRPhones] PRIMARY KEY CLUSTERED ([ESRPhoneId] ASC)
);

