CREATE VIEW checkandpay
AS
SELECT     dbo.claims_base.claimid, dbo.claims_base.claimno, dbo.claims_base.stage, dbo.employees.surname + ', ' + dbo.employees.title + ' ' + dbo.employees.firstname AS empname, 
                      dbo.claims_base.name, dbo.claims_base.description, dbo.claims_base.datesubmitted, dbo.claims_base.datepaid, dbo.claims_base.status, SUM(CASE WHEN normalreceipt = 1 AND primaryitem = 1 THEN 1 ELSE 0 END) AS numreceipts,
                      SUM(CASE WHEN receiptattached = 1 THEN 1 ELSE 0 END) as numreceiptsattached,
                          (SELECT     COUNT(DISTINCT dbo.savedexpensesFlags.expenseid) AS flagcount
                            FROM          dbo.savedexpensesFlags INNER JOIN
                                                   dbo.savedexpenses AS a ON a.expenseid = dbo.savedexpensesFlags.expenseid
                            WHERE      (a.claimid = dbo.claims_base.claimid)) AS flaggeditems, SUM(dbo.savedexpenses.total) AS total, SUM(CASE WHEN itemtype = 1 THEN savedexpenses.total ELSE 0 END) AS cash, 
                      SUM(CASE WHEN itemtype = 2 THEN savedexpenses.total ELSE 0 END) AS credit, SUM(CASE WHEN itemtype = 3 THEN savedexpenses.total ELSE 0 END) AS purchase, 
                      SUM(CASE WHEN subcats.reimbursable = 1 AND tempallow = 1 AND itemtype = 1 THEN savedexpenses.total ELSE 0 END) AS paiditems, dbo.claims_base.checkerid, dbo.claims_base.approved,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          dbo.signoffs AS a
                            WHERE      (groupid = dbo.employees.groupid)) AS stagecount, dbo.claims_base.employeeid, CASE (MAX(CASE itemtype WHEN 1 THEN 1 ELSE 0 END) 
                      + MAX(CASE itemtype WHEN 2 THEN 3 ELSE 0 END) + MAX(CASE itemtype WHEN 3 THEN 5 ELSE 0 END)) 
                      WHEN 1 THEN 1 WHEN 3 THEN 2 WHEN 4 THEN 2 WHEN 5 THEN 3 WHEN 6 THEN 3 ELSE 0 END AS claimType, dbo.claims_base.currencyid AS basecurrency, 
                      dbo.global_currencies.currencySymbol, CASE (CASE (MAX(CASE itemtype WHEN 1 THEN 1 ELSE 0 END) + MAX(CASE itemtype WHEN 2 THEN 3 ELSE 0 END) 
                      + MAX(CASE itemtype WHEN 3 THEN 5 ELSE 0 END)) WHEN 1 THEN
                          (MAX(signoffs.signofftype)) WHEN 2 THEN CASE WHEN employees.groupidcc IS NULL THEN
                          (MAX(signoffs.signofftype)) ELSE
                          (MAX(signoffscc.signofftype)) END WHEN 3 THEN CASE WHEN employees.groupidcc IS NULL THEN
                          (MAX(signoffs.signofftype)) ELSE
                          (MAX(signoffscc.signofftype)) END WHEN 4 THEN CASE WHEN employees.groupidcc IS NULL THEN
                          (MAX(signoffs.signofftype)) ELSE
                          (MAX(signoffscc.signofftype)) END WHEN 5 THEN CASE WHEN employees.groupidpc IS NULL THEN
                          (MAX(signoffs.signofftype)) ELSE
                          (MAX(signoffspc.signofftype)) END WHEN 6 THEN CASE WHEN employees.groupidpc IS NULL THEN
                          (MAX(signoffs.signofftype)) ELSE
                          (MAX(signoffspc.signofftype)) END WHEN 8 THEN
                          (MAX(signoffs.signofftype)) WHEN 9 THEN
                          (MAX(signoffs.signofftype)) END) WHEN 3 THEN CAST(1 AS BIT) WHEN 6 THEN CASE WHEN
                          (SELECT     TOP 1 [approverTeamId]
                            FROM          [approvalMatrixLevels]
                            WHERE      [approvalMatrixId] = (CASE (MAX(CASE itemtype WHEN 1 THEN 1 ELSE 0 END) + MAX(CASE itemtype WHEN 2 THEN 3 ELSE 0 END) + MAX(CASE itemtype WHEN 3 THEN 5 ELSE 0 END))
                                                    WHEN 1 THEN
                                                       (MAX(signoffs.relid)) WHEN 2 THEN CASE WHEN employees.groupidcc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffscc.relid)) END WHEN 3 THEN CASE WHEN employees.groupidcc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffscc.relid)) END WHEN 4 THEN CASE WHEN employees.groupidcc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffscc.relid)) END WHEN 5 THEN CASE WHEN employees.groupidpc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffspc.relid)) END WHEN 6 THEN CASE WHEN employees.groupidpc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffspc.relid)) END WHEN 8 THEN
                                                       (MAX(signoffs.relid)) WHEN 9 THEN
                                                       (MAX(signoffs.relid)) END) AND [approvalLimit] >= SUM(savedexpenses.total)
                            ORDER BY [approvalLimit] ASC) IS NULL THEN (CASE WHEN
                          (SELECT     TOP 1 [defaultApproverTeamId]
                            FROM          [approvalMatrices]
                            WHERE      [approvalMatrixId] = (CASE (MAX(CASE itemtype WHEN 1 THEN 1 ELSE 0 END) + MAX(CASE itemtype WHEN 2 THEN 3 ELSE 0 END) + MAX(CASE itemtype WHEN 3 THEN 5 ELSE 0 END))
                                                    WHEN 1 THEN
                                                       (MAX(signoffs.relid)) WHEN 2 THEN CASE WHEN employees.groupidcc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffscc.relid)) END WHEN 3 THEN CASE WHEN employees.groupidcc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffscc.relid)) END WHEN 4 THEN CASE WHEN employees.groupidcc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffscc.relid)) END WHEN 5 THEN CASE WHEN employees.groupidpc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffspc.relid)) END WHEN 6 THEN CASE WHEN employees.groupidpc IS NULL THEN
                                                       (MAX(signoffs.relid)) ELSE
                                                       (MAX(signoffspc.relid)) END WHEN 8 THEN
                                                       (MAX(signoffs.relid)) WHEN 9 THEN
                                                       (MAX(signoffs.relid)) END)) IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) ELSE CAST(1 AS BIT) 
                      END WHEN 8 THEN CASE
                          (SELECT     COUNT(savedexpenses_costcodes.savedcostcodeid)
                            FROM          savedexpenses_costcodes INNER JOIN
                                                   savedexpenses AS a ON savedexpenses_costcodes.expenseid = a.expenseid LEFT JOIN
                                                   costcodes ON savedexpenses_costcodes.costcodeid = costcodes.costcodeid
                            WHERE      a.claimid = claims_base.claimid AND ((savedexpenses_costcodes.costcodeid IS NULL AND
                                                   ((SELECT TOP 1 CASE WHEN charindex(',', stringvalue) IS NULL THEN 0 ELSE CAST(SUBSTRING(stringvalue, 1, charindex(',', stringvalue) - 1) AS INT) END
													FROM accountProperties
													WHERE stringKey = 'defaultCostCodeOwner')) = 49) OR
                                                   (savedexpenses_costcodes.costcodeid IS NOT NULL AND (costcodes.OwnerTeamId IS NOT NULL OR
                                                   (costcodes.OwnerTeamId IS NULL AND costcodes.OwnerEmployeeId IS NULL AND costcodes.OwnerBudgetHolderId IS NULL AND
                                                    ((SELECT TOP 1 CASE WHEN charindex(',', stringvalue) IS NULL THEN 0 ELSE CAST(SUBSTRING(stringvalue, 1, charindex(',', stringvalue) - 1) AS INT) END
													FROM accountProperties
													WHERE stringKey = 'defaultCostCodeOwner')) = 49))))) WHEN 0 THEN CAST(0 AS BIT) ELSE CASE WHEN SUM(CASE WHEN savedexpenses.tempallow = 1 AND primaryitem = 1 AND
                       claims_base.splitapprovalstage = 1 THEN 1 ELSE 0 END) > 0 THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END END ELSE CAST(0 AS BIT) END AS displayunallocate, 
                      CASE (MAX(CASE itemtype WHEN 1 THEN 1 ELSE 0 END) + MAX(CASE itemtype WHEN 2 THEN 3 ELSE 0 END) + MAX(CASE itemtype WHEN 3 THEN 5 ELSE 0 END)) WHEN 1 THEN
                          CAST((MAX( CAST (signoffs.singlesignoff as int))) AS bit) WHEN 2 THEN CASE WHEN employees.groupidcc IS NULL THEN
                          CAST((MAX(CAST (signoffs.singlesignoff as int))) AS bit) ELSE
                          CAST((MAX(CAST (signoffscc.singlesignoff as int))) AS bit) END WHEN 3 THEN CASE WHEN employees.groupidcc IS NULL OR
                      (partSubmittal.stringValue = 0 OR
                      onlyCashCredit.stringValue = 0) THEN
                          CAST((MAX(CAST (signoffs.singlesignoff as int))) AS bit) ELSE
                          CAST((MAX(CAST (signoffscc.singlesignoff as int))) AS bit) END WHEN 4 THEN
                          CAST((MAX(CAST (signoffs.singlesignoff as int))) AS bit) WHEN 5 THEN CASE WHEN employees.groupidpc IS NULL OR
                      (partSubmittal.stringValue = 0 OR
                      onlyCashCredit.stringValue = 0) THEN
                          CAST((MAX(CAST (signoffs.singlesignoff as int))) AS bit) ELSE
                          CAST((MAX(CAST (signoffspc.singlesignoff as int))) AS bit) END WHEN 6 THEN
                          CAST((MAX(CAST (signoffs.singlesignoff as int))) AS bit) WHEN 8 THEN
                          CAST((MAX(CAST (signoffs.singlesignoff as int))) AS bit) WHEN 9 THEN
                          CAST((MAX(CAST (signoffs.singlesignoff as int))) AS bit) END AS displaysinglesignoff, dbo.claims_base.splitApprovalStage,
                          (SELECT     COUNT(expenseid) AS Expr1
                            FROM          dbo.savedexpenses
                            WHERE      (primaryitem = 1) AND (claimid = dbo.claims_base.claimid)) AS ClaimItemsCount,
                          (SELECT     COUNT(expenseid) AS Expr1
                            FROM          dbo.savedexpenses
                            WHERE      (primaryitem = 1) AND (tempallow = 1) AND (claimid = dbo.claims_base.claimid)) AS ItemsApproved, CASE (MAX(CASE itemtype WHEN 1 THEN 1 ELSE 0 END) 
                      + MAX(CASE itemtype WHEN 2 THEN 3 ELSE 0 END) + MAX(CASE itemtype WHEN 3 THEN 5 ELSE 0 END)) WHEN 1 THEN (employees.groupid) 
                      WHEN 2 THEN CASE WHEN employees.groupidcc IS NULL THEN (employees.groupid) ELSE (employees.groupidcc) END WHEN 3 THEN CASE WHEN employees.groupidcc IS NULL OR
                      (partSubmittal.stringValue = 0 OR
                      onlyCashCredit.stringValue = 0) THEN (employees.groupid) ELSE (employees.groupidcc) END WHEN 4 THEN (employees.groupid) WHEN 5 THEN CASE WHEN employees.groupidpc IS NULL OR
                      (partSubmittal.stringValue = 0 OR
                      onlyCashCredit.stringValue = 0) THEN (employees.groupid) ELSE (employees.groupidpc) END WHEN 6 THEN (employees.groupid) WHEN 8 THEN (employees.groupid) 
                      WHEN 9 THEN (employees.groupid) END AS SignoffGroup, dbo.claims_base.stage AS currentStage, dbo.savedexpenses.itemCheckerId, SUM(CASE WHEN savedexpenses.tempallow = 0 AND 
                      primaryitem = 1 AND claims_base.splitapprovalstage = 1 THEN 1 ELSE 0 END) AS CheckerItemsUnapproved, SUM(CASE WHEN savedexpenses.tempallow = 1 AND primaryitem = 1 AND 
                      claims_base.splitapprovalstage = 1 THEN 1 ELSE 0 END) AS CheckerItemsApproved,
                      dbo.claims_base.ReferenceNumber AS ReferenceNumber
FROM         dbo.claims_base INNER JOIN
                      dbo.savedexpenses ON dbo.savedexpenses.claimid = dbo.claims_base.claimid LEFT OUTER JOIN
                      dbo.currencies ON dbo.currencies.currencyid = dbo.claims_base.currencyid LEFT OUTER JOIN
                      dbo.global_currencies ON dbo.global_currencies.globalcurrencyid = dbo.currencies.globalcurrencyid INNER JOIN
                      dbo.subcats ON dbo.subcats.subcatid = dbo.savedexpenses.subcatid INNER JOIN
                      dbo.employees ON dbo.claims_base.employeeid = dbo.employees.employeeid
					  LEFT JOIN dbo.signoffs ON dbo.signoffs.groupid = dbo.employees.groupid AND signoffs.stage = claims_base.stage
					  LEFT JOIN dbo.signoffs AS signoffscc ON signoffscc.groupid = dbo.employees.groupidcc AND signoffscc.stage = claims_base.stage
					  LEFT JOIN dbo.signoffs AS signoffspc ON signoffspc.groupid = dbo.employees.groupidpc AND signoffspc.stage = claims_base.stage
					   LEFT OUTER JOIN
                      dbo.accountProperties AS partSubmittal ON partSubmittal.stringKey = 'PartSubmittal' LEFT OUTER JOIN
                      dbo.accountProperties AS onlyCashCredit ON onlyCashCredit.stringKey = 'OnlyCashCredit'
WHERE     (dbo.claims_base.submitted = 1) AND (dbo.claims_base.paid = 0)
GROUP BY dbo.claims_base.claimid, dbo.claims_base.claimno, dbo.claims_base.stage, dbo.employees.surname + ', ' + dbo.employees.title + ' ' + dbo.employees.firstname, dbo.claims_base.name, 
                      dbo.claims_base.description, dbo.claims_base.datesubmitted, dbo.claims_base.datepaid, dbo.claims_base.status, dbo.claims_base.checkerid, dbo.claims_base.approved, dbo.claims_base.employeeid, dbo.claims_base.currencyid, 
                      dbo.employees.groupid, dbo.employees.groupidcc, dbo.employees.groupidpc, dbo.claims_base.splitApprovalStage, dbo.claims_base.stage, dbo.savedexpenses.itemCheckerId, 
                      dbo.global_currencies.currencySymbol, onlyCashCredit.stringValue, partSubmittal.stringValue, dbo.claims_base.ReferenceNumber