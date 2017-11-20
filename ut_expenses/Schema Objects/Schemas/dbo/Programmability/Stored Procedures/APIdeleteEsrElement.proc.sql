CREATE PROCEDURE [dbo].[APIdeleteEsrElement]
	@elementID int 
	
AS
BEGIN
	DELETE FROM ESRElements WHERE @elementID = elementID
END