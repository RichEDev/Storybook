CREATE PROCEDURE [dbo].[APIsaveEmployeeHomeLocation]
		   @employeeLocationID int out,
		   @employeeID int,
           @locationID int,
           @startDate datetime,
           @endDate datetime,
           @CreatedOn datetime,
           @CreatedBy int,
           @ModifiedOn datetime,
           @ModifiedBy int
AS
BEGIN
	IF @employeeLocationID = 0
		BEGIN
			DECLARE @currentId int;
			SELECT @currentId = EmployeeLocationId FROM employeeHomeLocations where locationID = @locationID and employeeid = @employeeID;
			IF @currentId is null
				BEGIN
					INSERT INTO [dbo].[employeeHomeLocations]
					   ([employeeID]
					   ,[locationID]
					   ,[startDate]
					   ,[endDate]
					   ,[CreatedOn]
					   ,[CreatedBy]
					   ,[ModifiedOn]
					   ,[ModifiedBy])
				 VALUES
					   (@employeeID ,
					   @locationID ,
					   @startDate ,
					   @endDate ,
					   @CreatedOn ,
					   @CreatedBy ,
					   @ModifiedOn ,
					   @ModifiedBy )
						set @employeeLocationID = scope_identity();
				END
			ELSE
				BEGIN
					SET @employeeLocationID = @currentId;
					UPDATE [dbo].[employeeHomeLocations]
					   SET [startDate] = @startDate 
						  ,[endDate] = @endDate
						  ,[ModifiedOn] = @ModifiedOn 
						  ,[ModifiedBy] = @ModifiedBy 
					 WHERE @employeeLocationID = employeeLocationID
				END
		END
	ELSE
		BEGIN
			UPDATE [dbo].[employeeHomeLocations]
			   SET [employeeID] = @employeeID 
				  ,[locationID] = @locationID 
				  ,[startDate] = @startDate 
				  ,[endDate] = @endDate 
				  ,[CreatedOn] = @CreatedOn 
				  ,[CreatedBy] = @CreatedBy 
				  ,[ModifiedOn] = @ModifiedOn 
				  ,[ModifiedBy] = @ModifiedBy 
			 WHERE @employeeLocationID = employeeLocationID
			 set @employeeLocationID = scope_identity();
		END
END