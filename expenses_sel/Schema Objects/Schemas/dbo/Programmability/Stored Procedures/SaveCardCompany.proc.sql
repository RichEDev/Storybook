
CREATE PROCEDURE [dbo].[SaveCardCompany] 
	@cardCompanyID int,
	@companyName nvarchar(80),
	@companyNumber nvarchar(80),
	@usedForImport bit,
	@UserID int,
	@DelegateID int
AS

DECLARE @recordTitle nvarchar(2000);

BEGIN
	IF @cardCompanyID = 0
	BEGIN
		INSERT INTO cardCompanies (companyName, companyNumber, usedForImport, createdOn, createdBy)
		VALUES (@companyName, @companyNumber, @usedForImport, GETUTCDATE(), @UserID);
		SET @cardCompanyID = SCOPE_IDENTITY();
		
		SET @recordTitle = 'Card Company: ' + CAST(@cardCompanyID AS nvarchar(20)) + ' (' + @companyName + ')';
		EXEC addInsertEntryToAuditLog @UserID, @DelegateID, 160, @cardCompanyID, @recordTitle, null;
	
	END
	ELSE
	BEGIN
		DECLARE @OldUsedForImport bit;
		
		SET @OldUsedForImport = (SELECT usedForImport FROM cardCompanies WHERE cardCompanyID = @cardCompanyID);
		
		UPDATE cardCompanies SET usedForImport = @usedForImport, modifiedOn = GETUTCDATE(), modifiedBy = @UserID WHERE cardCompanyID = @cardCompanyID;
		
		SET @recordTitle = 'Card Company: ' + CAST(@cardCompanyID AS nvarchar(20)) + ' (' + @companyName + ')';
		
		if @OldUsedForImport <> @usedForImport
			exec addUpdateEntryToAuditLog @UserID, @DelegateID, 160, @cardCompanyID, '0be46022-04fd-4ed9-a5b5-1847460dec93', @OldUsedForImport, @usedForImport, @recordtitle, null;
	END
	
	RETURN @cardCompanyID
END
