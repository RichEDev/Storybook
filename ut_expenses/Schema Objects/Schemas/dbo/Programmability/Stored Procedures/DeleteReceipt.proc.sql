CREATE PROCEDURE [dbo].[DeleteReceipt]
	@receiptId INT,
	@trueDelete bit = 0,
	@userId INT
AS
BEGIN
 SET NOCOUNT ON;

	-- find the owner of the receipt
	DECLARE @owner int;
	DECLARE @rUser int;
	DECLARE @rClaim int;
	SELECT @rUser = UserId, @rClaim = ClaimId FROM Receipts WHERE ReceiptId = @receiptId;
	
	IF @rUser IS NOT NULL SET @owner = @rUser
	ELSE IF @rClaim IS NOT NULL SELECT @owner = EmployeeID FROM claims_base WHERE claimid = @rClaim
	ELSE BEGIN
		SET @rClaim = (SELECT TOP 1 ClaimId FROM savedexpenses WHERE expenseid = (SELECT TOP 1 SavedExpenseId FROM ReceiptOwnership WHERE ReceiptId = @receiptId));
		SET @owner = (SELECT TOP 1 employeeid FROM claims_base WHERE claimid = @rClaim);
	END
	
	-- set the owner of the receipt to the user
	UPDATE Receipts SET UserId = @owner WHERE ReceiptId = @receiptId;

	-- Get a list from the ReceiptOwnership join table of all the savedExpenses using the receipt, and their ClaimId
	DECLARE @expensesUsingThisReceipt TABLE ( ExpenseId int);
	INSERT INTO @expensesUsingThisReceipt (ExpenseId) 
	(SELECT SavedExpenseId FROM ReceiptOwnership WHERE ReceiptId = @receiptId);
		
	-- Delete the Receipt/Expense join link
	DELETE FROM ReceiptOwnership WHERE ReceiptId = @receiptId;
	
	-- meant to be:
	-- for each savedExpenseId in the list above, 
	-- look through the ReceiptOwnership table again for how many receipts are mapped to the savedExpense
	-- set teh receiptsAttached accordingly
	UPDATE savedexpenses 
	SET receiptattached = (CASE WHEN (SELECT COUNT (ReceiptId) FROM ReceiptOwnership WHERE ReceiptOwnership.SavedExpenseId = savedexpenses.expenseid) > 0 THEN 1 ELSE 0 END)
	WHERE savedexpenses.expenseid IN (SELECT ExpenseId FROM @expensesUsingThisReceipt)	
	
	-- then do the delete.
	IF (@trueDelete = 0)
		BEGIN
			UPDATE Receipts
			SET Deleted = 1, ModifiedBy = @userId, ModifiedOn = CURRENT_TIMESTAMP
			WHERE ReceiptId = @receiptId;
		END
	ELSE
		BEGIN
			DELETE FROM Receipts 
			WHERE ReceiptId = @receiptId;
		END

    DECLARE @log nvarchar(4000) = 'Receipt (' + CONVERT(NVARCHAR, @receiptId) + ') deleted';
	EXEC addDeleteEntryToAuditLog @userId, @userId, 186, @receiptId, @log, NULL;

	RETURN 1;	
 END
GO