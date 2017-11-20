
CREATE PROCEDURE [dbo].[saveCustomEntity] @entityid INT
	,@entityname NVARCHAR(250)
	,@pluralname NVARCHAR(250)
	,@description NVARCHAR(4000)
	,@enableattachments BIT
	,@audienceViewType TINYINT
	,@allowdocmergeaccess BIT
	,@enableCurrencies BIT
	,@defaultCurrencyID INT
	,@enablePopupWindow BIT
	,@defaultPopupView INT
	,@formSelectionAttribute INT
	,@ownerId INT
	,@supportContactId INT
	,@supportQuestion NVARCHAR(250)
	,@enableLocking BIT
	,@builtIn BIT
	,@date DATETIME
	,@userid INT
	,@systemCustomEntityId UNIQUEIDENTIFIER = NULL
AS
DECLARE @currencySQL NVARCHAR(4000);
DECLARE @masterTableName NVARCHAR(250);
DECLARE @count INT;
DECLARE @SystemEntityGUID UNIQUEIDENTIFIER = NULL

IF (@entityid = 0)
BEGIN
	SET @count = (
			SELECT COUNT(*)
			FROM [customEntities]
			WHERE entity_name = @entityname
			);

	IF @count > 0
	BEGIN
		-- If the entity name has been used AND the plural name has been used, return -3
		SET @count = (
				SELECT COUNT(*)
				FROM [customEntities]
				WHERE plural_name = @pluralname
				);

		IF @count > 0
		BEGIN
			RETURN - 3;
		END
		ELSE
			-- If only the entity name has been used, return -1
		BEGIN
			RETURN - 1;
		END
	END

	-- If the plural name has been used, return -2
	SET @count = (
			SELECT COUNT(*)
			FROM [customEntities]
			WHERE plural_name = @pluralname
			);

	IF @count > 0
		RETURN - 2;

	-- Give the masterTableName a temporary value to avoid using NULL
	SET @masterTableName = 'MasterTableName_' + CAST(GETUTCDATE() AS NVARCHAR(236));

	IF @builtIn = 1 
		SET @SystemEntityGUID = IsNull(@systemCustomEntityId, NEWID())

	INSERT INTO [customEntities] (
		entity_name
		,plural_name
		,masterTableName
		,[description]
		,createdby
		,createdon
		,enableAttachments
		,audienceViewType
		,allowdocmergeaccess
		,enableCurrencies
		,defaultCurrencyID
		,enablePopupWindow
		,defaultPopupView
		,ownerId
		,supportContactId
		,supportQuestion
		,enableLocking
		,builtIn
		,CacheExpiry
		,dbo.customEntities.SystemCustomEntityID
		)
	VALUES (
		@entityname
		,@pluralname
		,@masterTableName
		,@description
		,@userid
		,@date
		,@enableattachments
		,@audienceViewType
		,@allowdocmergeaccess
		,@enableCurrencies
		,@defaultCurrencyID
		,@enablePopupWindow
		,@defaultPopupView
		,@ownerId
		,@supportContactId
		,@supportQuestion
		,@enableLocking
		,@builtIn
		,GETUTCDATE()
		,@SystemEntityGUID
		)

	SET @entityid = scope_identity();
	SET @masterTableName = @entityid;

	UPDATE [customEntities]
	SET masterTableName = @masterTableName
	WHERE entityid = @entityid;

	DECLARE @SystemEntityAttributeGUID UNIQUEIDENTIFIER = NULL

	IF @builtIn = 1
		SET @SystemEntityAttributeGUID = NEWID()

	INSERT INTO [customEntityAttributes] (
		entityid
		,[display_name]
		,[fieldtype]
		,createdon
		,createdby
		,is_key_field
		,is_audit_identity
		,allowEdit
		,allowDelete
		,[description]
		,system_attribute
		,CacheExpiry
		,dbo.customEntityAttributes.SystemCustomEntityAttributeID
		)
	VALUES (
		@entityid
		,'ID'
		,2
		,@date
		,@userid
		,1
		,1
		,0
		,0
		,'Built in identification field for the GreenLight records'
		,1
		,GETUTCDATE()
		,@SystemEntityAttributeGUID
		);

	DECLARE @attributeid INT;

	SET @attributeid = SCOPE_IDENTITY();


	IF @builtIn = 1
		SET @SystemEntityAttributeGUID = NEWID()

	INSERT INTO [customEntityAttributes] (
		entityid
		,[display_name]
		,[fieldtype]
		,createdon
		,createdby
		,is_key_field
		,is_audit_identity
		,allowEdit
		,allowDelete
		,[description]
		,system_attribute
		,CacheExpiry
		,dbo.customEntityAttributes.SystemCustomEntityAttributeID
		)
	VALUES (
		@entityid
		,'Archived'
		,5
		,@date
		,@userid
		,0
		,0
		,0
		,0
		,'Built in attribute to show if the GreenLight record has been archived'
		,1
		,GETUTCDATE()
		,@SystemEntityAttributeGUID
		);

	
	SET @currencySQL = '';

	IF @enableCurrencies = 1
	BEGIN
		SET @currencySQL = '[GreenLightCurrency] [INT] NULL,';
	END

	DECLARE @sql NVARCHAR(4000)

	SET @sql = 'CREATE TABLE [dbo].[custom_' + @masterTableName + '](' + '[att' + cast(@attributeid AS NVARCHAR) + '] [int] IDENTITY(1,1) NOT NULL,' + '[CreatedOn] [datetime] NULL,' + '[CreatedBy] [int] NULL,' + '[ModifiedOn] [datetime] NULL,' + '[ModifiedBy] [int] NULL,' + '[Archived] [bit] DEFAULT 0 NOT NULL,' + @currencySQL + 'CONSTRAINT [PK_' + @masterTableName + '] PRIMARY KEY CLUSTERED ' + '(' + '[att' + cast(@attributeid AS NVARCHAR) + '] ASC' + ')WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]' + ') ON [PRIMARY]'

	EXECUTE sp_executesql @sql;

	IF NOT EXISTS (
			SELECT *
			FROM sys.foreign_keys
			WHERE type = 'F'
				AND NAME = 'FK_custom_' + @masterTableName + '_CreatedBy_employees'
			)
	BEGIN
		SET @sql = 'alter table dbo.[custom_' + @masterTableName + '] add constraint [FK_custom_' + @masterTableName + '_CreatedBy_employees] foreign key (CreatedBy) references dbo.employees (employeeid)';

		EXECUTE sp_executesql @sql;
	END

	IF NOT EXISTS (
			SELECT *
			FROM sys.foreign_keys
			WHERE type = 'F'
				AND NAME = 'FK_custom_' + @masterTableName + '_ModifiedBy_employees'
			)
	BEGIN
		SET @sql = 'alter table dbo.[custom_' + @masterTableName + '] add constraint [FK_custom_' + @masterTableName + '_ModifiedBy_employees] foreign key (ModifiedBy) references dbo.employees (employeeid)';

		EXECUTE sp_executesql @sql;
	END

	DECLARE @empTableId UNIQUEIDENTIFIER = (
			SELECT TOP 1 tableid
			FROM tables
			WHERE tablename = 'employees'
				AND tableFrom = 0
			);
	
	IF @systemCustomEntityId	IS  NULL
	BEGIN
		IF @builtIn = 1
			SET @SystemEntityAttributeGUID = NEWID()

		INSERT INTO [customEntityAttributes] (
			entityid
			,[display_name]
			,[fieldtype]
			,createdon
			,createdby
			,is_key_field
			,allowEdit
			,allowDelete
			,[format]
			,[description]
			,system_attribute
			,CacheExpiry
			,dbo.customEntityAttributes.SystemCustomEntityAttributeID
			)
		VALUES (
			@entityid
			,'Created On'
			,3
			,@date
			,@userid
			,0
			,0
			,0
			,3
			,'Built in attribute to show the date the data for this GreenLight record was created on'
			,1
			,GETUTCDATE()
			,@SystemEntityAttributeGUID
			);

		IF @builtIn = 1
			SET @SystemEntityAttributeGUID = NEWID()

		INSERT INTO [customEntityAttributes] (
			entityid
			,[display_name]
			,[fieldtype]
			,createdon
			,createdby
			,is_key_field
			,allowEdit
			,allowDelete
			,relationshiptype
			,relatedtable
			,[description]
			,system_attribute
			,CacheExpiry
			,dbo.customEntityAttributes.SystemCustomEntityAttributeID
			)
		VALUES (
			@entityid
			,'Created By'
			,9
			,@date
			,@userid
			,0
			,0
			,0
			,1
			,@empTableId
			,'Built in attribute to show the user who created the data for this GreenLight record.'
			,1
			,GETUTCDATE()
			,@SystemEntityAttributeGUID
			);

		IF @builtIn = 1
			SET @SystemEntityAttributeGUID = NEWID()

		INSERT INTO [customEntityAttributes] (
			entityid
			,[display_name]
			,[fieldtype]
			,createdon
			,createdby
			,is_key_field
			,allowEdit
			,allowDelete
			,[format]
			,[description]
			,system_attribute
			,CacheExpiry
			,dbo.customEntityAttributes.SystemCustomEntityAttributeID
			)
		VALUES (
			@entityid
			,'Modified On'
			,3
			,@date
			,@userid
			,0
			,0
			,0
			,3
			,'Built in attribute to show the date the data for this GreenLight record was modified on'
			,1
			,GETUTCDATE()
			,@SystemEntityAttributeGUID
			);

		IF @builtIn = 1
			SET @SystemEntityAttributeGUID = NEWID()

		INSERT INTO [customEntityAttributes] (
			entityid
			,[display_name]
			,[fieldtype]
			,createdon
			,createdby
			,is_key_field
			,allowEdit
			,allowDelete
			,relationshiptype
			,relatedtable
			,[description]
			,system_attribute
			,CacheExpiry
			,dbo.customEntityAttributes.SystemCustomEntityAttributeID
			)
		VALUES (
			@entityid
			,'Modified By'
			,9
			,@date
			,@userid
			,0
			,0
			,0
			,1
			,@empTableId
			,'Built in attribute to show the user that modified the data for this GreenLight record'
			,1
			,GETUTCDATE()
			,@SystemEntityAttributeGUID
			);
	END

	IF @enableCurrencies = 1
	BEGIN
		SET @currencySQL = 'ALTER TABLE [dbo].[custom_' + @masterTableName + '] ADD CONSTRAINT [FK_' + @masterTableName + '_currencies] FOREIGN KEY ([GreenLightCurrency]) REFERENCES [dbo].[currencies] ([currencyid]) ON UPDATE NO ACTION ON DELETE NO ACTION';

		EXECUTE sp_executesql @currencySQL;

		IF @builtIn = 1
			SET @SystemEntityAttributeGUID = NEWID()

		INSERT INTO [customEntityAttributes] (
			entityid
			,[display_name]
			,[fieldtype]
			,createdon
			,createdby
			,is_key_field
			,allowEdit
			,allowDelete
			,[description]
			,system_attribute
			,RelatedTable
			,CacheExpiry
			,dbo.customEntityAttributes.SystemCustomEntityAttributeID
			)
		VALUES (
			@entityid
			,'GreenLight Currency'
			,17
			,@date
			,@userid
			,0
			,0
			,0
			,'Built in attribute reflecting the currency the GreenLight record will use.'
			,1
			,'850422EA-AD71-4CEF-B6AF-227933BF8065'
			,GETUTCDATE()
			,@SystemEntityAttributeGUID
			);
	END

	EXEC saveCustomAttributeReportConfig @attributeid
		,NULL
		,1
		,0;
END
ELSE
BEGIN
	SET @masterTableName = (
			SELECT masterTableName
			FROM [customEntities]
			WHERE entityid = @entityid
			);

	SELECT @attributeid = attributeid
	FROM [customEntityAttributes]
	WHERE entityid = @entityid
		AND is_key_field = 1;

	DECLARE @origEntityName NVARCHAR(250);
	DECLARE @origPluralName NVARCHAR(250);

	SELECT @origEntityName = entity_name
		,@origPluralName = plural_name
	FROM [customEntities]
	WHERE entityid = @entityid;

	IF @origEntityName <> @entityname
		OR @origPluralName <> @pluralname
	BEGIN
		-- renamed, so check that not duplicate
		SET @count = (
				SELECT COUNT(*)
				FROM [customEntities]
				WHERE entity_name = @entityname
					AND entityid <> @entityid
				)

		IF @count > 0
		BEGIN
			-- If the entity name has been used AND the plural name has been used, return -3
			IF @origPluralName <> @pluralname
			BEGIN
				SET @count = (
						SELECT COUNT(*)
						FROM [customEntities]
						WHERE plural_name = @pluralname
						);

				IF @count > 0
				BEGIN
					RETURN - 3;
				END
			END

			-- If only the entity name has been used, return -1
			RETURN - 1;
		END

		SET @count = (
				SELECT COUNT(*)
				FROM [customEntities]
				WHERE plural_name = @pluralname
					AND entityid <> @entityid
				)

		IF @count > 0
			RETURN - 2;
	END

	-- if the current built-in flag is true then never let it change to false
	DECLARE @currentBuiltIn BIT;

	SELECT @currentBuiltIn = BuiltIn
	FROM [customEntities]
	WHERE entityid = @entityid;

	IF @currentBuiltIn = 1
	BEGIN
		SET @builtIn = 1;
	END
	
	IF @builtIn = 1
		SET @SystemEntityGUID =   IsNull(@systemCustomEntityId, NEWID())


	UPDATE [customEntities]
	SET entity_name = @entityname
		,plural_name = @pluralname
		,[description] = @description
		,modifiedby = @userid
		,modifiedon = @date
		,enableattachments = @enableattachments
		,audienceViewType = @audienceViewType
		,allowdocmergeaccess = @allowdocmergeaccess
		,enableCurrencies = @enableCurrencies
		,defaultCurrencyID = @defaultCurrencyID
		,enablePopupWindow = @enablePopupWindow
		,defaultPopupView = @defaultPopupView
		,ownerId = @ownerId
		,supportContactId = @supportContactId
		,supportQuestion = @supportQuestion
		,enableLocking = @enableLocking
		,CacheExpiry = GETUTCDATE()
		,formSelectionAttribute = @formSelectionAttribute
		,builtIn = @builtIn
		,SystemCustomEntityID = ISNULL(SystemCustomEntityID, @SystemEntityGUID)
	WHERE entityid = @entityid;

	IF @enableCurrencies = 1
	BEGIN
		IF NOT EXISTS (
				SELECT *
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE COLUMN_NAME = 'GreenLightCurrency'
					AND TABLE_NAME = 'custom_' + @masterTableName
				)
		BEGIN
			SET @sql = 'ALTER TABLE dbo.[custom_' + @masterTableName + '] ADD GreenLightCurrency int null CONSTRAINT [FK_' + @masterTableName + '_currencies] FOREIGN KEY ([GreenLightCurrency]) REFERENCES [dbo].[currencies] ([currencyid]) ON UPDATE NO ACTION ON DELETE NO ACTION';

			EXEC sp_executesql @sql;
		END

		IF NOT EXISTS (
				SELECT *
				FROM [customEntityAttributes]
				WHERE entityid = @entityid
					AND display_name = 'GreenLight Currency'
				)
		BEGIN
			IF @builtIn = 1
				SET @SystemEntityAttributeGUID = NEWID()

			INSERT INTO [customEntityAttributes] (
				entityid
				,[display_name]
				,[fieldtype]
				,createdon
				,createdby
				,is_key_field
				,allowEdit
				,allowDelete
				,[description]
				,system_attribute
				,RelatedTable
				,CacheExpiry
				,dbo.customEntityAttributes.SystemCustomEntityAttributeID
				)
			VALUES (
				@entityid
				,'GreenLight Currency'
				,17
				,@date
				,@userid
				,0
				,0
				,0
				,'Built in attribute reflecting the currency the GreenLight record will use.'
				,1
				,'850422EA-AD71-4CEF-B6AF-227933BF8065'
				,GETUTCDATE()
				,@SystemEntityAttributeGUID
				);
		END
	END

	-- rename any viewgroups or systemviews that use the renamed entity
	DECLARE @viewgroupid UNIQUEIDENTIFIER;
	DECLARE @groupname NVARCHAR(250);

	DECLARE lp CURSOR
	FOR
	SELECT tableid
	FROM customEntities
	WHERE systemview_derivedentityid = @entityid
		OR systemview_entityid = @entityid
		OR entityid = @entityid

	OPEN lp

	FETCH NEXT
	FROM lp
	INTO @viewgroupid

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @groupname = (
				SELECT entity_name
				FROM customViewGroups
				WHERE viewgroupid = @viewgroupid
				);

		UPDATE customViewGroups
		SET entity_name = REPLACE(@groupname, @origEntityName, @entityname)
			,amendedon = GETDATE()
		WHERE viewgroupid = @viewgroupid;

		FETCH NEXT
		FROM lp
		INTO @viewgroupid
	END

	CLOSE lp;

	DEALLOCATE lp;

	UPDATE customEntities
	SET entity_name = REPLACE(entity_name, @origEntityName, @entityname)
		,plural_name = REPLACE(plural_name, @origPluralName, @pluralname)
		,enableLocking = @enableLocking
		,modifiedon = GETDATE()
		,modifiedby = @userid
		,CacheExpiry = GETUTCDATE()
	WHERE systemview = 1
		AND (
			systemview_derivedentityid = @entityid
			OR systemview_entityid = @entityid
			)

	IF @builtIn	= 1 AND @currentBuiltIn	= 0
	BEGIN
		UPDATE dbo.customEntityAttributes
		SET
			dbo.customEntityAttributes.BuiltIn	= 1
			,dbo.customEntityAttributes.SystemCustomEntityAttributeID	= NEWID()	
		WHERE dbo.customEntityAttributes.entityid	= @entityid	
			AND dbo.customEntityAttributes.BuiltIn	= 0
			AND dbo.customEntityAttributes.SystemCustomEntityAttributeID IS NULL
			AND dbo.customEntityAttributes.display_name	IN (
				'Modified By',
				'Modified On',
				'Created By',
				'Created On',
				'ID')

	END

END

IF @enableattachments = 1
	OR @allowdocmergeaccess = 1
BEGIN
	--create the attachment table
	DECLARE @attachmentname NVARCHAR(2000)

	SET @attachmentname = 'custom_' + @masterTableName + '_attachments';
	SET @count = (
			SELECT COUNT(*)
			FROM sys.tables
			WHERE [name] = @attachmentname
			);

	IF @count = 0
	BEGIN
		SET @attachmentname = 'custom_' + @masterTableName;

		EXEC createCustomEntityAttachmentTable @attachmentname;
	END
END

IF @audienceViewType > 0
BEGIN
	--create the audience table
	DECLARE @audiencename NVARCHAR(2000);

	SET @audiencename = 'custom_' + @masterTableName + '_audiences';
	SET @count = (
			SELECT COUNT(*)
			FROM sys.tables
			WHERE [name] = @audiencename
			);

	IF @count = 0
	BEGIN
		SET @audiencename = 'custom_' + @masterTableName;

		EXEC createCustomEntityAudienceTable @audiencename
			,@attributeid;
	END
END

RETURN @entityid



GO


