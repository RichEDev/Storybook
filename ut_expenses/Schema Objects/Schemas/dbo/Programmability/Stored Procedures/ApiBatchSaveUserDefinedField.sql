
CREATE PROCEDURE [dbo].[ApiBatchSaveUserDefinedField] @list ApiBatchSaveUserDefinedFieldType READONLY
AS
BEGIN
	DECLARE @index BIGINT
	DECLARE @count BIGINT
	DECLARE @userdefineid INT
	DECLARE @field NVARCHAR(100)
	DECLARE @fieldid UNIQUEIDENTIFIER
	DECLARE @tablename NVARCHAR(100)
	DECLARE @tableid UNIQUEIDENTIFIER
	DECLARE @fieldtype TINYINT
	DECLARE @value NVARCHAR(max)
	DECLARE @recordId INT
	DECLARE @displayField UNIQUEIDENTIFIER
	DECLARE @relatedtable UNIQUEIDENTIFIER
	DECLARE @keyField NVARCHAR(100);
	DECLARE @sqlParams NVARCHAR(250) = '@recId int, @dataValue ';
	DECLARE @dataType NVARCHAR(15);
	DECLARE @precision TINYINT;
	DECLARE @maxlength INT;
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,userdefineid BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY userdefineid
			)
		,userdefineid
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @userdefineid = (
				SELECT TOP 1 userdefineid
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @field = field
			,@fieldid = fieldid
			,@tablename = tablename
			,@tableid = tableid
			,@fieldtype = fieldtype
			,@value = value
			,@recordId = recordId
			,@displayField = displayField
			,@relatedtable = relatedtable
		FROM @list
		WHERE userdefineid = @userdefineid

		IF @fieldtype = 1
		BEGIN
			SELECT @maxlength = maxlength
			FROM userdefined
			WHERE fieldid = @fieldid;

			IF @maxlength IS NULL
				OR @maxlength = 0
			BEGIN
				SET @maxlength = 4000;
			END

			SET @dataType = 'nvarchar(' + cast(@maxlength AS NVARCHAR) + ')';
		END
		ELSE
			IF @fieldtype = 7
			BEGIN
				SELECT @precision = [precision]
				FROM userdefined
				WHERE fieldid = @fieldid

				SET @dataType = 'decimal(18,' + cast(@precision AS NVARCHAR) + ')';
			END
			ELSE
			BEGIN
				SET @dataType = CASE @fieldtype
						WHEN 2
							THEN 'int'
						WHEN 3
							THEN 'datetime'
						WHEN 4
							THEN 'nvarchar(50)'
						WHEN 5
							THEN 'bit'
						WHEN 6
							THEN 'money'
						WHEN 9
							THEN 'int'
						WHEN 16
							THEN 'nvarchar(2000)'
						ELSE 'nvarchar(max)' -- field types 8,10
						END
			END

		SET @sqlParams = @sqlParams + @dataType;

		SELECT @keyField = field
		FROM fields
		WHERE tableid = @tableid
			AND idfield = 1;

		DECLARE @sql NVARCHAR(1000) = 'IF NOT EXISTS (SELECT ' + @keyField + ' FROM ' + @tablename + ' WHERE ' + @keyField + ' = @recId) BEGIN ';

		SET @sql = @sql + 'INSERT INTO ' + @tablename + ' (' + @keyField + ',' + @field + ') VALUES (@recId, @dataValue)';
		SET @sql = @sql + ' END ELSE BEGIN ';
		SET @sql = @sql + 'UPDATE ' + @tablename + ' set ' + @field + ' = @dataValue WHERE ' + @keyField + ' = @recId';
		SET @sql = @sql + ' END';

		EXECUTE sp_executesql @sql
			,@sqlParams
			,@recId = @recordId
			,@dataValue = @value;

		SET @index = @index + 1
	END

	RETURN 0;
END