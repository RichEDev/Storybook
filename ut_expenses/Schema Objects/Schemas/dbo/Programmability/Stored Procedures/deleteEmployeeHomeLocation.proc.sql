

CREATE PROCEDURE [dbo].[deleteEmployeeHomeLocation]
	@employeelocationid int,
@CUemployeeID INT,
@CUdelegateID INT

AS
	/* SET NOCOUNT ON */
	declare @title1 nvarchar(500);
	select @title1 = username from employees where employeeid = (select employeeid from employeeHomeLocations where employeelocationid = @employeelocationid);

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select + 'Home location for ' + @title1);

	delete from employeeHomeLocations where employeelocationid = @employeelocationid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, @recordTitle, null;
	RETURN


