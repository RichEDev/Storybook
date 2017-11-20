<%@ Application Language="VB" %>

<script RunAt="server">  
    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        'System.Data.SqlClient.SqlDependency.Start(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)
        Dim crypt As cSecureData = New cSecureData()
        Dim sConnectionString As System.Data.SqlClient.SqlConnectionStringBuilder = New System.Data.SqlClient.SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)
        Dim sDBPassword = crypt.Decrypt(sConnectionString.Password)
        sConnectionString.Password = sDBPassword
        System.Data.SqlClient.SqlDependency.Start(sConnectionString.ToString())
        
        ' Set global variables for use in SML classes
        SpendManagementLibrary.GlobalVariables.MetabaseConnectionString = ConfigurationManager.ConnectionStrings("metabase").ConnectionString

        ' Caching of data
        Dim clsAccounts As SpendManagementLibrary.cAccounts = New SpendManagementLibrary.cAccounts()
        Dim clsTables As SpendManagementLibrary.cTables = New SpendManagementLibrary.cTables()
        Dim clsFields As SpendManagementLibrary.cFields = New SpendManagementLibrary.cFields()
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub
        
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        Dim exp As System.Web.HttpException
        Dim appinfo As System.Web.HttpApplication
        appinfo = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
        Dim sysError As New Spend_Management.cErrors()
        exp = CType(Server.GetLastError(), System.Exception)
        sysError.ReportError(exp)
                
    End Sub
    
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        
    End Sub

	Protected Sub Application_PreRequestHandlerExecute(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            If Not HttpContext.Current Is Nothing Then
                Dim p As Page = HttpContext.Current.Handler

                If Not p Is Nothing Then
                    Dim strTheme As String = "FrameworkTheme"
                    If Convert.ToString(HttpContext.Current.Session("THEME")) <> String.Empty Then
                        strTheme = Convert.ToString(HttpContext.Current.Session("THEME"))
                    End If

                    p.StyleSheetTheme = strTheme
                End If
            End If
        Catch ex As Exception

        End Try
	End Sub

    Protected Sub Application_BeginRequest(ByVal sender As Object, ByVal e As System.EventArgs)
        If ConfigurationManager.AppSettings("SSLRedirect") = "1" Then
            If Not Request.IsSecureConnection Then 'And (Request.Url.Host.ToLower().Contains("smartdiligence") Or Request.Url.Host.ToLower().Contains("sel-framework")) Then
                Dim domainStr As String = ConfigurationManager.AppSettings("SSLDomains")
                Dim domains As Array = domainStr.Split(",")
                Dim redir As Boolean = False
                
                For x As Integer = 0 To domains.Length - 1
                    If Request.Url.Host.ToLower().Contains(domains(x).ToString().ToLower().Trim()) Then
                        redir = True
                        Exit For
                    End If
                Next
                
                If redir Then
                    Response.Redirect("https://" & Request.Url.Host & Request.Url.AbsolutePath & Request.Url.Query, True)
                End If
            End If
        End If
    End Sub
</script>

