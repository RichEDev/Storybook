Module MaintRoutines
    Public Enum MaintType As Integer
        SingleInflator = 0
        GreaterXY = 1
        LesserXY = 2
    End Enum

    Public Enum ForecastType
        Prod_v_Inflator = 0
        InflatorOnly = 1
        Staged = 2
    End Enum

    Public Structure MaintParams
        Public CurMaintVal As Double
        Public ListPrice As Double
        Public PctOfLP As Double
        Public MaintCalcType As Integer
        Public MaintTypeX As Integer
        Public MaintTypeValueX As Double
        Public MaintExtraPercentX As Double
        Public MaintTypeY As Integer
        Public MaintTypeValueY As Double
        Public MaintExtraPercentY As Double
        Public ForecastType As ForecastType
    End Structure

    Public Structure NYMResult
        Public NYMValue As Double
        Public NYMCalculation As String
    End Structure


    Public Function CalcNYM(ByVal mp As MaintParams, ByVal maintYear As Integer) As NYMResult
        ' This routine assumes that the variables have all been set...
        Dim resultLP, resultX, resultY, tmpResult As Double
        Dim retVal As NYMResult
        Dim tmpStr As String
        Dim calcLine(12) As String
        Dim headerLine(2) As String

        headerLine(1) = "MAINTENANCE CALCULATION :- "
        If mp.ForecastType = ForecastType.Prod_v_Inflator Then
            headerLine(2) = "<b>THE LESSER OF </b>"
        Else
            headerLine(2) = ""
        End If

        With mp
            tmpResult = .CurMaintVal

            Do
                Select Case mp.ForecastType
                    Case ForecastType.Prod_v_Inflator
                        resultLP = (.ListPrice * (.PctOfLP / 100))
                        calcLine(1) = "<b>Result 1</b> = " & Trim(Str(mp.PctOfLP)) & "% of Product Cost (" & Format(mp.ListPrice, "#,###,##0.00") & ") = " & Format(resultLP, "#,###,##0.00")
                        calcLine(2) = "<b>OR</b>"

                    Case ForecastType.InflatorOnly
                        resultLP = 0
                        calcLine(1) = "<b>Result 1</b> = <i>Percentage of Product Cost calculation bypassed</i>"
                        calcLine(2) = "<b>THEREFORE</b>"

                    Case ForecastType.Staged

                End Select
                ' always use result X for any of the three options
                resultX = tmpResult + (tmpResult * ((.MaintTypeValueX + .MaintExtraPercentX) / 100))

                Select Case .MaintCalcType
                    Case 0  ' flat rate - use X vals only
                        calcLine(3) = "Fixed % increase on current maintenance"
                        calcLine(4) = "<b>Result 2</b> = Current Maintenance (" & Format(.CurMaintVal, "#,###,##0.00") & ") x (" & Trim(Str(.MaintTypeValueX)) & "% + " & Trim(Str(.MaintExtraPercentX)) & "%) = " & Format(resultX, "#,###,##0.00")

                    Case 1, 2 ' greater / lesser of x and y
                        resultY = tmpResult + (tmpResult * ((.MaintTypeValueY + .MaintExtraPercentY) / 100))
                        If .MaintCalcType = 1 Then
                            calcLine(3) = "The Greater of calculations X and Y :- "
                        Else
                            calcLine(3) = "The Lesser of calculations X and Y :- "
                        End If

                        calcLine(4) = "<b>Result X</b> = Current Maintenance (" & Format(.CurMaintVal, "#,###,##0.00") & ") x (" & Trim(Str(.MaintTypeValueX)) & "% + " & Trim(Str(.MaintExtraPercentX)) & "%) = " & Format(resultX, "#,###,##0.00")
                        calcLine(5) = "<b>AND</b>"
                        calcLine(6) = "<b>Result Y</b> = Current Maintenance (" & Format(.CurMaintVal, "#,###,##0.00") & ") x (" & Trim(Str(.MaintTypeValueY)) & "% + " & Trim(Str(.MaintExtraPercentY)) & "%) = " & Format(resultY, "#,###,##0.00")

                    Case Else
                End Select

                ' take the desired value
                Select Case .MaintCalcType
                    Case 0
                        tmpResult = resultX

                    Case 1 ' take greater value
                        calcLine(7) = "<b>Result Used (Result 2)</b> = "
                        If resultX >= resultY Then
                            If resultX <> 0 Then
                                tmpResult = resultX
                                calcLine(8) = "Result X " & "(" & Format(resultX, "#,###,##0.00") & ")"
                            Else
                                tmpResult = resultY
                                calcLine(8) = "Result Y " & "(" & Format(resultY, "#,###,##0.00") & ")"
                            End If
                        Else
                            If resultY <> 0 Then
                                tmpResult = resultY
                                calcLine(8) = "Result Y " & "(" & Format(resultY, "#,###,##0.00") & ")"
                            Else
                                tmpResult = resultX
                                calcLine(8) = "Result X " & "(" & Format(resultX, "#,###,##0.00") & ")"
                            End If
                        End If

                    Case 2 ' take lesser value
                        calcLine(7) = "Result 2 = "
                        If resultX >= resultY Then
                            If resultY <> 0 Then
                                tmpResult = resultY
                                calcLine(8) = "Result Y " & "(" & Format(resultY, "#,###,##0.00") & ")"
                            Else
                                tmpResult = resultX
                                calcLine(8) = "Result X " & "(" & Format(resultX, "#,###,##0.00") & ")"
                            End If
                        Else
                            If resultX <> 0 Then
                                tmpResult = resultX
                                calcLine(8) = "Result X " & "(" & Format(resultX, "#,###,##0.00") & ")"
                            Else
                                tmpResult = resultY
                                calcLine(8) = "Result Y " & "(" & Format(resultY, "#,###,##0.00") & ")"
                            End If
                        End If

                    Case Else
                End Select

                maintYear = maintYear - 1
            Loop While maintYear >= 0

            ' take the lesser of the calculated value and % of LP
            calcLine(9) = "<b><u>Next Period Maintenance = "

            If resultLP < tmpResult Then
                If resultLP <> 0 Then
                    retVal.NYMValue = resultLP
                    calcLine(10) = "Result 1 " & "(" & Format(resultLP, "#,###,##0.00") & ")"
                Else
                    retVal.NYMValue = tmpResult
                    calcLine(10) = "Result 2 " & "(" & Format(tmpResult, "#,###,##0.00") & ")"
                End If
            Else
                If tmpResult <> 0 Then
                    retVal.NYMValue = tmpResult
                    calcLine(10) = "Result 2 " & "(" & Format(tmpResult, "#,###,##0.00") & ")"
                Else
                    retVal.NYMValue = resultLP
                    calcLine(10) = "Result 1 " & "(" & Format(resultLP, "#,###,##0.00") & ")"
                End If
            End If

            calcLine(10) = calcLine(10) & "</u></b>"

            ' construct HTML table for output of results
            tmpStr = "<table class=""data"" align=""center"">" & vbNewLine
            tmpStr = tmpStr & "<tr class=""main"">" & vbNewLine
            tmpStr = tmpStr & "<td class=""main"">" & headerLine(1) & "</td></tr>" & vbNewLine
            tmpStr = tmpStr & "<tr class=""main"">" & vbNewLine
            tmpStr = tmpStr & "<td class=""main"">" & headerLine(2) & "</td></tr>" & vbNewLine
            Dim x As Integer
            For x = 1 To 10
                tmpStr = tmpStr & "<tr class=""main"">" & vbNewLine
                tmpStr = tmpStr & "<td class=""main"" align=""center"">" & calcLine(x) & "</td>"
                tmpStr = tmpStr & "</tr>" & vbNewLine
            Next

            tmpStr = tmpStr & "</table>"

            retVal.NYMCalculation = tmpStr

            CalcNYM = retVal
        End With
    End Function

    Public Function GetMaintParams(ByVal contractID As Integer) As MaintParams
        Dim sql As String
        Dim paramstruct As MaintParams

        sql = "SELECT [Forecast Type Id],[Maintenance Type],[Maintenance Inflator X],[Maintenance Percent X],[X].[Percentage] AS [PercentageX],[Maintenance Inflator Y],[Maintenance Percent Y],[Y].[Percentage] AS [PercentageY] FROM [Contract Details]" & vbNewLine & _
            "LEFT OUTER JOIN [Codes - Inflator Metrics] AS [X] ON [Contract Details].[Maintenance Inflator X] = [X].[Metric Id]" & vbNewLine & _
            "LEFT OUTER JOIN [Codes - Inflator Metrics] AS [Y] ON [Contract Details].[Maintenance Inflator Y] = [Y].[Metric Id]" & vbNewLine & _
            "WHERE [Contract Id] = " & Trim(Str(contractID))
        db.RunSQL(sql, db.glDBWorkD)
        If DB.GetRowCount(DB.glDBWorkD) > 0 Then
            With paramstruct
                .ForecastType = Val(DB.GetFieldValue(DB.glDBWorkD, "Forecast Type Id", 0))
                .MaintCalcType = Val(DB.GetFieldValue(DB.glDBWorkD, "Maintenance Type", 0))
                .MaintTypeX = Val(DB.GetFieldValue(DB.glDBWorkD, "Maintenance Inflator X", 0))
                .MaintTypeValueX = Val(DB.GetFieldValue(DB.glDBWorkD, "PercentageX", 0))
                .MaintExtraPercentX = Val(DB.GetFieldValue(DB.glDBWorkD, "Maintenance Percent X", 0))
                .MaintTypeY = Val(DB.GetFieldValue(DB.glDBWorkD, "Maintenance Inflator Y", 0))
                .MaintTypeValueY = Val(DB.GetFieldValue(DB.glDBWorkD, "PercentageY", 0))
                .MaintExtraPercentY = Val(DB.GetFieldValue(DB.glDBWorkD, "Maintenance Percent Y", 0))

            End With
        Else
            With paramstruct
                .ForecastType = ForecastType.Prod_v_Inflator
                .MaintCalcType = 0
                .MaintTypeX = 0
                .MaintTypeValueX = 0
                .MaintExtraPercentX = 0
                .MaintTypeY = 0
                .MaintTypeValueY = 0
                .MaintExtraPercentY = 0
            End With
        End If

        GetMaintParams = paramstruct
    End Function

    Public Function GetProducts(ByVal mParams As MaintParams, ByVal ContractID As Long) As String
        Dim sql As String
        Dim dRow As DataRow
        Dim strOut As String
        Dim NYmRes As NYMResult

        sql = "SELECT [Product Details].[Product Name],[Maintenance Value],[Product Value],[Maintenance Percent] FROM [Contract - Product Details] "
        sql += "INNER JOIN [Product Details] ON [Product Details].[Product Id] = [Contract - Product Details].[Product Id] "
        sql += "WHERE [Contract Id] = " & ContractID

        DB.RunSQL(sql, DB.glDBWorkA)

        If DB.glNumRowsReturned > 0 Then
            For Each dRow In DB.glDBWorkA.Tables(0).Rows
                mParams.CurMaintVal = dRow.Item("Maintenance Value")

                mParams.ListPrice = dRow.Item("Product Value")

                mParams.PctOfLP = dRow.Item("Maintenance Percent")

                NYmRes = CalcNYM(mParams, 0)

                strOut = strOut & dRow.Item("Product Name") & " - Current maintenance (£" & dRow.Item("Maintenance Value") & ") increase is capped not to exceed £" & NYmRes.NYMValue & " next year." & vbNewLine
            Next
            Return "Product Maintenance: " & vbNewLine & strOut

        Else
            Return ""
        End If


    End Function
End Module
