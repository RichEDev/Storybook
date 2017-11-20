CREATE PROCEDURE [dbo].[createCustomEntityAttachmentTable]
@tablename NVARCHAR(2000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @sql NVARCHAR(max)

	SET @sql = 'CREATE TABLE [dbo].[' + @tablename + '_attachmentData] (
	[attachmentID] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[fileData] [varbinary](max) NOT NULL )'
	EXECUTE sp_executesql @sql
	
	SET @sql =  'CREATE TABLE [dbo].[' + @tablename + '_attachments] (' +
	'[attachmentID] [int] NOT NULL, ' +
	'[id] [int] NOT NULL, ' +
	'[title] [nvarchar](200) COLLATE Latin1_General_CI_AS NULL, ' +
	'[description] [nvarchar](max) COLLATE Latin1_General_CI_AS NULL, ' +
	'[fileName] [nvarchar](150) COLLATE Latin1_General_CI_AS NULL, ' +
	'[mimeID] [int] NOT NULL, ' +
	'[createdon] [datetime] NULL, ' +
	'[createdby] [int] NOT NULL ' +
	') ON [PRIMARY]'
	EXECUTE sp_executesql @sql

	SET @sql = 'ALTER TABLE dbo.[' + @tablename + '_attachments] ADD CONSTRAINT [FK_' + @tablename + '_attachments_attachmentData] FOREIGN KEY ([attachmentID]) REFERENCES dbo.[' + @tablename + '_attachmentData] ([attachmentID]) ON DELETE CASCADE'
	EXECUTE sp_executesql @sql

	DECLARE @attachmentTableID uniqueidentifier
	DECLARE @parmDefinition nvarchar(50)

	SET @sql = 'select @returnID = tableid from tables where tablename = ' + char(39) + @tablename + '_attachments' + char(39)
	SET @parmDefinition = '@returnID uniqueidentifier OUTPUT'
	EXECUTE sp_executesql @sql, @parmDefinition, @returnID = @attachmentTableID OUTPUT
	
	if @attachmentTableID is not null
	begin
		INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
		VALUES (@attachmentTableID, 'attachmentID','N',0,'id of attachment data entry');

		INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
		VALUES (@attachmentTableID,'id','N',1,'id of parent record');

		INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
		VALUES (@attachmentTableID, 'title','S',0,'Attachment Title');
		
		INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
		VALUES (@attachmentTableID, 'description','S',0, 'Attachment Description');

		INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
		VALUES (@attachmentTableID, 'fileName','N',0,'Attachment Filename');

		INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
		VALUES (@attachmentTableID, 'mimeID','N',0,'id of attachment content type');

		INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
		VALUES (@attachmentTableID, 'createdon','D',0,'Date attachment created');

		INSERT INTO [customEntityAttachmentFields] ([tableid],[field],[fieldtype],[idfield],[description])
		VALUES (@attachmentTableID, 'createdby','N',0,'id of user who created attachment');
	end
END



