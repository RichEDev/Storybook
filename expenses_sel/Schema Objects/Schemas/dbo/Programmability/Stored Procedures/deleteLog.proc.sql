

CREATE PROCEDURE [dbo].[deleteLog] 
	
@logID int

AS
BEGIN	
	DELETE FROM logNames WHERE logID = @logID
END

