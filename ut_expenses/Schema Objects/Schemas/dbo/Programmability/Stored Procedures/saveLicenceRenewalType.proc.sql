CREATE PROCEDURE [dbo].[saveLicenceRenewalType]
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
	SET @count = (SELECT COUNT(*) FROM licenceRenewalTypes WHERE description = @description AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO licenceRenewalTypes (subAccountId, description, archived, createdOn, createdBy)
		VALUES (@subAccountId, @description, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Licence Renewal Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 115, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM licenceRenewalTypes WHERE renewalType <> @ID AND subAccountId = @subAccountId AND description = @description);
	
	IF @count = 0
	BEGIN
		DECLARE @olddescription nvarchar(50);
				
		select @olddescription = description from licenceRenewalTypes where renewalType = @ID;
		
		UPDATE licenceRenewalTypes SET description = @description, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE renewalType = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Licence Renewal Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';

		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 115, @ID, '6F291BA0-D13E-43DB-BBEA-3AFBCEDA0570', @olddescription, @description, @recordtitle, @subAccountId;

	END
END

RETURN @retVal;
