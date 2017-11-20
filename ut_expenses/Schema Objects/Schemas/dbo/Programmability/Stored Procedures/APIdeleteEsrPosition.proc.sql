CREATE PROCEDURE [dbo].[APIdeleteEsrPosition]
	@ESRPositionId bigint 
	
AS
BEGIN
	UPDATE esr_assignments SET ESRPositionId = NULL WHERE ESRPositionId = @ESRPositionId;

	DELETE FROM ESRPositions WHERE ESRPositionId = @ESRPositionId
END
