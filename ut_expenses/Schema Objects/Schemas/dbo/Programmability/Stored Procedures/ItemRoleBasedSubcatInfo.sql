CREATE PROCEDURE [dbo].[ItemRoleBasedSubcatInfo] 
@employeeId int
AS
BEGIN

SELECT
   dbo.subcats.subcatid,
   dbo.rolesubcats.maximum,
   dbo.rolesubcats.receiptmaximum,
   dbo.subcats.subcat,
   dbo.subcats.shortsubcat,
   dbo.subcats.categoryid,
   dbo.subcats.calculation,
   dbo.subcats.fromapp,
   dbo.subcats.toapp,
   dbo.subcats.description,
   dbo.rolesubcats.roleid,
   dbo.employee_roles.employeeid,
   dbo.employee_roles.startdate,
   dbo.employee_roles.enddate 
FROM
   dbo.subcats 
   INNER JOIN
      dbo.rolesubcats 
      ON dbo.subcats.subcatid = dbo.rolesubcats.subcatid 
   INNER JOIN
      dbo.employee_roles 
      ON dbo.rolesubcats.roleid = dbo.employee_roles.itemroleid 
WHERE
   (
      dbo.employee_roles.employeeid = @employeeId
   )

END