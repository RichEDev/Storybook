CREATE PROCEDURE [dbo].[deleteProjectcode] 
@projectcodeid INT, 
@employeeid INT,
@CUemployeeID INT,
@CUdelegateID INT
AS
 
BEGIN
	SET NOCOUNT ON;
	DECLARE @ReturnCode INT;
	SET @ReturnCode = 0;
	
	-- Check to see if project code is in use
	DECLARE @count INT;
	SET @count = (SELECT COUNT (projectcodeid) FROM employee_costcodes WHERE projectcodeid = @projectcodeid);
	IF @count > 0
	SET @ReturnCode = -2; 

	IF @ReturnCode = 0
	BEGIN
		SET @count = (SELECT COUNT (projectcodeid) FROM savedexpenses_costcodes WHERE projectcodeid = @projectcodeid);
		IF @count > 0
		SET @ReturnCode = -4;
	END
	 
	IF @ReturnCode = 0
	BEGIN
		DECLARE @username NVARCHAR(50);
		SELECT @username = username FROM employees WHERE employeeid = @employeeid;
		
		DECLARE @projectcode NVARCHAR(50);
		SELECT @projectcode = projectcode FROM project_codes WHERE projectcodeid = @projectcodeid;
		
		DELETE FROM project_codes WHERE projectcodeid = @projectcodeid;
		
		EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 3, @projectcodeid, @projectcode, null;
	END
	 
	RETURN @ReturnCode;
END
