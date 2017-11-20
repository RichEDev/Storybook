CREATE PROCEDURE [dbo].[GetGreenLightAttributeByEntityId]
 @tableId uniqueidentifier
AS 
BEGIN
SELECT fieldid,display_name from customEntityAttributes where fieldtype=23 and format = 11 and entityid = (select entityId from CustomEntities where tableId = @tableId)
END
