CREATE PROCEDURE [dbo].[SaveAddressDistances]
	@Distances Int_Int_Int_Decimal READONLY,
	@EmployeeID int,
	@DelegateID int
AS
BEGIN
	-- set up some variables for the audit log
	DECLARE @RecordTitle nvarchar(2000);
	DECLARE @LocationDetailsA nvarchar(250);
	DECLARE @LocationDetailsB nvarchar(250);

	DECLARE @DistanceID INT;
	DECLARE @SavedDistanceID INT;
	DECLARE @AddressIDA INT;
	DECLARE @AddressIDB INT;
	DECLARE @Distance DECIMAL(18,2);
	DECLARE @currentUser INT = COALESCE(@DelegateID, @EmployeeID);
	DECLARE distances_cursor CURSOR FORWARD_ONLY FAST_FORWARD READ_ONLY FOR SELECT [c1], [c2], [c3], [c4] FROM @Distances;
	
	OPEN distances_cursor;
	
	FETCH NEXT FROM distances_cursor INTO @DistanceID, @AddressIDA, @AddressIDB, @Distance;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @LocationDetailsA = (SELECT [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [dbo].[addresses] WHERE [AddressID] = @AddressIDA);
		SET @LocationDetailsB = (SELECT [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [dbo].[addresses] WHERE [AddressID] = @AddressIDB);
		SET @RecordTitle = (SELECT 'Distance for ' + @LocationDetailsA + ' to ' + @LocationDetailsB);
	
		IF EXISTS (SELECT * FROM addressDistances WHERE AddressIDA = @AddressIDA AND AddressIDB = @AddressIDB)
		BEGIN
			--Get a copy of old values for audit log
			DECLARE @OldCustomDistance DECIMAL(18,2);
			SELECT TOP 1 @OldCustomDistance = CustomDistance
				FROM [addressDistances] WHERE AddressIDA = @AddressIDA AND AddressIDB = @AddressIDB;
			
			UPDATE [dbo].[addressDistances]
				SET [CustomDistance] = @Distance, [ModifiedOn] = GETUTCDATE(), [ModifiedBy] = @currentUser
				WHERE AddressIDA = @AddressIDA AND AddressIDB = @AddressIDB;
				
			IF @OldCustomDistance <> @Distance
				EXEC addUpdateEntryToAuditLog @EmployeeID, @DelegateID, 38, @DistanceID, '407CD65C-EDCE-4A71-B320-DC1730498F2F', @OldCustomDistance, @Distance, @RecordTitle, NULL;
		END
		ELSE IF @AddressIDA > 0 AND @AddressIDB > 0
		BEGIN
			INSERT INTO [dbo].[addressDistances] ([AddressIDA], [AddressIDB], [CustomDistance], [CreatedOn], [CreatedBy])
				VALUES (@AddressIDA, @AddressIDB, @Distance, GETDATE(), @currentUser);
			
			SET @SavedDistanceID = SCOPE_IDENTITY();
			
			EXEC addInsertEntryToAuditLog @EmployeeID, @DelegateID, 38, @SavedDistanceID, @RecordTitle, NULL;
		END
		
		FETCH NEXT FROM distances_cursor INTO @DistanceID, @AddressIDA, @AddressIDB, @Distance;
	END
	
	CLOSE distances_cursor;
	DEALLOCATE distances_cursor;

	RETURN 0;
END
