CREATE PROCEDURE [dbo].[AddViewEntryToAuditLog] (@employeeID INT, @delegateID INT, @elementid INT, @recordTitle nvarchar (2000), @subAccountId int) 	
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
	END
    -- Insert statements for procedure here
    IF NOT @subAccountId IS NULL
    BEGIN	
		INSERT INTO auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, [action], recordtitle, subAccountId) values (@employeeID, @username, @delegateID, @delegateUsername, @elementID, 6, @recordTitle, @subAccountId);
	END
	ELSE
	BEGIN
		INSERT INTO auditLog (employeeID, employeeUsername, delegateID, delegateUsername, elementID, [action], recordtitle) values (@employeeID, @username, @delegateID, @delegateUsername, @elementID, 6, @recordTitle);
	END
END
