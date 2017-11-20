




CREATE PROCEDURE [dbo].[deleteAudienceBudgetHolder] (@audienceBudgetHolderID INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	declare @title1 nvarchar(500);
	select @title1 = audienceName from audiences where audienceID = (select audienceID from audienceBudgetHolders where audienceBudgetHolderID = @audienceBudgetHolderID);

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Audience Budget Holder for ' + @title1);

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	DELETE FROM audienceBudgetHolders WHERE audienceBudgetHolderID=@audienceBudgetHolderID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceBudgetHolderID, @recordTitle, null;
END
