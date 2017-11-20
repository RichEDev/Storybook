

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteVehicleJourneyRate] 
	-- Add the parameters for the stored procedure here
	@mileageid int,
	@employeeID INT,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @recordTitle nvarchar(2000);
	select @recordTitle = carsize from mileage_categories where mileageid = @mileageid;

    -- Insert statements for procedure here
	delete from mileage_categories where mileageid = @mileageid

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 51, @mileageid, @recordTitle, null;
END


