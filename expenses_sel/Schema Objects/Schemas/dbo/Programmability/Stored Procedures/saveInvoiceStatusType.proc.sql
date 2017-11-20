CREATE PROCEDURE [dbo].[saveInvoiceStatusType] 
(
@ID int, 
@subAccountId int, 
@description nvarchar(50), 
@isArchive bit,
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM invoiceStatusType WHERE description = @description AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO invoiceStatusType (subAccountId, description, isArchive, archived, createdOn, createdBy)
		VALUES (@subAccountId, @description, @isArchive, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Invoice Status ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 131, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM invoiceStatusType WHERE invoiceStatusTypeId <> @ID AND subAccountId = @subAccountId AND description = @description);
	
	IF @count = 0
	BEGIN
		DECLARE @olddescription nvarchar(50);
		DECLARE @oldIsArchive smallint;
				
		select @olddescription = description, @oldIsArchive = isArchive from invoiceStatusType where invoiceStatusTypeId = @ID;
		
		UPDATE invoiceStatusType SET description = @description, isArchive = @isArchive, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE invoiceStatusTypeId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Invoice Status ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';

		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 131, @ID, '1634E57F-FF62-4870-8BF1-B6317F33D92F', @olddescription, @description, @recordtitle, @subAccountId;

		if @oldIsArchive <> @isArchive
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 131, @ID, 'DD2F7732-A9B9-4D80-BE5C-34F093DCF268', @oldIsArchive, @isArchive, @recordtitle, @subAccountId;
	END
END

RETURN @retVal;
