CREATE PROCEDURE [dbo].[APIsaveEmployeeHomeAddress]
	@EmployeeHomeAddressId int out,
	@EmployeeId int,
	@AddressId int,
	@StartDate datetime,
	@EndDate datetime,
	@CreatedOn datetime,
	@CreatedBy int,
	@ModifiedOn datetime,
	@ModifiedBy int
AS
BEGIN
	IF @EmployeeHomeAddressId = 0
	BEGIN
		DECLARE @currentId int = null;
		SELECT @currentId = EmployeeHomeAddressId FROM [dbo].[employeeHomeAddresses] where AddressId = @AddressId and EmployeeId = @EmployeeId;
		
		IF @currentId is null
		BEGIN
			INSERT INTO [dbo].[employeeHomeAddresses]
				([EmployeeId]
				,[AddressId]
				,[StartDate]
				,[EndDate]
				,[CreatedOn]
				,[CreatedBy])
			VALUES
				(@EmployeeId ,
				@AddressId ,
				@StartDate ,
				@EndDate ,
				@CreatedOn ,
				@CreatedBy);
				
			SET @EmployeeHomeAddressId = SCOPE_IDENTITY();
		END
		ELSE
		BEGIN
			SET @EmployeeHomeAddressId = @currentId;
			
			UPDATE [dbo].[employeeHomeAddresses]
				SET [StartDate] = @StartDate 
				,[EndDate] = @EndDate
				,[ModifiedOn] = @ModifiedOn 
				,[ModifiedBy] = @ModifiedBy 
				WHERE EmployeeHomeAddressId = @EmployeeHomeAddressId;
		END
	END
	ELSE
	BEGIN
		UPDATE [dbo].[employeeHomeAddresses]
			SET [EmployeeId] = @EmployeeId 
			,[AddressId] = @AddressId 
			,[StartDate] = @StartDate 
			,[EndDate] = @EndDate
			,[ModifiedOn] = @ModifiedOn 
			,[ModifiedBy] = @ModifiedBy 
			WHERE EmployeeHomeAddressId = @EmployeeHomeAddressId;
	END
END