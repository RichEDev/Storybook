Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class RechargeEntityDefinitions
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
            Try
                Dim action As String
                Dim entityID As Integer
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
				Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
				Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
				Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
                Dim rs As cRechargeSetting = rscoll.getSettings

				If Me.IsPostBack = False Then
					Dim db As New cFWDBConnection

					action = Request.QueryString("action")
					db.DBOpen(fws, False)

					lblTitle.Text = "Recharge Entity Definition"
					Title = lblTitle.Text
					Master.title = Title

					lblCustomer.Text = rs.ReferenceAs
					lblCode.Text = rs.ReferenceAs & " Code"

					populateDDLists(db)

					Select Case action
						Case "add"

						Case "edit"
							' retrieve details of the entity for editing
							entityID = Val(Request.QueryString("id"))

							GetEntityDetails(db, entityID)
					End Select

					db.DBClose()
					db = Nothing
				End If

				cmdCancel.AlternateText = "Cancel"
				cmdCancel.ToolTip = "Return to the definition list page with saving any changes"
				cmdCancel.Attributes.Add("onmouseover", "window.status='Return to the definition list page with saving any changes';return true;")
				cmdCancel.Attributes.Add("onmouseout", "window.status='Done';")

				cmdOK.AlternateText = "Update"
				cmdOK.ToolTip = "Save to database"
				cmdOK.Attributes.Add("onmouseover", "window.status='Save details to the database';return true;")
				cmdOK.Attributes.Add("onmouseout", "window.status='Done';")

			Catch ex As Exception

			End Try
		End Sub

		Private Sub GetEntityDetails(ByVal db As cFWDBConnection, ByVal entityID As Integer)
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
				Dim clients As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
				Dim client As cRechargeClient = clients.GetClientById(entityID)

				If Not client Is Nothing Then
					txtCustomer.Text = client.ClientName
					chkShared.Checked = client.isShared
					If client.StaffRepId > 0 Then
						lstRepresentative.Items.FindByValue(client.StaffRepId).Selected = True
					End If
					If client.DeputyRepId > 0 Then
						lstDeputyRep.Items.FindByValue(client.DeputyRepId).Selected = True
					End If
					If client.AccountManagerId > 0 Then
						lstAccountMgr.Items.FindByValue(client.AccountManagerId).Selected = True
					End If
					If client.ServiceManagerId > 0 Then
						lstServiceMgr.Items.FindByValue(client.ServiceManagerId).Selected = True
					End If
					txtNotes.Text = client.Notes
					chkClosed.Checked = client.isClosed
					If client.DateClosed <> CDate("01/01/1900") Then
						meDateClosed.Value = client.DateClosed
					End If

					If client.DateCeased <> CDate("01/01/1900") Then
						meDateCeased.Value = client.DateCeased
					End If
					txtCode.Text = client.Code
					txtServiceLine.Text = client.ServiceLine
					txtSector.Text = client.Sector
				End If

			Catch ex As Exception

			End Try
		End Sub

		Private Sub populateDDLists(ByVal db As cFWDBConnection)
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
                'Dim sql As String

                '            sql = "SELECT [employeeId],[emName] FROM [staff_details] WHERE [Location Id] = @locId"
                'db.AddDBParam("locId", curUser.CurrentSubAccountId.Value, True)
                'db.RunSQL(sql, db.glDBWorkA, False, "", False)

				Dim am As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
				lstAccountMgr.Items.Clear()
				lstAccountMgr.Items.AddRange(am.GetListControlItems(True, True))
				'lstAccountMgr.DataSource = db.glDBWorkA
				'lstAccountMgr.DataTextField = "Staff Name"
				'lstAccountMgr.DataValueField = "Staff Id"
				'lstAccountMgr.DataBind()
				'lstAccountMgr.Items.Insert(0, New ListItem("[None]", 0))

				Dim rep As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
				lstRepresentative.Items.Clear()
				lstRepresentative.Items.AddRange(rep.GetListControlItems(True, True))
				'lstRepresentative.DataSource = db.glDBWorkA
				'lstRepresentative.DataTextField = "Staff Name"
				'lstRepresentative.DataValueField = "Staff Id"
				'lstRepresentative.DataBind()
				'lstRepresentative.Items.Insert(0, New ListItem("[None]", 0))

				Dim drep As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
				lstDeputyRep.Items.Clear()
				lstDeputyRep.Items.AddRange(drep.GetListControlItems(True, True))
				'lstDeputyRep.DataSource = db.glDBWorkA
				'lstDeputyRep.DataTextField = "Staff Name"
				'lstDeputyRep.DataValueField = "Staff Id"
				'lstDeputyRep.DataBind()
				'lstDeputyRep.Items.Insert(0, New ListItem("[None]", 0))

				Dim sm As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
				lstServiceMgr.Items.Clear()
				lstServiceMgr.Items.AddRange(sm.GetListControlItems(True, True))
				'lstServiceMgr.DataSource = db.glDBWorkA
				'lstServiceMgr.DataTextField = "Staff Name"
				'lstServiceMgr.DataValueField = "Staff Id"
				'lstServiceMgr.DataBind()
				'lstServiceMgr.Items.Insert(0, New ListItem("[None]", 0))

			Catch ex As Exception

			End Try
		End Sub

		Private Sub doUpdate()
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
				Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
				Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
				Dim db As New cFWDBConnection
				Dim ARec As New cAuditRecord
				Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
                Dim rs As cRechargeSetting = rscoll.getSettings
				Dim clients As New cRechargeClientList(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
				Dim success, firstpass As Boolean
				Dim tmpStr As String
                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.RechargeClients, curUser.CurrentSubAccountId)

				success = False

				db.DBOpen(fws, False)

				ARec.DataElementDesc = "RECHARGE " & rs.ReferenceAs.ToUpper

				Select Case Request.QueryString("action")
					Case "edit"
						ARec.Action = cFWAuditLog.AUDIT_UPDATE

						' get existing values from the database
						Dim itemID As Integer
						itemID = Val(Request.QueryString("id"))

						Dim client As cRechargeClient = clients.GetClientById(itemID)
						If Not client Is Nothing Then
							' compare to see if element has been changed
							firstpass = True

							tmpStr = client.ClientName
							If tmpStr <> Trim(txtCustomer.Text) Then
								db.SetFieldValue("Name", Trim(txtCustomer.Text), "S", firstpass)
								firstpass = False

								ARec.ElementDesc = "ENTITY NAME"
								ARec.PreVal = tmpStr
								ARec.PostVal = Trim(txtCustomer.Text)
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = IIf(client.isShared, "1", "0")
							If tmpStr <> IIf(chkShared.Checked, "1", "0") Then
								db.SetFieldValue("Shared", IIf(chkShared.Checked, 1, 0), "N", firstpass)
								firstpass = False

								ARec.ElementDesc = "SHARED"
								ARec.PreVal = IIf(tmpStr = "1", "CHECKED", "UNCHECKED")
								ARec.PostVal = IIf(chkShared.Checked, "CHECKED", "UNCHECKED")
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = client.StaffRepId.ToString
							If Integer.Parse(tmpStr) <> lstRepresentative.SelectedItem.Value Then
								If lstRepresentative.SelectedItem.Value = "0" Then
									db.SetFieldValue("Staff Rep", DBNull.Value, "#", firstpass)
								Else
									db.SetFieldValue("Staff Rep", lstRepresentative.SelectedItem.Value, "N", firstpass)
								End If
								firstpass = False

								ARec.ElementDesc = rs.ReferenceAs & " " & rs.StaffRepAs
								If client.StaffRepId > 0 Then
									ARec.PreVal = lstRepresentative.Items.FindByValue(tmpStr).Text
								End If
								ARec.PostVal = lstRepresentative.SelectedItem.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = client.DeputyRepId.ToString
							If Integer.Parse(tmpStr) <> lstDeputyRep.SelectedItem.Value Then
								If lstDeputyRep.SelectedItem.Value = "0" Then
									db.SetFieldValue("Deputy Rep", DBNull.Value, "#", firstpass)
								Else
									db.SetFieldValue("Deputy Rep", lstDeputyRep.SelectedItem.Value, "N", firstpass)
								End If
								firstpass = False

								ARec.ElementDesc = "DEPUTY " & rs.StaffRepAs
								If client.DeputyRepId > 0 Then
									ARec.PreVal = lstDeputyRep.Items.FindByValue(tmpStr).Text
								End If
								ARec.PostVal = lstDeputyRep.SelectedItem.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = client.AccountManagerId
							If Integer.Parse(tmpStr) <> lstAccountMgr.SelectedItem.Value Then
								If lstAccountMgr.SelectedItem.Value = "0" Then
									db.SetFieldValue("Account Mgr", DBNull.Value, "#", firstpass)
								Else
									db.SetFieldValue("Account Mgr", lstAccountMgr.SelectedItem.Value, "N", firstpass)
								End If
								firstpass = False

								ARec.ElementDesc = "ACCOUNT MGR"
								If client.AccountManagerId > 0 Then
									ARec.PreVal = lstAccountMgr.Items.FindByValue(tmpStr).Text
								End If
								ARec.PostVal = lstAccountMgr.SelectedItem.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = client.ServiceManagerId.ToString
							If Integer.Parse(tmpStr) <> lstServiceMgr.SelectedItem.Value Then
								If lstServiceMgr.SelectedItem.Value = "0" Then
									db.SetFieldValue("Service Mgr", DBNull.Value, "#", firstpass)
								Else
									db.SetFieldValue("Service Mgr", lstServiceMgr.SelectedItem.Value, "N", firstpass)
								End If
								firstpass = False

								ARec.ElementDesc = "SERVICE MGR"
								If client.ServiceManagerId > 0 Then
									ARec.PreVal = lstServiceMgr.Items.FindByValue(tmpStr).Text
								End If
								ARec.PostVal = lstServiceMgr.SelectedItem.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = IIf(client.isClosed, "1", "0")
							If tmpStr <> IIf(chkClosed.Checked, "1", "0") Then
								db.SetFieldValue("Closed", IIf(chkClosed.Checked, 1, 0), "N", firstpass)
								firstpass = False

								ARec.ElementDesc = "CLOSED"
								ARec.PreVal = IIf(tmpStr = "1", "CHECKED", "UNCHECKED")
								ARec.PostVal = IIf(chkClosed.Checked, "CHECKED", "UNCHECKED")
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = client.Notes
							If tmpStr <> Trim(txtNotes.Text) Then
								db.SetFieldValue("Notes", Trim(txtNotes.Text), "S", firstpass)
								firstpass = False

								ARec.ElementDesc = "NOTES"
								ARec.PreVal = Mid(tmpStr, 1, 60)
								ARec.PostVal = Mid(Trim(txtNotes.Text), 1, 60)
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							If client.DateClosed <> CDate("01/01/1900") Then
								tmpStr = Format(client.DateClosed, cDef.DATE_FORMAT)
							Else
								tmpStr = ""
							End If

							If tmpStr <> meDateClosed.Text Then
								db.SetFieldValue("Date Closed", Trim(meDateClosed.Text), "D", firstpass)
								firstpass = False

								ARec.ElementDesc = "DATE CLOSED"
								ARec.PreVal = tmpStr
								ARec.PostVal = meDateClosed.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							If client.DateCeased <> CDate("01/01/1900") Then
								tmpStr = Format(client.DateCeased, cDef.DATE_FORMAT)
							Else
								tmpStr = ""
							End If

							If tmpStr <> meDateCeased.Text Then
								db.SetFieldValue("Date Ceased", Trim(meDateCeased.Text), "D", firstpass)
								firstpass = False

								ARec.ElementDesc = "DATE CEASED"
								ARec.PreVal = tmpStr
								ARec.PostVal = meDateCeased.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = client.Sector
							If tmpStr <> txtSector.Text Then
								db.SetFieldValue("Sector", Trim(txtSector.Text), "S", firstpass)
								firstpass = False

								ARec.ElementDesc = "SECTOR"
								ARec.PreVal = tmpStr
								ARec.PostVal = txtSector.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = client.Code
							If tmpStr <> txtCode.Text Then
								db.SetFieldValue("Code", Trim(txtCode.Text), "S", firstpass)
								firstpass = False

								ARec.ElementDesc = "CODE"
								ARec.PreVal = tmpStr
								ARec.PostVal = txtCode.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							tmpStr = client.ServiceLine
							If tmpStr <> txtServiceLine.Text Then
								db.SetFieldValue("Service Line", Trim(txtServiceLine.Text), "S", firstpass)
								firstpass = False

								ARec.ElementDesc = "SERVICE LINE"
								ARec.PreVal = tmpStr
								ARec.PostVal = txtServiceLine.Text
								ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, itemID)
							End If

							If firstpass = False Then
								db.FWDb("A", "codes_rechargeentity", "Entity Id", itemID, "", "", "", "", "", "", "", "", "", "")
                                Dim newrc As New cRechargeClient(curUser.CurrentSubAccountId, itemID, txtCustomer.Text, chkShared.Checked, CInt(lstRepresentative.SelectedItem.Value), CInt(lstDeputyRep.SelectedItem.Value), CInt(lstAccountMgr.SelectedItem.Value), CInt(lstServiceMgr.SelectedItem.Value), txtNotes.Text, chkClosed.Checked, meDateClosed.Date, meDateCeased.Date, txtCode.Text, txtServiceLine.Text, txtSector.Text)
								clients.UpdateClient(itemID, newrc)
							End If
							success = True
						Else
							' error, cannot find the original
							lblError.Text = "ERROR: Cannot find original entity for update"
							Exit Select
						End If

					Case Else
						' must be adding a new one, so check that the name given is unique
						Dim client As cRechargeClient = clients.FindClientByName(txtCustomer.Text.Trim)

						If client.EntityId > 0 Then
							lblError.Text = "ERROR: " & rs.ReferenceAs & " definition supplied is not unique."
							Exit Select
						End If

						ARec.Action = cFWAuditLog.AUDIT_ADD
						ARec.PostVal = Trim(txtCustomer.Text)
						ARec.ElementDesc = "NEW ENTITY"

                        db.SetFieldValue("subAccountId", curUser.CurrentSubAccountId, "N", True)
						db.SetFieldValue("Name", Trim(txtCustomer.Text), "S", False)
						db.SetFieldValue("Code", Trim(txtCode.Text), "S", False)
						db.SetFieldValue("Shared", IIf(chkShared.Checked, 1, 0), "N", False)
						If lstRepresentative.SelectedValue <> "0" Then
                            db.SetFieldValue("StaffRep", lstRepresentative.SelectedValue, "N", False)
						Else
                            db.SetFieldValue("StaffRep", DBNull.Value, "#", False)
						End If
						If lstDeputyRep.SelectedValue <> "0" Then
                            db.SetFieldValue("DeputyRep", lstDeputyRep.SelectedValue, "N", False)
						Else
                            db.SetFieldValue("DeputyRep", DBNull.Value, "#", False)
						End If
						If lstAccountMgr.SelectedValue <> "0" Then
                            db.SetFieldValue("AccountMgr", lstAccountMgr.SelectedValue, "N", False)
						Else
                            db.SetFieldValue("AccountMgr", DBNull.Value, "#", False)
						End If
						If lstServiceMgr.SelectedValue <> "0" Then
                            db.SetFieldValue("ServiceMgr", lstServiceMgr.SelectedValue, "N", False)
						Else
                            db.SetFieldValue("ServiceMgr", DBNull.Value, "#", False)
						End If
                        db.SetFieldValue("ServiceLine", Trim(txtServiceLine.Text), "S", False)
						db.SetFieldValue("Sector", txtSector.Text, "S", False)
						db.SetFieldValue("Closed", IIf(chkClosed.Checked, 1, 0), "N", False)
						db.SetFieldValue("Notes", Trim(txtNotes.Text), "S", False)
                        db.SetFieldValue("DateClosed", meDateClosed.Text, "D", False)
                        db.SetFieldValue("DateCeased", meDateCeased.Text, "D", False)
						db.FWDb("W", "codes_rechargeentity", "", "", "", "", "", "", "", "", "", "", "", "")
						If db.glIdentity > 0 Then
                            Dim newrc As New cRechargeClient(curUser.CurrentSubAccountId, db.glIdentity, txtCustomer.Text, chkShared.Checked, CInt(lstRepresentative.SelectedItem.Value), CInt(lstDeputyRep.SelectedItem.Value), CInt(lstAccountMgr.SelectedItem.Value), CInt(lstServiceMgr.SelectedItem.Value), txtNotes.Text, chkClosed.Checked, meDateClosed.Date, meDateCeased.Date, txtCode.Text, txtServiceLine.Text, txtSector.Text)

							clients.AddClient(db.glIdentity, newrc)
						End If
						success = True

						ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, client.EntityId)
				End Select

				db.DBClose()
				db = Nothing

				If success = True Then
					Response.Redirect("BaseDefinitions.aspx?item=rechargeentity", True)
				End If

			Catch ex As Exception

			End Try
		End Sub

        Private Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            doUpdate()
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Response.Redirect("BaseDefinitions.aspx?item=rechargeentity", True)
        End Sub
    End Class
End Namespace
