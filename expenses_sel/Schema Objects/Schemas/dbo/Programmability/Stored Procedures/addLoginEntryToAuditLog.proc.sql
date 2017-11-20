CREATE PROCEDURE [dbo].[addLoginEntryToAuditLog] (@employeeid int, @delegateID INT, @elementid int, @entityid INT, @recordTitle nvarchar (2000), @subAccountId int) 	
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
	insert into auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, entityID, [action], recordTitle, subAccountId) values (@employeeid, @username, @delegateID, @delegateUsername, @elementid, @entityid, 4, @recordTitle, @subAccountId);
END
