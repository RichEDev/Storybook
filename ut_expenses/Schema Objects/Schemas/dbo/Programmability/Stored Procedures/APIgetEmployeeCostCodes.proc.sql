CREATE PROCEDURE [dbo].[APIgetEmployeeCostCodes]
	@employeecostcodeid int

AS
IF @employeecostcodeid = 0
	BEGIN
		SELECT [employeecostcodeid]
		  ,[employeeid]
		  ,[departmentid]
		  ,[costcodeid]
		  ,[projectcodeid]
		  ,[percentused]
	  FROM [dbo].[employee_costcodes]
	END
ELSE
	BEGIN
		SELECT [employeecostcodeid]
		  ,[employeeid]
		  ,[departmentid]
		  ,[costcodeid]
		  ,[projectcodeid]
		  ,[percentused]
	  FROM [dbo].[employee_costcodes]
	  WHERE [employeecostcodeid] = @employeecostcodeid
	END	
RETURN 0