create clustered index IX_ESRAssignmentLocations ON ESRAssignmentLocations
(
	esrAssignID,
	StartDate desc
)