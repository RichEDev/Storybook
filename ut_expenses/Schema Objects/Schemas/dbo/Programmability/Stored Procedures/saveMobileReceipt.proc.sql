CREATE PROCEDURE [dbo].[saveMobileReceipt]
	@mobileID int,
	@receiptData varbinary(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	insert into mobileExpenseItemReceipts (mobileID, receiptData) values (@mobileID, @receiptData)
END
