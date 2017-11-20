CREATE PROCEDURE [dbo].[addTeamEmp]
@teamEmpId int,
@teamId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @recordTitle nvarchar(2000);
	declare @subAccountId int;
	declare @employeeFullName nvarchar(max) = dbo.getEmployeeFullName(@teamEmpId);
	select @recordTitle = teamname , @subAccountId = subAccountId from teams where teamid = @teamid;

	if not exists (select teamempid from teamemps where teamid = @teamId and employeeid = @teamEmpId) 
	BEGIN
		insert into teamemps (teamid, employeeid) values (@teamId, @teamEmpId)
		exec addInsertEntryWithValueToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamEmpId, @recordTitle, @subAccountId, @employeeFullName;
	END
	
end
