Imports FWBase
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class UserHelpPaths
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

			If IsPostBack = False Then
				Title = "User Help Setup"
				Master.title = Title

				Page.SetFocus(txtContractHelpDir)
				'txtContractHelpDir.Attributes.Add("OnLoad", "Form1.txtContractHelpDir.focus();")
				AccessHelpData()
			End If

			cmdOK.ToolTip = "Save User Help Fields"
			cmdOK.AlternateText = "Save"
		End Sub

		Private Sub AccessHelpData()
			Dim uhp As UserHelpDocs

			uhp = Session("UserHelpDocs")
			With uhp
				' contract fields
				txtContractHelpDir.Text = .ContractHelpDir
				txtSupercedes.Text = .Supercedes
				txtConType.Text = .ConType
				txtCategory.Text = .ConCategory
				txtConStatus.Text = .ConStatus
				txtOwner.Text = .ConOwner
				txtContractValue.Text = .ConValue
				txtAnnualValue.Text = .ConAnnualValue
				txtTermType.Text = .ConTermType
				txtInvFreq.Text = .ConInvFreq

				' contract additional
				txtContractAdditional.Text = .ContractAdditional

				' vendor fields
				txtVendorHelpDir.Text = .VendorHelpDir
				txtFinStatus.Text = .VenFinancialStatus
				txtVendorCategory.Text = .VenCategory
				txtVendorStatus.Text = .VenStatus
				txtVendorName.Text = .VenName
				txtCustNumber.Text = .VenCustomerNumber
				txtFinancialYE.Text = .VenFinancialYE

			End With
		End Sub

		Private Sub SaveAccessHelpData()
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection
			Dim fi, pd, uhd As String

			fi = "Field Identifier"
			pd = "Path Def"
			uhd = "user_helpdocs"

            db.DBOpen(fws, False)

			db.FWDb("D", uhd, "", "", "", "", "", "", "", "", "", "", "", "")

			db.SetFieldValue(fi, "ContractHelpDir", "S", True)
			txtContractHelpDir.Text = Replace(Trim(txtContractHelpDir.Text), "\", "/")

			If Trim(txtContractHelpDir.Text) <> "" Then
				If Right(Trim(txtContractHelpDir.Text), 1) <> "/" Then
					txtContractHelpDir.Text = Trim(txtContractHelpDir.Text) & "/"
				End If
				db.SetFieldValue(pd, Trim(txtContractHelpDir.Text), "S", False)
			Else
				db.SetFieldValue(pd, "./", "S", False)
			End If
			db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "") ' write with exec, no return dataset

			If Trim(txtSupercedes.Text) <> "" Then
				db.SetFieldValue(fi, "ContractSupercedes", "S", True)
				db.SetFieldValue(pd, Trim(txtSupercedes.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtConType.Text) <> "" Then
				db.SetFieldValue(fi, "ContractType", "S", True)
				db.SetFieldValue(pd, Trim(txtConType.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtCategory.Text) <> "" Then
				db.SetFieldValue(fi, "ContractCategory", "S", True)
				db.SetFieldValue(pd, Trim(txtCategory.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtConStatus.Text) <> "" Then
				db.SetFieldValue(fi, "ContractStatus", "S", True)
				db.SetFieldValue(pd, Trim(txtConStatus.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtOwner.Text) <> "" Then
				db.SetFieldValue(fi, "ContractOwner", "S", True)
				db.SetFieldValue(pd, Trim(txtOwner.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtContractValue.Text) <> "" Then
				db.SetFieldValue(fi, "ContractValue", "S", True)
				db.SetFieldValue(pd, Trim(txtContractValue.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtAnnualValue.Text) <> "" Then
				db.SetFieldValue(fi, "ContractAnnualValue", "S", True)
				db.SetFieldValue(pd, Trim(txtAnnualValue.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtTermType.Text) <> "" Then
				db.SetFieldValue(fi, "ContractTermType", "S", True)
				db.SetFieldValue(pd, Trim(txtTermType.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtInvFreq.Text) <> "" Then
				db.SetFieldValue(fi, "ContractInvFreq", "S", True)
				db.SetFieldValue(pd, Trim(txtInvFreq.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtContractAdditional.Text) <> "" Then
				db.SetFieldValue(fi, "ContractAdditional", "S", True)
				db.SetFieldValue(pd, Trim(txtContractAdditional.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			db.SetFieldValue(fi, "VendorHelpDir", "S", True)
			txtVendorHelpDir.Text = Replace(Trim(txtVendorHelpDir.Text), "\", "/")

			If Trim(txtVendorHelpDir.Text) <> "" Then
				If Right(Trim(txtVendorHelpDir.Text), 1) <> "/" Then
					txtVendorHelpDir.Text = Trim(txtVendorHelpDir.Text) & "/"
				End If
				db.SetFieldValue(pd, Trim(txtVendorHelpDir.Text), "S", False)
			Else
				db.SetFieldValue(pd, "./", "S", False)
			End If
			db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")

			If Trim(txtFinStatus.Text) <> "" Then
				db.SetFieldValue(fi, "VendorFinStatus", "S", True)
				db.SetFieldValue(pd, Trim(txtFinStatus.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtFinancialYE.Text) <> "" Then
				db.SetFieldValue(fi, "VendorFinYE", "S", True)
				db.SetFieldValue(pd, Trim(txtFinancialYE.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtVendorStatus.Text) <> "" Then
				db.SetFieldValue(fi, "VendorStatus", "S", True)
				db.SetFieldValue(pd, Trim(txtVendorStatus.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtVendorName.Text) <> "" Then
				db.SetFieldValue(fi, "VendorName", "S", True)
				db.SetFieldValue(pd, Trim(txtVendorName.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtCustNumber.Text) <> "" Then
				db.SetFieldValue(fi, "CustNumber", "S", True)
				db.SetFieldValue(pd, Trim(txtCustNumber.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			If Trim(txtCategory.Text) <> "" Then
				db.SetFieldValue(fi, "VendorCategory", "S", True)
				db.SetFieldValue(pd, Trim(txtCategory.Text), "S", False)
				db.FWDb("WX", uhd, "", "", "", "", "", "", "", "", "", "", "", "")
			End If

			lblStatus.Text = "User Help Documents Updated"
			db.DBClose()
			db = Nothing
		End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            SaveAccessHelpData()
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Response.Redirect("Home.aspx", True)
        End Sub
    End Class

End Namespace
