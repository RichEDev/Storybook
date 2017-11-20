CREATE PROCEDURE [dbo].[saveVehicleJourneyRateTreshold]
    @mileagethresholdid int,
    @mileagedateid int,
    @rangetype tinyint,
    @rangevalue1 decimal(18, 2),
    @rangevalue2 decimal(18, 2),
    @passenger1 money,
    @passengerx money,
    @heavyBulkyEquipment money,
    @date DateTime,
    @userid int,
    @employeeID INT,
    @delegateID INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    declare @title1 nvarchar(500);
    declare @recordTitle nvarchar(2000);
    declare @mileageid int;
    
    select @title1 = carsize from mileage_categories where mileageid = (select mileageid from mileage_dateranges where mileagedateid = @mileagedateid);

    if @mileagethresholdid = 0
        begin
			insert mileage_thresholds
			(
				mileagedateid,
				rangetype,
				rangevalue1,
				rangevalue2,
				passenger1,
				passengerx,
				createdon,
				createdby,
				heavyBulkyEquipment
			)
			VALUES
			(
				@mileagedateid,
				@rangetype,
				@rangevalue1,
				@rangevalue2,
				@passenger1,
				@passengerx,
				@date,
				@userid,
				@heavyBulkyEquipment
			);

            set @mileagethresholdid = scope_identity();

            set @mileageid = (select mileageid from mileage_dateranges where mileagedateid = @mileagedateid);
            set @recordTitle = (select @title1 + ' Mileage Date Range ' + dbo.FormatDateRangeTitle(@mileagedateid, @mileagethresholdid));            
            update mileage_categories set catvalid = dbo.VehicleJourneyRateIsValid(@mileageid) where mileageid = @mileageid;

            
            exec addInsertEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, @recordTitle, null;
        end
    else
        begin
            declare @oldrangetype tinyint;
            declare @oldrangevalue1 decimal(18, 2);
            declare @oldrangevalue2 decimal(18, 2);
            declare @oldpassenger1 money;
            declare @oldpassengerx money;
            declare @oldheavyBulkyEquipment money;

			select
				@oldrangetype = rangetype,
				@oldrangevalue1 = rangevalue1,
				@oldrangevalue2 = rangevalue2,
				@oldpassenger1 = passenger1,
				@oldpassengerx = passengerx,
				@oldheavyBulkyEquipment = heavyBulkyEquipment
			from
				mileage_thresholds
			where
				mileagethresholdid = @mileagethresholdid;
            
			update
				mileage_thresholds
			set
				rangetype = @rangetype,
				rangevalue1 = @rangevalue1,
				rangevalue2 = @rangevalue2,
				passenger1 = @passenger1,
				passengerx = @passengerx,
				modifiedon = @date,
				modifiedby = @userid,
				heavyBulkyEquipment = @heavyBulkyEquipment
			where
				mileagethresholdid = @mileagethresholdid;

            set @mileageid = (select mileageid from mileage_dateranges where mileagedateid = @mileagedateid);
            
            set @recordTitle = (select @title1 + ' Mileage Date Range ' + dbo.FormatDateRangeTitle(@mileagedateid, @mileagethresholdid));
                        
            update mileage_categories set catvalid = dbo.VehicleJourneyRateIsValid(@mileageid) where mileageid = @mileageid;

            if @oldrangetype <> @rangetype
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, '2d17a97f-4533-45e3-be56-db8b605ec401', @oldrangetype, @rangetype, @recordtitle, null;
            if @oldrangevalue1 <> @rangevalue1
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, 'ceed8e88-7dbb-4268-8fdc-47bd8a6af415', @oldrangevalue1, @rangevalue1, @recordtitle, null;
            if @oldrangevalue2 <> @rangevalue2
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, 'f8b2bcc4-50e8-424e-86a4-d9f1176f6fad', @oldrangevalue2, @rangevalue2, @recordtitle, null;
            if @oldpassenger1 <> @passenger1
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, '44386ea3-5486-4916-9f44-fc572feee378', @oldpassenger1, @passenger1, @recordtitle, null;
            if @oldpassengerx <> @passengerx
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, '1f1e383c-219f-4498-9d42-f39f36edc374', @oldpassengerx, @passengerx, @recordtitle, null;
            if @oldheavyBulkyEquipment <> @heavyBulkyEquipment
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, 'be330f92-e3c6-42d1-9825-b6567c8adc22', @oldheavyBulkyEquipment, @heavyBulkyEquipment, @recordtitle, null;
        end

    return @mileagethresholdid;
END

