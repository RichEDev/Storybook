Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class UFSearch
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
            'Put user code to initialize the page here
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim action As String
            Dim rs As cRechargeSetting = Nothing
            Dim subaccs As New cAccountSubAccounts(curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            action = Request.QueryString("action")
            If Not action Is Nothing Then
                action = action.ToLower
            End If

            If fws.glUseRechargeFunction = True Then
                Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, fws.getConnectionString)
                rs = rscoll.getSettings
            End If

            If Me.IsPostBack = False Then
                Select Case action
                    Case "uftxt", "ufasptxt"
                        lblTitle.Text = "Extended Text Entry"
                        lblSearchField.Visible = False
                        txtSearchString.Visible = False
                        txtSearchString.Enabled = False
                        panelSearch.Visible = False
                        cmdFind.Visible = False
						GetText()

                    Case Else ' must be the basic search
                        Select Case CType(Request.QueryString("searchtype"), UserFieldType)
                            Case UserFieldType.RechargeAcc_Code
                                lblSearchField.Text = "Account Code :"
                                lblTitle.Text = "Search for Account Code"

                            Case UserFieldType.RechargeClient_Ref
                                lblSearchField.Text = rs.ReferenceAs
                                lblTitle.Text = "Search for " & rs.ReferenceAs

                            Case UserFieldType.Site_Ref
                                lblSearchField.Text = "Site :"
                                lblTitle.Text = "Search for Site"

                            Case UserFieldType.StaffName_Ref
                                lblSearchField.Text = "Employee Name :"
                                lblTitle.Text = "Search for Employee Name"

                            Case Else

                        End Select
                        txtPanel.Visible = False
                End Select
            End If

            Select Case action
                Case "uftxt"
                    txtTextEditor.Visible = False
                    litSelect.Text = "<a target=""main"" href=""javascript:getTextEntry();"" onmouseover=""window.status='Update text information to parent form';return true;"" onmouseout=""window.status='Done';""><img src=""./buttons/ok.gif"" /></a>"

                Case "ufasptxt"
                    litSelect.Text = "<a target=""ufasptxt"" href=""javascript:getASPTextEntry();"" onmouseover=""window.status='Update text information to parent form';return true;"" onmouseout=""window.status='Done';""><img src=""./buttons/ok.gif"" /></a>"

                Case "ufaspsearch"
                    cmdFind.AlternateText = "Search"
                    cmdFind.ToolTip = "Find matching entries"
                    cmdFind.Attributes.Add("onmouseout", "window.status='Done';")
                    cmdFind.Attributes.Add("onmouseover", "window.status='Find matching entries';return true;")

                    litSelect.Text = "<a target=""ufaspsearch"" href=""javascript:getASPSearchResult();"" onmouseover=""window.status='Use selected search result';return true;"" onmouseout=""window.status='Done';"" title=""Use selected search result""><img src=""./buttons/ok.gif"" /></a>"

                Case Else
                    cmdFind.AlternateText = "Search"
                    cmdFind.ToolTip = "Find matching entries"
                    cmdFind.Attributes.Add("onmouseout", "window.status='Done';")
                    cmdFind.Attributes.Add("onmouseover", "window.status='Find matching entries';return true;")

                    litSelect.Text = "<a target=""main"" href=""javascript:getSearchResult();"" onmouseover=""window.status='Use selected search result';return true;"" onmouseout=""window.status='Done';"" title=""Use selected search result""><img src=""./buttons/ok.gif"" /></a>"
            End Select

            litClose.Text = "<a style=""cursor: hand;"" onclick=""javascript:window.close();""><img src=""./buttons/page_close.gif"" /></a>"

            If action <> "uftxt" And action <> "ufasptxt" Then
                litSelect.Visible = False ' hide initially. Will show when valid search results output
            End If

            'Dim clscolours As New FWClasses.cColours(fws, Page.Theme)
            'litStyles.Text = clscolours.getStyleOverride(Request.ApplicationPath)
            litStyles.Text = Spend_Management.cColours.customiseStyles(False)
        End Sub

        Private Sub doSearch()
			Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim sql As New System.Text.StringBuilder
            Dim html As New System.Text.StringBuilder
            Dim html_title As New System.Text.StringBuilder
            Dim IDFieldName As String = ""
            Dim FieldName As String = ""
            Dim SecondaryFieldName As String = ""
            Dim rs As cRechargeSetting
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim numCols As Integer
            Dim subaccs As New cAccountSubAccounts(curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            If fws.glUseRechargeFunction = True Then
                Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, fws.getConnectionString)
                rs = rscoll.getSettings
            End If

            html_title.Append("<th width=""40"">Select</th>" & vbNewLine)

            Select Case CType(Request.QueryString("searchtype"), UserFieldType)
                Case UserFieldType.Site_Ref
                    sql.Append("SELECT [Site Id],[Site Code],[Site Description] FROM [codes_sites] WHERE [Location Id] = @locId")
                    sql.Append(" AND (")
                    sql.Append("[Site Code] LIKE LOWER(@searchVal1) OR [Site Code] LIKE LOWER(@searchVal2) ")
                    sql.Append("OR [Site Description] LIKE LOWER(@searchVal1) OR [Site Description] LIKE LOWER(@searchVal2) ")
                    sql.Append("OR [Post Code] LIKE LOWER(@searchVal1) ")
                    sql.Append(") ORDER BY [Site Code]")

                    numCols = 3
                    IDFieldName = "Site Id"
                    FieldName = "Site Code"
                    SecondaryFieldName = "Site Description"

                    html_title.Append("<th>Site Code</th>" & vbNewLine)
                    html_title.Append("<th>Description</th>" & vbNewLine)

                Case UserFieldType.RechargeAcc_Code
                    sql.Append("SELECT [Code Id],[Account Code],[Description] FROM [codes_accountcodes] WHERE [Location Id] = @locId ")
                    sql.Append(" AND (")
                    sql.Append("[Account Code] LIKE LOWER(@searchVal1) OR [Account Code] LIKE LOWER(@searchVal2) ")
                    sql.Append(" OR [Description] LIKE LOWER(@searchVal1) OR [Description] LIKE LOWER(@searchVal2)")
                    sql.Append(") ORDER BY [Account Code]")

                    numCols = 3
                    IDFieldName = "Code Id"
                    FieldName = "Account Code"
                    SecondaryFieldName = "Description"
                    html_title.Append("<th>Account Code</th>" & vbNewLine)
                    html_title.Append("<th>Description</th>" & vbNewLine)

                Case UserFieldType.RechargeClient_Ref
                    sql.Append("SELECT [Entity Id],[Name] FROM [codes_rechargeentity] WHERE [Location Id] = @locId ")
                    sql.Append(" AND (")
                    sql.Append("[Name] LIKE LOWER(@searchVal1) OR [Name] LIKE LOWER(@searchVal2) ")
                    sql.Append(") ")
                    sql.Append("ORDER BY [Name]")

                    numCols = 2
                    IDFieldName = "Entity Id"
                    FieldName = "Name"
                    SecondaryFieldName = ""
                    html_title.Append("<th>" & rs.ReferenceAs & "</th>" & vbNewLine)

                Case UserFieldType.StaffName_Ref
                    sql.Append("SELECT employeeid,firstname, surname FROM [employees]")
                    sql.Append(" AND (")
                    sql.Append("[surname] LIKE LOWER(@searchVal1) OR surname LIKE LOWER(@searchVal2)) ")
                    sql.Append("ORDER BY [Staff Name]")

                    IDFieldName = "employeeid"
                    FieldName = "firstname"
                    SecondaryFieldName = "surname"
                    numCols = 2
                    html_title.Append("<th>Employee Name</th>" & vbNewLine)

                Case Else
                    Exit Sub
            End Select

			db.DBOpen(fws, False)

            db.AddDBParam("searchVal1", "%" & txtSearchString.Text.Trim & "%", True)
            db.AddDBParam("searchVal2", txtSearchString.Text.Trim & "%", False)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            html.Append("<table class=""datatbl"">" & vbNewLine)
            html.Append("<thead")
            html.Append("<tr>" & vbNewLine)
            html.Append(html_title)
            html.Append("</tr>" & vbNewLine)
            html.Append("</thead>")
            html.Append("<tfoot>")
            html.Append("<tr><td colspan=""" & numCols.ToString.Trim & """ align=""center""><i><b>End of list</b></i></td></tr>")
            html.Append("</tfoot>")
            html.Append("<tbody>")

            If db.glError = False Then
                Dim drow As DataRow
                Dim hasdata As Boolean = False
                Dim rowalt As Boolean = False
                Dim rowClass As String = "row1"

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    hasdata = True
                    rowalt = (rowalt Xor True)
                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    html.Append("<tr>")
                    html.Append("<td class=""" & rowClass & """ align=""center"">")
                    ' include radio button to match against
                    Dim tmp As String
                    tmp = CStr(drow.Item(IDFieldName)).Trim
                    If SecondaryFieldName <> "" Then
                        html.Append("<input type=""radio"" name=""UFS"" value=""" & tmp & """ onclick=""populateSearchDetails(" & tmp & ",'" & CStr(drow.Item(FieldName)).Replace("'", "\'") & ":" & CStr(drow.Item(SecondaryFieldName)).Replace("'", "\'") & "');"" ")
                    Else
                        html.Append("<input type=""radio"" name=""UFS"" value=""" & tmp & """ onclick=""populateSearchDetails(" & tmp & ",'" & CStr(drow.Item(FieldName)).Replace("'", "\'") & "');"" ")
                    End If

                    html.Append("/>")
                    html.Append("</td>" & vbNewLine)
                    html.Append("<td class=""" & rowClass & """ align=""center"">")
                    If IsDBNull(drow.Item(FieldName)) = False Then
                        html.Append(drow.Item(FieldName))
                    End If
                    html.Append("</td>" & vbNewLine)
                    If SecondaryFieldName <> "" Then
                        html.Append("<td class=""" & rowClass & """ align=""center"">")

                        If IsDBNull(drow.Item(SecondaryFieldName)) = False Then
                            html.Append(drow.Item(SecondaryFieldName))
                        End If
                        html.Append("</td>" & vbNewLine)
                    End If

                    html.Append("</tr>")
                Next

                If hasdata = False Then
                    html.Append("<tr>" & vbNewLine)
                    html.Append("<td class=""row1"" align=""center"" colspan=""" & Trim(Str(numCols + 1)) & """><b>No data has been returned for your search criteria.</b></td>")
                    html.Append("</tr>" & vbNewLine)

                    litSelect.Visible = False
                Else
                    litSelect.Visible = True
                End If
            Else
                html.Append("<tr>" & vbNewLine)
                html.Append("<td class=""row1"" align=""center"" colspan=""" & Trim(Str(numCols + 1)) & """><b>An error has occurred during the database query.</b></td>")
                html.Append("</tr>" & vbNewLine)
            End If

            html.Append("</tbody>")
            html.Append("</table>" & vbNewLine)

            litSearchResults.Text = html.ToString
            db.DBClose()
            db = Nothing
        End Sub

		Private Sub GetText()
			Dim strHTML As New System.Text.StringBuilder

			Dim UF_Field As String
			Dim appArea As AppAreas
			Dim RecId As Integer
			Dim UF_Value As String = ""

			UF_Field = Request.QueryString("ufn").Trim

			'editText = Parent.Page.Request.Form("UF" & UF_Field.Trim)

			appArea = CType(Request.QueryString("ufa"), AppAreas)
			RecId = Integer.Parse(Request.QueryString("ufi"))

			UF_Value = Server.UrlDecode(Request.QueryString("ufv"))
			'Dim litTxt As New Literal
			hiddenEditorText.Value = UF_Value
			'litTxt.Text = "<input type=""hidden"" id=""hiddenEditorText"" value=""" & UF_Value & """ />"
			'Me.Controls.Add(hiddenEditorText)
		End Sub

        Protected Sub cmdFind_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdFind.Click
            doSearch()
        End Sub
    End Class
End Namespace
