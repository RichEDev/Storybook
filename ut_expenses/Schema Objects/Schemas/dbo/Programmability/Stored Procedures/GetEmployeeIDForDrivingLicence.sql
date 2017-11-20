CREATE PROCEDURE GetEmployeeIDForDrivingLicence (
@DoCEntityId int = 1119
)
AS

	DECLARE @IDAttributeColumn NVarChar(50)
	DECLARE @EntityIDTable NVARCHAR(50) 
	DECLARE @EmployeeIdColumn NVARCHAR(50)

	DECLARE @EntityId int 

	SELECT @IDAttributeColumn = 'att' + CAST(attributeid AS NVARCHAR(10))
		, @EntityIDTable = 'custom_' + CAST(entityid AS NVARCHAR(10)) 
		, @EntityId = entityid
	FROM customEntityAttributes 
	WHERE SystemCustomEntityAttributeID= 'C11ADAEB-27E0-4046-843F-7FD39513DACF'

	SELECT @EmployeeIdColumn = 'att' + CAST(attributeid AS NVARCHAR(10)) 
	FROM customEntityAttributes
	WHERE entityid = @EntityId AND display_name = 'Employee'

	DECLARE @SQL NVARCHAR(MAX) = 'SELECT '+@EmployeeIdColumn+' as employeeid FROM '+ @EntityIDTable + ' WHERE ' + @IDAttributeColumn + ' = ' + CAST(@DoCEntityId AS NVARCHAR(10))
	
	EXEC sp_executesql @SQL
