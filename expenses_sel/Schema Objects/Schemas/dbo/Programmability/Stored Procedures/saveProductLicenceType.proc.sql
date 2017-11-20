CREATE PROCEDURE [dbo].[saveProductLicenceType]
(
@ID int, 
@subAccountId int, 
@description nvarchar(150),
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_licencetypes WHERE description = @description AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_licencetypes (subAccountId, description, createdOn, createdBy)
		VALUES (@subAccountId, @description, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Licence Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 136, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_licencetypes WHERE licenceTypeId <> @ID AND subAccountId = @subAccountId AND description = @description);
	
	IF @count = 0
	BEGIN
		DECLARE @olddescription nvarchar(50);
				
		select @olddescription = description from codes_licencetypes where licenceTypeId = @ID;
		
		UPDATE codes_licencetypes SET description = @description, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE licenceTypeId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Licence Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';

		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 136, @ID, '72B9B98C-88F6-44B6-81F5-F0E131D0EF9C', @olddescription, @description, @recordtitle, @subAccountId;

	END
END

RETURN @retVal;
