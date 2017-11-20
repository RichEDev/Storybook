CREATE PROCEDURE [dbo].[SaveOrganisation]
	@OrganisationID INT,
	@Name NVARCHAR(256),
	@ParentOrganisationID INT,
	@PrimaryAddressID INT,
	@Comment NVARCHAR(4000),
	@Code NVARCHAR(60),
	@UserID INT, 
	@DelegateID INT
AS

DECLARE @CurrentUser INT = COALESCE(@DelegateID, @UserID);
DECLARE @RecordTitle NVARCHAR(MAX);

IF (@OrganisationID = 0)
BEGIN -- Insert New Record
	IF EXISTS (SELECT * FROM [dbo].[organisations] WHERE [OrganisationName] = @Name)
	BEGIN
		RETURN -1;
	END
 
	INSERT INTO [dbo].[organisations]
	(
		[OrganisationName],
		[ParentOrganisationID],
		[PrimaryAddressID],
		[Comment],
		[Code],
		[IsArchived],
		[CreatedOn],
		[CreatedBy]
	)
	VALUES
	(
		@Name,
		@ParentOrganisationID,
		@PrimaryAddressID,
		@Comment,
		@Code,
		0,
		GETUTCDATE(),
		@CurrentUser
	)
 
	SET @OrganisationID = SCOPE_IDENTITY();
 
	EXEC addInsertEntryToAuditLog @UserID, @DelegateID, 180, @OrganisationID, @Name, null;
END

ELSE --Update Existing Record
BEGIN
	IF EXISTS (SELECT * FROM [dbo].[organisations] WHERE [OrganisationName] = @Name AND [OrganisationID] <> @OrganisationID)
	BEGIN
		RETURN -1;
	END

	-- Create a backup of existing data
	DECLARE @oldName nvarchar(256);
	DECLARE @oldParentOrganisationID int;
	DECLARE @oldParentOrganisation NVARCHAR(MAX) = '';
	DECLARE @parentOrganisation NVARCHAR(MAX) = '';
	DECLARE @oldPrimaryAddressID int;
	DECLARE @oldPrimaryAddress NVARCHAR(MAX) = '';
	DECLARE @primaryAddress NVARCHAR(MAX) = '';
	DECLARE @oldComment nvarchar(4000);
	DECLARE @oldCode nvarchar(60);

	SELECT @oldName = OrganisationName,
		@oldParentOrganisationID = ParentOrganisationID,
		@oldPrimaryAddressID = PrimaryAddressID,
		@oldComment = Comment,
		@oldCode = Code
	FROM [organisations]
	WHERE [OrganisationID] = @OrganisationID;
	
	IF @oldParentOrganisationID > 0
	BEGIN
		SELECT @oldParentOrganisation = [OrganisationName]
		FROM [dbo].[organisations]
		WHERE [OrganisationID] = @oldParentOrganisationID;
	END
	
	IF @oldPrimaryAddressID > 0
	BEGIN
		SELECT @oldPrimaryAddress = [Line1] + ', ' + [City] + ', ' + [Postcode]
		FROM [dbo].[addresses]
		WHERE [AddressID] = @oldPrimaryAddressID;
	END

	-- Perform the update
	UPDATE [dbo].[organisations]
	SET  OrganisationName = @Name,
		ParentOrganisationID = @ParentOrganisationID,
		PrimaryAddressID = @PrimaryAddressID,
		Comment = @Comment,
		Code = @Code,
		ModifiedOn = GETUTCDATE(),
		ModifiedBy = @CurrentUser
	WHERE [OrganisationID] = @OrganisationID;

	IF @ParentOrganisationID > 0
	BEGIN
		SELECT @parentOrganisation = [OrganisationName]
		FROM [dbo].[organisations]
		WHERE [OrganisationID] = @ParentOrganisationID;
	END
	
	IF @PrimaryAddressID > 0
	BEGIN
		SELECT @primaryAddress = [Line1] + ', ' + [City] + ', ' + [Postcode]
		FROM [dbo].[addresses]
		WHERE [AddressID] = @PrimaryAddressID;
	END

	if (@oldName <> @Name)
		exec addUpdateEntryToAuditLog @userid, @delegateID, 180, @OrganisationID, '4d0f2409-0705-4f0f-9824-42057b25aebe', @oldName, @Name, @Name;

	if (@oldParentOrganisationID <> @ParentOrganisationID)
		exec addUpdateEntryToAuditLog @userid, @delegateID, 180, @OrganisationID, '9bb72dfe-cafd-459c-af10-97ffcfa1b06f', @oldParentOrganisation, @parentOrganisation, @Name;

	if (@oldPrimaryAddressID <> @PrimaryAddressID)
		exec addUpdateEntryToAuditLog @userid, @delegateID, 180, @OrganisationID, 'B74065BB-6D13-464F-92E8-D6A49AE2F65E', @oldPrimaryAddress, @primaryAddress, @Name;

	if (@oldComment <> @Comment)
		exec addUpdateEntryToAuditLog @userid, @delegateID, 180, @OrganisationID, '0d08813c-00e3-45eb-bdcc-0ea512615ade', @oldComment, @Comment, @Name;

	if (@oldCode <> @Code)
		exec addUpdateEntryToAuditLog @userid, @delegateID, 180, @OrganisationID, 'ac87c4c4-9107-4555-b2a3-27109b3ebfbb', @oldCode, @Code, @Name;

END

return @OrganisationID;