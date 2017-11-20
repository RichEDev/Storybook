CREATE PROCEDURE [dbo].[DeleteMileageThresholdRate]
	@MileageThresholdRateId int,
	@CuEmployeeId int,
	@CuDelegateId int
AS

	declare @ElementId int = 51,
			@deleted int,
			@RecordTitle nvarchar(2000);

	select
		@RecordTitle =
			mileage_categories.carsize +
			', Mileage Date Range ' +
			dbo.FormatDateRangeTitle(mileage_dateranges.mileagedateid, mileage_thresholds.mileagethresholdid) +
			', Fuel Rate ' +
			VehicleEngineTypes.Name
	from
		MileageThresholdRates
			join mileage_thresholds on mileage_thresholds.mileagethresholdid = MileageThresholdRates.MileageThresholdId
				join mileage_dateranges on mileage_dateranges.mileagedateid = mileage_thresholds.mileagedateid
					join mileage_categories on mileage_categories.mileageid = mileage_dateranges.mileageid
			join VehicleEngineTypes on VehicleEngineTypes.VehicleEngineTypeId = MileageThresholdRates.VehicleEngineTypeId
	where
		MileageThresholdRates.MileageThresholdRateId = @MileageThresholdRateId;

	delete from
		MileageThresholdRates
	where
		MileageThresholdRateId = @MileageThresholdRateId; 

	set @deleted = @@ROWCOUNT;

	if (@deleted > 0)
		exec addDeleteEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, @MileageThresholdRateId, @RecordTitle, null;
	
return @deleted
