
CREATE PROCEDURE [dbo].[deleteCar]
	@carid INT,
	@userid INT,
	@date datetime,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @employeeid INT;
	declare @recordTitle nvarchar(2000);
	declare @count int

	--Check if the trust is associated to a financial export
	set @count = (select count(expenseid) from savedexpenses where carid = @carid)
	if @count > 0
		return 1;
		
	SELECT @employeeid = employeeid, @recordTitle = registration FROM cars WHERE carid = @carid;
	
	delete from cars where carid = @carid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, @recordTitle, null;
   
	IF @employeeid <> 0
		UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeid;
	
	return 0;
END
