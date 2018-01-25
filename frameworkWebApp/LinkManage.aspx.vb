Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class LinkManage
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
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            Dim filterId, contractId As Integer
            Dim sql, strHTML, action, confirmStr As String
            Dim ARec As New cAuditRecord

            action = Request.QueryString("action")
            filterId = CInt(Request.QueryString("fid"))
            contractId = Request.QueryString("cid")
            If contractId > 0 Then
                hiddenContractId.Text = contractId.ToString
                ViewState("ActiveContract") = contractId
            Else
                hiddenContractId.Text = ""
            End If

            If action <> "print" Then
                cmdClose.ToolTip = "Exit the Link Management screen"
                cmdClose.AlternateText = "Close"
                cmdClose.Attributes.Add("onmouseover", "window.status='Exit the Link Management screen';return true;")
                cmdClose.Attributes.Add("onmouseout", "window.status='Done';")

                cmdOK.AlternateText = "Update"
                cmdOK.ToolTip = "Save Link Definition for the current contract"
                cmdOK.Attributes.Add("onmouseover", "window.status='Save Link Definition for the current contract';return true;")
                cmdOK.Attributes.Add("onmouseout", "window.status='Done';")
                cmdOK.Visible = False
            End If

            If Me.IsPostBack = False Then
                db.DBOpen(fws, False)

                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractLinks, curUser.CurrentSubAccountId)

                lblNewDefinition.Text = "New Link Definition: "
                lblNewDefinition.Enabled = False
                txtNewDefinition.Enabled = False

                Select Case action
                    Case "add"

                        If filterId <= 0 Then
                            ' adding a new link defintion for the selected contract
                            lstLinkDefs.Enabled = False
                            cmdDeleteDef.Visible = False
                            lblNewDefinition.Enabled = True
                            txtNewDefinition.Enabled = True
                            Page.SetFocus(txtNewDefinition)
                        End If

                        If filterId > 0 Then
                            db.FWDb("R", "link_matrix", "LinkId", filterId, "ContractId", contractId, "", "", "", "", "", "", "", "")
                            If db.FWDbFlag = False Then
                                db.SetFieldValue("LinkId", filterId, "N", True)
                                db.SetFieldValue("ContractId", contractId, "N", False)
                                db.SetFieldValue("IsArchived", 0, "N", False)
                                db.FWDb("WX", "link_matrix", "", "", "", "", "", "", "", "", "", "", "", "")

                                ARec.Action = cFWAuditLog.AUDIT_ADD
                                db.FWDb("R2", "contract_details", "ContractId", contractId, "", "", "", "", "", "", "", "", "", "")
                                If db.FWDb2Flag = True Then
                                    If db.FWDbFindVal("ContractKey", 2) <> "" Then
                                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                                    Else
                                        ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 2)
                                    End If
                                Else
                                    ARec.ContractNumber = "n/a"
                                End If
                                ARec.DataElementDesc = "CONTRACT LINK"
                                ARec.ElementDesc = "NEW LINK"
                                db.FWDb("R2", "link_definitions", "LinkId", filterId, "", "", "", "", "", "", "", "", "", "")
                                If db.FWDb2Flag = True Then
                                    ARec.PreVal = ""
                                    ARec.PostVal = "Link to:" & db.FWDbFindVal("LinkDescription", 2)
                                Else
                                    ARec.PreVal = ""
                                    ARec.PostVal = "Link to Unknown!"
                                End If
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, filterId)
                            Else
                                ' link of contract to selected grouping already exists, don't create another
                            End If
                        End If

                    Case "delete"
                        curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractLinks, False, True)

                        If filterId > 0 And contractId > 0 Then
                            db.FWDb("R2", "contract_details", "ContractId", contractId, "", "", "", "", "", "", "", "", "", "")
                            db.FWDb("R3", "link_definitions", "LinkId", filterId, "", "", "", "", "", "", "", "", "", "")
                            ARec.Action = cFWAuditLog.AUDIT_DEL
                            ARec.DataElementDesc = "CONTRACT LINK"
                            ARec.ElementDesc = "REMOVE LINK"
                            If db.FWDb2Flag = True Then
                                If db.FWDbFindVal("ContractKey", 2) <> "" Then
                                    ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                                Else
                                    ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 2)
                                End If
                            Else
                                ARec.ContractNumber = ""
                            End If

                            If db.FWDb3Flag = True Then
                                ARec.PreVal = db.FWDbFindVal("LinkDescription", 3)
                                ARec.PostVal = ""
                            Else
                                ARec.PreVal = "Unknown!"
                                ARec.PostVal = ""
                            End If
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, filterId)

                            db.FWDb("D", "link_matrix", "LinkId", filterId, "ContractId", contractId, "", "", "", "", "", "", "", "")
                        End If

                    Case "deletedef"
                        curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractLinks, False, True)

                        filterId = Request.QueryString("fid")
                        If filterId > 0 Then
                            DeleteAllForDefinition(filterId)
                        End If

                    Case Else

                End Select

                Title = "Link Management"
                Master.title = Title

                lblTitle.Text = "Link Management"

                ' populate filter ddlist with any known link definitions
                sql = "SELECT DISTINCT [LinkId],[LinkDefinition] FROM [link_definitions] "
                sql += "WHERE [IsScheduleLink] = 0 AND [subAccountId] = @Location_Id "
                sql += "ORDER BY [LinkDefinition]"
                db.AddDBParam("Location Id", curUser.CurrentSubAccountId, True)
                db.RunSQL(sql, db.glDBWorkA, False, "", False)

                lstLinkDefs.DataSource = db.glDBWorkA
                lstLinkDefs.DataTextField = "LinkDefinition"
                lstLinkDefs.DataValueField = "LinkId"
                lstLinkDefs.DataBind()

                lstLinkDefs.Items.Insert(0, New ListItem("[None]", 0))

                If filterId > 0 Then
                    'lstLinkDefs.SelectedIndex = lstLinkDefs.Items.IndexOf(lstLinkDefs.Items.FindByValue(Str(filterId)))
                    lstLinkDefs.Items(lstLinkDefs.SelectedIndex).Selected = False
                    lstLinkDefs.Items.FindByValue(Trim(Str(filterId))).Selected = True
                End If

                If action = "print" Then
                    lstLinkDefs.Visible = False
                    lblLinkDef.Visible = False
                End If

                strHTML = GetLinkedContracts(db, filterId, lstLinkDefs.SelectedItem.Text)
                litLinkData.Text = strHTML

                db.DBClose()
            End If

            If filterId <= 0 And action = "add" Then
                cmdOK.Visible = True
            End If

            cmdDeleteDef.Visible = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractLinks, False)

            If filterId > 0 And action <> "print" Then
                If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractLinks, False) = True Then
                    confirmStr = "javascript:if(confirm('Deletion of definition will lose all underlying links. OK to confirm?')){window.location.href='LinkManage.aspx?action=deletedef&fid=" & Trim(lstLinkDefs.SelectedValue) & "';}"
                    cmdDeleteDef.Attributes.Add("onclick", confirmStr)
                    cmdDeleteDef.Attributes.Add("onmouseover", "window.status='Delete the selected definition and all underlying links';return true;")
                    cmdDeleteDef.Attributes.Add("onmouseout", "window.status='Done';")
                End If
            End If

            db = Nothing
        End Sub

        Private Function GetLinkedContracts(ByVal db As cFWDBConnection, ByVal fId As Integer, ByVal linkDef As String) As String
            Dim tmpStr As String
            Dim strHTML As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim sql As New System.Text.StringBuilder
            Dim hasData As Boolean = False
            Dim rowalt As Boolean = False
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            hasData = False

            If fId <> -1 Then
                lblTitle.Text = " - " & linkDef.Trim
            Else
                lblTitle.Text = ""
            End If

            sql.Append("SELECT [link_matrix].[LinkId],[link_matrix].[ContractId],[contract_details].[supplierId],[contract_details].[ContractDescription],[contract_details].[ContractKey],[contract_details].[Archived],[supplier_details].[supplierName] FROM [link_matrix] ")
            sql.Append("LEFT OUTER JOIN [contract_details] ON [link_matrix].[ContractId] = [contract_details].[ContractId] ")
            sql.Append("LEFT OUTER JOIN [supplier_details] ON [contract_details].[supplierId] = [supplier_details].[supplierId] ")
            sql.Append("WHERE [LinkId] = @lnkId")
            db.AddDBParam("lnkId", fId, True)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th style=""width:30px""><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
            strHTML.Append("<th>Contract Key</th>" & vbNewLine)
            strHTML.Append("<th>Contract Description</th>" & vbNewLine)
            strHTML.Append("<th>Supplier Name</th>" & vbNewLine)
            strHTML.Append("<th>Archived?</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            Dim rowClass As String
            For Each drow In db.glDBWorkA.Tables(0).Rows
                rowalt = (rowalt Xor True)

                strHTML.Append("<tr>" & vbNewLine)
                If rowalt = True Then
                    rowClass = "row1"
                Else
                    rowClass = "row2"
                End If
                If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractLinks, False) Then
                    strHTML.Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Delete this contract link association with the active definition';return true;"" onmouseout=""window.status='Done';"" onclick=""javascript:if(confirm('Are you sure you wish to delete the contract link? OK to confirm.')){window.location.href='LinkManage.aspx?action=delete&fid=" & Trim(drow.Item("LinkId")) & "&cid=" & Trim(drow.Item("ContractId")) & "';}""><img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                Else
                    strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                End If

                If IsDBNull(drow.Item("ContractKey")) = True Then
                    tmpStr = "&nbsp;"
                Else
                    tmpStr = "<a onmouseover=""window.status='Transfer to Contract record';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?id=" & Trim(drow.Item("ContractId")) & """>" & Trim(drow.Item("ContractKey")) & "</a>"
                End If
                strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Transfer to Contract record';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Trim(drow.Item("ContractId")) & """>" & Trim(drow.Item("ContractDescription")) & "</a></td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Transfer to Supplier definition record';return true;"" onmouseout=""window.status='Done';"" href=""/shared/supplier_details.aspx?sid=" & Trim(drow.Item("supplierId")) & """>" & Trim(drow.Item("supplierName")) & "</a></td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """>" & IIf(drow.Item("Archived") = "N", "No", "Yes") & "</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
                hasData = True
            Next

            If hasData = False Then
                strHTML.Append("<tr><td class=""row1"" align=""center"" colspan=""5"">No linked contracts to display</td></tr>" & vbNewLine)
            End If
            strHTML.Append("</table>" & vbNewLine)

            GetLinkedContracts = strHTML.ToString
        End Function

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            ExitLinkManage()
        End Sub

        Private Sub ExitLinkManage()
            Select Case Request.QueryString("ret")
                Case "0"
                    Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & ViewState("ActiveContract"), True)
                Case "1"
                    Response.Redirect(Trim(Request.QueryString("returl")), True)
                Case "2"
                    Response.Redirect("Home.aspx", True)
                Case Else
                    Response.Redirect("Home.aspx", True)
            End Select
        End Sub

        Protected Sub lstLinkDefs_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As cAccountSubAccounts = New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            db.DBOpen(fws, False)

            litLinkData.Text = GetLinkedContracts(db, lstLinkDefs.SelectedValue, lstLinkDefs.SelectedItem.Text)
            'Response.Redirect("LinkManage.aspx?ret=" & Trim(Request.QueryString("ret")) & "&fid=" & Trim(lstLinkDefs.SelectedValue), True)

            db.DBClose()
            db = Nothing
        End Sub

        Private Sub DeleteAllForDefinition(ByVal id As Integer)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            Dim ARec As New cAuditRecord

            db.DBOpen(fws, False)

            db.FWDb("R2", "link_definitions", "LinkId", id, "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag = True Then
                ARec.PreVal = db.FWDbFindVal("LinkDefinition", 2)
            Else
                ARec.PreVal = "Unknown"
            End If

            ' remove all links in the matrix for given id and the defintion itself
            db.FWDb("D", "link_matrix", "LinkId", id, "", "", "", "", "", "", "", "", "", "")
            db.FWDb("D", "link_definitions", "LinkId", id, "", "", "", "", "", "", "", "", "", "")

            ARec.Action = cFWAuditLog.AUDIT_DEL
            ARec.ContractNumber = ""
            ARec.DataElementDesc = "LINK DEFINITION"
            ARec.ElementDesc = "DEF+ALL LINKS"
            ARec.PostVal = ""
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractLinks, curUser.CurrentSubAccountId)
            ALog.AddAuditRec(ARec, True)
            ALog.CommitAuditLog(curUser.Employee, id)

            db.DBClose()
            db = Nothing
            Response.Redirect("LinkManage.aspx?fid=0&ret=2", True)
        End Sub

        Private Sub SaveLink()
            Dim db As New cFWDBConnection
            Dim tmpId As Integer
            Dim ARec As New cAuditRecord
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            If Trim(txtNewDefinition.Text) <> "" Then
                db.DBOpen(fws, False)

                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractLinks, curUser.CurrentSubAccountId)

                ' check that a definition of the supplied name doesn't already exist
                db.FWDb("R2", "link_definitions", "linkDefinition", Trim(txtNewDefinition.Text), "", "", "", "", "", "", "", "", "", "")
                If db.FWDb2Flag = False Then
                    If Trim(hiddenContractId.Text) <> "" Then
                        ' must be a link to an existing contract
                        db.SetFieldValue("linkDefinition", txtNewDefinition.Text.Trim, "S", True)
                        db.SetFieldValue("subAccountId", curUser.CurrentSubAccountId, "N", False)
                        db.SetFieldValue("isScheduleLink", 0, "N", False)
                        db.FWDb("W", "link_definitions", "", "", "", "", "", "", "", "", "", "", "", "")
                        tmpId = db.glIdentity
                        If tmpId > 0 Then
                            db.SetFieldValue("linkId", tmpId, "N", True)
                            db.SetFieldValue("contractId", hiddenContractId.Text.Trim, "N", False)
                            db.SetFieldValue("isArchived", 0, "N", False)
                            db.FWDb("WX", "link_matrix", "", "", "", "", "", "", "", "", "", "", "", "")

                            db.FWDb("R2", "contract_details", "contractId", hiddenContractId.Text, "", "", "", "", "", "", "", "", "", "")
                            If db.FWDb2Flag = True Then
                                If db.FWDbFindVal("ContractKey", 2) <> "" Then
                                    ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                                Else
                                    ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 2)
                                End If
                            End If
                            ARec.Action = cFWAuditLog.AUDIT_ADD
                            ARec.DataElementDesc = "LINK DEFINITION"
                            ARec.ElementDesc = "DEFINITION+LINK"
                            ARec.PreVal = ""
                            ARec.PostVal = Trim(txtNewDefinition.Text)
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, tmpId)
                        End If
                    Else
                        ' must be just adding a definition
                        db.SetFieldValue("LinkDefinition", txtNewDefinition.Text.Trim, "S", True)
                        db.SetFieldValue("IsScheduleLink", 0, "N", False)
                        db.FWDb("W", "link_definitions", "", "", "", "", "", "", "", "", "", "", "", "")
                        ARec.Action = cFWAuditLog.AUDIT_ADD
                        ARec.ContractNumber = ""
                        ARec.DataElementDesc = "LINK DEFINITION"
                        ARec.ElementDesc = "DEFINITION ONLY"
                        ARec.PreVal = ""
                        If txtNewDefinition.Text.Trim.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                            ARec.PostVal = txtNewDefinition.Text.Trim.Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                        Else
                            ARec.PostVal = txtNewDefinition.Text.Trim
                        End If

                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, tmpId)
                    End If
                Else
                    lblErrorStatus.Text = "ERROR! Definition of this name already exists. Cannot duplicate."
                    db.DBClose()
                    Exit Sub
                End If

                db.DBClose()
            Else
                lblErrorStatus.Text = "ERROR! Definition cannot be blank"
            End If

            db = Nothing
            ExitLinkManage()
        End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            SaveLink()
        End Sub
    End Class

End Namespace
