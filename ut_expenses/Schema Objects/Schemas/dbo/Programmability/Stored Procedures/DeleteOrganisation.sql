CREATE PROCEDURE [dbo].[DeleteOrganisation]
	@OrganisationID INT,
	@UserID INT,
	@DelegateID INT
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [dbo].[organisations] WHERE [OrganisationID] = @OrganisationID)
	BEGIN
		RETURN -1;
	END

	IF EXISTS (SELECT * FROM [dbo].[savedexpenses] WHERE [organisationIdentifier] = @OrganisationID)
	BEGIN
		RETURN -2;
	END

	IF EXISTS (SELECT * FROM [dbo].[organisations] WHERE [ParentOrganisationID] = @OrganisationID)
	BEGIN
		RETURN -3;
	END

	DECLARE @returnCode INT;
	DECLARE @tableid UNIQUEIDENTIFIER = (SELECT [tableid] FROM [tables] WHERE [tablename] = 'organisations');
	-- greenlight reference check
	EXEC @returnCode = dbo.checkReferencedBy @tableid, @OrganisationID;

	IF @returnCode = 0
	BEGIN
		DECLARE @recordTitle NVARCHAR(MAX) = '';
		SELECT @recordTitle = [OrganisationName] FROM [dbo].[organisations] WHERE [OrganisationID] = @OrganisationID;
    
		DELETE FROM	[dbo].[organisationAddresses] WHERE [OrganisationID] = @OrganisationID;
		DELETE FROM [dbo].[organisations] WHERE [OrganisationID] = @OrganisationID;
    
		EXEC addDeleteEntryToAuditLog @UserID, @DelegateID, 180, @OrganisationID, @recordTitle, NULL;
	END

	RETURN @returnCode;
END