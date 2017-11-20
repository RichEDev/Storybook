CREATE PROCEDURE [dbo].[saveTermType] 
(
@ID int, 
@subAccountId int, 
@termTypeDescription nvarchar(50), 
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_termtype WHERE termTypeDescription = @termTypeDescription AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_termtype (subAccountId, termTypeDescription, archived, createdOn, createdBy)
		VALUES (@subAccountId, @termTypeDescription, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Term Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @termTypeDescription + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 113, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_termtype WHERE termTypeId <> @ID AND subAccountId = @subAccountId AND termTypeDescription = @termTypeDescription);
	
	IF @count = 0
	BEGIN
		DECLARE @oldtypedescription nvarchar(50);
		
		select @oldtypedescription = termTypeDescription from codes_termtype where termTypeId = @ID;
		
		UPDATE codes_termtype SET termTypeDescription = @termTypeDescription, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE termTypeId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Term Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @termTypeDescription + ')';

		if @oldtypedescription <> @termTypeDescription
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 113, @ID, '707067D6-6632-43DF-BE2A-3CB78A272A33', @oldtypedescription, @termTypeDescription, @recordtitle, @subAccountId;
	END
END

RETURN @retVal;

