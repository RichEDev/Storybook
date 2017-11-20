CREATE PROCEDURE [dbo].[createCustomEntityAttachmentTable]
@tablename NVARCHAR(2000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @sql NVARCHAR(max)

	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tablename + '_attachmentData')
	BEGIN
		SET @sql = 'CREATE TABLE [dbo].[' + @tablename + '_attachmentData] (
		[attachmentID] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
		[fileData] [varbinary](max) NOT NULL )'
		EXECUTE sp_executesql @sql
	END

	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tablename + '_attachments')
	BEGIN
		SET @sql =  'CREATE TABLE [dbo].[' + @tablename + '_attachments] (' +
		'[attachmentID] [int] NOT NULL, ' +
		'[id] [int] NOT NULL, ' +
		'[title] [nvarchar](200) COLLATE Latin1_General_CI_AS NULL, ' +
		'[description] [nvarchar](max) COLLATE Latin1_General_CI_AS NULL, ' +
		'[fileName] [nvarchar](150) COLLATE Latin1_General_CI_AS NULL, ' +
		'[mimeID] [int] NOT NULL, ' +
		'[createdon] [datetime] NULL, ' +
		'[createdby] [int] NOT NULL, ' +
		'[TorchGenerated] BIT NOT NULL, ' +
		'[Published] BIT NOT NULL' +
		') ON [PRIMARY]'
		EXECUTE sp_executesql @sql
	END

	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE CONSTRAINT_NAME = 'FK_' + @tablename + '_attachments_attachmentData')
	BEGIN
		SET @sql = 'ALTER TABLE dbo.[' + @tablename + '_attachments] ADD CONSTRAINT [FK_' + @tablename + '_attachments_attachmentData] FOREIGN KEY ([attachmentID]) REFERENCES dbo.[' + @tablename + '_attachmentData] ([attachmentID]) ON DELETE CASCADE'
		EXECUTE sp_executesql @sql
	END

	IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_' + @tablename + '_attachments_Published]') AND type = 'D')
	BEGIN
		SET @sql = 'ALTER TABLE [dbo].[' + @tablename + '_attachments] ADD CONSTRAINT [DF_' + @tablename + '_attachments_Published]  DEFAULT ((0)) FOR [Published]';
		EXECUTE sp_executesql @sql;
	END
	
	IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_' + @tablename + '_attachments_TorchGenerated]') AND type = 'D')
	BEGIN
		SET @sql = 'ALTER TABLE [dbo].[' + @tablename + '_attachments] ADD CONSTRAINT [DF_' + @tablename + '_attachments_TorchGenerated]  DEFAULT ((0)) FOR [TorchGenerated]';
		EXECUTE sp_executesql @sql;
	END

	DECLARE @attachmentTableID uniqueidentifier
	DECLARE @parmDefinition nvarchar(50)

	SET @sql = 'select @returnID = tableid from tables where tablename = ' + char(39) + @tablename + '_attachments' + char(39)
	SET @parmDefinition = '@returnID uniqueidentifier OUTPUT'
	EXECUTE sp_executesql @sql, @parmDefinition, @returnID = @attachmentTableID OUTPUT
	
	if @attachmentTableID is not null
	begin
		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'attachmentID')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'attachmentID','N',0,'id of attachment data entry');
		END

		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'id')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID,'id','N',1,'id of parent record');
		END

		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'title')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'title','S',0,'Attachment Title');
		END
				
		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'description')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'description','S',0, 'Attachment Description');
		END

		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'fileName')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'fileName','N',0,'Attachment Filename');
		END

		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'mimeID')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'mimeID','N',0,'id of attachment content type');
		END

		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'createdon')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'createdon','D',0,'Date attachment created');
		END

		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'createdby')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'createdby','N',0,'id of user who created attachment');
		END
		
		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'TorchGenerated')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'TorchGenerated','X',0, 'Record is a generated torch document');
		END

		IF NOT EXISTS (SELECT * FROM customEntityAttachmentFields WHERE tableid = @attachmentTableID AND field = 'Published')
		BEGIN
			INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
			VALUES (@attachmentTableID, 'Published','X',0,'Record is shown in the list of torch generated attachments');
		END
	end
END