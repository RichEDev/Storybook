CREATE PROCEDURE [dbo].[APIsaveEsrAssignmentLocation]
	@esrAssignID int,
	@ESRLocationId bigint,
	@StartDate datetime,
	@DeletedDateTime datetime
AS
BEGIN

	if EXISTS(select * from ESRAssignmentLocations where esrAssignID = @esrAssignID and DeletedDateTime is null)
		set @StartDate = getdate();

	if EXISTS(select * from ESRAssignmentLocations where esrAssignID = @esrAssignID and ESRLocationId = @ESRLocationId and StartDate = @StartDate and (DeletedDateTime = @DeletedDateTime or (DeletedDateTime is null and @DeletedDateTime is null)))
		return

	declare @currentLocation int
	select top 1
		@currentLocation = ESRLocationId
	from
		ESRAssignmentLocations
	where
		esrAssignID = @esrAssignID
		and StartDate <= @StartDate
		and DeletedDateTime is null
	order by
		StartDate desc

	if @currentLocation is null or @ESRLocationId <> @currentLocation
	begin
		insert into ESRAssignmentLocations
			values
			(
				@esrAssignID,
				@ESRLocationId,
				@StartDate,
				@DeletedDateTime
			)
	end

END