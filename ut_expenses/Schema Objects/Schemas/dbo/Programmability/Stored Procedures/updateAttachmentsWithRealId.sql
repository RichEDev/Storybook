CREATE PROCEDURE dbo.updateAttachmentsWithRealId
@entityid int,
@id int,
@CUemployeeID INT,
@CUdelegateID INT
AS
	DECLARE @sql nvarchar(2500)
	DECLARE @tablename nvarchar(300)
	DECLARE @datatablename nvarchar(300)
	BEGIN
		set @tablename = 'custom_' + CAST(@entityid as nvarchar) + '_attachments'
        SET @sql = 'UPDATE [' + @tablename + '] set id= ' + CAST(@id  as nvarchar) + ' where createdby = ' + CAST(@CUemployeeID as nvarchar) + ' and id = 0 and createdon >= ''' + CAST(dateadd(hour , -1, getutcdate()) as nvarchar) + ''''
        execute sp_executesql @sql
	END
