CREATE PROCEDURE [dbo].[saveUnit]
(
@ID int, 
@subAccountId int, 
@description nvarchar(50),
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_units WHERE description = @description AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_units (subAccountId, description, archived, createdOn, createdBy)
		VALUES (@subAccountId, @description, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Unit ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 118, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_units WHERE unitId <> @ID AND subAccountId = @subAccountId AND description = @description);
	
	IF @count = 0
	BEGIN
		DECLARE @olddescription nvarchar(50);
				
		select @olddescription = description from codes_units where unitId = @ID;
		
		UPDATE codes_units SET description = @description, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE unitId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Unit ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';

		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 118, @ID, 'C9DB0368-196A-4091-AA67-3230A810DBA1', @olddescription, @description, @recordtitle, @subAccountId;

	END
END

RETURN @retVal;

