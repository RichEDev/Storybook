CREATE TABLE [dbo].[ESRVehicles] (
    [ESRVehicleAllocationId]  BIGINT        NOT NULL,
    [ESRPersonId]             BIGINT        NOT NULL,
    [ESRAssignmentId]         BIGINT        NOT NULL,
    [EffectiveStartDate]      DATETIME      NULL,
    [EffectiveEndDate]        DATETIME      NULL,
    [RegistrationNumber]      NVARCHAR (30) NULL,
    [Make]                    NVARCHAR (30) NULL,
    [Model]                   NVARCHAR (30) NULL,
    [Ownership]               NVARCHAR (30) NULL,
    [InitialRegistrationDate] DATETIME      NULL,
    [EngineCC]                INT           NULL,
    [ESRLastUpdate]           DATETIME      NULL,
    [UserRatesTable]          NVARCHAR (80) NULL,
    [FuelType]                NVARCHAR (30) NULL,
    [ESRAssignId]             INT           NULL,
    CONSTRAINT [PK_ESRVehicles] PRIMARY KEY CLUSTERED ([ESRVehicleAllocationId] ASC)
);


