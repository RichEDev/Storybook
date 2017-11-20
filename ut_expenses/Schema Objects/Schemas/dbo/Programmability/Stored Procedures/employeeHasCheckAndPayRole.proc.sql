CREATE PROCEDURE dbo.employeeHasCheckAndPayRole
@employeeID int
AS
declare @arID int;
declare @cpAccess bit = 0;

declare lp cursor for
select accessroleID from employeeAccessRoles where employeeID = @employeeID
open lp
fetch next from lp into @arID
while @@FETCH_STATUS = 0
begin
	select @cpAccess = viewAccess from accessRoleElementDetails where roleID = @arID and elementID = 14; -- element 14 = check & pay
	
	if @cpAccess = 1
	begin
		goto exitLoop;
	end
	
	fetch next from lp into @arID
end
exitLoop:
close lp
deallocate lp

return @cpAccess
