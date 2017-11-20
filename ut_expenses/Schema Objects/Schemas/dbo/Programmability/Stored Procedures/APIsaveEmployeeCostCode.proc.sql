

CREATE PROCEDURE [dbo].[APIsaveEmployeeCostCode]
	@employeecostcodeid int,
	@employeeid int ,
	@departmentid int,
	@costcodeid int ,
	@projectcodeid int,
	@percentused int
AS
IF @employeecostcodeid = 0
	BEGIN
		INSERT INTO [dbo].[employee_costcodes]
			   ([employeeid]
			   ,[departmentid]
			   ,[costcodeid]
			   ,[projectcodeid]
			   ,[percentused])
		 VALUES
			   (@employeeid  
			   ,@departmentid  
			   ,@costcodeid  
			   ,@projectcodeid  
			   ,@percentused  )
	SET @employeecostcodeid = SCOPE_IDENTITY();
	END
ELSE
	BEGIN
		UPDATE [dbo].[employee_costcodes]
	   SET [employeeid] = @employeeid  
		  ,[departmentid] = @departmentid  
		  ,[costcodeid] = @costcodeid  
		  ,[projectcodeid] = @projectcodeid  
		  ,[percentused] = @percentused  
	 WHERE [employeecostcodeid] = @employeecostcodeid
	END	
RETURN @employeecostcodeid