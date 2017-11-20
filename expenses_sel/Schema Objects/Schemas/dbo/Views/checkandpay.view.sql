CREATE VIEW dbo.checkandpay
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
                            WHERE      (claimid = dbo.claims.claimid) AND (itemtype = 1)) AS cash,
                          (SELECT     SUM(total) AS Expr1
                            FROM          dbo.savedexpenses_current AS savedexpenses_current_3
                            WHERE      (claimid = dbo.claims.claimid) AND (itemtype = 2)) AS credit,
                          (SELECT     SUM(total) AS Expr1
                            FROM          dbo.savedexpenses_current AS savedexpenses_current_2
                            WHERE      (claimid = dbo.claims.claimid) AND (itemtype = 3)) AS purchase,
                          (SELECT     SUM(savedexpenses_current_1.total) AS Expr1
                            FROM          dbo.savedexpenses_current AS savedexpenses_current_1 INNER JOIN
                                                   dbo.subcats ON dbo.subcats.subcatid = savedexpenses_current_1.subcatid
                            WHERE      (dbo.subcats.reimbursable = 1) AND (savedexpenses_current_1.claimid = dbo.claims.claimid) AND 
                                                   (savedexpenses_current_1.tempallow = 1) AND (savedexpenses_current_1.itemtype = 1)) AS paiditems, dbo.claims.checkerid, 
                      dbo.claims.approved,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          dbo.signoffs
                            WHERE      (groupid = dbo.employees.groupid)) AS stagecount, dbo.claims.employeeid, dbo.claimType(dbo.claims.claimid) AS claimtype, 
                      dbo.claims.currencyid AS basecurrency, dbo.displayUnallocate(dbo.getGroupIdByClaim(dbo.claims.claimid, dbo.claims.employeeid), dbo.claims.stage) 
                      AS displayunallocate, dbo.displaySinglesignoff(dbo.getGroupIdByClaim(dbo.claims.claimid, dbo.claims.employeeid), dbo.claims.stage) 
                      AS displaysinglesignoff
FROM         dbo.claims INNER JOIN
                      dbo.employees ON dbo.claims.employeeid = dbo.employees.employeeid
WHERE     (dbo.claims.submitted = 1) AND (dbo.claims.paid = 0)

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[15] 4[15] 2[52] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "claims"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 121
               Right = 190
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "employees"
            Begin Extent = 
               Top = 6
               Left = 228
               Bottom = 121
               Right = 429
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'checkandpay';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'checkandpay';

