CREATE PROCEDURE [dbo].[GetEsrAssignmentLocationById] 
	@esrAssignID int,
	@asOfDate datetime
AS
BEGIN
	
	select top 1
		ESRAssignmentLocationId,
		esrAssignID,
		ESRLocationId,
		StartDate,
		DeletedDateTime
	from
		ESRAssignmentLocations
	where
		esrAssignID = @esrAssignID
		and StartDate <= @asOfDate
		and DeletedDateTime is null
	order by
		StartDate desc

END