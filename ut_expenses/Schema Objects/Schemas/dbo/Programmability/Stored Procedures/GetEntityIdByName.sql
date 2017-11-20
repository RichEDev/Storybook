
Create PROCEDURE  GetEntityIdByName
	@entityName VARCHAR(250),
	@pluralName  VARCHAR(250),
	@builtIn  BIT

AS
	DECLARE @entityId INT
	SELECT @entityId= entityid FROM customEntities WHERE entity_name = @entityName AND plural_name =@pluralName AND BuiltIn = @builtIn
	IF (@entityId = NULL) RETURN 0 ELSE RETURN @entityId