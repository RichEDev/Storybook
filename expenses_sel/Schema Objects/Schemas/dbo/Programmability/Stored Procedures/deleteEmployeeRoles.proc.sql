



--Create deleteEmployeeRoles sp
CREATE PROCEDURE [dbo].[deleteEmployeeRoles] (@employeeid int,
@CUemployeeID INT,
@CUdelegateID INT) 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	delete from employee_roles where employeeid = @employeeid;

	
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'All item roles for user ' + CAST(@employeeid AS nvarchar(20)));
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, @recordTitle, null;
END



