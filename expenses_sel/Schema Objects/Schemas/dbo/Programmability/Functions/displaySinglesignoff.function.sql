-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[displaySinglesignoff] (@groupid int, @stage tinyint) 
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @singlesignoff bit;
	
	-- Add the T-SQL statements to compute the return value here
	SELECT @singlesignoff = singlesignoff from signoffs where groupid = @groupid and stage = @stage
	return @singlesignoff;
	
	
	

END
