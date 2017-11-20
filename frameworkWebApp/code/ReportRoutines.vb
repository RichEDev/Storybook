Imports Microsoft.VisualBasic
Imports SpendManagementLibrary

Namespace Framework2006
    Public Module ReportRoutines
        ' Generic routines used by the custom reporting.
        ' Functions and Procedures held in this module in order to maintain tidiness within core application code.
        Public Function RemoveCurrencyFormat(ByVal fmtStr As String) As Double
            Dim i As Integer

            For i = 1 To fmtStr.Length
                If Mid(fmtStr, i, 1) = "." Or IsNumeric(Mid(fmtStr, i, 1)) = True Then
                    ' ok
                Else
					fmtStr = Replace(fmtStr, Mid(fmtStr, i, 1), "#", False)
                End If
            Next
            fmtStr = Replace(fmtStr, "#", "")

            If IsNumeric(fmtStr) = True Then
                RemoveCurrencyFormat = Val(fmtStr)
            Else
                RemoveCurrencyFormat = 0
            End If
        End Function
    End Module
End Namespace
