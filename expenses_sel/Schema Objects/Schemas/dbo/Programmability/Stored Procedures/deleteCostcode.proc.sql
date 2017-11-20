


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteCostcode]
	@costcodeid INT,
	@employeeID int,
	@delegateID int
AS
BEGIN

DECLARE @count INT;
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @count = (select count(*) from signoffs where include = 4 and includeid = @costcodeid)
    IF @count > 0
		RETURN -1;
		
	update [savedexpenses_costcodes] set [costcodeid] = null where costcodeid = @costcodeid;
	update employee_costcodes set costcodeid = null where costcodeid = @costcodeid;
	
	DECLARE @costcode NVARCHAR(50);
	SELECT @costcode = costcode FROM costcodes WHERE costcodeid = @costcodeid
	DELETE FROM costcodes WHERE costcodeid = @costcodeid;
	
	EXEC addDeleteEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, @costcode, null
	RETURN 0;
END
