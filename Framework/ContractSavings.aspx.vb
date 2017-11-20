Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class ContractSavings
        Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
			Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
			Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)

            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSavings, False, True)

			If Me.IsPostBack = False Then
				Dim db As New cFWDBConnection

				Title = "Contract Savings"
				Master.title = Title

				db.DBOpen(fws, False)

				Dim action As String
				action = Request.QueryString("action")
				Select Case action
                    Case "edit"
                        curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractSavings, False, True)

                        EditSaving(db, Request.QueryString("savingid"), curUser)

                    Case "delete"
                        curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractSavings, False, True)

                        Dim sId As Integer
                        sId = Request.QueryString("savingid")

                        db.FWDb("R2", "savings", "SavingsId", sId, "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            Dim ARec As New cAuditRecord
                            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractSavings, curUser.CurrentSubAccountId)

                            db.FWDb("R3", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                            If db.FWDb3Flag Then
                                ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3)
                            End If
                            ARec.Action = cFWAuditLog.AUDIT_DEL
                            ARec.DataElementDesc = "CONTRACT SAVING"
                            ARec.ElementDesc = "SAVING ENTRY"
                            ARec.PreVal = db.FWDbFindVal("Amount", 2)

                            ARec.PostVal = dateSaving.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, sId)

                            db.FWDb("D", "savings", "SavingsId", sId, "", "", "", "", "", "", "", "", "", "")
                        End If

					Case Else
				End Select

				If action <> "edit" Then
					' preset the date display range for the from and to date boxes
					If Session("SavingFrom") Is Nothing Then
						dateFrom.Value = DateAdd(DateInterval.Month, -3, Today)
					Else
						dateFrom.Value = Session("SavingFrom")
					End If

					If Session("SavingTo") Is Nothing Then
						dateTo.Value = Today()
					Else
						dateTo.Value = Session("SavingTo")
					End If

					' remove any potential lock from the parent contract
                    cLocks.RemoveLockItem(curUser.Account.accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.EmployeeID)

					litSavingsTable.Text = GetSavingsGrid(db, Session("ActiveContract"))
				End If

                db.FWDb("R", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
				If db.FWDbFlag = True Then
                    lblTitle.Text = " - " & db.FWDbFindVal("ContractDescription", 1)
				End If

				db.DBClose()
				db = Nothing
			End If

			cmdUpdate.ImageUrl = "./buttons/update.gif"
			cmdUpdate.AlternateText = "Update"
			cmdUpdate.ToolTip = "Update all changes made to the savings grid"
			cmdUpdate.Attributes.Add("onmouseover", "window.status='Update all changes made to the savings grid';return true;")
            cmdUpdate.Attributes.Add("onmouseout", "window.status='Done';")
            cmdUpdate.Visible = True
            'cmdUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractSavings, False)

			cmdClose.AlternateText = "Close"
			cmdClose.ToolTip = "Exit savings and return to your active contract"
			cmdClose.Attributes.Add("onmouseover", "window.status='Exit savings and return to your active contract';return true;")
            cmdClose.Attributes.Add("onmouseout", "window.status='Done';")

            lnkAdd.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractSavings, False)
		End Sub

		Private Function GetSavingsGrid(ByVal db As cFWDBConnection, ByVal conId As Integer) As String
			Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
			Dim strHTML As New System.Text.StringBuilder
			Dim sql As New System.Text.StringBuilder
			Dim drow As DataRow
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)

            sql.Append("SELECT [savings].*,[contract_details].[contractCurrency] FROM [savings] " & vbNewLine)
            sql.Append("INNER JOIN [contract_details] ON [savings].[ContractId] = [contract_details].[ContractId] " & vbNewLine)
            sql.Append("WHERE [savings].[ContractId] = @conId ")
            sql.Append("ORDER BY [savings].[SavingDate] DESC")
			db.AddDBParam("conId", conId, True)
			db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

			With strHTML
				.Append("<div class=""inputpaneltitle"">Current Savings for Period</div>" & vbNewLine)
				.Append("<table class=""datatbl"">" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractSavings, True) Then
                    .Append("<th><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
                End If

                If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractSavings, True) Then
                    .Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
                End If
                .Append("<th>Reference</th>" & vbNewLine)
                .Append("<th>Saving Date</th>" & vbNewLine)
                .Append("<th>Amount Saved</th>" & vbNewLine)
                .Append("<th>Comment</th>" & vbNewLine)
                .Append("</tr>" & vbNewLine)

                If db.glNumRowsReturned > 0 Then
                    Dim rowClass As String
                    Dim rowalt As Boolean = False

                    For Each drow In db.glDBWorkA.Tables(0).Rows
                        rowalt = (rowalt Xor True)
                        If rowalt Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        .Append("<tr>" & vbNewLine)
                        If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractSavings, True) Then
                            .Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Edit the saving entry';return true;"" onmouseout=""window.status='Done';"" href=""ContractSavings.aspx?action=edit&savingid=" & drow.Item("SavingsId") & """ title=""Edit saving entry""><img src=""./icons/edit.gif"" /></a></td>" & vbNewLine)
                        End If

                        If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractSavings, True) Then
                            .Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Delete the saving entry';return true;"" onmouseout=""window.status='Done';"" onclick=""javascript:if(confirm('Click OK to confirm deletion of this saving entry')){window.location.href='ContractSavings.aspx?action=delete&savingid=" & drow.Item("SavingsId") & "';}"" title=""Delete saving entry"" style=""cursor: hand;""><img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                        End If

                        .Append("<td class=""" & rowClass & """>" & drow.Item("Reference") & "</td>" & vbNewLine)
                        Dim tmpdate As String
                        If IsDate(drow.Item("SavingDate")) = True Then
                            tmpdate = Format(drow.Item("SavingDate"), cDef.DATE_FORMAT)
                        Else
                            tmpdate = "Unspecified"
                        End If
                        .Append("<td class=""" & rowClass & """>" & tmpdate & "</td>" & vbNewLine)
                        If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                            If Not drow.Item("ContractCurrency") Is DBNull.Value Then
                                .Append("<td class=""" & rowClass & """>" & currency.FormatCurrency(drow.Item("Amount"), currency.getCurrencyById(drow.Item("ContractCurrency")), False) & "</td>" & vbNewLine)
                            Else
                                .Append("<td class=""" & rowClass & """>" & drow.Item("Amount") & "</td>" & vbNewLine)

                            End If
                        Else
                            .Append("<td class=""" & rowClass & """>N/A</td>" & vbNewLine)
                        End If

                        .Append("<td class=""" & rowClass & """><textarea readonly rows=""2"" style=""width: 150px;"">" & drow.Item("Comment") & "</textarea></td>" & vbNewLine)
                        .Append("</tr>" & vbNewLine)
                    Next
                Else
                    .Append("<tr>" & vbNewLine)
                    .Append("<td class=""row1"" colspan=""6"" align=""center"">No Savings for specified period</td>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                End If
                .Append("</table>" & vbNewLine)
            End With

			Return strHTML.ToString
		End Function

		Private Sub doUpdate()
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
				Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
				Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
				Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)
				Dim db As New cFWDBConnection
				Dim ARec As New cAuditRecord
                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractSavings, curUser.CurrentSubAccountId)
				'Dim collRE As System.Collections.SortedList

				db.DBOpen(fws, False)

				Dim updId, conId As Integer

				updId = Integer.Parse(hiddenSavingId.Text)

				'If fws.glUseRechargeFunction = True And updId > 0 Then
				'    conId = e.SheetView.Cells(updRow, SavingsCols.Contract_ID).Value
				'    collRE = GetRechargeEntityCollection(db, conId)
				'End If

                db.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")

                If updId > 0 Then
                    curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractSavings, False, True)

                    ' get the original record
                    db.FWDb("R3", "savings", "SavingsId", updId, "", "", "", "", "", "", "", "", "", "")

                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    If db.FWDb2Flag = True Then
                        conId = Integer.Parse(db.FWDbFindVal("ContractId", 2))
                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                    Else
                        conId = 0
                    End If

                    ARec.DataElementDesc = "CONTRACT SAVING"

                    Dim firstchange As Boolean = True

                    If IsDate(db.FWDbFindVal("Saving Date", 3)) Then
                        Dim tmpDateOld As String = ""
                        Dim tmpdateNew As String = ""

                        If IsDate(db.FWDbFindVal("SavingDate", 3)) = True Then
                            tmpDateOld = Format(CDate(db.FWDbFindVal("SavingDate", 3)), cDef.DATE_FORMAT)
                        End If

                        tmpdateNew = Format(CDate(dateSaving.Text), cDef.DATE_FORMAT)

                        If tmpDateOld <> tmpdateNew Then
                            db.SetFieldValue("SavingDate", dateSaving.Text, "D", firstchange)
                            firstchange = False

                            ARec.ElementDesc = "SAVING DATE"
                            If db.FWDb3Flag = True Then
                                ARec.PreVal = db.FWDbFindVal("SavingDate", 3)
                            End If
                            ARec.PostVal = dateSaving.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, updId)
                        End If
                    End If

                    If db.FWDbFindVal("Reference", 3) <> txtReference.Text Then
                        db.SetFieldValue("Reference", txtReference.Text, "S", firstchange)
                        firstchange = False

                        ARec.ElementDesc = "SAVING REFERENCE"
                        If db.FWDb3Flag = True Then
                            ARec.PreVal = db.FWDbFindVal("Reference", 3)
                        End If
                        ARec.PostVal = txtReference.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, updId)
                    End If

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                        If db.FWDbFindVal("Amount", 3) <> txtAmount.Text Then
                            ARec.ElementDesc = "SAVING AMOUNT"
                            ARec.PreVal = db.FWDbFindVal("Amount", 3)
                            ARec.PostVal = txtAmount.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, updId)


                            db.SetFieldValue("Amount", txtAmount.Text, "N", firstchange)
                            firstchange = False
                        End If
                    End If

                    If db.FWDbFindVal("Comment", 3) <> txtComment.Text Then
                        ARec.ElementDesc = "SAVING COMMENT"
                        ARec.PreVal = Mid(db.FWDbFindVal("Comment", 3), 1, cFWAuditLog.MAX_AUDITVAL_LEN)
                        ARec.PostVal = Mid(txtComment.Text, 1, cFWAuditLog.MAX_AUDITVAL_LEN)
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, updId)

                        db.SetFieldValue("Comment", txtComment.Text, "S", firstchange)
                        firstchange = False
                    End If
                    'Case SavingsCols.RechargeEntity
                    '    If updId > 0 Then
                    '        ARec.ElementDesc = "SAVING RECHARGEE"
                    '        If db.FWDb3Flag = True Then
                    '            If Val(db.FWDbFindVal("Recharge Entity Id", 3)) > 0 Then
                    '                Dim tmpIdx, idx2 As Integer
                    '                Dim tmpVal As String
                    '                Dim tmpDbId As Integer
                    '                System.Diagnostics.Debug.WriteLine("e.EditValues(col) = *" & e.EditValues(col) & "*")
                    '                System.Diagnostics.Debug.WriteLine("collRE.Count = " & Trim(Str(collRE.Count)))
                    '                System.Diagnostics.Debug.WriteLine("Output of Collection is as follows: - ")
                    '                For idx2 = 0 To collRE.Count - 1
                    '                    System.Diagnostics.Debug.WriteLine("collRE.GetKey(" & Trim(Str(idx2)) & ")=*" & collRE.GetKey(idx2) & "*")
                    '                    System.Diagnostics.Debug.WriteLine("collRE.GetByIndex(" & Trim(Str(idx2)) & ")=*" & collRE.GetByIndex(idx2) & "*")
                    '                Next
                    '                tmpDbId = Val(db.FWDbFindVal("Recharge Entity Id", 3))
                    '                System.Diagnostics.Debug.WriteLine("count = " & collRE.Count)
                    '                System.Diagnostics.Debug.WriteLine("Key of index 1 = " & collRE.GetKey(1))
                    '                tmpIdx = collRE.IndexOfKey(tmpDbId)
                    '                tmpVal = collRE.GetByIndex(tmpIdx)
                    '                ARec.PreVal = tmpVal
                    '            Else
                    '                ARec.PreVal = ""
                    '            End If
                    '        End If
                    '        'System.Diagnostics.Debug.WriteLine("e.EditValues(col)=*" & e.EditValues(col) & "*")
                    '        ARec.PostVal = e.EditValues(col)
                    '        ALog.AddAuditRec( ARec,true)
                    '        ALog.CommitAuditLog(uinfo)
                    '    End If
                    '    Dim idx As Integer
                    '    Dim newVal As Object
                    '    'System.Diagnostics.Debug.WriteLine("e.EditValues(col) = *" & e.EditValues(col) & "*")
                    '    'System.Diagnostics.Debug.WriteLine("collRE.Count = " & Trim(Str(collRE.Count)))
                    '    'System.Diagnostics.Debug.WriteLine("Output of Collection is as follows: - ")
                    '    'For idx = 0 To collRE.Count - 1
                    '    '    System.Diagnostics.Debug.WriteLine("collRE.GetKey(" & Trim(Str(idx)) & ")=*" & collRE.GetKey(idx) & "*")
                    '    '    System.Diagnostics.Debug.WriteLine("collRE.GetByIndex(" & Trim(Str(idx)) & ")=*" & collRE.GetByIndex(idx) & "*")
                    '    'Next
                    '    idx = collRE.IndexOfValue(Trim(e.EditValues(col)))
                    '    newVal = collRE.GetKey(idx)
                    '    db.SetFieldValue("Recharge Entity Id", newVal, "N", firstpass)
                    '    firstpass = False

                    If firstchange = False Then
                        ' must be updating
                        db.FWDb("A", "savings", "SavingsId", updId, "", "", "", "", "", "", "", "", "", "")
                    End If
                Else
                    curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractSavings, False, True)

                    ' must be inserting
                    db.SetFieldValue("subAccountId", curUser.CurrentSubAccountId, "N", True)
                    db.SetFieldValue("ContractId", Session("ActiveContract"), "N", False)
                    db.SetFieldValue("Reference", txtReference.Text, "S", False)
                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                        db.SetFieldValue("Amount", txtAmount.Text, "N", False)
                    End If
                    db.SetFieldValue("Comment", txtComment.Text, "S", False)
                    db.SetFieldValue("SavingDate", dateSaving.Text, "D", False)
                    db.SetFieldValue("LoggedByUserId", curUser.Employee.employeeid, "N", False)
                    db.SetFieldValue("LoggedByTimestamp", Now, "D", False)
                    db.FWDb("W", "savings", "", "", "", "", "", "", "", "", "", "", "", "")

                    ARec.Action = cFWAuditLog.AUDIT_ADD
                    If db.FWDb2Flag = True Then
                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3)
                    End If
                    ARec.DataElementDesc = "CONTRACT SAVING"
                    ARec.ElementDesc = dateSaving.Text & " | " & txtAmount.Text
                    ARec.PostVal = ""
                    ARec.PreVal = ""
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, updId)
                    End If

                    db.DBClose()
                    db = Nothing

            Catch ex As Exception

            End Try

			Response.Redirect("ContractSavings.aspx", True)
		End Sub

		Private Sub cmdRefresh_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdRefresh.Click
			Session("SavingFrom") = dateFrom.Value
			Session("SavingTo") = dateTo.Value
			Response.Redirect("ContractSavings.aspx", True)
		End Sub

		Private Sub LeaveScreen()
			Session("SavingFrom") = Nothing
			Session("SavingTo") = Nothing
			Response.Redirect("ContractSummary.aspx?id=" & Trim(Str(Session("ActiveContract"))), True)
		End Sub

		Private Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
			LeaveScreen()
		End Sub

        Private Sub EditSaving(ByVal db As cFWDBConnection, ByVal savingId As Integer, ByVal curUser As CurrentUser)
            panelEditFields.Visible = True
            panelDataTable.Visible = False
            cmdClose.Visible = False

            Master.enablenavigation = False
            Master.RefreshBreadcrumbInfo()

            db.FWDb("R", "savings", "SavingsId", savingId, "", "", "", "", "", "", "", "", "", "")
            If db.FWDbFlag Then
                If db.FWDbFindVal("SavingDate", 1).Trim <> "" Then
                    dateSaving.Value = CDate(db.FWDbFindVal("SavingDate", 1))
                Else
                    dateSaving.Text = ""
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                    txtAmount.Text = db.FWDbFindVal("Amount", 1)
                Else
                    txtAmount.Text = "0"
                    txtAmount.Enabled = False
                End If
                txtComment.Text = db.FWDbFindVal("Comment", 1)
                txtReference.Text = db.FWDbFindVal("Reference", 1)
                hiddenSavingId.Text = savingId.ToString
            Else
                Response.Redirect("ContractSavings.aspx", True)
            End If
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Response.Redirect("ContractSavings.aspx", True)
        End Sub

        Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdUpdate.Click
            doUpdate()
        End Sub

        Protected Sub lnkAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAdd.Click
            panelEditFields.Visible = True
            panelDataTable.Visible = False
            cmdClose.Visible = False

            Master.enablenavigation = False
            Master.useCloseNavigationMsg = False
            Master.RefreshBreadcrumbInfo()

            litSavingsTable.Text = ""

            txtAmount.Text = "0"
            dateSaving.Text = Now.ToShortDateString
            txtComment.Text = ""
            txtReference.Text = ""
            hiddenSavingId.Text = "0"
        End Sub
    End Class
End Namespace
