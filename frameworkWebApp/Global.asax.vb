Imports System.Web.Optimization
Imports SimpleInjector
Imports SpendManagementLibrary
Imports Spend_Management
Imports WebBootstrap

<assembly: PreApplicationStartMethod(GetType(PageInitializerModule), "Initialize")>
Partial Public Class [Global]
    Inherits HttpApplication

    Dim Shared container As Container

    Public Shared Sub InitializeHandler(ByVal handler As IHttpHandler)
        If (TypeOf handler Is Page) Then
            InitializePage(CType(handler,Page))
        End If
    End Sub
    
    Private Shared Sub InitializePage(ByVal page As Page)
        container.GetRegistration(page.GetType, true).Registration.InitializeInstance(page)

        AddHandler page.InitComplete, New EventHandler(Sub(sender As Object, e As EventArgs)
            Dim control As Control = page
            InitializeControl(control)
        End Sub)
    End Sub

    Public Shared Sub InitializeControl(ByVal control As Control)
        If (TypeOf control Is UserControl) Then
            container.GetRegistration(control.GetType, true).Registration.InitializeInstance(control)
        End If
        
        For Each child As Control In control.Controls
            InitializeControl(child)
        Next
    End Sub

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)

        container = Bootstraper.Bootstrap()

        FunkyInjector.Container = container

        If (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers")) Then
            Dim crypt As cSecureData = New cSecureData()
            Dim sConnectionString As System.Data.SqlClient.SqlConnectionStringBuilder = New System.Data.SqlClient.SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)
            sConnectionString.Password = crypt.Decrypt(sConnectionString.Password)
            System.Data.SqlClient.SqlDependency.Start(sConnectionString.ToString())
        End If

        ' Set global variables for use in SML classes
        Dim gv As New SpendManagementLibrary.GlobalVariables(SpendManagementLibrary.GlobalVariables.ApplicationType.Web)

        Dim enableOptimisations As Boolean
        If Boolean.TryParse(ConfigurationManager.AppSettings("BundlingEnableOptimisations"), enableOptimisations) Then
            BundleTable.EnableOptimizations = enableOptimisations
        Else
            BundleTable.EnableOptimizations = True
        End If
        Spend_Management.App_Start.BundleConfig.RegisterBundles(BundleTable.Bundles)

        ' Caching of data
        Dim cAccountsCached As Boolean = New SpendManagementLibrary.cAccounts().CacheList()
        Dim clsTables As SpendManagementLibrary.cTables = New SpendManagementLibrary.cTables()
        Dim clsFields As SpendManagementLibrary.cFields = New SpendManagementLibrary.cFields()
        HostManager.SetHostInformation()
        AutomapperConfig.Configure()
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        Dim exp As System.Web.HttpException
        Dim appinfo As System.Web.HttpApplication
        appinfo = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
        exp = CType(Server.GetLastError(), System.Exception)

        Dim errorHandler As Spend_Management.ErrorHandlerWeb = New Spend_Management.ErrorHandlerWeb()
        errorHandler.SendError(Spend_Management.cMisc.GetCurrentUser(), exp)
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
                    p.StyleSheetTheme = HostManager.GetTheme(Request.Url.Host, "FrameworkTheme")
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Application_AcquireRequestState(sender As Object, e As EventArgs)

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
End Class