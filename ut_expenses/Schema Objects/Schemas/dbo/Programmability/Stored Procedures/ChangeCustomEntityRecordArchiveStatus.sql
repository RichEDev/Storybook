
CREATE PROCEDURE [dbo].[ChangeCustomEntityRecordArchiveStatus]
@entityid INT,
@recordMatchAttributeId INT,
@recordid INT,
@CUemployeeID INT,
@CUdelegateID INT,
@recordTitle NVARCHAR(2000),
@archived BIT

AS
	DECLARE @parentTable NVARCHAR(250)
	DECLARE @parentTableId UNIQUEIDENTIFIER	
	DECLARE @hasAttachments BIT

	SET @parentTableId = (SELECT tableid FROM [customEntities] WHERE entityid = @entityid)
	SET @parentTable = (SELECT tablename FROM tables WHERE tableid = @parentTableId)
	DECLARE @retCode int = 0;		
    DECLARE @sql NVARCHAR(3000)

	SET @sql = 'UPDATE  [' + @parentTable + '] SET Archived = ' + cast(@archived AS NVARCHAR) + ' WHERE att' + cast(@recordMatchAttributeId AS NVARCHAR) + ' = ' + cast(@recordid AS NVARCHAR)

    EXEC sp_executesql @sql	
		
	IF(@@ERROR>0)		
	BEGIN
	SET @retCode=-1;
	END

	RETURN @retCode;	
GO


