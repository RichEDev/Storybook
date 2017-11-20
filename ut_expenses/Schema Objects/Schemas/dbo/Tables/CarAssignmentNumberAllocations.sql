CREATE TABLE dbo.CarAssignmentNumberAllocations
(
	ESRVehicleAllocationId bigint not null,
	ESRAssignId int not null,
	CarId int not null,
	Archived bit not null
)