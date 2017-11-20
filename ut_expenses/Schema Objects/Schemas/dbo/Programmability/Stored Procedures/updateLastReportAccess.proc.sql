CREATE PROCEDURE updateLastReportAccess
	@employeeID int,
	@subaccountID int,
	@reportID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @count int
   
	set @count = (SELECT count(*) FROM [subAccountAccess] WHERE [employeeId] = @employeeId AND [subAccountId] = @subaccountID)
	if @count = 0
		insert into subAccountAccess (subAccountId, employeeId, lastReportId) values (@subaccountID, @employeeID, @reportID)		
	else
		UPDATE [subAccountAccess] SET  [lastReportId] = @reportID WHERE [employeeId] = @employeeID AND subAccountId = @subaccountID


END
