alter table dbo.MileageThresholdRates add constraint
	FK_MileageThresholdRates_mileage_threshold foreign key (MileageThresholdId)
	references dbo.mileage_thresholds (mileagethresholdid)
	on delete cascade;
