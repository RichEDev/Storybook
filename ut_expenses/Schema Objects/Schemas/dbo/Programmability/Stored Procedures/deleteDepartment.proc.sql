 
CREATE PROCEDURE [dbo].[deleteDepartment] (
@departmentid INT,
@userid INT,
@CUemployeeID INT,
@CUdelegateID INT) 
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @retCode INT = 0;
	DECLARE @count INT;
	SET @count = (SELECT COUNT (departmentid) FROM employee_costcodes WHERE departmentid = @departmentid);
	
	IF @count > 0
		RETURN -2;
	
	SET @count = (SELECT COUNT (departmentid) FROM savedexpenses_costcodes WHERE departmentid = @departmentid);
	IF @count > 0
		RETURN -4;
	 
	DECLARE @tableid UNIQUEIDENTIFIER = (SELECT tableid FROM tables WHERE tablename = 'departments'); 
	IF EXISTS (
		SELECT TOP 1 dbo.fieldFilters.value
		FROM dbo.fieldFilters INNER JOIN
		dbo.customEntityAttributes ON dbo.fieldFilters.attributeid = dbo.customEntityAttributes.attributeid
		WHERE (dbo.customEntityAttributes.relatedtable = @tableid) AND (dbo.fieldFilters.value = CAST(@departmentid AS NVARCHAR(150)))
	)
		RETURN -6;
	 
	EXEC @retCode = dbo.checkReferencedBy @tableid, @departmentid;
	
	IF @retCode = 0
	BEGIN
		DECLARE @department NVARCHAR(50)
		SELECT @department = department FROM departments WHERE departmentid = @departmentid;
		DELETE FROM departments WHERE departmentid = @departmentid;
		EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 2, @departmentid, @department, null
	END
	 
	RETURN @retCode;
END
