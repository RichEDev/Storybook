CREATE PROCEDURE [dbo].[deleteFlaggedItems] 
	@expenseId int,
	@flaggedItems IntPk readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from savedexpensesFlags where flaggedItemId in (select c1 from @flaggedItems)
END


GO