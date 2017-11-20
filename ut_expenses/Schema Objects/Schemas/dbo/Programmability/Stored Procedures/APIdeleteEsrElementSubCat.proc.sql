CREATE PROCEDURE [dbo].[APIdeleteEsrElementSubCat]
	@elementSubcatID int 
	
AS
BEGIN
	DELETE FROM ESRElementSubcats WHERE elementSubcatID = @elementSubcatID
END