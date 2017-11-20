CREATE PROCEDURE [dbo].[createCustomEntityAudienceTable]
@tablename NVARCHAR(2000),
@attributeid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @sql NVARCHAR(max)
	DECLARE @count int

	SET @sql = 'IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ' + CHAR(39) + replace(@tablename,' ','_') + '_audiences'  + CHAR(39) + ') BEGIN ' +
	'CREATE TABLE [dbo].[' + replace(@tablename,' ','_') + '_audiences] (' +
	'[id] [int]  IDENTITY(1,1) NOT NULL, ' +
	'[parentID] [int] NOT NULL,' +	
	'[canView] [bit] NULL,' +
	'[canEdit] [bit] NULL,' +
	'[canAdd] [bit] NULL,' +
	'[canDelete] [bit] NULL,' +
    '[audienceID] [int] NOT NULL, ' +
    '[CacheExpiry] [datetime] NULL, ' +
	'CONSTRAINT [PK_' + replace(@tablename,' ','_') + '] PRIMARY KEY CLUSTERED ' +
	'(' +
		'[id] ASC' +
	')WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]' +
	') ON [PRIMARY]' +
	' END'
	EXECUTE sp_executesql @sql;
	
	
	SET @sql = 'IF NOT EXISTS (SELECT 1 from sys.objects where name = ' + CHAR(39) + 'DF_' + replace(@tablename,' ','_') 
	+ '_audiences_canView' + CHAR(39) + ') BEGIN ALTER TABLE ' + replace(@tablename,' ','_') + '_audiences ADD CONSTRAINT DF_' 
	+ replace(@tablename,' ','_') + '_audiences_canView DEFAULT 1 FOR canView END';
	EXECUTE sp_executesql @sql;
	
	SET @sql = 'IF NOT EXISTS (SELECT 1 from sys.objects where name = ' + CHAR(39) + 'DF_' + replace(@tablename,' ','_') 
	+ '_audiences_canAdd' + CHAR(39) + ') BEGIN ALTER TABLE ' + replace(@tablename,' ','_') + '_audiences ADD CONSTRAINT DF_' 
	+ replace(@tablename,' ','_') + '_audiences_canAdd DEFAULT 1 FOR canAdd END';	
	EXECUTE sp_executesql @sql;
	
	SET @sql = 'IF NOT EXISTS (SELECT 1 from sys.objects where name = ' + CHAR(39) + 'DF_' + replace(@tablename,' ','_') 
	+ '_audiences_canEdit' + CHAR(39) + ') BEGIN ALTER TABLE ' + replace(@tablename,' ','_') + '_audiences ADD CONSTRAINT DF_' 
	+ replace(@tablename,' ','_') + '_audiences_canEdit DEFAULT 1 FOR canEdit END';	
	EXECUTE sp_executesql @sql;
	
	SET @sql = 'IF NOT EXISTS (SELECT 1 from sys.objects where name = ' + CHAR(39) + 'DF_' + replace(@tablename,' ','_') 
	+ '_audiences_canDelete' + CHAR(39) + ') BEGIN ALTER TABLE ' + replace(@tablename,' ','_') + '_audiences ADD CONSTRAINT DF_' 
	+ replace(@tablename,' ','_') + '_audiences_canDelete DEFAULT 1 FOR canDelete END';	
	EXECUTE sp_executesql @sql;
	
	
	SET @sql = 'IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE TABLE_NAME = ' + CHAR(39) + replace(@tablename,' ','_') + '_audiences' + CHAR(39) + ' AND CONSTRAINT_NAME = ' + CHAR(39) + 'FK_' + replace(@tablename,' ','_') + '_audiences_audiences' + CHAR(39) + ')' +
	'BEGIN ALTER TABLE dbo.[' + replace(@tablename,' ','_') + '_audiences] ADD CONSTRAINT [FK_' + replace(@tablename,' ','_') + '_audiences_audiences] FOREIGN KEY ([audienceID]) REFERENCES dbo.[audiences] ([audienceID]) ON DELETE CASCADE ' +
	'END'
	EXECUTE sp_executesql @sql;

	DECLARE @audienceTableID uniqueidentifier
	DECLARE @parmDefinition nvarchar(100)

	SET @sql = 'select @returnID = tableid from tables where tablename = ' + char(39) + replace(@tablename,' ','_') + '_audiences' + char(39)
	SET @parmDefinition = '@returnID uniqueidentifier OUTPUT'
	EXECUTE sp_executesql @sql, @parmDefinition, @returnID = @audienceTableID OUTPUT;
	
	SET @count = (SELECT COUNT(*) FROM sys.tables WHERE [name] = replace(@tablename,' ','_') + '_audiences');
	IF @count > 0
	begin
		IF NOT EXISTS (SELECT * FROM custom_fields WHERE tableid = @audienceTableID AND field = 'id')
		BEGIN
			INSERT INTO [custom_fields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@audienceTableID,'id','N',1,'id audience');
		END
		
		IF NOT EXISTS (SELECT * FROM custom_fields WHERE tableid = @audienceTableID AND field = 'parentID')
		BEGIN
			INSERT INTO [custom_fields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@audienceTableID, 'parentID','N',0,'Parent Record ID');
		END
		
		IF NOT EXISTS (SELECT * FROM custom_fields WHERE tableid = @audienceTableID AND field = 'audienceID')
		BEGIN
			INSERT INTO [custom_fields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@audienceTableID, 'audienceID','N',0, 'Audience ID');
		END
		
		DECLARE @audiencesTable uniqueidentifier
		DECLARE @audiencesPKeyID uniqueidentifier
		DECLARE @countTableid int
		DECLARE @joinOrder tinyint
		DECLARE @entAudFieldID uniqueidentifier


		SET @sql = 'select @returnID1 = tableid, @returnID2 = primarykey from tables where tablename = ' + char(39) + 'audiences' + char(39)
		SET @parmDefinition = '@returnID1 uniqueidentifier OUTPUT,@returnID2 uniqueidentifier OUTPUT'
		EXECUTE sp_executesql @sql, @parmDefinition, @returnID1 = @audiencesTable OUTPUT, @returnID2 = @audiencesPKeyID OUTPUT

		SET @countTableid = ((select isnull(max(jointableid),0) from jointables where jointableid > 50000) + 1);
		SET @joinOrder = CAST(1 AS tinyint)
		SET @entAudFieldID = (select fieldid from custom_fields where field='audienceID' and tableid=@audienceTableID)

		IF NOT EXISTS (SELECT * FROM custom_jointables WHERE basetableid = @audienceTableID AND tableid = @audiencesTable)
		BEGIN
			INSERT INTO [custom_jointables] ([jointableid],[tableid],[basetableid],[description], amendedon)
			VALUES (@countTableid,@audiencesTable,@audienceTableID,'Entity ' + @tablename + '_audiences to audiences', getdate());
		END
		
		IF NOT EXISTS (SELECT * FROM custom_joinbreakdown WHERE sourcetable = @audienceTableID and tableid = @audiencesTable)
		BEGIN
			INSERT INTO [custom_joinbreakdown] ([joinbreakdownid],[jointableid],[order],[tableid],[sourcetable],[joinkey],[destinationkey], amendedon)
			VALUES (@countTableid,@countTableid,@joinOrder,@audiencesTable,@audienceTableID,@audiencesPKeyID,@entAudFieldID, getdate());
		END
	end
END
