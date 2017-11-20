CREATE FUNCTION [dbo].[resolveReport] (@employeeid int, @linemanagers LineManagers readonly)  
RETURNS @reportPermissions table ([employeeid] int) AS  
begin
declare @empid int

if (select count(*) from @linemanagers where employeeid = @employeeid) = 0
BEGIN
	insert into @reportPermissions(employeeid) values (@employeeid)

	declare employees_cursor cursor for
	 select employeeid from employees 
		inner join groups on groups.groupid = employees.groupid
		inner join signoffs on signoffs.groupid = groups.groupid
	where  employeeid not in (select employeeid from @linemanagers) and 
	(
		(signofftype = 1 and relid in (select budgetholderid from budgetholders where employeeid = @employeeid)) or
		(signofftype = 2 and relid = @employeeid) or
		(signofftype = 3 and relid in (select teamid from teamemps where employeeid = @employeeid)) or
		(signofftype = 4 and linemanager = @employeeid)
	)
		
	open employees_cursor

	fetch next from employees_cursor into @empid
		while @@fetch_status = 0
			BEGIN
				if not exists (select * from @reportPermissions where employeeid = @empid)
				begin
					insert into @reportPermissions (employeeid) select employeeid from dbo.resolveReport(@empid, @linemanagers)
				end
				fetch next from employees_cursor into @empid
			end
		close employees_cursor
	deallocate employees_cursor
END
	return
end
