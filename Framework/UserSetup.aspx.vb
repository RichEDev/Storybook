Imports Spend_Management
Imports SpendManagementLibrary
Imports FWClasses
Imports FWReportsLibrary
Imports System.Collections
Imports System.Collections.Generic

Namespace Framework2006
    Partial Class UserSetup
        Inherits System.Web.UI.Page
        Private Logons As New DataSet
        Private ARec As New cAuditRecord

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

		'      Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		'          ' Set up the Infragistics grid for User Logons
		'	Dim FWDb As New cFWDBConnection
		'	Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
		'	Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
		'	Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId.Value).SubAccountProperties
		'	Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
		'	Dim AdminId As Integer
		'	Dim action As String
		'	Dim userid As Integer
		'	Dim hasdata As Boolean

		'	FWDb.DBOpen(fws, False)
		'	hasdata = False

		'	If Me.IsPostBack = False Then
		'		Dim ALog As New cFWAuditLog(fws)

		'		' Get List of users
		'		action = Request.QueryString("action")
		'		userid = Request.QueryString("userid")
		'		Dim users As New cEmployees(curUser.Account.accountid)

		'		Select Case action
		'			Case "archive"
		'				If userid <> 0 Then
		'					Dim user As cEmployee = users.GetEmployeeById(userid)

		'					If Not user Is Nothing Then
		'						ARec.Action = cFWAuditLog.AUDIT_DEL
		'						ARec.DataElementDesc = "USER LOGON"
		'						ARec.ElementDesc = "User:" & user.UserName
		'						ALog.AddAuditRec(ARec, True)
		'						ALog.CommitAuditLog(curUser.Employee)

		'						Dim DeleteSQL As New System.Text.StringBuilder
		'						DeleteSQL.Append("SELECT [team_members].[Team Id] FROM team_members INNER JOIN teams ON [teams].[teamid] = [team_members].[Team Id] WHERE [Team Type] = 1 AND [Member Id] = @userid")
		'						FWDb.AddDBParam("userid", userid, True)
		'						FWDb.RunSQL(DeleteSQL.ToString, FWDb.glDBWorkD, False, "", False)
		'						Dim drow As DataRow
		'						For Each drow In FWDb.glDBWorkD.Tables(0).Rows
		'							FWDb.FWDb("D", "team_members", "Member Id", userid, "Team Id", drow.Item("Team Id"), "", "", "", "", "", "", "", "")
		'						Next
		'						FWDb.FWDb("D", "contract_audience", "Audience Type", AudienceType.Individual, "Access Id", userid, "", "", "", "", "", "", "", "")
		'						FWDb.FWDb("D", "attachment_audience", "Audience Type", AudienceType.Individual, "Access Id", userid, "", "", "", "", "", "", "", "")
		'						FWDb.FWDb("D", "favourite_reports", "User Id", userid, "", "", "", "", "", "", "", "", "", "")
		'						FWDb.FWDb("D", "security_locations", "User Id", userid, "", "", "", "", "", "", "", "", "", "")
		'						'FWDb.FWDb("D", "security_history", "User Id", userid)
		'						FWDb.SetFieldValue("Status", LogonStatus.Archived, "N", True)
		'						FWDb.FWDb("A", "security", "User Id", userid, "", "", "", "", "", "", "", "", "", "")
		'						FWDb.SetFieldValue("userid", 1, "N", True)
		'						FWDb.FWDb("A", "reports", "userid", userid, "", "", "", "", "", "", "", "", "", "")
		'						FWDb.FWDb("A", "report_folders", "userid", userid, "", "", "", "", "", "", "", "", "", "")
		'						FWDb.FWDb("D", "user_preferences", "User Id", userid, "", "", "", "", "", "", "", "", "", "")

		'						user.archived = True
		'						users.saveEmployee(user)

		'                              Dim teams As New cTeams(fws.MetabaseCustomerId)
		'                              'teams.RemoveMemberFromTeams(userid, TeamType.User)

		'						InvalidateSchedulerCache()
		'					End If
		'				End If

		'			Case "unfreeze"
		'				If userid <> 0 Then
		'					Dim user As cEmployee = users.GetEmployeeById(userid)
		'					If Not user Is Nothing Then
		'						ARec.Action = cFWAuditLog.AUDIT_UPDATE
		'						ARec.DataElementDesc = "USER LOGON"
		'						ARec.ElementDesc = "User:" & user.UserName
		'						ARec.PreVal = "**Account Frozen**"
		'						ARec.PostVal = "**Account Unfrozen**"
		'						ALog.AddAuditRec(ARec, True)
		'						ALog.CommitAuditLog(curUser.Employee)

		'						FWDb.SetFieldValue("Status", LogonStatus.Active, "N", True)
		'						FWDb.SetFieldValue("Retry Count", 0, "N", False)
		'						FWDb.SetFieldValue("Frozen", 0, "N", False)
		'						FWDb.FWDb("A", "security", "User Id", userid, "", "", "", "", "", "", "", "", "", "")

		'						user.archived = False
		'						'user.LogonRetryCount = 0
		'						users.saveEmployee(user)

		'						InvalidateSchedulerCache()
		'					End If
		'				End If

		'			Case "reinstate"
		'				If userid <> 0 Then
		'					Dim user As cEmployee = users.GetEmployeeById(userid)
		'					If Not user Is Nothing Then
		'						ARec.Action = cFWAuditLog.AUDIT_UPDATE
		'						ARec.DataElementDesc = "USER LOGON"
		'						ARec.ElementDesc = "User:" & user.UserName
		'						ARec.PreVal = "**Account Suspended/Archived**"
		'						ARec.PostVal = "**Account Reinstated**"
		'						ALog.AddAuditRec(ARec, True)
		'						ALog.CommitAuditLog(curUser.Employee)

		'						FWDb.SetFieldValue("Status", LogonStatus.Active, "N", True)
		'						FWDb.SetFieldValue("Frozen", 0, "N", False)
		'						FWDb.SetFieldValue("Retry Count", 0, "N", False)
		'						FWDb.FWDb("A", "security", "User Id", userid, "", "", "", "", "", "", "", "", "", "")

		'						user.archived = False
		'						'user.LogonRetryCount = 0
		'						users.saveEmployee(user)

		'						InvalidateSchedulerCache()
		'					End If
		'				End If
		'			Case Else

		'		End Select

		'		'FWDb.FWDb("R2", "Security", "User Name", "Admin")
		'		FWDb.RunSQL("SELECT [User Id] FROM [security] WHERE LOWER([User Name]) = 'admin'", FWDb.glDBWorkL, False, "", False)
		'		If FWDb.glNumRowsReturned > 0 Then
		'			AdminId = FWDb.GetFieldValue(FWDb.glDBWorkL, "User Id", 0, 0)
		'		Else
		'			AdminId = 0
		'		End If

		'		Dim tmpLiveStatus As Boolean = True

		'		'sql = "SELECT [User Id],[User Name],ISNULL([Position],'') AS [Position],[Full Name],[Logon Count],[Frozen],[Status] FROM [security] WHERE LOWER([User Name]) <> 'admin' AND [Status]"
		'		If Request.QueryString("sa") = "1" Then
		'			'sql += " = @archivestatus "
		'			lnkLive.Visible = True
		'			tmpLiveStatus = False
		'			'Else
		'			'sql += " <> @archivestatus "
		'		End If
		'		'sql += "ORDER BY [Full Name]"

		'		'FWDb.AddDBParam("archivestatus", LogonStatus.Archived, True)
		'		'FWDb.RunSQL(sql, FWDb.glDBWorkA)

		'		litUserList.Text = UserDisplayBasic(curUser, tmpLiveStatus, AdminId)
		'	End If

		'	lnkNew.ToolTip = "Add a new user logon"
		'	lnkNew.Attributes.Add("onmouseover", "window.status='Add a new user logon';return true;")
		'	lnkNew.Attributes.Add("onmouseout", "window.status='Done';")
		'	lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Employees, False)

		'	lnkReset.ToolTip = "Reset the logon counters"
		'	lnkReset.Attributes.Add("onmouseover", "window.status='Reset the logon counters';return true;")
		'	lnkReset.Attributes.Add("onmouseout", "window.status='Done';")
		'	lnkReset.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Employees, False)

		'	lnkArchived.ToolTip = "View Archived User Logons"
		'	lnkArchived.Attributes.Add("onmouseover", "window.status='View Archived User Logons';return true;")
		'	lnkArchived.Attributes.Add("onmouseout", "window.status='Done';")
		'	lnkArchived.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, False) And Not lnkLive.Visible

		'	Title = "User Setup"
		'	Master.title = Title

		'	FWDb.DBClose()
		'	FWDb = Nothing
		'End Sub

		'Private Function UserDisplayBasic(ByVal curUser As CurrentUser, ByVal DisplayLive As Boolean, ByVal AdminId As Integer) As String
		'	Dim strHTML As New System.Text.StringBuilder
		'	Dim rowseq As Boolean = False
		'	Dim hasdata As Boolean
		'	Dim rowClass As String = "row1"
		'	Dim users As New cEmployees(curUser.Account.accountid)

		'	rowseq = False
		'	hasdata = False

		'	strHTML.Append("<table class=""datatbl"">" & vbNewLine)
		'	strHTML.Append("<tr>")
		'	strHTML.Append("<th><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
		'	If DisplayLive Then
		'		strHTML.Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
		'	End If
		'	strHTML.Append("<th>Login ID</th>" & vbNewLine)
		'	strHTML.Append("<th>Full Name</th>" & vbNewLine)
		'	strHTML.Append("<th>Position</th>" & vbNewLine)
		'	strHTML.Append("<th>Logon Count</th>" & vbNewLine)
		'	strHTML.Append("</tr>")

		'	Dim bDisplayUser As Boolean = False

		'	For Each i As KeyValuePair(Of String, cUser) In users.g(False)	'For Each drow In fwdb.glDBWorkA.Tables(0).Rows
		'		Dim user As cUser = CType(i.Value, cUser)

		'		If DisplayLive Then
		'			Select Case user.AccountStatus
		'				Case LogonStatus.Active, LogonStatus.Frozen
		'					bDisplayUser = True
		'				Case Else
		'					bDisplayUser = False
		'			End Select
		'		Else
		'			Select Case user.AccountStatus
		'				Case LogonStatus.Archived, LogonStatus.Suspended
		'					bDisplayUser = True
		'				Case Else
		'					bDisplayUser = False
		'			End Select
		'		End If

		'		If bDisplayUser Then
		'			rowseq = (rowseq Xor True)

		'			If rowseq = True Then
		'				rowClass = "row1"
		'			Else
		'				rowClass = "row2"
		'			End If

		'			strHTML.Append("<tr>" & vbNewLine)
		'			strHTML.Append("<td class=""" & rowClass & """>")
		'			If uinfo.permAmend Then
		'				Select Case user.AccountStatus
		'					Case LogonStatus.Active
		'						strHTML.Append("<a onmouseover=""window.status='Edit the user details';return true;"" onmouseout=""window.status='Done';"" href=""UserSetupDetail.aspx?action=edit&userid=" & user.UserId.ToString & """><img src=""./icons/edit.gif"" /></a>")
		'					Case LogonStatus.Frozen
		'						strHTML.Append("<a onmouseover=""window.status='Unfreeze the user account';return true;"" onmouseout=""window.status='Done';"" href=""UserSetup.aspx?action=unfreeze&userid=" & user.UserId.ToString & """><img src=""./icons/16/plain/lock_open.gif"" /></a>")
		'					Case LogonStatus.Archived, LogonStatus.Suspended
		'						strHTML.Append("<a onmouseover=""window.status='Reinstate the user account';return true;"" onmouseout=""window.status='Done';"" href=""UserSetup.aspx?action=reinstate&userid=" & user.UserId.ToString & """><img src=""./icons/16/plain/lock_open.gif"" /></a>")
		'					Case Else
		'						strHTML.Append("&nbsp;")
		'				End Select
		'			End If
		'			strHTML.Append("</td>" & vbNewLine)

		'			If DisplayLive Then
		'				strHTML.Append("<td class=""" & rowClass & """>")
		'				If uinfo.permDelete Then
		'					strHTML.Append("<a onmouseover=""window.status='Archive the user definition from the database';return true;"" onmouseout=""window.status='Done';"" href=""javascript:if(confirm('Are you sure you want to archive the user logon? Click OK to confirm')){window.location.href='UserSetup.aspx?action=archive&userid=" & user.UserId.ToString & "';}""><img src=""./icons/delete2.gif"" /></a>")
		'				End If
		'				strHTML.Append("</td>" & vbNewLine)
		'			End If
		'			strHTML.Append("<td class=""" & rowClass & """>" & user.UserName & "</td>" & vbNewLine)
		'			strHTML.Append("<td class=""" & rowClass & """>" & user.FullName & "</td>" & vbNewLine)
		'			strHTML.Append("<td class=""" & rowClass & """>" & user.Position & "</td>" & vbNewLine)
		'			'strHTML += "<td class=""" & rowclass & """>" & drow.Item("Role Name") & "</td>" & vbNewLine
		'			Select Case user.AccountStatus
		'				Case LogonStatus.Active
		'					strHTML.Append("<td class=""" & rowClass & """>" & user.LogonCount.ToString & "</td>" & vbNewLine)
		'				Case LogonStatus.Frozen
		'					strHTML.Append("<td class=""" & rowClass & """><i>Account Frozen</i></td>" & vbNewLine)
		'				Case LogonStatus.Suspended
		'					strHTML.Append("<td class=""" & rowClass & """><i>Account Suspended</i></td>" & vbNewLine)
		'				Case LogonStatus.Archived
		'					strHTML.Append("<td class=""" & rowClass & """><i>Account Archived</i></td>" & vbNewLine)
		'				Case Else
		'			End Select

		'			strHTML.Append("</tr>" & vbNewLine)
		'			hasdata = True
		'		End If
		'	Next

		'	If hasdata = False Then
		'		strHTML.Append("<tr><td class=""row1"" colspan=""6"" align=""center"">No logons defined</td></tr>" & vbNewLine)
		'	End If

		'	strHTML.Append("</table>" & vbNewLine)

		'	Return strHTML.ToString
		'End Function

		'Private Sub ResetCounters()
		'	Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
		'	Dim users As New cUsers(curUser.UserFWS)

		'	users.ResetLogonCounts()

		'	Response.Redirect("UserSetup.aspx", True)
		'End Sub

		'Protected Sub lnkNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNew.Click
		'	Response.Redirect("UserSetupDetail.aspx?action=add", True)
		'End Sub

		'Protected Sub lnkReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkReset.Click
		'	ResetCounters()
		'End Sub

		'Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
		'	Response.Redirect("MenuMain.aspx?menusection=employee", True)
		'End Sub

		'Protected Sub lnkArchived_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkArchived.Click
		'	Response.Redirect("UserSetup.aspx?sa=1", True)
		'End Sub

		'Protected Sub lnkLive_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkLive.Click
		'	Response.Redirect("UserSetup.aspx", True)
		'End Sub

		'Private Sub InvalidateSchedulerCache()
		'	Try
		'		Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
		'		Dim clsscheduler As IScheduler = CType(Activator.GetObject(GetType(IScheduler), "tcp://localhost:7887/scheduler.rem"), IScheduler)
		'		clsscheduler.InvalidateUsers(curUser.currentUser.userInfo, curUser.UserFWS)

		'	Catch ex As Exception

		'	End Try
		'End Sub
    End Class
End Namespace
