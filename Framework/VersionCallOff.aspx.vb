Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class VersionCallOff
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
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

			If Me.IsPostBack = False Then
				FWDb.DBOpen(fws, False)

				Title = "Version Registry Call-off"
				Master.title = Title

				Session("CurRegIdx") = Request.QueryString("id")
				lblTitle.Text = Request.QueryString("desc")

                lnkDelete.Visible = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.VersionRegistry, False)
                lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.VersionRegistry, False)
                cmdUpdate.Enabled = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.VersionRegistry, False)
                lnkUpgrade.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.VersionRegistry, False)
				lnkDelete.ToolTip = "Remove highlighted calloff entry"
				lnkNew.ToolTip = "Add a new calloff entry"
				cmdUpdate.ToolTip = "Make an amendment to the highlighted entry"
				cmdCancel.ToolTip = "Return to the Version Registry screen"
				lnkUpgrade.ToolTip = "Upgrade quantity to newer version"

				Session("subtotal") = 0

				sql = "SELECT * FROM [version_registry_calloff] WHERE [Registry Id] = " & Trim(Session("CurRegIdx"))
				FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "", False)

				With CallOffGrid
					.DataSource = FWDb.glDBWorkA.Tables(0).DefaultView
					.DataBind()

					.ExpandAll(True)
				End With

				cmpQuantity.ErrorMessage = "Data value must be numeric"
				lblQuantity.Text = "Quantity: "
				lblLocale.Text = "Locale: "
				lblComment.Text = "Comment: "
				lblDateObtained.Text = "Date Obtained: "

				FWDb.DBClose()
			End If

			FWDb = Nothing
		End Sub

		Private Sub ShowDetail(ByVal x As Boolean)
			'lblQuantity.Visible = x
			'txtQuantity.Visible = x
			'lblLocale.Visible = x
			'txtLocale.Visible = x
			'lblComment.Visible = x
			'txtComment.Visible = x
			'cmpQuantity.Visible = x
			'dateObtained.Visible = x
			'lblDateObtained.Visible = x

			'txtQuantity.Text = ""
			'txtLocale.Text = ""
			'txtComment.Text = ""
			'dateObtained.Value = ""

			panelEditFields.Visible = x

			If x = False Then
				lstUpgradeVersions.Visible = x
			End If
		End Sub

		Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdUpdate.Click
			Dim FWDb As New cFWDBConnection
			Dim histDb As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
			Dim ARec As New cAuditRecord
			Dim tmpStr As String
			Dim firstchange, QtyChange As Boolean
			Dim newQty, tmpidx As Integer

			FWDb.DBOpen(fws, False)
			histDb.DBOpen(fws, False)

			newQty = -1
			firstchange = True

			' version history record preparation
			histDb.SetFieldValue("Datestamp", Format(Now(), cDef.DATE_FORMAT), "D", True)
			histDb.SetFieldValue("Registry Id", Session("CurRegIdx"), "N", False)
            histDb.SetFieldValue("Changed By", curUser.Employee.firstname & " " & curUser.Employee.surname, "S", False)

            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.VersionRegistry, curUser.CurrentSubAccountId)

			' add or update the version calloff table
			Select Case Session("Action")
				Case "ADD"
					histDb.SetFieldValue("PreVal", "", "N", False)
					histDb.SetFieldValue("PostVal", txtQuantity.Text, "N", False)
					histDb.SetFieldValue("Locale", txtLocale.Text, "S", False)
					histDb.SetFieldValue("Comment", txtComment.Text, "S", False)
					histDb.SetFieldValue("Date Obtained", dateObtained.Text, "D", False)
					histDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")

					FWDb.SetFieldValue("Registry Id", Session("CurRegIdx"), "N", True)
					FWDb.SetFieldValue("Quantity Used", txtQuantity.Text, "N", False)
					FWDb.SetFieldValue("Locale", txtLocale.Text, "S", False)
					FWDb.SetFieldValue("Comment", txtComment.Text, "S", False)
					FWDb.SetFieldValue("Date Obtained", dateObtained.Text, "D", False)
					FWDb.FWDb("W", "version_registry_callOff", "", "", "", "", "", "", "", "", "", "", "", "")

					newQty = Session("subtotal") + Val(txtQuantity.Text)

					ARec.Action = cFWAuditLog.AUDIT_ADD
					ARec.ContractNumber = ""
					ARec.DataElementDesc = "VERSION REGISTRY:BREAKDOWN"
					ARec.ElementDesc = "QUANTITY USED"
					ARec.PreVal = ""
					ARec.PostVal = txtLocale.Text & ":" & txtQuantity.Text
					ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))

                Case "UPDATE"
                    FWDb.FWDb("R2", "version_registry_callOff", "CallOff Id", Session("CurCallOffIdx"), "", "", "", "", "", "", "", "", "", "")
                    If FWDb.FWDb2Flag = True Then
                        tmpStr = FWDb.FWDbFindVal("Quantity Used", 2)
                        If tmpStr <> txtQuantity.Text Then
                            FWDb.SetFieldValue("Quantity Used", txtQuantity.Text, "N", firstchange)
                            firstchange = False

                            ' new quantity = new value plus subtotal (minus the original value before update)
                            newQty = (Val(txtQuantity.Text) + Session("subtotal")) - Val(tmpStr)

                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ""
                            ARec.DataElementDesc = "VERSION REGISTRY:BREAKDOWN"
                            ARec.ElementDesc = "QUANTITY USED"
                            ARec.PreVal = tmpStr
                            ARec.PostVal = txtQuantity.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))
                        End If
                    End If

                    tmpStr = FWDb.FWDbFindVal("Locale", 2)
                    If tmpStr <> txtLocale.Text Then
                        FWDb.SetFieldValue("Locale", txtLocale.Text, "S", firstchange)
                        firstchange = False
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ""
                        ARec.DataElementDesc = "VERSION REGISTRY:BREAKDOWN"
                        ARec.ElementDesc = "LOCALE"
                        ARec.PreVal = tmpStr
                        ARec.PostVal = txtLocale.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))
                    End If

                    tmpStr = FWDb.FWDbFindVal("Comment", 2)
                    If tmpStr <> txtComment.Text Then
                        FWDb.SetFieldValue("Comment", txtComment.Text, "S", firstchange)
                        firstchange = False
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ""
                        ARec.DataElementDesc = "VERSION REGISTRY:BREAKDOWN"
                        ARec.ElementDesc = "COMMENT"
                        ARec.PreVal = Left(tmpStr, 60)
                        ARec.PostVal = Left(txtComment.Text, 60)
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))
                    End If

                    tmpStr = FWDb.FWDbFindVal("Date Obtained", 2)
                    If tmpStr <> "" Then
                        tmpStr = Format(CDate(tmpStr), cDef.DATE_FORMAT)
                    End If
                    If tmpStr <> dateObtained.Text Then
                        FWDb.SetFieldValue("Date Obtained", dateObtained.Text, "D", firstchange)
                        firstchange = False
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ""
                        ARec.DataElementDesc = "VERSION REGISTRY:BREAKDOWN"
                        ARec.ElementDesc = "DATE OBTAINED"
                        ARec.PreVal = tmpStr
                        ARec.PostVal = dateObtained.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("CurRegIdx"))
                    End If

                    If firstchange = False Then
                        FWDb.FWDb("A", "Version Registry CallOff", "Calloff Id", Session("CurCallOffIdx"), "", "", "", "", "", "", "", "", "", "")
                        histDb.SetFieldValue("PreVal", FWDb.FWDbFindVal("Quantity Used", 2), "N", False)
                        histDb.SetFieldValue("PostVal", txtQuantity.Text, "N", False)
                        histDb.SetFieldValue("Locale", txtLocale.Text, "S", False)
                        histDb.SetFieldValue("Comment", txtComment.Text, "S", False)
                        histDb.SetFieldValue("Date Obtained", dateObtained.Text, "D", False)
                        histDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")
                    End If
                Case "UPGRADE"
                    If Val(txtQuantity.Text) = 0 Then
                        Exit Select
                    End If

                    If Val(txtQuantity.Text) > Session("UpgradeFromQuantity") Then
                        lblErrorString.Text = "Insufficient Quantity to perform upgrade"
                        QtyChange = False
                        Exit Select
                    End If

                    tmpidx = lstUpgradeVersions.SelectedItem.Value

                    ' log in the history table that upgrade to new version took place
                    histDb.SetFieldValue("PreVal", Session("UpgradeFromQuantity"), "N", False)
                    histDb.SetFieldValue("PostVal", Session("UpgradeFromQuantity") - Val(txtQuantity.Text), "N", False)
                    histDb.SetFieldValue("Locale", "", "S", False)
                    histDb.SetFieldValue("Comment", "Upgrade version to :" & lstUpgradeVersions.SelectedItem.Text, "S", False)
                    histDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")

                    ' log entry in the calloff that a quantity were called off for upgrade
                    FWDb.SetFieldValue("Registry Id", Session("CurRegIdx"), "N", True)
                    FWDb.SetFieldValue("Locale", txtLocale.Text, "S", False)
                    FWDb.SetFieldValue("Date Obtained", Now(), "D", False)
                    FWDb.SetFieldValue("Comment", "Upgrade version to :" & lstUpgradeVersions.SelectedItem.Text, "S", False)
                    FWDb.SetFieldValue("Quantity Used", 0 - (Val(txtQuantity.Text)), "N", False)
                    FWDb.FWDb("W", "version_registry_callOff", "", "", "", "", "", "", "", "", "", "", "", "")

                    ' update the current version to reflect removal of quantity
                    FWDb.SetFieldValue("Quantity", Session("UpgradeFromQuantity") - Val(txtQuantity.Text), "N", True)
                    FWDb.FWDb("A", "version_registry", "Registry Id", Session("CurRegIdx"), "", "", "", "", "", "", "", "", "", "")

                    ' amend upgrade version to display the new upgraded values
                    ' retrieve record for the version being upgraded to
                    FWDb.FWDb("R2", "version_registry", "Registry Id", tmpidx, "", "", "", "", "", "", "", "", "", "")
                    If FWDb.FWDb2Flag = True Then
                        ' add new versions to the total quantity used
                        FWDb.SetFieldValue("Quantity", Val(FWDb.FWDbFindVal("Quantity", 2)) + Val(txtQuantity.Text), "N", True)
                        FWDb.FWDb("A", "version_registry", "Registry Id", tmpidx, "", "", "", "", "", "", "", "", "", "")

                        ' log entry in the version calloff
                        FWDb.SetFieldValue("Registry Id", tmpidx, "N", True)
                        FWDb.SetFieldValue("Quantity Used", txtQuantity.Text, "N", False)
                        FWDb.SetFieldValue("Comment", "Upgrade version from :" & Session("UpgradeFromVersion"), "S", False)
                        FWDb.SetFieldValue("Locale", FWDb.FWDbFindVal("Locale", 2), "S", False)
                        FWDb.SetFieldValue("Date Obtained", Now(), "D", False)
                        FWDb.FWDb("W", "version_registry_calloff", "", "", "", "", "", "", "", "", "", "", "", "")

                        ' log entry in the version history file
                        FWDb.SetFieldValue("Registry Id", tmpidx, "N", True)
                        FWDb.SetFieldValue("Changed by", curUser.Employee.firstname & " " & curUser.Employee.surname, "S", False)
                        FWDb.SetFieldValue("Datestamp", Now(), "D", False)
                        FWDb.SetFieldValue("Comment", "Upgrade version from :" & Session("UpgradeFromVersion"), "S", False)
                        FWDb.SetFieldValue("Locale", FWDb.FWDbFindVal("Locale", 2), "S", False)
                        FWDb.SetFieldValue("PreVal", FWDb.FWDbFindVal("Quantity", 2), "S", False)
                        FWDb.SetFieldValue("PostVal", Val(FWDb.FWDbFindVal("Quantity", 2)) + Val(txtQuantity.Text), "N", False)
                        FWDb.FWDb("W", "version_history", "", "", "", "", "", "", "", "", "", "", "", "")
                    End If
                Case Else
            End Select

            If newQty <> -1 Or QtyChange = True Then
                tmpStr = "UPDATE [productDetails] SET [Calloff Quantity] = (SELECT SUM([Quantity Used]) AS [QtyTotal] FROM [version_registry_calloff] WHERE [Product Id] = " & Session("ActiveProduct") & ")" & vbNewLine & _
                 "WHERE [ProductId] = " & Trim(Session("ActiveProduct"))
                FWDb.RunSQL(tmpStr, FWDb.glDBWorkD, False, "", False)
            End If

            ShowDetail(False)

            histDb.DBClose()
            FWDb.DBClose()

            FWDb = Nothing
            histDb = Nothing

            Response.Redirect("VersionCallOff.aspx?desc=" & Trim(lblTitle.Text) & "&id=" & Trim(Session("CurRegIdx")) & "&qty=" & Trim(Session("total")), True)
        End Sub

        Private Sub CallOffGrid_DblClick(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebGrid.ClickEventArgs) Handles CallOffGrid.DblClick
            Dim tmpStr As String

            Session("CurCallOffIdx") = e.Row.Cells.FromKey("CallOff Id").Value
            Session("Action") = "UPDATE"

            ShowDetail(True)
            txtQuantity.Text = e.Row.Cells.FromKey("Quantity Used").Value
            txtLocale.Text = e.Row.Cells.FromKey("Locale").Text
            txtComment.Text = e.Row.Cells.FromKey("Comment").Text
            tmpStr = e.Row.Cells.FromKey("Date Obtained").Text
            If tmpStr <> "" Then
                dateObtained.Value = Format(CDate(tmpStr), cDef.DATE_FORMAT)
            Else
                dateObtained.Value = ""
            End If

            lnkNew.Visible = False
            cmdUpdate.Visible = True
            ' override the permission for an insert
            cmdUpdate.Enabled = True
        End Sub

        Private Sub doDelete()
            Dim FWDb As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim Idx As Integer
            Dim ARec As New cAuditRecord

            With CallOffGrid
                If Not .DisplayLayout.ActiveRow Is Nothing Then
                    FWDb.DBOpen(fws, False)
                    Dim ALog As New cFWAuditLog(fws, SpendManagementElement.VersionRegistry, curUser.CurrentSubAccountId)

                    Idx = .DisplayLayout.ActiveRow.Index

                    ARec.Action = cFWAuditLog.AUDIT_DEL
                    ARec.DataElementDesc = "VERSION REGISTRY:BREAKDOWN"
                    ARec.ElementDesc = lblTitle.Text
                    ARec.PreVal = .Rows(Idx).Cells.FromKey("Quantity Used").Text
                    ARec.PostVal = ""
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, .Rows(Idx).Cells.FromKey("CallOff Id").Value)

					FWDb.FWDb("D", "version_registry_calloff", "CallOff Id", .Rows(Idx).Cells.FromKey("CallOff Id").Value, "", "", "", "", "", "", "", "", "", "")

					FWDb.DBClose()
				End If
			End With

			FWDb = Nothing

			Response.Redirect("VersionCallOff.aspx?desc=" & Trim(lblTitle.Text) & "&id=" & Trim(Session("CurRegIdx")), True)
		End Sub

		Private Sub CallOffGrid_InitializeLayout(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebGrid.LayoutEventArgs) Handles CallOffGrid.InitializeLayout
			e.Layout.AllowSortingDefault = Infragistics.WebUI.UltraWebGrid.AllowSorting.Yes
			e.Layout.HeaderClickActionDefault = Infragistics.WebUI.UltraWebGrid.HeaderClickAction.SortSingle
			e.Layout.ColFootersVisibleDefault = Infragistics.WebUI.UltraWebGrid.ShowMarginInfo.Yes

			SetRowStyle(e)

			e.Layout.Bands(0).Columns.FromKey("CallOff Id").Hidden = True
			e.Layout.Bands(0).Columns.FromKey("Registry Id").Hidden = True
			SetHeaderStyle(e, 0, "Quantity Used")
			SetHeaderStyle(e, 0, "Locale")
			SetHeaderStyle(e, 0, "Comment")
			SetHeaderStyle(e, 0, "Date Obtained")

			e.Layout.Bands(0).Columns.FromKey("Date Obtained").Format = cDef.DATE_FORMAT
			e.Layout.Bands(0).Columns.FromKey("Date Obtained").CellStyle.HorizontalAlign = HorizontalAlign.Center

			e.Layout.Bands(0).Columns.FromKey("Date Obtained").SortIndicator = Infragistics.WebUI.UltraWebGrid.SortIndicator.Descending
			e.Layout.Bands(0).SortedColumns.Add(e.Layout.Bands(0).Columns.FromKey("Date Obtained"))

			With e.Layout.Bands(0).Columns.FromKey("Quantity Used")
				.FooterTotal = Infragistics.WebUI.UltraWebGrid.SummaryInfo.Sum
				.FooterText = "Subtotal: "
				.FooterStyle.Font.Name = "Arial"
				.FooterStyle.Font.Size = FontUnit.XXSmall
			End With
		End Sub

		Private Sub CallOffGrid_InitializeRow(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebGrid.RowEventArgs) Handles CallOffGrid.InitializeRow
			Session("subtotal") = Session("subtotal") + e.Row.Cells.FromKey("Quantity Used").Value
		End Sub

		Private Sub doUpgrade()
			Dim FWDb As New cFWDBConnection
			Dim rowIdx As Integer
			Dim sql As String
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As cAccountSubAccounts = New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)

			With CallOffGrid
				If Not .DisplayLayout.ActiveRow Is Nothing Then
					rowIdx = .DisplayLayout.ActiveRow.Index

					Session("CurRegIdx") = .Rows(rowIdx).Cells.FromKey("Registry Id").Value

                    FWDb.DBOpen(fws, False)

					lblQuantity.Visible = True
					lstUpgradeVersions.Visible = True
					lblLocale.Visible = True
					lblLocale.Text = "Locale: "
					txtLocale.Text = .Rows(rowIdx).Cells.FromKey("Locale").Value

					txtQuantity.Visible = True
					txtQuantity.Text = .Rows(rowIdx).Cells.FromKey("Quantity Used").Value
					Session("Action") = "UPGRADE"
					Session("UpgradeFromQuantity") = .Rows(rowIdx).Cells.FromKey("Quantity Used").Value
					lnkNew.Visible = False
					cmdUpdate.Visible = True
					lnkUpgrade.Visible = False

					sql = "SELECT DISTINCT([Version]), [Registry Id] FROM [version_registry] WHERE [Product Id] = " & Trim(Session("ActiveProduct")) & " AND [Version Order] > " & Trim(Session("CurVersionOrder"))
					FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "", False)

					lstUpgradeVersions.Items.Clear()
					With lstUpgradeVersions
						.DataSource = FWDb.glDBWorkA
						.DataTextField = "Version"
						.DataValueField = "Registry Id"
						.DataBind()
					End With

					lstUpgradeVersions.Visible = True

					FWDb.DBClose()
				End If
			End With

			FWDb = Nothing
		End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Response.Redirect(Session("VerRegURL"), True)
        End Sub

        Protected Sub lnkNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNew.Click
            Session("Action") = "ADD"
            ShowDetail(True)
            lnkNew.Visible = False
            cmdUpdate.Visible = True
        End Sub

        Protected Sub lnkDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkDelete.Click
            doDelete()
        End Sub
    End Class
End Namespace
