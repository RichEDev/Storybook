CREATE PROCEDURE dbo.SaveAccessKey 
	-- Add the parameters for the stored procedure here
	@KeyID int,
	@Key nvarchar(18),
	@EmployeeID int,
	@DeviceID nvarchar(MAX),
	@Active bit,
	@UserID int,
	@DelegateID int
AS

DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

BEGIN
	IF @KeyID = 0
	BEGIN
		INSERT INTO accessKeys ([Key], EmployeeID, Active, CreatedOn, CreatedBy)
		VALUES (@Key, @EmployeeID, @Active, GETUTCDATE(), @UserID)		
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Access Key: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @Key + ')';
		exec addInsertEntryToAuditLog @UserID, @DelegateID, 154, @retVal, @recordTitle, null;
	END
	ELSE
	BEGIN
		DECLARE @OldActiveStatus bit;
		DECLARE @OldDeviceID nvarchar(MAX);
		
		SELECT @OldActiveStatus =  Active, @DeviceID = DeviceID FROM accessKeys WHERE KeyID = @KeyID
		
		UPDATE accessKeys SET DeviceID = @DeviceID, Active = @Active, ModifiedOn = GETUTCDATE(), ModifiedBy = @UserID WHERE KeyID = @KeyID
		
		set @recordTitle = 'Access Key: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @Key + ')';
		if @OldActiveStatus <> @Active
			exec addUpdateEntryToAuditLog @UserID, @DelegateID, 154, @KeyID, 'e2b0007d-b333-41aa-8f93-015f77b6b362', @OldActiveStatus, @Active, @recordtitle, null;
		if @OldDeviceID <> @DeviceID
			exec addUpdateEntryToAuditLog @UserID, @DelegateID, 154, @KeyID, 'dc75d01d-0d7e-4c32-8dab-9a18a6ead30e', @OldDeviceID, @DeviceID, @recordtitle, null;
	END
END
