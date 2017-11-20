create table dbo.VehicleEngineTypes
(
	VehicleEngineTypeId int not null identity,
	CreatedBy int not null,
	CreatedOn datetime not null,
	ModifiedBy int,
	ModifiedOn datetime,
	Name nvarchar(450) not null,
	Code nvarchar(50)
);
