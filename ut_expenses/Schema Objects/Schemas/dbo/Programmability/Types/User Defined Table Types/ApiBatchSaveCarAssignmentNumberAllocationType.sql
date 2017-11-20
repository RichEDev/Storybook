CREATE TYPE [dbo].[ApiBatchSaveCarAssignmentNumberAllocationType] AS TABLE (
    [ESRVehicleAllocationId] BIGINT NULL,
    [ESRAssignId]            INT    NULL,
    [CarId]                  INT    NULL,
    [Archived]               BIT    NULL);

