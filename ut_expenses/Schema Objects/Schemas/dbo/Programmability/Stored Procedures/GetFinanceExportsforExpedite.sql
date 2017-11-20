CREATE PROCEDURE [dbo].[GetFinanceExportsforExpedite] (@expeditePaymentProcessStatus INT)
AS
BEGIN
	--IF the Finance Exports are downloaded @expeditePaymentProcessStatus =1
	--Below Select Query Brings financial Exports whcih are paid and ready to mark as executed
	IF (@expeditePaymentProcessStatus = 1)
	BEGIN
		SELECT DISTINCT FE.financialexportid
			,RE.reportname
			,SUM(SE.amountpayable) AS Amount
			,ExpeditePaymentProcessStatus
		FROM financial_exports FE
		INNER JOIN reports RE ON RE.reportID = FE.reportID
		INNER JOIN exporthistory EH ON EH.financialexportid = FE.financialexportid
		INNER JOIN exporteditems EI ON EI.exporthistoryid = EH.exporthistoryid
		INNER JOIN savedexpenses SE ON SE.expenseid = EI.expenseid
		INNER JOIN claims_base CB ON CB.claimid = SE.claimid
		WHERE ExpeditePaymentProcessStatus = @expeditePaymentProcessStatus
			AND ExpeditePaymentReport = 1
			AND CB.STATUS = 10
			AND CB.Approved = 1
			AND CB.Paid = 1
		GROUP BY FE.financialexportid
			,RE.reportname
			,ExpeditePaymentProcessStatus
	END

	--IF the Finance Exports are downloaded @expeditePaymentProcessStatus =0
	--Below Select Query Brings financial Exports whcih are ready for physical payment
	IF (@expeditePaymentProcessStatus = 0)
	BEGIN
		SELECT financialexportid
			,RE.reportname
			,0.00 AS Amount
			,ExpeditePaymentProcessStatus
		FROM financial_exports FE
		INNER JOIN reports RE ON RE.reportID = FE.reportID
		WHERE ExpeditePaymentReport = 1
		
		UNION ALL
		
		SELECT DISTINCT FE.financialexportid
			,RE.reportname
			,SUM(SE.amountpayable) AS Amount
			,ExpeditePaymentProcessStatus
		FROM financial_exports FE
		INNER JOIN reports RE ON RE.reportID = FE.reportID
		INNER JOIN exporthistory EH ON EH.financialexportid = FE.financialexportid
		INNER JOIN exporteditems EI ON EI.exporthistoryid = EH.exporthistoryid
		INNER JOIN savedexpenses SE ON SE.expenseid = EI.expenseid
		INNER JOIN claims_base CB ON CB.claimid = SE.claimid
		WHERE ExpeditePaymentProcessStatus = 1
			AND ExpeditePaymentReport = 1
			AND CB.STATUS <> 9
			AND CB.Approved = 1
			AND CB.Paid = 1
		GROUP BY FE.financialexportid
			,RE.reportname
			,ExpeditePaymentProcessStatus
	END
END
