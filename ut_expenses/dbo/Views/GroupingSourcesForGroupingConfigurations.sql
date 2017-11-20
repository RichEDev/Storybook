CREATE VIEW dbo.GroupingSourcesForGroupingConfigurations
AS
SELECT        dbo.DocumentGroupingConfigurations.GroupingId, dbo.DocumentGroupingConfigurations.MergeProjectId, dbo.document_mergesources.groupingsource, 
                         dbo.reportsview.description
FROM            dbo.DocumentGroupingConfigurations INNER JOIN
                         dbo.document_mergeprojects ON dbo.DocumentGroupingConfigurations.MergeProjectId = dbo.document_mergeprojects.mergeprojectid INNER JOIN
                         dbo.document_mergesources ON dbo.document_mergeprojects.mergeprojectid = dbo.document_mergesources.mergeprojectid INNER JOIN
                         dbo.reportsview ON dbo.document_mergesources.reportid = dbo.reportsview.reportID
WHERE        (dbo.document_mergesources.groupingsource = 1) AND (dbo.DocumentGroupingConfigurations.MergeProjectId = 3) AND 
                         (dbo.DocumentGroupingConfigurations.GroupingId = 12)
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'GroupingSourcesForGroupingConfigurations';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'GroupingSourcesForGroupingConfigurations';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[48] 4[14] 2[20] 3) )"
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
         Begin Table = "document_mergesources"
            Begin Extent = 
               Top = 10
               Left = 749
               Bottom = 203
               Right = 922
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DocumentGroupingConfigurations"
            Begin Extent = 
               Top = 18
               Left = 122
               Bottom = 234
               Right = 387
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "document_mergeprojects"
            Begin Extent = 
               Top = 23
               Left = 465
               Bottom = 152
               Right = 655
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "reportsview"
            Begin Extent = 
               Top = 6
               Left = 960
               Bottom = 322
               Right = 1133
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
         Or = 1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'GroupingSourcesForGroupingConfigurations';

