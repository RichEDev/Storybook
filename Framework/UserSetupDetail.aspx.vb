Imports SpendManagementLibrary
Imports FWClasses

Namespace Framework2006
    Partial Class UserSetupDetail
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
        '	Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
        '	Dim UserInfo As UserInfo = curUser.currentUser.userInfo
        '	Dim UserId As Integer
        '	Dim action As String

        '	FWDb.DBOpen(curUser.UserFWS, False)

        '	Title = "User Logon Detail"
        '	Master.title = Title
        '	Master.enablenavigation = False
        '	Master.onloadfunc = "PageSetup"

        '	If Me.IsPostBack = False Then
        '		UserId = Request.QueryString("userid")
        '		action = Request.QueryString("action")

        '		Select Case action
        '			Case "edit"
        '				If UserId <> 0 Then
        '					FWDb.FWDb("R2", "security", "User Id", UserId, "", "", "", "", "", "", "", "", "", "")
        '					If FWDb.FWDb2Flag = True Then
        '						txtUserName.Text = FWDb.FWDbFindVal("User Name", 2)
        '						'txtUserName.ReadOnly = True
        '						txtPassword.Text = ""
        '						txtConfirmPwd.Text = ""
        '						hiddenuserid.Text = FWDb.FWDbFindVal("User Id", 2)
        '						txtFullName.Text = FWDb.FWDbFindVal("Full Name", 2)
        '						txtPosition.Text = FWDb.FWDbFindVal("Position", 2)
        '						txtHint.Text = FWDb.FWDbFindVal("Hint", 2)
        '						txtEmail.Text = FWDb.FWDbFindVal("Email", 2)
        '						lstIconSize.Items.FindByValue(FWDb.FWDbFindVal("Icon Size", 2)).Selected = True
        '						reqConfirmPwd.Enabled = False
        '						reqPassword.Enabled = False
        '						valexPwdEq.Enabled = False
        '					Else
        '						hiddenuserid.Text = "0"
        '					End If
        '					reqPassword.Enabled = False
        '					litTip.Text = "<b>Tip:</b> Only enter password and confirm to override current password."
        '				End If
        '			Case Else
        '				litTip.Text = "<b>Note:</b> Password must be supplied twice to confirm correct entry"
        '				hiddenuserid.Text = "0"
        '		End Select

        '		lblUserName.Text = "User Name"
        '		lblPassword.Text = "Password"
        '		lblConfirmPwd.Text = "Confirm password"
        '		lblFullName.Text = "Full Name"
        '		lblHint.Text = "Password Hint"
        '		lblEmail.Text = "Email"
        '		lblIconSize.Text = "Menu Icon Size"

        '		litLocations.Text = GetUserLocations(FWDb, UserId)
        '		Dim policy As New cPasswordPolicyDescription(curUser.UserFWS)
        '		lblPasswordPolicy.Text = policy.GetPolicyDescription
        '	End If

        '	validatePwdEq.ErrorMessage = "Password and Confirmed Pwd entry do not match."
        '	reqfullname.ErrorMessage = "Full Name definition not specified"
        '	requsername.ErrorMessage = "User ID definition not specified"
        '	reqPassword.ErrorMessage = "Password definition not specified"

        '	cmdOK.ToolTip = "Update system with the new information"
        '	cmdOK.ImageUrl = "./buttons/update.gif"
        '	cmdOK.AlternateText = "Update"
        '	cmdOK.Attributes.Add("onmouseover", "window.status='Update system with the new information';return true;")
        '	cmdOK.Attributes.Add("onmouseout", "window.status='Done';")

        '	'cmdCancel.Attributes.Add("onclick", "window.location.href='UserSetup.aspx';")
        '	cmdCancel.Attributes.Add("onmouseover", "window.status='Exit and return users list';return true;")
        '	cmdCancel.Attributes.Add("onmouseout", "window.status='Done';")

        '	FWDb.DBClose()
        '	FWDb = Nothing
        'End Sub

        'Private Sub UpdateUser()
        '	Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
        '	Dim sql As String
        '	Dim fwdb As New cFWDBConnection
        '	Dim userinfo As UserInfo = curUser.currentUser.userInfo
        '	Dim firstrec As Boolean
        '	Dim drow As DataRow
        '	Dim chkVal As String
        '	Dim defLoc, prevDefLoc As Integer
        '	Dim defLocDesc, prevDefLocDesc As String
        '	Dim fws As cFWSettings = curUser.UserFWS
        '	Dim pwdRetCode As PwdValidateReturnCodes
        '	Dim pwdSeq As Integer
        '	Dim locRoleId As Integer

        '	firstrec = True

        '	Dim users As New cUsers(fws)
        '	Dim ALog As New cFWAuditLog(fws)

        '	lblErrorString.Text = ""

        '	' check that a default location has been selected before proceeding (and ensure that location 0 is not valid
        '	defLoc = Val(Request.Form("DEFLOC"))
        '	If defLoc = 0 And Request.Form("LOC0") <> "on" Then
        '		lblErrorString.Text = "ERROR! A Default Location must be specified."
        '		fwdb = Nothing
        '		Exit Sub
        '	Else
        '		' ensure that the default location chosen is actually ticked as an accessible location!!
        '		If Request.Form("LOC" & Trim(Str(defLoc))) <> "on" Then
        '			lblErrorString.Text = "ERROR! Accessible location must be selected as accessible."
        '			fwdb = Nothing
        '			Exit Sub
        '		End If
        '	End If

        '	If InStr(txtUserName.Text, "'", CompareMethod.Text) > 0 Then
        '		' invalid apostrophe in user name
        '		lblErrorString.Text = "ERROR! Illegal apostrophe specified in username"
        '		fwdb = Nothing
        '		Exit Sub
        '	End If

        '	fwdb.DBOpen(fws, False)

        '	If hiddenuserid.Text <> "0" Then
        '		sql = "SELECT [security_locations].[Location Id],[location].[Description] FROM [security_locations] "
        '		sql += "INNER JOIN [location] ON [security_locations].[Location Id] = [location].[Location Id] "
        '		sql += "WHERE [Default Location] = 1 AND [User Id] = " & hiddenuserid.Text.Trim
        '		fwdb.RunSQL(sql, fwdb.glDBWorkD, False, "", False)

        '		If fwdb.GetRowCount(fwdb.glDBWorkD, 0) > 0 Then
        '			prevDefLoc = Val(fwdb.GetFieldValue(fwdb.glDBWorkD, "Location Id", 0, 0))
        '			prevDefLocDesc = fwdb.GetFieldValue(fwdb.glDBWorkD, "Description", 0, 0)
        '		Else
        '			prevDefLoc = -1
        '			prevDefLocDesc = "n/a"
        '		End If

        '		' must be editing an existing user
        '		Dim editUser As cUser = users.GetUserById(Integer.Parse(hiddenuserid.Text))

        '		If Not curUser Is Nothing Then
        '			ARec.Action = cFWAuditLog.AUDIT_UPDATE
        '			ARec.DataElementDesc = "USER LOGON"

        '			' if username has been modified, ensure that it is not a duplicate
        '			If editUser.UserName.Trim <> txtUserName.Text.Trim Then
        '				Dim tmpUser As cUser = users.GetUserByName(txtUserName.Text.Trim)

        '				If Not tmpUser Is Nothing Then
        '					' cannot use the modified user name as it already exists
        '					If tmpUser.AccountStatus = LogonStatus.Archived Then
        '						lblErrorString.Text = "ERROR! Cannot proceed as modified user name matches an existing archived user."
        '					Else
        '						lblErrorString.Text = "ERROR! Cannot proceed as modified user name already exists on the system."
        '					End If

        '					fwdb.DBClose()
        '					fwdb = Nothing
        '					Exit Sub
        '				End If

        '				editUser.UserName = txtUserName.Text.Trim
        '			End If

        '			editUser.PasswordHint = txtHint.Text.Trim
        '			If txtPassword.Text.Trim <> "" Then
        '				editUser.Password = txtPassword.Text.Trim
        '				editUser.PasswordEncryted = False
        '				If txtConfirmPwd.Text = txtPassword.Text Then
        '					' must be a valid password and matches the confirm
        '					'pwdseq = AllowPassword(fwdb, Val(hiddenuserid.Text), Crypt(txtPassword.Text, 1), fws)
        '					pwdRetCode = cPassword.ValidateNewPassword(fwdb, txtPassword.Text.Trim, fws, Val(hiddenuserid.Text))

        '					Select Case pwdRetCode
        '						Case PwdValidateReturnCodes.Pwd_BadLength
        '							Dim tmpStr As String
        '							tmpStr = "ERROR! Password length is invalid. "
        '							Select Case fws.glPwdLengthSetting
        '								Case PasswordLengthSetting.MustBeBetween
        '									tmpStr += "Must be between " & Trim(Str(fws.glPwdLength1)) & " and " & Trim(Str(fws.glPwdLength2)) & " characters."
        '								Case PasswordLengthSetting.MustBeGreaterThan
        '									tmpStr += "Must be greater than " & Trim(Str(fws.glPwdLength1)) & " characters."
        '								Case PasswordLengthSetting.MustBeLessThan
        '									tmpStr += "Must be less than " & Trim(Str(fws.glPwdLength1)) & " characters."
        '								Case PasswordLengthSetting.MustEqual
        '									tmpStr += "Must be " & Trim(Str(fws.glPwdLength1)) & " characters in length."

        '							End Select

        '							lblErrorString.Text = tmpStr
        '							fwdb.DBClose()
        '							fwdb = Nothing
        '							Exit Sub

        '						Case PwdValidateReturnCodes.Pwd_NoCaps
        '							lblErrorString.Text = "ERROR! Password does not contain the mandatory 1 or more UPPER CASE characters."
        '							fwdb.DBClose()
        '							fwdb = Nothing
        '							Exit Sub

        '						Case PwdValidateReturnCodes.Pwd_NoNum
        '							lblErrorString.Text = "ERROR! Password does not contain the mandatory 1 or more Numeric (0-9) characters."
        '							fwdb.DBClose()
        '							fwdb = Nothing
        '							Exit Sub

        '						Case PwdValidateReturnCodes.Pwd_Previous
        '							' new pwd in recent history
        '							lblErrorString.Text = "ERROR! Password exists in recent history. New Pwd not saved"
        '							fwdb.DBClose()
        '							fwdb = Nothing
        '							Exit Sub

        '						Case PwdValidateReturnCodes.Pwd_OK
        '							' password not in recent history and complies with rules, so ok
        '							' add new password to history
        '							pwdSeq = cPassword.AllowPassword(fwdb, Val(hiddenuserid.Text), Trim(txtPassword.Text), fws, False)

        '							fwdb.SetFieldValue("Password", cPassword.SHA_HashPassword(Trim(txtPassword.Text)), "S", True)
        '							fwdb.SetFieldValue("Password Method", 2, "N", False)
        '							fwdb.SetFieldValue("History Sequence", pwdSeq, "N", False)
        '							fwdb.SetFieldValue("User Id", hiddenuserid.Text, "N", False)
        '							fwdb.FWDb("W", "security_history", "", "", "", "", "", "", "", "", "", "", "", "")
        '					End Select
        '				End If
        '			End If

        '			editUser.FullName = txtFullName.Text.Trim
        '			editUser.Position = txtPosition.Text.Trim
        '			editUser.Email = txtEmail.Text.Trim
        '			editUser.IconSize = Integer.Parse(lstIconSize.SelectedItem.Value)
        '			users.UpdateUser(editUser, userinfo)

        '			' check for updates to the security locations
        '			sql = "SELECT * FROM [location]"
        '			fwdb.RunSQL(sql, fwdb.glDBWorkD, False, "", False)

        '			For Each drow In fwdb.glDBWorkD.Tables(0).Rows
        '				chkVal = Request.Form("LOC" & Trim(drow.Item("Location Id")))
        '				locRoleId = Val(Request.Form("ROLE" & Trim(Str(drow.Item("Location Id")))))

        '				fwdb.FWDb("R2", "security_locations", "User Id", hiddenuserid.Text, "Location Id", drow.Item("Location Id"), "", "", "", "", "", "", "", "")
        '				If fwdb.FWDb2Flag = True Then
        '					If chkVal <> "on" Then
        '						' was selected, must now be deselected, so remove the security location
        '						fwdb.FWDb("D", "security_locations", "User Id", hiddenuserid.Text, "Location Id", drow.Item("Location Id"), "", "", "", "", "", "", "", "")
        '						ARec.Action = cFWAuditLog.AUDIT_DEL
        '						ARec.DataElementDesc = "SECURITY LOCATION"
        '						ARec.ElementDesc = txtFullName.Text
        '						ARec.PostVal = drow.Item("Description")
        '						ALog.AddAuditRec(ARec, True)
        '						ALog.CommitAuditLog(userinfo)
        '					Else
        '						' check to see if the role has changed
        '						If locRoleId <> Val(fwdb.FWDbFindVal("Role Id", 2)) Then
        '							fwdb.SetFieldValue("Role Id", locRoleId, "N", True)
        '							fwdb.FWDb("A", "security_locations", "User Id", hiddenuserid.Text, "Location Id", drow.Item("Location Id"), "", "", "", "", "", "", "", "")

        '							fwdb.FWDb("R3", "user_roles", "Role Id", fwdb.FWDbFindVal("Role Id", 2), "", "", "", "", "", "", "", "", "", "")
        '							If fwdb.FWDb3Flag = True Then
        '								ARec.PreVal = fwdb.FWDbFindVal("Role Name", 3)
        '							Else
        '								ARec.PreVal = ""
        '							End If

        '							fwdb.FWDb("R3", "user_roles", "Role Id", locRoleId, "", "", "", "", "", "", "", "", "", "")
        '							If fwdb.FWDb3Flag = True Then
        '								ARec.PostVal = fwdb.FWDbFindVal("Role Name", 3)
        '							Else
        '								ARec.PostVal = ""
        '							End If

        '							ARec.Action = cFWAuditLog.AUDIT_UPDATE
        '							ARec.DataElementDesc = "SECURITY LOCATION ROLE"
        '							ARec.ElementDesc = txtFullName.Text & ":" & Trim(drow.Item("Description"))
        '							ALog.AddAuditRec(ARec, True)
        '							ALog.CommitAuditLog(userinfo)
        '						End If
        '					End If
        '				Else
        '					If chkVal = "on" Then
        '						' not previously selected, is now, so add it
        '						fwdb.SetFieldValue("Location Id", drow.Item("Location Id"), "N", True)
        '						fwdb.SetFieldValue("User Id", hiddenuserid.Text, "N", False)
        '						fwdb.SetFieldValue("Access", 0, "N", False)
        '						' only store the role if the location is selected for accessibility
        '						fwdb.SetFieldValue("Role Id", locRoleId, "N", False)
        '						fwdb.FWDb("W", "security_locations", "", "", "", "", "", "", "", "", "", "", "", "")

        '						ARec.Action = cFWAuditLog.AUDIT_ADD
        '						ARec.DataElementDesc = "SECURITY LOCATION"
        '						ARec.ElementDesc = txtFullName.Text
        '						ARec.PostVal = drow.Item("Description")
        '						ALog.AddAuditRec(ARec, True)
        '						ALog.CommitAuditLog(userinfo)
        '					End If
        '				End If
        '			Next

        '			If defLoc <> prevDefLoc Then
        '				fwdb.FWDb("R2", "location", "Location Id", defLoc, "", "", "", "", "", "", "", "", "", "")
        '				If fwdb.FWDb2Flag = True Then
        '					defLocDesc = fwdb.FWDbFindVal("Description", 2)
        '				Else
        '					defLocDesc = ""
        '				End If
        '				ARec.Action = cFWAuditLog.AUDIT_UPDATE
        '				ARec.DataElementDesc = "SECURITY LOCATION"
        '				ARec.ElementDesc = "DEFAULT LOCATION"
        '				ARec.ContractNumber = "USER:" & Trim(Left(txtFullName.Text, 50))
        '				ARec.PostVal = defLocDesc
        '				ARec.PreVal = prevDefLocDesc
        '				ALog.AddAuditRec(ARec, True)
        '				ALog.CommitAuditLog(userinfo)
        '				' reset the default location for all of the user's active locations
        '				fwdb.SetFieldValue("Default Location", 0, "N", True)
        '				fwdb.FWDb("A", "security_locations", "User Id", Val(hiddenuserid.Text), "", "", "", "", "", "", "", "", "", "")
        '				' set the single default location
        '				fwdb.SetFieldValue("Default Location", 1, "N", True)
        '				fwdb.FWDb("A", "security_locations", "User Id", Val(hiddenuserid.Text), "Location Id", defLoc, "", "", "", "", "", "", "", "")
        '			End If
        '		End If
        '	Else
        '		If users.UsernameExists(txtUserName.Text.Trim) = False Then
        '			pwdRetCode = cPassword.ValidateNewPassword(fwdb, Trim(txtPassword.Text), fws, 0)

        '			Select Case pwdRetCode
        '				Case PwdValidateReturnCodes.Pwd_BadLength
        '					Dim tmpStr As String
        '					tmpStr = "ERROR! Password length is invalid. "
        '					Select Case fws.glPwdLengthSetting
        '						Case PasswordLengthSetting.MustBeBetween
        '							tmpStr += "Must be between " & Trim(Str(fws.glPwdLength1)) & " and " & Trim(Str(fws.glPwdLength2)) & " characters."
        '						Case PasswordLengthSetting.MustBeGreaterThan
        '							tmpStr += "Must be greater than " & Trim(Str(fws.glPwdLength1)) & " characters."
        '						Case PasswordLengthSetting.MustBeLessThan
        '							tmpStr += "Must be less than " & Trim(Str(fws.glPwdLength1)) & " characters."
        '						Case PasswordLengthSetting.MustEqual
        '							tmpStr += "Must be " & Trim(Str(fws.glPwdLength1)) & " characters in length."

        '					End Select

        '					lblErrorString.Text = tmpStr
        '					fwdb.DBClose()
        '					fwdb = Nothing
        '					Exit Sub

        '				Case PwdValidateReturnCodes.Pwd_NoCaps
        '					lblErrorString.Text = "ERROR! Password does not contain the mandatory 1 or more UPPER CASE characters."
        '					fwdb.DBClose()
        '					fwdb = Nothing
        '					Exit Sub

        '				Case PwdValidateReturnCodes.Pwd_NoNum
        '					lblErrorString.Text = "ERROR! Password does not contain the mandatory 1 or more Numeric (0-9) characters."
        '					fwdb.DBClose()
        '					fwdb = Nothing
        '					Exit Sub

        '				Case PwdValidateReturnCodes.Pwd_Previous
        '					' new pwd in recent history
        '					lblErrorString.Text = "ERROR! Password exists in recent history. New Pwd not saved"
        '					fwdb.DBClose()
        '					fwdb = Nothing
        '					Exit Sub

        '				Case PwdValidateReturnCodes.Pwd_OK
        '					' user name does not exist
        '					Dim newUser As New cUser(fwdb.glIdentity, txtUserName.Text.Trim, txtPassword.Text.Trim, False, txtFullName.Text.Trim, txtPosition.Text.Trim, txtEmail.Text.Trim, txtHint.Text.Trim, lstIconSize.SelectedItem.Value, 0, PwdMethod.SHA_Hash, 0, Today, LogonStatus.Active, "", "", userinfo.xMax, userinfo.yMax, Nothing)
        '					hiddenuserid.Text = users.AddUser(newUser).ToString

        '					ARec.Action = cFWAuditLog.AUDIT_ADD
        '					ARec.PostVal = txtUserName.Text
        '					ARec.DataElementDesc = "USER LOGON"
        '					ALog.AddAuditRec(ARec, True)
        '					ALog.CommitAuditLog(userinfo)

        '					' add password to the history
        '					'pwdseq = AllowPassword(fwdb, Val(hiddenuserid.Text), Crypt(txtPassword.Text, 1), fws)
        '					pwdSeq = cPassword.AllowPassword(fwdb, Val(hiddenuserid.Text), txtPassword.Text.Trim, fws, False)

        '					fwdb.SetFieldValue("User Id", Val(hiddenuserid.Text), "N", True)
        '					fwdb.SetFieldValue("History Sequence", 1, "N", False)
        '					'fwdb.SetFieldValue("Password", Crypt(txtPassword.Text, 1), "S", False)
        '					fwdb.SetFieldValue("Password", cPassword.SHA_HashPassword(Trim(txtPassword.Text)), "S", False)
        '					fwdb.SetFieldValue("Password Method", 2, "N", False)
        '					fwdb.FWDb("WX", "security_history", "", "", "", "", "", "", "", "", "", "", "", "")
        '			End Select

        '			fwdb.FWDb("R2", "security", "User Name", txtUserName.Text, "", "", "", "", "", "", "", "", "", "")
        '			If fwdb.FWDb2Flag = True Then
        '				sql = "SELECT * FROM [location]"
        '				fwdb.RunSQL(sql, fwdb.glDBWorkD, False, "", False)

        '				For Each drow In fwdb.glDBWorkD.Tables(0).Rows
        '					chkVal = Request.Form("LOC" & Trim(drow.Item("Location Id")))
        '					locRoleId = Request.Form("ROLE" & Trim(drow.Item("Location Id")))

        '					If chkVal = "on" Then
        '						fwdb.SetFieldValue("Location Id", drow.Item("Location Id"), "N", True)
        '						fwdb.SetFieldValue("User Id", fwdb.FWDbFindVal("User Id", 2), "N", False)
        '						fwdb.SetFieldValue("Access", 0, "N", False)
        '						If drow.Item("Location Id") = defLoc Then
        '							fwdb.SetFieldValue("Default Location", 1, "N", False)
        '						Else
        '							fwdb.SetFieldValue("Default Location", 0, "N", False)
        '						End If
        '						fwdb.SetFieldValue("Role Id", locRoleId, "N", False)
        '						fwdb.FWDb("W", "security_locations", "", "", "", "", "", "", "", "", "", "", "", "")
        '					End If
        '				Next
        '			End If
        '		Else
        '			' cannot use the modified user name as it already exists
        '			Dim tmpUser As cUser = users.GetUserByName(txtUserName.Text.Trim)
        '			' cannot use the modified user name as it already exists
        '			If tmpUser.AccountStatus = LogonStatus.Archived Then
        '				lblErrorString.Text = "ERROR! Cannot proceed as modified user name matches an existing archived user."
        '			Else
        '				lblErrorString.Text = "ERROR! Cannot proceed as modified user name already exists on the system."
        '			End If

        '			fwdb.DBClose()
        '			fwdb = Nothing
        '			Exit Sub
        '		End If
        '	End If

        '	fwdb.DBClose()
        '	fwdb = Nothing
        '	Response.Redirect("UserSetup.aspx", True)
        'End Sub

        'Private Function GetUserLocations(ByVal db As cFWDBConnection, ByVal userid As Integer) As String
        '	Dim strHTML As New System.Text.StringBuilder
        '	Dim sql As String
        '	Dim drow1, drow2, role_drow As DataRow
        '	Dim hasaccess As Boolean
        '	Dim isDefaultLoc As Boolean
        '	Dim locRoleId As Integer
        '	Dim rowseq As Boolean = False
        '	Dim rowClass As String = "row1"

        '	sql = "SELECT * FROM [location] ORDER BY [Description]"
        '          db.RunSQL(sql, db.glDBWorkC, False, "", False)

        '	sql = "SELECT * FROM [security_locations] WHERE [User Id] = " & userid.ToString
        '          db.RunSQL(sql, db.glDBWorkD, False, "", False)

        '	sql = "SELECT [Role Id],[Role Name] FROM [user_roles] ORDER BY [Role Name]"
        '          db.RunSQL(sql, db.glDBWorkB, False, "", False)

        '	strHTML.Append("<table class=""datatbl"">" & vbNewLine)
        '	strHTML.Append("<tr>" & vbNewLine)
        '	strHTML.Append("<th>Location Name</th>" & vbNewLine)
        '	strHTML.Append("<th>Is Accessible?</th>" & vbNewLine)
        '	strHTML.Append("<th>Default Location</th>" & vbNewLine)
        '	strHTML.Append("<th>Location Role</th>" & vbNewLine)
        '	strHTML.Append("</tr>" & vbNewLine)

        '	For Each drow1 In db.glDBWorkC.Tables(0).Rows
        '		rowseq = (rowseq Xor True)

        '		If rowseq = True Then
        '			rowClass = "row1"
        '		Else
        '			rowClass = "row2"
        '		End If

        '		strHTML.Append("<tr>" & vbNewLine)
        '		strHTML.Append("<td class=""" & rowClass & """>" & drow1.Item("Description") & "</td>" & vbNewLine)

        '		hasaccess = False
        '		isDefaultLoc = False
        '		For Each drow2 In db.glDBWorkD.Tables(0).Rows
        '			If drow2.Item("Location Id") = drow1.Item("Location Id") Then
        '				hasaccess = True
        '				If drow2.Item("Default Location") = 1 Then
        '					isDefaultLoc = True
        '				End If

        '				If IsDBNull(drow2.Item("Role Id")) = False Then
        '					locRoleId = drow2.Item("Role Id")
        '				Else
        '					' must have a role assigned, resort to default role
        '                          db.FWDb("R2", "user_roles", "Default Role", 1, "", "", "", "", "", "", "", "", "", "")
        '					If db.FWDb2Flag = True Then
        '						locRoleId = Val(db.FWDbFindVal("Role Id", 2))
        '						db.SetFieldValue("Role Id", locRoleId, "N", True)
        '                              db.FWDb("A", "security_locations", "Security Location Id", drow2.Item("Security Location Id"), "", "", "", "", "", "", "", "", "", "")
        '					Else
        '						locRoleId = 0
        '					End If
        '				End If
        '				Exit For
        '			End If
        '		Next

        '		Dim strDDL As New System.Text.StringBuilder

        '		strDDL.Append("<SELECT name=""ROLE" & Trim(Str(drow1.Item("Location Id"))) & """ style=""width: 100px;"">" & vbNewLine)

        '		For Each role_drow In db.glDBWorkB.Tables(0).Rows
        '			strDDL.Append("<OPTION value=""" & Trim(role_drow.Item("Role Id")) & """ ")
        '			If role_drow.Item("Role Id") = locRoleId Then
        '				strDDL.Append("selected ")
        '			End If
        '			strDDL.Append("> " & Trim(role_drow.Item("Role Name")) & "</OPTION>" & vbNewLine)
        '		Next
        '		strDDL.Append("</SELECT>" & vbNewLine)

        '		strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=checkbox name=""LOC" & drow1.Item("Location Id") & """ " & IIf(hasaccess = True, "checked", "") & " /></td>" & vbNewLine)
        '		strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=radio name=""DEFLOC"" value=""" & drow1.Item("Location Id") & """ " & IIf(isDefaultLoc = True, "checked ", "") & " /></td>" & vbNewLine)
        '		strHTML.Append("<td class=""" & rowClass & """ align=""center"">" & strDDL.ToString & "</td>" & vbNewLine)
        '		strHTML.Append("</tr>" & vbNewLine)
        '	Next

        '	strHTML.Append("</table>")

        '	GetUserLocations = strHTML.ToString
        'End Function

        '      Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
        '          UpdateUser()
        '      End Sub

        '      Protected Sub txtPassword_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPassword.TextChanged
        '          If hiddenuserid.Text <> "0" Then
        '              Dim active As Boolean = True

        '              If txtPassword.Text.Trim = "" Then
        '                  active = False
        '              End If
        '              reqConfirmPwd.Enabled = active
        '              reqPassword.Enabled = active
        '              valexPwdEq.Enabled = active
        '          End If
        '      End Sub

        '      Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
        '          Response.Redirect("UserSetup.aspx", True)
        '      End Sub
    End Class
End Namespace
