Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.IO
Imports BusinessLogic
Imports BusinessLogic.DataConnections
Imports BusinessLogic.ProductModules
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management
Imports SpendManagementLibrary.Enumerators
Imports SpendManagementLibrary.Interfaces
Imports BusinessLogic.Modules

Partial Class FWMaster
    Inherits System.Web.UI.MasterPage
    Implements Interfaces.IMasterPage

    Private sTitle As String

    Private sHelpUrl As String
    Private bShowFWLogo As Boolean = True
    Private bShowDummyMenu As Boolean = False
    Private bShowGoHome As Boolean = True
    Private sOnloadfunc As String
    Private bHome As Boolean
    Private bStylesheet As Boolean = True
    Public sPage As String = ""
    Private bEnableNavigation As Boolean = True
    Private bUseCloseNavigationMsg As Boolean = False
    Private uReferer As Uri
    Private sStyle As String = ""
    Private bIsSubfolder As Boolean = False
    Private bIsExecutedReport As Boolean = False
    Private bIsContractScreen As Boolean = False

#Region "properties"
    Public Property isContractScreen() As Boolean
        Get
            Return bIsContractScreen
        End Get
        Set(ByVal value As Boolean)
            bIsContractScreen = value
        End Set
    End Property

    Public Property isExecutedReport() As Boolean
        Get
            Return bIsExecutedReport
        End Get
        Set(ByVal value As Boolean)
            bIsExecutedReport = value
        End Set
    End Property

    Public Property isSubFolder() As Boolean
        Get
            Return bIsSubfolder
        End Get
        Set(ByVal value As Boolean)
            bIsSubfolder = value
        End Set
    End Property

    Public Property addStyle() As String
        Get
            Return sStyle
        End Get
        Set(ByVal value As String)
            sStyle = value
        End Set
    End Property

    Public Property title() As String Implements IMasterPage.title
        Get
            Return sTitle
        End Get
        Set(ByVal value As String)
            sTitle = value
            litpagetitle.Text = value
        End Set
    End Property

    Public Property helpurl() As String
        Get
            Return sHelpUrl
        End Get
        Set(ByVal value As String)
            sHelpUrl = value
        End Set
    End Property

    Public Property showdummymenu() As Boolean Implements IMasterPage.showdummymenu
        Get
            Return bShowDummyMenu
        End Get
        Set(ByVal value As Boolean)
            bShowDummyMenu = value
        End Set
    End Property

    Public Property onloadfunc() As String Implements IMasterPage.onloadfunc
        Get
            Return sOnloadfunc
        End Get
        Set(ByVal value As String)
            sOnloadfunc = value
        End Set
    End Property

    Public Property ShowGoHome() As Boolean
        Get
            Return bShowGoHome
        End Get
        Set(ByVal value As Boolean)
            bShowGoHome = value
        End Set
    End Property

    Public Property stylesheet() As Boolean Implements IMasterPage.stylesheet
        Get
            Return bStylesheet
        End Get
        Set(ByVal value As Boolean)
            bStylesheet = value
        End Set
    End Property

    Public Property enablenavigation() As Boolean Implements IMasterPage.enablenavigation
        Get
            Return bEnableNavigation
        End Get
        Set(ByVal value As Boolean)
            bEnableNavigation = value
        End Set
    End Property

    Public Property ShowFWLogo() As Boolean
        Get
            Return bShowFWLogo
        End Get
        Set(ByVal value As Boolean)
            bShowFWLogo = value
        End Set
    End Property

    Public Property useCloseNavigationMsg() As Boolean
        Get
            Return bUseCloseNavigationMsg
        End Get
        Set(ByVal value As Boolean)
            bUseCloseNavigationMsg = value
        End Set
    End Property

    Private cFWS As cFWSettings
    Private Property fws() As cFWSettings
        Get
            Return cFWS
        End Get
        Set(ByVal value As cFWSettings)
            cFWS = value
        End Set
    End Property

    Public Property UseDynamicCSS As Boolean = False Implements IMasterPage.UseDynamicCSS

    Public Property helpid As Integer Implements IMasterPage.helpid
    Public Property home As Boolean Implements IMasterPage.home
    Public Property PageSubTitle As String Implements IMasterPage.PageSubTitle
    Public Property menutitle As String Implements IMasterPage.menutitle

#End Region

    ''' <summary>
    ''' An instance of <see cref="IDataFactory{IProductModule,Modules}"/> to get a <see cref="IProductModule"/>
    ''' </summary>
    <Dependency()> _
    Public Property ProductModuleFactory As IDataFactory(Of IProductModule, Modules)
        Get
        End Get
        Set
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Force IE out of compat mode
        Response.AddHeader("X-UA-Compatible", "IE=edge")
        'load Breadcrumbs when custom messages are set false
        If enablenavigation = True Then
            LoadBreadcrumbs()
        End If
        fws = New cFWSettings
        Dim curUser As CurrentUser = cMisc.GetCurrentUser()
        Dim theme As String = Me.Page.StyleSheetTheme
        Dim clsMasterPageMethods As New cMasterPageMethods(curUser, theme, Me.ProductModuleFactory)
        clsMasterPageMethods.PreventBrowserFromCachingTheResponse()
        clsMasterPageMethods.RedirectUserBypassingChangePassword()
        clsMasterPageMethods.UseDynamicCSS = UseDynamicCSS
        clsMasterPageMethods.SetupJQueryReferences(jQueryCss, ajaxScriptManager)
        clsMasterPageMethods.SetupSessionTimeoutReferences(ajaxScriptManager, Page)
        clsMasterPageMethods.SetupDynamicStyles(Head1)

        Dim productModule = Me.ProductModuleFactory(curUser.CurrentActiveModule)

        Page.Title = productModule.BrandName & " from Selenity Ltd."

        'Load data from user controls to setup the page 
        Dim litUser = TryCast(Me.side_bar.FindControl("lituser"), Literal)
        Dim litLogo = TryCast(Me.header.FindControl("litlogo"), Literal)
        clsMasterPageMethods.SetupCommonMaster(litUser, litLogo, favLink, litstyles, litemplogon, windowOnError)

        Me.litstyles.Text = cColours.customiseStyles(False)

        clsMasterPageMethods.AttachOnLoadAndUnLoads(body, onloadfunc)

        If Not curUser.CheckUserHasAccesstoWebsite() Then
            Response.Redirect(ErrorHandlerWeb.LogOut, True)
        End If


        If enablenavigation = False Then
            litok.Text = clsMasterPageMethods.DisableBreadCrumbsMessage
            sitemap.Visible = False
        End If

        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "reporttag", clsMasterPageMethods.SetMasterPageJavaScriptVars) '"var isSubFolder = " & bIsSubfolder.ToString.ToLower & "; var appPath = '" & cMisc.path & "'; theme = '" & Session("THEME") & "'; var accountid = " & curUser.AccountID & ";", True)
        Page.ClientScript.RegisterStartupScript(GetType(String), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(mdlMasterPopup))
    End Sub


    Public Sub RefreshBreadcrumbInfo()
        SetBreadcrumbInfo()
    End Sub

    Private Sub SetBreadcrumbInfo()
        If Not isExecutedReport Then
            If enablenavigation = False Then
                sitemap.Visible = False
                If Not bUseCloseNavigationMsg Then
                    litok.Text = "<ol class=""breadcrumb""><li>Please click Update at the bottom of the page to save your changes, otherwise, click Cancel</li></ol>"
                Else
                    litok.Text = "<ol class=""breadcrumb""><li>Please click Close at the bottom of the page to exit from the current page</li></ol>"
                End If
            Else
                sitemap.Visible = True
                litok.Text = ""
            End If
        Else
            sitemap.Visible = False
            litok.Text = "Report Viewer"
        End If
    End Sub


    Private Sub LoadBreadcrumbs()
        Dim currentNode As SiteMapNode = Web.SiteMap.CurrentNode
        Dim rootNode As SiteMapNode = Web.SiteMap.RootNode
        Dim breadCrumbs As New StringBuilder()
        Dim list = New List(Of String)()
        If currentNode IsNot Nothing AndAlso currentNode.Equals(rootNode) Then
            breadCrumbs.Append("<li class=""active""><a href=""#""><i><img src=""/static/images/expense/menu-icons/bradcrums-dashboard-icon.png"" alt=""""/></i> " + currentNode.Title + "</a></li>")
        ElseIf currentNode IsNot Nothing Then
            breadCrumbs.Append("<li><a href=""" + rootNode.Url + """><i><img src=""/static/images/expense/menu-icons/bradcrums-dashboard-icon.png"" alt=""""/></i> " + rootNode.Title + "</a></li>")

            While currentNode.Title <> rootNode.Title
                If currentNode Is Nothing Then
                    Exit While
                Else

                    list.Add("<li><a href=""" + currentNode.Url + """><label class=""breadcrumb_arrow"">/</label>" + currentNode.Title + "</a></li>")
                    currentNode = currentNode.ParentNode
                End If
            End While
            For i As Integer = list.Count - 1 To -1 + 1 Step -1
                breadCrumbs.Append(list(i))
            Next
        End If
        litBreadcrumb.Text = breadCrumbs.ToString()
    End Sub

End Class

