Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class InvoiceForecastBU
        Inherits System.Web.UI.Page
        Protected WithEvents hypUpdate As New LinkButton
        Protected WithEvents cmdUpdate As New ImageButton

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
            Dim contractId, fEditId As Integer
			Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            If Me.IsPostBack = False Then
                db.DBOpen(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), False)
                fEditId = Integer.Parse(Request.Form("fid"))
                contractId = Integer.Parse(Request.Form("cid"))

                db.DBClose()
            End If

            'Select Case userinfo.ViewType
            '    Case ViewType.Basic
            '        hypUpdate.CssClass = "linkbutton"
            '        hypUpdate.Text = "Update"
            '        hypUpdate.ToolTip = "Apply current updated values to the selected invoices"
            '        hypUpdate.Attributes.Add("onmouseover", "window.status='Apply current updated values to the selected invoices';return true;")
            '        hypUpdate.Attributes.Add("onmouseout", "window.status='Done';")
            '        holderUpdateButton.Controls.Add(hypUpdate)

            '    Case ViewType.Enhanced, ViewType.Normal
            cmdUpdate.ImageUrl = "buttons/update.gif"
            cmdUpdate.ToolTip = "Apply current updated values to the selected invoices"
            cmdUpdate.Attributes.Add("onmouseover", "window.status='Apply current updated values to the selected invoices';return true;")
            cmdUpdate.Attributes.Add("onmouseout", "window.status='Done';")
            holderUpdateButton.Controls.Add(cmdUpdate)
            'End Select

            holderUpdateButton.Controls.Add(cmdUpdate)
            db = Nothing
        End Sub

    End Class

End Namespace
