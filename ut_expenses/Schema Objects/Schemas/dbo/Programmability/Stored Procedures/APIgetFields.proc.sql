CREATE PROCEDURE [dbo].[APIgetFields]
	
AS
	SELECT     fields.fieldid, fields.tableid, fields.description, fields.field, tables.tablename
FROM         fields INNER JOIN
                      tables ON fields.tableid = tables.tableid
WHERE     (tables.tablename LIKE 'esr%') OR
                      (tables.tablename = 'employees')
RETURN 0