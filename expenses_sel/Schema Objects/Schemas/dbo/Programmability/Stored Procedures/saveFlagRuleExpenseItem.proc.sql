CREATE PROCEDURE [dbo].[saveFlagRuleExpenseItem]
	-- Add the parameters for the stored procedure here
	@flagID INT,
	@subcatID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO dbo.flagAssociatedExpenseItems ( flagID, subCatID ) VALUES  (@flagID, @subcatID)
END
