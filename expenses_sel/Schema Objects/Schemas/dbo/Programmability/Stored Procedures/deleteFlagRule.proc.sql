﻿CREATE PROCEDURE [dbo].[deleteFlagRule]
	@flagID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM flags WHERE flagID = @flagID
END

