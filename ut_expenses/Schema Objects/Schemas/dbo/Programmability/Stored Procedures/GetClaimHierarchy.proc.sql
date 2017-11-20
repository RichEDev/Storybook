CREATE PROCEDURE [dbo].[GetClaimHierarchy]
@employeeid int ,
@matchFieldID uniqueidentifier
AS
DECLARE @fieldname nvarchar(200);
DECLARE @tablename nvarchar(200);
DECLARE @tableid UNIQUEIDENTIFIER;
DECLARE @temptable Employees
DECLARE @employeeCount int = 0;
DECLARE @lastEmployeeCount int = 1;

 SELECT @fieldname = field, @tableid = tableid from fields where fieldid = @matchFieldID;
SELECT @tablename = tablename from tables where tableid = @tableid;

 CREATE TABLE #Hierarchy (  [employeeid] [int],  [checked] bit DEFAULT (0),  PRIMARY KEY CLUSTERED ([employeeid] ASC) WITH (IGNORE_DUP_KEY = ON));

INSERT INTO #Hierarchy (employeeid) VALUES (@employeeid)


DECLARE @sql NVARCHAR(MAX) = 'INSERT INTO #Hierarchy (employeeid) SELECT ' + @tablename + '.[employeeid] FROM ' + @tablename + ' WHERE ' + @tablename + '.' + @fieldname + '  IN (SELECT employeeid FROM #Hierarchy)';

WHILE @employeeCount <> @lastEmployeeCount
BEGIN
  SET @lastEmployeeCount = @employeeCount 
  EXEC sp_executesql @sql;
  SET @employeeCount = (SELECT distinct count(employeeid) FROM #Hierarchy)
END

 -- Start of extras 
 
declare @budgetholderid int 
declare @teamid int 
declare @approvalMatrixId int
-- Create a list of employees and their groups
CREATE TABLE #EmployeesGroups (  [employeeid] [int],  [groupid] int ,  PRIMARY KEY CLUSTERED ([employeeid], [groupid] ASC) WITH (IGNORE_DUP_KEY = ON));
insert into #EmployeesGroups select employeeid, groupid from employees where groupid is not null
insert into #EmployeesGroups select employeeid, groupidcc from employees where groupidcc is not null
insert into #EmployeesGroups select employeeid, groupidpc from employees where groupidpc is not null

--Get stages where employee is assigned.

insert into #Hierarchy  select employeeid, 0 from #EmployeesGroups where groupid in (select groupid from signoffs where relid = @employeeid and signofftype = 2)

-- Get stages where employee is a budget holder.

declare budgetsCursor cursor for select budgetholderid from budgetholders where employeeid = @employeeid
open budgetsCursor
fetch next from budgetsCursor into @budgetholderid
WHILE @@FETCH_STATUS = 0
BEGIN
  insert into #Hierarchy  select employeeid , 0 from #employeesgroups where groupid in (select groupid from signoffs where relid = @budgetholderid and signofftype = 1)  
  fetch next from budgetsCursor into @budgetholderid
END
CLOSE budgetsCursor;
DEALLOCATE budgetsCursor;

-- Get stages where employee is in a team.
declare teamsCursor cursor for select teamid from teamemps where employeeid = @employeeid
open teamsCursor
fetch next from teamsCursor into @teamid
while @@FETCH_STATUS = 0
BEGIN
insert into #Hierarchy  select employeeid , 0 from #employeesgroups where groupid in (select groupid from signoffs where relid = @teamid and signofftype = 3)
fetch next from teamsCursor into @teamid
END
CLOSE teamsCursor
deallocate teamsCursor;

-- Get approval Matrix

declare aMatrixEmployee cursor for select approvalMatrixID from approvalMatrixLevels where approverEmployeeID = @employeeid
open aMatrixEmployee
fetch next from aMatrixEmployee into @approvalMatrixId
while @@FETCH_STATUS = 0
BEGIN
insert into #Hierarchy  select employeeid , 0 from #employeesgroups where groupid in (select groupid from signoffs where relid = @approvalMatrixId and (signofftype = 6 OR signofftype = 7))
fetch next from aMatrixEmployee into @approvalMatrixId
END
CLOSE aMatrixEmployee
deallocate aMatrixEmployee;

declare aMatrixTeam cursor for select approvalMatrixID from approvalMatrixLevels where approverTeamID in (select teamid from teamemps where employeeid = @employeeid)
open aMatrixTeam
fetch next from aMatrixTeam into @approvalMatrixId
while @@FETCH_STATUS = 0
BEGIN
insert into #Hierarchy  select employeeid , 0 from #employeesgroups where groupid in (select groupid from signoffs where relid = @approvalMatrixId and (signofftype = 6 OR signofftype = 7))
fetch next from aMatrixTeam into @approvalMatrixId
END
CLOSE aMatrixTeam
deallocate aMatrixTeam;

declare aMatrixBudget cursor for select approvalMatrixID from approvalMatrixLevels where approverBudgetholderID in (select Budgetholderid from budgetholders where employeeid = @employeeid)
open aMatrixBudget
fetch next from aMatrixBudget into @approvalMatrixId
while @@FETCH_STATUS = 0
BEGIN
insert into #Hierarchy  select employeeid , 0 from #employeesgroups where groupid in (select groupid from signoffs where relid = @approvalMatrixId and (signofftype = 6 OR signofftype = 7))
fetch next from aMatrixBudget into @approvalMatrixId
END
CLOSE aMatrixBudget

declare aMatrixOwnerE cursor for select approvalMatrixID from approvalMatrices where defaultApproverEmployeeId  = @employeeid
open aMatrixOwnerE
fetch next from aMatrixOwnerE into @approvalMatrixId
while @@FETCH_STATUS = 0
BEGIN
insert into #Hierarchy  select employeeid , 0 from #employeesgroups where groupid in (select groupid from signoffs where relid = @approvalMatrixId and (signofftype = 6 OR signofftype = 7))
fetch next from aMatrixOwnerE into @approvalMatrixId
END
CLOSE aMatrixOwnerE

declare aMatrixOwnerT cursor for select approvalMatrixID from approvalMatrices where defaultApproverTeamId in (select teamid from teamemps where employeeid = @employeeid)
open aMatrixOwnerT
fetch next from aMatrixOwnerT into @approvalMatrixId
while @@FETCH_STATUS = 0
BEGIN
insert into #Hierarchy  select employeeid , 0 from #employeesgroups where groupid in (select groupid from signoffs where relid = @approvalMatrixId and (signofftype = 6 OR signofftype = 7))
fetch next from aMatrixOwnerT into @approvalMatrixId
END
CLOSE aMatrixOwnerT


declare aMatrixOwnerB cursor for select approvalMatrixID from approvalMatrices where defaultapproverBudgetholderID in (select Budgetholderid from budgetholders where employeeid = @employeeid)
open aMatrixOwnerB
fetch next from aMatrixOwnerB into @approvalMatrixId
while @@FETCH_STATUS = 0
BEGIN
insert into #Hierarchy  select employeeid , 0 from #employeesgroups where groupid in (select groupid from signoffs where relid = @approvalMatrixId and (signofftype = 6 OR signofftype = 7))
fetch next from aMatrixOwnerB into @approvalMatrixId
END
CLOSE aMatrixOwnerB



deallocate aMatrixBudget;
drop table #EmployeesGroups
-- End of Extras

 
 
 SELECT employeeid, checked FROM #Hierarchy;

RETURN 0
