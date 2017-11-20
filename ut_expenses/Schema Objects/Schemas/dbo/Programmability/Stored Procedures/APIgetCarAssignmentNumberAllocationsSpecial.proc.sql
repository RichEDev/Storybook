CREATE PROCEDURE [dbo].[APIgetCarAssignmentNumberAllocationsSpecial] 
 @reference nvarchar(100)
 
AS
 BEGIN
  select ESRVehicleAllocationId, ESRAssignId, CarId, archived from CarAssignmentNumberAllocations WHERE CarId = @reference;
 END
  
RETURN 0