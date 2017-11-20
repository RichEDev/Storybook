
CREATE PROCEDURE [dbo].[APIgetEsrAssignmentCostingsByEsrId]
	@EsrId bigint 

AS	
BEGIN
	SELECT [ESRAssignmentCostings].[ESRCostingAllocationId]
		  ,[ESRAssignmentCostings].[ESRPersonId]
		  ,[ESRAssignmentCostings].[ESRAssignmentId]
		  ,[ESRAssignmentCostings].[EffectiveStartDate]
		  ,[ESRAssignmentCostings].[EffectiveEndDate]
		  ,[ESRAssignmentCostings].[EntityCode]
		  ,[ESRAssignmentCostings].[CharitableIndicator]
		  ,[ESRAssignmentCostings].[CostCentre]
		  ,[ESRAssignmentCostings].[Subjective]
		  ,[ESRAssignmentCostings].[Analysis1]
		  ,[ESRAssignmentCostings].[Analysis2]
		  ,[ESRAssignmentCostings].[ElementNumber]
		  ,[ESRAssignmentCostings].[SpareSegment]
		  ,[ESRAssignmentCostings].[PercentageSplit]
		  ,[ESRAssignmentCostings].[ESRLastUpdate]
		  ,[ESRAssignmentCostings].[EsrAssignID]
	FROM [dbo].[ESRAssignmentCostings]
		join esr_assignments on esr_assignments.ESRAssignID = [ESRAssignmentCostings].[ESRAssignID]
	WHERE  esr_assignments.[ESRPersonId] = @EsrId
END