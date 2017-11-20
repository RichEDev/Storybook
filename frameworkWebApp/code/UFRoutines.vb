Imports Microsoft.VisualBasic
Imports SpendManagementLibrary
Imports System.Web.UI

Namespace Framework2006
    Module UFRoutines
        Public Function CheckMandatoryUF(ByRef ActivePage As Page, ByVal db As cFWDBConnection, ByVal AppArea As AppAreas, Optional ByVal checkGroupings As Boolean = False, Optional ByVal ContractCategory As Integer = 0) As Boolean
            Dim sql As New System.Text.StringBuilder

            If checkGroupings = True Then
                Select Case AppArea
                    Case AppAreas.CONTRACT_DETAILS
                        sql.Append("SELECT * FROM [user_fields] ")
                        sql.Append("WHERE [AppArea] = @apparea AND [Mandatory] = 1")
                        db.AddDBParam("apparea", AppArea, True)

                    Case AppAreas.CONTRACT_GROUPING

                    Case AppAreas.CONTRACT_PRODUCTS, AppAreas.CONPROD_GROUPING
                        sql.Append("SELECT ")
                        'sql.Append(GetUFieldList(db, ContractCategory))
                    Case Else
                        Return True
                        Exit Function
                End Select
                sql.Append("")
            End If
            sql.Append("WHERE [AppArea] = @apparea")

            Return True
        End Function

    End Module


End Namespace