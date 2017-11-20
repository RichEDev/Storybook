
CREATE PROCEDURE [dbo].[saveCustomEntityView] @viewid INT
	,@entityid INT
	,@viewname NVARCHAR(100)
	,@description NVARCHAR(4000)
	,@date DATETIME
	,@userid INT
	,@menuid INT
	,@MenuDescription NVARCHAR(4000)
	,@showRecordCount BIT
	,@allowadd BIT
	,@addformid INT
	,@allowedit BIT
	,@editformid INT
	,@allowdelete BIT
	,@SortColumn UNIQUEIDENTIFIER
	,@SortOrder TINYINT
	,@SortJoinViaID INT
	,@AllowApproval BIT
	,@MenuIcon NVARCHAR(100)
	,@BuiltIn BIT
	,@SystemCustomEntityViewId UNIQUEIDENTIFIER = NULL
	,@allowarchive BIT
AS
BEGIN
	DECLARE @count INT;

	IF (@viewid = 0)
	BEGIN
	
		SELECT @count = COUNT(1)
		FROM [customEntityViews]
		WHERE entityid = @entityid
			AND view_name = @viewname

		IF @count > 0
			RETURN - 1;

		IF @builtIn = 1 AND @SystemCustomEntityViewId IS NULL
			SET @SystemCustomEntityViewId = NEWID()

		INSERT INTO [customEntityViews] (
			entityid
			,view_name
			,[description]
			,createdby
			,createdon
			,menuid
			,[allowadd]
			,[add_formid]
			,[allowedit]
			,[edit_formid]
			,[allowdelete]
			,[SortColumn]
			,[SortOrder]
			,[SortColumnJoinViaID]
			,[allowapproval]
			,[MenuDescription]
			,[MenuIcon]
			,[BuiltIn]
			,[SystemCustomEntityViewId]
			,[CacheExpiry]
			,ShowRecordCount
			,Allowarchive
			)
		VALUES (
			@entityid
			,@viewname
			,@description
			,@userid
			,@date
			,@menuid
			,@allowadd

			,@addformid
			,@allowedit
			,@editformid
			,@allowdelete
			,@SortColumn
			,@SortOrder
			,@SortJoinViaID
			,@AllowApproval
			,@MenuDescription
			,@MenuIcon
			,@BuiltIn
			,@SystemCustomEntityViewId
			,GETUTCDATE()
			,@showRecordCount
			,@allowarchive
			)

		SET @viewid = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN

		SELECT @count = COUNT(1) FROM [customEntityViews] WHERE entityid = @entityid AND view_name = @viewname AND viewid <> @viewid
		IF @count > 0
			RETURN -1;

		DECLARE @OldBuiltIn BIT
		DECLARE @OldSystemCustomEntityViewId UNIQUEIDENTIFIER

		SELECT @OldBuiltIn = BuiltIn
			,@OldSystemCustomEntityViewId = dbo.customEntityViews.SystemCustomEntityViewId
		FROM [customEntityViews]
		WHERE viewid = @viewid

		IF @OldBuiltIn = 1 AND @BuiltIn = 0
			SET @BuiltIn = 1;

		IF @builtIn = 1	AND @SystemCustomEntityViewId IS NULL AND @OldSystemCustomEntityViewId IS NULL
			SET @SystemCustomEntityViewId = NEWID()

		UPDATE [customEntityViews]
		SET view_name = @viewname
			,[description] = @description
			,modifiedby = @userid
			,modifiedon = @date
			,menuid = @menuid
			,allowadd = @allowadd
			,[add_formid] = @addformid
			,[allowedit] = @allowedit
			,[edit_formid] = @editformid
			,[allowdelete] = @allowdelete
			,SortColumn = @SortColumn
			,SortOrder = @SortOrder
			,[SortColumnJoinViaID] = @SortJoinViaID
			,allowapproval = @AllowApproval
			,[MenuDescription] = @MenuDescription
			,[MenuIcon] = @MenuIcon
			,[BuiltIn] = @BuiltIn
			,[SystemCustomEntityViewId] = @SystemCustomEntityViewId
			,[CacheExpiry] = GETUTCDATE()
			,ShowRecordCount = @showRecordCount
			,allowarchive=@allowarchive
		WHERE viewid = @viewid
	END

	RETURN @viewid
END
GO
