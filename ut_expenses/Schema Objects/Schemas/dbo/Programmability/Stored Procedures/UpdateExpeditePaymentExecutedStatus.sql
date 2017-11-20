CREATE PROCEDURE [dbo].[UpdateExpeditePaymentExecutedStatus] @financeExports Int_TinyInt_Int readonly
	,@fund MONEY
	,@AvailableFund MONEY
AS
DECLARE @isUpdated INT

SET @isUpdated = 0

DECLARE @finexportid INT

DECLARE finExport_cursor CURSOR
FOR
SELECT e.[c1]
FROM @financeExports e

OPEN finExport_cursor

FETCH NEXT
FROM finExport_cursor
INTO @finexportid

WHILE @@FETCH_STATUS = 0
BEGIN
	IF EXISTS (
			SELECT DISTINCT 1
			FROM [dbo].[financial_exports] finExp
			INNER JOIN exporthistory EH ON EH.financialexportid = finExp.financialexportid
			INNER JOIN exporteditems EI ON EI.exporthistoryid = EH.exporthistoryid
			INNER JOIN savedexpenses SE ON SE.expenseid = EI.expenseid
			INNER JOIN claims_base CB ON CB.claimid = SE.claimid
			WHERE CB.STATUS = 10
				AND CB.Approved = 1
				AND SE.Paid = 1
				AND finExp.financialexportid = @finexportid
			)
	BEGIN
		INSERT INTO FundTransaction
		VALUES (
			GetUTCDate()
			,@fund
			,2
			,@AvailableFund
			)
	END

	UPDATE finExp
	SET finExp.ExpeditePaymentProcessStatus = e.c2
	FROM @financeExports e
	INNER JOIN [dbo].[financial_exports] finExp ON finExp.[financialexportid] = e.[c1];

	UPDATE CB
	SET CB.STATUS = 9
	FROM @financeExports e
	INNER JOIN [dbo].[financial_exports] finExp ON finExp.[financialexportid] = e.[c1]
	INNER JOIN exporthistory EH ON EH.financialexportid = finExp.financialexportid
	INNER JOIN exporteditems EI ON EI.exporthistoryid = EH.exporthistoryid
	INNER JOIN savedexpenses SE ON SE.expenseid = EI.expenseid
	INNER JOIN claims_base CB ON CB.claimid = SE.claimid
	WHERE CB.STATUS = 10
		AND CB.Approved = 1
		AND SE.Paid = 1
		AND finExp.financialexportid = @finexportid

	SET @isUpdated = 1

	FETCH NEXT
	FROM finExport_cursor
	INTO @finexportid
END

CLOSE finExport_cursor

DEALLOCATE finExport_cursor

IF (@@ERROR > 0)
BEGIN
	SET @isUpdated = 0
END

IF (@@ERROR > 0)
BEGIN
	SET @isUpdated = 0
END

RETURN @isUpdated
