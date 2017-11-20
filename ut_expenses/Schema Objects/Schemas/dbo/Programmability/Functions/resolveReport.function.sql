
CREATE FUNCTION [dbo].[resolveReport] (@employeeid int, @linemanagers LineManagers readonly)  
RETURNS @reportPermissions table ([employeeid] int) AS  
begin
declare @empid int

declare @newLineManagers LineManagers;
DECLARE @employeeSignofftype TABLE (employeeID int,signOffType int);
declare @generalOptionCheck bit =0 --General option check for default costcode owner

if (select count(*) from @linemanagers where employeeid = @employeeid) = 0
BEGIN
      insert into @newLineManagers
            select @employeeid union select employeeid from @linemanagers
            
      insert into @reportPermissions(employeeid) values (@employeeid)

	  
if exists( select * from accountProperties  where stringKey ='defaultCostCodeOwner' 
and
( stringValue = '25,'+CONVERT(varchar(36),@employeeid)
or stringValue in (select '11,'+CONVERT(varchar(36),budgetholderid) from budgetholders where employeeid =@employeeid)
or stringValue in (select '49,'+CONVERT(varchar(36),teamid) from teamemps where employeeid =@employeeid)
))
begin
set @generalOptionCheck =1
end

	  	  	insert into @employeeSignofftype (employeeID,signOffType) 
	  
	  --Budget Holder ,team ,employees,line manager
	  select distinct employees.employeeid,signofftype  from employees
      inner join signoffs on (signoffs.groupid = employees.groupid )
	  left join budgetholders on (signoffs.relid = budgetholders.budgetholderid)
	  left join teamemps  on (signoffs.relid = teamemps.teamid )
	  where signoffs.signofftype in (1,2,3,4) and(
	  (budgetholders.employeeid =@employeeid) or 
	  ( teamemps.employeeid =@employeeid ) or 
	  (signoffs.relid =@employeeid) or
	  ( employees.linemanager =@employeeid)) 

      declare employees_cursor cursor for

      select distinct employeeid from employees 
            inner join signoffs on signoffs.groupid = employees.groupid
      where  employeeid not in (select distinct employeeid from @linemanagers) and 
	  (  
	   --For Budget Holder ,team ,employees,line manager
	  (employeeid in (select t.employeeID from @employeeSignofftype t where t.signOffType = signoffs.signofftype))  
	  or
	  	  
	   --Costcode 
	  (signofftype =8 and employeeid in (
	  --Costcode can be Budget Holder ,team ,employees,line manager
	   select t.employeeID from @employeeSignofftype t 
	   UNION
	   select distinct employees.employeeid from employees
       inner join signoffs on (signoffs.groupid = employees.groupid and signofftype =8)
       inner join employee_costcodes on (employee_costcodes.employeeid =employees.employeeid)
       inner join costcodes c on ( c.costcodeid=employee_costcodes.costcodeid) where employees.employeeid =@employeeid
	   UNION 
	   --Cost code can be Assigment supervisor
	   select employeeid from esr_assignments where esr_assignments.SignOffOwner = '25,'+CONVERT(varchar(39),@employeeid)
	   UNION
	   --Default costcode from general option
	   
       select employeeid from employee_costcodes where @generalOptionCheck =1 and costcodeid is null))

	   or
	   --Assignment supervisor
	   (signofftype =9 and employees.employeeid in (
	   select employeeid from esr_assignments where esr_assignments.SignOffOwner ='25,'+CONVERT(varchar(39),@employeeid)
	   Union
	   --Assignment supervisor can be Budget Holder ,team ,employees
	   select employeesignoff.EmployeeID from @employeeSignofftype employeesignoff where employeesignoff.SignOffType in (1,2,3)))
	    
		or

		--Approval Matrix can Budget Holder ,team ,employees
		(signoffs.signofftype =6 and 
		(signoffs.relid in (select approvalMatrices.approvalMatrixId from approvalMatrices inner join approvalMatrixLevels aml on (approvalMatrices.approvalMatrixId = aml.approvalMatrixId and aml.approverEmployeeId =@employeeid) or
		employees.employeeid in (select t.EmployeeID from @employeeSignofftype t where t.SignOffType in (1,2,3)))))

	  )

      open employees_cursor

      fetch next from employees_cursor into @empid
            while @@fetch_status = 0
                  BEGIN
                        if not exists (select * from @reportPermissions where employeeid = @empid)
                        begin
                              insert into @reportPermissions (employeeid) select employeeid from dbo.resolveReport(@empid, @newLineManagers)
                              insert into @newLineManagers 
                                    select distinct employeeid from @reportPermissions where employeeid not in (select employeeid from @newLineManagers)
                        end
                        fetch next from employees_cursor into @empid
                  end
            close employees_cursor
      deallocate employees_cursor
END
      return
end