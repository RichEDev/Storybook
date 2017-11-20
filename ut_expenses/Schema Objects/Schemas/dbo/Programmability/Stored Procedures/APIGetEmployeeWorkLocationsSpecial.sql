CREATE PROCEDURE [dbo].[APIGetEmployeeWorkLocationsSpecial]
	@reference nvarchar(100)
AS
BEGIN
	BEGIN
		SELECT [dbo].[WorkLocations].[employeeLocationID]
			,[dbo].[WorkLocations].[employeeID]
			,[dbo].[WorkLocations].[locationID]
			,[dbo].[WorkLocations].[startDate]
			,[dbo].[WorkLocations].[endDate]
			,[dbo].[WorkLocations].[active]
			,[dbo].[WorkLocations].[temporary]
			,[dbo].[WorkLocations].[CreatedOn]
			,[dbo].[WorkLocations].[CreatedBy]
			,[dbo].[WorkLocations].[ModifiedOn]
			,[dbo].[WorkLocations].[ModifiedBy]
		FROM [dbo].[WorkLocations]
		inner join esr_assignments on esr_assignments.employeeid = WorkLocations.employeeID and esr_assignments.ESRLocationId = WorkLocations.esrlocationID 
		  and esr_assignments.EffectiveStartDate = WorkLocations.startDate
		WHERE [dbo].[esr_assignments].[AssignmentNumber] = @reference
	END
END