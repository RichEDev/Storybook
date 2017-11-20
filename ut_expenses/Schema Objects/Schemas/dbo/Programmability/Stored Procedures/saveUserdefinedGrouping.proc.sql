CREATE PROCEDURE [dbo].[saveUserdefinedGrouping]
@userdefinedGroupID int,
@groupName nvarchar(50),
@order INT,
@associatedTable uniqueidentifier,
@date DateTime,
@userid INT,
@CUemployeeID INT,
@CUdelegateID INT
AS

DECLARE @count INT;

if @userdefinedGroupID = 0
BEGIN
	SET @count = (SELECT COUNT(*) FROM userdefinedGroupings WHERE associatedTable = @associatedtable AND groupName = @groupName);
	IF @count > 0
		RETURN -1;
	
	IF @order = 0 AND EXISTS (SELECT * FROM userdefinedGroupings WHERE associatedTable = @associatedTable)
	BEGIN
		SET @order = (SELECT TOP 1 userdefinedGroupings.[order]+1 FROM userdefinedGroupings WHERE associatedTable=@associatedTable ORDER BY [order] DESC);
	END
	ELSE IF @order = 0
	BEGIN
		SET @order = 1;
	END
	
	INSERT INTO userDefinedGroupings (associatedTable, groupName, [order], createdon, createdby) VALUES (@associatedTable, @groupName, @order, @date, @userid);
	set @userdefinedGroupID = scope_identity()
	
	EXEC addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 56, @userdefinedGroupID, @groupName, null
end
else
BEGIN
	SET @count = (SELECT COUNT(*) FROM userdefinedGroupings WHERE associatedTable = @associatedTable AND groupName = @groupName AND userdefinedGroupID <> @userdefinedGroupID);
	IF @count > 0
		RETURN -1;

	declare @oldgroupName nvarchar(50);
	declare @oldorder INT;
	declare @oldassociatedTable uniqueidentifier;
	select @oldassociatedTable = associatedTable, @oldgroupName = groupName, @oldorder = [order] from userdefinedGroupings WHERE [userdefinedGroupID] = @userdefinedGroupID;
	
	UPDATE userdefinedGroupings SET associatedTable = @associatedTable, groupName = @groupName, [ModifiedOn] = @date, modifiedby = @userid WHERE [userdefinedGroupID] = @userdefinedGroupID;
	
	if @oldgroupName <> @groupName
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 56, @userdefinedGroupID, 'f2489d12-eb85-41ac-9c99-38ebfe9f6f86', @oldgroupName, @groupName, @groupName, null;
	if @oldorder <> @order
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 56, @userdefinedGroupID, 'b4d8c2a8-7136-491b-a094-ec61f9fdbd88', @oldorder, @order, @groupName, null;
	if @oldassociatedTable <> @associatedTable
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 56, @userdefinedGroupID, 'afab318f-8739-4f2e-9420-51aa0df39dc4', @oldassociatedTable, @associatedTable, @groupName, null;
end

return @userdefinedGroupID
