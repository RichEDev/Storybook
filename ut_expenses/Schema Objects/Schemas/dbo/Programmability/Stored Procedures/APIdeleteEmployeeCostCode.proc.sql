CREATE PROCEDURE [dbo].[APIdeleteEmployeeCostCode]
	@employeecostcodeid int
AS
	DELETE FROM employee_costcodes WHERE [employeecostcodeid] = @employeecostcodeid
RETURN 0