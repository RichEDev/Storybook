
CREATE PROCEDURE [dbo].[APIgetEsrVehiclesByEsrId]
 @EsrId bigint
AS
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
        ,[ESRAssignId]   FROM [dbo].[ESRVehicles]
 WHERE [dbo].[ESRVehicles].[ESRPersonId] = @EsrId
END