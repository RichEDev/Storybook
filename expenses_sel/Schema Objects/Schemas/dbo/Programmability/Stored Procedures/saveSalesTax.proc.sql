CREATE PROCEDURE [dbo].[saveSalesTax] 
(
@ID int, 
@subAccountId int,
@description nvarchar(50), 
@salesTax decimal(18,2),
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_salestax WHERE [description] = @description);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_salestax (subAccountId, [description], salesTax, archived, createdOn, createdBy)
		VALUES (@subAccountId, @description, @salesTax, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Sales Tax ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 114, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_salestax WHERE salesTaxId <> @ID AND [description] = @description);
	
	IF @count = 0
	BEGIN
		DECLARE @olddescription nvarchar(50);
		DECLARE @oldsalesTax nvarchar(150);
		
		select @olddescription = [description], @oldsalesTax = salesTax from codes_salesTax where salesTaxId = @ID;
		
		UPDATE codes_salestax SET [description] = @description, salesTax = @salesTax, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE salesTaxId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Sales Tax ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';

		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 114, @ID, '7B716399-889B-41E1-8BCC-BB0B87989D21', @olddescription, @description, @recordtitle, null;
		if @oldsalesTax <> @salesTax
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 114, @ID, '86EE822F-032A-4D31-A34C-BEB89DACF5CF', @oldsalesTax, @salesTax, @recordtitle, null;
	END
END

RETURN @retVal;
