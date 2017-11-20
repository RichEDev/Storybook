Imports SpendManagementLibrary
Imports Spend_Management
Imports FWClasses

Namespace Framework2006

    Partial Class ContractProductInfo
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
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False, True)

			If Me.IsPostBack = False Then
				Try
					Master.onloadfunc = "Check4CloseForm();"
					Master.enablenavigation = False
					Master.useCloseNavigationMsg = Not curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False)

					Dim conProdId As Integer

					conProdId = Val(Request.QueryString("cpid"))

					If conProdId > 0 Then
						Dim db As New cFWDBConnection

						db.DBOpen(fws, False)

						Dim sql As New System.Text.StringBuilder

                        sql.Append("SELECT [contract_details].[contractDescription],[productDetails].[ProductName] FROM [contract_productdetails] ")
                        sql.Append("INNER JOIN [contract_details] ON [contract_details].[contractId] = [contract_productdetails].[contractId] ")
                        sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[productId] ")
                        sql.Append("WHERE [contract_productdetails].[contractProductId] = @conprodID")
						db.AddDBParam("conprodID", conProdId, True)
						db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

						Title = "Product Information"
						Master.title = Title

                        lblTitle.Text = " - " & db.GetFieldValue(db.glDBWorkA, "contractDescription", 0, 0) & " [" & db.GetFieldValue(db.glDBWorkA, "ProductName", 0, 0) & "]"

                        db.FWDb("R", "contract_productinformation", "contractProductId", conProdId, "", "", "", "", "", "", "", "", "", "")
                        If db.FWDbFlag = True Then
                            txtInfo.Text = db.FWDbFindVal("Information", 1)
                        Else
                            txtInfo.Text = ""
                            lblErrorMsg.Text = "No additional information defined."
                        End If

						db.DBClose()
						db = Nothing
					Else
						lblErrorMsg.Text = "Product to display information for is unknown"
					End If

				Catch ex As Exception
					lblErrorMsg.Text = "** ERROR ON PAGE **"
					txtInfo.Text = ex.Message
				End Try
			End If

			cmdOK.AlternateText = "Update"
			cmdOK.ToolTip = "Save any changes made and exit screen"
			cmdOK.Attributes.Add("onmouseover", "window.status='Save any changes made and exit screen';return true;")
			cmdOK.Attributes.Add("onmouseout", "window.status='Done';")
			cmdOK.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False)

			If Not curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False) Then
				cmdCancel.ImageUrl = "~/buttons/page_close.gif"
			End If

			If Mid(txtInfo.Text, 1, 2) = "**" Then
				cmdOK.Enabled = False
			End If
		End Sub

		Private Sub DoUpdate()
			Try
				Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
				Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
				Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
				Dim db As New cFWDBConnection
				Dim conProdId As Integer
				Dim ARec As New cAuditRecord
				Dim sql As New System.Text.StringBuilder
                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractProducts, curUser.CurrentSubAccountId)

				db.DBOpen(fws, False)

				conProdId = Val(Request.QueryString("cpid"))

				db.SetFieldValue("Information", txtInfo.Text.Trim, "S", True)
                db.SetFieldValue("contractProductId", conProdId, "N", False)

                db.FWDb("R2", "contract_productinformation", "contractProductId", conProdId, "", "", "", "", "", "", "", "", "", "")
				If db.FWDb2Flag = True Then
                    db.FWDb("A", "contract_productinformation", "contractProductId", conProdId, "", "", "", "", "", "", "", "", "", "")
					ARec.Action = cFWAuditLog.AUDIT_UPDATE
				Else
					db.FWDb("W", "contract_productinformation", "", "", "", "", "", "", "", "", "", "", "", "")
					ARec.Action = cFWAuditLog.AUDIT_ADD
				End If

                sql.Append("SELECT [productDetails].[ProductName],[contract_details].[contractKey] FROM [contract_productinformation] ")
                sql.Append("INNER JOIN [contract_productdetails] ON [contract_productdetails].[contractProductId] = [contract_productinformation].[contractProductId] ")
                sql.Append("INNER JOIN [contract_details] ON [contract_details].[contractId] = [contract_productdetails].[contractId] ")
                sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[productId] ")
                sql.Append("WHERE [contract_productdetails].[contractProductId] = @ConProdId")
				db.AddDBParam("ConProdId", conProdId, True)
				db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

				If db.glNumRowsReturned > 0 Then
                    If IsDBNull(db.GetFieldValue(db.glDBWorkA, "contractKey", 0, 0)) = True Then
                        ARec.ContractNumber = ""
                    Else
                        ARec.ContractNumber = db.GetFieldValue(db.glDBWorkA, "contractKey", 0, 0)
                    End If
                    If IsDBNull(db.GetFieldValue(db.glDBWorkA, "ProductName", 0, 0)) = True Then
                        ARec.DataElementDesc = "PROD ???"
                    Else
                        ARec.DataElementDesc = "PROD" & db.GetFieldValue(db.glDBWorkA, "ProductName", 0, 0)
                    End If
				Else
					ARec.ContractNumber = ""
					ARec.DataElementDesc = ""
				End If

				ARec.ElementDesc = "CON PROD INFO"
				ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, conProdId)

				db.DBClose()
				db = Nothing

				Response.Redirect("ContractProductInfo.aspx?action=close", True)
			Catch ex As Exception

			End Try
		End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            DoUpdate()
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Response.Redirect("ContractProductInfo.aspx?action=close", True)
        End Sub
    End Class
End Namespace
