CREATE PROCEDURE [dbo].[GetClaimsAwaitingApprovalByEmployee] @employeeId INT
AS
BEGIN
 SELECT c.employeeid AS employeeId
  ,c.empname  AS employeeName
  ,c.claimid AS claimId
  ,c.claimno AS claimNumber
  ,c.description as description
  ,c.STATUS
  ,c.checkerid AS checkerId
  ,c.stage
  ,c.NAME AS claimName
  ,c.basecurrency AS baseCurrencyId
  ,c.ReferenceNumber AS referenceNumber
  ,c.currencySymbol
  ,c.Total
  ,c.displaysinglesignoff
  ,c.ClaimItemsCount as numberOfItems
  ,CAST(1 as bit) as Submitted
  ,c.datesubmitted as dateSubmitted
  ,c.datePaid as datePaid
  ,c.displayunallocate as canBeUnassigned
  ,c.itemCheckerId
     FROM [checkandpay] c where (checkerid = @employeeId OR (itemcheckerid is not null and itemcheckerid = @employeeId))
 ORDER BY claimNumber
END
