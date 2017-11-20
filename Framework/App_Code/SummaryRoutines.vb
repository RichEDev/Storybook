Imports Microsoft.VisualBasic
Imports SpendManagementLibrary

Namespace Framework2006
    Public Module SummaryRoutines
        Public Function SetStyle(ByVal employee As cEmployee, Optional ByVal isHomePage As Boolean = True) As String ', ByVal xMax As Integer, ByVal yMax As Integer, 
            Dim script As New System.Text.StringBuilder
            'Dim bmHeight As Integer
            'Dim bmWidth As Integer
            'Dim xVal As Integer
            'Dim yVal As Integer

            'If isHomePage Then
            '	xVal = xMax
            '	yVal = yMax
            'Else
            '	xVal = xMax - 180
            '	yVal = yMax
            'End If

            script.Append("<style type=""text/css"">" & vbNewLine)
            script.Append("#righthandcontent" & vbNewLine)
            script.Append("{" & vbNewLine)
            If employee.username = "admin" Then
                script.Append("display: none;" & vbNewLine)
            End If
            script.Append("position: absolute; " & vbNewLine)
            script.Append("top: 135px;" & vbNewLine)
            script.Append("left: 50%;" & vbNewLine)
            script.Append("width: 40%;" & vbNewLine)
            'script.Append("height: " & vbNewLine)
            script.Append("}" & vbNewLine)

            'bmHeight = CInt((yVal / 100) * 75)
            'bmWidth = CInt((xVal / 100) * 75)

            script.Append("#broadcastmsg" & vbNewLine)
            script.Append("{" & vbNewLine)
            script.Append("position: absolute; " & vbNewLine)
            script.Append("left: 20%;" & vbNewLine)
            script.Append("top: 20%;" & vbNewLine)
            script.Append("width: 70%;" & vbNewLine)
            script.Append("height: 70%;" & vbNewLine)
            script.Append("}" & vbNewLine)

            script.Append("#broadcasttxt" & vbNewLine)
            script.Append("{" & vbNewLine)
            script.Append("width: 95%" & vbNewLine)
            script.Append("height: 85%;" & vbNewLine)
            script.Append("}" & vbNewLine)
            script.Append("</style>" & vbNewLine)

            Return script.ToString
        End Function
    End Module
End Namespace