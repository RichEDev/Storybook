CREATE PROCEDURE [dbo].[ArchiveOrganisation]
 @OrganisationID INT,
 @UserID INT, 
 @DelegateID INT
AS

IF (@OrganisationID > 0)
BEGIN
 -- Get and flip the current archived status
 DECLARE @Archived BIT;
 DECLARE @NewArchived BIT;
 DECLARE @SubAccountID BIT;
 DECLARE @Name NVARCHAR(MAX) = '';
 SELECT @Name = [OrganisationName], @Archived = [IsArchived] FROM [dbo].[organisations] WHERE [OrganisationID] = @OrganisationID;
 SELECT @NewArchived = @Archived ^ 1;
 
 -- Perform the update
 UPDATE [dbo].[organisations]
 SET  [IsArchived] = @NewArchived,
   [ModifiedOn] = GETUTCDATE(),
   [ModifiedBy] = COALESCE(@DelegateID, @UserID)
 WHERE [OrganisationID] = @OrganisationID;

 -- Update the audit log  
 if @Archived = 1
 EXEC addUpdateEntryToAuditLog @UserID, @DelegateID, 180, @OrganisationID, '4b7873d6-8edc-44d4-94f7-b8abcbd87692', 'Archived', 'Live', @Name, null;
 else
 EXEC addUpdateEntryToAuditLog @UserID, @DelegateID, 180, @OrganisationID, '4b7873d6-8edc-44d4-94f7-b8abcbd87692', 'Live', 'Archived', @Name, null;

 RETURN 1;
END
ELSE
BEGIN
 RETURN -1;
END