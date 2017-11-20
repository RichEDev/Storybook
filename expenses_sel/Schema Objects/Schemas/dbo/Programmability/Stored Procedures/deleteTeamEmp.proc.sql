CREATE procedure [dbo].[deleteTeamEmp]
@teamEmpId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @recordTitle nvarchar(2000);
	declare @subAccountId int;
	select @recordTitle = teamname, @subAccountId = subAccountId from teams where teamid = (select teamid from teamemps where teamempid = @teamEmpId);

	delete from teamemps where teamempid = @teamEmpId;
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamEmpId, @recordTitle, @subAccountId;
end
