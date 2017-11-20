CREATE procedure [dbo].[deleteTeamEmp]
@teamEmpId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @recordTitle nvarchar(2000);
	declare @subAccountId int;
	declare @teamid int;
	declare @employeeid int;
	
	select @teamid= teamid, @employeeid = employeeid from teamemps where teamempid = @teamEmpId;

	declare @employeeFullName nvarchar(max) = dbo.getEmployeeFullName(@employeeid);

	select @recordTitle = teamname, @subAccountId = subAccountId from teams where teamid = @teamid;

	delete from teamemps where teamempid = @teamEmpId;
	exec addDeleteEntryWithValueToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamEmpId, @recordTitle, @employeeFullName, @subAccountId;
	return @teamid;
end
