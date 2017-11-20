CREATE PROCEDURE GetVehicleOwnerForDocumentID (
@DoCEntityId int = 1119
)
AS

	DECLARE @IDAttributeColumn NVarChar(50)
	DECLARE @EntityIDTable NVARCHAR(50) 
	DECLARE @VehicleIdColumn NVARCHAR(50)
	DECLARE @VehicleIdColoumn NVARCHAR(50)

	DECLARE @EntityId int 

	SELECT @IDAttributeColumn = 'att' + CAST(attributeid AS NVARCHAR(10))
		, @EntityIDTable = 'custom_' + CAST(entityid AS NVARCHAR(10)) 
		, @EntityId = entityid
	FROM customEntityAttributes 
	WHERE SystemCustomEntityAttributeID= '2192098D-6FB6-4374-B72D-2A86A1C1E8DF'

	SELECT @VehicleIdColoumn = 'att' + CAST(attributeid AS NVARCHAR(10)) 
	FROM customEntityAttributes
	WHERE entityid = @EntityId AND display_name = 'Vehicle'

	DECLARE @SQL NVARCHAR(MAX) = 'SELECT employeeid FROM cars INNER JOIN ' + @EntityIDTable + ' ON Cars.Carid = ' + @EntityIDTable + '.' + @VehicleIdColoumn + ' WHERE ' + @IDAttributeColumn + ' = ' + CAST(@DoCEntityId AS NVARCHAR(10))
	
	EXEC sp_executesql @SQL
