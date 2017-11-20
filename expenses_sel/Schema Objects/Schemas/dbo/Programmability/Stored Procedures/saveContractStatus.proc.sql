CREATE PROCEDURE [dbo].[saveContractStatus]
(
@ID int, 
@subAccountId int, 
@statusDescription nvarchar(30), 
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
	SET @count = (SELECT COUNT(*) FROM codes_contractstatus WHERE statusDescription = @statusDescription AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_contractstatus (subAccountId, statusDescription, isArchive, archived, createdOn, createdBy)
		VALUES (@subAccountId, @statusDescription, @isArchive, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Contract Status ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @statusDescription + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 111, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_contractstatus WHERE statusId <> @ID AND subAccountId = @subAccountId AND statusDescription = @statusDescription);
	
	IF @count = 0
	BEGIN
		DECLARE @oldstatusDescription nvarchar(30);
		DECLARE @oldisArchive bit;
		
		select @oldstatusDescription = statusDescription, @oldisArchive = isArchive from codes_contractstatus where statusId = @ID;
		
		UPDATE codes_contractstatus SET statusDescription = @statusDescription, isArchive = @isArchive, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE statusId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Contract Status ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @statusDescription + ')';

		if @oldstatusDescription <> @statusDescription
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 111, @ID, '309174D1-72C9-4E08-9406-EACC7C97B9BF', @oldstatusDescription, @statusDescription, @recordtitle, @subAccountId;
			
		if @oldisArchive <> @isArchive
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 111, @ID, '9058A55E-6648-4FC3-9E48-BF3E8C7E542C', @oldisArchive, @isArchive, @recordtitle, @subAccountId;
	END
END

RETURN @retVal;
