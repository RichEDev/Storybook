
CREATE PROCEDURE [dbo].[addDeleteEntryWithValueToAuditLog] (@employeeID INT, @delegateID INT, @elementid INT, @entityid INT, @recordTitle nvarchar (2000), @origValue nvarchar(max), @subAccountId int)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @username NVARCHAR(50)
	DECLARE @delegateUsername NVARCHAR(50)
	SELECT @username = username FROM employees WHERE employeeid = @employeeid;
	
	IF @delegateID IS NOT NULL
	BEGIN
		SELECT @delegateUsername = username FROM employees WHERE employeeid = @delegateID
	end
	-- Insert statements for procedure here
	if not @subAccountId is null
	begin
		insert into auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, entityID, [action], recordtitle, oldvalue, subAccountId) values (@employeeID, @username, @delegateID, @delegateUsername, @elementID, @entityID, 3, @recordTitle, @origValue, @subAccountId);
	end
	else
	begin
		insert into auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, entityID, [action], recordtitle, oldvalue) values (@employeeID, @username, @delegateID, @delegateUsername, @elementID, @entityID, 3, @recordTitle, @origValue);
	end
END
