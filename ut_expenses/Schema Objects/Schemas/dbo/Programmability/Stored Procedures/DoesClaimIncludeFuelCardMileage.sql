CREATE PROCEDURE DoesClaimIncludeFuelCardMileage 
	@claimID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select count(savedexpenses.expenseid) from savedexpenses inner join subcats on subcats.subcatid = savedexpenses.subcatid where savedexpenses.claimid = @claimid and subcats.calculation = 8
END
GO