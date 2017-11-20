create table ESRAssignmentLocations
(
	ESRAssignmentLocationId int identity,
	esrAssignID int not null,
	ESRLocationId bigint not null,
	StartDate datetime not null,
	DeletedDateTime datetime
)