CREATE PROCEDURE [dbo].[APIdeleteEsrTrust]
	@trustID int 
	
AS
BEGIN
	DELETE FROM esrTrusts WHERE trustID = @trustID
END