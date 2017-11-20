CREATE PROCEDURE [dbo].[APIgetCarAssignmentNumberAllocations]	
	@ESRVehicleAllocationId bigint = 0
	
AS
IF @ESRVehicleAllocationId = 0
	BEGIN
		select ESRVehicleAllocationId, ESRAssignId, CarId, archived from CarAssignmentNumberAllocations
	END
ELSE
	BEGIN
		select ESRVehicleAllocationId, ESRAssignId, CarId, archived from CarAssignmentNumberAllocations WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId
	END
		
RETURN 0
