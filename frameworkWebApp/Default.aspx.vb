
Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.IsPostBack = False Then
            If User.Identity.IsAuthenticated Then
                Dim home As String = "~/Home.aspx"
                If Not ConfigurationManager.AppSettings("defaultHomePage") Is Nothing Then
                    home = ConfigurationManager.AppSettings("HomePageDefault")
                End If
                Response.Redirect(home, True)
            Else
                Response.Redirect("~/shared/logon.aspx", True)
            End If
        End If
    End Sub
End Class
