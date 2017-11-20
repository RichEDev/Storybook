CREATE PROCEDURE [dbo].[DeleteCustomEntityImageData] 
	@fileId uniqueidentifier,
	@attributeId int,
	@entityId int,
	@entityRecordId int,
	@entityPrimaryKey nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	delete from dbo.CustomEntityImageData where fileID = @fileId;
	
	declare @sql nvarchar(1000);
	
	set @sql = (select 'update custom_' + cast(@entityId as nvarchar(15)) + ' set att' + cast(@attributeId as nvarchar(15)) + ' = null WHERE custom_' + cast(@entityId as nvarchar(15)) + '.' + @entityPrimaryKey + ' = ' + cast(@entityRecordId as nvarchar(15)) + '');
	print @sql;
	exec(@sql);
	
	return 0;
END
