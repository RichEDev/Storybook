
CREATE PROCEDURE [dbo].[saveSubAccount]
(
@ID int, --ID of the sub account being saved
@subAccountId int, --ID of the sub account the user is logged in under
@description nvarchar(50),
@associatedSubAccountID int,
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);
DECLARE @accessRoleID INT;

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM accountsSubAccounts WHERE [description] = @description);
	
	IF @count = 0
	BEGIN
		INSERT INTO accountsSubAccounts ([description], archived, createdOn, createdBy)
		VALUES (@description, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		IF @retVal > -1
		BEGIN
			--Set the employee adding the sub account with all the access roles that are attached to 
			--the currently logged in sub account
			DECLARE loop_cursor CURSOR FOR
			SELECT accessRoleID FROM employeeAccessRoles WHERE subAccountID = @subAccountId
			OPEN loop_cursor
			FETCH NEXT FROM loop_cursor INTO @accessRoleID
			WHILE @@FETCH_STATUS = 0
			BEGIN
				exec saveEmployeeAccessRoles @employeeId, @retVal, @accessRoleID, @employeeId, @delegateID
				FETCH NEXT FROM loop_cursor INTO @accessRoleID
			END
			CLOSE loop_cursor
			DEALLOCATE loop_cursor
			
			--Add sub-account account properties the same as the ones in the associated sub-account
			
			INSERT INTO [dbo].[accountProperties]
			   ([subAccountID]
			   ,[stringKey]
			   ,[stringValue]
			   ,[createdOn]
			   ,[createdBy]
			   ,[modifiedOn]
			   ,[modifiedBy]
			   ,[formPostKey]
			   ,[isGlobal])
			SELECT @retVal
			  ,[stringKey]
			  ,[stringValue]
			  ,[createdOn]
			  ,[createdBy]
			  ,[modifiedOn]
			  ,[modifiedBy]
			  ,[formPostKey]
			  ,[isGlobal]
			FROM [dbo].[accountProperties] WHERE subAccountID = @associatedSubAccountID
			
			set @recordTitle = 'SubAccount ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';
			exec addInsertEntryToAuditLog @employeeId, @delegateID, 132, @retVal, @recordTitle, @subAccountId;
		END
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM accountsSubAccounts WHERE subAccountId <> @ID AND [description] = @description);
	
	IF @count = 0
	BEGIN
		DECLARE @olddescription nvarchar(50);
				
		select @olddescription = [description] from accountsSubAccounts where subAccountID = @ID;
		
		UPDATE accountsSubAccounts SET [description] = @description, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE subAccountID = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'SubAccount ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @description + ')';

		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 132, @retVal, '2D43703F-7FF2-43E2-BC4D-2BAE727A25F8', @olddescription, @description, @recordtitle, @subAccountId;
	END
END

RETURN @retVal;
