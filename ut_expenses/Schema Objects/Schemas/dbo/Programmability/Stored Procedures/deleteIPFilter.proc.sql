CREATE PROCEDURE [dbo].[deleteIPFilter] (@ipfilterid INT, @userid int, @CUemployeeID INT, @CUdelegateID INT)	
AS
BEGIN
	DECLARE @ipaddress NVARCHAR(50);
	DECLARE @returnVal INT;
	SELECT @ipaddress = ipAddress FROM ipfilters WHERE ipFilterID = @ipfilterid;
				
	DELETE FROM ipfilters WHERE ipFilterID = @ipfilterid;
	SET @returnVal = @@ROWCOUNT;	
		
	EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 162, @ipfilterid, @ipaddress, null
	
	RETURN @returnVal;
END
