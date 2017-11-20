CREATE PROCEDURE [dbo].[APIsaveEmployeeWorkAddress]
	@EmployeeWorkAddressId int out,
	@EmployeeId int,
	@AddressId int,
	@StartDate datetime,
	@EndDate datetime,
	@Active bit,
	@Temporary bit,
	@CreatedOn datetime,
	@CreatedBy int,
	@ModifiedOn datetime,
	@ModifiedBy int,
	@ESRAssignmentLocationId int
AS
BEGIN
	IF @EmployeeWorkAddressId = 0
	BEGIN
		DECLARE @existingId int = null;
		SELECT @existingId = EmployeeWorkAddressId FROM [dbo].[EmployeeWorkAddresses] where EmployeeId = @EmployeeId and AddressId = @AddressId;

		IF @existingId IS NULL
		BEGIN		
			INSERT INTO [dbo].[EmployeeWorkAddresses]
			([EmployeeId]
			,[AddressId]
			,[StartDate]
			,[EndDate]
			,[Active]
			,[Temporary]
			,[CreatedOn]
			,[CreatedBy]
			,[ESRAssignmentLocationId])
			VALUES
			(@EmployeeId
			,@AddressId
			,@StartDate
			,@EndDate
			,@Active
			,@Temporary
			,@CreatedOn
			,@CreatedBy
			,@ESRAssignmentLocationId)
		END
		ELSE
		BEGIN
			SET @EmployeeWorkAddressId = @existingId;
			
			UPDATE [dbo].[EmployeeWorkAddresses]
				SET  [StartDate] = @StartDate
				,[EndDate] = @EndDate 
				,[ModifiedOn] = @ModifiedOn 
				,[ModifiedBy] = @ModifiedBy 
				,[Active]= @Active
				,[Temporary] = @Temporary
				,[ESRAssignmentLocationId] = @ESRAssignmentLocationId
				WHERE EmployeeWorkAddressId = @EmployeeWorkAddressId;
		END
	END
	ELSE
	BEGIN
		UPDATE [dbo].[EmployeeWorkAddresses]
			SET [EmployeeId] = @EmployeeId 
			,[AddressId] = @AddressId 
			,[StartDate] = @StartDate 
			,[EndDate] = @EndDate
			,[ModifiedOn] = @ModifiedOn 
			,[ModifiedBy] = @ModifiedBy 
			,[Active]= @Active
			,[Temporary] = @Temporary
			,[ESRAssignmentLocationId] = @ESRAssignmentLocationId
			WHERE EmployeeWorkAddressId = @EmployeeWorkAddressId;
	END
END