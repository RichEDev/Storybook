


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteVehicleJourneyRateThreshold]
	-- Add the parameters for the stored procedure here
	@mileagethresholdid int,
	@employeeID INT,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @title1 nvarchar(500);
	declare @recordTitle nvarchar(2000);
	declare @mileagedateid int;
	select @title1 = carsize from mileage_categories where mileageid = (select mileageid from mileage_dateranges where mileagedateid = @mileagedateid);
	select @mileagedateid = mileagedateid from mileage_thresholds WHERE mileagethresholdid = @mileagethresholdid;

    -- Insert statements for procedure here
	DELETE FROM mileage_thresholds WHERE mileagethresholdid = @mileagethresholdid;

	set @recordTitle = (select @title1 + ' Mileage Date Range ' + CAST(@mileagedateid AS nvarchar(20)) + ' Threshold ' + CAST(@mileagethresholdid AS nvarchar(20)));
	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 51, @mileagethresholdid, @recordTitle, null;
END



