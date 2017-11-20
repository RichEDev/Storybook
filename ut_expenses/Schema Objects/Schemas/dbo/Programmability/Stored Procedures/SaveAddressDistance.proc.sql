CREATE PROCEDURE [dbo].[SaveAddressDistance]
	@AddressIDA INT,
	@AddressIDB INT,
	@CustomDistance DECIMAL(18,2) = NULL,
	@PostcodeAnywhereFastestDistance DECIMAL(18,2) = NULL,
	@PostcodeAnywhereShortestDistance DECIMAL(18,2) = NULL,
	@CreatedBy INT,
	@ModifiedBy INT,
	@EmployeeID INT,
	@DelegateID INT
AS
BEGIN
	-- set up some variables for the audit log
	DECLARE @RecordTitle NVARCHAR(2000);
	DECLARE @LocationDetailsA NVARCHAR(250);
	DECLARE @LocationDetailsB NVARCHAR(250);
	SET @LocationDetailsA = (SELECT [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [dbo].[addresses] WHERE [AddressID] = @AddressIDA);
	SET @LocationDetailsB = (SELECT [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [dbo].[addresses] WHERE [AddressID] = @AddressIDB);
	SET @RecordTitle = (SELECT 'Distance for ' + @LocationDetailsA + ' to ' + @LocationDetailsB);
	
	DECLARE @AddressDistanceID INT = 0;
	SELECT TOP 1 @AddressDistanceID = [AddressDistanceID] FROM [dbo].[addressDistances] WHERE [AddressIDA] = @AddressIDA AND [AddressIDB] = @AddressIDB;
	
	IF @AddressDistanceID = 0
	BEGIN
		INSERT INTO [dbo].[addressDistances] 
		(
			[AddressIDA]
			,[AddressIDB]
			,[CustomDistance]
			,[PostcodeAnywhereFastestDistance]
			,[PostcodeAnywhereShortestDistance]
			,[CreatedOn]
			,[CreatedBy]
			,[ModifiedOn]
			,[ModifiedBy]
		)
		VALUES 
		(
			@AddressIDA
			,@AddressIDB
			,COALESCE(@CustomDistance, 0)
			,@PostcodeAnywhereFastestDistance
			,@PostcodeAnywhereShortestDistance
			,GETDATE()
			,@CreatedBy
			,NULL
			,NULL
		);
		
		SET @AddressDistanceID = SCOPE_IDENTITY();

		EXEC addInsertEntryToAuditLog @EmployeeID, @DelegateID, 38, @AddressDistanceID, @RecordTitle, null;
		
		RETURN @AddressDistanceID;
	END
	ELSE --Update existing entry
	BEGIN
		--Get a copy of old values for audit log
		DECLARE @OldCustomDistance DECIMAL(18,2);
		DECLARE @OldPostcodeAnywhereFastestDistance DECIMAL(18,2);
		DECLARE @OldPostcodeAnywhereShortestDistance DECIMAL(18,2);
		SELECT TOP 1 @OldCustomDistance = CustomDistance,
			@OldPostcodeAnywhereFastestDistance = PostcodeAnywhereFastestDistance,
			@OldPostcodeAnywhereShortestDistance = PostcodeAnywhereShortestDistance 
			FROM [addressDistances] 
			WHERE [AddressDistanceID] = @AddressDistanceID;

		UPDATE [dbo].[addressDistances]   
			SET [AddressIDA] = @AddressIDA
			,[AddressIDB] = @AddressIDB
			,[CustomDistance] = COALESCE(@CustomDistance, CustomDistance)
			,[PostcodeAnywhereFastestDistance] = COALESCE(@PostcodeAnywhereFastestDistance, PostcodeAnywhereFastestDistance) 
			,[PostcodeAnywhereShortestDistance] = COALESCE(@PostcodeAnywhereShortestDistance, PostcodeAnywhereShortestDistance)
			,[ModifiedOn] = GETDATE()
			,[ModifiedBy] = @ModifiedBy
			WHERE [AddressDistanceID] = @AddressDistanceID;

		IF @PostcodeAnywhereFastestDistance <> @OldPostcodeAnywhereFastestDistance
			EXEC addUpdateEntryToAuditLog @EmployeeID, @DelegateID, 38, @AddressDistanceID, '80164ADB-C459-4261-AD4B-3D274C696C86', @OldPostcodeAnywhereFastestDistance, @PostcodeAnywhereFastestDistance, @RecordTitle, null;

		IF @PostcodeAnywhereShortestDistance <> @OldPostcodeAnywhereShortestDistance
			EXEC addUpdateEntryToAuditLog @EmployeeID, @DelegateID, 38, @AddressDistanceID, 'C0483A8B-EF01-4C2E-9A03-BD1D7E175816', @OldPostcodeAnywhereShortestDistance, @PostcodeAnywhereShortestDistance, @RecordTitle, null;
		
		IF @CustomDistance <> @OldCustomDistance
			EXEC addUpdateEntryToAuditLog @EmployeeID, @DelegateID, 38, @AddressDistanceID, '407CD65C-EDCE-4A71-B320-DC1730498F2F', @OldCustomDistance, @CustomDistance, @RecordTitle, null;
		
		RETURN @AddressDistanceID;
	END
	
	RETURN -1;
END