CREATE PROCEDURE [dbo].[APIgetEsrAssignmentCostings]
	@ESRCostingAllocationId bigint 

AS	
BEGIN
	IF @ESRCostingAllocationId = 0
	BEGIN
		SELECT [ESRCostingAllocationId]
		  ,[ESRPersonId]
		  ,[ESRAssignmentId]
		  ,[EffectiveStartDate]
		  ,[EffectiveEndDate]
		  ,[EntityCode]
		  ,[CharitableIndicator]
		  ,[CostCentre]
		  ,[Subjective]
		  ,[Analysis1]
		  ,[Analysis2]
		  ,[ElementNumber]
		  ,[SpareSegment]
		  ,[PercentageSplit]
		  ,[ESRLastUpdate]
		  ,[EsrAssignID]
	  FROM [dbo].[ESRAssignmentCostings]
	END
ELSE
	BEGIN
		SELECT [ESRCostingAllocationId]
		  ,[ESRPersonId]
		  ,[ESRAssignmentId]
		  ,[EffectiveStartDate]
		  ,[EffectiveEndDate]
		  ,[EntityCode]
		  ,[CharitableIndicator]
		  ,[CostCentre]
		  ,[Subjective]
		  ,[Analysis1]
		  ,[Analysis2]
		  ,[ElementNumber]
		  ,[SpareSegment]
		  ,[PercentageSplit]
		  ,[ESRLastUpdate]
		  ,[EsrAssignID]
	  FROM [dbo].[ESRAssignmentCostings]
	  WHERE  [ESRCostingAllocationId] = @ESRCostingAllocationId
	END
END