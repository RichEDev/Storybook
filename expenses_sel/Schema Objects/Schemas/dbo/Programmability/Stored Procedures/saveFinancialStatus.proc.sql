CREATE PROCEDURE [dbo].[saveFinancialStatus]
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
	SET @count = (SELECT COUNT(*) FROM financial_status WHERE [description] = @description AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO financial_status (subAccountId, [description], archived, createdOn, createdBy)
		VALUES (@subAccountId, @description, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Financial Status ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 129, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM financial_status WHERE statusId <> @ID AND subAccountId = @subAccountId AND [description] = @description);
	
	IF @count = 0
	BEGIN
		DECLARE @oldDescription nvarchar(30);
		
		select @oldDescription = [description] from financial_status where statusId = @ID;
		
		UPDATE financial_status SET description = @description, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE statusId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Contract Status ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';

		if @oldDescription <> @description
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 129, @ID, 'b26fe20b-1c3a-41f3-a648-31632ac1164d', @oldDescription, @description, @recordtitle, @subAccountId;
			
	END
END

RETURN @retVal;

