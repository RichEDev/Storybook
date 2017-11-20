CREATE PROCEDURE flagItem
	@expenseID INT,
	@flagID INT, 
	@flagType TINYINT,
	@flagText NVARCHAR(MAX),
	@duplicateExpenseID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO dbo.savedexpensesFlags (expenseid , flagID , flagType , flagText,duplicateExpenseID) VALUES  (@expenseID, @flagID, @flagType, @flagText, @duplicateExpenseID)
END
