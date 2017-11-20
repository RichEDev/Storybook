CREATE PROCEDURE [dbo].[saveVehicleJourneyRateDateRange]
    @mileagedateid int,
    @mileageid int,
    @daterangetype tinyint,
    @datevalue1 DateTime,
    @datevalue2 DateTime,
    @userid int,
    @date DateTime,
    @employeeID INT,
    @delegateID INT
AS
BEGIN
    SET NOCOUNT ON;

    declare @title1 nvarchar(500);
    declare @recordTitle nvarchar(2000);
    select @title1 = carsize from mileage_categories where mileageid = @mileageid;


    if @mileagedateid = 0
        begin
            insert into mileage_dateranges (mileageid, daterangetype, datevalue1, datevalue2, createdon, createdby) values (@mileageid, @daterangetype, @datevalue1, @datevalue2, @date, @userid);
            set @mileagedateid = scope_identity();
            set @recordTitle = (select @title1 + ' Mileage Date Range ' + dbo.FormatDateRangeTitle(@mileagedateid, -1));
            update mileage_categories set catvalid = dbo.VehicleJourneyRateIsValid(@mileageid) where mileageid = @mileageid;
            
            exec addInsertEntryToAuditLog @employeeID, @delegateID, 51, @mileagedateid, @recordTitle, null;
        end
    else
        begin
            declare @olddaterangetype tinyint;
            declare @olddatevalue1 datetime;
            declare @olddatevalue2 datetime;
            select @olddaterangetype = daterangetype, @olddatevalue1 = datevalue1, @olddatevalue2 = datevalue2 from mileage_dateranges where mileagedateid = @mileagedateid;
            set @recordTitle = (select @title1 + ' Mileage Date Range ' + dbo.FormatDateRangeTitle(@mileagedateid, -1));
            update mileage_dateranges set daterangetype = @daterangetype, datevalue1 = @datevalue1, datevalue2 = @datevalue2, modifiedon = @date, modifiedby = @userid where mileagedateid = @mileagedateid;

            update mileage_categories set catvalid = dbo.VehicleJourneyRateIsValid(@mileageid) where mileageid = @mileageid;
            
            if @olddaterangetype <> @daterangetype
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagedateid, '09d9a31a-7554-49f6-9f39-5a4e6383d72e', @olddaterangetype, @daterangetype, @recordtitle, null;
            if @olddatevalue1 <> @datevalue1
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagedateid, '1c5f7671-808c-4a71-b494-a83bde1ace09', @olddatevalue1, @datevalue1, @recordtitle, null;
            if @olddatevalue2 <> @datevalue2
                exec addUpdateEntryToAuditLog @employeeID, @delegateID, 51, @mileagedateid, 'eaf0b711-24d7-40d3-a10f-fdec028fc543', @olddatevalue2, @datevalue2, @recordtitle, null;
        end
    return @mileagedateid;
END

