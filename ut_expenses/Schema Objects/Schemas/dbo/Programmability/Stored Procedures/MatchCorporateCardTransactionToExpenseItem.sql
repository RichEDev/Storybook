CREATE PROCEDURE [dbo].[MatchCorporateCardTransactionToExpenseItem]
	(@transactionId INT,
	@expenseId INT)
AS
BEGIN
	DECLARE @claimantsSettleBill BIT = 0;

	SELECT @claimantsSettleBill = corporate_cards.claimants_settle_bill FROM card_transactions
	INNER JOIN card_statements_base ON card_transactions.statementid = card_statements_base.statementid
	INNER JOIN corporate_cards ON card_statements_base.cardproviderid = corporate_cards.cardproviderid
	 WHERE transactionid = @transactionId

	 UPDATE savedexpenses SET transactionid = @transactionid, amountpayable = 
	 (CASE WHEN @claimantsSettleBill = 1 AND (SELECT reimbursable FROM subcats WHERE subcatid = savedexpenses.subcatid) = 1 THEN total ELSE 0 END)
	  WHERE expenseid = @expenseId OR expenseid IN (SELECT splititem FROM savedexpensesSplitHierarchy WHERE primaryitem = @expenseId)
END
