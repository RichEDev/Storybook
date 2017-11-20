CREATE PROCEDURE [dbo].[ItemRoleBasedSubcatInfo] 
@employeeId int
AS
BEGIN

declare @roletable TABLE (
    itemroleid int,
	startDate Date,
	endDate Date
)

insert @roletable
    SELECT [itemroleid], startDate, endDate  FROM [dbo].[employee_roles]
  where employeeid = @employeeId


select distinct a.subcatid, a.maximum, a.receiptmaximum, c.subcat, c.shortsubcat, c.categoryid, c.calculation, c.fromapp, c.toapp, c.[description], a.roleid, b.startDate, b.endDate
from rolesubcats a 
inner join @roletable b on a.roleid = b.itemroleid 
inner join subcats c on a.subcatid = c.subcatid order  by a.subcatid

END