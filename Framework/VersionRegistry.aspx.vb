Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class VersionRegistry
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
            Dim FWDb As New cFWDBConnection
			Dim sql As String
			Dim tmpId As Integer
			Dim hasdata As Boolean
			Dim action As String
			Dim ARec As New cAuditRecord
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.VersionRegistry, curUser.CurrentSubAccountId)

			FWDb.DBOpen(fws, False)

            If Me.IsPostBack = False Then
                curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VersionRegistry, False, True)

                Session("isUpdate") = False
                Session("ConProdDesc") = Request.QueryString("desc")

                Title = "Version Registry"
                Master.title = Title

                action = Request.QueryString("action")
                Select Case LCase(action)
                    Case "amend"
                        curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.VersionRegistry, False, True)

                        Session("Action") = "UPDATE"
                        tmpId = Integer.Parse(Request.QueryString("vid"))
                        Session("CurRegIdx") = tmpId
                        FWDb.FWDb("R2", "version_registry", "Registry Id", tmpId, "", "", "", "", "", "", "", "", "", "")
                        If FWDb.FWDb2Flag = True Then
                            ShowDetail(True)
                            txtVersion.Text = FWDb.FWDbFindVal("Version", 2)
                            dateFirstObtained.Value = FWDb.FWDbFindVal("First Obtained Date", 2)
                            'dateFirstObtained.ReadOnly = True
                            txtQuantity.Text = "0"
                            'x = lstType.SelectedIndex

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

                            lblTo.Visible = True
                            lstToVersion.Visible = True
                            ' populate with all versions available except this one
                            sql = "SELECT [Registry Id],[Version] FROM [version_registry] WHERE [Product Id] = " & Trim(Session("ActiveProduct")) & " AND [Registry Id] <> " & tmpId.ToString.Trim & " ORDER BY [First Obtained Date]"
                            FWDb.RunSQL(sql, FWDb.glDBWorkC, False, "", False)
                            lstToVersion.DataSource = FWDb.glDBWorkC
                            lstToVersion.DataTextField = "Version"
                            lstToVersion.DataValueField = "Registry Id"
                            lstToVersion.DataBind()
                            lstToVersion.Items.Insert(0, New ListItem("please select...", 0))
                            Session("isUpdate") = True
                        End If

                    Case "delete"
                        curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.VersionRegistry, False, True)

                        tmpId = Val(Request.QueryString("vid"))
                        FWDb.FWDb("R2", "version_registry", "Registry Id", tmpId, "", "", "", "", "", "", "", "", "", "")
                        If FWDb.FWDb2Flag = True Then
                            FWDb.FWDb("R", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDbFlag = True Then
                                ARec.ContractNumber = FWDb.FWDbFindVal("Contract Key", 1)
                            End If
                            ARec.Action = cFWAuditLog.AUDIT_DEL
                            ARec.DataElementDesc = "VERSION REGISTRY"
                            ARec.ElementDesc = "VERSION:" & FWDb.FWDbFindVal("Version", 2)
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, tmpId)

                            ' delete version and accompanying history
                            FWDb.FWDb("D", "version_registry", "Registry Id", tmpId, "", "", "", "", "", "", "", "", "", "")
                            FWDb.FWDb("D", "version_history", "Registry Id", tmpId, "", "", "", "", "", "", "", "", "", "")

                            UpdateQuantities(FWDb)
                        End If

                    Case "view"
                        Dim curCPId As Integer = Request.QueryString("cpid")

                        Session("ActiveProduct") = curCPId
                        litVRGrid.Text = GetVersionData(FWDb, curUser, Session("ActiveProduct"))

                        cmdClose.ToolTip = "Return to the version registry list"
                        cmdClose.AlternateText = "Close"
                        cmdClose.Attributes.Add("onmouseover", "window.status='Return to the version registry list';return true;")
                        cmdClose.Attributes.Add("onmouseout", "window.status='Done';")

                    Case Else
                        ' display contract products to review versions for
                        litVRGrid.Text = GetContractProducts(FWDb, curUser, Session("ActiveContract"))

                        cmdClose.ToolTip = "Exit the version registry"
                        cmdClose.AlternateText = "Close"
                        cmdClose.Attributes.Add("onmouseover", "window.status='Exit the version registry';return true;")
                        cmdClose.Attributes.Add("onmouseout", "window.status='Done';")
                End Select

                SetHeadings()

                hasdata = False

                Session("subtotal") = 0

                Session("VerRegURL") = "VersionRegistry.aspx?desc=" & Trim(Session("ConProdDesc"))
            End If

            lnkNew.ToolTip = "Define a new version of the product"
            lnkNew.Attributes.Add("onmouseover", "window.status='Define a new version of the product';return true;")
            lnkNew.Attributes.Add("onmouseout", "window.status='Done';")

            cmdUpdate.ToolTip = "Confirm and save version detail"
            cmdUpdate.Attributes.Add("onmouseover", "window.status='Confirm and save version detail';return true;")
            cmdUpdate.Attributes.Add("onmouseout", "window.status='Done';")
            cmdUpdate.AlternateText = "Update"

            lnkNew.Enabled = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.VersionRegistry, False)

            lnkHistory.ToolTip = "View history for all listed versions"
            lnkHistory.Attributes.Add("onmouseover", "window.status='Confirm and save version detail';return true;")
            lnkHistory.Attributes.Add("onmouseout", "window.status='Done';")

            FWDb.DBClose()
            FWDb = Nothing
        End Sub

        Private Sub SetHeadings()
            If Not Session("ConProdDesc") Is Nothing Or Session("ConProdDesc") <> "" Then
                lblTitle.Text = " for " & Session("ConProdDesc")
            Else
                lblTitle.Text = ""
            End If

            lblQuantity.Text = "Quantity" ' getlang(248)
            lblType.Text = "Type"
            'lblVersionOrder.Text = "Version Order" 'GetLang(217)
            lblFirstObtained.Text = "First Obtained"
            lblVersion.Text = "Version" 'GetLang(431)
            lblComment.Text = "Comment"
            lblReseller.Text = "Reseller"
        End Sub

        Private Sub ShowDetail(ByVal x As Boolean)
            panelEditFields.Visible = x
            reqVersion.Enabled = x
            cmpQuantity.Enabled = x
        End Sub

        Private Sub CloseVR()
            Dim tmpStr As String

            tmpStr = "ContractSummary.aspx?id=" & Trim(Session("ActiveContract")) & "&tab=" & SummaryTabs.ContractProduct
            'tmpStr = Session("ReturnURL")
            'If tmpStr.Trim = "" Then
            ' tmpStr = "Home.aspx"
            ' End If
            Session("ReturnURL") = Nothing
            Session("ConProdDesc") = Nothing
            Session("Action") = Nothing
            Session("isUpdate") = Nothing
            Session("CurRegIdx") = Nothing
            Response.Redirect(tmpStr, True)
        End Sub

        Private Sub UpdateVersion()
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim FWDb As New cFWDBConnection
            Dim ARec As New cAuditRecord
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.VersionRegistry, curUser.CurrentSubAccountId)
            Dim firstchange, QtyChange As Boolean
            Dim tmpVal, tmpKey As String
            Dim tmpIdx, curQty As Integer

            firstchange = True
            FWDb.DBOpen(fws, False)

            FWDb.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If FWDb.FWDb2Flag = True Then
                tmpKey = FWDb.FWDbFindVal("Contractkey", 2)
                If Trim(tmpKey) = "" Then
                    tmpKey = FWDb.FWDbFindVal("ContractNumber", 2)
                End If
            Else
                tmpKey = "n/a"
            End If

            'tmpQty = Session("subtotal")
            'sql = "SELECT SUM([Quantity]) AS [Qty] FROM [Version Registry] WHERE [Product Id] = " & Trim(Session("ActiveProduct")) & " AND [Contract Id] = " & Trim(Session("ActiveContract"))
            'FWDb.RunSQL(sql, FWDb.glDBWorkD)
            'tmpQty = Val(FWDb.GetFieldValue(FWDb.glDBWorkD, "Qty", 0))

            QtyChange = False

            Select Case Session("Action")
                Case "ADD"
                    FWDb.SetFieldValue("Contract Id", Session("ActiveContract"), "N", True)
                    FWDb.SetFieldValue("Product Id", Session("ActiveProduct"), "N", False)
                    FWDb.SetFieldValue("Version", txtVersion.Text, "S", False)
                    'FWDb.SetFieldValue("Version Order", txtVersionOrder.Text, "N", False)
                    If dateFirstObtained.Text <> "" Then
                        FWDb.SetFieldValue("First Obtained Date", dateFirstObtained.Value, "D", False)
                    End If
                    FWDb.SetFieldValue("Quantity", txtQuantity.Text, "N", False)
                    FWDb.FWDb("W", "version_registry", "", "", "", "", "", "", "", "", "", "", "", "")

                    tmpIdx = FWDb.glIdentity
                    If tmpIdx <> 0 Then
                        Session("CurRegIdx") = tmpIdx
                        ARec.Action = cFWAuditLog.AUDIT_ADD
                        ARec.ContractNumber = tmpKey
                        ARec.DataElementDesc = "VERSION REGISTRY"
                        ARec.ElementDesc = ""
                        ARec.PreVal = ""
                        ARec.PostVal = txtVersion.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, tmpIdx)

                        ' add entry into the history to indicate addition of new version
                        FWDb.SetFieldValue("Registry Id", tmpIdx, "N", True)
                        If lstType.SelectedItem.Value = 0 Then
                            FWDb.SetFieldValue("PlusMinusQty", Val(txtQuantity.Text), "N", False)
                        Else
                            FWDb.SetFieldValue("PlusMinusQty", 0 - Val(txtQuantity.Text), "N", False)
                        End If
                        FWDb.SetFieldValue("Type", lstType.SelectedItem.Text, "S", False)
                        FWDb.SetFieldValue("Comment", txtComment.Text, "S", False)
                        FWDb.SetFieldValue("Changed by", curUser.Employee.firstname & " " & curUser.Employee.surname, "S", False)
                        If dateFirstObtained.Text <> "" Then
                            FWDb.SetFieldValue("Datestamp", dateFirstObtained.Value, "D", False)
                        End If
                        FWDb.SetFieldValue("Reseller", lstReseller.SelectedItem.Text, "S", False)
                        FWDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")
                    End If

                    'tmpQty = tmpQty + Val(txtQuantity.Text)
                    QtyChange = True

                Case "UPDATE"
                    FWDb.FWDb("R2", "version_registry", "Registry Id", Session("CurRegIdx"), "", "", "", "", "", "", "", "", "", "")
                    If FWDb.FWDb2Flag = True Then
                        curQty = Val(FWDb.FWDbFindVal("Quantity", 2))

                        Select Case lstType.SelectedItem.Value
                            Case 0 ' purchase
                                ' additional licences must have been purchased, so increment quantity
                                'FWDb.FWDb("R2", "Version Registry - Contracts", "Contract Id", Session("ActiveContract"), "Registry Id", Session("CurRegIdx"))
                                'If FWDb.FWDb2Flag = True Then

                                ' total exists for this contract and registry amount
                                FWDb.SetFieldValue("Quantity", curQty + Val(txtQuantity.Text), "N", True)
                                FWDb.FWDb("A", "version_registry", "Registry Id", Session("CurRegIdx"), "", "", "", "", "", "", "", "", "", "")

                                'FWDb.FWDb("A", "Version Registry - Contracts", "Contract Id", Session("ActiveContract"), "Registry Id", Session("CurRegIdx"))
                                'Else
                                '    ' new version purchase for this contract & product combination
                                '    FWDb.SetFieldValue("Contract Id", Session("ActiveContract"), "N", True)
                                '    FWDb.SetFieldValue("Registry Id", Session("CurRegIdx"), "N", False)
                                '    FWDb.SetFieldValue("Quantity", txtQuantity.Text, "N", False)
                                '    FWDb.FWDb("W", "Version Registry - Contracts")
                                'End If

                                ARec.Action = cFWAuditLog.AUDIT_UPDATE
                                ARec.ContractNumber = tmpKey
                                ARec.DataElementDesc = "VERSION REGISTRY"
                                ARec.ElementDesc = "PURCHASE:" & txtVersion.Text
                                ARec.PreVal = "Qty: " & Trim(Str(curQty))
                                ARec.PostVal = "Qty: " & Trim(Str(curQty + Val(txtQuantity.Text)))
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))

                                ' write entry to the version history
                                FWDb.SetFieldValue("Registry Id", Session("CurRegIdx"), "N", True)
                                FWDb.SetFieldValue("PlusMinusQty", txtQuantity.Text, "N", False)
                                FWDb.SetFieldValue("Comment", txtComment.Text, "S", False)
                                FWDb.SetFieldValue("Type", lstType.SelectedItem.Text, "S", False)
                                FWDb.SetFieldValue("Changed by", curUser.Employee.firstname & " " & curUser.Employee.surname, "S", False)
                                If dateFirstObtained.Text <> "" Then
                                    FWDb.SetFieldValue("Datestamp", dateFirstObtained.Value, "D", False)
                                End If
                                FWDb.SetFieldValue("Reseller", lstReseller.SelectedItem.Text, "S", False)
                                FWDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")

                            Case 1 ' upgrade
                                If Val(txtQuantity.Text) > curQty Then
                                    lblErrorString.Text = "ERROR! Quantity specified exceeds quantity available for upgrade"
                                    FWDb.DBClose()
                                    FWDb = Nothing
                                    Exit Sub
                                End If

                                If lstToVersion.SelectedItem.Value = 0 Then
                                    lblErrorString.Text = "ERROR! Upgrade MUST provide version upgrading to."
                                    FWDb.DBClose()
                                    FWDb = Nothing
                                    Exit Sub
                                End If

                                FWDb.SetFieldValue("Quantity", curQty - Val(txtQuantity.Text), "N", firstchange)
                                firstchange = False

                                ARec.Action = cFWAuditLog.AUDIT_UPDATE
                                ARec.ContractNumber = tmpKey
                                ARec.DataElementDesc = "VERSION REGISTRY"
                                ARec.ElementDesc = "UPGRADE:" & txtVersion.Text
                                ARec.PreVal = "Qty: " & Trim(Str(curQty))
                                ARec.PostVal = "Qty: " & Trim(Str(curQty - Val(txtQuantity.Text)))
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))

                                FWDb.FWDb("A", "version_registry", "Registry Id", Session("CurRegIdx"), "", "", "", "", "", "", "", "", "", "")

                                ' history for version being upgraded
                                FWDb.SetFieldValue("Registry Id", Session("CurRegIdx"), "N", True)
                                FWDb.SetFieldValue("PlusMinusQty", 0 - Val(txtQuantity.Text), "N", False)
                                FWDb.SetFieldValue("Comment", "[to " & lstToVersion.SelectedItem.Text & "]" & txtComment.Text, "S", False)
                                FWDb.SetFieldValue("Type", lstType.SelectedItem.Text, "S", False)
                                FWDb.SetFieldValue("Changed by", curUser.Employee.firstname & " " & curUser.Employee.surname, "S", False)
                                If dateFirstObtained.Text <> "" Then
                                    FWDb.SetFieldValue("Datestamp", dateFirstObtained.Value, "D", False)
                                Else
                                    FWDb.SetFieldValue("Datestamp", Now, "D", False)
                                End If
                                FWDb.SetFieldValue("Reseller", lstReseller.SelectedItem.Text, "S", False)
                                FWDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")

                                ' update the quantity of the version upgraded to and create history entry
                                FWDb.FWDb("R2", "version_registry", "Registry Id", lstToVersion.SelectedItem.Value, "", "", "", "", "", "", "", "", "", "")
                                If FWDb.FWDb2Flag = True Then
                                    FWDb.SetFieldValue("Quantity", Val(FWDb.FWDbFindVal("Quantity", 2)) + Val(txtQuantity.Text), "N", True)
                                    FWDb.FWDb("A", "version_registry", "Registry Id", FWDb.FWDbFindVal("Registry Id", 2), "", "", "", "", "", "", "", "", "", "")

                                    FWDb.SetFieldValue("Registry Id", FWDb.FWDbFindVal("Registry Id", 2), "N", True)
                                    FWDb.SetFieldValue("PlusMinusQty", Val(txtQuantity.Text), "N", False)
                                    FWDb.SetFieldValue("Comment", "[from " & txtVersion.Text & "]" & txtComment.Text, "S", False)
                                    FWDb.SetFieldValue("Type", lstType.SelectedItem.Text, "S", False)
                                    FWDb.SetFieldValue("Changed by", curUser.Employee.firstname & " " & curUser.Employee.surname, "S", False)
                                    If dateFirstObtained.Text <> "" Then
                                        FWDb.SetFieldValue("Datestamp", dateFirstObtained.Value, "D", False)
                                    Else
                                        FWDb.SetFieldValue("Datestamp", Now, "D", False)
                                    End If
                                    FWDb.SetFieldValue("Reseller", lstReseller.SelectedItem.Text, "S", False)
                                    FWDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")
                                End If

                            Case 99 ' cancellation
                                ' cancelled - just remove quantity specified.
                                FWDb.SetFieldValue("Quantity", curQty - Val(txtQuantity.Text), "N", firstchange)
                                firstchange = False
                                FWDb.FWDb("A", "version_registry", "Registry Id", Session("CurRegIdx"), "", "", "", "", "", "", "", "", "", "")

                                ARec.Action = cFWAuditLog.AUDIT_UPDATE
                                ARec.ContractNumber = tmpKey
                                ARec.DataElementDesc = "VERSION REGISTRY"
                                ARec.ElementDesc = "CANCELLATION:" & txtVersion.Text
                                ARec.PreVal = "Qty: " & Trim(Str(curQty))
                                ARec.PostVal = "Qty: " & Trim(Str(curQty - Val(txtQuantity.Text)))
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))

                                ' write entry to the version history
                                FWDb.SetFieldValue("Registry Id", Session("CurRegIdx"), "N", True)
                                FWDb.SetFieldValue("PlusMinusQty", 0 - Val(txtQuantity.Text), "N", False)
                                FWDb.SetFieldValue("Comment", txtComment.Text, "S", False)
                                FWDb.SetFieldValue("Type", lstType.SelectedItem.Text, "S", False)
                                FWDb.SetFieldValue("Changed by", curUser.Employee.firstname & " " & curUser.Employee.surname, "S", False)
                                If dateFirstObtained.Text <> "" Then
                                    FWDb.SetFieldValue("Datestamp", dateFirstObtained.Value, "D", False)
                                Else
                                    FWDb.SetFieldValue("Datestamp", Now, "D", False)
                                End If
                                FWDb.SetFieldValue("Reseller", "n/a", "S", False)
                                FWDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")

                            Case 2 ' adjustment
                                ' adjustment to enable changes for errors
                                FWDb.SetFieldValue("Quantity", curQty + Val(txtQuantity.Text), "N", firstchange)
                                firstchange = False
                                FWDb.FWDb("A", "version_registry", "Registry Id", Session("CurRegIdx"), "", "", "", "", "", "", "", "", "", "")

                                ARec.Action = cFWAuditLog.AUDIT_UPDATE
                                ARec.ContractNumber = tmpKey
                                ARec.DataElementDesc = "VERSION REGISTRY"
                                ARec.ElementDesc = "ADJUSTMENT:" & txtVersion.Text
                                ARec.PreVal = "Qty: " & Trim(Str(curQty))
                                ARec.PostVal = "Qty: " & Trim(Str(curQty + Val(txtQuantity.Text)))
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))

                                ' write entry to the version history
                                FWDb.SetFieldValue("Registry Id", Session("CurRegIdx"), "N", True)
                                FWDb.SetFieldValue("PlusMinusQty", Val(txtQuantity.Text), "N", False)
                                FWDb.SetFieldValue("Comment", txtComment.Text, "S", False)
                                FWDb.SetFieldValue("Type", lstType.SelectedItem.Text, "S", False)
                                FWDb.SetFieldValue("Changed by", curUser.Employee.firstname & " " & curUser.Employee.surname, "S", False)
                                If dateFirstObtained.Text <> "" Then
                                    FWDb.SetFieldValue("Datestamp", dateFirstObtained.Value, "D", False)
                                Else
                                    FWDb.SetFieldValue("Datestamp", Now, "D", False)
                                End If
                                FWDb.SetFieldValue("Reseller", "n/a", "S", False)
                                FWDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")
                            Case Else

                        End Select

                        firstchange = True
                        FWDb.FWDb("R2", "version_registry", "Registry Id", Session("CurRegIdx"), "", "", "", "", "", "", "", "", "", "")
                        tmpVal = FWDb.FWDbFindVal("Version", 2)
                        If tmpVal <> txtVersion.Text Then
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = tmpKey
                            ARec.DataElementDesc = "VERSION REGISTRY"
                            ARec.ElementDesc = "VERSION"
                            ARec.PreVal = tmpVal
                            ARec.PostVal = txtVersion.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))

                            FWDb.SetFieldValue("Version", txtVersion.Text, "S", firstchange)
                            firstchange = False
                        End If

                        tmpVal = FWDb.FWDbFindVal("First Obtained Date", 2)
                        If tmpVal <> "" Then
                            tmpVal = Format(CDate(tmpVal), cDef.DATE_FORMAT)
                        End If

                        If tmpVal <> Trim(dateFirstObtained.Text) Then
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = tmpKey
                            ARec.DataElementDesc = "VERSION REGISTRY"
                            ARec.ElementDesc = "DATE FIRST OBTAINED"
                            ARec.PreVal = tmpVal
                            ARec.PostVal = dateFirstObtained.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))

							FWDb.SetFieldValue("First Obtained Date", dateFirstObtained.Text, "D", firstchange)
							firstchange = False
						End If

						If firstchange = False Then
							FWDb.FWDb("A", "version_registry", "Registry Id", Session("CurRegIdx"), "", "", "", "", "", "", "", "", "", "")
						End If
						QtyChange = True
					End If
				Case Else
			End Select

			If QtyChange = True Then
				UpdateQuantities(FWDb)
			End If

			Session("UpgradeFromVersion") = Nothing
			Session("UpgradeFromQuantity") = Nothing

			Session("Action") = Nothing
			FWDb.DBClose()
			FWDb = Nothing
			Response.Redirect("VersionRegistry.aspx?action=view&cpid=" & Session("ActiveProduct") & "&desc=" & Trim(Session("ConProdDesc")), True)
		End Sub

		Private Sub UpdateQuantities(ByVal db As cFWDBConnection)
			Dim sql As New System.Text.StringBuilder

			' update the version registry total regardless of contract
			'sql = "UPDATE [Version Registry] SET [Quantity] = (SELECT SUM([Quantity]) AS [Total] FROM [Version Registry - Contracts] WHERE [Registry Id] = " & Trim(Session("CurRegIdx")) & ") WHERE [Registry Id] = " & Trim(Session("CurRegIdx"))
			'FWDb.RunSQL(sql, FWDb.glDBWorkD)

			' update the master Contract Product quantity field with the new total
			' quantity in the version registry for this contract and product
			sql.Append("UPDATE [contract_productdetails] SET [Quantity] = ")
			sql.Append("(SELECT SUM([PlusMinusQty]) AS [Total] FROM [version_history] INNER JOIN [version_registry] ON [version_history].[Registry Id] = [version_registry].[Registry Id] WHERE [version_registry].[Product Id] = " & Trim(Session("ActiveProduct")) & " AND [version_registry].[Contract Id] = " & CStr(Session("ActiveContract")) & ") ")
			sql.Append("WHERE [Product Id] = @prodID AND [Contract Id] = @conID")
			db.AddDBParam("prodID", Session("ActiveProduct"), True)
            db.AddDBParam("conID", Session("ActiveContract"), False)
            db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)

			' update the product details entry for new 'number of licenced copies'
			sql = New System.Text.StringBuilder
            sql.Append("UPDATE [productDetails] SET [NumLicencedCopies] = (SELECT SUM([Quantity]) AS [Total] FROM [version_registry] WHERE [Product Id] = " & Session("ActiveProduct") & ")")
            sql.Append(" WHERE [ProductId] = @prodID")
			db.AddDBParam("prodID", Session("ActiveProduct"), True)
			db.ExecuteSQL(sql.ToString)
		End Sub

        Private Function GetVersionData(ByVal db As cFWDBConnection, ByVal curUser As CurrentUser, ByVal prodID As Integer) As String
            Dim sql, tmpStr, tmpWhere As String
            Dim drow As DataRow
            Dim sumVal, totalLicenced As Integer
            Dim loopIdx, tmpQty As Integer
            Dim strHTML As New System.Text.StringBuilder

            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th><img src=""./icons/delete2.gif"" alt=""Delete"" /></th>" & vbNewLine)
            strHTML.Append("<th><img src=""./icons/edit.gif"" alt=""Edit"" /></th>" & vbNewLine)
            strHTML.Append("<th>Version Name</th>" & vbNewLine)
            strHTML.Append("<th>Qty Purchased</th>" & vbNewLine)
            strHTML.Append("<th>Upgraded<br>from<br>Prior Versions</th>" & vbNewLine)
            strHTML.Append("<th>Upgraded<br>to<br>New Versions</th>" & vbNewLine)
            strHTML.Append("<th>Current<br>Quantity Held</th>" & vbNewLine)
            strHTML.Append("<th>Date First Obtained</th>" & vbNewLine)
            strHTML.Append("<th>History</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            sql = "SELECT * FROM [version_registry] WHERE [Product Id] = (SELECT [ProductId] FROM [contract_productdetails] WHERE [ContractProductId] = @cpId) ORDER BY [First Obtained Date] DESC"
            db.AddDBParam("cpId", prodID, True)
            db.RunSQL(sql, db.glDBWorkB, False, "", False)

            totalLicenced = 0

            Dim rowalt As Boolean = False
            Dim rowClass As String = "row1"

            If db.GetRowCount(db.glDBWorkB, 0) > 0 Then
                For Each drow In db.glDBWorkB.Tables(0).Rows
                    rowalt = (rowalt Xor True)
                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>" & vbNewLine)
                    If IsDBNull(drow.Item("Quantity")) = True Then
                        db.SetFieldValue("Quantity", 0, "N", True)
                        db.FWDb("A", "version_registry", "Registry Id", drow.Item("Registry Id"), "", "", "", "", "", "", "", "", "", "")

                        tmpQty = 0
                    Else
                        tmpQty = drow.Item("Quantity")
                    End If

                    If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.VersionRegistry, False) And tmpQty = 0 Then
                        strHTML.Append("<a href=""javascript:if(confirm('This will delete this version and remove all associated history. Confirm?')){window.location.href='VersionRegistry.aspx?action=delete&desc=" & Trim(Session("ConProdDesc")) & "&vid=" & Trim(Str(drow.Item("Registry Id"))) & "';}""><img src=""./icons/delete2.gif"" /></a>")
                    Else
                        strHTML.Append("&nbsp;")
                    End If
                    strHTML.Append("</td>" & vbNewLine)

                    strHTML.Append("<td class=""" & rowClass & """>" & vbNewLine)
                    If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.VersionRegistry, False) Then
                        strHTML.Append("<a href=""VersionRegistry.aspx?action=amend&desc=" & Trim(Session("ConProdDesc")) & "&vid=" & Trim(Str(drow.Item("Registry Id"))) & """><img src=""./icons/edit.gif"" /></a>")
                    Else
                        strHTML.Append("n/a")
                    End If
                    strHTML.Append("</td>" & vbNewLine)

                    strHTML.Append("<td class=""" & rowClass & """>" & Trim(drow.Item("Version")) & "</td>" & vbNewLine)

                    For loopIdx = 1 To 3
                        Select Case loopIdx
                            Case 1
                                tmpStr = "Purchase"
                                tmpWhere = ""

                            Case 2
                                tmpStr = "Upgrade"
                                tmpWhere = " AND [PlusMinusQty] > 0"

                            Case 3
                                tmpStr = "Upgrade"
                                tmpWhere = " AND [PlusMinusQty] < 0"

                            Case Else
                                Exit For
                        End Select
                        sql = "SELECT ABS(SUM([PlusMinusQty])) AS [Qty] FROM [version_history] WHERE [Registry Id] = " & Trim(Str(drow.Item("Registry Id"))) & " AND [Type] = '" & tmpStr & "'"
                        If tmpWhere <> "" Then
                            sql = sql & tmpWhere
                        End If

                        db.RunSQL(sql, db.glDBWorkC, False, "", False)
                        If db.GetRowCount(db.glDBWorkC, 0) > 0 Then
                            If IsDBNull(db.GetFieldValue(db.glDBWorkC, "Qty", 0, 0)) = True Then
                                sumVal = 0
                            Else
                                sumVal = db.GetFieldValue(db.glDBWorkC, "Qty", 0, 0)
                            End If
                        End If
                        strHTML.Append("<td class=""" & rowClass & """>" & sumVal.ToString & "</td>" & vbNewLine)
                    Next

                    If IsDBNull(drow.Item("Quantity")) = True Then
                        tmpStr = "0"
                    Else
                        tmpStr = drow.Item("Quantity")
                    End If

                    totalLicenced = totalLicenced + Val(tmpStr)

                    strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
                    If IsDBNull(drow.Item("First Obtained Date")) = False Then
                        tmpStr = Format(CDate(drow.Item("First Obtained Date")), cDef.DATE_FORMAT)
                    Else
                        tmpStr = "n/a"
                    End If
                    strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)

                    strHTML.Append("<td class=""" & rowClass & """><a href=""VersionHistory.aspx?displaytype=1&id=" & Trim(drow.Item("Registry Id")) & "&desc=" & Trim(drow.Item("Version")) & """>View History</a></td>" & vbNewLine)
                    strHTML.Append("</tr>" & vbNewLine)
                Next

                strHTML.Append("<tr><td class=""" & rowClass & """ colspan=""6"" align=""right"">Total licenced: </td><td class=""" & rowClass & """>" & totalLicenced & "</td><td colspan=""2"">&nbsp;</td></tr>" & vbNewLine)
            Else
                strHTML.Append("<tr><td class=""row1"" colspan=""9"" align=""center"">No version information returned</td></tr>" & vbNewLine)
            End If

            strHTML.Append("</table>" & vbNewLine)

            GetVersionData = strHTML.ToString
        End Function

		'Private Sub cmdUpgrade_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs)
		'    Dim FWDb As New cFWDBConnection
		'    Dim rowIdx As Integer
		'    Dim sql As String

		'    If Not VersionRegistryGrid.DisplayLayout.ActiveRow Is Nothing Then
		'        rowIdx = VersionRegistryGrid.DisplayLayout.ActiveRow.Index

		'        Session("CurRegIdx") = VersionRegistryGrid.Rows(rowIdx).Cells.FromKey("Registry Id").Value

		'        lblQuantity.Visible = True
		'        lblVersion.Visible = True
		'        txtQuantity.Visible = True
		'        txtQuantity.Text = VersionRegistryGrid.Rows(rowIdx).Cells.FromKey("Quantity").Value
		'        Session("Action") = "UPGRADE"
		'        Session("UpgradeFromVersion") = VersionRegistryGrid.Rows(rowIdx).Cells.FromKey("Version").Text
		'        Session("UpgradeFromQuantity") = VersionRegistryGrid.Rows(rowIdx).Cells.FromKey("Quantity").Value
		'        cmdInsert.Visible = False
		'        cmdUpdate.Visible = True

		'        FWDb.DBOpen(fws,false)

		'        sql = "SELECT DISTINCT([Version]), [Registry Id] FROM [Version Registry] WHERE [Product Id] = " & Trim(Session("ActiveProduct")) & " AND [Version Order] > " & Trim(VersionRegistryGrid.Rows(rowIdx).Cells.FromKey("Version Order").Value)
		'        FWDb.RunSQL(sql, FWDb.glDBWorkA)

		'        lstUpgradeVersions.Items.Clear()
		'        With lstUpgradeVersions
		'            .DataSource = FWDb.glDBWorkA
		'            .DataTextField = "Version"
		'            .DataValueField = "Registry Id"
		'            .DataBind()
		'        End With

		'        txtVersion.Visible = False
		'        lstUpgradeVersions.Visible = True

		'        FWDb.DBClose()
		'    End If

		'    FWDb = Nothing
		'End Sub

		Private Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
			If Request.QueryString("action") = "view" Then
				Response.Redirect("VersionRegistry.aspx?id=" & Session("ActiveContract"), True)
			Else
				CloseVR()
			End If

		End Sub

		Protected Sub lnkNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNew.Click
			ShowDetail(True)
			txtVersion.Text = ""
			txtQuantity.Text = "0"
			Session("Action") = "ADD"
			lstType.Enabled = False
		End Sub

		Protected Sub lnkHistory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkHistory.Click
			Response.Redirect("VersionHistory.aspx?displaytype=0", True)
		End Sub

		Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdUpdate.Click
			UpdateVersion()
		End Sub

		Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
			Response.Redirect("VersionRegistry.aspx?action=view&cpid=" & Session("ActiveProduct") & "&desc=" & Session("ConProdDesc"), True)
		End Sub

        Private Function GetContractProducts(ByVal db As cFWDBConnection, ByVal curUser As CurrentUser, ByVal ConId As Integer) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)

            Dim sql As New System.Text.StringBuilder
            With sql
                .Append("SELECT [ContractProductId],ISNULL([Currencyid],0) AS [CurrencyId],[productDetails].[ProductName],ISNULL([MaintenanceValue],0) AS [MaintenanceValue],ISNULL([Quantity],0) AS [Quantity],ISNULL([codes_units].[Description],'') AS [Description] " & vbNewLine)
                .Append("FROM [contract_productdetails] " & vbNewLine)
                .Append("LEFT JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[ProductId] " & vbNewLine)
                .Append("LEFT JOIN [codes_units] ON [codes_units].[UnitId] = [contract_productdetails].[UnitId] " & vbNewLine)
                .Append("WHERE [ArchiveStatus] = 0 AND [ContractId] = @conId " & vbNewLine)
                .Append("ORDER BY [ProductName]" & vbNewLine)
                db.AddDBParam("conId", ConId, True)
                db.RunSQL(.ToString, db.glDBWorkA, False, "", False)
            End With

            With strHTML
                .Append("<table class=""datatbl"">" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                .Append("<th><img src=""./icons/16/plain/view.gif"" /></th>" & vbNewLine)
                .Append("<th>Product / Service Name</th>" & vbNewLine)
                .Append("<th>Annual Cost</th>" & vbNewLine)
                .Append("<th>Quantity</th>" & vbNewLine)
                .Append("<th>Unit Description</th>" & vbNewLine)
                .Append("</tr>" & vbNewLine)

                Dim rowClass As String = "row1"
                Dim rowalt As Boolean = False

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    rowalt = (rowalt Xor True)
                    If rowalt Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    .Append("<tr>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """><a title=""Review Version registry for " & drow.Item("ProductName") & """ onmouseover=""window.status='Review Version registry for " & drow.Item("ProductName") & "';return true;"" onmouseout=""window.status='Done';"" href=""VersionRegistry.aspx?cpid=" & drow.Item("ContractProductId") & "&action=view&desc=" & drow.Item("ProductName") & """><img src=""./icons/16/plain/view.gif"" /></a></td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & drow.Item("ProductName") & "</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & currency.FormatCurrency(drow.Item("MaintenanceValue"), currency.getCurrencyById(drow.Item("CurrencyId")), False) & "</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & drow.Item("Quantity") & "</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & drow.Item("Description") & "</td>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                Next

                .Append("</table>" & vbNewLine)

            End With
            Return strHTML.ToString
        End Function
    End Class
End Namespace
