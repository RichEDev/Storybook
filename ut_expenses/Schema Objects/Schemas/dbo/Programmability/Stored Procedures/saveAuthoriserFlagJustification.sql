CREATE PROCEDURE [dbo].[saveAuthoriserFlagJustification] 
	@flaggedItemId int,
	@justification nvarchar(max),
	@authoriser int,
	@delegateID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @stage int

	select @stage = claims_base.stage from savedexpensesFlags inner join savedexpenses on savedexpenses.expenseid = savedexpensesFlags.expenseid inner join claims_base on claims_base.claimid = savedexpenses.claimid where savedexpensesFlags.flaggeditemid = @flaggedItemId
    -- Insert statements for procedure here

    if @justification is null
		delete from savedExpensesFlagsApproverJustifications where flaggedItemId = @flaggedItemId and stage = @stage
	else
		if (select count(historyid) from savedExpensesFlagsApproverJustifications where flaggedItemId = @flaggedItemId and stage = @stage) > 0
			update savedExpensesFlagsApproverJustifications set justification = @justification, delegateID = @delegateID where flaggedItemId = @flaggedItemId and stage = @stage
		else
			insert into savedexpensesFlagsApproverJustifications (flaggeditemId, stage, approverId, justification, delegateID)
				values (@flaggedItemId, @stage, @authoriser, @justification, @delegateID)
END

GO