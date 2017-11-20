CREATE TABLE [dbo].[ESRAssignmentCostings] (
    [ESRCostingAllocationId] BIGINT         NOT NULL,
    [ESRPersonId]            BIGINT         NOT NULL,
    [ESRAssignmentId]        BIGINT         NOT NULL,
    [EffectiveStartDate]     DATETIME       NOT NULL,
    [EffectiveEndDate]       DATETIME       NULL,
    [EntityCode]             NVARCHAR (3)   NULL,
    [CharitableIndicator]    NVARCHAR (1)   NULL,
    [CostCentre]             NVARCHAR (15)  NULL,
    [Subjective]             NVARCHAR (15)  NULL,
    [Analysis1]              NVARCHAR (15)  NULL,
    [Analysis2]              NVARCHAR (15)  NULL,
    [ElementNumber]          INT            NULL,
    [SpareSegment]           NVARCHAR (60)  NULL,
    [PercentageSplit]        DECIMAL (5, 2) NULL,
    [ESRLastUpdate]          DATETIME       NULL,
    [ESRAssignId]            INT            NULL,
    CONSTRAINT [PK_ESRAssignmentCostings] PRIMARY KEY CLUSTERED ([ESRCostingAllocationId] ASC)
);

