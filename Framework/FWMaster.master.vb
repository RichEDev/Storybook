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
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Partial Class FWMaster
    Inherits System.Web.UI.MasterPage

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

    Public Property title() As String
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

    Public Property showdummymenu() As Boolean
        Get
            Return bShowDummyMenu
        End Get
        Set(ByVal value As Boolean)
            bShowDummyMenu = value
        End Set
    End Property

    Public Property onloadfunc() As String
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

    Public Property stylesheet() As Boolean
        Get
            Return bStylesheet
        End Get
        Set(ByVal value As Boolean)
            bStylesheet = value
        End Set
    End Property

    Public Property enablenavigation() As Boolean
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
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        fws = New cFWSettings
        Dim clsModules As New cModules

        Dim curUser As CurrentUser = cMisc.getCurrentUser()
        Dim clsMasterPageMethods As New cMasterPageMethods(curUser)

        Dim clsModule As cModule = clsModules.GetModuleByID(CInt(curUser.CurrentActiveModule))
        Page.Title = clsModule.BrandNamePlainText & " from Software (Europe) Ltd."

        clsMasterPageMethods.SetupCommonMaster(lituser, litlinks, litlogo, favLink, litstyles, litemplogon)
        clsMasterPageMethods.AttachOnLoadAndUnLoads(body, onloadfunc)

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
                    litok.Text = "Please click Update at the bottom of the page to save your changes, otherwise, click Cancel"
                Else
                    litok.Text = "Please click Close at the bottom of the page to exit from the current page"
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
End Class

