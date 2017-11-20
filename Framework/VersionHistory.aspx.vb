Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class VersionHistory
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
			Dim FWDb As New cFWDBConnection
			Dim sql As String
			Dim versionIdx As Integer
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim hasdata, isEdit As Boolean
			Dim tmpStr, action As String
			Dim ARec As New cAuditRecord
			Dim itemID As Integer

			FWDb.DBOpen(fws, False)

			If Me.IsPostBack = False Then
				lblTitle.Text = "Version History"
				tmpStr = Trim(Request.QueryString("desc"))
				Session("desc") = tmpStr

				Title = "Version History"
				Master.title = Title

                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.VersionRegistry, curUser.CurrentSubAccountId)

				If tmpStr <> "" Then
					lblTitle.Text += " - [" & tmpStr.ToString & "]"
				End If

				Session("displayType") = Request.QueryString("displaytype")

				action = Request.QueryString("action")
				Select Case LCase(action)
					Case "delete"
						Dim vId As Integer
						vId = Val(Request.QueryString("deleteid"))
						FWDb.FWDb("R", "version_history", "History Id", vId, "", "", "", "", "", "", "", "", "", "")
						FWDb.FWDb("R2", "contract_details", "Contract Id", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
						If FWDb.FWDb2Flag = True And FWDb.FWDbFlag = True Then
							ARec.Action = cFWAuditLog.AUDIT_DEL
							tmpStr = FWDb.FWDbFindVal("Contract Key", 2)
							If tmpStr <> "" Then
								ARec.ContractNumber = tmpStr
							Else
								ARec.ContractNumber = FWDb.FWDbFindVal("Contract Number", 2)
							End If
							ARec.DataElementDesc = "VERSION HISTORY"
							ARec.ElementDesc = FWDb.FWDbFindVal("Type", 1)
							ARec.PreVal = FWDb.FWDbFindVal("PlusMinusQty", 1)
							ARec.PostVal = ""
							ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, vId)

                            FWDb.FWDb("D", "version_history", "History Id", vId, "", "", "", "", "", "", "", "", "", "")
                        End If

                    Case "edit"
                        itemID = Val(Request.QueryString("editid"))
                        If itemID > 0 Then
                            isEdit = True

                            panelEditFields.Visible = True
                            Master.enablenavigation = False

                            FWDb.FWDb("R", "version_history", "History Id", itemID, "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDbFlag = True Then
                                txtDate.Value = FWDb.FWDbFindVal("Datestamp", 1)
                                txtComment.Text = FWDb.FWDbFindVal("Comment", 1)

                                ' populate the reseller's available for the active product
                                sql = "SELECT [product_suppliers].[supplierId],[supplier_details].[supplierName] FROM [product_suppliers] " & vbNewLine & _
                                "INNER JOIN [supplier_details] ON [product_suppliers].[supplierId] = [supplier_details].[supplierId] " & vbNewLine & _
                                "WHERE [product_suppliers].[ProductId] = @productId"
                                FWDb.AddDBParam("productId", Session("ActiveProduct"), True)
                                FWDb.RunSQL(sql, FWDb.glDBWorkD, False, "", False)

                                lstReseller.DataSource = FWDb.glDBWorkD
                                lstReseller.DataTextField = "supplierName"
                                lstReseller.DataValueField = "supplierId"
                                lstReseller.DataBind()

                                lstReseller.Items.Insert(0, New ListItem("Unspecified", 0))
                                Try
                                    lstReseller.Items.FindByText(FWDb.FWDbFindVal("Reseller", 1)).Selected = True
                                Catch ex As Exception

                                End Try

                            End If
                        End If

                    Case Else

                End Select

                Select Case Session("displayType")
                    Case 0
                        ' display history for all versions
                        sql = "SELECT [Registry Id],[Version],[Quantity] FROM [version_registry] " & vbNewLine & _
                        "WHERE [Contract Id] = " & Trim(Session("ActiveContract")) '& " AND [Product Id] = " & Trim(Session("ActiveProduct"))
                        FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "Versions", False)

                        sql = "SELECT [version_registry].[Registry Id],[version_history].[DateStamp],[version_history].[Changed By],[version_history].[PlusMinusQty],[version_history].[Type],[version_history].[Comment],[version_history].[Reseller] FROM [version_registry]" & vbNewLine & _
                        "INNER JOIN [version_history] ON [version_registry].[Registry Id] = [version_history].[Registry Id]" & vbNewLine & _
                        "WHERE [Contract Id] = " & Trim(Session("ActiveContract")) '& " AND [Product Id] = " & Trim(Session("ActiveProduct"))
                        FWDb.RunSQL(sql, FWDb.glDBWorkA, True, "History", False)

                        'litHistoryGrid.Text = GetAllVersionHistory(FWDb)
                        Session("vhgrid") = GetAllVersionHistory(FWDb) ' litHistoryGrid.Text

                    Case 1
                        ' display history for one version. should have another query param
                        versionIdx = Request.QueryString("id")

                        sql = "SELECT [History Id],[Registry Id],[DateStamp],[Changed By],[PlusMinusQty],[Type],[Comment],[Reseller] FROM [version_history]" & vbNewLine & _
                        "WHERE [Registry Id] = " & versionIdx.ToString
                        FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "", False)

                        If FWDb.glNumRowsReturned > 0 Then
                            hasdata = True
                            Session("vhgrid") = GetVersionHistory(FWDb)
                        End If
                End Select
            End If

            cmdOK.ToolTip = "Return to the Version Registry"
            cmdOK.AlternateText = "Close"
            cmdOK.Attributes.Add("onmouseover", "window.status='Return to the Version Registry';return true;")
            cmdOK.Attributes.Add("onmouseout", "window.status='Done';")

            If Not Session("vhgrid") Is Nothing Then
                litHistoryGrid.Text = Session("vhgrid")
            End If

            cmdUpdate.AlternateText = "Update"
            cmdUpdate.ToolTip = "Commit changes to the database"
            cmdUpdate.Attributes.Add("onmouseover", "window.status='Commit changes made to the database';return true;")
            cmdUpdate.Attributes.Add("onmouseout", "window.status='Done';")

            FWDb.DBClose()
            FWDb = Nothing
        End Sub

        Private Sub doClose()
            Dim str As String
            Session("vhgrid") = Nothing

            str = Session("VerRegURL")
            Response.Redirect(str, True)
        End Sub

        Private Function GetAllVersionHistory(ByVal db As cFWDBConnection) As String
            Dim script As New System.Text.StringBuilder
            Dim strHTML As New System.Text.StringBuilder
            Dim drow, drow1 As DataRow
            Dim curRegId As String
            Dim hasHistory, firstRow As Boolean

            ' two tiers of information
            script.Append("<script language=""javascript"" type=""text/javascript"">" & vbNewLine)
            script.Append("function expandBranch(idName)" & vbNewLine)
            script.Append("{" & vbNewLine)
            script.Append("if(document.getElementById('regid' + idName).style.display != 'block')" & vbNewLine)
            script.Append("{" & vbNewLine)
            script.Append("document.getElementById('regid' + idName).style.display = 'block';" & vbNewLine)
            script.Append("document.getElementById('img' + idName).src = './buttons/close.gif';")
            script.Append("}" & vbNewLine)
            script.Append("else" & vbNewLine)
            script.Append("{" & vbNewLine)
            script.Append("document.getElementById('regid' + idName).style.display = 'none';" & vbNewLine)
            script.Append("document.getElementById('img' + idName).src = './buttons/open.gif';")
            script.Append("}" & vbNewLine)
            script.Append("return;" & vbNewLine)
            script.Append("}" & vbNewLine)
            script.Append("</script>" & vbNewLine)

            ' "document.write('<style>.sublist { display: none }</style>');" & vbNewLine & _

            strHTML.Append(script)
            strHTML.Append("<table>" & vbNewLine)
            strHTML.Append("<tr><td width=""500"">" & vbNewLine)
            firstRow = True

            Dim rowalt As Boolean = False
            Dim rowClass As String = "row1"
            Dim rowalt_inner As Boolean = False
            Dim rowClass_inner As String = "row1"

            For Each drow In db.glDBWorkA.Tables("Versions").Rows
                rowalt = (rowalt Xor True)
                If rowalt = True Then
                    rowClass = "row1"
                Else
                    rowClass = "row2"
                End If

                ' [Registry Id],[Version],[Quantity] from [Version Registry]
                If firstRow = False Then
                    strHTML.Append("<tr><td>" & vbNewLine)
                End If

                strHTML.Append("<table class=""datatbl"">" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<th>&nbsp;</td>" & vbNewLine)
                strHTML.Append("<th>Version</th>" & vbNewLine)
                strHTML.Append("<th>Quantity</th>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """><a onclick=""javascript:expandBranch('" & Trim(drow.Item("Registry Id")) & "');""><img src=""./buttons/open.gif"" id=""img" & drow.Item("Registry Id") & """ /></a></td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("Version") & "</td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("Quantity") & "</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
                strHTML.Append("</table>" & vbNewLine)

                ' outer table break
                strHTML.Append("</td></tr>" & vbNewLine & "<tr><td>" & vbNewLine)

                strHTML.Append("<div id=""regid" & Trim(drow.Item("Registry Id")) & """ class=""sublist"">" & vbNewLine)

                curRegId = Trim(drow.Item("Registry Id"))
                hasHistory = False

                ' [Registry Id],[DateStamp],[Changed By],[PlusMinusQty],[Type],[Comment] linked from [Version history]
                strHTML.Append("<table class=""datatbl"">" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<th>Date</th>" & vbNewLine)
                strHTML.Append("<th>Changed By</th>" & vbNewLine)
                strHTML.Append("<th>Variance</th>" & vbNewLine)
                strHTML.Append("<th>Change Type</th>" & vbNewLine)
                strHTML.Append("<th>Reseller</b></th>" & vbNewLine)
                strHTML.Append("<th width=""100"">Comment</th>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)

                For Each drow1 In db.glDBWorkA.Tables("History").Rows
                    Dim tmpStr As String

                    rowalt_inner = (rowalt_inner Xor True)
                    If rowalt_inner = True Then
                        rowClass_inner = "row1"
                    Else
                        rowClass_inner = "row2"
                    End If

                    If drow1.Item("Registry Id") = curRegId Then
                        hasHistory = True
                        strHTML.Append("<tr>" & vbNewLine)
                        If IsDBNull(drow1.Item("Datestamp")) = True Then
                            strHTML.Append("<td class=""" & rowClass_inner & """>n/a</td>" & vbNewLine)
                        Else
                            strHTML.Append("<td class=""" & rowClass_inner & """>" & Format(CDate(drow1.Item("Datestamp")), cDef.DATE_FORMAT) & "</td>" & vbNewLine)
                        End If

                        strHTML.Append("<td class=""" & rowClass_inner & """>" & drow1.Item("Changed By") & "</td>" & vbNewLine)
                        strHTML.Append("<td class=""" & rowClass_inner & """>" & drow1.Item("PlusMinusQty") & "</td>" & vbNewLine)
                        strHTML.Append("<td class=""" & rowClass_inner & """>" & drow1.Item("Type") & "</td>" & vbNewLine)
                        If IsDBNull(drow1.Item("Reseller")) = True Then
                            tmpStr = "n/a"
                        Else
                            tmpStr = drow1.Item("Reseller")
                        End If
                        strHTML.Append("<td class=""" & rowClass_inner & """>" & tmpStr & "</td>" & vbNewLine)
                        strHTML.Append("<td class=""" & rowClass_inner & """>" & drow1.Item("Comment") & "</td>" & vbNewLine)
                        strHTML.Append("</tr>" & vbNewLine)
                    End If
                Next

                If hasHistory = False Then
                    strHTML.Append("<tr><td class=""row1"" align=""center"" colspan=""6"">No history returned for this version</td></tr>" & vbNewLine)
                End If
                strHTML.Append("</table>" & vbNewLine)

                strHTML.Append("</div>" & vbNewLine)
                strHTML.Append("</td></tr>" & vbNewLine)

                ' outer table row break
                firstRow = False
                'strHTML.Append("</td></tr>" & vbNewLine)
            Next

            strHTML.Append("</table>" & vbNewLine)

            Return strHTML.ToString
        End Function

        Private Function GetVersionHistory(ByVal db As cFWDBConnection) As String
            Dim tmpStr As String
            Dim drow As DataRow
            Dim total As Integer
            Dim strHTML As New System.Text.StringBuilder
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            total = 0

            ' [History Id],[Registry Id],[DateStamp],[Changed By],[PlusMinusQty],[Type],[Comment]
            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
            strHTML.Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
            strHTML.Append("<th>Date</th>" & vbNewLine)
            strHTML.Append("<th>Changed by</th>" & vbNewLine)
            strHTML.Append("<th>Variance</th>" & vbNewLine)
            strHTML.Append("<th>Change Type</th>" & vbNewLine)
            strHTML.Append("<th>Reseller Used</th>" & vbNewLine)
            strHTML.Append("<th>Comment</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            Dim rowalt As Boolean = False
            Dim rowClass As String = "row1"

            For Each drow In db.glDBWorkA.Tables(0).Rows
                rowalt = (rowalt Xor True)
                If rowalt = True Then
                    rowClass = "row1"
                Else
                    rowClass = "row2"
                End If

                strHTML.Append("<tr>" & vbNewLine)
                If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.VersionRegistry, False) Then
                    strHTML.Append("<td class=""" & rowClass & """><a href=""VersionHistory.aspx?action=edit&editid=" & drow.Item("History Id") & "&desc=" & Session("desc") & "&displaytype=" & Session("displayType") & "&id=" & drow.Item("Registry Id") & """><img src=""./icons/edit.gif"" /></a></td>" & vbNewLine)
                Else
                    strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                End If

                If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.VersionRegistry, False) Then
                    strHTML.Append("<td class=""" & rowClass & """><a onclick=""javascript:if(confirm('Click OK to confirm deletion of history record.')){window.location.href='VersionHistory.aspx?action=delete&deleteid=" & drow.Item("History Id") & "&desc=" & Session("desc") & "&displaytype=" & Session("displayType") & "&id=" & drow.Item("Registry Id") & "';}""><img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                Else
                    strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                End If

                If IsDBNull(drow.Item("Datestamp")) = True Then
                    strHTML.Append("<td class=""" & rowClass & """>n/a</td>" & vbNewLine)
                Else
                    strHTML.Append("<td class=""" & rowClass & """>" & Format(CDate(drow.Item("Datestamp")), cDef.DATE_FORMAT) & "</td>" & vbNewLine)
                End If

                strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("Changed By") & "</td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("PlusMinusQty") & "</td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("Type") & "</td>" & vbNewLine)
                If IsDBNull(drow.Item("Reseller")) = True Then
                    tmpStr = "n/a"
                Else
                    tmpStr = drow.Item("Reseller")
                End If
                strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """ width=""100"">" & drow.Item("Comment") & "</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
                total = total + Val(drow.Item("PlusMinusQty"))
            Next

            If rowClass = "row2" Then
                rowClass = "row1"
            Else
                rowClass = "row2"
            End If

            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<td class=""" & rowClass & """ colspan=""4"" align=""right"">Total:&nbsp;</td>" & vbNewLine)
            strHTML.Append("<td class=""" & rowClass & """>" & total.ToString & "</td>" & vbNewLine)
            strHTML.Append("<td class=""" & rowClass & """ colspan=""3"">&nbsp;</td>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)
            strHTML.Append("</table>" & vbNewLine)

            GetVersionHistory = strHTML.ToString
        End Function

        Private Sub doHTMLUpdate()
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ARec As New cAuditRecord
            Dim firstupdate As Boolean
            Dim editID As Integer
            Dim tmpStr As String

            db.DBOpen(fws, False)

            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.VersionRegistry, curUser.CurrentSubAccountId)

            ARec.Action = cFWAuditLog.AUDIT_UPDATE
            ARec.ContractNumber = Trim(Mid(Request.QueryString("desc"), 1, 50))
            ARec.DataElementDesc = "VERSION HISTORY"

            firstupdate = True
            editID = Val(Request.QueryString("editid"))

            db.FWDb("R2", "version_history", "History Id", editID, "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag = True Then
                tmpStr = Trim(db.FWDbFindVal("Datestamp", 2))
                If tmpStr <> "" Then
                    Format(CDate(db.FWDbFindVal("Datestamp", 2)), cDef.DATE_FORMAT)
                End If

                If txtDate.Text <> tmpStr Then
                    ' date has been updated
                    db.SetFieldValue("Datestamp", txtDate.Text, "D", firstupdate)
                    firstupdate = False

                    ARec.ElementDesc = "DATE"

                    ARec.PreVal = tmpStr
                    ARec.PostVal = txtDate.Text
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, editID)
                End If

                If txtComment.Text <> Trim(db.FWDbFindVal("Comment", 2)) Then
                    ' comment field has been modified
                    db.SetFieldValue("Comment", txtComment.Text, "S", firstupdate)
                    firstupdate = False

                    ARec.ElementDesc = "COMMENT"
                    ARec.PreVal = Mid(db.FWDbFindVal("Comment", 2), 1, 60)
                    ARec.PostVal = Mid(txtComment.Text, 1, 60)
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, editID)
                End If

                If Trim(lstReseller.SelectedItem.Text) <> Trim(db.FWDbFindVal("Reseller", 2)) Then
                    ' reseller selection has changed
                    db.SetFieldValue("Reseller", lstReseller.SelectedItem.Text, "S", firstupdate)
                    firstupdate = False

                    ARec.ElementDesc = "RESELLER"
                    ARec.PreVal = Trim(db.FWDbFindVal("Reseller", 2))
                    ARec.PostVal = Trim(lstReseller.SelectedItem.Text)
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, editID)
				End If

				If firstupdate = False Then
					' changes must have been made
					db.FWDb("A", "version_history", "History Id", editID, "", "", "", "", "", "" < "", "", "", "", "")
				End If
			End If
			db.DBClose()
			db = Nothing

			Dim tmpURL As String
			tmpURL = "VersionHistory.aspx?id=" & Trim(Request.QueryString("id")) & "&desc=" & Session("desc") & "&displaytype=" & Session("displayType")
			Response.Redirect(tmpURL, True)
		End Sub

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdUpdate.Click
            doHTMLUpdate()
        End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            doClose()
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Dim tmpURL As String
            tmpURL = "VersionHistory.aspx?id=" & Trim(Request.QueryString("id")) & "&desc=" & Session("desc") & "&displaytype=" & Session("displayType")
            Response.Redirect(tmpURL, True)
        End Sub
    End Class

End Namespace
