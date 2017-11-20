CREATE PROCEDURE [dbo].[APIdeleteEsrElementFields]
	@elementFieldID int 
	
AS
BEGIN
	DELETE FROM ESRElementFields WHERE @elementFieldID = elementFieldID
END