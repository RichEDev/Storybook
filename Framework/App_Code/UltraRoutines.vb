Imports Microsoft.VisualBasic
Imports System.Web.UI.WebControls

Namespace Framework2006

    Public Module UltraRoutines
        Public Sub SetHeaderStyle(ByVal e As Infragistics.WebUI.UltraWebGrid.LayoutEventArgs, ByVal BandNumber As Integer, ByVal colName As String, Optional ByVal ColWidthPixels As Integer = 0, Optional ByVal altHeading As String = "")
            e.Layout.HeaderStyleDefault.Wrap = False
            e.Layout.HeaderStyleDefault.CssClass = "ig_gridtitle"

            Select Case BandNumber
                Case 0
                    With e.Layout.Bands(0).Columns.FromKey(colName)
                        If altHeading <> "" Then
                            .HeaderText = altHeading
                        End If
                        .Tag = colName

                        If ColWidthPixels <> 0 Then
                            .Width = Unit.Pixel(ColWidthPixels)
                        End If
                    End With
                Case 1
                    With e.Layout.Bands(1).Columns.FromKey(colName)
                        If altHeading <> "" Then
                            .HeaderText = altHeading
                        End If
                        .Tag = colName
                        .Type = Infragistics.WebUI.UltraWebGrid.ColumnType.CheckBox

                        If ColWidthPixels <> 0 Then
                            .Width = Unit.Pixel(ColWidthPixels)
                        End If
                    End With
                Case Else
            End Select
        End Sub

        Public Sub SetRowStyle(ByRef e As Infragistics.WebUI.UltraWebGrid.LayoutEventArgs)
            'e.Layout.EnableInternalRowsManagement = False
            'e.Layout.RowSelectorStyleDefault.CssClass = "ig_rowselectors"
            e.Layout.RowSelectorsDefault = Infragistics.WebUI.UltraWebGrid.RowSelectors.No
            'e.Layout.RowStyleDefault.CssClass = "ig_row1"

            e.Layout.RowSizingDefault = Infragistics.WebUI.UltraWebGrid.AllowSizing.Free

            'e.Layout.RowAlternateStyleDefault.CssClass = "ig_row2"
            'e.Layout.RowAlternateStyleDefault.CssClass = "ig_row1"
            'e.Layout.EditCellStyleDefault.CssClass = "ig_row1"
            'e.Layout.RowExpAreaStyleDefault.CssClass = "ig_rowexparea"
            'e.Layout.RowHeightDefault = Unit.Pixel(15)

            e.Layout.CellClickActionDefault = Infragistics.WebUI.UltraWebGrid.CellClickAction.RowSelect
        End Sub

        Public Sub SetCellButtonStyle(ByVal e As Infragistics.WebUI.UltraWebGrid.LayoutEventArgs, ByVal band As Integer, ByVal fieldKey As String)
            e.Layout.Bands(band).Columns.FromKey(fieldKey).CellButtonStyle.CssClass = "ig_cellbutton"
            e.Layout.Bands(band).Columns.FromKey(fieldKey).CellButtonDisplay = Infragistics.WebUI.UltraWebGrid.CellButtonDisplay.Always
            e.Layout.Bands(band).Columns.FromKey(fieldKey).CellButtonStyle.HorizontalAlign = HorizontalAlign.Center
        End Sub

        Public Function CreateSummaryTotal(ByRef grid As Infragistics.WebUI.UltraWebGrid.UltraWebGrid, ByVal xMax As Integer, Optional ByVal isExport As Boolean = False) As String
            Dim colIdx, arrIdx As Integer
            Dim bestIdx As Integer
            Dim priority() As String = {"Contract Value", "Total Maintenance Value", "Maintenance Value", "Maintenance Next Year", "Maintenance Next Year + 1", "Maintenance Next Year + 2", "Maintenance Next Year + 3", "Invoice Total", "Product Value", "Projected Saving"}

            bestIdx = 999

            For colIdx = 0 To grid.Columns.Count - 1
                For arrIdx = UBound(priority) To 0 Step -1
                    '    For arrIdx = 3 To 0 Step -1
                    If grid.Columns(colIdx).BaseColumnName = priority(arrIdx) Then
                        ' ensure that the column is currently being subtotalled
                        If grid.Columns(colIdx).FooterTotal = Infragistics.WebUI.UltraWebGrid.SummaryInfo.Sum Then
                            If arrIdx < bestIdx Then
                                bestIdx = arrIdx
                            End If
                        End If

                    End If
                Next
            Next

            If bestIdx <> 999 Then
                ' must be one of the currency fields included, so use in groupby description header
                If isExport = True Then
                    grid.DisplayLayout.GroupByRowDescriptionMaskDefault = "[value] : Group Total [sum:" & grid.Columns.FromKey(priority(bestIdx)).BaseColumnName.ToString & "] Items: [count]"
                    'CreateSummaryTotal = "[value] : Gouping Total [sum:" & grid.Columns.FromKey(priority(bestIdx)).BaseColumnName.ToString & "] Items: [count]"
                Else
                    grid.DisplayLayout.GroupByRowDescriptionMaskDefault = "<table style=""display: inline;""><tr><td width=""" & CStr((xMax / 100) * 20) & """>[value]</td><td width=""" & CStr((xMax / 100) * 20) & """>Group Total :<b> [sum:" & grid.Columns.FromKey(priority(bestIdx)).BaseColumnName.ToString & "]</b></td><td width=""" & CStr((xMax / 100) * 15) & """>Items: <b>[count]</b></td></tr></table>"
                    'grid.DisplayLayout.GroupByRowDescriptionMaskDefault = "<span width=""" & CStr((xMax / 100) * 15) & """>[value]</span><span width=""" & CStr((xMax / 100) * 10) & """>Group Total :<b> [sum:" & grid.Columns.FromKey(priority(bestIdx)).BaseColumnName.ToString & "]</b></span><span width=""" & CStr((xMax / 100) * 15) & """>Items: <b>[count]</b></span>"
                End If
                CreateSummaryTotal = grid.Columns.FromKey(priority(bestIdx)).ToString
            Else
                CreateSummaryTotal = ""
            End If
        End Function
    End Module

End Namespace
