CREATE PROCEDURE  [dbo].[APIdeleteCarAssignmentNumberAllocation]
	@ESRVehicleAllocationId bigint
AS
	DELETE FROM CarAssignmentNumberAllocations WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId;
