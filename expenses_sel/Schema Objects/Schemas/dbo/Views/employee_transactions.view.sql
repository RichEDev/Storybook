﻿CREATE VIEW [dbo].[employee_transactions]
AS
SELECT     dbo.card_transactions.transactionid, dbo.card_transactions.statementid, dbo.card_transactions.corporatecardid, 
                      dbo.card_transactions.transaction_date, dbo.card_transactions.description, dbo.card_transactions.card_number, employees.surname + ', ' + employees.title + ' ' + employees.firstname AS employee, dbo.card_transactions.transaction_amount, 
                      dbo.card_transactions.original_amount, dbo.global_currencies.label, exchangerate,dbo.global_countries.country,
                          (SELECT     SUM(total) AS Expr1
                            FROM          dbo.savedexpenses
                            WHERE      (transactionid = dbo.card_transactions.transactionid)) AS allocated_amount, dbo.card_transactions.transaction_amount -
                          (SELECT     SUM(total) AS Expr1
                            FROM          dbo.savedexpenses AS savedexpenses_3
                            WHERE      (transactionid = dbo.card_transactions.transactionid)) AS unallocated_amount,
                          (SELECT     SUM(convertedtotal) AS Expr1
                            FROM          dbo.savedexpenses AS savedexpenses_2
                            WHERE      (transactionid = dbo.card_transactions.transactionid)) AS allocated_original_amount, dbo.card_transactions.original_amount -
                          (SELECT     SUM(convertedtotal) AS Expr1
                            FROM          dbo.savedexpenses AS savedexpenses_1
                            WHERE      (transactionid = dbo.card_transactions.transactionid)) AS unallocated_original_amount
FROM         dbo.card_transactions 
	left JOIN
                      dbo.global_currencies ON dbo.card_transactions.globalcurrencyid = dbo.global_currencies.globalcurrencyid
                      LEFT JOIN employee_corporate_cards ON [employee_corporate_cards].corporatecardid = card_transactions.corporatecardid
                      left JOIN employees ON employees.employeeid = employee_corporate_cards.employeeid
                      LEFT JOIN GLOBAL_countries ON [GLOBAL_countries].globalcountryid = card_transactions.globalcountryid
                      


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
         Begin Table = "card_transactions"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 121
               Right = 215
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "global_currencies"
            Begin Extent = 
               Top = 6
               Left = 253
               Bottom = 121
               Right = 412
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
End', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'employee_transactions';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'employee_transactions';

