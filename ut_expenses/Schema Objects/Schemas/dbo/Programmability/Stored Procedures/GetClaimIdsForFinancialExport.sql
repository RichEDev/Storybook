CREATE PROCEDURE GetClaimIdsForFinancialExport 
@financialexportid INT
AS
BEGIN
	SELECT DISTINCT	SE.claimid FROM financial_exports FE
	INNER JOIN exporthistory EH on EH.financialexportid =FE.financialexportid
	INNER JOIN exporteditems EI on EI.exporthistoryid=EH.exporthistoryid
	INNER JOIN savedexpenses SE on SE.expenseid=EI.expenseid
	INNER JOIN claims_base CB on CB.claimid=se.claimid
	WHERE FE.financialexportid=@financialexportid AND SE.Paid=1 AND CB.approved=1 AND CB.status=10
END