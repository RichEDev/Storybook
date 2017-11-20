Namespace Framework2006

Partial Class ErrorPage
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
            Dim brandName As String = "Framework"
            If System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("ApplicationInstanceName") Then
                brandName = System.Configuration.ConfigurationManager.AppSettings("ApplicationInstanceName")
            End If

            Title = brandName & " Error"
            Master.title = Title

            If Request.QueryString("errmsg") <> "" Then
                lblErrMsg.Text = Request.QueryString("errmsg")
            Else
                lblErrMsg.Text = Err.Source & ":" & Err.Number & " - " & Err.Description
            End If
        End Sub
End Class

End Namespace
