CREATE PROCEDURE [dbo].[saveContractType] 
(
@ID int, 
@subAccountId int, 
@contractTypeDescription nvarchar(50),
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_contracttype WHERE contractTypeDescription = @contractTypeDescription AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_contracttype (subAccountId, contractTypeDescription, archived, createdOn, createdBy)
		VALUES (@subAccountId, @contractTypeDescription, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Contract Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @contractTypeDescription + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 110, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_contracttype WHERE contractTypeId <> @ID AND subAccountId = @subAccountId AND contractTypeDescription = @contractTypeDescription);
	
	IF @count = 0
	BEGIN
		DECLARE @oldtypedescription nvarchar(50);
		
		select @oldtypedescription = contractTypeDescription from codes_contracttype where contractTypeId = @ID;
		
		UPDATE codes_contracttype SET contractTypeDescription = @contractTypeDescription, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE contractTypeId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Contract Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @contractTypeDescription + ')';

		if @oldtypedescription <> @contractTypeDescription
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 110, @ID, '3E18CD92-2C6A-4815-8614-8D1742DDB98A', @oldtypedescription, @contractTypeDescription, @recordtitle, @subAccountId;
	END
END

RETURN @retVal;
