Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class MaintCalc
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

            If Me.IsPostBack = False Then
                Dim id As Integer

                litScript.Text = GetScriptBlock()

                id =  Request.QueryString("id")
                If id = 0
                    id = Server.UrlDecode(Request.QueryString("id"))
                End If
                litCalculation.Text = Server.UrlDecode(Request.QueryString("calcText"))
            End If

            Dim strStyles As New StringBuilder
            'Dim clscolours As New FWClasses.cColours(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), Page.Theme)
            'strStyles.Append(clscolours.getStyleOverride(Request.ApplicationPath))
            strStyles.Append(Spend_Management.cColours.customiseStyles(False))
            'Dim styles As New cAppStyle(Request.ApplicationPath)
            'strStyles.Append(styles.GetGlobalStyles())
            litStyles.Text = strStyles.ToString
        End Sub

        Private Function GetScriptBlock() As String
            Dim strScript As New System.Text.StringBuilder
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            Dim xPos As Integer = 1024 '(uinfo.xMax / 100) * 30
            Dim yPos As Integer = 768 '(uinfo.yMax / 100) * 20

            With strScript
                .Append("<script language=""javascript"" type=""text/javascript"">" & vbNewLine)
                .Append("window.moveTo(" & xPos.ToString & "," & yPos.ToString & ");")
                .Append("window.resizeTo(500,600);" & vbNewLine)
                .Append("</script>" & vbNewLine)
            End With

            Return strScript.ToString
        End Function
    End Class
End Namespace
