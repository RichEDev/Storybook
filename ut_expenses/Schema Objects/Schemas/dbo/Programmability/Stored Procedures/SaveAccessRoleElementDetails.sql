CREATE PROCEDURE [dbo].[SaveAccessRoleElementDetails]
	@CUemployeeID int,
	@CUdelegateID int,
	@accessRoleID int,
	@accessRoles AccessRoleElement readonly
AS
	declare @roleName nvarchar(200) = (select rolename from accessRoles WHERE roleID = @accessRoleID);
	declare @title nvarchar(max) = '';
	declare @elementID int;
	declare @elementType int;
	declare @entityElementID int;
	declare @viewAccess bit;
	declare @insertAccess bit;
	declare @updateAccess bit;
	declare @deleteAccess bit;
	declare @oldViewAccess bit;
	declare @oldInsertAccess bit;
	declare @OldUpdateAccess bit;
	declare @OldDeleteAccess bit;
	declare @recordTitle nvarchar(max);
	declare @oldValue nvarchar(max);
	declare @newValue nvarchar(max);
	declare @newRole bit = 1;
	IF EXISTS (select elementID from accessRoleElementDetails WHERE roleID = @accessRoleID)
	BEGIN
		SET @newRole = 0;
	END
		-- Look for items that have been removed from the list.

	declare lp cursor for
			select elementID, viewAccess, insertAccess, updateAccess, deleteAccess from accessRoleElementDetails where roleID = @accessRoleID and elementID not in (select elementID from @accessRoles where elementType = 0)
			open lp
			fetch next from lp into @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			while @@FETCH_STATUS = 0
			BEGIN
				select @recordTitle = elementFriendlyName from Elements where elementid = @elementID and elementType = 0
				set @title = @roleName + ' - ' + @recordTitle;
				delete from accessRoleElementDetails where roleID = @accessRoleID and elementID = @elementID
				exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 6, @elementID, @title, null
				fetch next from lp into @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			END
			close lp;
			deallocate lp;
		-- Custom Entity
			declare lp cursor for
			select customEntityID, viewAccess, insertAccess, updateAccess, deleteAccess from accessRoleCustomEntityDetails where roleID = @accessRoleID and customEntityID not in (select elementID from @accessRoles where elementType = 1)
			open lp
			fetch next from lp into @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			while @@FETCH_STATUS = 0
			BEGIN
				select @recordTitle = elementFriendlyName from Elements where elementid = @elementID and elementType = 1
				set @title = @roleName + ' - ' + @recordTitle;
				delete from accessRoleCustomEntityDetails where roleID = @accessRoleID and customEntityID = @elementID and roleID = @accessRoleID
				exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 6, @elementID, @title, null
				fetch next from lp into @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			END
			close lp;
			deallocate lp;
		-- Custom Entity View
			declare lp cursor for
			select accessRoleCustomEntityViewDetailsID, viewAccess, insertAccess, updateAccess, deleteAccess from accessRoleCustomEntityViewDetails where roleID = @accessRoleID and accessRoleCustomEntityViewDetailsID not in 
			(select accessRoleCustomEntityViewDetailsID from accessRoleCustomEntityViewDetails inner join @accessRoles on elementID = customEntityID and entityElementID = customEntityViewID
where roleID = @accessRoleID)

			open lp
			fetch next from lp into @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			while @@FETCH_STATUS = 0
			BEGIN
				select @recordTitle = elementFriendlyName from Elements where elementid = @elementID and elementType = 2
				set @title = @roleName + ' - ' + @recordTitle;
				delete from accessRoleCustomEntityViewDetails where roleID = @accessRoleID and accessRoleCustomEntityViewDetailsID = @elementID
				exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 6, @elementID, @title, null
				fetch next from lp into @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			END
			close lp;
			deallocate lp;
		-- Custom Entity Form
			declare lp cursor for
			select accessRoleCustomEntityFormDetailsID, viewAccess, insertAccess, updateAccess, deleteAccess from accessRoleCustomEntityFormDetails where roleID = @accessRoleID and accessRoleCustomEntityFormDetailsID not in 
			(select accessRoleCustomEntityFormDetailsID from accessRoleCustomEntityFormDetails inner join @accessRoles on elementID = customEntityID and entityElementID = customEntityFormID where roleID  = @accessRoleID)
			open lp
			fetch next from lp into @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			while @@FETCH_STATUS = 0
			BEGIN
				select @recordTitle = elementFriendlyName from Elements where elementid = @elementID and elementType = 3
				set @title = @roleName + ' - ' + @recordTitle;
				delete from accessRoleCustomEntityFormDetails where roleID = @accessRoleID and accessRoleCustomEntityFormDetailsID = @elementID
				exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 6, @elementID, @title, null
				fetch next from lp into @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			END
			close lp;
			deallocate lp;

		--Update / Insert items.

		declare lp cursor for
			select elementID, elementType, entityElementID, viewAccess, insertAccess, updateAccess, deleteAccess from @accessRoles
			open lp
			fetch next from lp into @elementID, @elementType, @entityElementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
			while @@FETCH_STATUS = 0
			BEGIN --1
				
				IF @elementType = 0
				BEGIN --2
					select @recordTitle = elementFriendlyName from Elements where elementid = @elementID and elementType = 0
					IF EXISTS (select * from accessRoleElementDetails where roleID = @accessRoleID and elementID = @elementID)
					BEGIN --3
						set @oldValue = '';
						set @newValue = '';
					
						select @oldViewAccess = viewAccess, @oldInsertAccess = insertAccess, @OldUpdateAccess = updateAccess, @OldDeleteAccess = deleteAccess from accessRoleElementDetails where roleID = @accessRoleID and elementID = @elementID;

						if	@oldViewAccess <> @viewAccess or @oldInsertAccess <> @insertAccess or @OldUpdateAccess <> @updateAccess or @OldDeleteAccess <> @deleteAccess
						BEGIN --4
							if @oldViewAccess = 1 set @oldValue = @oldValue + 'View ';
							if @oldInsertAccess = 1 set @oldValue = @oldValue + 'Insert ';
							if @OldUpdateAccess = 1 set @oldValue = @oldValue + 'Update ';
							if @OldDeleteAccess = 1 set @oldValue = @oldValue + 'Delete ';

							if @viewAccess = 1 set @newValue = @newValue + 'View ';
							if @insertAccess = 1 set @newValue = @newValue + 'Insert ';
							if @updateAccess = 1 set @newValue = @newValue + 'Update ';
							if @deleteAccess = 1 set @newValue = @newValue + 'Delete ';

							set @title = @roleName + ' - ' + @recordTitle;

							exec addUpdateEntryToAuditLog  @CUemployeeID, @CUdelegateID, 6, @elementID, null, @oldValue, @newValue, @title, null 

							update accessRoleElementDetails set viewAccess = @viewAccess, insertAccess = @insertAccess, updateAccess = @updateAccess, deleteAccess = @deleteAccess WHERE roleID = @accessRoleID and elementID = @elementID
						END --4
					END --3
					ELSE 
					BEGIN --3
						insert into accessRoleElementDetails (roleID, elementID, viewAccess, insertAccess, updateAccess, deleteAccess) VALUES (@accessRoleID, @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess)
						set @newValue = '';
						if @viewAccess = 1 set @newValue = @newValue + 'View ';
						if @insertAccess = 1 set @newValue = @newValue + 'Insert ';
						if @updateAccess = 1 set @newValue = @newValue + 'Update ';
						if @deleteAccess = 1 set @newValue = @newValue + 'Delete ';
						set @title = @roleName + ' - ' + @recordTitle;
						if @newRole = 0 and @newValue <> ''
						BEGIN --4
							EXEC addInsertEntryWithValueToAuditLog  @CUemployeeID, @CUdelegateID, 6, @elementID, @title, null, @newValue
						END --4
					END --3
				END --2
				if @elementType = 1  -- Entity
				BEGIN --2
					select @recordTitle = elementFriendlyName from Elements where elementid = @elementID and elementType = 1
					IF EXISTS (select * from accessRoleCustomEntityDetails where roleID = @accessRoleID and customEntityID = @elementID)
					BEGIN --3
						set @oldValue = '';
						set @newValue = '';
						select @oldViewAccess = viewAccess, @oldInsertAccess = insertAccess, @OldUpdateAccess = updateAccess, @OldDeleteAccess = deleteAccess from accessRoleCustomEntityDetails where roleID = @accessRoleID and customEntityID = @elementID 

						if	@oldViewAccess <> @viewAccess or @oldInsertAccess <> @insertAccess or @OldUpdateAccess <> @updateAccess or @OldDeleteAccess <> @deleteAccess
						BEGIN --4
							update accessRoleCustomEntityDetails set viewAccess = @viewAccess, insertAccess = @insertAccess, updateAccess = @updateAccess, deleteAccess = @deleteAccess WHERE roleID = @accessRoleID and customEntityID = @elementID
						END --4
					END --3
					ELSE
					BEGIN --3
						insert into accessRoleCustomEntityDetails (roleID, customEntityID, viewAccess, insertAccess, updateAccess, deleteAccess) VALUES (@accessRoleID, @elementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess)
					END	--3
				END --2
				IF @elementType = 2 -- View
				BEGIN --3
				select @recordTitle = elementFriendlyName from Elements where elementid = @elementID and elementType = 2
					IF EXISTS (select * from accessRoleCustomEntityViewDetails where roleID = @accessRoleID and customEntityID = @elementID and customEntityViewID = @entityElementID)
					BEGIN --3
						set @oldValue = '';
						set @newValue = '';
						select @oldViewAccess = viewAccess, @oldInsertAccess = insertAccess, @OldUpdateAccess = updateAccess, @OldDeleteAccess = deleteAccess from accessRoleCustomEntityViewDetails 
							where roleID = @accessRoleID and customEntityID = @elementID and customEntityViewID = @entityElementID

						if	@oldViewAccess <> @viewAccess or @oldInsertAccess <> @insertAccess or @OldUpdateAccess <> @updateAccess or @OldDeleteAccess <> @deleteAccess
						BEGIN --4
							if @oldViewAccess = 1 set @oldValue = @oldValue + 'View ';
							if @oldInsertAccess = 1 set @oldValue = @oldValue + 'Insert ';
							if @OldUpdateAccess = 1 set @oldValue = @oldValue + 'Update ';
							if @OldDeleteAccess = 1 set @oldValue = @oldValue + 'Delete ';

							if @viewAccess = 1 set @newValue = @newValue + 'View ';
							if @insertAccess = 1 set @newValue = @newValue + 'Insert ';
							if @updateAccess = 1 set @newValue = @newValue + 'Update ';
							if @deleteAccess = 1 set @newValue = @newValue + 'Delete ';

							set @title = @roleName + ' - ' + @recordTitle;

							exec addUpdateEntryToAuditLog  @CUemployeeID, @CUdelegateID, 6, @elementID, null, @oldValue, @newValue, @title, null 

							update accessRoleCustomEntityViewDetails set viewAccess = @viewAccess, insertAccess = @insertAccess, updateAccess = @updateAccess, deleteAccess = @deleteAccess 
								WHERE roleID = @accessRoleID and customEntityID = @elementID and customEntityViewID = @entityElementID
						END --4
					END --3
					ELSE
					BEGIN --3
						insert into accessRoleCustomEntityViewDetails (roleID, customEntityID, customEntityViewID, viewAccess, insertAccess, updateAccess, deleteAccess) VALUES (@accessRoleID, @elementID, @entityElementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess)
						set @newValue = '';
						if @viewAccess = 1 set @newValue = @newValue + 'View ';
						if @insertAccess = 1 set @newValue = @newValue + 'Insert ';
						if @updateAccess = 1 set @newValue = @newValue + 'Update ';
						if @deleteAccess = 1 set @newValue = @newValue + 'Delete ';
						set @title = @roleName + ' - ' + @recordTitle;
						if @newRole = 0 and @newValue <> ''
						BEGIN --4
							EXEC addInsertEntryWithValueToAuditLog  @CUemployeeID, @CUdelegateID, 6, @elementID, @title, null, @newValue
						END --4
					END	--3
				END --3
				IF @elementType = 3-- Form
				BEGIN --2
				select @recordTitle = elementFriendlyName from Elements where elementid = @elementID and elementType = 2
					IF EXISTS (select * from accessRoleCustomEntityFormDetails where roleID = @accessRoleID and customEntityID = @elementID and customEntityFormID = @entityElementID)
					BEGIN --3
						set @oldValue = '';
						set @newValue = '';
						select @oldViewAccess = viewAccess, @oldInsertAccess = insertAccess, @OldUpdateAccess = updateAccess, @OldDeleteAccess = deleteAccess from accessRoleCustomEntityFormDetails 
							where roleID = @accessRoleID and customEntityID = @elementID and customEntityFormID = @entityElementID

						if	@oldviewAccess <> @viewAccess or @oldInsertAccess <> @insertAccess or @OldUpdateAccess <> @updateAccess or @OldDeleteAccess <> @deleteAccess
						BEGIN --4
							if @oldViewAccess = 1 set @oldValue = @oldValue + 'Form ';
							if @oldInsertAccess = 1 set @oldValue = @oldValue + 'Insert ';
							if @OldUpdateAccess = 1 set @oldValue = @oldValue + 'Update ';
							if @OldDeleteAccess = 1 set @oldValue = @oldValue + 'Delete ';

							if @viewAccess = 1 set @newValue = @newValue + 'Form ';
							if @insertAccess = 1 set @newValue = @newValue + 'Insert ';
							if @updateAccess = 1 set @newValue = @newValue + 'Update ';
							if @deleteAccess = 1 set @newValue = @newValue + 'Delete ';

							set @title = @roleName + ' - ' + @recordTitle;

							exec addUpdateEntryToAuditLog  @CUemployeeID, @CUdelegateID, 6, @elementID, null, @oldValue, @newValue, @title, null 

							update accessRoleCustomEntityFormDetails set viewAccess = @viewAccess, insertAccess = @insertAccess, updateAccess = @updateAccess, deleteAccess = @deleteAccess 
								WHERE roleID = @accessRoleID and customEntityID = @elementID and customEntityFormID = @entityElementID
						END --4
					END --3
					ELSE
					BEGIN --3
						insert into accessRoleCustomEntityFormDetails (roleID, customEntityID, customEntityFormID, viewAccess, insertAccess, updateAccess, deleteAccess) VALUES (@accessRoleID, @elementID, @entityElementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess)
						set @newValue = '';
						if @viewAccess = 1 set @newValue = @newValue + 'Form ';
						if @insertAccess = 1 set @newValue = @newValue + 'Insert ';
						if @updateAccess = 1 set @newValue = @newValue + 'Update ';
						if @deleteAccess = 1 set @newValue = @newValue + 'Delete ';
						set @title = @roleName + ' - ' + @recordTitle;
						if @newRole = 0 and @newValue <> ''
						BEGIN --4
							EXEC addInsertEntryWithValueToAuditLog  @CUemployeeID, @CUdelegateID, 6, @elementID, @title, null, @newValue
						END --4
					END	--3
				END --3
				fetch next from lp into @elementID, @elementType, @entityElementID, @viewAccess, @insertAccess, @updateAccess, @deleteAccess
				
			END --1
			close lp;
			deallocate lp;

	


RETURN 0
