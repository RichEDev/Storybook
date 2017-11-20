CREATE PROCEDURE [GetStatementTransactionsForEmployee] @EmployeeId  INT, 
                                                       @StatementId INT 
AS 
  BEGIN 
      SELECT dbo.employee_transactions.transactionid, 
             dbo.employee_transactions.transaction_date, 
             dbo.employee_transactions.description, 
             dbo.employee_transactions.card_number, 
             dbo.employee_transactions.transaction_amount, 
             dbo.employee_transactions.original_amount, 
             dbo.employee_transactions.label, 
             dbo.employee_transactions.exchangerate, 
             dbo.employee_transactions.country, 
             dbo.employee_transactions.allocated_amount, 
             dbo.employee_transactions.unallocated_amount,
			 dbo.employee_transactions.currencySymbol,  
			 dbo.employee_transactions.currencyid  
      FROM   dbo.employee_transactions 
             INNER JOIN dbo.employee_corporate_cards 
                     ON dbo.employee_transactions.corporatecardid = 
                        dbo.employee_corporate_cards.corporatecardid 
      WHERE  ( dbo.employee_transactions.statementid = @StatementId ) 
             AND ( dbo.employee_corporate_cards.employeeid = @EmployeeId ) 
             AND ( dbo.employee_transactions.unallocated_amount IS NULL 
                    OR dbo.employee_transactions.unallocated_amount > 0 ) 
      ORDER BY dbo.employee_transactions.transaction_date ASC
  END 