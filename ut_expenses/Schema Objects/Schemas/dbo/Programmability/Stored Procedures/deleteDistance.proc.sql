

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteDistance]
	-- Add the parameters for the stored procedure here
	@distanceid int,
	@employeeID INT,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @locationa varchar(250);
	declare @locationb varchar(250);
	declare @recordTitle varchar(2000);
	select @locationa = company from companies where companyid = (select locationa from location_distances where distanceid = @distanceid);
	select @locationb = company from companies where companyid = (select locationb from location_distances where distanceid = @distanceid);
	set @recordTitle = (select @locationa + ' to ' + @locationb);

    -- Insert statements for procedure here
	delete from location_distances where distanceid = @distanceid;
	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 38, @distanceid, @recordTitle, null;
END
