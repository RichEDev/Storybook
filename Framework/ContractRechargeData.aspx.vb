Imports SpendManagementLibrary
Imports Spend_Management
Imports FWClasses

Namespace Framework2006
    Partial Class ContractRechargeData
        Inherits System.Web.UI.Page

        Private Enum SQL_GET_TYPE
            CONTRACT = 0
            CONPROD = 1
        End Enum
        Private CP_Count As Integer

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
            Dim conId As Integer
			Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
			Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
			Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)
			Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr)
            Dim rs As cRechargeSetting = rscoll.getSettings

			Dim cCOLL As New cCPFieldInfo(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr, curUser.Employee.employeeid, Session("ActiveContract"))

			Title = "Recharge Payments"
			Master.title = Title

			conId = Request.QueryString("id")

			Dim action As String
			action = Request.QueryString("action")

			If Me.IsPostBack = False Then
				Dim sql As System.Text.StringBuilder
				Dim db As New cFWDBConnection

				db.DBOpen(fws, False)

				Select Case action
					Case "edit"
						RPEditPanel.Visible = True
						RPFilterPanel.Visible = False
						Master.enablenavigation = False

						ShowEdit(db, Request.QueryString("rpid"))

					Case "delete"
						DeletePayment(db, Request.QueryString("rpid"))

					Case Else
				End Select

				If action <> "edit" Then
					sql = New System.Text.StringBuilder
					sql.Append("SELECT COUNT(*) AS [ConProdCount] ")
					sql.Append(" FROM [contract_productdetails]")
                    sql.Append(" WHERE [contract_productdetails].[ContractId] = @conId")
					db.AddDBParam("conId", Session("ActiveContract"), True)
					db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)

					CP_Count = db.GetFieldValue(db.glDBWorkD, "ConProdCount", 0, 0)

					' if date range is not stored in session vars, set from and to as -1/+1 from todays date (3 month period)
					Dim dateFStr, dateTStr As String
					Dim curdate As DateTime

					If Session("DisplayRechargeDataFrom") Is Nothing Then
						curdate = Today
						Session("DisplayRechargeDataFrom") = curdate.Day.ToString & "/" & curdate.Month.ToString & "/" & curdate.Year.ToString
					End If

					' set the from date automatically (with to date 1 calendar month from that)
					If Session("DisplayRechargeDataTo") Is Nothing Then
						curdate = DateTime.Parse(Session("DisplayRechargeDataFrom"))
						Session("DisplayRechargeDataTo") = curdate.AddDays(DateTime.DaysInMonth(curdate.Year, curdate.Month)).ToShortDateString
					End If

					dateFStr = Session("DisplayRechargeDataFrom")
					txtFromDate.Text = DateTime.Parse(dateFStr).ToShortDateString

					dateTStr = Session("DisplayRechargeDataTo")
					txtToDate.Text = DateTime.Parse(dateTStr).ToShortDateString

					If Not Cache("RCG_" & Session("ActiveContract")) Is Nothing Then
						litReturnCount.Text = "<img alt=""Alert"" src=""./images/information.gif"" />&nbsp; Payment generation still in progress. Please wait...Press F5 to refresh."
						cmdRefresh.Visible = False
					ElseIf Not Cache("RCP_" & Session("ActiveContract")) Is Nothing Then
						litReturnCount.Text = "<img alt=""Alert"" src=""./images/information.gif"" />&nbsp; Payment data still being loaded. Please wait...Press F5 to refresh."
						cmdRefresh.Visible = False
					Else
						Dim rPayments As New cRechargePaymentCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr, cCOLL, Session("ActiveContract"), New cProducts(curUser.Account.accountid, curUser.CurrentSubAccountId), DateTime.Parse(txtFromDate.Text))

						Dim arrData As ArrayList = rPayments.GetPaymentsBetween(CDate(txtFromDate.Text), CDate(txtToDate.Text))

						GetPaymentsGrid(db, arrData, cCOLL)
					End If
				End If

				db.DBClose()
				db = Nothing
			End If
		End Sub

		Private Sub GetPaymentsGrid(ByVal db As cFWDBConnection, ByVal data As ArrayList, ByVal cColl As cCPFieldInfo)
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.AccountID).getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim cFWCurrencies As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
				Dim RP_Table As New Table
				RP_Table.ID = "RP_Table"
				RP_Table.CssClass = "datatbl"

				Dim rowClass As String = "row1"
				Dim rowalt As Boolean = False

                Dim rscoll As New cRechargeSettings(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID))
                Dim rs As cRechargeSetting = rscoll.getSettings

				Dim headeroffset As Integer = 0
				Dim RPheader As New TableHeaderRow
				If cColl.CPFieldInfoItem.CP_UF1 > 0 Then
					headeroffset += 1
				End If
				If cColl.CPFieldInfoItem.CP_UF2 > 0 Then
					headeroffset += 1
				End If
				Dim RPheadercell(5 + headeroffset) As TableHeaderCell
				headeroffset = 0

				Dim editimg As New System.Web.UI.WebControls.Image
				editimg.ImageUrl = "./icons/edit.gif"
				RPheadercell(0) = New TableHeaderCell
				RPheadercell(0).Controls.Add(editimg)

				Dim delimg As New System.Web.UI.WebControls.Image
				delimg.ImageUrl = "./icons/delete2.gif"
				RPheadercell(1) = New TableHeaderCell
				RPheadercell(1).Controls.Add(delimg)
				RPheadercell(2) = New TableHeaderCell
				RPheadercell(2).Text = rs.ReferenceAs
				RPheadercell(3) = New TableHeaderCell
				RPheadercell(3).Text = "Product Name"
				If cColl.CPFieldInfoItem.CP_UF1 > 0 Then
					RPheadercell(4 + headeroffset) = New TableHeaderCell
					RPheadercell(4 + headeroffset).Text = cColl.CPFieldInfoItem.CP_UF1_Title
					headeroffset += 1
				End If
				If cColl.CPFieldInfoItem.CP_UF2 > 0 Then
					RPheadercell(4 + headeroffset) = New TableHeaderCell
					RPheadercell(4 + headeroffset).Text = cColl.CPFieldInfoItem.CP_UF2_Title
					headeroffset += 1
				End If
				RPheadercell(4 + headeroffset) = New TableHeaderCell
				RPheadercell(4 + headeroffset).Text = "Recharge Period"
				RPheadercell(5 + headeroffset) = New TableHeaderCell
				RPheadercell(5 + headeroffset).Text = "Recharge Amount"
				'RPheadercell(6 + headeroffset) = New TableHeaderCell
				'RPheadercell(6 + headeroffset).Text = "One-off?"

				RPheader.Cells.AddRange(RPheadercell)
				RP_Table.Rows.Add(RPheader)

				Dim RProw As TableRow
				Dim RPcell(5 + headeroffset) As TableCell
				Dim litDelete As Literal
				Dim litEdit As Literal
				Dim strDelete As String
				Dim hasdata As Boolean = False
				strDelete = "<a style=""cursor: hand;"" onmouseover=""window.status='Delete Recharge Payment';return true;"" onmouseout=""window.status='Done';"" onclick=""javascript:if(confirm('Click OK to confirm deletion')){window.location.href='ContractRechargeData.aspx?action=delete&rpid=%RPID%&id=" & Session("ActiveContract") & "';}"" onmouseover=""window.status='Delete payment item';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/delete2.gif"" alt=""Delete"" /></a>"
				Dim strEdit As String
				strEdit = "<a style=""cursor: hand;"" onmouseover=""window.status='Edit Recharge Payment';return true;"" onmouseout=""window.status='Done';"" onclick=""javascript:window.location.href='ContractRechargeData.aspx?action=edit&rpid=%RPID%&id=" & Session("ActiveContract") & "';"" onmouseover=""window.status='Edit payment item';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/edit.gif"" alt=""Edit"" /></a>"

				Dim data_ceiling As Integer = data.Count
				If data_ceiling > 100 Then
					' too much data, only display top 100
					data_ceiling = 100
				End If

				For i As Integer = 0 To data_ceiling - 1
					rowalt = (rowalt Xor True)
					If rowalt Then
						rowClass = "row1"
					Else
						rowClass = "row2"
					End If

					Dim rpItem As cRechargePayment = CType(data(i), cRechargePayment)

					RProw = New TableRow

					RPcell(0) = New TableCell
					RPcell(0).CssClass = rowClass
					If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.RechargePayments, False) Then
						litEdit = New Literal
						litEdit.Text = strEdit.Replace("%RPID%", rpItem.PaymentId.ToString)
						RPcell(0).Controls.Add(litEdit)
					End If

					RPcell(1) = New TableCell
					RPcell(1).CssClass = rowClass
					If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.RechargePayments, False) Then
						litDelete = New Literal
						litDelete.Text = strDelete.Replace("%RPID%", rpItem.PaymentId.ToString)
						RPcell(1).Controls.Add(litDelete)
					End If

					RPcell(2) = New TableCell
					RPcell(2).CssClass = rowClass
					RPcell(2).Text = rpItem.EntityName
					RPcell(2).Width = Unit.Pixel(100)

					RPcell(3) = New TableCell
					RPcell(3).CssClass = rowClass
					RPcell(3).Width = Unit.Pixel(100)
					RPcell(3).Text = rpItem.ProductName

					Dim offset As Integer = 0

					If cColl.CPFieldInfoItem.CP_UF1 > 0 Then
						RPcell(4 + offset) = New TableCell
						RPcell(4 + offset).CssClass = rowClass
						RPcell(4 + offset).Width = Unit.Pixel(70)
						RPcell(4 + offset).Text = rpItem.CPUF1_Value
						offset += 1
					End If

					If cColl.CPFieldInfoItem.CP_UF2 > 0 Then
						RPcell(4 + offset) = New TableCell
						RPcell(4 + offset).CssClass = rowClass
						RPcell(4 + offset).Width = Unit.Pixel(70)
						RPcell(4 + offset).Text = rpItem.CPUF2_Value
						offset += 1
					End If

					RPcell(4 + offset) = New TableCell
					RPcell(4 + offset).CssClass = rowClass
					RPcell(4 + offset).Width = Unit.Pixel(70)
					RPcell(4 + offset).HorizontalAlign = HorizontalAlign.Center
					RPcell(4 + offset).Text = rpItem.PaymentDate.ToShortDateString
					RPcell(5 + offset) = New TableCell
					RPcell(5 + offset).Width = Unit.Pixel(70)
					RPcell(5 + offset).CssClass = rowClass
                    RPcell(5 + offset).HorizontalAlign = HorizontalAlign.Right
                    Dim currency As cCurrency = cFWCurrencies.getCurrencyById(rpItem.CurrencyId)
                    RPcell(5 + offset).Text = cFWCurrencies.FormatCurrency(rpItem.Amount, currency, False) ' format as currency
					'RPcell(6 + offset) = New TableCell
					'RPcell(6 + offset).CssClass = rowClass
					'RPcell(6 + offset).HorizontalAlign = HorizontalAlign.Center
					'Dim chk As New CheckBox
					'chk.ID = "chk" & rpItem.PaymentId.ToString
					'chk.Checked = rpItem.IsOneOffCharge
					'chk.Enabled = False
					'RPcell(6 + offset).Controls.Add(chk)

					RProw.Cells.AddRange(RPcell)
					RP_Table.Rows.Add(RProw)

					hasdata = True
				Next

				If Not hasdata Then
					Dim emptyCell As New TableCell
					emptyCell.CssClass = "row1"
					emptyCell.ColumnSpan = 6 + headeroffset
					emptyCell.HorizontalAlign = HorizontalAlign.Center
					emptyCell.Text = "No recharge payments to display for specified date period"
					Dim emptyRow As New TableRow
					emptyRow.Cells.Add(emptyCell)
					RP_Table.Rows.Add(emptyRow)
				End If

				RPData.Controls.Add(RP_Table)

				Dim strCount As New StringBuilder

				strCount.Append("<table class=""datatbl""><tr><th>")
				If data_ceiling <> data.Count Then
					strCount.Append(data.Count.ToString & " items returned for display - ** Top " & data_ceiling.ToString & " only displayed **")
				Else
					strCount.Append(data.Count.ToString & " items returned for display")
				End If
				strCount.Append("</th></tr></table>" & vbNewLine)
				litReturnCount.Text = strCount.ToString

			Catch ex As Exception
				Dim errStr As New System.Text.StringBuilder

				With errStr
					.Append("<div class=""inputpanel"">" & vbNewLine)
					.Append("<table class=""datatbl"">" & vbNewLine)
					.Append("<tr>" & vbNewLine)
					.Append("<th>An error has occurred displaying Contract Recharge data</th>" & vbNewLine)
					.Append("</tr>" & vbNewLine)
					.Append("<tr>" & vbNewLine)
					.Append("<td class=""row1"">")
					.Append(ex.Message)
					.Append("</td>" & vbNewLine)
					.Append("</tr>" & vbNewLine)
					.Append("</table>" & vbNewLine)
					.Append("</div>" & vbNewLine)
					Dim litErr As New Literal
					litErr.Text = errStr.ToString
					RPData.Controls.Add(litErr)
				End With
			End Try
		End Sub

		Private Sub DeletePayment(ByVal db As cFWDBConnection, ByVal delete_rpId As Integer)
			Dim ARec As New cAuditRecord
			Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
			Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
			Dim curContractId As Integer = Session("ActiveContract")
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
			Dim employees As New cEmployees(curUser.Account.accountid)
			Dim CP_UF As New cCPFieldInfo(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid), curUser.Employee.employeeid, curContractId)

			Dim rPayments As New cRechargePaymentCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid), CP_UF, curContractId, New cProducts(curUser.Account.accountid, curUser.CurrentSubAccountId))
			Dim rPayment As cRechargePayment = rPayments.GetPaymentById(delete_rpId)
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.RechargePayments, curUser.CurrentSubAccountId)

			If Not rPayment Is Nothing Then
                db.FWDb("R", "contract_details", "ContractId", curContractId, "", "", "", "", "", "", "", "", "", "")
				'db.FWDb("R2", "contract_productdetails_recharge", "Recharge Item Id", delete_rpId)
                db.FWDb("D", "contract_productdetails_recharge", "RechargeItemId", delete_rpId, "", "", "", "", "", "", "", "", "", "")

				If db.FWDbFlag = True Then
                    ARec.ContractNumber = db.FWDbFindVal("ContractKey", 1)
					If ARec.ContractNumber = "" Then
                        ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 1)
					End If
				End If
				ARec.Action = cFWAuditLog.AUDIT_DEL
				ARec.DataElementDesc = "RECHARGE PAYMENTS"
				ARec.ElementDesc = rPayment.PaymentDate.ToShortDateString
				ARec.PreVal = rPayment.Amount.ToString()
				ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, delete_rpId)

				rPayments.DeleteRechargePayment(rPayment)
			End If
		End Sub

		Private Sub doUpdate()
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
				Dim db As New cFWDBConnection
				Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
				Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)
				Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
				db.DBOpen(fws, False)

                Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

				Dim employees As New cEmployees(curUser.Account.accountid)
				Dim cColl As New cCPFieldInfo(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr, curUser.Employee.employeeid, Session("ActiveContract"))
                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.RechargePayments, curUser.CurrentSubAccountId)
				Dim rPayments As New cRechargePaymentCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr, cColl, Session("ActiveContract"), New cProducts(curUser.Account.accountid, curUser.CurrentSubAccountId))

				Select Case Request.QueryString("action")
					Case "edit"
						Dim RP_id As Integer
						Dim firstpass As Boolean = True
						Dim ARec As New cAuditRecord
						Dim tmpStr, tmpStr2 As String

						RP_id = Request.QueryString("rpid")

                        Dim curPayment As cRechargePayment = rPayments.GetPaymentById(RP_id)
						Dim newpayment As cRechargePayment = curPayment

						ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        db.FWDb("R3", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
						If db.FWDb3Flag = True Then
							If db.FWDbFindVal("Contract Key", 3).Trim <> "" Then
                                ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3)
							Else
                                ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 3).Trim
							End If
						End If

						ARec.DataElementDesc = "RECHARGE PAYMENT"

						If curPayment.Amount <> CType(txtRechargeAmount.Text.Trim, Double) Then
							ARec.ElementDesc = "AMOUNT"
							ARec.PreVal = curPayment.Amount.ToString
							ARec.PostVal = txtRechargeAmount.Text
							ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, RP_id)

							db.SetFieldValue("Recharge Amount", txtRechargeAmount.Text, "N", firstpass)
							firstpass = False

							newpayment.Amount = Double.Parse(txtRechargeAmount.Text)
						End If

						If IsDate(curPayment.PaymentDate) = True Then
							tmpStr = curPayment.PaymentDate.ToShortDateString
						Else
							tmpStr = ""
						End If

						tmpStr2 = dateRechargePeriod.Text.Trim
						If IsDate(tmpStr2) = True Then
							If CDate(tmpStr2).Year > 1900 Then
								' not a blank year entry
								tmpStr2 = Format(CDate(tmpStr2), cDef.DATE_FORMAT)
							End If
						End If

						If tmpStr <> tmpStr2 Then
							ARec.ElementDesc = "RECHARGE PERIOD"
							ARec.PreVal = tmpStr
							ARec.PostVal = tmpStr2
							ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, RP_id)

                            db.SetFieldValue("RechargePeriod", dateRechargePeriod.Text, "D", firstpass)
							firstpass = False

							newpayment.PaymentDate = dateRechargePeriod.Value
						End If

						'If curPayment.IsOneOffCharge <> chkOneOff.Checked Then
						'    ARec.ElementDesc = "ONE OFF CHARGE"
						'    ARec.PreVal = IIf(curPayment.IsOneOffCharge, "CHECKED", "UNCHECKED")
						'    ARec.PostVal = IIf(chkOneOff.Checked, "CHECKED", "UNCHECKED")
						'    ALog.AddAuditRec( ARec,true)
						'    ALog.CommitAuditLog(uinfo)

						'    db.SetFieldValue("One Off Charge", IIf(chkOneOff.Checked, 1, 0), "N", firstpass)
						'    firstpass = False
						'End If

						If firstpass = False Then
                            db.FWDb("A", "contract_productdetails_recharge", "RechargeItemId", RP_id, "", "", "", "", "", "", "", "", "", "")

							rPayments.UpdateRechargePayment(newpayment)
						End If

					Case Else

				End Select

				db.DBClose()
				db = Nothing

				Response.Redirect("ContractRechargeData.aspx?id=" & Session("ActiveContract"), True)

			Catch ex As Exception
				System.Diagnostics.Debug.WriteLine(ex.Message)
			End Try
		End Sub

		Private Sub ShowEdit(ByVal db As cFWDBConnection, ByVal edit_rpId As Integer)
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
				Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
				Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
				Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)
				Dim employees As New cEmployees(curUser.Account.accountid)
				Dim cColl As New cCPFieldInfo(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr, curUser.Employee.employeeid, Session("ActiveContract"))

				Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr)
                Dim rs As cRechargeSetting = rscoll.getSettings

				Dim rPayments As New cRechargePaymentCollection(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr, cColl, Session("ActiveContract"), New cProducts(curUser.Account.accountid, curUser.CurrentSubAccountId))

				lblCustomer.Text = rs.ReferenceAs

				PopulateCustomerList(db)

				If edit_rpId > 0 Then
					Dim rPayment As cRechargePayment = rPayments.GetPaymentById(edit_rpId)

					txtRechargeAmount.Text = rPayment.Amount.ToString

					lstCustomer.Items.FindByValue(rPayment.EntityId).Selected = True
					lstCustomer.Enabled = False

					If IsDate(rPayment.PaymentDate) = True Then
						dateRechargePeriod.Value = rPayment.PaymentDate
					End If

					'chkOneOff.Checked = rPayment.IsOneOffCharge
				End If

			Catch ex As Exception
				System.Diagnostics.Debug.WriteLine(ex.Message)
			End Try
		End Sub

		Private Sub PopulateCustomerList(ByVal db As cFWDBConnection)
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
				Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
				Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
				Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)
				Dim clients As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, connStr)
                Dim slClients As SortedList = clients.GetClientsByLocation(curUser.CurrentSubAccountId)

				Dim cEnum As IDictionaryEnumerator = slClients.GetEnumerator
				While cEnum.MoveNext
					Dim curClient As cRechargeClient = CType(cEnum.Value, cRechargeClient)
					lstCustomer.Items.Add(New ListItem(curClient.ClientName, curClient.EntityId))
				End While
				lstCustomer.Items.Insert(0, New ListItem("[Please Select]", 0))

			Catch ex As Exception
				System.Diagnostics.Debug.WriteLine(ex.Message)
			End Try
		End Sub

		Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
			doCancel()
		End Sub

		Private Sub doCancel()
			Response.Redirect("ContractRechargeData.aspx?id=" & Session("ActiveContract"), True)
		End Sub

		Protected Sub cmdRefresh_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdRefresh.Click
			Session("DisplayRechargeDataFrom") = txtFromDate.Text.Trim
			Session("DisplayRechargeDataTo") = txtToDate.Text.Trim

			Response.Redirect("ContractRechargeData.aspx?id=" & Trim(Session("ActiveContract")), True)
		End Sub

		Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
			Response.Redirect("Home.aspx", True)
		End Sub

		Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdUpdate.Click
			doUpdate()
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

		Private Sub SetTabOptions(ByVal db As cFWDBConnection, ByVal fws As cFWSettings, ByVal uinfo As UserInfo)
			If Session("ActiveContract") Is Nothing Then
				Session("ActiveContract") = 0
			End If

			Dim omout As String = "window.status='Done';"
			Dim omover As String = "window.status='%msg%';return true;"

			If Session("ActiveContract") > 0 Then
				If uinfo.TabAccessList(CInt(SummaryTabs.ContractAdditional)) = False Then
					lnkCAnav.Visible = False
				Else
					lnkCAnav.ToolTip = "Display additional contract details"
					lnkCAnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCAnav.ToolTip))
					lnkCAnav.Attributes.Add("onmouseout", omout)
				End If

				If uinfo.TabAccessList(CInt(SummaryTabs.ContractProduct)) = False Then
					lnkCPnav.Visible = False
				Else
					lnkCPnav.ToolTip = "Display contract products associated with this contract"
					lnkCPnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCPnav.ToolTip))
					lnkCPnav.Attributes.Add("onmouseout", omout)
				End If

				If uinfo.TabAccessList(CInt(SummaryTabs.InvoiceDetail)) = False Then
					lnkIDnav.Visible = False
				Else
					lnkIDnav.ToolTip = "Display invoice details associated with the active contract"
					lnkIDnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkIDnav.ToolTip))
					lnkIDnav.Attributes.Add("onmouseout", omout)
				End If

				If uinfo.TabAccessList(CInt(SummaryTabs.InvoiceForecast)) = False Then
					lnkIFnav.Visible = False
				Else
					lnkIFnav.ToolTip = "Display forecast payments associated with the active contract"
					lnkIFnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkIFnav.ToolTip))
					lnkIFnav.Attributes.Add("onmouseout", omout)
				End If

				If uinfo.TabAccessList(CInt(SummaryTabs.NotesSummary)) = False Then
					lnkNSnav.Visible = False
				Else
					lnkNSnav.ToolTip = "Display collated summary of free text notes associated with the active contract"
					lnkNSnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNSnav.ToolTip))
					lnkNSnav.Attributes.Add("onmouseout", omout)
				End If

				lnkCHnav.ToolTip = "Display amendment history of the active contract"
				lnkCHnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCHnav.ToolTip))
				lnkCHnav.Attributes.Add("onmouseout", omout)

				If uinfo.TabAccessList(CInt(SummaryTabs.LinkedContracts)) = True Then
					If Session("ActiveContract") > 0 Then
                        db.FWDb("R2", "contract_details", "Contract Id", Session("ActiveContract"), "Location Id", uinfo.ActiveLocation, "", "", "", "", "", "", "", "")
						If db.FWDb2Flag = True Then
							' contract was found previously
							Dim Sql As New System.Text.StringBuilder
							Sql.Append("SELECT COUNT(*) AS [LnkCnt] FROM [link_matrix] ")
							Sql.Append("INNER JOIN [link_definitions] ON [link_definitions].[Link Id] = [link_matrix].[Link Id] ")
							Sql.Append("WHERE [link_definitions].[Is Schedule Link] = 0 AND [Contract Id] = @cId")
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

				If fws.glUseRechargeFunction = True And uinfo.TabAccessList(CInt(SummaryTabs.RechargeTemplate)) = True Then
                    Dim rscoll As New cRechargeSettings(fws.MetabaseCustomerId, uinfo.ActiveLocation, fws.getConnectionString)
                    Dim rs As cRechargeSetting = rscoll.getSettings

					If uinfo.TabAccessList(CInt(SummaryTabs.RechargePayments)) = True Then
						lnkRPnav.Visible = True
						lnkRPnav.ToolTip = "View recharge payments defined against contract products"
						lnkRPnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkRPnav.ToolTip))
						lnkRPnav.Attributes.Add("onmouseout", omout)
					End If

					If uinfo.TabAccessList(CInt(SummaryTabs.RechargeTemplate)) = True Then
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
    End Class
End Namespace
