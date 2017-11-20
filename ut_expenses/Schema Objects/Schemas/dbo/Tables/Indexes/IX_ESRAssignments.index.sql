CREATE NONCLUSTERED INDEX IX_ESRAssignments
	ON [dbo].[esr_assignments] ([Active])
	INCLUDE ([esrAssignID],[employeeid],[FinalAssignmentEndDate])
