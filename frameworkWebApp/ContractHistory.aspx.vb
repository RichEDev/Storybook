Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class ContractHistory
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            Title = "Contract History"
            Master.title = Title
            'Master.enablenavigation = False
            Master.useCloseNavigationMsg = True

            If Me.IsPostBack = False Then
                curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractHistory, False, True)

                Dim db As New cFWDBConnection
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
                db.DBOpen(fws, False)

                Select Case Request.QueryString("action")
                    Case "delete"
                        curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractHistory, False, True)
                        Session("ExpId") = Request.QueryString("st")
                        DeleteHistoryItem(db, Request.QueryString("id"))
                    Case Else

                End Select

                SetTabOptions(db, curUser)
                If Request.QueryString("lc") = "1" Then
                    lnkLCnav.Visible = True
                End If

                litHistory.Text = GetContractHistory(db, curUser, Session("ActiveContract"), Session("ExpId"))
                db.DBClose()
                db = Nothing
            End If
        End Sub

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            Session("ExpId") = Nothing
            Response.Redirect("Home.aspx", True)
        End Sub

        Private Function GetContractHistory(ByVal db As cFWDBConnection, ByVal curUser As CurrentUser, ByVal contractId As Integer, ByVal ExpandSectionId As SummaryTabs) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim strHeader As New System.Text.StringBuilder
            Dim sql As New System.Text.StringBuilder

            sql.Append("SELECT contract_history.*,[contract_details].[ContractDescription] FROM [contract_history] " & vbNewLine)
            sql.Append("INNER JOIN [contract_details] ON [contract_history].[ContractId] = [contract_details].[ContractId] " & vbNewLine)
            sql.Append("WHERE [contract_history].[ContractId] = @contractId ORDER BY [SummaryTab],[ActionDate] DESC")
            db.AddDBParam("contractId", Session("ActiveContract"), True)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            If db.glNumRowsReturned = 0 Then
                ' no contract history exists
                With strHTML
                    .Append("<div class=""formpanel formpanel_padding"">" & vbNewLine)
                    .Append("<div class=""inputpaneltitle"">Contract History</div>" & vbNewLine)
                    .Append("<table class=""datatbl""><tr><td class=""row1"" width=""250"" align=""center"">No history records currently exist</td></tr></table>" & vbNewLine)
                    .Append("</div>" & vbNewLine)
                End With
            Else
                Dim drow As DataRow
                Dim rowalt As Boolean = False
                Dim rowClass As String = "row1"
                Dim summaryTabArea As SummaryTabs = Nothing
                Dim firstTable As Boolean = True
                Dim tmpDate As String

                With strHeader
                    .Append("<table class=""datatbl"">" & vbNewLine)
                    .Append("<tr>" & vbNewLine)
                    If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractHistory, False) Then
                        .Append("<th style=""width:30px""><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
                    End If
                    .Append("<th>Action Date</th>" & vbNewLine)
                    .Append("<th>Action Taken</th>" & vbNewLine)
                    .Append("<th>Modifier</th>" & vbNewLine)
                    .Append("<th>Change Element</th>" & vbNewLine)
                    .Append("<th>Pre-Change Value</th>" & vbNewLine)
                    .Append("<th>Post Change Value</th>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                End With

                With strHTML
                    For Each drow In db.glDBWorkA.Tables(0).Rows
                        If summaryTabArea <> drow.Item("SummaryTab") Or firstTable = True Then
                            summaryTabArea = drow.Item("SummaryTab")
                            rowalt = False

                            Dim expType As String = "none"
                            Dim expImg As String = "open"

                            If ExpandSectionId = summaryTabArea Then
                                expType = "block"
                                expImg = "close"
                            End If
                            ' summary area changed, so start new section
                            If firstTable = False Then
                                .Append("</table>" & vbNewLine)
                                .Append("</div>" & vbNewLine)
                                .Append("</div>" & vbNewLine)
                            End If

                            .Append("<div class=""formpanel formpanel_padding"">" & vbNewLine)
                            .Append("<div class=""inputpaneltitle""><a style=""cursor:hand;"" onclick=""javascript:toggle('" & summaryTabArea & "');""><img src=""./buttons/" & expImg & ".gif"" id=""STimg" & summaryTabArea & """ /></a>")
                            Select Case summaryTabArea
                                Case SummaryTabs.ContractDetail
                                    .Append("Contract Detail")
                                Case SummaryTabs.ContractAdditional
                                    .Append("Additional Contract Detail")
                                Case SummaryTabs.ContractProduct
                                    .Append("Contract Products")
                                Case SummaryTabs.InvoiceDetail
                                    .Append("Invoice Details")
                                Case SummaryTabs.InvoiceForecast
                                    .Append("Invoice Forecasts")
                                Case Else
                                    .Append("Unspecified Area")
                            End Select
                            .Append(" - " & drow.Item("ContractDescription"))
                            .Append("</div>" & vbNewLine)

                            .Append("<div id=""ST" & summaryTabArea & """ style=""display: " & expType & ";"">" & vbNewLine)
                            .Append(strHeader.ToString)

                            firstTable = False
                        End If

                        rowalt = (rowalt Xor True)
                        If rowalt Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        tmpDate = Format(drow.Item("actionDate"), cDef.DATE_FORMAT)

                        .Append("<tr>" & vbNewLine)
                        If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractHistory, False) Then
                            .Append("<td class=""" & rowClass & """><a onclick=""javascript:if(confirm('Click OK to confirm deletion of history item')){window.location.href='ContractHistory.aspx?action=delete&id=" & drow.Item("HistoryId") & "&st=" & summaryTabArea & "'};"" onmouseover=""window.status='Delete History item';return true;"" onmouseout=""window.status='Done';"" title=""Delete History Item"" style=""cursor: hand;""><img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                        End If
                        .Append("<td class=""" & rowClass & """>" & tmpDate & "</td>" & vbNewLine)
                        .Append("<td class=""" & rowClass & """>" & drow.Item("Action") & "</td>" & vbNewLine)
                        .Append("<td class=""" & rowClass & """>" & drow.Item("ModifierName") & "</td>" & vbNewLine)
                        .Append("<td class=""" & rowClass & """>" & drow.Item("ChangeField") & "</td>" & vbNewLine)
                        .Append("<td class=""" & rowClass & """>" & drow.Item("PreVal") & "</td>" & vbNewLine)
                        .Append("<td class=""" & rowClass & """>" & drow.Item("PostVal") & "</td>" & vbNewLine)
                        .Append("</tr>" & vbNewLine)
                    Next

                    strHTML.Append("</table>" & vbNewLine)
                    strHTML.Append("</div>" & vbNewLine)
                    strHTML.Append("</div>" & vbNewLine)
                End With
            End If
            Return strHTML.ToString
        End Function

        Private Sub DeleteHistoryItem(ByVal db As cFWDBConnection, ByVal historyId As Integer)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            If historyId > 0 Then
                Dim ARec As New cAuditRecord

                db.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                If db.FWDb2Flag Then
                    db.FWDb("R3", "contract_history", "HistoryId", historyId, "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        ARec.Action = cFWAuditLog.AUDIT_DEL
                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                        Select Case CType(db.FWDbFindVal("SummaryTab", 3), SummaryTabs)
                            Case SummaryTabs.ContractDetail
                                ARec.DataElementDesc = "CON.DETAIL HISTORY ITEM"
                            Case SummaryTabs.ContractAdditional
                                ARec.DataElementDesc = "CON.ADDITIONAL HISTORY ITEM"
                            Case SummaryTabs.ContractProduct
                                ARec.DataElementDesc = "CON.PRODUCT HISTORY ITEM"
                            Case SummaryTabs.InvoiceDetail
                                ARec.DataElementDesc = "INVOICE HISTORY ITEM"
                            Case SummaryTabs.InvoiceForecast
                                ARec.DataElementDesc = "FORECAST HISTORY ITEM"
                            Case SummaryTabs.RechargeTemplate
                                ARec.DataElementDesc = "RECHARGE TEMPLATE HISTORY ITEM"
                            Case SummaryTabs.RechargePayments
                                ARec.DataElementDesc = "RECHARGE PAYMENT HISTORY ITEM"
                            Case Else
                                ARec.DataElementDesc = "UNSPECIFIED TYPE HISTORY ITEM"
                        End Select
                        ARec.ElementDesc = db.FWDbFindVal("ChangeField", 3)
                        ARec.PreVal = db.FWDbFindVal("PreVal", 3)
                        ARec.PostVal = db.FWDbFindVal("PostVal", 3)
                        Dim ALog As New cFWAuditLog(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), SpendManagementElement.ContractHistory, curUser.CurrentSubAccountId)
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, historyId)

                        db.FWDb("D", "contract_history", "HistoryId", historyId, "", "", "", "", "", "", "", "", "", "")
                    End If
                End If
            End If
        End Sub

		Protected Sub lnkCDnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCDnav.Click
			Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract"), True)
		End Sub

		Protected Sub lnkCAnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCAnav.Click
			Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractAdditional & "&id=" & Session("ActiveContract"), True)
		End Sub

		Protected Sub lnkCPnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCPnav.Click
			Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractProduct & "&id=" & Session("ActiveContract"), True)
		End Sub

		Protected Sub lnkIDnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkIDnav.Click
			Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & Session("ActiveContract"), True)
		End Sub

		Protected Sub lnkIFnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkIFnav.Click
			Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
		End Sub

		Protected Sub lnkNSnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNSnav.Click
			Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.NotesSummary & "&id=" & Session("ActiveContract"), True)
		End Sub

		Protected Sub lnkLCnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkLCnav.Click
			Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.LinkedContracts & "&id=" & Session("ActiveContract"), True)
		End Sub

		Protected Sub lnkCHnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCHnav.Click
			Response.Redirect("ContractHistory.aspx", True)
		End Sub

		Private Sub SetTabOptions(ByVal db As cFWDBConnection, ByVal curUser As CurrentUser)
			If Session("ActiveContract") Is Nothing Then
				Session("ActiveContract") = 0
			End If

			Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

			Dim omout As String = "window.status='Done';"
			Dim omover As String = "window.status='%msg%';return true;"

			If Session("ActiveContract") > 0 Then
				If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractAdditional, False) = False Then
					lnkCAnav.Visible = False
				Else
					lnkCAnav.ToolTip = "Display additional contract details"
					lnkCAnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCAnav.ToolTip))
					lnkCAnav.Attributes.Add("onmouseout", omout)
				End If

				If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProducts, False) = False Then
					lnkCPnav.Visible = False
				Else
					lnkCPnav.ToolTip = "Display contract products associated with this contract"
					lnkCPnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCPnav.ToolTip))
					lnkCPnav.Attributes.Add("onmouseout", omout)
				End If

				If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Invoices, False) = False Then
					lnkIDnav.Visible = False
				Else
					lnkIDnav.ToolTip = "Display invoice details associated with the active contract"
					lnkIDnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkIDnav.ToolTip))
					lnkIDnav.Attributes.Add("onmouseout", omout)
				End If

				If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceForecasts, False) = False Then
					lnkIFnav.Visible = False
				Else
					lnkIFnav.ToolTip = "Display forecast payments associated with the active contract"
					lnkIFnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkIFnav.ToolTip))
					lnkIFnav.Attributes.Add("onmouseout", omout)
				End If

				If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.NotesSummary, False) = False Then
					lnkNSnav.Visible = False
				Else
					lnkNSnav.ToolTip = "Display collated summary of free text notes associated with the active contract"
					lnkNSnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNSnav.ToolTip))
					lnkNSnav.Attributes.Add("onmouseout", omout)
				End If

				lnkCHnav.ToolTip = "Display amendment history of the active contract"
				lnkCHnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCHnav.ToolTip))
				lnkCHnav.Attributes.Add("onmouseout", omout)

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractLinks, False) = True Then
                    ' links with the contract exist, so display the tab and separator image
                    lnkLCnav.Visible = True
                    lnkLCnav.ToolTip = "Display contracts linked via definitions"
                    lnkLCnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkLCnav.ToolTip))
                    lnkLCnav.Attributes.Add("onmouseout", omout)
                End If

				If params.EnableRecharge And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAssociations, False) Then
					Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
					Dim rs As cRechargeSetting = rscoll.getSettings

					If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAssociations, False) Then
						lnkRPnav.Visible = True
						lnkRPnav.ToolTip = "View recharge payments defined against contract products"
						lnkRPnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkRPnav.ToolTip))
						lnkRPnav.Attributes.Add("onmouseout", omout)
					End If

					If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargePayments, False) Then
						lnkRTnav.Visible = True
						lnkRTnav.ToolTip = "Define / Modify recharge apportioning between " & rs.ReferenceAs & "s"
						lnkRTnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkRTnav.ToolTip))
						lnkRTnav.Attributes.Add("onmouseout", omout)
					End If
				End If
			Else
				' must be adding a contract, so hide other tabs other than contract detail
				lnkCAnav.Visible = False
				lnkCPnav.Visible = False
				lnkIDnav.Visible = False
				lnkIFnav.Visible = False
				lnkNSnav.Visible = False
				lnkLCnav.Visible = False
				lnkRTnav.Visible = False
				lnkRPnav.Visible = False
				lnkCHnav.Visible = False
			End If
		End Sub

        Protected Sub lnkRTnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkRTnav.Click
            Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub lnkRPnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkRPnav.Click

        End Sub
    End Class
End Namespace
