CREATE VIEW [dbo].[ExpensesRequiringReceiptValidation] 
AS 
SELECT ex.claimid , ex.expenseid, clm.modifiedOn
  FROM signoffs sig LEFT JOIN groups grp ON sig.groupid = grp.groupid
  LEFT JOIN employees emp ON grp.groupid = emp.groupid
  LEFT JOIN claims_base clm ON clm.employeeid = emp.employeeid AND clm.stage = sig.stage
  LEFT JOIN savedexpenses ex ON ex.claimid = clm.claimid

  WHERE ex.ValidationProgress = 0 
  AND ex.receiptattached = 1 
  AND ex.ValidationCount < 2
  AND sig.signofftype = 101