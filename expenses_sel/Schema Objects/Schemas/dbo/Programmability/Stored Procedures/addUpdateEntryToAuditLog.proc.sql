CREATE PROCEDURE [dbo].[addUpdateEntryToAuditLog] (@employeeID INT, @delegateID INT, @elementID int, @entityid INT, @field uniqueidentifier, @oldvalue nvarchar(4000), @newvalue nvarchar(4000), @recordtitle NVARCHAR(2000), @subAccountId int) 
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
		insert into auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, entityID, [action], field, oldvalue, newvalue, recordtitle, subAccountId) values (@employeeID, @username, @delegateID, @delegateUsername, @elementid, @entityid, 2, @field, @oldvalue, @newvalue, @recordtitle, @subAccountId);
	end
	else
	begin
		insert into auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, entityID, [action], field, oldvalue, newvalue, recordtitle) values (@employeeID, @username, @delegateID, @delegateUsername, @elementid, @entityid, 2, @field, @oldvalue, @newvalue, @recordtitle);
	end
END
