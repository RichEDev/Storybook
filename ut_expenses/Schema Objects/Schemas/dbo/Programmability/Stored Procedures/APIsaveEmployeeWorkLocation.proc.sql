
CREATE PROCEDURE [dbo].[APIsaveEmployeeWorkLocation]
		   @employeeLocationID int out,
		   @employeeID int,
           @locationID int,
           @startDate datetime,
           @endDate datetime,
		   @active bit,
		   @temporary bit,
           @CreatedOn datetime,
           @CreatedBy int,
           @ModifiedOn datetime,
           @ModifiedBy int
AS
BEGIN
	IF @employeeLocationID = 0
		BEGIN
			DECLARE @existingId int;
			SELECT @existingId = employeelocationid FROM employeeWorkLocations where employeeid = @employeeid and locationID = @locationID and startDate = @startDate;
			IF @existingId IS NULL
			BEGIN		
			INSERT INTO [dbo].[employeeWorkLocations]
			   ([employeeID]
			   ,[locationID]
			   ,[startDate]
			   ,[endDate]
			   ,[active]
			   ,[temporary]
			   ,[CreatedOn]
			   ,[CreatedBy]
			   ,[ModifiedOn]
			   ,[ModifiedBy])
		 VALUES
			   (@employeeID ,
			   @locationID ,
			   @startDate ,
			   @endDate ,
			   @active,
			   @temporary,
			   @CreatedOn ,
			   @CreatedBy ,
			   @ModifiedOn ,
			   @ModifiedBy )
			END
			ELSE
			BEGIN
				SET @employeeLocationID = @existingId;
				UPDATE [dbo].[employeeWorkLocations]
				SET  [startDate] = @startDate 
					,[endDate] = @endDate 
					,[ModifiedOn] = @ModifiedOn 
					,[ModifiedBy] = @ModifiedBy 
					,[active]= @active
					,[temporary] = @temporary
				WHERE @employeeLocationID = employeeLocationID
			END
		END
	ELSE
		BEGIN
		UPDATE [dbo].[employeeWorkLocations]
		   SET [employeeID] = @employeeID 
			  ,[locationID] = @locationID 
			  ,[startDate] = @startDate 
			  ,[endDate] = @endDate 
			  ,[CreatedOn] = @CreatedOn 
			  ,[CreatedBy] = @CreatedBy 
			  ,[ModifiedOn] = @ModifiedOn 
			  ,[ModifiedBy] = @ModifiedBy 
			  ,[active]= @active
			  ,[temporary] = @temporary
		 WHERE @employeeLocationID = employeeLocationID
		END
END