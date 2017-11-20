CREATE PROCEDURE [dbo].[GetEsrAssignmentLocationByESRAssignmentLocationId] 
	@ESRAssignmentLocationId int
AS
BEGIN
	
	select
		ESRAssignmentLocationId,
		esrAssignID,
		ESRLocationId,
		StartDate,
		DeletedDateTime
	from
		ESRAssignmentLocations
	where
		ESRAssignmentLocationId = @ESRAssignmentLocationId

END