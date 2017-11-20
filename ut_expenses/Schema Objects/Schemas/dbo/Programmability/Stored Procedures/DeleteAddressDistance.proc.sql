CREATE PROCEDURE [dbo].[DeleteAddressDistance]
	@AddressDistanceID int,
	@RemoveRecommendedDistances bit = 0,
	@EmployeeID int,
	@DelegateID int
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[addressDistances] WHERE [AddressDistanceID] = @AddressDistanceID)
	BEGIN
		RETURN -1;
	END
	
	-- set up some variables for the audit log
	DECLARE @CurrentUser INT = COALESCE(@DelegateID, @EmployeeID);
	DECLARE @Count INT;
	DECLARE @RecordTitle nvarchar(2000);
	DECLARE @LocationDetailsA nvarchar(250);
	DECLARE @LocationDetailsB nvarchar(250);
	SET @LocationDetailsA = (select [Line1] + ', ' + [City] + ', ' + [Postcode] from [addresses] where [AddressID] = (SELECT [AddressIDA] FROM [dbo].[addressDistances] WHERE AddressDistanceID = @AddressDistanceID));
	SET @LocationDetailsB = (select [Line1] + ', ' + [City] + ', ' + [Postcode] from [addresses] where [AddressID] = (SELECT [AddressIDB] FROM [dbo].[addressDistances] WHERE AddressDistanceID = @AddressDistanceID));
	SET @RecordTitle = (SELECT 'Distances for ' + @LocationDetailsA + ' to ' + @LocationDetailsB);

	IF @RemoveRecommendedDistances = 1
	BEGIN
		-- Insert statements for procedure here
		DELETE FROM [dbo].[addressDistances] WHERE AddressDistanceID = @AddressDistanceID;

		EXEC addDeleteEntryToAuditLog @EmployeeID, @DelegateID, 38, @AddressDistanceID, @RecordTitle, null;
	END
	ELSE
	BEGIN
		DECLARE @OldDistance DECIMAL(18,2);
		SELECT @OldDistance = [CustomDistance] FROM [dbo].[addressDistances];
		
		UPDATE [dbo].[addressDistances] SET [CustomDistance] = 0, [ModifiedBy] = @CurrentUser, [ModifiedOn] = GETUTCDATE() WHERE [AddressDistanceID] = @AddressDistanceID;
		
		EXEC addUpdateEntryToAuditLog @EmployeeID, @DelegateID, 38, @AddressDistanceID, '407CD65C-EDCE-4A71-B320-DC1730498F2F', @OldDistance, '0', @RecordTitle, NULL;
	END
	
	RETURN @AddressDistanceID;
END