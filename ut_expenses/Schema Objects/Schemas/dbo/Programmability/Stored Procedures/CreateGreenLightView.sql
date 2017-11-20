CREATE PROCEDURE CreateGreenLightView (@entityGuid UNIQUEIDENTIFIER)
AS
BEGIN
	DECLARE @ViewName NVARCHAR(200)
	DECLARE @CustomEntityName NVARCHAR(200)
	DECLARE @CustomEntityId INT
	DECLARE @FieldListSQL NVARCHAR(MAX)
	DECLARE @LeftJoinListItemSQL NVARCHAR(MAX) = ''
	DECLARE @LeftJoinRelatedTablesSQL NVARCHAR(MAX) = ''

	SELECT @ViewName = replace(entity_name, ' ', '') + 'View'
		,@CustomEntityName = 'Custom_' + cast(entityid AS NVARCHAR(200))
		,@CustomEntityId = entityid
	FROM customentities
	WHERE systemcustomentityid = @entityGuid;

	WITH ABC (attributeid,colname)
	AS (
		SELECT 1
			,CAST('' AS NVARCHAR(MAX))
		
		UNION ALL
		
		SELECT B.attributeid + 1
			,B.colname + A.colname + CHAR(10) + ','
		FROM (
			SELECT Row_Number() OVER (
					ORDER BY attributeid
					) AS RN
				,CASE 
					WHEN customEntityAttributes.fieldtype = 4
						THEN replace(display_name, ' ', '') + '.item'
					ELSE @CustomEntityName + '.' + 'att' + cast(attributeid AS NVARCHAR(100))
					END + ' AS [' + CASE 
					WHEN customEntityAttributes.fieldtype = 21
						THEN replace(customEntityAttributes.[description], ' ', '')
					ELSE replace(display_name, ' ', '')
					END + ']' AS colname
			FROM customEntityAttributes
			WHERE entityid = @CustomEntityId
				AND customEntityAttributes.fieldtype NOT IN (
					19
					,21
					,9
					)
				AND (
					system_attribute = 0
					OR is_key_field = 1
					)
				AND display_name NOT LIKE '%_validate'
			) A
		INNER JOIN ABC B ON A.RN = B.attributeid
		)
	SELECT TOP 1 @FieldListSQL = colname
	FROM ABC
	ORDER BY attributeid DESC;

	WITH ABC (
		attributeid
		,colname
		)
	AS (
		SELECT 1
			,CAST('' AS NVARCHAR(MAX))
		
		UNION ALL
		
		SELECT B.attributeid + 1
			,B.colname + A.colname + CHAR(10) + ','
		FROM (
			SELECT Row_Number() OVER (
					ORDER BY attributeid
					) AS RN
				,replace(display_name, ' ', '') + '.' + fields.field + ' AS [' + replace(display_name, ' ', '') + ']' AS colname
			FROM customEntityAttributes
			INNER JOIN tables ON tables.tableid = customEntityAttributes.relatedtable
			INNER JOIN fields ON fields.fieldid = tables.keyfield
			WHERE entityid = @CustomEntityId
				AND customEntityAttributes.fieldtype IN (9)
				AND display_name NOT LIKE '%_validate'
			) A
		INNER JOIN ABC B ON A.RN = B.attributeid
		)
	SELECT TOP 1 @FieldListSQL = @FieldListSQL + colname
	FROM ABC
	ORDER BY attributeid DESC

	SET @FieldListSQL = @FieldListSQL + @CustomEntityName + '.[CreatedOn],' + @CustomEntityName + '.[ModifiedOn]';

	WITH ABC (
		attributeid
		,colname
		)
	AS (
		SELECT 1
			,CAST('' AS NVARCHAR(MAX))
		
		UNION ALL
		
		SELECT B.attributeid + 1
			,B.colname + A.colname + CHAR(10)
		FROM (
			SELECT Row_Number() OVER (
					ORDER BY attributeid
					) AS RN
				,'LEFT JOIN customEntityAttributeListItems ' + replace(display_name, ' ', '') + ' ON ' + @CustomEntityName + '.att' + cast(attributeid AS NVARCHAR(100)) + ' = ' + replace(display_name, ' ', '') + '.valueid' AS colname
			FROM customEntityAttributes
			WHERE entityid = @CustomEntityId
				AND fieldtype IN (4)
				AND (
					system_attribute = 0
					OR is_key_field = 1
					)
				AND display_name NOT LIKE '%_validate'
			) A
		INNER JOIN ABC B ON A.RN = B.attributeid
		)
	SELECT TOP 1 @LeftJoinListItemSQL = colname
	FROM ABC
	ORDER BY attributeid DESC

	SET @LeftJoinListItemSQL = left(@LeftJoinListItemSQL, len(@LeftJoinListItemSQL) - 1);

	WITH ABC (
		attributeid
		,colname
		)
	AS (
		SELECT 1
			,CAST('' AS NVARCHAR(MAX))
		
		UNION ALL
		
		SELECT B.attributeid + 1
			,B.colname + A.colname + CHAR(10)
		FROM (
			SELECT Row_Number() OVER (
					ORDER BY attributeid
					) AS RN
				,'LEFT JOIN ' + tables.tablename + ' ' + replace(display_name, ' ', '') + ' ON ' + @CustomEntityName + '.att' + cast(customEntityAttributes.attributeid AS NVARCHAR(10)) + ' = ' + replace(display_name, ' ', '') + '.' + fields.field AS colname
			FROM customEntityAttributes
			INNER JOIN tables ON tables.tableid = customEntityAttributes.relatedtable
			INNER JOIN fields ON fields.fieldid = tables.primarykey
			WHERE entityid = @CustomEntityId
				AND customEntityAttributes.fieldtype IN (9)
				AND system_attribute = 0
				AND display_name NOT LIKE '%_validate'
			) A
		INNER JOIN ABC B ON A.RN = B.attributeid
		)
	SELECT TOP 1 @LeftJoinRelatedTablesSQL = colname
	FROM ABC
	ORDER BY attributeid DESC

	SET @LeftJoinRelatedTablesSQL = left(@LeftJoinRelatedTablesSQL, len(@LeftJoinRelatedTablesSQL) - 1)
	SET @LeftJoinRelatedTablesSQL = @LeftJoinRelatedTablesSQL + '
	LEFT JOIN employees CreatedBy ON ' + @CustomEntityName + '.CreatedBy = CreatedBy.employeeid
	LEFT JOIN employees ModifiedBy ON ' + @CustomEntityName + '.ModifiedBy = ModifiedBy.employeeid
	'

	DECLARE @dropsql NVARCHAR(max) = 'drop view ' + QUOTENAME(@ViewName)
	DECLARE @createsql NVARCHAR(MAX) = '
		CREATE VIEW ' + @ViewName + '
		AS 
		SELECT
			' + @FieldListSQL + '
		FROM ' + @CustomEntityName + '
		' + @LeftJoinListItemSQL + '
		' + @LeftJoinRelatedTablesSQL

	IF EXISTS (
			SELECT *
			FROM sys.VIEWS
			WHERE NAME = @ViewName
			)
		EXEC sp_executesql @dropsql

	EXEC sp_executesql @createsql
END
GO