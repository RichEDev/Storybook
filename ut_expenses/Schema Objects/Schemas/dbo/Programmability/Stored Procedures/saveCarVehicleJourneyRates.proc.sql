CREATE PROCEDURE [dbo].[saveCarVehicleJourneyRates]
	@carID INT,
	@CUemployeeID INT,
	@CUdelegateID INT,
	@carJourneyRates IntPK READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @auditValue nvarchar(MAX);
	declare @carName nvarchar(MAX);
	declare @recordTitle nvarchar(MAX);
	declare @newCarVJRs IntPK;
	declare @removeCarVJRs IntPK;
	declare @mileageID int;
	
	set @auditValue = '';
	set @carName = '';
	set @recordTitle = '';
	
	set @carName = (select make + ' ' + model + ', ' + registration from cars where carid = @carID);
	set @recordTitle = 'Vehicle Journey Rate for ' + @carName;
	
	insert into @removeCarVJRs (c1) (select mileageID from car_mileagecats where carID = @carID);
	
	declare vjr_cursor cursor for
	select c1 from @carJourneyRates
	open vjr_cursor
	fetch next from vjr_cursor into @mileageID
	while @@FETCH_STATUS = 0
	begin
		if not exists (select * from car_mileagecats where carID = @carID and mileageID = @mileageID)
		begin
			-- must be adding a mileage category
			insert into @newCarVJRs values (@mileageID);
		end
		
		-- exclude from the removal list as it is still selected
		delete from @removeCarVJRs where c1 = @mileageID;
		
		fetch next from vjr_cursor into @mileageID
	end
	close vjr_cursor;
	deallocate vjr_cursor;
	
	-- loop through the additions
	declare vjr_cursor cursor for
	select c1 from @newCarVJRs
	open vjr_cursor
	fetch next from vjr_cursor into @mileageID
	while @@FETCH_STATUS = 0
	begin
		set @auditValue = (select carsize from mileage_categories where mileageid = @mileageID);

		insert into car_mileagecats (carid, mileageid) values (@carID, @mileageID);
	            
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carID, NULL, '', @auditValue, @recordTitle, NULL;
		
		fetch next from vjr_cursor into @mileageID
	end
	close vjr_cursor;
	deallocate vjr_cursor;
	
	-- loop through the deletions
	declare vjr_cursor cursor for
	select c1 from @removeCarVJRs
	open vjr_cursor
	fetch next from vjr_cursor into @mileageID
	while @@FETCH_STATUS = 0
	begin
		set @auditValue = (select carsize from mileage_categories where mileageid = @mileageID);
		
		delete from car_mileagecats where carid = @carID and mileageID = @mileageID;

		exec addDeleteEntryWithValueToAuditLog @CUemployeeID, @CUdelegateID, 13, @carID, @recordTitle, @auditValue, NULL;
		
		fetch next from vjr_cursor into @mileageID
	end
	close vjr_cursor;
	deallocate vjr_cursor;

	return 0;
END
