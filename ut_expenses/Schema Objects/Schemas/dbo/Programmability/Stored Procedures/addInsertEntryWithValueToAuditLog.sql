CREATE PROCEDURE [dbo].[addInsertEntryWithValueToAuditLog] (@employeeid int, @delegateID INT, @elementid int, @entityid INT, @recordTitle nvarchar (2000), @subAccountId int, @newValue nvarchar(max)) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @username NVARCHAR(50);
	DECLARE @delegateUsername NVARCHAR(50);
	
	SELECT @username = username FROM employees WHERE employeeid = @employeeid
	IF @delegateID IS NOT NULL
	BEGIN
		SELECT @delegateUsername = username FROM employees WHERE employeeid = @delegateID
	END

    -- Insert statements for procedure here
    if not @subAccountId is null
    begin
		insert into auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, entityID, [action], recordTitle, subAccountId, newvalue) values (@employeeid, @username, @delegateID, @delegateUsername, @elementid, @entityid, 1, @recordTitle, @subAccountId, @newValue);
	end
	else
	begin
		insert into auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, entityID, [action], recordTitle, newvalue) values (@employeeid, @username, @delegateID, @delegateUsername, @elementid, @entityid, 1, @recordTitle, @newValue);
	end
END

