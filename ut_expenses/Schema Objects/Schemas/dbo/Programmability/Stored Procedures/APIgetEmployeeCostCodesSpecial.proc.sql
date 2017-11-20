CREATE PROCEDURE [dbo].[APIgetEmployeeCostCodesSpecial]
	@reference nvarchar(100)
AS

declare @employeeid int = cast(@reference as int);

	BEGIN
		SELECT [employeecostcodeid]
		  ,[employeeid]
		  ,[departmentid]
		  ,[costcodeid]
		  ,[projectcodeid]
		  ,[percentused]
	  FROM [dbo].[employee_costcodes]
	  WHERE [employeeid] = @employeeid
	END	
RETURN 0