




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteSubcatVatRate] (@vatrateid int, @employeeID INT, @delegateID INT)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @title1 nvarchar(500);
	declare @recordTitle nvarchar(2000);

	select @title1 = subcat from subcats where subcatid = (select subcatid from subcat_vat_rates where @vatrateid = vatrateid);
	set @recordTitle = (select @title1 + ' vat rate ' + CAST(@vatrateid AS nvarchar(20)));

    -- Insert statements for procedure here
	delete from subcat_vat_rates where vatrateid = @vatrateid;

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, @recordTitle, null;
END




