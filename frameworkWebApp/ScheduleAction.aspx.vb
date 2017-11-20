Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class ScheduleAction
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection

            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSchedules, False, True)

            db.DBOpen(fws, False)

            ' if no schedules exist, then hide merge option
            Dim hideMergeFunction As String = "HideMerge();"

            Dim sql As New System.Text.StringBuilder
            sql.Append("SELECT [link_matrix].[LinkId], [link_matrix].[ContractId] " & vbNewLine)
            sql.Append("FROM [link_matrix] " & vbNewLine)
            sql.Append("WHERE [LinkId] IN ( " & vbNewLine)
            sql.Append("SELECT [link_definitions].[LinkId] FROM [link_definitions] ")
            sql.Append("INNER JOIN [link_matrix] ON [link_matrix].[LinkId] = [link_definitions].[LinkId] ")
            sql.Append("WHERE [link_matrix].[ContractId] = @conId AND [IsScheduleLink] = 1) ")
            sql.Append("AND dbo.CheckContractAccess(@userId,[link_matrix].[ContractId], @subAccountId) > 0")
            db.AddDBParam("conId", Session("ActiveContract"), True)
            db.AddDBParam("userId", curUser.EmployeeID, False)
            db.AddDBParam("subAccountID", curUser.CurrentSubAccountId, False)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)
            If db.glNumRowsReturned > 0 Then
                hideMergeFunction = ""
            End If

            Master.onloadfunc = "SetContractId(" & Session("ActiveContract") & ");SetPermissions(" & curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractDetails, False).ToString.ToLower & "," & curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractDetails, False).ToString.ToLower & "," & curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractDetails, False).ToString.ToLower & ");" & hideMergeFunction
            Title = "Contract Schedule Actions"
            Master.title = Title

            Select Case Request.QueryString("action")
                Case "delete"
                    curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractSchedules, False, True)

                    If DeleteLink(db, Request.QueryString("linkid")) = False Then
                        lblError.Text = "An error occurred trying to remove schedule link"
                    Else
                        db.DBClose()
                        Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract"), True)
                    End If

                Case Else

            End Select
            litCurrentSchedules.Text = GetSchedules(db, curUser, Session("ActiveContract"))

            db.DBClose()
            db = Nothing
        End Sub

        Private Function GetSchedules(ByVal db As cFWDBConnection, ByVal curUser As CurrentUser, ByVal contractId As Integer) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim sql As New System.Text.StringBuilder

            sql.Append("SELECT dbo.CheckContractAccess(@userId,[link_matrix].[ContractId], @subAccountId) AS [PermitVal],[link_matrix].[LinkId],[link_matrix].[ContractId],[contract_details].[ContractDescription],[contract_details].[ContractKey],ISNULL([contract_details].[ScheduleNumber],'') AS [ScheduleNumber],ISNULL([contract_details].[EndDate],'') AS [EndDate],ISNULL([contract_details].[StartDate],'') AS [StartDate],ISNULL([codes_contractstatus].[StatusDescription],'') AS [ContractStatus] FROM [link_matrix] " & vbNewLine)
            sql.Append("INNER JOIN [contract_details] ON [link_matrix].[ContractId] = [contract_details].[ContractId] " & vbNewLine)
            sql.Append("LEFT JOIN [codes_contractstatus] ON [contract_details].[ContractStatusId] = [codes_contractstatus].[StatusId] " & vbNewLine)
            sql.Append("WHERE [LinkId] IN (" & vbNewLine)
            sql.Append("SELECT [link_definitions].[LinkId] FROM [link_definitions] " & vbNewLine)
            sql.Append("INNER JOIN [link_matrix] ON [link_matrix].[LinkId] = [link_definitions].[LinkId] " & vbNewLine)
            sql.Append("WHERE [link_matrix].[ContractId] = @conId AND [IsScheduleLink] = 1) " & vbNewLine)
            sql.Append("ORDER BY [StartDate],[EndDate] ASC" & vbNewLine)
            db.AddDBParam("conId", Session("ActiveContract"), True)
            db.AddDBParam("userId", curUser.EmployeeID, False)
            db.AddDBParam("subAccountId", curUser.CurrentSubAccountId, False)
            db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)

            Dim drow As DataRow
            Dim curId As Integer
            curId = Session("ActiveContract")

            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th style=""width:30px""></th>" & vbNewLine)
            strHTML.Append("<th style=""width:30px""><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
            strHTML.Append("<th>Contract Key</th>" & vbNewLine)
            strHTML.Append("<th>Contract Description</th>" & vbNewLine)
            strHTML.Append("<th>Schedule Number</th>" & vbNewLine)
            strHTML.Append("<th>Start Date</th>" & vbNewLine)
            strHTML.Append("<th>End Date</th>" & vbNewLine)
            strHTML.Append("<th>Contract Status</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            If db.glNumRowsReturned > 0 Then
                Dim rowalt As Boolean = False
                Dim rowClass As String = "row1"
                Dim tmpDate As String = ""

                For Each drow In db.glDBWorkD.Tables(0).Rows
                    rowalt = (rowalt Xor True)
                    If rowalt Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    strHTML.Append("<tr>" & vbNewLine)
                    If drow.Item("ContractId") = curId Then
                        strHTML.Append("<td class=""" & rowClass & """><img src=""./images/arrow_right.png"" /></td>" & vbNewLine)
                    Else
                        strHTML.Append("<td class=""" & rowClass & """></td>" & vbNewLine)
                    End If

                    If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractSchedules, False) And drow.Item("PermitVal") <> 0 Then
                        strHTML.Append("<td class=""" & rowClass & """><a onclick=""javascript:if(confirm('Click OK to confirm removal of schedule link')){window.location.href='ScheduleAction.aspx?action=delete&linkid=" & drow.Item("LinkId") & "&cid=" & drow.Item("ContractId") & "';}"" onmouseover=""window.status='Delete this contract link from schedule chain';return true;"" onmouseout=""window.status='Done';"" title=""Delete this contract link from schedule chain"" style=""cursor: hand;""><img src=""./icons/delete2.gif"" /></td>" & vbNewLine)
                    Else
                        strHTML.Append("<td class=""" & rowClass & """></td>" & vbNewLine)
                    End If
                    strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("ContractKey") & "</td>" & vbNewLine)
                    If drow.Item("PermitVal") <> 0 Then
                        strHTML.Append("<td class=""" & rowClass & """><a href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & drow.Item("ContractId") & """>" & drow.Item("ContractDescription") & "</a></td>" & vbNewLine)
                        strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("ScheduleNumber") & "</td>" & vbNewLine)
                        If IsDate(drow.Item("StartDate")) Then
                            tmpDate = Format(CDate(drow.Item("StartDate")), cDef.DATE_FORMAT)
                        Else
                            tmpDate = ""
                        End If
                        strHTML.Append("<td class=""" & rowClass & """>" & tmpDate & "</td>" & vbNewLine)
                        If IsDate(drow.Item("EndDate")) Then
                            tmpDate = Format(CDate(drow.Item("EndDate")), cDef.DATE_FORMAT)
                        Else
                            tmpDate = ""
                        End If
                        strHTML.Append("<td class=""" & rowClass & """>" & tmpDate & "</td>" & vbNewLine)
                        strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("ContractStatus") & "</td>" & vbNewLine)
                    Else
                        strHTML.Append("<td class=""" & rowClass & """ colspan=""5"" align=""center"">*** User access to this schedule denied ***</td>" & vbNewLine)
                    End If
                    strHTML.Append("</tr>" & vbNewLine)
                Next
            Else
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""row1"" colspan=""8"" align=""center"">No Schedules currently exist</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
            End If
            strHTML.Append("</table>" & vbNewLine)

            Return strHTML.ToString
        End Function

        Private Function DeleteLink(ByVal db As cFWDBConnection, ByVal linkId As Integer) As Boolean
            Try
                Dim curUser As CurrentUser = cMisc.GetCurrentUser()
                Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim cId As Integer
                Dim ARec As New cAuditRecord

                cId = Request.QueryString("cid")

                ARec.Action = cFWAuditLog.AUDIT_DEL
                ARec.DataElementDesc = "SCHEDULE LINK"

                If cId > 0 Then
                    ' remove the contract schedule link for this contract
                    db.FWDb("R2", "contract_details", "ContractId", cId, "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb2Flag = True Then
                        Dim Sql As New System.Text.StringBuilder
                        Sql.Append("SELECT COUNT(*) AS [NumLinks],[LinkId] FROM [link_matrix] WHERE [LinkId] = (")
                        Sql.Append("SELECT [LinkId] FROM [link_matrix] WHERE [ContractId] = " & cId.ToString.Trim & " ")
                        Sql.Append("AND [LinkId] = (SELECT [link_definitions].[LinkId] FROM [link_matrix] ")
                        Sql.Append("INNER JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId]" & " ")
                        Sql.Append("WHERE [link_definitions].[IsScheduleLink] = 1 AND [ContractId] = @cId)")
                        Sql.Append(") GROUP BY [LinkId]")
                        db.AddDBParam("cId", cId, True)
                        db.RunSQL(Sql.ToString, db.glDBWorkI, False, "", False)

                        If db.GetFieldValue(db.glDBWorkI, "NumLinks", 0, 0) <= 2 Then
                            ' this will leave a single link chain, so remove all links and the link definition
                            Dim curlinkId As Integer
                            curlinkId = db.GetFieldValue(db.glDBWorkI, "LinkId", 0, 0)

                            db.FWDb("D", "link_matrix", "LinkId", curlinkId, "", "", "", "", "", "", "", "", "", "")
                            db.FWDb("D", "link_definitions", "LinkId", curlinkId, "", "", "", "", "", "", "", "", "", "")
                        Else
                            ' remove just the required schedule link
                            Sql = New System.Text.StringBuilder
                            Sql.Append("DELETE FROM [link_matrix] WHERE [ContractId] = @cId ")
                            Sql.Append("AND [LinkId] = (SELECT [link_definitions].[LinkId] FROM [link_matrix] ")
                            Sql.Append("INNER JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] ")
                            Sql.Append("WHERE [link_definitions].[IsScheduleLink] = 1 AND [ContractId] = @cId)")
                            db.AddDBParam("cId", cId, True)
                            db.ExecuteSQL(Sql.ToString)
                        End If

                        ARec.Action = cFWAuditLog.AUDIT_DEL
                        If Trim(db.FWDbFindVal("ContractKey", 2)) <> "" Then
                            ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                        Else
                            ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 2)
                        End If
                        ARec.DataElementDesc = "CONTRACT SCHEDULE"
                        ARec.ElementDesc = "REMOVE SCHEDULE LINK"
                        ARec.PreVal = ""
                        ARec.PostVal = ""
                        Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractSchedules, curUser.CurrentSubAccountId)
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, cId)
                    End If
                End If

                Return True

            Catch ex As Exception
                Return False
            End Try
        End Function

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract"), True)
        End Sub

        Public Sub New()

        End Sub
    End Class
End Namespace
