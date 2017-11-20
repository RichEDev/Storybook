alter table dbo.MileageThresholdRates add constraint
	FK_MileageThresholdRates_VehicleEngineTypes foreign key (VehicleEngineTypeId)
	references dbo.VehicleEngineTypes (VehicleEngineTypeId)
	on delete cascade;
