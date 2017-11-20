Namespace Framework2006

Public Module StandardReportRoutines
    'Private Sub MonthlyBudgetReport(ByVal db As DBConnection, ByVal uInfo As UserInfo)
    '    Try
    '        Dim glSQL As String
    '        Dim WhereVendor, WhereContractType As String
    '        Dim curYear, curMonth As Integer
    '        Dim tmpDate, tmpDate2 As String
    '        Dim CustNum As Integer

    '        db.FWDb("R2", "System Parameters")
    '        If db.FWDbFlag = True Then
    '            CustNum = Val(db.FWDbFindVal("Contract No"))
    '        Else
    '            CustNum = 0
    '        End If

    '        WhereVendor = DoWhereVendor
    '        WhereContractType = DoWhereContractType

    '        glSQL = "SELECT " & vbNewLine
    '        glSQL = glSQL & "[Vendor Details].[Vendor Name], " & vbNewLine & _
    '            "[Codes - Contract Type].[Contract Type Description] AS [Contract Type], " & vbNewLine & _
    '            "ISNULL([Product Details].[Product Name],'** product undefined **') AS [Product Name], " & vbNewLine & _
    '            "[Contract Details].[Start Date], " & vbNewLine & _
    '            "[Contract Details].[End Date], " & vbNewLine & _
    '            "[Contract - Forecast Details].[Payment Date], " & vbNewLine & _
    '            "[Contract Details].[Contract Id], " & vbNewLine & _
    '            "([Contract - Forecast Details].[Forecast Amount] / [Codes - Currency].[Conversion Rate]) AS [Forecast Amount] " & vbNewLine

    '        glSQL = glSQL & "FROM (((((([Contract Details] " & vbNewLine & _
    '            "LEFT OUTER JOIN [Vendor Details] " & vbNewLine & _
    '            "ON [Contract Details].[Vendor Id] = [Vendor Details].[Vendor Id]) " & vbNewLine & _
    '            "LEFT OUTER JOIN [Codes - Contract Type] " & vbNewLine & _
    '            "ON [Contract Details].[Contract Type Id] = [Codes - Contract Type].[Contract Type Id]) " & vbNewLine & _
    '            "LEFT OUTER JOIN [Contract - Forecast Details] " & vbNewLine & _
    '            "ON [Contract Details].[Contract Id] = [Contract - Forecast Details].[Contract Id]) " & vbNewLine & _
    '            "LEFT OUTER JOIN [Codes - Currency] " & vbNewLine & _
    '            "ON [Contract Details].[Contract Currency] = [Codes - Currency].[Currency Id]) " & vbNewLine & _
    '            "LEFT OUTER JOIN [Contract - Forecast Products] " & vbNewLine & _
    '            "ON [Contract - Forecast Details].[Contract-Forecast Id] = [Contract - Forecast Products].[Forecast Id]) " & vbNewLine & _
    '            "LEFT OUTER JOIN [Product Details] " & vbNewLine & _
    '            "ON [Product Details].[Product Id] = [Contract - Forecast Products].[Product Id]) " & vbNewLine

    '        glSQL = glSQL & " WHERE ([Contract Details].[Contract Id] <> 0)" & vbNewLine

    '        If cboContractStatus.ListIndex <> 2 Then
    '            glSQL = glSQL & " AND ([Contract Details].[Archived] = '" & IIf(cboContractStatus.list(cboContractStatus.ListIndex) = "Live", "N", "Y") & "')" & vbNewLine
    '        End If

    '        glSQL = glSQL & " AND ([Contract Details].[Location Id] = " & Trim(Str(uInfo.ActiveLocation)) & ")" & vbNewLine

    '        tmpDate = hiddenDay.Caption & "/" & Trim(Str(cboReportMonth.ListIndex + 1)) & "/" & txtYear.text

    '        ' VM = customer 44
    '        '    If CustNum = 44 Then
    '        '        ' Virgin mobile run in non-standard binary collation, so are case sensitive and reverses the year!
    '        '        tmpDate = Format(tmpDate, "yyyy-MM-dd")
    '        '        tmpDate2 = Format(DateAdd("d", -1, DateAdd("yyyy", 1, CDate(tmpDate))), "yyyy-MM-dd")
    '        '    Else
    '        '        tmpDate = Format(tmpDate, "dd/MM/yyyy")
    '        '        tmpDate2 = Format(DateAdd("d", -1, DateAdd("yyyy", 1, CDate(tmpDate))), "dd/MM/yyyy")
    '        '    End If

    '        Dim CD As Date
    '        Dim safeDate As String

    '        safeDate = tmpDate
    '        CD = CDate(tmpDate)
    '        tmpDate = "CONVERT(datetime,'" & Format(CD, "yyyy-mm-dd hh:mm:ss") & "',120)"

    '        CD = DateAdd("d", -1, DateAdd("yyyy", 1, CDate(safeDate)))
    '        tmpDate2 = "CONVERT(datetime,'" & Format(CD, "yyyy-mm-dd hh:mm:ss") & "',120)"

    '        ' sql server
    '        glSQL = glSQL & " AND ([Contract - Forecast Details].[Payment Date] >= " & tmpDate & " AND [Contract - Forecast Details].[Payment Date] <= " & tmpDate2 & ")" & vbNewLine

    '        If WhereVendor <> "" Then
    '            glSQL = glSQL & WhereVendor
    '        End If

    '        If WhereContractType <> "" Then
    '            glSQL = glSQL & WhereContractType
    '        End If

    '        ' apply filter for Virgin Mobile UF1 [GL Code] if filter chosen
    '        If CustNum = 44 Then
    '            ' must be VM
    '            If cboUF1.ListIndex <> -1 Then
    '                glSQL = glSQL & " AND [Contract Details].[UF1] = '" & Trim(cboUF1.list(cboUF1.ListIndex)) & "' "
    '            End If
    '        End If

    '        glSQL = glSQL & "ORDER BY [Vendor Name] ASC,[Contract Details].[Contract Id],[Contract Type Description],[Product Name],[Payment Date] " & vbNewLine

    '        db.RunSQL(glSQL, db.glDBWorkA)

    '        rep.ApplicationName = zz1GL(1)
    '        rep.title = lblTitle.Caption

    '        If cboUF1.Visible = True Then
    '            If cboUF1.ListIndex <> -1 Then
    '                rep.title = lblTitle.Caption & " [filter: " & cboUF1.list(cboUF1.ListIndex) & "]"
    '            End If
    '        End If

    '        rep.ColHeadsReqd = 8
    '        rep.ColHeads(1) = "a02"
    '        rep.ColHeads(2) = "a15Vendor Name"      'Vendor name
    '        rep.ColHeads(3) = "a15Contract Type"    'Contract Type
    '        rep.ColHeads(4) = "a08Product"
    '        rep.ColHeads(5) = "d07Start Date"
    '        rep.ColHeads(6) = "d07End Date"
    '        rep.ColHeads(7) = "h00Payment Date"
    '        rep.ColHeads(8) = "h00Contract Id"
    '        rep.ColHeads(9) = "a02"
    '        rep.ColsReqd = 7  ' The number of columns required
    '        rep.SubTotCols = 2
    '        rep.GrandTotal = True
    '        rep.Orientation = 2

    '        zz1Report6(db.glDBWorkA, sprReport, rep)
    '        'zz1Report5 sprReport, 8, "2,4", True
    '        zz1Report7(sprReport, rep) 'Add the sub-totals and grand total

    '    Catch ex As Exception

    '    End Try
    'End Sub

    'Private Sub MaintenanceAnalysis(ByVal db As DBConnection, ByVal uInfo As UserInfo)
    '    Try
    '        Dim glSQL As String
    '        Dim WhereVendor, WhereContractType As String

    '        WhereVendor = DoWhereVendor
    '        WhereContractType = DoWhereContractType

    '        glSQL = "SELECT DISTINCT " & vbNewLine & _
    '            " [Vendor Details].[Vendor Name]," & vbNewLine & _
    '            " [Codes - Contract Type].[Contract Type Description] AS [Contract Type]," & vbNewLine & _
    '            " ISNULL([Product Details].[Product Name],'** product undefined **') AS [Product Name]," & vbNewLine & _
    '            " [Contract Details].[Start Date]," & vbNewLine & _
    '            " [Contract Details].[End Date]," & vbNewLine & _
    '            " ([Invoice Details].[Total Invoice Amount] / [Codes - Currency].[Conversion Rate]) AS [Current Month Actual]," & vbNewLine & _
    '            " ([Contract - Forecast Details].[Forecast Amount] / [Codes - Currency].[Conversion Rate]) AS [Current Month Budget]," & vbNewLine & _
    '            " [Contract Details].[Contract Id]" & vbNewLine & _
    '            " FROM ((((((([Contract Details]" & vbNewLine

    '        glSQL = glSQL & " LEFT OUTER JOIN [Vendor Details]" & vbNewLine & _
    '            " ON [Contract Details].[Vendor Id] = [Vendor Details].[Vendor Id])" & vbNewLine & _
    '            " LEFT OUTER JOIN [Codes - Contract Type]" & vbNewLine & _
    '            " ON [Contract Details].[Contract Type Id] = [Codes - Contract Type].[Contract Type Id])" & vbNewLine & _
    '            " LEFT OUTER JOIN [Contract - Forecast Details]" & vbNewLine & _
    '            " ON [Contract Details].[Contract Id] = [Contract - Forecast Details].[Contract Id])" & vbNewLine & _
    '            " LEFT OUTER JOIN [Codes - Currency]" & vbNewLine & _
    '            " ON [Contract Details].[Contract Currency] = [Codes - Currency].[Currency Id])" & vbNewLine & _
    '            " LEFT OUTER JOIN [Contract - Forecast Products]" & vbNewLine & _
    '            " ON [Contract - Forecast Details].[Contract-Forecast Id] = [Contract - Forecast Products].[Forecast Id])" & vbNewLine & _
    '            " LEFT OUTER JOIN [Product Details]" & vbNewLine & _
    '            " ON [Product Details].[Product Id] = [Contract - Forecast Products].[Product Id])" & vbNewLine & _
    '            " LEFT OUTER JOIN [Invoice Details]" & vbNewLine & _
    '            " ON [Invoice Details].[Contract Id] = [Contract Details].[Contract Id])" & vbNewLine & _
    '            " WHERE ([Contract Details].[Contract Id] <> 0)" & vbNewLine

    '        If cboContractStatus.ListIndex <> 2 Then
    '            glSQL = glSQL & " AND ([Contract Details].[Archived] = '" & IIf(cboContractStatus.list(cboContractStatus.ListIndex) = "Live", "N", "Y") & "')" & vbNewLine
    '        End If

    '        glSQL = glSQL & " AND ([Contract Details].[Location Id] = " & Trim(Str(uInfo.ActiveLocation)) & ")" & vbNewLine

    '        glSQL = glSQL & " AND ((DATEPART(mm,[Contract - Forecast Details].[Payment Date]) = " & cboReportMonth.ListIndex + 1 & " AND DATEPART(yyyy,[Contract - Forecast Details].[Payment Date]) = " & txtYear.text & ")" & vbNewLine & _
    '        " OR (DATEPART(mm,[Invoice Details].[Invoice Due Date]) = " & cboReportMonth.ListIndex + 1 & " AND DATEPART(yyyy,[Invoice Details].[Invoice Due Date]) = " & txtYear.text & "))" & vbNewLine

    '        If WhereVendor <> "" Then
    '            glSQL = glSQL & WhereVendor
    '        End If

    '        If WhereContractType <> "" Then
    '            glSQL = glSQL & WhereContractType
    '        End If

    '        glSQL = glSQL & "ORDER BY [Vendor Details].[Vendor Name] " & vbNewLine

    '        db.RunSQL(glSQL, db.glDBWorkA)

    '        ' compile report
    '        rep.ApplicationName = zz1GL(1)
    '        rep.title = lblTitle.Caption & " (" & cboReportMonth & ")"
    '        rep.ColHeadsReqd = 10
    '        rep.ColHeads(1) = "a02"
    '        rep.ColHeads(2) = "a24Vendor Name"      'Vendor name
    '        rep.ColHeads(3) = "a24Contract Type"    'Contract Type
    '        rep.ColHeads(4) = "a15Product"
    '        rep.ColHeads(5) = "d11Start Date"
    '        rep.ColHeads(6) = "d11End Date"
    '        rep.ColHeads(7) = "n10Cur. Actual"
    '        rep.ColHeads(8) = "n10Cur. Budget"
    '        rep.ColHeads(9) = "h00ContractId"       ' hidden contract id returned as part of query but not required.
    '        rep.ColHeads(10) = "c10Variance#08-07#"
    '        rep.ColHeads(11) = "a02"
    '        rep.ColsReqd = 9   ' The number of columns required
    '        rep.SubTotCols = 1
    '        rep.GrandTotal = True
    '        rep.Orientation = 2
    '        zz1Report4(db.glDBWorkA, sprReport, rep)   'Load the data into the spreadsheet
    '        zz1Report5(sprReport, 9, "2,4")
    '        zz1Report2(sprReport, rep) 'Add the sub-totals and grand total

    '    Catch ex As Exception

    '    End Try
    'End Sub
End Module

End Namespace
