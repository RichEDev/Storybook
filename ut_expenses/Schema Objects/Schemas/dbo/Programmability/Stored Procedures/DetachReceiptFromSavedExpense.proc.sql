CREATE PROCEDURE [dbo].[DetachReceiptFromSavedExpense]
 @receiptId int,
 @savedExpenseId int,
 @userId int
AS
BEGIN
 SET NOCOUNT ON;
 DELETE FROM ReceiptOwnership
 WHERE ReceiptId = @receiptId AND SavedExpenseId = @savedExpenseId;
 
 UPDATE Receipts
 SET ModifiedBy = @userId, ModifiedOn = CURRENT_TIMESTAMP
 WHERE ReceiptId = @receiptId
 
 DECLARE @textReceiptId nvarchar(100) = CONVERT(nvarchar, @receiptId); 
 DECLARE @textExpenseId nvarchar(100) = CONVERT(nvarchar, @savedExpenseId);
 DECLARE @textExpenseType nvarchar(100) = (SELECT subcat FROM subcats INNER JOIN savedExpenses ON savedExpenses.subcatid = subcats.subcatid WHERE savedExpenses.expenseid = @savedExpenseId);
  DECLARE @textExpenseDate nvarchar(100); 
 DECLARE @claimId INT;
 SELECT @textExpenseDate = CONVERT(VARCHAR(8), [date], 3), @claimId = claimid FROM savedexpenses WHERE expenseid = @savedExpenseId;
 DECLARE @claimName NVARCHAR(100);
 SELECT @claimName = [name] FROM claims WHERE claimid = @claimId;
  
 DECLARE @log nvarchar(4000) = 'Receipt (' + @textReceiptId + ') detached from expense item ' + @textExpenseId + ' (' + @textExpenseType + ' on ' + @textExpenseDate + '), claim (' + @claimName + ')';
 EXEC addUpdateEntryToAuditLog @userId, @userId, 186, @receiptId, NULL, @textExpenseId, NULL, @log, NULL;
 
UPDATE savedexpenses SET receiptattached = 1 WHERE savedexpenses.expenseid = @savedExpenseId 
UPDATE savedexpenses SET ValidationProgress = CASE WHEN ValidationProgress = 0 AND receiptattached = 0 THEN -4 END WHERE expenseid = @savedExpenseId

 RETURN @receiptId;
END
GO