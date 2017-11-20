CREATE PROCEDURE [dbo].[GetUnsubmittedClaimsByEmployee] @employeeId INT
AS
BEGIN
	SELECT dbo.employees.employeeid AS employeeId
		,dbo.employees.surname + N', ' + dbo.employees.title + N' ' + dbo.employees.firstname AS employeeName
		,dbo.claims_base.claimid AS claimId
		,dbo.claims_base.claimno AS claimNumber
		,dbo.claims_base.description
		,dbo.claims_base.STATUS
		,dbo.claims_base.checkerid AS checkerId
		,dbo.claims_base.stage
		,dbo.claims_base.NAME AS claimName
		,dbo.claims_base.currencyid AS baseCurrencyId
		,dbo.claims_base.ReferenceNumber AS referenceNumber
		,dbo.global_currencies.currencySymbol
		,dbo.claims_base.submitted
		,IsNull(SUM(dbo.savedexpenses.total), 0) AS total
		,CAST(0 AS BIT) AS displaysinglesignoff
		,sum(CASE 
				WHEN dbo.savedexpenses.primaryitem = 1
					THEN 1
				ELSE 0
				END) AS numberOfItems
		,dbo.claims_base.datesubmitted as dateSubmitted
    	,dbo.claims_base.datePaid as datePaid
     	,dbo.savedexpenses.itemCheckerId
	FROM dbo.global_currencies
	INNER JOIN dbo.currencies ON dbo.global_currencies.globalcurrencyid = dbo.currencies.globalcurrencyid
	INNER JOIN dbo.employees
	INNER JOIN dbo.claims_base ON dbo.employees.employeeid = dbo.claims_base.employeeid ON dbo.currencies.currencyid = dbo.claims_base.currencyid LEFT OUTER JOIN dbo.savedexpenses ON dbo.claims_base.claimid = dbo.savedexpenses.claimid WHERE (dbo.claims_base.submitted = 0)
		AND (dbo.employees.employeeid = @employeeId)
	GROUP BY dbo.employees.employeeid
		,dbo.employees.surname + N', ' + dbo.employees.title + N' ' + dbo.employees.firstname
		,dbo.claims_base.claimid
		,dbo.claims_base.claimno
		,dbo.claims_base.description
		,dbo.claims_base.STATUS
		,dbo.claims_base.checkerid
		,dbo.claims_base.stage
		,dbo.claims_base.NAME
		,dbo.claims_base.currencyid
		,dbo.claims_base.ReferenceNumber
		,dbo.global_currencies.currencySymbol
		,dbo.claims_base.submitted
		,dbo.claims_base.datesubmitted
        ,dbo.claims_base.datePaid 
		,dbo.savedexpenses.itemCheckerId
	ORDER BY claimNumber
END