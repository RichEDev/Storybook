CREATE PROCEDURE [dbo].[saveCustomEntity]
@entityid int,
@entityname nvarchar(250),
@pluralname NVARCHAR(250),
@description nvarchar(4000),
@enableattachments BIT,
@enableAudiences bit,
@allowdocmergeaccess bit,
@enableCurrencies bit,
@defaultCurrencyID int,
@date DateTime,
@userid int --,
--@CUemployeeID INT,
--@CUdelegateID INT
AS
declare @rename_sql nvarchar(1500);
DECLARE @currencySQL NVARCHAR(30);

DECLARE @count INT;
if (@entityid = 0)
BEGIN
	SET @count = (SELECT COUNT(*) FROM custom_entities WHERE entity_name = @entityname);
	IF @count > 0
		RETURN -1;
		
	insert into custom_entities (entity_name, plural_name, [description], createdby, createdon, enableAttachments, enableAudiences, allowdocmergeaccess, enableCurrencies, defaultCurrencyID) values (@entityname, @pluralname, @description, @userid, @date, @enableattachments, @enableAudiences, @allowdocmergeaccess, @enableCurrencies, @defaultCurrencyID)
	set @entityid = scope_identity()

	declare @nextAttributeId int
	set @nextAttributeId = ident_current('custom_entity_attributes') + 1;	


	INSERT INTO custom_entity_attributes (entityid, attribute_name, [display_name], [fieldtype], createdon, createdby, is_key_field, allowEdit, allowDelete) VALUES (@entityid, 'att' + cast(@nextAttributeId as nvarchar), 'id', 2, @date, @userid,1, 0, 0);
	
	DECLARE @attributeid INT;
	SET @attributeid = SCOPE_IDENTITY();
	
	SET @currencySQL = '';
	IF @enableCurrencies = 1
	BEGIN
		SET @currencySQL = '[EntityCurrency] [INT] NULL,';
	END

	DECLARE @sql NVARCHAR(4000)
	SET @sql =  'CREATE TABLE [dbo].[custom_' + replace(@pluralname,' ', '_') + '](' +
	'[att' + cast(@attributeid as nvarchar) + '] [int] IDENTITY(1,1) NOT NULL,' +
	'[CreatedOn] [datetime] NULL,' +
	'[CreatedBy] [int] NULL,' +
	'[ModifiedOn] [datetime] NULL,' +
	'[ModifiedBy] [int] NULL,' +
	@currencySQL +
	'CONSTRAINT [PK_' + replace(@pluralname,' ','_') + '] PRIMARY KEY CLUSTERED ' +
	'(' +
		'[att' + cast(@attributeid as nvarchar) + '] ASC' +
	')WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]' +
	') ON [PRIMARY]'
	EXECUTE sp_executesql @sql
	
	EXECUTE saveCustomAttributeReportConfig 0, null, 1, 2	
	
	INSERT INTO custom_entity_attributes (entityid, attribute_name, [display_name], [fieldtype], createdon, createdby, is_key_field, allowEdit, allowDelete, [format]) VALUES (@entityid, 'CreatedOn', 'Created On', 3, @date, @userid,0, 0,0,3);
	INSERT INTO custom_entity_attributes (entityid, attribute_name, [display_name], [fieldtype], createdon, createdby, is_key_field, allowEdit, allowDelete, relationshiptype, relatedtable) VALUES (@entityid, 'CreatedBy', 'Created By', 9, @date, @userid,0, 0, 0, 1, '618DB425-F430-4660-9525-EBAB444ED754');
	INSERT INTO custom_entity_attributes (entityid, attribute_name, [display_name], [fieldtype], createdon, createdby, is_key_field, allowEdit, allowDelete,[format]) VALUES (@entityid, 'ModifiedOn', 'Modified On', 3, @date, @userid,0, 0,0,3);
	INSERT INTO custom_entity_attributes (entityid, attribute_name, [display_name], [fieldtype], createdon, createdby, is_key_field, allowEdit, allowDelete, relationshiptype, relatedtable) VALUES (@entityid, 'ModifiedBy', 'Modified By', 9, @date, @userid,0, 0, 0, 1, '618DB425-F430-4660-9525-EBAB444ED754');
	IF @enableCurrencies = 1
	BEGIN
		SET @currencySQL = 'ALTER TABLE [dbo].[custom_' + replace(@pluralname,' ', '_') + '] ADD CONSTRAINT [FK_' + replace(@pluralname,' ','_') + '_currencies] FOREIGN KEY ([EntityCurrency]) REFERENCES [dbo].[currencies] ([currencyid]) ON UPDATE NO ACTION ON DELETE NO ACTION';
		EXECUTE sp_executesql @currencySQL;

		INSERT INTO custom_entity_attributes (entityid, attribute_name, [display_name], [fieldtype], createdon, createdby, is_key_field, allowEdit, allowDelete) VALUES (@entityid, 'EntityCurrency', 'Currency', 17, @date, @userid, 0, 0, 0);
	END
end
else
BEGIN
	select @attributeid = attributeid from custom_entity_attributes where entityid = @entityid AND is_key_field = 1;
	
	declare @origEntityName nvarchar(250)
	declare @origPluralName nvarchar(250)
	declare @renameEntity bit

	set @renameEntity = 0

	select @origEntityName = entity_name, @origPluralName = plural_name from custom_entities where entityid = @entityid;

	if @origEntityName <> @entityname or @origPluralName <> @pluralname
	begin
		SET @renameEntity = 1
		-- renamed, so check that not duplicate and rename tables
		SET @count = ((SELECT COUNT(*) FROM custom_entities WHERE entity_name = @entityname AND entityid <> @entityid) + (SELECT COUNT(*) FROM custom_entities WHERE plural_name = @pluralname AND entityid <> @entityid))
		if @count > 0
			return -1;
		
		set @rename_sql = 'exec sp_rename ' + char(39) + 'custom_' + @origPluralName + char(39) + ', ' + char(39) + 'custom_' + @pluralname + char(39);

		exec sp_executesql @rename_sql;
	end

	SET @count = (SELECT COUNT(*) FROM custom_entities WHERE entity_name = @entityname AND entityid <> @entityid);
	IF @count > 0
		RETURN -1;
		
	update custom_entities set entity_name = @entityname, plural_name = @pluralname, [description] = @description, modifiedby = @userid, modifiedon = @date, enableattachments = @enableattachments, enableAudiences = @enableAudiences, allowdocmergeaccess = @allowdocmergeaccess, enableCurrencies = @enableCurrencies, defaultCurrencyID = @defaultCurrencyID where entityid = @entityid;
	
	IF @enableCurrencies = 1
	BEGIN
		IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'EntityCurrency' AND TABLE_NAME = 'custom_' + replace(@pluralname,' ', '_'))
		BEGIN
			SET @sql = 'ALTER TABLE dbo.[custom_' + replace(@pluralname,' ', '_') + '] ADD EntityCurrency int null CONSTRAINT [FK_' + replace(@pluralname,' ','_') + '_currencies] FOREIGN KEY ([EntityCurrency]) REFERENCES [dbo].[currencies] ([currencyid]) ON UPDATE NO ACTION ON DELETE NO ACTION';
			EXEC sp_executesql @sql;
		END
		
		IF NOT EXISTS (SELECT * FROM custom_entity_attributes WHERE entityid = @entityid AND attribute_name = 'EntityCurrency')
		BEGIN
			INSERT INTO custom_entity_attributes (entityid, attribute_name, [display_name], [fieldtype], createdon, createdby, is_key_field, allowEdit, allowDelete) VALUES (@entityid, 'EntityCurrency', 'Currency', 17, @date, @userid, 0, 0, 0);
		END
	END
end

IF @enableattachments = 1
BEGIN
	--create the attachment table
	DECLARE @attachmentname NVARCHAR(2000)
	SET @attachmentname = 'custom_' + replace(@pluralname,' ', '_') + '_attachments';
	SET @count = (SELECT COUNT(*) FROM sys.tables WHERE [name] = @attachmentname);
	IF @count = 0
	BEGIN
		SET @attachmentname = 'custom_' + replace(@pluralname,' ', '_');
		EXEC createCustomEntityAttachmentTable @attachmentname

--		declare @attachmentTableID uniqueidentifier
--		SET @attachmentname = 'custom_' + replace(@pluralname,' ', '_') + '_attachments';
--		set @attachmentTableID = (select tableid from tables where tablename = @attachmentname);
--
--		update custom_entities set attachmentTableID = @attachmentTableID where entityid = @entityid;
	end
	else if @renameEntity = 1
	begin		
		set @rename_sql = 'exec sp_rename ' + char(39) + 'custom_' + @origPluralName + '_attachments' + char(39) + ', ' + char(39) + 'custom_' + @pluralname + '_attachments' + char(39);
		exec sp_executesql @rename_sql;

		set @rename_sql = 'exec sp_rename ' + char(39) + 'custom_' + @origPluralName + '_attachmentData' + char(39) + ', ' + char(39) + 'custom_' + @pluralname + '_attachmentData' + char(39);
		exec sp_executesql @rename_sql;
	end
END
		
IF @enableAudiences = 1
BEGIN
	--create the audience table
	DECLARE @audiencename NVARCHAR(2000)
	SET @audiencename = 'custom_' + replace(@pluralname,' ', '_') + '_audiences';
	SET @count = (SELECT COUNT(*) FROM sys.tables WHERE [name] = @audiencename);
	IF @count = 0
	BEGIN
		SET @audiencename = 'custom_' + replace(@pluralname,' ', '_');
		EXEC createCustomEntityAudienceTable @audiencename, @attributeid

--		declare @audienceTableID uniqueidentifier
--		SET @audiencename = 'custom_' + replace(@pluralname,' ', '_') + '_audiences';
--		set @audienceTableID = (select tableid from tables where tablename = @audiencename);
--
--		update custom_entities set audienceTableID = @audienceTableID where entityid = @entityid;
	end
	else if @renameEntity = 1
	begin		
		set @rename_sql = 'exec sp_rename ' + char(39) + 'custom_' + @origPluralName + '_audiences' + char(39) + ', ' + char(39) + 'custom_' + @pluralname + '_audiences' + char(39);
		exec sp_executesql @rename_sql;
	end
END
		
return @entityid
