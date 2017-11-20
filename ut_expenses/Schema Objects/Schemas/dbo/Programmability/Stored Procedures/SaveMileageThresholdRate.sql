CREATE PROCEDURE dbo.SaveMileageThresholdRate
	@MileageThresholdRateId int,
	@CuEmployeeId int,
	@CuDelegateId int,
	@MileageThresholdId int,
	@VehicleEngineTypeId int,
	@RatePerUnit money,
	@AmountForVat money
AS

	if exists (select MileageThresholdRateId from MileageThresholdRates where MileageThresholdId = @MileageThresholdId and VehicleEngineTypeId = @VehicleEngineTypeId and (MileageThresholdRateId <> @MileageThresholdRateId or @MileageThresholdRateId is null))
		return -1 -- MileageThreshold+VehicleEngineType already exists
		
	declare @ElementId int = 51,
			@RecordTitle nvarchar(2000);

	select
		@RecordTitle = 
			mileage_categories.carsize +
			', Mileage Date Range ' +
			dbo.FormatDateRangeTitle(mileage_dateranges.mileagedateid, mileage_thresholds.mileagethresholdid) +
			', Fuel Rate {{VehicleEngineTypes.Name}}'
	from
		mileage_thresholds
			join mileage_dateranges on mileage_dateranges.mileagedateid = mileage_thresholds.mileagedateid
				join mileage_categories on mileage_categories.mileageid = mileage_dateranges.mileageid
	where
		mileage_thresholds.mileagethresholdid = @MileageThresholdId;

	if exists (select MileageThresholdRateId from MileageThresholdRates where MileageThresholdRateId = @MileageThresholdRateId)
	begin
		declare
			@PrevVehicleEngineTypeId int,
			@PrevVehicleEngineTypeName nvarchar(450),
			@PrevRatePerUnit money,
			@PrevAmountForVat money

		select
			@PrevVehicleEngineTypeId = MileageThresholdRates.VehicleEngineTypeId,
			@PrevVehicleEngineTypeName = VehicleEngineTypes.Name,
			@PrevRatePerUnit = MileageThresholdRates.RatePerUnit,
			@PrevAmountForVat = MileageThresholdRates.AmountForVat,
			@RecordTitle = REPLACE(@RecordTitle, '{{VehicleEngineTypes.Name}}', VehicleEngineTypes.Name)
		from
			MileageThresholdRates
				left join VehicleEngineTypes on VehicleEngineTypes.VehicleEngineTypeId = MileageThresholdRates.VehicleEngineTypeId
		where
			MileageThresholdRates.MileageThresholdRateId = @MileageThresholdRateId;

		update
			MileageThresholdRates
		set
			ModifiedBy = @CuEmployeeId,
			ModifiedOn = GETUTCDATE(),
			VehicleEngineTypeId = @VehicleEngineTypeId,
			RatePerUnit = @RatePerUnit,
			AmountForVat = @AmountForVat
		where
			MileageThresholdRateId = @MileageThresholdRateId;
			
		if NOT EXISTS(SELECT @PrevVehicleEngineTypeId INTERSECT SELECT @VehicleEngineTypeId) -- IF x IS DISTINCT FROM y, see http://stackoverflow.com/questions/10416789/
		begin
			-- log the engine type Name in addition to its Id
			declare @VehicleEngineTypeName nvarchar(450);
			select	@VehicleEngineTypeName = Name
			from	VehicleEngineTypes
			where	VehicleEngineTypeId = @VehicleEngineTypeId;
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, '4135F171-2037-41ED-B47A-76B1A45667DC', @PrevVehicleEngineTypeName, @VehicleEngineTypeName, @RecordTitle, null;
		end
		if NOT EXISTS(SELECT @PrevRatePerUnit INTERSECT SELECT @RatePerUnit)
		begin
			-- if we don't convert the money values to string here we lose precision
			declare	@PrevRatePerUnitAsString	varchar(max) = convert(varchar, @PrevRatePerUnit, 2),
					@RatePerUnitAsString		varchar(max) = convert(varchar, @RatePerUnit, 2);
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, 'D863B98C-BDBB-49D1-BCC6-E966F843D994', @PrevRatePerUnitAsString, @RatePerUnitAsString, @RecordTitle, null;
		end
		if NOT EXISTS(SELECT @PrevAmountForVat INTERSECT SELECT @AmountForVat)
		begin
			-- if we don't convert the money values to string here we lose precision
			declare	@PrevAmountForVatAsString	varchar(max) = convert(varchar, @PrevAmountForVat, 2),
					@AmountForVatAsString		varchar(max) = convert(varchar, @AmountForVat, 2);
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, 'B6D354B8-9D28-4C30-B883-99C937EFA9AC', @PrevAmountForVatAsString, @AmountForVatAsString, @RecordTitle, null;
		end

	end
	else
	begin
		insert MileageThresholdRates
			(CreatedBy, CreatedOn, MileageThresholdId, VehicleEngineTypeId, RatePerUnit, AmountForVat)
		values
			(@CuEmployeeId, GETUTCDATE(), @MileageThresholdId, @VehicleEngineTypeId, @RatePerUnit, @AmountForVat);
			
		set @MileageThresholdRateId = @@IDENTITY;

		select @RecordTitle = REPLACE(@RecordTitle, '{{VehicleEngineTypes.Name}}', Name)
		from VehicleEngineTypes
		where VehicleEngineTypeId = @VehicleEngineTypeId;

		exec addInsertEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, @RecordTitle, null;

	end

return @MileageThresholdRateId
