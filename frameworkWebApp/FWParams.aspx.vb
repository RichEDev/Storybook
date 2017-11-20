Imports FWClasses
Imports FWReportsLibrary
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class FWParams
        Inherits System.Web.UI.Page

        '      Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '          If Me.IsPostBack = False Then
        '              Dim action As String
        '		Dim curUser As CurrentUser = cMisc.GetCurrentUser()
        '		Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
        '		Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

        '		Title = "Framework Parameters"
        '		Master.title = Title

        '		action = Request.QueryString("action")
        '		Select Case action
        '			Case "new"
        '				txtParameter.Text = ""
        '				txtValue.Text = ""
        '				panelEditFields.Visible = True
        '				panelParamList.Visible = False
        '				lstLocation.Enabled = False

        '			Case "edit"
        '				GetParam(curUser, Request.QueryString("param"))

        '			Case "delete"
        '				DeleteParam(curUser, Request.QueryString("param"))

        '			Case "success"
        '				lblStatusMessage.Text = "Action completed successfully."
        '			Case Else

        '		End Select

        '		LoadLocationFilter(curUser)

        '		litParams.Text = GetParamList(curUser)
        '	End If
        'End Sub

        'Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
        '	Dim action As String
        '	Dim success As Boolean = True
        '	Dim curUser As CurrentUser = cMisc.GetCurrentUser()
        '	Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
        '	Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
        '	Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId.Value).SubAccountProperties
        '	Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)

        '	action = Request.QueryString("action")

        '	Select Case action
        '		Case "new", "success"
        '			If params.ParamExists(txtParameter.Text) = False Then
        '				params.SetParameterValue(txtParameter.Text, txtValue.Text.Trim)
        '			Else
        '				lblStatusMessage.Text = "Parameter already exists. Cannot insert new parameter"
        '				success = False
        '			End If

        '		Case "edit"
        '			If params.ParamExists(txtParameter.Text) = True Then
        '				params.SetParameterValue(txtParameter.Text, txtValue.Text.Trim)
        '			Else
        '				lblStatusMessage.Text = "Parameter doesn't exist. Cannot amend parameter"
        '				success = False
        '			End If
        '	End Select

        '	If success = True Then
        '		Try
        '			Dim clsreports As IReports = CType(Activator.GetObject(GetType(IReports), ConfigurationManager.AppSettings("ReportsServicePath") & "/reports.rem"), IReports)
        '			clsreports.InvalidateReportCache(usrInfo, fws, usrInfo.ActiveLocation)
        '			clsreports.InvalidateReportDefinitions(usrInfo, fws)
        '			clsreports.InvalidateParameters(usrInfo, fws)
        '			Dim f As New cFields(fws.MetabaseCustomerId)
        '			'f.InvalidateCache()
        '			Dim t As New cTables(fws.MetabaseCustomerId)
        '			't.InvalidateCache()
        '			Dim j As New cJoins(fws.MetabaseCustomerId, fws.getConnectionString, ConfigurationManager.ConnectionStrings("metabase").ConnectionString, t, f)
        '			'j.InvalidateCache()

        '		Catch ex As Exception

        '		End Try

        '		Response.Redirect("FWParams.aspx?action=success", True)
        '	End If
        'End Sub

        'Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
        '	Response.Redirect("FWParams.aspx", True)
        'End Sub

        'Private Sub GetParam(ByVal fws As cFWSettings, ByVal uinfo As UserInfo, ByVal param As String)
        '	panelEditFields.Visible = True
        '	panelParamList.Visible = False
        '	lstLocation.Enabled = False

        '	Dim paramColl As New FWClasses.cParams(uinfo, fws, uinfo.ActiveLocation)
        '	Dim curParam As cParam = paramColl.GetParamByName(param)

        '	If curParam Is Nothing Then
        '		txtParameter.Text = param
        '		txtValue.Text = ""
        '	Else
        '		txtParameter.Text = curParam.ParameterName
        '		txtValue.Text = curParam.ParameterValue
        '	End If
        '	'db.FWDb("R", "fwparams", "Param", param, "Location Id", uinfo.ActiveLocation)
        '	'If db.FWDbFlag = True Then
        '	'    txtParameter.Text = param
        '	'    txtValue.Text = db.FWDbFindVal("Value")
        '	'End If
        'End Sub

        'Private Sub DeleteParam(ByVal db As cFWDBConnection, ByVal uinfo As UserInfo, ByVal param As String)
        '	Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)

        '	db.FWDb("D", "fwparams", "Param", param, "Location Id", uinfo.ActiveLocation, "", "", "", "", "", "", "", "")

        '	Dim fws As cFWSettings = curUser.UserFWS
        '	Dim params As New FWClasses.cParams(uinfo, fws, uinfo.ActiveLocation)
        '	params.InvalidateCache()

        '	Dim clsreports As IReports = CType(Activator.GetObject(GetType(IReports), ConfigurationManager.AppSettings("ReportsServicePath") & "/reports.rem"), IReports)
        '	clsreports.InvalidateReportCache(uinfo, fws, uinfo.ActiveLocation)

        '	lblStatusMessage.Text = "Deletion completed."
        '	txtParameter.Text = ""
        '	txtValue.Text = ""
        'End Sub

        'Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
        '	Response.Redirect("Home.aspx", True)
        'End Sub

        'Private Function GetParamList(ByVal db As cFWDBConnection, ByVal uinfo As UserInfo) As String
        '	Dim strHTML As System.Text.StringBuilder
        '	Try
        '		Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
        '		Dim fws As cFWSettings = curUser.UserFWS
        '		Dim params As New FWClasses.cParams(uinfo, fws, uinfo.ActiveLocation)

        '		Dim allowDelete As Boolean = False
        '		If Request.QueryString("allowdelete") = "1" Then
        '			allowDelete = True
        '		End If
        '		'Dim sql As String

        '		'sql = "SELECT * FROM [fwparams] WHERE [Location Id] = @locId AND [Editable] = 1 ORDER BY [Param]"
        '		'db.AddDBParam("locId", uinfo.ActiveLocation, True)
        '		'db.RunSQL(sql, db.glDBWorkA)

        '		'Dim drow As DataRow
        '		Dim rowClass As String = "row1"
        '		Dim rowalt As Boolean = False

        '		strHTML = New System.Text.StringBuilder
        '		With strHTML
        '			.Append("<table class=""datatbl"">" & vbNewLine)
        '			.Append("<tr>" & vbNewLine)
        '			.Append("<th><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
        '			If allowDelete Then
        '				.Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
        '			End If
        '			.Append("<th>Parameter Name</th>" & vbNewLine)
        '			.Append("<th>Parameter Value</th>" & vbNewLine)
        '			.Append("</tr>" & vbNewLine)

        '			Dim arrParams As ArrayList = params.GetParamList

        '			If arrParams.Count > 0 Then	' db.glNumRowsReturned > 0
        '				For i As Integer = 0 To arrParams.Count - 1	'For Each drow In db.glDBWorkA.Tables(0).Rows
        '					Dim ParamItem As cParam = CType(arrParams(i), cParam)

        '					If ParamItem.IsEditable = True Then
        '						rowalt = (rowalt Xor True)
        '						If rowalt Then
        '							rowClass = "row1"
        '						Else
        '							rowClass = "row2"
        '						End If

        '						.Append("<tr>" & vbNewLine)
        '						.Append("<td class=""" & rowClass & """>")
        '						If (uinfo.permAmend) Then
        '							.Append("<a href=""FWParams.aspx?action=edit&param=" & ParamItem.ParameterName & """ onmouseover=""window.status='Edit the parameters value';return true;"" onmouseout=""window.status='Done';"" title=""Edit the parameters value"" alt=""Edit""><img src=""./icons/edit.gif"" /></a>")
        '						End If
        '						.Append("</td>" & vbNewLine)
        '						If allowDelete Then
        '							.Append("<td class=""" & rowClass & """>")
        '							If uinfo.permDelete Then
        '								.Append("<a href=""javascript:if(confirm('Click OK to confirm deletion')){window.location.href='FWParams.aspx?action=delete&param=" & ParamItem.ParameterName & "';}"" onmouseover=""window.status='Delete this parameter';return true;"" onmouseout=""window.status='Done';"" title=""Delete this parameter"" alt=""Delete""><img src=""./icons/delete2.gif"" /></a>")
        '							End If
        '							.Append("</td>" & vbNewLine)
        '						End If
        '						.Append("<td class=""" & rowClass & """>" & ParamItem.ParameterName & "</td>" & vbNewLine)
        '						.Append("<td class=""" & rowClass & """>" & ParamItem.ParameterValue & "</td>" & vbNewLine)
        '						.Append("</tr>" & vbNewLine)
        '					End If
        '				Next
        '			Else
        '				.Append("<tr>" & vbNewLine)
        '				If allowDelete Then
        '					.Append("<td class=""row1"" align=""center"" colspan=""4"">No parameters found</td>" & vbNewLine)
        '				Else
        '					.Append("<td class=""row1"" align=""center"" colspan=""3"">No parameters found</td>" & vbNewLine)
        '				End If
        '				.Append("</tr>" & vbNewLine)
        '			End If
        '			.Append("</table>" & vbNewLine)
        '		End With

        '	Catch ex As Exception
        '		strHTML = New System.Text.StringBuilder

        '		With strHTML
        '			.Append("<table class=""datatbl"" width=""500"">" & vbNewLine)
        '			.Append("<tr>" & vbNewLine)
        '			.Append("<th>An error has occurred retrieving parameters</th>" & vbNewLine)
        '			.Append("</tr>" & vbNewLine)
        '			.Append("<tr>" & vbNewLine)
        '			.Append("<td class=""row1"">" & ex.Message & "</td>" & vbNewLine)
        '			.Append("</tr>" & vbNewLine)
        '			.Append("</table>" & vbNewLine)
        '		End With
        '	End Try

        '	Return strHTML.ToString
        'End Function

        'Protected Sub lstLocation_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstLocation.SelectedIndexChanged
        '	Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
        '	Dim uinfo As UserInfo = curUser.currentUser.userInfo
        '	Dim db As New cFWDBConnection

        '	db.DBOpen(curUser.UserFWS, False)

        '	uinfo.ActiveLocation = lstLocation.SelectedItem.Value
        '	uinfo = uinfo

        '	litParams.Text = GetParamList(db, uinfo)

        '	db.DBClose()
        '	db = Nothing
        'End Sub

        'Private Sub LoadLocationFilter(ByVal db As cFWDBConnection, ByVal uinfo As UserInfo)
        '	Dim x As Integer

        '	For x = 1 To uinfo.LocListId.Count
        '		lstLocation.Items.Add(New ListItem(uinfo.LocListNames(x), uinfo.LocListId(x)))
        '	Next
        '	lstLocation.Items.FindByValue(uinfo.ActiveLocation.ToString).Selected = True
        'End Sub
    End Class
End Namespace