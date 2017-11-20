CREATE PROCEDURE [dbo].[getEsrAssignmentsCount]
	@assignmentnumber nvarchar(30) = null,
	@employeeid int = null,
	@employeeNum nvarchar(8) = null
AS
	select
		count(*)
	from
		esr_assignments
	where
		(
			@assignmentnumber is null
			or assignmentnumber = @assignmentnumber
		)
		and
		(
			@employeeid is null
			or employeeid = @employeeid
		)
		and
		(
			@employeeNum is null
			or left(AssignmentNumber, 8) = @employeeNum
		)

RETURN 0
