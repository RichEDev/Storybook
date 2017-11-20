CREATE PROCEDURE deleteExpenseItemFlags
	@expenseid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from savedexpensesFlags where expenseid = @expenseid
END
