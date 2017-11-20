
CREATE PROCEDURE [dbo].[RecordDelegateLogon] (@employeeID int, @delegateID int, @delegateType int, @subAccountID int) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @username NVARCHAR(50);
	DECLARE @delegateUsername NVARCHAR(50);
	DECLARE @recordTitle NVARCHAR(4000);
	
	SET @delegateUsername = 'Error - delegateID not set during delegate logon';
	
	SELECT @username = username FROM employees WHERE employeeid = @employeeID
	IF @delegateID IS NOT NULL
	BEGIN
		SELECT @delegateUsername = username FROM employees WHERE employeeid = @delegateID
	END
	
	SET @recordTitle = (select 'Employee (' + @delegateUsername + ') logged in as employee (' + @username + ') using delegate type (' + cast(@delegateType as nvarchar(2)) + ')');
	
	
	exec addLoginEntryToAuditLog @employeeID, @delegateID, 25, @delegateID, @recordTitle, @subAccountID
	
	return 0;
END
