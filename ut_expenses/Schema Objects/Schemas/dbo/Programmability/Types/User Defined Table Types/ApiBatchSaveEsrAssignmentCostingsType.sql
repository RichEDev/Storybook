CREATE TYPE [dbo].[ApiBatchSaveEsrAssignmentCostingsType] AS TABLE (
    [ESRCostingAllocationId] BIGINT         NULL,
    [ESRPersonId]            BIGINT         NULL,
    [ESRAssignmentId]        BIGINT         NULL,
    [EffectiveStartDate]     DATETIME       NULL,
    [EffectiveEndDate]       DATETIME       NULL,
    [EntityCode]             NVARCHAR (500) NULL,
    [CharitableIndicator]    NVARCHAR (500) NULL,
    [CostCentre]             NVARCHAR (500) NULL,
    [Subjective]             NVARCHAR (500) NULL,
    [Analysis1]              NVARCHAR (500) NULL,
    [Analysis2]              NVARCHAR (500) NULL,
    [ElementNumber]          INT            NULL,
    [SpareSegment]           NVARCHAR (500) NULL,
    [PercentageSplit]        DECIMAL (5, 2) NULL,
    [ESRLastUpdate]          DATETIME       NULL,
    [EsrAssignId]            INT            NULL);

