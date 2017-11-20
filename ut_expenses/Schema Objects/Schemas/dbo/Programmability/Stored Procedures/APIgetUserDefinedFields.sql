CREATE PROCEDURE [dbo].[APIgetUserDefinedFields]
	@recordId int
AS
	select userdefined.fieldtype, userdefined.tableid, tables.tablename ,userdefined.fieldid, fields.field, CAST(0 AS int) AS Value, CAST(0 AS int) AS recordId, userdefined.displayField, userdefined.relatedTable, userdefined.userdefineid
	FROM userdefined, tables, fields 
	WHERE userdefined.tableid = tables.tableid and userdefined.fieldid = fields.fieldid
RETURN 0