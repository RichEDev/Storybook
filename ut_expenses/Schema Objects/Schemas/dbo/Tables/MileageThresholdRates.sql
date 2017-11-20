create table dbo.MileageThresholdRates
(
	MileageThresholdRateId int not null identity,
	CreatedBy int not null,
	CreatedOn datetime not null,
	ModifiedBy int,
	ModifiedOn datetime,
	MileageThresholdId int not null,
	VehicleEngineTypeId int not null,
	RatePerUnit money not null,
	AmountForVat money not null
);
