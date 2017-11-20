
CREATE FUNCTION [dbo].[GetAttributeId] (@SystemCustomEntityAttributeID uniqueidentifier ,@fullName bit =1) 
RETURNS VARCHAR(100) AS
BEGIN

DECLARE @attributeId VARCHAR(100)

  SELECT @attributeId= attributeid FROM customEntityAttributes WHERE SystemCustomEntityAttributeID = @SystemCustomEntityAttributeID

    IF @fullName =0
    BEGIN
 	RETURN @attributeId
    END

	RETURN 'att'+@attributeId
END