create unique index UK_ESRAssignmentLocations ON ESRAssignmentLocations
(
	esrAssignID,
	ESRLocationId,
	StartDate,
	DeletedDateTime
)