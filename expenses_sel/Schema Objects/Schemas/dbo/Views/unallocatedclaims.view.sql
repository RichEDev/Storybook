CREATE VIEW [dbo].[unallocatedclaims]
AS
SELECT     dbo.claims.claimid, dbo.claims.claimno, dbo.claims.stage, 
                      dbo.employees.surname + ', ' + dbo.employees.title + ' ' + dbo.employees.firstname AS empname, dbo.claims.name, dbo.claims.status,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          dbo.savedexpenses_current
                            WHERE      (claimid = dbo.claims.claimid) AND (normalreceipt = 1)) AS numreceipts,
                          (SELECT     COUNT(DISTINCT expenseid) AS Expr1
                            FROM          dbo.savedexpenses_flags
                            WHERE      (expenseid IN
                                                       (SELECT     expenseid
                                                         FROM          dbo.savedexpenses_current AS savedexpenses_current_6
                                                         WHERE      (claimid = dbo.claims.claimid)))) AS flaggeditems,
                          (SELECT     SUM(total) AS Expr1
                            FROM          dbo.savedexpenses_current AS savedexpenses_current_5
                            WHERE      (claimid = dbo.claims.claimid)) AS total,
                          (SELECT     SUM(total) AS Expr1
                            FROM          dbo.savedexpenses_current AS savedexpenses_current_4
                            WHERE      (claimid = dbo.claims.claimid) AND itemtype=1) AS cash,
                          (SELECT     SUM(total) AS Expr1
                            FROM          dbo.savedexpenses_current AS savedexpenses_current_3
                            WHERE      (claimid = dbo.claims.claimid) AND itemtype = 2) AS credit,
                          (SELECT     SUM(total) AS Expr1
                            FROM          dbo.savedexpenses_current AS savedexpenses_current_2
                            WHERE      (claimid = dbo.claims.claimid) AND itemtype = 3) AS purchase,
                          (SELECT     SUM(savedexpenses_current_1.amountpayable) AS Expr1
                            FROM          dbo.savedexpenses_current AS savedexpenses_current_1 INNER JOIN
                                                   dbo.subcats ON dbo.subcats.subcatid = savedexpenses_current_1.subcatid
                            WHERE      (dbo.subcats.reimbursable = 1) AND (savedexpenses_current_1.claimid = dbo.claims.claimid) AND 
                                                   (savedexpenses_current_1.tempallow = 1)) AS paiditems, dbo.claims.checkerid, dbo.claims.approved,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          dbo.signoffs
                            WHERE      (groupid = dbo.employees.groupid)) AS stagecount, dbo.claimType(dbo.claims.claimid) AS claimtype, 
                      dbo.teamemps.employeeid AS teamemployeeid, dbo.claims.employeeid, dbo.claims.currencyid AS basecurrency
FROM         dbo.claims INNER JOIN
                      dbo.employees ON dbo.claims.employeeid = dbo.employees.employeeid INNER JOIN
                      dbo.teamemps ON dbo.teamemps.teamid = dbo.claims.teamid
WHERE     (dbo.claims.paid = 0) AND (dbo.claims.checkerid IS NULL)

