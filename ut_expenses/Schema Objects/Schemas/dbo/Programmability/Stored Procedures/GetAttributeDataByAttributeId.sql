CREATE PROCEDURE [dbo].[GetAttributeDataByAttributeId]
 @attributeId int,
 @recId int = 0
AS 
BEGIN
DECLARE @tableName varchar(100)
DECLARE @fieldName varchar(50)
DECLARE @keyField  varchar(50)

DECLARE @qry varchar(max)

select @tableName =  'custom_'+ convert(varchar,entityid) from customEntityAttributes where attributeid = @attributeId

select @keyField = field from fields INNER JOIN tables on tables.tableid = fields.tableid WHERE tables.tablename = @tableName AND fields.fieldid = tables.primarykey

select @fieldNAme = field from customEntityAttributes inner join fields on fields.fieldid = customEntityAttributes.fieldid where attributeid = @attributeId

set @qry = 'select top 1 ' + @fieldName + ' as data from '+ @tableName + ' WHERE ' + CAST(@recId AS NVarchar(18)) + ' = 0 OR ' + CAST(@recId AS NVarchar(18)) + ' = ' + @keyField
Exec (@qry)
END

GO


