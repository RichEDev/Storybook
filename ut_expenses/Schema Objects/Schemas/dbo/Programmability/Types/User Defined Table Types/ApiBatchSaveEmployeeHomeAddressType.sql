CREATE TYPE [dbo].[ApiBatchSaveEmployeeHomeAddressType] AS TABLE (
    [EmployeeHomeAddressId] INT      NULL,
    [EmployeeId]            INT      NULL,
    [AddressId]             INT      NULL,
    [StartDate]             DATETIME NULL,
    [EndDate]               DATETIME NULL,
    [CreatedOn]             DATETIME NULL,
    [CreatedBy]             INT      NULL,
    [ModifiedOn]            DATETIME NULL,
    [ModifiedBy]            INT      NULL);

