CREATE PROCEDURE [dbo].[deleteAllowance]
	-- Add the parameters for the stored procedure here
	@allowanceid int,
	@employeeID INT,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @count int;
	set @count = (select count(*) from savedexpenses where allowanceid = @allowanceid)
	if  @count > 0
		return -1;

	set @count = (select count(*) from mobileExpenseItems where allowanceid = @allowanceid);
	if @count > 0
		return -2;
	
	declare @recordTitle nvarchar(2000);
	select @recordTitle = allowance from allowances where allowanceid = @allowanceid;

	delete from allowances where allowanceid = @allowanceid;

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 8, @allowanceid, @recordTitle, null;
return 0;
END
