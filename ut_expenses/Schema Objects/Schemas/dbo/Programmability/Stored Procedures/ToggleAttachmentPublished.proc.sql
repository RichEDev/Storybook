CREATE PROCEDURE [dbo].[ToggleAttachmentPublished]
 @AttachmentId INT,
 @TableName NVARCHAR(200),
 @UserId INT, 
 @DelegateId INT
AS

DECLARE @paramDefinition nvarchar(MAX);
DECLARE @sql NVARCHAR(MAX);

-- Get and flip the current archived status
DECLARE @Published BIT;
DECLARE @NewPublished BIT;
SET @sql = 'SELECT @ReturnID = [Published] FROM ' + @TableName + ' WHERE [attachmentid] = ' + CAST(@AttachmentId AS NVARCHAR);
SET @paramDefinition = '@ReturnID BIT OUTPUT';
EXECUTE sp_executesql @sql, @paramDefinition, @ReturnID = @Published OUTPUT;
SELECT @NewPublished = @Published ^ 1;

-- Perform the update
SET @sql =  'UPDATE [' + @TableName + '] ' +
			'SET [Published] = ' + CAST(@NewPublished AS NVARCHAR) + ' ' +
			'WHERE [attachmentid] = ' + CAST(@AttachmentId AS NVARCHAR) + ';';
EXECUTE sp_executesql @sql;