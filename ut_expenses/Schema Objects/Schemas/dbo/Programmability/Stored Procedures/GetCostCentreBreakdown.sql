CREATE PROCEDURE [dbo].[GetCostCentreBreakdown] @empId INT
AS
BEGIN
	SELECT ISNULL(dbo.costcodes.costcode, '') AS costcode
		,ISNULL(dbo.project_codes.projectcode, '') AS projectcode
		,ISNULL(dbo.departments.department, '') AS department
		,dbo.employee_costcodes.percentused
		,dbo.employee_costcodes.employeeid
	FROM dbo.employee_costcodes
	LEFT OUTER JOIN dbo.departments ON dbo.employee_costcodes.departmentid = dbo.departments.departmentid
	LEFT OUTER JOIN dbo.costcodes ON dbo.employee_costcodes.costcodeid = dbo.costcodes.costcodeid
	LEFT OUTER JOIN dbo.project_codes ON dbo.employee_costcodes.projectcodeid = dbo.project_codes.projectcodeid
	WHERE employeeid = @empId
END
