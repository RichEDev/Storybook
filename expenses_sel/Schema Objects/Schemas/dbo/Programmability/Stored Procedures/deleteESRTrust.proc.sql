
CREATE PROCEDURE [dbo].[deleteESRTrust]
	@trustID int,
	@employeeID INT,
	@delegateID INT
AS 
	declare @recordTitle nvarchar(2000);
	declare @count int

	--Check if the trust is associated to a financial export
	set @count = (select count(*) from financial_exports where NHSTrustID = @trustID)
	if @count > 0
		return 1;
		
	select @recordTitle = trustname from esrtrusts where trustID=@trustID;

	DELETE FROM esrTrusts WHERE trustID=@trustID;

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 27, @trustID, @recordTitle, null;
	return 0;
