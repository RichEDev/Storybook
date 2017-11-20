CREATE PROCEDURE [dbo].[getEsrAssignmentsEmployeeId]
	@assignmentNum nvarchar(30) = null,
	@employeeNum nvarchar(8) = null
AS

	select top 1
		employeeid
	from
		esr_assignments
	where
		(
			@assignmentNum is null
			or AssignmentNumber = @assignmentNum
		)
		and
		(
			@employeeNum is null
			or left(AssignmentNumber, 8) = @employeeNum
		)
	order by
		esrAssignID desc

RETURN 0
