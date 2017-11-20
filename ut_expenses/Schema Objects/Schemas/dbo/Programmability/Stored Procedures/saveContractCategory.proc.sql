CREATE PROCEDURE [dbo].[saveContractCategory] 
(
@ID int, 
@subAccountId int, 
@categoryDescription nvarchar(50), 
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_contractcategory WHERE categoryDescription = @categoryDescription AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_contractcategory (subAccountId, categoryDescription, archived, createdOn, createdBy)
		VALUES (@subAccountId, @categoryDescription, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Contract Category ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @categoryDescription + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 109, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_contractcategory WHERE categoryId <> @ID AND subAccountId = @subAccountId AND categoryDescription = @categoryDescription);
	
	IF @count = 0
	BEGIN
		DECLARE @oldcategorydescription nvarchar(50);
		
		select @oldcategorydescription = categoryDescription from codes_contractcategory where categoryId = @ID;
		
		UPDATE codes_contractcategory SET categoryDescription = @categoryDescription, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE categoryId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Contract Category ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @categoryDescription + ')';

		if @oldcategorydescription <> @categoryDescription
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 109, @ID, '49ACE124-585C-45BB-B02D-F43E9B0C876F', @oldcategorydescription, @categoryDescription, @recordtitle, @subAccountId;
	END
END

RETURN @retVal;
