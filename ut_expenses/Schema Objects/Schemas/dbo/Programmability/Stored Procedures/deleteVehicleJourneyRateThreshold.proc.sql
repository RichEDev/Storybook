CREATE PROCEDURE [dbo].[deleteVehicleJourneyRateThreshold]
    @mileagethresholdid int,
    @employeeID INT,
    @delegateID INT
AS
BEGIN
    SET NOCOUNT ON;

    declare @title1 nvarchar(500);
    declare @recordTitle nvarchar(2000);
    declare @mileagedateid int;
    select @title1 = carsize from mileage_categories where mileageid = (select mileageid from mileage_dateranges where mileagedateid = @mileagedateid);
    select @mileagedateid = mileagedateid from mileage_thresholds WHERE mileagethresholdid = @mileagethresholdid;
    set @recordTitle = (select ISNULL(@title1, '') + ' Mileage Date Range ' + dbo.FormatDateRangeTitle(@mileagedateid, @mileagethresholdid));

    DELETE FROM mileage_thresholds WHERE mileagethresholdid = @mileagethresholdid;

    exec addDeleteEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, @recordTitle, null;
END





