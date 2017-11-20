CREATE TYPE [dbo].[ApiBatchSaveEsrVehicleType] AS TABLE (
    [ESRVehicleAllocationId]  BIGINT          NULL,
    [ESRPersonId]             BIGINT          NULL,
    [ESRAssignmentId]         BIGINT          NULL,
    [EffectiveStartDate]      DATETIME        NULL,
    [EffectiveEndDate]        DATETIME        NULL,
    [RegistrationNumber]      NVARCHAR (500)  NULL,
    [Make]                    NVARCHAR (500)  NULL,
    [Model]                   NVARCHAR (500)  NULL,
    [Ownership]               NVARCHAR (500)  NULL,
    [InitialRegistrationDate] DATETIME        NULL,
    [EngineCC]                DECIMAL (12, 3) NULL,
    [ESRLastUpdate]           DATETIME        NULL,
    [UserRatesTable]          NVARCHAR (500)  NULL,
    [FuelType]                NVARCHAR (500)  NULL,
    [ESRAssignId]             INT             NULL);

