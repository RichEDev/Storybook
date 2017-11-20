-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[displayUnallocate] (@groupid int, @stage tinyint) 
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @signofftype tinyint;
	
	-- Add the T-SQL statements to compute the return value here
	SELECT @signofftype = signofftype from signoffs where groupid = @groupid and stage = @stage

	if @signofftype = 3
		begin
			return 1;
		end
	
			return 0;
	
	

END
