CREATE TABLE [dbo].[version_registry_calloff] (
    [CallOffId]    INT           IDENTITY (1, 1) NOT NULL,
    [RegistryId]   INT           NOT NULL,
    [QuantityUsed] INT           NOT NULL,
    [Locale]       VARCHAR (50)  NULL,
    [Comment]      VARCHAR (100) NULL,
    [DateObtained] DATETIME      NULL
);

