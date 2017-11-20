CREATE PROCEDURE [dbo].[CheckIfListItemIsUsedWithinFieldFilter] 
	@listItemID nvarchar(20),
	@fieldID uniqueidentifier

AS
BEGIN

	DECLARE @filterValues nvarchar(max);
	DECLARE @found int = 0;

	DECLARE viewField_cursor CURSOR FOR 
	SELECT value 
	FROM fieldFilters 
	WHERE fieldid = @fieldID

	OPEN viewField_cursor;

	FETCH NEXT FROM viewField_cursor 
	INTO @filterValues;

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF @filterValues LIKE '%' + @listItemID + '%'
		BEGIN
			SET @found = 1;					
			GOTO ENDCURSOR;
		END			
			
		FETCH NEXT FROM viewField_cursor 
		INTO @filterValues;

	END
	
	ENDCURSOR:
	CLOSE viewField_cursor;
	DEALLOCATE viewField_cursor;

	RETURN @found;
END