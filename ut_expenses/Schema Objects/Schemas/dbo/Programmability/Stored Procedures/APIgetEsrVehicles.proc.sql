CREATE PROCEDURE [dbo].[APIgetEsrVehicles]
	@ESRVehicleAllocationId bigint 
AS
BEGIN
IF @ESRVehicleAllocationId = 0
	BEGIN
	SELECT [ESRVehicleAllocationId]
		  ,[ESRPersonId]
		  ,[ESRAssignmentId]
		  ,[EffectiveStartDate]
		  ,[EffectiveEndDate]
		  ,[RegistrationNumber]
		  ,[Make]
		  ,[Model]
		  ,[Ownership]
		  ,[InitialRegistrationDate]
		  ,[EngineCC]
		  ,[ESRLastUpdate]
		  ,[UserRatesTable]
          ,[FuelType]
          ,[ESRAssignId]
	  FROM [dbo].[ESRVehicles]
	END
ELSE
	BEGIN
	SELECT [ESRVehicleAllocationId]
		  ,[ESRPersonId]
		  ,[ESRAssignmentId]
		  ,[EffectiveStartDate]
		  ,[EffectiveEndDate]
		  ,[RegistrationNumber]
		  ,[Make]
		  ,[Model]
		  ,[Ownership]
		  ,[InitialRegistrationDate]
		  ,[EngineCC]
		  ,[ESRLastUpdate]
		  ,[UserRatesTable]
          ,[FuelType]
          ,[ESRAssignId]	  FROM [dbo].[ESRVehicles]
	  WHERE [dbo].[ESRVehicles].[ESRVehicleAllocationId] = @ESRVehicleAllocationId
	END
END