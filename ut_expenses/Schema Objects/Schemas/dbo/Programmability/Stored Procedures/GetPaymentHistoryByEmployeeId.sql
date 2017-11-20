
CREATE PROC [dbo].[GetPaymentHistoryByEmployeeId]
@EmployeeId int 
AS
SELECT DISTINCT savedexpenses.amountpayable 'Amount Payable',claims_base.datepaid 'Date Paid' 
FROM claims_base 
    INNER JOIN savedexpenses ON claims_base.claimid=savedexpenses.claimid
    INNER JOIN exporteditems ON savedexpenses.expenseid=exporteditems.expenseid
    INNER JOIN exporthistory ON exporthistory.exporthistoryid=exporteditems.exporthistoryid
    INNER JOIN financial_exports ON financial_exports.financialexportid=exporthistory.financialexportid 
    WHERE 
    ExpeditePaymentProcessStatus=2 AND
    financial_exports.ExpeditePaymentReport=1 AND 
    claims_base.employeeid=@EmployeeId
