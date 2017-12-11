CREATE VIEW [dbo].[unallocatedclaims]
AS
WITH
 ClaimStageSignOffTypes
 AS
 (
  SELECT DISTINCT 
   signoffs.signofftype,
   signoffs.stage,
   GroupIdsForClaimsView.claimid
  FROM
   signoffs
    INNER JOIN groups ON signoffs.groupid = groups.groupid
     INNER JOIN GroupIdsForClaimsView ON groups.groupid = GroupIdsForClaimsView.groupid
 )

SELECT
 claims_base.claimid,
 claims_base.claimno,
 claims_base.stage,
 employees.surname + ', ' + employees.title + ' ' + employees.firstname AS empname,
 claims_base.name,
 claims_base.datesubmitted,
 claims_base.[status],
 SUM(CAST (savedexpenses.normalreceipt AS tinyint)) AS numreceipts,
 SUM(CAST (savedexpenses.receiptattached AS tinyint)) AS numreceiptsattached,
 (
  SELECT
   COUNT(DISTINCT savedexpenses_flags.expenseid) AS Expr1
  FROM
   savedexpenses_flags
    INNER JOIN savedexpenses AS a ON a.expenseid = savedexpenses_flags.expenseid
  WHERE
   a.claimid = 1
 ) AS flaggeditems,
 SUM(savedexpenses.total) AS total,
 SUM(CASE WHEN itemtype = 1 THEN total ELSE 0 END) AS cash,
 SUM(CASE WHEN itemtype = 2 THEN total ELSE 0 END) AS credit,
 SUM(CASE WHEN itemtype = 3 THEN total ELSE 0 END) AS purchase, 
  CASE (
  MAX(CASE itemtype WHEN 1 THEN 1 ELSE 0 END) +
  MAX(CASE itemtype WHEN 2 THEN 3 ELSE 0 END) +
  MAX(CASE itemtype WHEN 3 THEN 5 ELSE 0 END)
  )
  WHEN 1
   THEN 1
  WHEN 3
   THEN 2
  WHEN 4
   THEN 2
  WHEN 5
   THEN 3
  WHEN 6
   THEN 3
  ELSE 0
 END AS claimType,
 teamemps.employeeid AS teamemployeeid,
 claims_base.employeeid,
 claims_base.currencyid AS basecurrency,
 global_currencies.currencySymbol,
 costcodeTeamEmps.employeeid AS costcodeTeamEmployeeid,
 itemCheckerTeamEmps.employeeid AS itemCheckerTeamEmployeeId,
 savedexpenses.itemCheckerId
FROM
 claims_base
  INNER JOIN employees ON claims_base.employeeid = employees.employeeid
  INNER JOIN savedexpenses ON savedexpenses.claimid = claims_base.claimid
   INNER JOIN subcats ON subcats.subcatid = savedexpenses.subcatid
   INNER JOIN ReceiptCountForUnallocatedClaimsView ON savedexpenses.expenseid = ReceiptCountForUnallocatedClaimsView.expenseid
   LEFT JOIN savedexpenses_costcodes ON savedexpenses.expenseid = savedexpenses_costcodes.expenseid
    LEFT JOIN costcodes ON costcodes.costcodeid = savedexpenses_costcodes.costcodeid
     LEFT JOIN teams AS costcodeTeams ON costcodeTeams.teamid = costcodes.OwnerTeamId
      LEFT JOIN teamemps AS costcodeTeamEmps ON costcodeTeamEmps.teamid = costcodeTeams.teamid
                AND
                (
                 SELECT
                  cssot.signofftype
                 FROM
                  ClaimStageSignOffTypes cssot
                 WHERE
                  cssot.claimid = claims_base.claimid
                  AND cssot.stage = claims_base.stage
                ) = 8 -- CostCodeOwner
   LEFT JOIN teams AS itemCheckerTeams ON itemCheckerTeams.teamid = savedexpenses.itemCheckerTeamId 
    LEFT JOIN teamemps AS itemCheckerTeamEmps ON itemCheckerTeamEmps.teamid = itemCheckerTeams.teamid	
  LEFT JOIN teamemps ON teamemps.teamid = claims_base.teamid
  LEFT JOIN currencies ON currencies.currencyid = claims_base.currencyid
   LEFT JOIN global_currencies ON global_currencies.globalcurrencyid = currencies.globalcurrencyid
WHERE
 claims_base.paid = 0
 AND claims_base.submitted = 1
 AND claims_base.checkerid IS NULL
GROUP BY
 claims_base.claimid,
 claims_base.claimno,
 claims_base.stage,
 employees.surname + ', ' + employees.title + ' ' + employees.firstname,
 claims_base.name,
 claims_base.datesubmitted,
 claims_base.[status],
 teamemps.employeeid,
 claims_base.employeeid,
 claims_base.currencyid,
 claims_base.checkerid,
 claims_base.approved,
 costcodeTeamEmps.employeeid,
 itemCheckerTeamEmps.employeeid,
 savedexpenses.itemCheckerId,
 global_currencies.currencySymbol
