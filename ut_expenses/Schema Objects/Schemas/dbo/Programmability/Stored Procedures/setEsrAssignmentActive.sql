CREATE PROCEDURE [dbo].[setEsrAssignmentActive]
	@active bit,
	@assignmentID bigint = null,
	@recId int = null
AS
	update
		esr_assignments
	set
		active = @active
	where
		(
			@assignmentID is null
			or assignmentID = @assignmentID
		)
		and
		(
			@recId is null
			or esrAssignID = @recId
		)

RETURN 0
