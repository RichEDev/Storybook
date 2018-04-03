CREATE PROCEDURE [dbo].[SaveItemRole]
	@itemRoleId int,
	@rolename nvarchar(50),
	@description nvarchar(4000),
	@employeeId INT,
	@delegateId INT
AS
BEGIN
	
	DECLARE @count int
	if @itemRoleId > 0
		BEGIN
			SET @count = (SELECT count(itemroleid) FROM item_roles WHERE rolename = @rolename and itemroleid <> @itemroleid)
			if @count > 0
				return -1

				DECLARE @oldRoleName nvarchar(50);
				DECLARE @oldDescription nvarchar(4000);

				SELECT @oldRoleName = rolename, @oldDescription = description FROM item_roles WHERE itemroleid = @itemRoleId

			UPDATE item_roles SET rolename = @rolename, description = @description, modifiedon = getDate(), modifiedby = @employeeId WHERE itemroleid = @itemroleid

			if @employeeID > 0
				BEGIN
					if @oldRoleName <> @roleName
						EXEC addUpdateEntryToAuditLog @employeeID, @delegateID, 37, @itemRoleId, '54825039-9125-4705-b2d4-eb340d1d30de', @oldRoleName, @roleName, @roleName, null;
					if @olddescription <> @description
						EXEC addUpdateEntryToAuditLog @employeeID, @delegateID, 37, @itemRoleId, 'dcc4c3e7-1ed8-40b9-94bc-f5c52897fd86', @olddescription, @description, @RoleName, null;
				END
		END
	else
		BEGIN
			SET @count = (SELECT count(itemroleid) FROM item_roles WHERE rolename = @rolename)
			if @count > 0
				return -1

			INSERT INTO item_roles (rolename, description, createdon, createdby)
				VALUES (@rolename,@description, getDate(), @employeeId);
			SET @itemRoleId = scope_identity()

			if @employeeID > 0
				BEGIN
					EXEC addInsertEntryToAuditLog @employeeID, @delegateID, 37, @itemRoleId, @rolename, null;
				END
		END

		return @itemRoleId
	END
