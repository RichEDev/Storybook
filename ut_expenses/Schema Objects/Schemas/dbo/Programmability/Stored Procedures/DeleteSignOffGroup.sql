CREATE PROCEDURE [dbo].[DeleteSignOffGroup]
@groupId INT,
@modifiedBy INT,
@delegateID INT

AS
DECLARE @count INT;
DECLARE @elementId INT; 
DECLARE @recordTitle NVARCHAR(2000);	

BEGIN
	
    SET @count = (SELECT count(groupid) FROM employees WHERE groupid = @groupId)
	
	IF @count > 0
	RETURN 1;

	SET @count = (SELECT count(advancegroupid) FROM employees WHERE advancegroupid = @groupId)

	IF @count > 0
	RETURN 2;

	SELECT @elementId = elementID FROM [Elements] WHERE elementFriendlyName = 'Groups'
	SET @recordTitle = (SELECT groupname FROM groups WHERE groupid = @groupId)
    DELETE FROM groups WHERE groupid = @groupId;
	EXEC addDeleteEntryToAuditLog @modifiedBy, @delegateID, @elementId, @groupId, @recordTitle, null;

	RETURN 0;
	
END