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

Partial Class FWMenu
    Inherits System.Web.UI.MasterPage

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

    Public Property menutitle() As String
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

    Public Property showsinglecolumnmenu() As Boolean
        Get
            Return bSingleColumnMenu
        End Get
        Set(ByVal value As Boolean)
            bSingleColumnMenu = value
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

    Public Property home() As Boolean
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
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim currentUser As CurrentUser = cMisc.getCurrentUser()
        Dim clsMasterPageMethods As New cMasterPageMethods(currentUser)


        If Me.IsPostBack = False Then

            clsMasterPageMethods.SetupCommonMaster(lituser, litlinks, litlogo, favLink, litstyles, litemplogon)

            Dim curUser As CurrentUser = cMisc.getCurrentUser(Page.User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim subacc As cAccountSubAccount = subaccs.getSubAccountById(curUser.CurrentSubAccountId)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            Dim clsModules As New cModules
            Dim clsModule As cModule = clsModules.GetModuleByID(CInt(curUser.CurrentActiveModule))
            Page.Title = clsModule.BrandNamePlainText & " from Software (Europe) Ltd."

            If Request.QueryString("cl") = "1" Then
                ' clear any locks held for the current user
                Dim db As New cFWDBConnection
                db.DBOpen(fws, False)
                cLocks.RemoveLockItem(curUser.Account.accountid, fws.getConnectionString, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.Employee.employeeid)
                cLocks.RemoveLockItem(curUser.Account.accountid, fws.getConnectionString, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.Employee.employeeid)
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
            menuItemScript.Append("document.getElementById('menuitemlabel' + i).style.color = """ & clscolours.hovercolour & """;" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemline' + i).style.color = """ & clscolours.hovercolour & """;" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemline' + i).style.backgroundColor = """ & clscolours.hovercolour & """;" & vbNewLine)
            menuItemScript.Append("}" & vbNewLine)
            menuItemScript.Append("function menuItemOut(i)" & vbNewLine)
            menuItemScript.Append("{" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemlabel' + i).style.color = """ & clscolours.pageOptionFGColour & """;" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemline' + i).style.color = """ & clscolours.pageOptionFGColour & """;" & vbNewLine)
            menuItemScript.Append("document.getElementById('menuitemline' + i).style.backgroundColor = """ & clscolours.pageOptionFGColour & """;" & vbNewLine)
            menuItemScript.Append("}" & vbNewLine)
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "menuJS", menuItemScript.ToString, True)
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "reporttag", clsMasterPageMethods.SetMasterPageJavaScriptVars) ' "var isSubFolder = 0; var appPath = '" & cMisc.path & "'; var isReports = " & bIsReports.ToString.ToLower & "; theme = '" & Session("THEME") & "'; " & vbNewLine & menuItemScript.ToString & "var accountid = " & curUser.AccountID & ";", True)
            Page.ClientScript.RegisterStartupScript(GetType(String), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(mdlMasterPopup))
        End If
    End Sub

    Public Sub addMenuItem(ByVal logo As String, ByVal size As Integer, ByVal label As String, ByVal description As String, ByVal url As String, Optional ByVal urltarget As String = "", Optional ByVal extension As String = "gif")
        Dim item As cMenuItem = New cMenuItem(logo, size, label, description, url, urltarget, extension)
        arrMenuitems.Add(item)
    End Sub

    Private Function createMenu() As String
        Dim curUser As CurrentUser = cMisc.getCurrentUser(Page.User.Identity.Name)
        Dim MenuItem As cMenuItem
        Dim output As New System.Text.StringBuilder()
        Dim clsColours As New Spend_Management.cColours(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.CurrentActiveModule)

        output.Append("<div class=""menutitle"">")
        output.Append(menutitle)
        output.Append("</div>")

        Dim i As Integer = 0
        Dim numitems As Double
        Dim even As Boolean = False

        numitems = Decimal.Parse(menuitems.Count.ToString())

        If numitems = 0 Then
            Return ""
        End If

        MenuItem = CType(menuitems(0), cMenuItem)

        output.Append("<div class=""menu"">")
        For i = 0 To menuitems.Count - 1
            even = (even Xor True)

            MenuItem = CType(menuitems(i), cMenuItem)

            If even = True Or bSingleColumnMenu = True Then
                output.Append("<table align=""center"" style=""width: 95%; border: 1px solid #fff; cursor: pointer; cursor: hand; margin-left: auto; margin-right: auto; margin-bottom: 20px;"">")
                output.Append("<tr>")
            End If

            output.Append("<td style=""width: 45%;"" valign=""top"">")
            output.Append("<table id=""menuitem" & i & """ onclick=""")
            If MenuItem.target.Length = 0 Then
                output.Append("document.location.href='" & MenuItem.url & "';")
            Else
                output.Append("window.open('" & MenuItem.url & "','" & MenuItem.target & "','width=1024,height=768,locationbar=no,menubar=no,scrollbars=yes,status=1,resizable=1');")
            End If

            output.Append(""" border=""0"" width=""100%"" onmouseover=""menuItemOver(" & i & ");changeIcon('" & i.ToString & "','./icons/" & MenuItem.logosize & "/shadow/" & MenuItem.logo & "." & MenuItem.extension & "');"" onmouseout=""menuItemOut(" & i & ");changeIcon('" & i.ToString & "','./icons/" & MenuItem.logosize & "/plain/" & MenuItem.logo & "." & MenuItem.extension & "');"">")
            output.Append("<tr>")
            output.Append("<td style=""vertical-align: top; width: 58px; text-align: left;"">")
            output.Append("<img id=""icon" & i.ToString & """ class=""menuitemlogo"" src=""./icons/" & MenuItem.logosize & "/plain/" & MenuItem.logo & "." & MenuItem.extension & """ border=""0"">")
            output.Append("</td>")

            output.Append("<td>")

            output.Append("<span id=""menuitemlabel" & i & """ class=""menuitemtitle"">" & MenuItem.label & "</span>")
            output.Append("<hr id=""menuitemline" & i & """ class=""mastermenuitemline"" style=""color: " & clsColours.pageOptionFGColour & "; background-color: " & clsColours.pageOptionFGColour & ";"">")
            output.Append("<span class=""menuitemdescription"">")
            output.Append(MenuItem.description)
            output.Append("</span>")
            output.Append("</td>")
            output.Append("</tr>")
            output.Append("</table>")
            output.Append("</td>")

            If bSingleColumnMenu = False Then
                If even = False Or i = (arrMenuitems.Count - 1) Then
                    If (arrMenuitems.Count - 1) = i And even = True Then
                        output.Append("<td style=""width: 5%;"">&nbsp;</td>")
                        output.Append("<td style=""width: 45%;"">&nbsp;</td>")
                    End If
                    output.Append("</tr>")
                    output.Append("</table>")
                Else
                    output.Append("<td style=""width: 5%;"">&nbsp;</td>")
                End If
            Else
                output.Append("<td style=""width: 5%;"">&nbsp;</td>")
                output.Append("<td style=""width: 45%;"">&nbsp;</td>")
                output.Append("</tr>")
                output.Append("</table>")
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
#End Region

    Public Sub New(ByVal logo As String, ByVal logosize As Integer, ByVal label As String, ByVal description As String, ByVal url As String, ByVal urltarget As String, ByVal extension As String)
        sLogo = logo
        nLogoSize = logosize
        sLabel = label
        sDescription = description
        sURL = url
        sTarget = urltarget
        sExtension = extension
    End Sub
End Class

