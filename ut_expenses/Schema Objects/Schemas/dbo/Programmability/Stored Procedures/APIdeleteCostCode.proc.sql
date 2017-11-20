CREATE PROCEDURE [dbo].[APIdeleteCostCode]
	@costcodeid int
AS
	DELETE FROM costcodes WHERE [costcodeid] = @costcodeid;
RETURN 0