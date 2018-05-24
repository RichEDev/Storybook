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
Imports Spend_Management.shared.code.GreenLight
Imports SpendManagementLibrary.Interfaces
Imports SpendManagementLibrary.Enumerators
Imports BusinessLogic.Modules

Partial Class FWMenu
    Inherits System.Web.UI.MasterPage
    Implements IMasterPage

    Private arrMenuitems As ArrayList = New ArrayList()

    Private sMenuTitle As String
    Private sTitle As String
    Private sHelpUrl As String
    Private bSingleColumnMenu As Boolean = False
    Private bShowFWLogo As Boolean = True
    Private sOnloadfunc As String
    Private bHome As Boolean
    Private hovercolour As String = ""
    Public sPage As String
    Private sStyle As String = ""
    Private misCurIconSize As fwIconSize = fwIconSize.Large
    Private bIsReports As Boolean = False

#Region "properties"
    Public Property isReports() As Boolean
        Get
            Return bIsReports
        End Get
        Set(ByVal value As Boolean)
            bIsReports = value
        End Set
    End Property

    Public Property iconSize() As fwIconSize
        Get
            Return misCurIconSize
        End Get
        Set(ByVal IconSize As fwIconSize)
            misCurIconSize = IconSize
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

    Public Property menutitle() As String Implements IMasterPage.menutitle
        Get
            Return sMenuTitle
        End Get
        Set(ByVal value As String)
            sMenuTitle = value
        End Set
    End Property

    Public ReadOnly Property menuitems() As ArrayList
        Get
            Return arrMenuitems
        End Get
    End Property

    Public Property title() As String Implements IMasterPage.title
        Get
            Return sTitle
        End Get
        Set(ByVal value As String)
            sTitle = value
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

    Public Property showsinglecolumnmenu() As Boolean
        Get
            Return bSingleColumnMenu
        End Get
        Set(ByVal value As Boolean)
            bSingleColumnMenu = value
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

    Public Property home() As Boolean Implements IMasterPage.home
        Get
            Return bHome
        End Get
        Set(ByVal value As Boolean)
            bHome = value
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

    Public Property UseDynamicCSS As Boolean = False Implements IMasterPage.UseDynamicCSS

    Public Property helpid As Integer Implements IMasterPage.helpid
    Public Property PageSubTitle As String Implements IMasterPage.PageSubTitle
    Public Property enablenavigation As Boolean Implements IMasterPage.enablenavigation
    Public Property showdummymenu As Boolean Implements IMasterPage.showdummymenu
    Public Property stylesheet As Boolean Implements IMasterPage.stylesheet

#End Region

    ''' <summary>
    ''' An instance of <see cref="IDataFactory{IProductModule,Modules}"/> to get a <see cref="IProductModule"/>
    ''' </summary>
    Dim ReadOnly _productModuleFactory As IDataFactory(Of IProductModule, Modules) = (FunkyInjector.Container.GetInstance(GetType(IDataFactory(Of IProductModule,Modules))))

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Force IE out of compat mode
        Response.AddHeader("X-UA-Compatible", "IE=edge")
        Dim currentUser As CurrentUser = cMisc.GetCurrentUser()
        Dim theme As String = Me.Page.StyleSheetTheme
        Dim clsMasterPageMethods As New cMasterPageMethods(currentUser, theme, Me._productModuleFactory)
        clsMasterPageMethods.PreventBrowserFromCachingTheResponse()
        clsMasterPageMethods.RedirectUserBypassingChangePassword()
        clsMasterPageMethods.UseDynamicCSS = UseDynamicCSS
        clsMasterPageMethods.SetupJQueryReferences(jQueryCss, scriptman)
        clsMasterPageMethods.SetupSessionTimeoutReferences(scriptman, Page)
        clsMasterPageMethods.SetupDynamicStyles(Head1)

        If Me.IsPostBack = False Then

            Dim lituser = TryCast(Me.side_bar.FindControl("litUser"), Literal)
            Dim litlogo = TryCast(Me.header.FindControl("litLogo"), Literal)
            clsMasterPageMethods.SetupCommonMaster(lituser, litlogo, favLink, litstyles, litemplogon, windowOnError)

            Me.litstyles.Text = cColours.customiseStyles(False)

            clsMasterPageMethods.AttachOnLoadAndUnLoads(body, onloadfunc)

            If Not currentUser.CheckUserHasAccesstoWebsite() Then
                Response.Redirect(ErrorHandlerWeb.LogOut, True)
            End If

            Dim subaccs As New cAccountSubAccounts(currentUser.Account.accountid)
            Dim subacc As cAccountSubAccount = subaccs.getSubAccountById(currentUser.CurrentSubAccountId)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(currentUser.Account, subaccs.getSubAccountsCollection, currentUser.CurrentSubAccountId)

            Dim productModule = Me._productModuleFactory(currentUser.CurrentActiveModule)
            Page.Title = productModule.BrandName & " from Selenity Ltd."

            If Request.QueryString("cl") = "1" Then
                ' clear any locks held for the current user
                Dim db As New cFWDBConnection
                db.DBOpen(fws, False)
                cLocks.RemoveLockItem(currentUser.Account.accountid, fws.getConnectionString, Cache, "CD_" & currentUser.AccountID.ToString, ViewState("ActiveContract"), currentUser.Employee.EmployeeID)
                cLocks.RemoveLockItem(currentUser.Account.accountid, fws.getConnectionString, Cache, "CA_" & currentUser.AccountID.ToString, ViewState("ActiveContract"), currentUser.Employee.EmployeeID)
                db.DBClose()
            End If

            litmenu.Text = createMenu()



            Dim strStyles As New StringBuilder
            If ShowFWLogo = False Then
                strStyles.Append("<style type=""text/css"">.applogo { url(); }</style>")
            End If



            strStyles.Append("<style type=""text/css""> body { background-image: url();}  .submenuholder { width: 1px; }</style>")

            strStyles.Append(sStyle)

            litstyles.Text += strStyles.ToString

            Dim clscolours As New Spend_Management.cColours(currentUser.Account.accountid, currentUser.CurrentSubAccountId, currentUser.CurrentActiveModule)

            Dim menuItemScript As New StringBuilder
            menuItemScript.Append("function menuItemOver(i)" & vbNewLine)
            menuItemScript.Append("{" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemlabel' + i).style.color = """ & clscolours.menuOptionHoverTxtColour & """;" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemline' + i).style.color = """ & clscolours.menuOptionHoverTxtColour & """;" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemline' + i).style.backgroundColor = """ & clscolours.menuOptionHoverTxtColour & """;" & vbNewLine)
            menuItemScript.Append("}" & vbNewLine)
            menuItemScript.Append("function menuItemOut(i)" & vbNewLine)
            menuItemScript.Append("{" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemlabel' + i).style.color = """ & clscolours.defaultMenuOptionStandardTxtColour & """;" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemline' + i).style.color = """ & clscolours.defaultMenuOptionStandardTxtColour & """;" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemline' + i).style.backgroundColor = """ & clscolours.defaultMenuOptionStandardTxtColour & """;" & vbNewLine)
            menuItemScript.Append("}" & vbNewLine)
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "menuJS", menuItemScript.ToString, True)
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "reporttag", clsMasterPageMethods.SetMasterPageJavaScriptVars) ' "var isSubFolder = 0; var appPath = '" & cMisc.path & "'; var isReports = " & bIsReports.ToString.ToLower & "; theme = '" & Session("THEME") & "'; " & vbNewLine & menuItemScript.ToString & "var accountid = " & curUser.AccountID & ";", True)
            Page.ClientScript.RegisterStartupScript(GetType(String), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(mdlMasterPopup))
        End If
    End Sub

    Public Sub addMenuItem(ByVal logo As String, ByVal size As Integer, ByVal label As String, ByVal description As String, ByVal isCustom As Boolean, ByVal url As String)
        Dim item As cMenuItem = New cMenuItem(logo, size, label, description, isCustom, url)
        arrMenuitems.Add(item)
    End Sub

    Public Sub addMenuItem(ByVal logo As String, ByVal size As Integer, ByVal label As String, ByVal description As String, ByVal url As String, Optional ByVal urltarget As String = "", Optional ByVal extension As String = "png")
        Dim item As cMenuItem = New cMenuItem(logo, size, label, description, url, urltarget, extension)
        arrMenuitems.Add(item)
    End Sub

    Private Function createMenu() As String
        Dim curUser As CurrentUser = cMisc.GetCurrentUser()
        Dim MenuItem As cMenuItem
        Dim output As New System.Text.StringBuilder()
        Dim clsColours As New Spend_Management.cColours(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.CurrentActiveModule)


        Dim currentNode As SiteMapNode = SiteMap.CurrentNode
        Dim rootNode As SiteMapNode = SiteMap.RootNode
        Dim breadCrumbs As New StringBuilder()
        breadCrumbs.Append("<ol class=""breadcrumb"">")
        Dim list = New List(Of String)()
        If currentNode.Equals(rootNode) Then
            breadCrumbs.Append("<li class=""active""><a href=""#""><i><img src=""/static/images/expense/menu-icons/bradcrums-dashboard-icon.png""  alt=""""/></i> " + currentNode.Title + "</a></li>")
        Else
            breadCrumbs.Append("<li><a href=""" + rootNode.Url + """><i><img src=""/static/images/expense/menu-icons/bradcrums-dashboard-icon.png""  alt=""""/></i> " + rootNode.Title + "</a></li>")
            While currentNode.Title <> rootNode.Title
                If currentNode Is Nothing Then
                    Exit While
                Else

                    list.Add("<li><a href=""" + currentNode.Url + """>  <label class=""breadcrumb_arrow"">/</label>" + currentNode.Title + "</a></li>")
                    currentNode = currentNode.ParentNode
                End If
            End While
            For x As Integer = list.Count - 1 To -1 + 1 Step -1
                breadCrumbs.Append(list(x))
            Next
        End If

        breadCrumbs.Append("</ol>")
        Me.litBreadCrumbs.Text = breadCrumbs.ToString()

        Dim i As Integer = 0
        Dim numitems As Double
        Dim even As Boolean = False

        numitems = Decimal.Parse(menuitems.Count.ToString())

        If numitems = 0 Then
            Return ""
        End If

        MenuItem = CType(menuitems(0), cMenuItem)

        output.Append("<div class=""main-content-area"" style=""background:#eeeeee !important;""><div>")
        For i = 0 To menuitems.Count - 1
            even = (even Xor True)

            MenuItem = CType(menuitems(i), cMenuItem)


            output.Append(" <div class=""col-md-6""><a style=""cursor:pointer"" onclick=""")
            If MenuItem.target.Length = 0 Then
                output.Append("document.location.href='" & MenuItem.url & "';")
            Else
                output.Append("window.open('" & MenuItem.url & "','" & MenuItem.target & "','width=1024,height=768,locationbar=no,menubar=no,scrollbars=yes,status=1,resizable=1');")
            End If
            output.Append(""">")
            Dim iconPath As String = GlobalVariables.StaticContentLibrary
            Dim iconName As String

            If MenuItem.isCustom Then
                iconName = MenuItem.logo
            Else
                iconName = MenuItem.logo & "." & MenuItem.extension
            End If
            output.Append("<div class=""well""><div class=""media contact-info wow fadeInDown"" data-wow-duration=""1000ms"" data-wow-delay=""600ms"" style=""margin-left: -6px;"">")
            output.Append("<div class=""pull-left"" style=""margin-top: 5px;""><img src=""" + iconPath + "/icons/48/new/" & iconName & """ alt="""" /></div>")
            output.Append("<div class=""media-body""><h2 id=""menuitemlabel" & i & """>" & MenuItem.label & "</h2>")
            output.Append("<p>" + MenuItem.description + "</p>")
            output.Append("</div></div></div></a></div>")


            If Not even Then
                output.Append("<div class=""clearfix""></div>")
            End If

        Next

        output.Append("</div>")
        Return output.ToString
    End Function
End Class

Public Class cMenuItem
    Private sLogo As String
    Private sLabel As String
    Private sDescription As String
    Private sURL As String
    Private nLogoSize As Integer
    Private sTarget As String
    Private sExtension As String
    Private bIsCustom As Boolean

#Region "properties"
    Public ReadOnly Property logo() As String
        Get
            Return sLogo
        End Get
    End Property

    Public ReadOnly Property label() As String
        Get
            Return sLabel
        End Get
    End Property

    Public ReadOnly Property description() As String
        Get
            Return sDescription
        End Get
    End Property

    Public ReadOnly Property logosize() As Integer
        Get
            Return nLogoSize
        End Get
    End Property

    Public ReadOnly Property url() As String
        Get
            Return sURL
        End Get
    End Property

    Public ReadOnly Property target() As String
        Get
            Return sTarget
        End Get
    End Property

    Public ReadOnly Property extension() As String
        Get
            Return sExtension
        End Get
    End Property

    Public ReadOnly Property isCustom() As String
        Get
            Return bIsCustom
        End Get
    End Property
#End Region

    Public Sub New(ByVal logo As String, ByVal logosize As Integer, ByVal label As String, ByVal description As String, ByVal url As String, ByVal urltarget As String, ByVal extension As String)
        sLogo = logo
        nLogoSize = logosize
        sLabel = label
        sDescription = description
        sURL = url
        sTarget = urltarget
        sExtension = extension
        bIsCustom = isCustom
    End Sub

    Public Sub New(ByVal logo As String, ByVal logosize As Integer, ByVal label As String, ByVal description As String, ByVal iscustom As Boolean, ByVal url As String)
        sLogo = logo
        nLogoSize = logosize
        sLabel = label
        sDescription = description
        sURL = url
        sTarget = ""
        sExtension = extension
        bIsCustom = iscustom
    End Sub
End Class

