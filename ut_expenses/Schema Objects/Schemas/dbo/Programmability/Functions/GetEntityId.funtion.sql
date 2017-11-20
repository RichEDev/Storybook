
CREATE FUNCTION [dbo].[GetEntityId] (@SystemCustomEntityID UNIQUEIDENTIFIER,@fullName BIT =1) 
RETURNS VARCHAR(100) AS
BEGIN

DECLARE @entityId VARCHAR(100)

 select @entityId=entityid FROM customEntities WHERE SystemCustomEntityID =@SystemCustomEntityID
 
 IF @fullName =0
 BEGIN
 	RETURN @entityId
 END	
 RETURN 'custom_'+@entityId
END

