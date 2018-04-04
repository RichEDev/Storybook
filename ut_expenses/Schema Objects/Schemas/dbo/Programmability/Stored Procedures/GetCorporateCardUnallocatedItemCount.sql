CREATE PROCEDURE [dbo].[GetCorporateCardUnallocatedItemCount] @statementId INT
 ,@employeeId INT
AS
BEGIN
SELECT count(transactionid)
FROM [employee_transactions]
LEFT JOIN [employee_corporate_cards] ON [employee_corporate_cards].[corporatecardid] = [employee_transactions].[corporatecardid]
WHERE employee_transactions.statementid = @statementId
	AND employee_corporate_cards.employeeid = @employeeId
	AND (
		unallocated_amount IS NULL
		OR unallocated_amount > 0
		)
END