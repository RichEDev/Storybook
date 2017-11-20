
CREATE FUNCTION [dbo].[GetTableOrFieldName] (@input uniqueidentifier,@isfield bit=0)
RETURNS VARCHAR(250)
AS BEGIN
    DECLARE @return VARCHAR(250)

	IF @isfield = 0
	BEGIN
	 SELECT @return= tablename from tables WHERE tableid =@input
	END
	ELSE
	BEGIN
	SELECT @return= field FROM fields WHERE fieldid =@input
	END
    RETURN @return
END