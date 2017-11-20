
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


	-- CHECK TO SEE IF THE COSTCODE IS ON USE #1827
	SET @count = (select count(*) from signoffs where include = 4 and includeid = @costcodeid)
	IF @count > 0
		RETURN -1;	
	SET @count = (select count (costcodeid) from employee_costcodes where costcodeid = @costcodeid)
	IF @count > 0
		RETURN -2;	
	SET @count = (select count (costcodeid) from savedexpenses_costcodes where costcodeid = @costcodeid)		
	IF @count > 0
		RETURN -4;
	-- CHECK END		

	declare @tableid uniqueidentifier = (select tableid from tables where tablename = 'costcodes');
	exec @count = dbo.checkReferencedBy @tableid, @costcodeid;

	if @count = 0
	begin
		update [savedexpenses_costcodes] set [costcodeid] = null where costcodeid = @costcodeid;
		update employee_costcodes set costcodeid = null where costcodeid = @costcodeid;
	
		DECLARE @costcode NVARCHAR(50);
		SELECT @costcode = costcode FROM costcodes WHERE costcodeid = @costcodeid
		DELETE FROM costcodes WHERE costcodeid = @costcodeid;
	
		EXEC addDeleteEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, @costcode, null
	end

	RETURN @count;
END
GO

