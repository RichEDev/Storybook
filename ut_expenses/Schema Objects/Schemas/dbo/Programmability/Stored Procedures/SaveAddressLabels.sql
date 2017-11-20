CREATE PROCEDURE [dbo].[SaveAddressLabel]
	@AddressLabelID INT = 0,
	@EmployeeID INT,
	@AddressID INT,
	@Label NVARCHAR(50),
	@IsPrimary BIT,
	@UserID INT, 
	@DelegateID INT
AS
	
-- RESERVED KEYWORDS
IF UPPER(@Label) = 'HOME' 
 OR UPPER(@Label) = (SELECT UPPER(stringValue) FROM accountProperties WHERE stringKey = 'homeAddressKeyword')
 OR UPPER(@Label) = 'OFFICE' 
 OR UPPER(@Label) = (SELECT UPPER(stringValue) FROM accountProperties WHERE stringKey = 'workAddressKeyword')
BEGIN
	RETURN -3;
END

DECLARE @RecordTitle NVARCHAR(MAX);

IF (@AddressLabelID = 0)
BEGIN -- Insert New Record
	
	IF @EmployeeID IS NULL
	BEGIN
		IF EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [EmployeeID] IS NULL AND [Label] = @Label)
		BEGIN
			-- duplicate label text
			RETURN -1;
		END
	END
	ELSE
	BEGIN
		IF EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [EmployeeID] = @EmployeeID AND [Label] = @Label)
		BEGIN
			-- duplicate label text
			RETURN -1;
		END
		ELSE IF EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [EmployeeID] = @EmployeeID AND [AddressID] = @AddressID)
		BEGIN
			-- already a personal label on this address
			RETURN -2;
		END
	END

	INSERT INTO [dbo].[addressLabels]
	(
		[EmployeeID],
		[AddressID],
		[Label],
		[IsPrimary]
	)
	VALUES
	(
		@EmployeeID,
		@AddressID,
		@Label,
		@IsPrimary
	)

	SET @AddressLabelID = SCOPE_IDENTITY();

	IF @EmployeeID IS NULL
	BEGIN
		IF @IsPrimary = 1
		BEGIN
			-- this awabel is primary so un-primary any others on this address marked as primary
			UPDATE [dbo].[addressLabels] SET [IsPrimary] = 0 WHERE [IsPrimary] = 1 AND [AddressLabelID] <> @AddressLabelID AND [AddressID] = @AddressID AND [EmployeeID] IS NULL;
		END
		ELSE IF NOT EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [AddressID] = @AddressID AND [EmployeeID] IS NULL AND [IsPrimary] = 1)
		BEGIN
			-- this awabel is not marked primary but no other exists as primary, so make it primary
			UPDATE [dbo].[addressLabels] SET [IsPrimary] = 1 WHERE [AddressLabelID] = @AddressLabelID;
		END
	END
	
	SELECT @RecordTitle = @Label + ' on ' + [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [dbo].[addresses] WHERE [AddressId] = @AddressID;
	
	IF @EmployeeID IS NULL
	BEGIN
		-- only audit awabels, not personal labels
		EXEC addInsertEntryToAuditLog @UserID, @DelegateID, 38, @AddressLabelID, @RecordTitle, NULL;
	END

	RETURN @AddressLabelID;
	
END
ELSE --Update Existing Record
BEGIN

	IF @EmployeeID IS NULL
	BEGIN
		IF EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [EmployeeID] IS NULL AND [Label] = @Label AND [AddressLabelID] <> @AddressLabelID)
		BEGIN
			-- duplicate label text
			RETURN -1;
		END
	END
	ELSE
	BEGIN
		IF EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [EmployeeID] = @EmployeeID AND [Label] = @Label AND [AddressLabelID] <> @AddressLabelID)
		BEGIN
			-- duplicate label text for this user
			RETURN -1;
		END
		ELSE IF EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [EmployeeID] = @EmployeeID AND [AddressID] = @AddressID AND [AddressLabelID] <> @AddressLabelID)
		BEGIN
			-- already a personal label on this address
			RETURN -2;
		END
	END

	-- Create a backup of existing data
	DECLARE @oldEmployeeID NVARCHAR(32);
	DECLARE @oldAddressID NVARCHAR(256);
	DECLARE @oldLabel NVARCHAR(50);
	DECLARE @oldIsPrimary BIT;

	SELECT @oldEmployeeID = [EmployeeID],
		@oldAddressID = [AddressID],
		@oldLabel = [Label],
		@oldIsPrimary = [IsPrimary]
		FROM [dbo].[addressLabels]
		WHERE [AddressLabelID] = @AddressLabelID;
		
	SELECT @RecordTitle = @Label + ' on ' + [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [dbo].[addresses] WHERE [AddressId] = @AddressID;
	
	-- if it's the only awabel for that address, they can't un-primary it
	IF @IsPrimary = 0 AND NOT EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [AddressID] = @AddressID AND [EmployeeID] IS NULL AND [AddressLabelID] <> @AddressLabelID)
	BEGIN
		SET @IsPrimary = 1;
	END
	
	-- Perform the update
	UPDATE [dbo].[addressLabels] SET [EmployeeID] = @EmployeeID, [AddressID] = @AddressID, [Label] = @Label, [IsPrimary] = @IsPrimary WHERE [AddressLabelID] = @AddressLabelID;
	
	IF @oldIsPrimary <> @IsPrimary AND @EmployeeID IS NULL
	BEGIN
		IF @IsPrimary = 1
		BEGIN
			-- set an awabel that wasn't primary to be the new primary, so remove primary awabel status for any other awabel on this address
			UPDATE [dbo].[addressLabels] SET [IsPrimary] = 0 WHERE [IsPrimary] = 1 AND [AddressLabelID] <> @AddressLabelID AND [AddressID] = @AddressID AND [EmployeeID] IS NULL;
		END
		ELSE
		BEGIN
			-- set the primary awabel to not be primary, so set the first other awabel for that address alphabetically to be primary 
			UPDATE [dbo].[addressLabels] SET [IsPrimary] = 1 WHERE [AddressLabelID] = (SELECT TOP 1 [AddressLabelID] FROM [addressLabels] WHERE [AddressID] = @AddressID AND [EmployeeID] IS NULL AND [AddressLabelID] <> @AddressLabelID ORDER BY [Label]);
		END
	END
	
	
	-- Update the audit log if it's not a personal label
	IF @EmployeeID IS NULL
	BEGIN
		IF (@oldAddressID <> @AddressID)
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 38, @AddressLabelID, '2ba5481d-219f-470c-8001-e079eae4d76e', @oldAddressID, @AddressID, @RecordTitle, NULL;

		IF (@oldLabel <> @Label)
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 38, @AddressLabelID, 'b0793892-ed59-42f6-82fc-a84111c15648', @oldLabel, @Label, @RecordTitle, NULL;

		IF (@oldIsPrimary <> @IsPrimary)
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 38, @AddressLabelID, '9D9C488D-B554-4858-80F8-DC95AC487970', @oldIsPrimary, @IsPrimary, @RecordTitle, NULL;
	END

	RETURN @AddressLabelID;
END