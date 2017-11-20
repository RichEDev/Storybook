CREATE PROCEDURE [dbo].[UpdateExpeditePaymentStatus] @financeExports Int_TinyInt_Int readonly
AS
DECLARE @isUpdated INT

SET @isUpdated = 0

UPDATE finExp
SET finExp.ExpeditePaymentProcessStatus = e.c2
FROM @financeExports e
INNER JOIN [dbo].[financial_exports] finExp ON finExp.[financialexportid] = e.[c1];

SET @isUpdated = 1

IF (@@ERROR > 0)
BEGIN
	SET @isUpdated = 0
END
ELSE
BEGIN
	UPDATE claims_base
	SET claims_base.STATUS = 10
	FROM @financeExports e
	INNER JOIN exporteditems EI ON EI.exporthistoryid = e.c3
	INNER JOIN savedexpenses SE ON SE.expenseid = EI.expenseid
	INNER JOIN claims_base cb ON cb.claimid = se.claimid
END

RETURN @isUpdated
