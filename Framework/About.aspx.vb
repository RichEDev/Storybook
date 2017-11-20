Imports FWBase
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006

    Partial Class About
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
            Dim v As New System.Version(5, 24, 1)
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subAccs As New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subAccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            If Me.IsPostBack = False Then
                lblCurrentVersion.Text = "2010.3.1 (beta) (" & v.Major & "." & v.Minor & "." & v.Build & ")"

                Dim live_accounts As New cAccounts
                'ConfigurationManager.ConnectionStrings("metabase").ConnectionString)


				Dim db As New cFWDBConnection
                Dim licensedUsers As String = ""
                db.DBOpen(fws, False)
                Dim accProperties As cAccountProperties = subAccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
                'db.FWDb("R2", "system_parameters", "", "", "", "", "", "", "", "", "", "", "", "")


                'If tmpCode = "" Then
                '    licensedUsers = "0"
                'Else
                '    Dim crypt As New FWCrypt(FWCrypt.Providers.RC2)
                '    licensedUsers = crypt.Decrypt(db.FWDbFindVal("Licensed Users", 2), cDef.EncryptionKey)
                'End If
                licensedUsers = curUser.Account.LicencedUsers

                Dim concurrentUsers As New cConcurrentUsers(curUser.AccountID, curUser.EmployeeID)
                Dim concurrentCount As Integer = concurrentUsers.Count

                lblActiveUsers.Text = concurrentCount.ToString & " (" & CStr(CInt(licensedUsers) - concurrentCount) & " remaining)"

                Title = "About"
                Master.title = Title

                ' remove any contract lock that might exist in case they exit contractsummary.aspx via the About Framework link
                If Session("ActiveContract") > 0 Then
                    cLocks.RemoveLockItem(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.EmployeeID)
                    cLocks.RemoveLockItem(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.EmployeeID)
                End If

                db.DBClose()
                db = Nothing
            End If
        End Sub

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            Response.Redirect("Home.aspx", True)
        End Sub
    End Class

End Namespace
