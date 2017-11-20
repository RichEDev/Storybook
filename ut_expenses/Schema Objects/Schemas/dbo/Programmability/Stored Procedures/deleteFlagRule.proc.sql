CREATE PROCEDURE [dbo].[deleteFlagRule]
	@flagID int,
	@employeeID INT,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--determine if the flag has been used already
	if (select count(flaggeditemid) from savedexpensesFlags where flagid = @flagID) > 0
		return -1

	declare @recordTitle nvarchar(2000);
    select @recordTitle = description from flags where flagid = @flagID;
            
    -- Insert statements for procedure here
	DELETE FROM flags WHERE flagID = @flagID

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 33, @flagid, @recordTitle, null;
	return 0
END



GO
