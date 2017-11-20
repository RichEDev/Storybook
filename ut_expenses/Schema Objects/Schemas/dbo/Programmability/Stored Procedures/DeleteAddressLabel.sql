CREATE PROCEDURE [dbo].[DeleteAddressLabel]
	@AddressLabelID INT,
	@UserID INT,
	@DelegateID INT
AS
BEGIN
	IF EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [AddressLabelID] = @AddressLabelID AND [EmployeeID] IS NULL AND [IsPrimary] = 1)
	BEGIN
		DECLARE @AltPrimary INT = 0;
		DECLARE @AddressID INT = 0;
		
		SELECT @AddressID = [AddressID] FROM [dbo].[addressLabels] WHERE [AddressLabelID] = @AddressLabelID;
		SELECT TOP 1 @AltPrimary = [AddressLabelID] FROM [dbo].[addressLabels] WHERE [AddressLabelID] <> @AddressLabelID AND [AddressID] = @AddressID AND [EmployeeID] IS NULL ORDER BY [Label];
		
		IF @AltPrimary > 0
		BEGIN
			UPDATE [dbo].[addressLabels] SET [IsPrimary] = 1 WHERE [AddressLabelID] = @AltPrimary;
		END
	END

	DECLARE @AccountWide BIT = 0;
	DECLARE @recordTitle NVARCHAR(MAX);
	IF EXISTS (SELECT * FROM [dbo].[addressLabels] WHERE [AddressLabelID] = @AddressLabelID AND [EmployeeID] IS NULL)
	BEGIN
		SET @AccountWide = 1;
		SET @recordTitle = (SELECT [Label] FROM [dbo].[addressLabels] WHERE [AddressLabelID] = @AddressLabelID);
		SELECT TOP 1 @recordTitle = @recordTitle + ' on ' + [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [dbo].[addresses] WHERE [AddressID] = (SELECT [AddressID] FROM [dbo].[addressLabels] WHERE [AddressLabelID] = @AddressLabelID);
	END
	
	DELETE FROM [dbo].[addressLabels] WHERE [AddressLabelID] = @AddressLabelID;

	IF @AccountWide = 1
	BEGIN
		EXEC addDeleteEntryToAuditLog @UserID, @DelegateID, 38, @AddressLabelID, @recordTitle, NULL;
	END
	
	RETURN 0;
END