


--Create deleteDepartment sp
CREATE PROCEDURE [dbo].[deleteDepartment] (@departmentid INT, @userid int,
@CUemployeeID INT,
@CUdelegateID INT)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
    update [savedexpenses_costcodes] set [departmentid] = null where departmentid = @departmentid;
	update employee_costcodes set departmentid = null where departmentid = @departmentid;
	
	DECLARE @department NVARCHAR(50)
	SELECT @department = department FROM departments WHERE departmentid = @departmentid;
	
	delete from departments where departmentid = @departmentid;
	
	
	EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 2, @departmentid, @department, null
END
