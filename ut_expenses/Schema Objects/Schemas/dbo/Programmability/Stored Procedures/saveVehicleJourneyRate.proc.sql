CREATE PROCEDURE [dbo].[saveVehicleJourneyRate]
	@mileageid int,
	@carsize nvarchar(50),
	@comment nvarchar(4000),
	@calcmilestotal bit,
	@thresholdtype tinyint,
	@unit tinyint,
	@currencyid int,
	@date DateTime,
	@userid int,
	@userRateTable nvarchar(80),
	@userRateFromEngine int,
	@userRateToEngine int,
	@employeeID INT,
	@delegateID INT,
	@FinancialYearID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @count int

	if @mileageid = 0
		begin
			set @count = (select count(*) from mileage_categories where carsize = @carsize);
			if @count > 0
				return -1;
			
			insert into mileage_categories (carsize, comment, thresholdtype, calcmilestotal, unit, currencyid, UserRatesTable, UserRatesFromEngineSize, UserRatesToEngineSize, createdon, createdby, FinancialYearID) 
				values (@carsize,@comment, @thresholdtype, @calcmilestotal, @unit, @currencyid, @userRateTable, @userRateFromEngine, @userRateToEngine,  @date, @userid, @FinancialYearID);
			set @mileageid = scope_identity();
			
			exec addInsertEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, @carsize, null;
		end
	else
		begin
			set @count = (select count(*) from mileage_categories where carsize = @carsize and mileageid <> @mileageid);
			if @count > 0
				return -1;

			declare @oldcarsize nvarchar(50);
			declare @oldcomment nvarchar(4000);
			declare @oldcalcmilestotal bit;
			declare @oldthresholdtype tinyint;
			declare @oldunit tinyint;
			declare @oldcurrencyid int;
			declare @oldUserRatesTable nvarchar(80);
			declare @oldUserRateFromEngine int;
			declare @oldUserRateToEngine int;
			select @oldcarsize = carsize, @oldcomment = comment, @oldcalcmilestotal = calcmilestotal, @oldthresholdtype = thresholdtype, @oldunit = unit, @oldcurrencyid = currencyid, @oldUserRatesTable = userratestable, @oldUserRateFromEngine = userRatesFromEngineSize, @oldUserRateToEngine = userRatesToEngineSize    from mileage_categories where mileageid = @mileageid;
			
			update mileage_categories 
				set carsize = @carsize, comment = @comment, 
					thresholdtype = @thresholdtype, 
					calcmilestotal = @calcmilestotal, 
					unit = @unit, currencyid = @currencyid, 
					catvalid = dbo.VehicleJourneyRateIsValid(@mileageid), 
					UserRatesTable = @userRateTable,
					UserRatesFromEngineSize = @userRateFromEngine,
					UserRatesToEngineSize = @userRateToEngine,
					modifiedon = @date, 
					modifiedby = @userid ,
					FinancialYearID = @FinancialYearID
				where mileageid = @mileageid;

			if @oldcarsize <> @carsize
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, '0d57533c-9723-4923-87c7-bf8d58a97046', @oldcarsize, @carsize, @carsize, null;
			if @oldcomment <> @comment
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, '7de68cb6-082f-4ec1-a378-75157bd3cb73', @oldcomment, @comment, @carsize, null;
			if @oldcalcmilestotal <> @calcmilestotal
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, '479d4914-48b9-4663-86c4-30f569c7ff81', @oldcalcmilestotal, @calcmilestotal, @carsize, null;
			if @oldthresholdtype <> @thresholdtype
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, '319df682-0bda-4c64-93ec-cb86109d8e3b', @oldthresholdtype, @thresholdtype, @carsize, null;
			if @oldunit <> @unit
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, '18214da3-675f-47f4-b37c-906f9d76fe80', @oldunit, @unit, @carsize, null;
			if @oldcurrencyid <> @currencyid
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, '00504588-4cb7-4a42-8cf0-0ad561369509', @oldcurrencyid, @currencyid, @carsize, null;
			if ISNULL(@oldUserRatesTable, '') <> @userRateTable
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, 'B08ED590-418B-4078-B75C-BB26B4E3BBAD', @oldUserRatesTable, @userRateTable, @carsize, null;
			if @oldUserRateFromEngine <> @userRateFromEngine
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, '7CA8DCA3-9B79-4CD0-A421-A85BBCB24831', @oldUserRateFromEngine, @userRateFromEngine, @carsize, null;
			if @oldUserRateToEngine <> @userRateToEngine
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, 'DEDC8350-295A-4EA4-97C4-1AE7E81E522E', @oldUserRateToEngine, @userRateToEngine, @carsize, null;

		end

	return @mileageid
END


