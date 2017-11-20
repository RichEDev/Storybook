CREATE PROCEDURE [dbo].[SaveAccessRolesLink]
	@CUemployeeID int,
	@CUdelegateID int,
	@accessRoleID int,
	@accessRoles AccessRoleElement readonly
AS
	declare @roleName nvarchar(200) = (select rolename from accessRoles WHERE roleID = @accessRoleID);
	declare @primaryAccessRoleID int;
	declare @secondaryAccessRoleID int;
	declare @title nvarchar(max) = '';
	declare @recordTitle nvarchar(max);
	
	-- Look for items that have been removed from the list.

	declare lp cursor for
			select secondaryAccessRoleID from accessRolesLink where primaryAccessRoleID = @accessRoleID and secondaryAccessRoleID not in (select elementID from @accessRoles where elementType = 0)
			open lp
			fetch next from lp into @secondaryAccessRoleID
			while @@FETCH_STATUS = 0
			BEGIN
				set @recordTitle = (select rolename from accessroles where roleid = @secondaryAccessRoleID)
				set @title = @roleName + ' - Linked Role - ' + @recordTitle;
				delete from accessRolesLink where primaryAccessRoleID = @accessRoleID and secondaryAccessRoleID = @secondaryAccessRoleID
				exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 6, @secondaryAccessRoleID, @title, null
				fetch next from lp into @secondaryAccessRoleID
			END
			close lp;
			deallocate lp;

	-- Update new entries
	declare lp cursor for
			select elementID from @accessRoles
			open lp
			fetch next from lp into @secondaryAccessRoleID
			while @@FETCH_STATUS = 0
			BEGIN --1
				insert into accessRolesLink (primaryAccessRoleID, secondaryAccessRoleID) 
					VALUES (@accessRoleID,@secondaryAccessRoleID )
					select @recordTitle = (select rolename from accessroles where roleid = @secondaryAccessRoleID)
				set @title = @roleName + ' - Linked Role - ' + @recordTitle;
				EXEC addInsertEntryWithValueToAuditLog  @CUemployeeID, @CUdelegateID, 6, @secondaryAccessRoleID, @title, null, @recordTitle
				fetch next from lp into @secondaryAccessRoleID
			END



RETURN 0