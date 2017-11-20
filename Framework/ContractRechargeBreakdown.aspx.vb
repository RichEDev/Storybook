Imports System.Collections.Generic
Imports System.Web.Services
Imports SpendManagementLibrary
Imports Spend_Management
Imports FWClasses
Imports FarPoint.Web

Namespace Framework2006
    Partial Class ContractRechargeBreakdown
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
			Dim action As String
			Dim sql As New System.Text.StringBuilder
			Dim strTableContent As New System.Text.StringBuilder
			Dim strHTML As New System.Text.StringBuilder
			Dim numConProds As Integer
			Dim db As New cFWDBConnection
			Dim conId As Integer
			Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

			ViewState("OOCReturn") = Nothing

			Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
			Dim rs As cRechargeSetting = rscoll.getSettings

			conId = Val(Request.QueryString("id"))
			Session("ActiveContract") = conId

			db.DBOpen(fws, False)
			action = Request.QueryString("action")

			If Me.IsPostBack = False Then
				Title = "Recharge Template"
				Master.title = Title

				Dim employees As New cEmployees(curUser.Account.accountid)

				Select Case action
					Case "delete"
						Dim delID As Integer
						delID = Request.QueryString("raid")

                        db.FWDb("D", "recharge_associations", "RechargeId", delID, "", "", "", "", "", "", "", "", "", "")
                        Dim rechargeColl As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, conId, cAccounts.getConnectionString(curUser.Account.accountid), params)
						rechargeColl.DeleteRechargeItem(rechargeColl.GetRechargeItemById(delID))

						Dim newaction As String = Request.QueryString("ret")
						If Not newaction Is Nothing Then
							action = newaction
						End If

						'Case "callback"
						'    Dim tmpCPId As Integer

						'    tmpCPId = Request.QueryString("cpid")
						'    strHTML = GetRechargeItem(db, tmpCPId)
						'    Response.Write(strHTML.ToString)
						'    Response.Flush()
						'    Response.End()
						'    Exit Sub

					Case Else

				End Select

				If action <> "bulk" Then
					lblAddClient.Text = "Add New Recharge " & rs.ReferenceAs

                    sql.Append("SELECT COUNT(*) AS [ConProds] FROM [contract_productdetails] WHERE [ContractId] = @ConId")
					If cDef.DBVERSION >= 18 Then
                        sql.Append(" AND [ArchiveStatus] = 0")
					End If
					db.AddDBParam("ConId", conId, True)
					db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

					numConProds = db.GetFieldValue(db.glDBWorkA, "ConProds", 0, 0)

					If numConProds > 0 Then
						' populate ddlist with unallocated recharge clients
						PopulateClientList()

						If lstEntityList.Items.Count <= 1 Then
							cmdAddClient.Enabled = False
						End If
					End If

					SetTabOptions(db,curUser)
				End If
			End If

			Select Case action
				Case "bulk"
					RTFilterPanel.Visible = False
					cmdClose.Visible = False

					Dim editList As String
					editList = Request.QueryString("bu")
					ViewState("RTAction") = "bulk"

					ShowTemplate(db, editList)

				Case Else
					If action = "showtemplate" Then
						Dim tmpCPId As Integer
						ViewState("RTAction") = "show"

						RTFilterPanel.Visible = False

						tmpCPId = Request.QueryString("cpid")
						ShowTemplate(db, tmpCPId)

						cmdClose.Visible = False
						lnkOnOffLine.Visible = False
					Else
						ViewState("RTAction") = "normal"

						If Not Cache("RCT_" & Session("ActiveContract")) Is Nothing Then
							litReturnCount.Text = "<img alt=""Alert"" src=""./images/information.gif"" />&nbsp; Recharge Templates are still being loaded. Please wait...Press F5 to refresh."
							cmdRefresh.Visible = False
						Else
							BuildTemplateHTML(db)
						End If

						cmdTemplateUpdate.Visible = False
						cmdTemplateCancel.Visible = False
					End If
			End Select

			db.DBClose()
			db = Nothing

			If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.RechargeAssociations, False) = True Then
				lnkBulkEdit.ToolTip = "Amend multiple checked items"
				lnkBulkEdit.Attributes.Add("onmouseover", "window.status='Amend multiple checked items';return true;")
				lnkBulkEdit.Attributes.Add("onmouseout", "window.status='Done';")

				lnkOneOff.ToolTip = "Create a one-off charge entry for any selected recharge items"
				lnkOneOff.Attributes.Add("onmouseover", "window.status='Create a one-off charge entry for any selected recharge items';return true;")
				lnkOneOff.Attributes.Add("onmouseout", "window.status='Done';")
			End If

			If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.RechargeAssociations, False) = True Then
				lnkGenerate.Text = "Generate Recharge"
				lnkGenerate.ToolTip = "Generate current recharge figures for a specified date period."
				lnkGenerate.Attributes.Add("onmouseover", "window.status='Generate current recharge figures for a specified date period.';return true;")
				lnkGenerate.Attributes.Add("onmouseout", "window.status='Done';")
			End If

			If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.RechargeAssociations, False) = True Then
				lnkAddClient.Text = "Add " & rs.ReferenceAs
				lnkAddClient.ToolTip = "Add another " & rs.ReferenceAs & " for recharge"
				lnkAddClient.Attributes.Add("onmouseover", "window.status='Add another " & rs.ReferenceAs & " for recharge';return true;")
				lnkAddClient.Attributes.Add("onmouseout", "window.status='Done';")

				If action = "edit" Or action = "bulk" Then
					lnkAddClient.Visible = False
					cmdAddClient.Visible = False
					lstEntityList.Enabled = False
					lnkBulkEdit.Visible = False
					lnkGenerate.Visible = False
					lnkOneOff.Visible = False
				End If
			End If

			If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.RechargePayments, False) = True Then
				cmdGenUpdate.AlternateText = "Update Generate"
				cmdGenUpdate.ToolTip = "Generate recharge amounts"
				cmdGenUpdate.Attributes.Add("onmouseover", "window.status='Generate the recharge amounts for specified dates';return true;")
				cmdGenUpdate.Attributes.Add("onmouseout", "window.status='Done';")
			End If
		End Sub

		Private Sub doAddClient()
			If Not lstEntityList.SelectedItem Is Nothing Then
				If lstEntityList.SelectedValue > 0 Then
                    ' add the selected client to the recharge list for this contractProductId
                    Dim ARec As New cAuditRecord
                    Dim sql As New System.Text.StringBuilder
                    Dim db As New cFWDBConnection
                    Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
                    Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                    Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                    Dim employees As New cEmployees(curUser.Account.accountid)
                    Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
                    Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
                    Dim rs As cRechargeSetting = rscoll.getSettings

                    If lstEntityList.SelectedIndex = 0 Then
                        ' can't perform action as nothing selected
                        litMessage.Text = "<span class=""errormessage"">Cannot perform update. Items requiring customer adding must be selected</span>"
                        Exit Sub
                    End If

                    db.DBOpen(fws, False)

                    Dim ALog As New cFWAuditLog(fws, SpendManagementElement.RechargeAssociations, curUser.CurrentSubAccountId)

                    ' this only retrieves the current recharges assigned, need to check those ConProds that do not have anything assigned to them at the moment!
                    Dim rechargeColl As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), params)

                    Dim retURL As String = "ContractRechargeBreakdown.aspx?id=" & CStr(Session("ActiveContract"))
                    Dim prods As New cProducts(curUser.Account.accountid, curUser.CurrentSubAccountId)

                    Select Case ViewState("RTAction")
                        Case "show"
                            ' add customer to current template only
                            Dim curCPId As Integer = Request.QueryString("cpid")

                            If curCPId > 0 Then
                                db.FWDb("R3", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                                If db.FWDb3Flag Then
                                    ARec.Action = cFWAuditLog.AUDIT_ADD
                                    If IsDBNull(db.FWDbFindVal("ContractKey", 3)) = False Then
                                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3)
                                    Else
                                        ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 3)
                                    End If
                                    ARec.DataElementDesc = "RECHARGE " & rs.ReferenceAs.ToUpper
                                    ARec.PreVal = ""
                                    ARec.PostVal = lstEntityList.SelectedItem.Text
                                End If

                                db.FWDb("R2", "contract_productdetails", "ContractProductId", curCPId, "", "", "", "", "", "", "", "", "", "")

                                Dim curProd As cProduct = prods.GetProductById(Integer.Parse(db.FWDbFindVal("ProductId", 2)))
                                ARec.ElementDesc = curProd.ProductName

                                db.SetFieldValue("ContractProductId", curCPId, "N", True)
                                db.SetFieldValue("RechargeEntityId", lstEntityList.SelectedValue, "N", False)
                                db.FWDb("W", "recharge_associations", "", "", "", "", "", "", "", "", "", "", "", "")

                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, curCPId)

                                Dim newclient As New cRecharge(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
                                With newclient
                                    .ApportionType = RechargeApportionType.Percentage
                                    .ContractProductId = curCPId
                                    If db.FWDb2Flag Then
                                        If db.FWDbFindVal("CurrencyId", 0) <> "" Then
                                            .CurrencyId = Integer.Parse(db.FWDbFindVal("CurrencyId", 2))
                                        Else
                                            .CurrencyId = 0
                                        End If
                                    Else
                                        .CurrencyId = 0
                                    End If
                                    If db.FWDbFindVal("MaintenanceValue", 2) <> "" Then
                                        .Maintenance = Double.Parse(db.FWDbFindVal("MaintenanceValue", 2))
                                    Else
                                        .Maintenance = 0
                                    End If
                                    .Portion = 0
                                    .PostWarrantyApportionType = RechargeApportionType.Percentage
                                    .PostWarrantyPortion = 0
                                    .ProductId = curProd.ProductId
                                    .ProductName = curProd.ProductName
                                    .RechargeEntityId = lstEntityList.SelectedItem.Value
                                    .RechargeEntityName = lstEntityList.SelectedItem.Text
                                    .RechargeId = db.glIdentity
                                End With

                                rechargeColl.AddRechargeItem(newclient)
                            End If

                            retURL += "&action=showtemplate&cpid=" & curCPId.ToString

                        Case Else
                            db.FWDb("R3", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                            If db.FWDb3Flag Then
                                ARec.Action = cFWAuditLog.AUDIT_ADD
                                If IsDBNull(db.FWDbFindVal("ContractKey", 3)) = False Then
                                    ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3)
                                Else
                                    ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 3)
                                End If
                                ARec.DataElementDesc = "RECHARGE " & rs.ReferenceAs.ToUpper
                            End If

                            sql.Append("SELECT *,productDetails.[ProductName] FROM contract_productdetails ")
                            sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[ProductId] ")
                            sql.Append("WHERE [ContractId] = @conId")
                            db.AddDBParam("conId", Session("ActiveContract"), True)
                            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

                            Dim drow As DataRow

                            For Each drow In db.glDBWorkA.Tables(0).Rows
                                'rItem = rechargeColl.GetRechargeItemByIndex(rEnum)

                                Dim tmpCHK As CheckBox = RTData.FindControl("CHK" & CStr(drow("ContractProductId")))
                                If Not tmpCHK Is Nothing Then
                                    If tmpCHK.Checked Then
                                        db.SetFieldValue("ContractProductId", drow("ContractProductId"), "N", True)
                                        db.SetFieldValue("RechargeEntityId", lstEntityList.SelectedItem.Value, "N", False)
                                        db.FWDb("W", "recharge_associations", "", "", "", "", "", "", "", "", "", "", "", "")

                                        ARec.ElementDesc = drow("ProductName")
                                        ARec.PreVal = ""
                                        ARec.PostVal = lstEntityList.SelectedItem.Text
                                        ALog.AddAuditRec(ARec, True)
                                        ALog.CommitAuditLog(curUser.Employee, drow("ContractProductId"))

                                        Dim newclient As New cRecharge(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID))
                                        Dim curProd As cProduct = prods.GetProductById(drow("ProductId"))

                                        With newclient
                                            .ApportionType = RechargeApportionType.Percentage
                                            .ContractProductId = drow("ContractProductId")
                                            If db.FWDb2Flag Then
                                                If db.FWDbFindVal("CurrencyId", 0) <> "" Then
                                                    .CurrencyId = Integer.Parse(db.FWDbFindVal("CurrencyId", 2))
                                                Else
                                                    .CurrencyId = 0
                                                End If
                                            Else
                                                .CurrencyId = 0
                                            End If
                                            If db.FWDbFindVal("MaintenanceValue", 2) <> "" Then
                                                .Maintenance = Double.Parse(db.FWDbFindVal("MaintenanceValue", 2))
                                            Else
                                                .Maintenance = 0
                                            End If
                                            .Portion = 0
                                            .PostWarrantyApportionType = RechargeApportionType.Percentage
                                            .PostWarrantyPortion = 0
                                            .ProductId = curProd.ProductId
                                            .ProductName = curProd.ProductName
                                            .RechargeEntityId = lstEntityList.SelectedItem.Value
                                            .RechargeEntityName = lstEntityList.SelectedItem.Text
                                            .RechargeId = db.glIdentity
                                        End With

                                        rechargeColl.AddRechargeItem(newclient)
                                    End If
                                End If
                            Next
                    End Select

                    db.DBClose()
                    db = Nothing

                    Response.Redirect(retURL, True)
                End If
            End If
        End Sub

        Private Sub doGenerate(Optional ByVal All As Boolean = False)
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            Dim rechargeItems As New cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties)
            Dim comma As String = ""
            Dim resList As New StringBuilder

            ViewState("GenList") = Nothing

            If Not All Then
                ' get a list of checked items - if none checked, then generate for all.
                For i As Integer = 0 To rechargeItems.Count - 1
                    Dim rItem As cRecharge = rechargeItems.GetRechargeItemByIndex(i)
                    Dim tmpCHK As CheckBox = RTData.FindControl("CHK" & rItem.ContractProductId.ToString)
                    If Not tmpCHK Is Nothing Then
                        If tmpCHK.Checked Then
                            resList.Append(comma)
                            resList.Append(rItem.RechargeId.ToString)
                            comma = ","
                        End If
                    End If
                Next

                If resList.Length = 0 Then
                    litGenMessage.Text = "ERROR! No templates were selected for payment generation."
                    cmdGenUpdate.Visible = False
                Else
                    ViewState("GenList") = resList.ToString
                End If
            End If

            ' hide the main stuff
            lnkAddClient.Visible = False
            lnkBulkEdit.Visible = False
            lnkOneOff.Visible = False
            RTDisplayPanel.Visible = False
            RTFilterPanel.Visible = False

            ' show the generation fields
            RTGeneratePanel.Visible = True
        End Sub

        Private Sub GenOK()
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim genStart, genEnd As String
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim employees As New cEmployees(curUser.Account.accountid)

            genStart = txtStartDate.Text.Trim
            genEnd = txtEndDate.Text.Trim

            Dim rechargeItems As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), params)
            Dim btasks As New cBackgroundTasks(curUser)
            Dim updateList As String = ""
            Dim items() As String
            Dim hasItemList As Boolean = False

            If Not ViewState("GenList") Is Nothing Then
                updateList = ViewState("GenList")
                items = updateList.Split(",")

                If updateList.Trim = "" Then
                    litMessage.Text = "<div class=""inputpanel"">ERROR! No contract products were selected to generate charges for.</div>"
                    Exit Sub
                End If

                hasItemList = True
            End If

            Dim dateGenStart As Date = CDate(genStart)
            Dim dateGenEnd As Date = CDate(genEnd)

            ' generate payments as dictated by the grid for periods specified at the frequency indicated in RechargeSettings
            Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
            Dim rs As cRechargeSetting = rscoll.getSettings

            Dim rcPeriodMths As Integer

            Select Case rs.RechargePeriod
                Case 0 ' monthly
                    rcPeriodMths = 1
                Case 1 ' quarterly
                    rcPeriodMths = 3
                Case 2 ' 6 monthly
                    rcPeriodMths = 6
            End Select

            Dim curPeriodDate As Date
            Dim sql As New System.Text.StringBuilder

            curPeriodDate = CDate("01/" & dateGenStart.Month.ToString & "/" & dateGenStart.Year.ToString & " 00:00:00")

            If hasItemList Then
                ' generate for tagged items only
                Cache("RCG_" & Session("ActiveContract")) = 1

                Dim syncres As IAsyncResult
                Dim rg As New cBackgroundTasks.GeneratePayments(AddressOf btasks.GenerateRechargePayments)
                syncres = rg.BeginInvoke(Session("ActiveContract"), items, curPeriodDate, dateGenEnd, rcPeriodMths, New AsyncCallback(AddressOf CallBackMethod), rg)
            Else
                ' generate for all items
                Cache("RCG_" & Session("ActiveContract")) = 1

                Dim syncres As IAsyncResult
                Dim rg As New cBackgroundTasks.GeneratePayments(AddressOf btasks.GenerateRechargePayments)

                syncres = rg.BeginInvoke(Session("ActiveContract"), rechargeItems.GetIdList, curPeriodDate, dateGenEnd, rcPeriodMths, New AsyncCallback(AddressOf CallBackMethod), rg)
            End If

            ViewState("GenList") = Nothing

            ' hide the main stuff
            lnkAddClient.Visible = True
            lnkBulkEdit.Visible = True
            lnkOneOff.Visible = True
            RTDisplayPanel.Visible = True
            RTFilterPanel.Visible = True

            ' show the generation fields
            RTGeneratePanel.Visible = False

            litMessage.Text = "<img alt=""Alert!"" src=""./images/information.gif"" />&nbsp;Payment Generation successfully started as as a background task"
            'Response.Redirect("ContractRechargeData.aspx?id=" & Session("ActiveContract"), True)
            'Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Trim(Session("ActiveContract")), True)
        End Sub

        Private Sub CallBackMethod(ByVal result As IAsyncResult)
            Dim rg As cBackgroundTasks.GeneratePayments = CType(result.AsyncState, cBackgroundTasks.GeneratePayments)
            rg.EndInvoke(result)

            If Not Cache("RCG_" & Session("ActiveContract")) Is Nothing Then
                Cache.Remove("RCG_" & Session("ActiveContract"))
            End If
        End Sub

        Private Sub lnkGenerate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkGenerate.Click
            doGenerate()
        End Sub

        Private Sub lnkAddClient_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkAddClient.Click
            doAddClient()
        End Sub

        Private Sub PopulateClientList()
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim clientList As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
            'Dim slClients As SortedList = clientList.GetClientsByLocation(curUser.currentUser.userInfo.ActiveLocation)

            lstEntityList.Items.AddRange(clientList.GetListControlItems(True, True))
            'For i As Integer = 0 To slClients.Count - 1
            '	Dim client As cRechargeClient = CType(slClients.GetByIndex(i), cRechargeClient)
            '	lstEntityList.Items.Add(New ListItem(client.ClientName, client.EntityId))
            'Next

            'lstEntityList.Items.Insert(0, New ListItem("[None]", 0))
        End Sub

        Private Sub BuildTemplateHTML(ByVal db As cFWDBConnection)
            Try
                Dim strHTML As New System.Text.StringBuilder
                Dim sql As New System.Text.StringBuilder
                Dim cCOLL As cCPFieldInfo
                Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
                Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
                Dim employees As New cEmployees(curUser.Account.accountid)

                Dim rechargeItems As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), params)

                cCOLL = New cCPFieldInfo(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid), curUser.Employee.employeeid, Session("ActiveContract"))

                If rechargeItems.Count < cDef.MAX_CP_DISPLAY Then
                    ' not many entities, don't display filter box
                    RTFilterPanel.Visible = False
                ElseIf Me.IsPostBack = False Then
                    If Session("CRB_Filter") Is Nothing Then
                        Session("CRB_Filter") = ""
                    Else
                        txtFilter.Text = Session("CRB_Filter")
                    End If
                End If

                sql = New System.Text.StringBuilder
                sql.Append("SELECT ")
                If rechargeItems.Count > cDef.MAX_CP_DISPLAY And Session("CRB_Filter") = "" Then
                    sql.Append("TOP " & cDef.MAX_CP_DISPLAY.ToString & " ")
                End If

                sql.Append("[contractProductId],[productDetails].[ProductName]")

                If cDef.DBVERSION >= 18 Then
                    If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                        sql.Append(", [UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim & "]")
                    End If

                    If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                        sql.Append(", [UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim & "]")
                    End If
                End If

                sql.Append(", dbo.GetUnrecoveredRecharge(CONVERT(datetime,CAST(DATEPART(year,getdate()) AS nvarchar) + '-' + CAST(DATEPART(month,getdate()) AS nvarchar) + '-01',120),[contract_productdetails].[contractProductId]) AS [Unrecovered]")
                sql.Append(", dbo.GetTemplateClientIdList([contractProductId]) AS [ClientIdList]")
                sql.Append(" FROM [contract_productdetails]")
                sql.Append(" LEFT JOIN [productDetails] ON [contract_productdetails].[productId] = [productDetails].[ProductId]")
                sql.Append(" WHERE [Archive Status] = 0 AND [contract_productdetails].[contractId] = " & Session("ActiveContract"))
                If Session("CRB_Filter") <> "" And Session("CRB_Filter") <> "*" Then
                    sql.Append(" AND (")
                    sql.Append("[ProductName] LIKE '%" & Session("CRB_Filter") & "%'")
                    If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                        Select Case cCOLL.CPFieldInfoItem.CP_UF1_Type
                            Case UserFieldType.DateField
                                If IsDate(Session("CRB_Filter")) Then
                                    Dim dt As DateTime = CType(Session("CRB_Filter"), DateTime)
                                    Dim tmpDateStr As String = dt.Year.ToString & "-" & dt.Month.ToString & "-" & dt.Day.ToString
                                    sql.Append(" OR [UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim & "] = CONVERT(datetime,'" & tmpDateStr & "',120)")
                                End If
                            Case UserFieldType.Float, UserFieldType.Number, UserFieldType.Checkbox
                                If IsNumeric(Session("CRB_Filter")) Then
                                    sql.Append(" OR [UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim & "] = " & Session("CRB_Filter"))
                                End If
                            Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref

                            Case Else
                                sql.Append(" OR [UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim & "] LIKE '%" & Session("CRB_Filter") & "%'")
                        End Select
                    End If
                    If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                        Select Case cCOLL.CPFieldInfoItem.CP_UF2_Type
                            Case UserFieldType.DateField
                                If IsDate(Session("CRB_Filter")) Then
                                    Dim dt As DateTime = CType(Session("CRB_Filter"), DateTime)
                                    Dim tmpDateStr As String = dt.Year.ToString & "-" & dt.Month.ToString & "-" & dt.Day.ToString
                                    sql.Append(" OR [UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim & "] = CONVERT(datetime,'" & tmpDateStr & "',120)")
                                End If
                            Case UserFieldType.Float, UserFieldType.Number, UserFieldType.Checkbox
                                If IsNumeric(Session("CRB_Filter")) Then
                                    sql.Append(" OR [UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim & "] = " & Session("CRB_Filter"))
                                End If
                            Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref

                            Case Else
                                sql.Append(" OR [UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim & "] LIKE '%" & Session("CRB_Filter") & "%'")
                        End Select


                    End If
                    sql.Append(")")
                End If
                sql.Append(" ORDER BY")

                Dim comma As String = ""

                If cDef.DBVERSION >= 18 Then
                    If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                        Select Case cCOLL.CPFieldInfoItem.CP_UF1_Type
                            Case UserFieldType.Text
                            Case Else
                                sql.Append(" [UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim & "]")
                                comma = ","
                        End Select
                    End If

                    If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                        Select Case cCOLL.CPFieldInfoItem.CP_UF1_Type
                            Case UserFieldType.Text
                            Case Else
                                sql.Append(comma & " [UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim & "]")
                                comma = ","
                        End Select
                    End If
                End If
                sql.Append(comma & " [ProductName]")

                db.RunSQL(sql.ToString, db.glDBWorkL, False, "", False)

                Dim strCount As New StringBuilder
                strCount.Append("<table class=""datatbl""><tr><th>")
                strCount.Append(db.glNumRowsReturned.ToString & " Items returned for display")
                strCount.Append("</th></tr></table>" & vbNewLine)

                litReturnCount.Text = strCount.ToString

                Dim rowClass As String = "row1"
                Dim rowalt As Boolean = False

                If rechargeItems.Count > cDef.MAX_CP_DISPLAY And Session("CRB_Filter") = "" Then
                    strHTML.Append("<div class=""inputpaneltitle"">** TOP " & cDef.MAX_CP_DISPLAY.ToString & " displayed only - use Filter or '*' to display all</div>" & vbNewLine)
                End If

                ' create ASP table and controls
                CreateRTData(db.glDBWorkL, cCOLL)

                If rechargeItems.Count = 0 Then
                    lnkOnOffLine.Visible = False
                End If

            Catch ex As Exception
                Dim errStr As New System.Text.StringBuilder
                Dim litError As New Literal

                With errStr
                    .Append("<table class=""datatbl"">" & vbNewLine)
                    .Append("<tr>" & vbNewLine)
                    .Append("<th>An error has occurred created Contract Recharge Templates</th>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                    .Append("<tr>" & vbNewLine)
                    .Append("<td class=""row1"">")
                    .Append(ex.Message)
                    .Append("</td>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                    .Append("</table>" & vbNewLine)
                End With

                litError.Text = errStr.ToString
                RTData.Controls.Add(litError)
            End Try
        End Sub

        Private Sub lnkBulkEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkBulkEdit.Click
            doBulkEdit()
        End Sub

        Private Sub doBulkEdit()
            Try
                Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
                Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim rechargeColl As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties)
                Dim resList As String = ""

                Dim i As Integer
                Dim comma As String = ""

                For i = 0 To rechargeColl.Count - 1
                    Dim ritem As cRecharge = rechargeColl.GetRechargeItemByIndex(i)
                    Dim tmpCHK As CheckBox = RTData.FindControl("CHK" & ritem.ContractProductId.ToString)
                    If Not tmpCHK Is Nothing Then
                        If tmpCHK.Checked Then
                            resList += comma & ritem.RechargeId.ToString
                            comma = ","
                        End If
                    End If
                Next

                If resList.Length > 0 And comma <> "" Then
                    Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract") & "&bu=" & resList.Trim & "&action=bulk", True)
                Else
                    litMessage.Text = "<table class=""datatbl""><tr><th>ERROR</th></tr><tr><td class=""row1"">No items selected for bulk edit</td></tr></table>"
                End If

            Catch ex As Exception
                litMessage.Text = "<table class=""datatbl""><tr><th>ERROR</th></tr><tr><td class=""row1"">" & ex.Message & "</td></tr></table>"
            End Try
        End Sub

        Private Sub lnkOneOff_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkOneOff.Click
            ViewState("OOCReturn") = 1
            Response.Redirect("admin/oneoffrechargecosts.aspx", True)
        End Sub

        Private Sub CreateOneOffCharge()
            Try
                ' get a list of RA checkboxes selected to apply a one-off charge
                Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
                Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim rechargeColl As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), fws.getConnectionString, subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties)
                Dim resList As String = ""

                Dim i As Integer
                Dim comma As String = ""

                For i = 0 To rechargeColl.Count - 1
                    Dim ritem As cRecharge = rechargeColl.GetRechargeItemByIndex(i)
                    Dim tmpCHK As CheckBox = RTData.FindControl("CHK" & ritem.ContractProductId.ToString)
                    If Not tmpCHK Is Nothing Then
                        If tmpCHK.Checked Then
                            resList += comma & ritem.RechargeId.ToString
                            comma = ","
                        End If
                    End If
                Next

                If resList.Trim.Length > 0 Then
                    ' must be items selected for a one-off charge
                    Response.Redirect("ContractRechargeData.aspx?action=ooc&raidlist=" & resList.Trim & "&id=" & Session("ActiveContract"), True)
                End If

            Catch ex As Exception

            End Try
        End Sub

        Private Sub cmdRefresh_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdRefresh.Click
            Session("CRB_Filter") = txtFilter.Text.Trim
            Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub cmdGenUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdGenUpdate.Click
            GenOK()
        End Sub

#Region "Navigation Clicks"
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

        Protected Sub lnkRPnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkRPnav.Click
            Response.Redirect("ContractRechargeData.aspx?id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub lnkRTnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkRTnav.Click
            Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract"), True)
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
                    If Session("ActiveContract") > 0 Then
                        db.FWDb("R2", "contract_details", "contractId", Session("ActiveContract"), "Location Id", curUser.CurrentSubAccountId, "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag = True Then
                            ' contract was found previously
                            Dim Sql As New System.Text.StringBuilder
                            Sql.Append("SELECT COUNT(*) AS [LnkCnt] FROM [link_matrix] ")
                            Sql.Append("INNER JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] ")
                            Sql.Append("WHERE [link_definitions].[IsScheduleLink] = 0 AND [contractId] = @cId")
                            db.AddDBParam("cId", Session("ActiveContract"), True)
                            db.RunSQL(Sql.ToString, db.glDBWorkC, False, "", False)

                            If db.GetFieldValue(db.glDBWorkC, "LnkCnt", 0, 0) > 0 Then
                                ' links with the contract exist, so display the tab and separator image
                                lnkLCnav.Visible = True
                                lnkLCnav.ToolTip = "Display contracts linked via definitions"
                                lnkLCnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkLCnav.ToolTip))
                                lnkLCnav.Attributes.Add("onmouseout", omout)
                            End If
                        End If
                    End If
                End If

                If params.EnableRecharge And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAssociations, False) Then
                    Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
                    Dim rs As cRechargeSetting = rscoll.getSettings

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargePayments, False) Then
                        lnkRPnav.Visible = True
                        lnkRPnav.ToolTip = "View recharge payments defined against contract products"
                        lnkRPnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkRPnav.ToolTip))
                        lnkRPnav.Attributes.Add("onmouseout", omout)
                    End If

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAssociations, False) = True Then
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
#End Region

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            Dim retURL As String = "Home.aspx"

            Select Case ViewState("RTAction")
                Case "normal"
                    retURL = "Home.aspx"

                Case "show", "bulk"
                    retURL = "ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract")
            End Select
            ViewState("RTAction") = Nothing
            Response.Redirect(retURL, True)
        End Sub

        Private Sub CreateRTData(ByVal dset As DataSet, ByVal cCOLL As cCPFieldInfo)
            Dim RT_table As New Table
            Dim drow As DataRow
            Dim rowalt As Boolean = False
            Dim rowClass As String = "row1"
            Dim cellText As String = ""
            Dim RTtablerow(dset.Tables(0).Rows.Count - 1) As TableRow
            Dim rowCount As Integer = 0
            Dim extraCellCount As Integer = 0
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim rsettings As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
            Dim rsetting As cRechargeSetting = rsettings.getSettings
            Dim rclients As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))

            If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                extraCellCount += 1
            End If
            If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                extraCellCount += 1
            End If

            Dim RTtableCell(4 + extraCellCount) As TableCell

            RT_table.CssClass = "datatbl"

            Dim RTtableheader As New TableHeaderRow
            Dim RTtableheadercell As New TableHeaderCell
            RTtableheadercell.Text = ""
            RTtableheader.Cells.Add(RTtableheadercell)

            RTtableheadercell = New TableHeaderCell
            Dim img As New System.Web.UI.WebControls.Image
            img.ImageUrl = "./icons/16/plain/view.gif"
            RTtableheadercell.Controls.Add(img)
            RTtableheader.Cells.Add(RTtableheadercell)

            RTtableheadercell = New TableHeaderCell
            RTtableheadercell.Text = "Product Name"
            RTtableheader.Cells.Add(RTtableheadercell)

            If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                RTtableheadercell = New TableHeaderCell
                RTtableheadercell.Text = cCOLL.CPFieldInfoItem.CP_UF1_Title
                RTtableheader.Cells.Add(RTtableheadercell)
            End If

            If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                RTtableheadercell = New TableHeaderCell
                RTtableheadercell.Text = cCOLL.CPFieldInfoItem.CP_UF2_Title
                RTtableheader.Cells.Add(RTtableheadercell)
            End If

            RTtableheadercell = New TableHeaderCell
            RTtableheadercell.Text = "No. " & rsetting.ReferenceAs
            RTtableheader.Cells.Add(RTtableheadercell)

            RTtableheadercell = New TableHeaderCell
            RTtableheadercell.Text = params.RechargeUnrecoveredTitle
            RTtableheader.Cells.Add(RTtableheadercell)

            RT_table.Rows.Add(RTtableheader)

            Dim chk As Boolean = False
            If Request.QueryString("chk") = "1" Then
                chk = True
            End If

            For Each drow In dset.Tables(0).Rows
                rowalt = (rowalt Xor True)
                If rowalt Then
                    rowClass = "row1"
                Else
                    rowClass = "row2"
                End If

                RTtableCell(0) = New TableCell
                RTtableCell(0).CssClass = rowClass
                Dim chkCPSelect As New CheckBox
                chkCPSelect.ID = "CHK" & CStr(drow.Item("contractProductId"))
                chkCPSelect.Checked = chk
                RTtableCell(0).Controls.Add(chkCPSelect)

                RTtableCell(1) = New TableCell
                RTtableCell(1).CssClass = rowClass
                Dim imgCallCP As New Literal
                imgCallCP.Text = "<a href=""ContractRechargeBreakdown.aspx?action=showtemplate&id=" & Session("ActiveContract") & "&cpid=" & CStr(drow.Item("contractProductId")).Trim & """><img src=""./icons/16/plain/view.gif"" alt=""View""></a>"
                RTtableCell(1).Controls.Add(imgCallCP)

                RTtableCell(2) = New TableCell
                RTtableCell(2).CssClass = rowClass
                RTtableCell(2).Text = drow.Item("ProductName")

                Dim curCell As Integer = 3
                If cDef.DBVERSION >= 18 Then
                    If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                        Try
                            RTtableCell(curCell) = New TableCell
                            RTtableCell(curCell).CssClass = rowClass

                            If IsDBNull(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim)) = False Then
                                Select Case cCOLL.CPFieldInfoItem.CP_UF1_Type
                                    Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref
                                        RTtableCell(curCell).Text = cCOLL.CPFieldInfoItem.CP_UF1_Coll(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim))
                                    Case UserFieldType.DateField
                                        If IsDate(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim)) Then
                                            RTtableCell(curCell).Text = CDate(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim)).ToShortDateString
                                        Else
                                            RTtableCell(curCell).Text = ""
                                        End If
                                    Case UserFieldType.Float, UserFieldType.Number
                                        RTtableCell(curCell).Text = CStr(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim))
                                    Case UserFieldType.Checkbox
                                        Dim chkUF As New CheckBox
                                        chkUF.ID = "chkUF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString
                                        chkUF.Enabled = False
                                        chkUF.Checked = IIf(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim) = 1, True, False)
                                        RTtableCell(curCell).Controls.Add(chkUF)
                                    Case Else
                                        RTtableCell(curCell).Text = drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim)
                                End Select
                            End If
                            curCell += 1

                        Catch ex As Exception
                            ' uf selected may not be listed (due to grouping etc.)
                        End Try
                    End If

                    If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                        Try
                            RTtableCell(curCell) = New TableCell
                            RTtableCell(curCell).CssClass = rowClass

                            If IsDBNull(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim)) = False Then
                                Select Case cCOLL.CPFieldInfoItem.CP_UF2_Type
                                    Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref
                                        RTtableCell(curCell).Text = cCOLL.CPFieldInfoItem.CP_UF2_Coll(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim))
                                    Case UserFieldType.DateField
                                        If IsDate(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim)) Then
                                            RTtableCell(curCell).Text = CDate(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim)).ToShortDateString
                                        Else
                                            RTtableCell(curCell).Text = ""
                                        End If
                                    Case UserFieldType.Float, UserFieldType.Number
                                        RTtableCell(curCell).Text = CStr(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim))
                                    Case UserFieldType.Checkbox
                                        Dim chkUF As New CheckBox
                                        chkUF.ID = "chkUF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString
                                        chkUF.Enabled = False
                                        chkUF.Checked = IIf(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim) = 1, True, False)
                                        RTtableCell(curCell).Controls.Add(chkUF)
                                    Case Else
                                        RTtableCell(curCell).Text = drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim)
                                End Select
                            End If
                            curCell += 1

                        Catch ex As Exception
                            ' uf selected may not be listed (due to grouping etc.)
                        End Try
                    End If
                End If

                ' display number of customers on a template. Tooltip displays list
                RTtableCell(curCell) = New TableCell
                RTtableCell(curCell).CssClass = rowClass

                Dim csvClients As String = drow("ClientIdList")
                If csvClients.Trim() <> "" Then
                    Dim clients() As String = csvClients.Split(",")
                    RTtableCell(curCell).Text = clients.Length.ToString & " " & rsetting.ReferenceAs

                    Dim tooltip As New StringBuilder
                    Dim comma As String = ""
                    tooltip.Append(rsetting.ReferenceAs & ":" & vbNewLine)
                    For x As Integer = 0 To clients.Length - 1
                        Dim rclient As cRechargeClient = rclients.GetClientById(Integer.Parse(clients(x)))
                        If Not rclient Is Nothing Then
                            tooltip.Append(comma)
                            tooltip.Append(rclient.ClientName)
                            comma = "," & vbNewLine
                        End If
                    Next

                    RTtableCell(curCell).ToolTip = tooltip.ToString
                Else
                    RTtableCell(curCell).Text = "0 " & rsetting.ReferenceAs
                End If
                curCell += 1

                RTtableCell(curCell) = New TableCell
                If drow("Unrecovered") < 0 Or drow("Unrecovered") > 100 Then
                    RTtableCell(curCell).CssClass = rowClass & " badrecovery"
                Else
                    RTtableCell(curCell).CssClass = rowClass
                End If

                RTtableCell(curCell).Text = Format(drow("Unrecovered"), "##0.##")

                RTtablerow(rowCount) = New TableRow
                RTtablerow(rowCount).Cells.AddRange(RTtableCell)
                rowCount += 1
            Next

            If rowCount > 0 Then
                lnkSelectAll.Visible = True
                lnkDeselectAll.Visible = True
            End If

            RT_table.Rows.AddRange(RTtablerow)
            RTData.Controls.Add(RT_table)
        End Sub

        Private Function GetTableHeaderRow(ByVal rs As cRechargeSetting, ByVal includeProduct As Boolean) As TableHeaderRow
            ' compile table headers
            Dim offset As Integer
            Dim RTheaderrow As TableHeaderRow

            If includeProduct Then
                offset = 1
            Else
                offset = 0
            End If

            Dim RTheadercell(10 + offset) As TableHeaderCell

            RTheadercell(0) = New TableHeaderCell
            Dim viewimg As New System.Web.UI.WebControls.Image
            viewimg.ImageUrl = "./icons/16/plain/view.gif"
            viewimg.AlternateText = "Delete"
            RTheadercell(0).Controls.Add(viewimg)

            RTheadercell(1) = New TableHeaderCell
            Dim img As New System.Web.UI.WebControls.Image
            img.ImageUrl = "./icons/delete2.gif"
            img.AlternateText = "Delete"
            RTheadercell(1).Controls.Add(img)

            If includeProduct Then
                RTheadercell(2) = New TableHeaderCell
                RTheadercell(2).Text = "Product Name"
                RTheadercell(2).Width = Unit.Pixel(130)
            End If

            RTheadercell(2 + offset) = New TableHeaderCell
            RTheadercell(2 + offset).Text = rs.ReferenceAs
            RTheadercell(2 + offset).Width = Unit.Pixel(130)

            RTheadercell(3 + offset) = New TableHeaderCell
            RTheadercell(3 + offset).Text = "Annual £"
            RTheadercell(3 + offset).Width = Unit.Pixel(100)

            RTheadercell(4 + offset) = New TableHeaderCell
            RTheadercell(4 + offset).Text = "Apportion By"
            'RTheadercell(3).Width = Unit.Pixel(80)

            RTheadercell(5 + offset) = New TableHeaderCell
            RTheadercell(5 + offset).Text = "Portion"
            'RTheadercell(4).Width = Unit.Pixel(80)

            RTheadercell(6 + offset) = New TableHeaderCell
            RTheadercell(6 + offset).Text = "PW Apportion"
            'RTheadercell(5).Width = Unit.Pixel(80)

            RTheadercell(7 + offset) = New TableHeaderCell
            RTheadercell(7 + offset).Text = "PW Portion"
            'RTheadercell(6).Width = Unit.Pixel(80)

            RTheadercell(8 + offset) = New TableHeaderCell
            RTheadercell(8 + offset).Text = "Start Date"
            'RTheadercell(7).Width = Unit.Pixel(80)

            RTheadercell(9 + offset) = New TableHeaderCell
            RTheadercell(9 + offset).Text = "WE Date"
            'RTheadercell(8).Width = Unit.Pixel(80)

            RTheadercell(10 + offset) = New TableHeaderCell
            RTheadercell(10 + offset).Text = "End Date"
            'RTheadercell(9).Width = Unit.Pixel(80)

            RTheaderrow = New TableHeaderRow
            RTheaderrow.Cells.AddRange(RTheadercell)

            Return RTheaderrow
        End Function

        Public Overloads Sub ShowTemplate(ByVal db As cFWDBConnection, ByVal cpList As String)
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
            Dim rs As cRechargeSetting = rscoll.getSettings

            Dim rechargeItems As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), params)
            Dim arrItems As ArrayList = rechargeItems.GetRechargeItemsByIdList(cpList)

            ViewState("EditTemplates") = arrItems

            Dim RT_table As New Table
            RT_table.CssClass = "datatbl"
            RT_table.ID = "templatetable"

            RT_table.Rows.Add(GetTableHeaderRow(rs, True))

            Dim rowcount As Integer = 0
            Dim rowClass As String = "row1"
            Dim rowalt As Boolean = False

            If arrItems.Count > 0 Then
                Dim RTtablerow(arrItems.Count - 1) As TableRow
                Dim enumItems As System.Collections.IEnumerator = arrItems.GetEnumerator

                While enumItems.MoveNext
                    rowalt = (rowalt Xor True)
                    If rowalt Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    Dim curItem As cRecharge = CType(enumItems.Current, cRecharge)
                    Dim curCurrency As cCurrency = currency.getCurrencyById(curItem.CurrencyId)
                    Dim tableCell(10) As TableCell

                    ' delete icon
                    tableCell(0) = New TableCell
                    tableCell(0).CssClass = rowClass
                    If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.RechargeAssociations, False) Then
                        Dim delIcon As New Literal
                        With delIcon
                            Dim strHTML As New StringBuilder
                            strHTML.Append("<a onclick=""javascript:if(confirm('Click OK to confirm deletion')){window.location.href='ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract") & "&ret=bulk&bu=" & cpList & "&action=delete&raid=" & curItem.RechargeId.ToString & "&cpid=" & curItem.ContractProductId.ToString & "';}"" ")
                            strHTML.Append("onmouseover=""window.status='Delete recharge item';return true;"" ")
                            strHTML.Append("onmouseout=""window.status='Done';"">")
                            strHTML.Append("<img alt=""Delete item"" src=""./icons/delete2.gif"">")
                            strHTML.Append("</a>")
                            delIcon.Text = strHTML.ToString
                        End With
                        tableCell(0).Controls.Add(delIcon)
                    End If

                    ' contract product name
                    tableCell(1) = New TableCell
                    tableCell(1).Text = curItem.ProductName
                    tableCell(1).CssClass = rowClass
                    tableCell(1).Width = Unit.Pixel(100)
                    tableCell(1).HorizontalAlign = HorizontalAlign.Center

                    ' customer
                    tableCell(2) = New TableCell
                    tableCell(2).Text = curItem.RechargeEntityName
                    tableCell(2).CssClass = rowClass
                    tableCell(2).Width = Unit.Pixel(100)
                    tableCell(2).HorizontalAlign = HorizontalAlign.Center

                    ' maint value
                    tableCell(3) = New TableCell
                    tableCell(3).HorizontalAlign = HorizontalAlign.Right
                    tableCell(3).CssClass = rowClass
                    tableCell(3).Text = currency.FormatCurrency(curItem.Maintenance, curCurrency, False)

                    ' apportion type
                    tableCell(4) = New TableCell
                    tableCell(4).Controls.Add(curItem.CreateApportionTypeListControl("APP" & curItem.RechargeId.ToString, curItem.ApportionType))
                    'tableCell(4).Text = curItem.GetApportionTypeDesc
                    tableCell(4).CssClass = rowClass
                    tableCell(4).HorizontalAlign = HorizontalAlign.Center

                    ' portion
                    tableCell(5) = New TableCell
                    Dim txtPortionControl As New TextBox
                    txtPortionControl.ID = "POR" & curItem.RechargeId.ToString
                    txtPortionControl.Text = curItem.Portion.ToString
                    txtPortionControl.Width = Unit.Pixel(70)
                    tableCell(5).CssClass = rowClass
                    tableCell(5).Controls.Add(txtPortionControl)
                    Dim valPortion As New CompareValidator
                    With valPortion
                        .ID = "valPortion" & curItem.RechargeId.ToString
                        .ControlToValidate = "POR" & curItem.RechargeId.ToString
                        .SetFocusOnError = True
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .ErrorMessage = "Numeric values only"
                        .Type = ValidationDataType.Double
                        .Text = "**"
                    End With
                    tableCell(5).Controls.Add(valPortion)
                    Dim valexPortion As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexPortion
                        .ID = "valexPortion" & curItem.RechargeId.ToString
                        .TargetControlID = "valPortion" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexPortion)

                    ' PW apportion type
                    tableCell(6) = New TableCell
                    tableCell(6).Controls.Add(curItem.CreateApportionTypeListControl("PW_APP" & curItem.RechargeId.ToString, curItem.PostWarrantyApportionType))
                    tableCell(6).CssClass = rowClass
                    tableCell(6).HorizontalAlign = HorizontalAlign.Center

                    ' PW portion
                    tableCell(7) = New TableCell
                    Dim txtPWPortion As New TextBox
                    txtPWPortion.ID = "PW_POR" & curItem.RechargeId.ToString
                    txtPWPortion.Text = curItem.PostWarrantyPortion.ToString
                    txtPWPortion.Width = Unit.Pixel(70)
                    tableCell(7).Controls.Add(txtPWPortion)
                    tableCell(7).CssClass = rowClass
                    Dim valPWPortion As New CompareValidator
                    With valPWPortion
                        .ID = "valPWPortion" & curItem.RechargeId.ToString
                        .ControlToValidate = "PW_POR" & curItem.RechargeId.ToString
                        .SetFocusOnError = True
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .ErrorMessage = "Numeric values only"
                        .Type = ValidationDataType.Double
                        .Text = "**"
                    End With
                    tableCell(7).Controls.Add(valPWPortion)
                    Dim valexPWPortion As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexPWPortion
                        .ID = "valexPWPortion" & curItem.RechargeId.ToString
                        .TargetControlID = "valPWPortion" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexPWPortion)

                    ' start date
                    tableCell(8) = New TableCell
                    Dim dateSSD As New TextBox
                    With dateSSD
                        .ID = "SSD" & curItem.RechargeId.ToString
                        If IsDate(curItem.SupportStartDate) And curItem.SupportStartDate.Year > 1900 Then
                            dateSSD.Text = Format(curItem.SupportStartDate, cDef.DATE_FORMAT)
                        Else
                            dateSSD.Text = ""
                        End If
                        .Width = Unit.Pixel(70)
                    End With
                    tableCell(8).CssClass = rowClass
                    tableCell(8).Controls.Add(dateSSD)

                    Dim valSSD As New CompareValidator
                    With valSSD
                        .ID = "valSSD" & curItem.RechargeId.ToString
                        .ControlToValidate = "SSD" & curItem.RechargeId.ToString
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .Type = ValidationDataType.Date
                        .Text = "**"
                        .ErrorMessage = "Incorrect date format entered"
                    End With
                    tableCell(8).Controls.Add(valSSD)

                    Dim valexSSD As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexSSD
                        .ID = "valexSSD" & curItem.RechargeId.ToString
                        .TargetControlID = "valSSD" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexSSD)

                    ' pw date
                    tableCell(9) = New TableCell
                    Dim dateWED As New TextBox
                    dateWED.ID = "WED" & curItem.RechargeId.ToString
                    If IsDate(curItem.WarrantyEndDate) And curItem.WarrantyEndDate.Year > 1900 Then
                        dateWED.Text = Format(curItem.WarrantyEndDate, cDef.DATE_FORMAT)
                    Else
                        dateWED.Text = ""
                    End If
                    dateWED.Width = Unit.Pixel(70)
                    tableCell(9).CssClass = rowClass
                    tableCell(9).Controls.Add(dateWED)

                    Dim valWED As New CompareValidator
                    With valWED
                        .ID = "valWED" & curItem.RechargeId.ToString
                        .ControlToValidate = "WED" & curItem.RechargeId.ToString
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .Type = ValidationDataType.Date
                        .Text = "**"
                        .ErrorMessage = "Incorrect date format entered"
                    End With
                    tableCell(9).Controls.Add(valWED)

                    Dim valexWED As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexWED
                        .ID = "valexWED" & curItem.RechargeId.ToString
                        .TargetControlID = "valWED" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexWED)

                    ' end date
                    tableCell(10) = New TableCell
                    Dim dateSED As New TextBox
                    dateSED.ID = "SED" & curItem.RechargeId.ToString
                    If IsDate(curItem.SupportEndDate) And curItem.SupportEndDate.Year > 1900 Then
                        dateSED.Text = Format(curItem.SupportEndDate, cDef.DATE_FORMAT)
                    Else
                        dateSED.Text = ""
                    End If
                    dateSED.Width = Unit.Pixel(70)
                    tableCell(10).CssClass = rowClass
                    tableCell(10).Controls.Add(dateSED)

                    Dim valSED As New CompareValidator
                    With valSED
                        .ID = "valSED" & curItem.RechargeId.ToString
                        .ControlToValidate = "SED" & curItem.RechargeId.ToString
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .Type = ValidationDataType.Date
                        .ErrorMessage = "Incorrect date format entered"
                        .Text = "**"
                    End With
                    tableCell(10).Controls.Add(valSED)

                    Dim valexSED As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexSED
                        .ID = "valexSED" & curItem.RechargeId.ToString
                        .TargetControlID = "valSED" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexSED)

                    RTtablerow(rowcount) = New TableRow
                    RTtablerow(rowcount).Cells.AddRange(tableCell)
                    rowcount += 1
                End While

                RT_table.Rows.AddRange(RTtablerow)
            Else
                ' no items to display
                Dim tableCell As New TableCell
                tableCell.ColumnSpan = 10
                tableCell.HorizontalAlign = HorizontalAlign.Center
                tableCell.Text = "There are not currently any " & rs.ReferenceAs & " assignments"

                Dim RTtablerow As New TableRow
                RTtablerow.Cells.Add(tableCell)
                RT_table.Rows.Add(RTtablerow)
            End If

            RTData.Controls.Add(RT_table)
            RTDataButtons.Visible = True
        End Sub

        Public Overloads Sub ShowTemplate(ByVal db As cFWDBConnection, ByVal cpId As Integer)
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
            Dim rs As cRechargeSetting = rscoll.getSettings
            Dim unrecTable As New Table
            Dim employees As New cEmployees(curUser.Account.accountid)
            'Dim ufieldcoll As New cUserFieldGroupingCollection(fws, uinfo, AppAreas.RECHARGE_GROUPING, cpId, employees)

            Dim rechargeItems As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), params)
            Dim arrItems As ArrayList = rechargeItems.GetRechargeItemsByCPId(cpId)
            Dim conCategoryId As Integer = getContractCategory(db, cpId)

            Dim ufields As New cUserdefinedFields(fws.MetabaseCustomerId)
            Dim tables As New cTables(fws.MetabaseCustomerId)
            Dim rafields As List(Of cUserDefinedField) = ufields.getFieldsByTable(tables.getTableByName("recharge_associations"))

            Dim hasUFields As Boolean = False
            If Not rafields Is Nothing Then
                If rafields.Count > 0 Then
                    'If ufieldcoll.GetFieldGroupsForCategoryId(conCategoryId).Count > 0 Then
                    hasUFields = True
                End If
            End If

            ViewState("EditTemplates") = arrItems

            Dim RT_table As New Table

            RT_table.CssClass = "datatbl"
            RT_table.ID = "templatetable"

            RT_table.Rows.Add(GetTableHeaderRow(rs, False))

            Dim rowcount As Integer = 0
            Dim rowClass As String = "row1"
            Dim rowalt As Boolean = False

            If arrItems.Count > 0 Then
                Dim RTtablerow(arrItems.Count - 1) As TableRow
                Dim enumItems As System.Collections.IEnumerator = arrItems.GetEnumerator

                While enumItems.MoveNext
                    rowalt = (rowalt Xor True)
                    If rowalt Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    Dim curItem As cRecharge = CType(enumItems.Current, cRecharge)
                    Dim tableCell(10) As TableCell

                    ' edit UF data
                    tableCell(0) = New TableCell
                    tableCell(0).CssClass = rowClass

                    If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.RechargeAssociations, False) Then
                        If hasUFields Then
                            Dim updateCmd As New System.Web.UI.WebControls.Image
                            updateCmd.ImageUrl = "~/icons/16/plain/view.gif"
                            updateCmd.Attributes.Add("onclick", "javascript:window.location.href='ContractRechargeBreakdownUF.aspx?cpid=" & curItem.ContractProductId.ToString & "&rid=" & curItem.RechargeId.ToString & "&returl=" & Server.UrlEncode(Request.RawUrl) & "&cid=" & curItem.ContractId.ToString() & "';")
                            updateCmd.ID = "UFedit" & curItem.RechargeId.ToString
                            tableCell(0).Controls.Add(updateCmd)
                        End If
                    End If

                    ' delete icon
                    tableCell(1) = New TableCell
                    tableCell(1).CssClass = rowClass
                    If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.RechargeAssociations, False) Then
                        Dim delIcon As New Literal
                        With delIcon
                            Dim strHTML As New StringBuilder
                            strHTML.Append("<a onclick=""javascript:if(confirm('Click OK to confirm deletion')){window.location.href='ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract") & "&ret=showtemplate&action=delete&raid=" & curItem.RechargeId.ToString & "&cpid=" & curItem.ContractProductId.ToString & "';}"" ")
                            strHTML.Append("onmouseover=""window.status='Delete recharge item';return true;"" ")
                            strHTML.Append("onmouseout=""window.status='Done';"">")
                            strHTML.Append("<img alt=""Delete Item"" src=""./icons/delete2.gif"">")
                            strHTML.Append("</a>")
                            delIcon.Text = strHTML.ToString
                        End With
                        tableCell(1).Controls.Add(delIcon)
                    End If

                    ' customer
                    tableCell(2) = New TableCell
                    tableCell(2).Text = curItem.RechargeEntityName
                    tableCell(2).CssClass = rowClass
                    tableCell(2).HorizontalAlign = HorizontalAlign.Center
                    tableCell(2).Width = Unit.Pixel(120)

                    ' maint value
                    tableCell(3) = New TableCell
                    tableCell(3).HorizontalAlign = HorizontalAlign.Right
                    tableCell(3).CssClass = rowClass
                    Dim curCurrency As cCurrency = currency.getCurrencyById(curItem.CurrencyId)
                    tableCell(3).Text = currency.FormatCurrency(curItem.Maintenance, curCurrency, False)
                    tableCell(3).Width = Unit.Pixel(100)

                    ' apportion type
                    tableCell(4) = New TableCell
                    tableCell(4).Controls.Add(curItem.CreateApportionTypeListControl("APP" & curItem.RechargeId.ToString, curItem.ApportionType))
                    'tableCell(4).Text = curItem.GetApportionTypeDesc
                    tableCell(4).CssClass = rowClass
                    tableCell(4).HorizontalAlign = HorizontalAlign.Center

                    ' portion
                    tableCell(5) = New TableCell
                    Dim txtPortionControl As New TextBox
                    txtPortionControl.ID = "POR" & curItem.RechargeId.ToString
                    txtPortionControl.Text = curItem.Portion.ToString
                    txtPortionControl.Width = Unit.Pixel(70)
                    tableCell(5).CssClass = rowClass
                    tableCell(5).Controls.Add(txtPortionControl)
                    Dim valPortion As New CompareValidator
                    With valPortion
                        .ID = "valPortion" & curItem.RechargeId.ToString
                        .ControlToValidate = "POR" & curItem.RechargeId.ToString
                        .SetFocusOnError = True
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .ErrorMessage = "Numeric values only"
                        .Type = ValidationDataType.Double
                        .Text = "**"
                    End With
                    tableCell(5).Controls.Add(valPortion)
                    Dim valexPortion As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexPortion
                        .ID = "valexPortion" & curItem.RechargeId.ToString
                        .TargetControlID = "valPortion" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexPortion)

                    ' PW apportion type
                    tableCell(6) = New TableCell
                    tableCell(6).Controls.Add(curItem.CreateApportionTypeListControl("PW_APP" & curItem.RechargeId.ToString, curItem.PostWarrantyApportionType))
                    tableCell(6).CssClass = rowClass
                    tableCell(6).HorizontalAlign = HorizontalAlign.Center

                    ' PW portion
                    tableCell(7) = New TableCell
                    Dim txtPWPortion As New TextBox
                    txtPWPortion.ID = "PW_POR" & curItem.RechargeId.ToString
                    txtPWPortion.Text = curItem.PostWarrantyPortion.ToString
                    txtPWPortion.Width = Unit.Pixel(70)
                    tableCell(7).Controls.Add(txtPWPortion)
                    tableCell(7).CssClass = rowClass
                    Dim valPWPortion As New CompareValidator
                    With valPWPortion
                        .ID = "valPWPortion" & curItem.RechargeId.ToString
                        .ControlToValidate = "PW_POR" & curItem.RechargeId.ToString
                        .SetFocusOnError = True
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .ErrorMessage = "Numeric values only"
                        .Type = ValidationDataType.Double
                        .Text = "**"
                    End With
                    tableCell(7).Controls.Add(valPWPortion)
                    Dim valexPWPortion As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexPWPortion
                        .ID = "valexPWPortion" & curItem.RechargeId.ToString
                        .TargetControlID = "valPWPortion" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexPWPortion)

                    ' start date
                    tableCell(8) = New TableCell
                    Dim dateSSD As New TextBox
                    With dateSSD
                        .ID = "SSD" & curItem.RechargeId.ToString
                        If IsDate(curItem.SupportStartDate) And curItem.SupportStartDate.Year > 1900 Then
                            dateSSD.Text = Format(curItem.SupportStartDate, cDef.DATE_FORMAT)
                        Else
                            dateSSD.Text = ""
                        End If
                        .Width = Unit.Pixel(70)
                    End With
                    tableCell(8).CssClass = rowClass
                    tableCell(8).Controls.Add(dateSSD)

                    Dim valSSD As New CompareValidator
                    With valSSD
                        .ID = "valSSD" & curItem.RechargeId.ToString
                        .ControlToValidate = "SSD" & curItem.RechargeId.ToString
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .Type = ValidationDataType.Date
                        .Text = "**"
                        .ErrorMessage = "Incorrect date format entered"
                    End With
                    tableCell(8).Controls.Add(valSSD)

                    Dim valexSSD As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexSSD
                        .ID = "valexSSD" & curItem.RechargeId.ToString
                        .TargetControlID = "valSSD" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexSSD)

                    ' pw date
                    tableCell(9) = New TableCell
                    Dim dateWED As New TextBox
                    dateWED.ID = "WED" & curItem.RechargeId.ToString
                    If IsDate(curItem.WarrantyEndDate) And curItem.WarrantyEndDate.Year > 1900 Then
                        dateWED.Text = Format(curItem.WarrantyEndDate, cDef.DATE_FORMAT)
                    Else
                        dateWED.Text = ""
                    End If
                    dateWED.Width = Unit.Pixel(70)
                    tableCell(9).CssClass = rowClass
                    tableCell(9).Controls.Add(dateWED)

                    Dim valWED As New CompareValidator
                    With valWED
                        .ID = "valWED" & curItem.RechargeId.ToString
                        .ControlToValidate = "WED" & curItem.RechargeId.ToString
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .Type = ValidationDataType.Date
                        .Text = "**"
                        .ErrorMessage = "Incorrect date format entered"
                    End With
                    tableCell(9).Controls.Add(valWED)

                    Dim valexWED As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexWED
                        .ID = "valexWED" & curItem.RechargeId.ToString
                        .TargetControlID = "valWED" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexWED)

                    ' end date
                    tableCell(10) = New TableCell
                    Dim dateSED As New TextBox
                    dateSED.ID = "SED" & curItem.RechargeId.ToString
                    If IsDate(curItem.SupportEndDate) And curItem.SupportEndDate.Year > 1900 Then
                        dateSED.Text = Format(curItem.SupportEndDate, cDef.DATE_FORMAT)
                    Else
                        dateSED.Text = ""
                    End If
                    dateSED.Width = Unit.Pixel(70)
                    tableCell(10).CssClass = rowClass
                    tableCell(10).Controls.Add(dateSED)

                    Dim valSED As New CompareValidator
                    With valSED
                        .ID = "valSED" & curItem.RechargeId.ToString
                        .ControlToValidate = "SED" & curItem.RechargeId.ToString
                        .Operator = ValidationCompareOperator.DataTypeCheck
                        .Type = ValidationDataType.Date
                        .ErrorMessage = "Incorrect date format entered"
                        .Text = "**"
                    End With
                    tableCell(10).Controls.Add(valSED)

                    Dim valexSED As New AjaxControlToolkit.ValidatorCalloutExtender
                    With valexSED
                        .ID = "valexSED" & curItem.RechargeId.ToString
                        .TargetControlID = "valSED" & curItem.RechargeId.ToString
                    End With
                    RTData.Controls.Add(valexSED)

                    RTtablerow(rowcount) = New TableRow
                    RTtablerow(rowcount).Cells.AddRange(tableCell)
                    rowcount += 1
                End While

                RT_table.Rows.AddRange(RTtablerow)

                ' display unrecovered item in separate table
                Dim unrecovered As cRecharge = CType(arrItems(0), cRecharge)
                unrecTable.CssClass = "datatbl"
                Dim unrecHeader As New TableHeaderRow
                Dim unrecHCell As New TableHeaderCell
                unrecHCell.Width = Unit.Pixel(250)

                unrecHCell.Text = params.RechargeUnrecoveredTitle
                unrecHeader.Cells.Add(unrecHCell)
                unrecTable.Rows.Add(unrecHeader)

                Dim unrecRow As New TableRow
                Dim unrecCell As New TableCell
                unrecCell.HorizontalAlign = HorizontalAlign.Center
                If unrecovered.UnrecoveredPercent < 0 Or unrecovered.UnrecoveredPercent > 100 Then
                    unrecCell.CssClass = "row1 badrecovery"
                Else
                    unrecCell.CssClass = "row1"
                End If
                unrecCell.Text = Format(unrecovered.UnrecoveredPercent, "##0.##") & " %"
                unrecRow.Cells.Add(unrecCell)
                unrecTable.Rows.Add(unrecRow)
            Else
                ' no items to display
                Dim tableCell As New TableCell
                tableCell.ColumnSpan = 10
                tableCell.HorizontalAlign = HorizontalAlign.Center
                tableCell.Text = "There are not currently any " & rs.ReferenceAs & " assignments"

                Dim RTtablerow As New TableRow
                RTtablerow.Cells.Add(tableCell)
                RT_table.Rows.Add(RTtablerow)
            End If

            'Dim litTitle As New Literal
            'litTitle.Text = "<div class=""inputpaneltitle"">Recharge Templates not defined for this product</div>"
            'RTData.Controls.Add(litTitle)
            RTData.Controls.Add(RT_table)

            Dim litSeparator As New Literal
            litSeparator.Text = "<div style=""height: 15px;"">&nbsp;</div>"
            RTData.Controls.Add(litSeparator)
            RTData.Controls.Add(unrecTable)
            RTDataButtons.Visible = True
        End Sub

        Protected Sub cmdTemplateUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdTemplateUpdate.Click
            Select Case ViewState("RTAction")
                Case "show", "bulk"
                    If doTemplateUpdate() = False Then
                        litMessage.Text = "** ERROR - an error occurred trying to update the template"
                        Exit Sub
                    End If

                Case Else
            End Select

            Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub cmdTemplateCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdTemplateCancel.Click
            Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract"), True)
        End Sub

        Private Function doTemplateUpdate() As Boolean
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim retStatus As Boolean = True
            Dim ARec As New cAuditRecord
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            Dim firstlog As Boolean = True
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.RechargeAssociations, curUser.CurrentSubAccountId)
            Dim employees As New cEmployees(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            db.DBOpen(fws, False)
            db.FWDb("R2", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag Then
                ARec.ContractNumber = db.FWDbFindVal("Contract Key", 2)
            Else
                ARec.ContractNumber = ""
            End If
            ARec.Action = cFWAuditLog.AUDIT_UPDATE

            ' get copy of data on the screen
            Dim tabledata As ArrayList = CType(ViewState("EditTemplates"), ArrayList)
            Dim listenum As IEnumerator = tabledata.GetEnumerator
            Dim rechargeItems As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), params)

            While listenum.MoveNext
                Dim rItem As cRecharge = CType(listenum.Current, cRecharge)

                ARec.DataElementDesc = rItem.ProductName

                ' check portion type
                Dim lstPOR As DropDownList = CType(RTData.FindControl("APP" & rItem.RechargeId.ToString), DropDownList)
                If rItem.ApportionType <> lstPOR.SelectedItem.Value Then
                    ' changed
                    ARec.ElementDesc = "APPORTION TYPE"
                    ARec.PreVal = rItem.GetApportionTypeDesc
                    ARec.PostVal = lstPOR.SelectedItem.Text
                    ALog.AddAuditRec(ARec, firstlog)

                    rItem.ApportionType = CType(lstPOR.SelectedItem.Value, RechargeApportionType)

                    db.SetFieldValue("ApportionId", lstPOR.SelectedItem.Value, "N", firstlog)
                    firstlog = False
                End If

                ' check portion
                Dim txtPOR As TextBox = CType(RTData.FindControl("POR" & rItem.RechargeId.ToString), TextBox)
                If txtPOR.Text.Trim = "" Then
                    txtPOR.Text = "0"
                End If

                If rItem.Portion <> txtPOR.Text Then
                    ' changed
                    ARec.ElementDesc = "PORTION"
                    ARec.PreVal = rItem.Portion
                    ARec.PostVal = txtPOR.Text
                    ALog.AddAuditRec(ARec, firstlog)

                    rItem.Portion = Double.Parse(txtPOR.Text)

                    db.SetFieldValue("Portion", txtPOR.Text, "N", firstlog)
                    firstlog = False
                End If

                ' check PW Apportion Type
                Dim lstPWPOR As DropDownList = CType(RTData.FindControl("PW_APP" & rItem.RechargeId.ToString), DropDownList)
                If rItem.PostWarrantyApportionType <> lstPWPOR.SelectedItem.Value Then
                    ' changed
                    ARec.ElementDesc = "PW APPORTION TYPE"
                    ARec.PreVal = rItem.GetPWApportionTypeDesc
                    ARec.PostVal = lstPWPOR.SelectedItem.Text
                    ALog.AddAuditRec(ARec, firstlog)

                    rItem.PostWarrantyApportionType = CType(lstPWPOR.SelectedItem.Value, RechargeApportionType)

                    db.SetFieldValue("PostWarrantyApportionId", lstPWPOR.SelectedItem.Value, "N", firstlog)
                    firstlog = False
                End If

                ' check portion
                Dim txtPWPOR As TextBox = CType(RTData.FindControl("PW_POR" & rItem.RechargeId.ToString), TextBox)
                If txtPWPOR.Text.Trim = "" Then
                    txtPWPOR.Text = "0"
                End If

                If rItem.PostWarrantyPortion <> txtPWPOR.Text Then
                    ' changed
                    ARec.ElementDesc = "PW PORTION"
                    ARec.PreVal = rItem.PostWarrantyPortion
                    ARec.PostVal = txtPWPOR.Text
                    ALog.AddAuditRec(ARec, firstlog)

                    rItem.PostWarrantyPortion = Double.Parse(txtPWPOR.Text)

                    db.SetFieldValue("PostWarrantyPortion", txtPWPOR.Text, "N", firstlog)
                    firstlog = False
                End If

                ' check support start date
                Dim dateSSD As TextBox = CType(RTData.FindControl("SSD" & rItem.RechargeId.ToString), TextBox)
                If dateSSD.Text <> "" Then
                    If rItem.SupportStartDate <> CDate(dateSSD.Text) Then
                        'changed
                        ARec.ElementDesc = "SUPPORT START"
                        ARec.PreVal = rItem.SupportStartDate.ToShortDateString
                        ARec.PostVal = dateSSD.Text
                        ALog.AddAuditRec(ARec, firstlog)

                        rItem.SupportStartDate = CDate(dateSSD.Text)
                        db.SetFieldValue("SupportStartDate", dateSSD.Text, "D", firstlog)
                        firstlog = False
                    End If
                Else
                    ' check date has not been blanked
                    If rItem.SupportStartDate.Year > 1900 Then
                        ARec.ElementDesc = "SUPPORT START"
                        ARec.PreVal = rItem.SupportStartDate.ToShortDateString
                        ARec.PostVal = ""
                        ALog.AddAuditRec(ARec, firstlog)

                        rItem.SupportStartDate = DateTime.MinValue
                        db.SetFieldValue("SupportStartDate", DBNull.Value, "N", firstlog)
                        firstlog = False
                    End If
                End If

                Dim dateWED As TextBox = CType(RTData.FindControl("WED" & rItem.RechargeId.ToString), TextBox)
                If dateWED.Text <> "" Then
                    If CDate(rItem.WarrantyEndDate) <> CDate(dateWED.Text) Then
                        'changed
                        ARec.ElementDesc = "WARRANTY END"
                        ARec.PreVal = rItem.WarrantyEndDate.ToShortDateString
                        ARec.PostVal = dateWED.Text
                        ALog.AddAuditRec(ARec, firstlog)

                        rItem.WarrantyEndDate = CDate(dateWED.Text)
                        db.SetFieldValue("WarrantyEndDate", dateWED.Text, "D", firstlog)
                        firstlog = False
                    End If
                Else
                    If rItem.WarrantyEndDate.Year > 1900 Then
                        ARec.ElementDesc = "WARRANTY END"
                        ARec.PreVal = rItem.WarrantyEndDate.ToShortDateString
                        ARec.PostVal = ""
                        ALog.AddAuditRec(ARec, firstlog)

                        rItem.WarrantyEndDate = DateTime.MinValue
                        db.SetFieldValue("WarrantyEndDate", DBNull.Value, "N", firstlog)
                        firstlog = False
                    End If
                End If

                Dim dateSED As TextBox = CType(RTData.FindControl("SED" & rItem.RechargeId.ToString), TextBox)
                If dateSED.Text <> "" Then
                    If CDate(rItem.SupportEndDate) <> CDate(dateSED.Text) Then
                        'changed
                        ARec.ElementDesc = "SUPPORT END"
                        ARec.PreVal = rItem.SupportEndDate.ToShortDateString
                        ARec.PostVal = dateSED.Text
                        ALog.AddAuditRec(ARec, firstlog)

                        rItem.SupportEndDate = CDate(dateSED.Text)
                        db.SetFieldValue("SupportEndDate", dateSED.Text, "D", firstlog)
                        firstlog = False
                    End If
                Else
                    If rItem.SupportEndDate.Year > 1900 Then
                        'blank
                        ARec.ElementDesc = "SUPPORT END"
                        ARec.PreVal = rItem.SupportEndDate.ToShortDateString
                        ARec.PostVal = ""
                        ALog.AddAuditRec(ARec, firstlog)

                        rItem.SupportEndDate = DateTime.MaxValue
                        db.SetFieldValue("SupportEndDate", DBNull.Value, "N", firstlog)
                        firstlog = False
                    End If
                End If

                If firstlog = False Then
                    db.FWDb("A", "recharge_associations", "RechargeId", rItem.RechargeId, "", "", "", "", "", "", "", "", "", "")
                    firstlog = True
                    ALog.CommitAuditLog(curUser.Employee, rItem.RechargeId)

                    rechargeItems.UpdateRechargeItem(rItem)
                End If
            End While

            Return retStatus
        End Function

        Protected Sub cmdAddClient_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdAddClient.Click
            doAddClient()
        End Sub

        Protected Sub cmdGenCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdGenCancel.Click
            Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub lnkSelectAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkSelectAll.Click
            Dim url As String = Request.RawUrl.Replace(Request.ApplicationPath & "/", "")
            url.Replace("&chk=1", "")
            url.Replace("&chk=0", "")
            Response.Redirect(url & "&chk=1", True)
        End Sub

        Protected Sub lnkDeselectAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkDeselectAll.Click
            Dim url As String = Request.RawUrl.Replace(Request.ApplicationPath & "/", "")
            url.Replace("&chk=1", "")
            url.Replace("&chk=0", "")
            Response.Redirect(url & "&chk=0", True)
        End Sub

        Protected Sub lnkOnOffLine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkOnOffLine.Click
            ' get list of checked rechargeitems
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ritems As New cRechargeCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties)

            Dim csvList As New System.Text.StringBuilder
            Dim comma As String = ""

            For i As Integer = 0 To ritems.Count - 1
                Dim ritem As cRecharge = ritems.GetRechargeItemByIndex(i)

                Dim ctrl As CheckBox = CType(RTData.FindControl("CHK" & ritem.ContractProductId.ToString), CheckBox)
                If Not ctrl Is Nothing Then
                    If ctrl.Checked Then
                        csvList.Append(comma)
                        csvList.Append(ritem.ContractProductId.ToString)
                        comma = ","
                    End If
                End If
            Next

            If csvList.Length > 0 Then
                Response.Redirect("ContractRechargeOnOffLine.aspx?cid=" & Session("ActiveContract") & "&cplist=" & csvList.ToString, True)
            End If
        End Sub

        Protected Sub lnkGenerateAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkGenerateAll.Click
            doGenerate(True)
        End Sub

        Private Shared Function getContractCategory(ByVal db As cFWDBConnection, ByVal cpId As Integer) As Integer
            Dim conCategory As Integer = 0

            Dim sql As String = "SELECT contract_details.[CategoryId] FROM contract_productdetails INNER JOIN contract_details ON contract_productdetails.[contractId] = contract_details.[contractId] WHERE [contractProductId] = @cpId"
            db.AddDBParam("cpId", cpId, True)
            db.RunSQL(sql, db.glDBWorkA, False, "", False)

            conCategory = db.GetFieldValue(db.glDBWorkA, "CategoryId", 0, 0)
            Return conCategory
        End Function
    End Class
End Namespace
