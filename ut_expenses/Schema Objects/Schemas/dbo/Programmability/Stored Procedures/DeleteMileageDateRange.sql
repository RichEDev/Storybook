CREATE PROCEDURE [dbo].[DeleteMileageDateRange]
	@mileagedateid int,
    @employeeID INT,
    @delegateID INT
AS
	SET NOCOUNT ON;

    declare @title1 nvarchar(500);
    declare @recordTitle nvarchar(2000);
    
    select @title1 = carsize from mileage_categories where mileageid = (select mileageid from mileage_dateranges where mileagedateid = @mileagedateid);
    set @recordTitle = (select ISNULL(@title1, '') + ' Mileage Date Range ' + dbo.FormatDateRangeTitle(@mileagedateid, -1));

    DELETE FROM mileage_dateranges WHERE mileagedateid = @mileagedateid;

    exec addDeleteEntryToAuditLog @employeeID, @delegateID, 51, @mileagedateid, @recordTitle, null;

RETURN 0
