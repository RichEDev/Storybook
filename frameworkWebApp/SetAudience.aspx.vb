Imports System.Collections.Generic
Imports SpendManagementLibrary
Imports Spend_Management
Imports FWClasses

Namespace Framework2006
    Partial Class SetAudience
        Inherits System.Web.UI.Page

        <Serializable()>
        Private Structure Selected
            Dim UorE As TeamType ' user or employee
            Dim entityId As Integer
            Dim TorI As AudienceType ' team or individual
        End Structure

        Dim teamList As ListItem()

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim helpID As String = "#0"

            Title = "Select Approved Audience"
            Master.title = Title
            Master.enablenavigation = False

            If Me.IsPostBack = False Then
                Dim arlSelected As New System.Collections.ArrayList
                Dim db As New cFWDBConnection
                db.DBOpen(fws, False)

                hiddenEntityId.Text = Request.QueryString("id")

                Select Case Request.QueryString("type")
                    Case "contract"
                        curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractAudience, False, True)

                        hiddenAudienceTable.Text = "contract_audience"
                        hiddenAudienceField.Text = "ContractId"
                        arlSelected = GetCurrentSelections(db, hiddenAudienceTable.Text, hiddenEntityId.Text)
                        Session("arlSelected") = arlSelected
                        GetUserAudience(db, fws, curUser, hiddenEntityId.Text, arlSelected)
                        helpID = "#1074"

                    Case "attachment"
                        curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentAudience, False, True)
                        hiddenAudienceTable.Text = "attachment_audience"
                        hiddenAudienceField.Text = "AttachmentId"
                        arlSelected = GetCurrentSelections(db, hiddenAudienceTable.Text, hiddenEntityId.Text)
                        Session("arlSelected") = arlSelected
                        GetUserAudience(db, fws, curUser, hiddenEntityId.Text, arlSelected)
                        helpID = "#1023"
                    Case Else

                End Select

                db.DBClose()
                db = Nothing
            End If
            
            cmdOK.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractAudience, False)
        End Sub

        Private Function IsSelected(ByVal entityId As Integer, ByVal audience_type As AudienceType, ByVal arl As System.Collections.ArrayList) As Boolean
            Dim curSel As Selected
            Dim x As Integer = 0
            Dim retVal As Boolean = False

            For x = 0 To arl.Count - 1
                curSel = arl.Item(x)
                If curSel.TorI = audience_type And curSel.entityId = entityId Then
                    retVal = True
                    Exit For
                End If
            Next

            Return retVal
        End Function

        Private Sub GetUserAudience(ByVal db As cFWDBConnection, ByVal fws As cFWSettings, ByVal curuser As CurrentUser, ByVal curId As Integer, ByVal arlSelected As System.Collections.ArrayList)
            Dim sql As New System.Text.StringBuilder
            Dim drow As DataRow

            Dim lstItems As New List(Of ListItem)
            Dim empsItem As ListItem

            ' update any user selections
            'sql.Append("SELECT employeeId, username, firstname + ' ' + surname AS [FullName] FROM employees WHERE (LOWER(username) not like 'admin%') AND archived = 0 AND EXISTS (SELECT employeeid FROM employeeAccessRoles WHERE subAccountId = @subAccountId) ORDER BY [FullName]")
            sql.Append("SELECT DISTINCT e.username, e.employeeId, e.[surname] + ', ' + e.[title] + ' ' + e.[firstname] AS [empname] FROM employees AS e INNER JOIN employeeAccessRoles AS ear ON e.employeeid = ear.employeeID INNER JOIN accessRoleElementDetails AS ared ON ear.accessRoleID = ared.roleID WHERE (LOWER(e.username) NOT LIKE 'admin%') AND e.archived = 0 AND ear.subAccountId = @subAccountId AND ared.elementID = @contractElementID AND ared.viewAccess = 1 ORDER BY [empname]")
            db.AddDBParam("contractElementID", SpendManagementElement.ContractDetails, True)
            db.AddDBParam("subAccountId", curuser.CurrentSubAccountId, False)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            For Each drow In db.glDBWorkA.Tables(0).Rows
                empsItem = New ListItem
                empsItem.Value = drow.Item("employeeId")
                empsItem.Text = drow.Item("empname") + " (" + drow.Item("username") + ")"
                lstItems.Add(empsItem)
            Next

            Dim lstItem As ListItem() = lstItems.ToArray

            'Dim employees As New cEmployees(fws.MetabaseCustomerId)

            'Dim lstItem As ListItem() = employees.CreateDropDown(0, fws.MetabaseCustomerId)
            'Dim lstItem As ListItem() = employees.CreateDropDown(0, False)
            chkPersonList.Items.AddRange(lstItem)

            For Each empItem As ListItem In lstItem
                chkPersonList.Items.FindByValue(empItem.Value).Selected = IsSelected(empItem.Value, AudienceType.Individual, arlSelected)
            Next

            Dim teams As New cTeams(curuser.AccountID, curuser.CurrentSubAccountId)
            'Dim uteams As SortedList(Of Integer, cTeam) = teams.list

            'For Each i As KeyValuePair(Of Integer, cTeam) In uteams
            '    Dim curTeam As cTeam = CType(i.Value, cTeam)

            '    Dim item As New ListItem
            '    item.Text = curTeam.teamname
            '    item.Value = curTeam.teamid
            '    item.Selected = IsSelected(curTeam.teamid, AudienceType.Team, arlSelected)

            '    chkTeamList.Items.Add(item)
            'Next

            teamList = teams.CreateDropDown(-1)

            For Each currTeamItem In teamList
                currTeamItem.Selected = IsSelected(currTeamItem.Value.ToString, AudienceType.Team, arlSelected)
            Next

            chkTeamList.Items.AddRange(teamList)

            'sql.Remove(0, sql.Length)
            'sql.Append("SELECT [Team Id],[Team Name] FROM [teams] WHERE [Location Id] = @locId AND [Team Type] = " & TeamType.User & " ORDER BY [Team Name]")
            'db.AddDBParam("locId", uinfo.ActiveLocation, True)
            'db.RunSQL(sql.ToString, db.glDBWorkB)

            'For Each drow In db.glDBWorkB.Tables(0).Rows
            '    Dim item As New ListItem
            '    item.Text = drow.Item("Team Name")
            '    item.Value = drow.Item("Team Id")
            '    item.Selected = IsSelected(drow.Item("Team Id"), AudienceType.Team, arlSelected)

            '    chkTeamList.Items.Add(item)
            'Next
        End Sub

        Private Sub GetEmployeeAudience(ByVal fws As cFWSettings, ByVal curuser As CurrentUser)
            Dim employees As New cEmployees(curuser.AccountID)
            chkPersonList.Items.AddRange(employees.CreateDropDown(0, False))

            Dim empteams As New cTeams(curuser.AccountID, curuser.CurrentSubAccountId)
            chkTeamList.Items.AddRange(empteams.CreateDropDown(0))
        End Sub

        Private Function GetCurrentSelections(ByVal db As cFWDBConnection, ByVal table As String, ByVal curId As Integer) As System.Collections.ArrayList
            Dim al As New System.Collections.ArrayList
            Dim sql As New System.Text.StringBuilder
            Dim curSelection As Selected

            curSelection.UorE = TeamType.User

            sql.Append("SELECT * FROM ")
            sql.Append(table)
            sql.Append(" WHERE ")
            Select Case table
                Case "contract_audience"
                    sql.Append("[ContractId] = ")
                Case "attachment_audience"
                    sql.Append("[AttachmentId] = ")
                Case Else
                    Return al
                    Exit Function
            End Select
            sql.Append(curId.ToString)

            db.RunSQL(sql.ToString, db.glDBWorkL, False, "", False)
            Dim drow As DataRow

            For Each drow In db.glDBWorkL.Tables(0).Rows
                curSelection.TorI = CType(drow.Item("AudienceType"), AudienceType)
                curSelection.entityId = drow.Item("AccessId")
                al.Add(curSelection)
            Next

            Return al
        End Function

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            ExitAudience()
        End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            If UpdateSelections(hiddenAudienceTable.Text, hiddenAudienceField.Text, Integer.Parse(hiddenEntityId.Text)) = True Then
                ExitAudience()
            Else
                lblErrorMessage.Text = "ERROR: An error has occurred while trying to update audience selection"
            End If
        End Sub

        Private Sub ExitAudience()
            If Not Session("AudienceReturnURL") Is Nothing Then
                Response.Redirect(Session("AudienceReturnURL"), True)
            Else
                Response.Redirect("Home.aspx", True)
            End If
        End Sub

        Private Function UpdateSelections(ByVal audienceTable As String, ByVal entityField As String, ByVal entityID As Integer) As Boolean
            Dim updSuccess As Boolean = False
            Dim arlSelected As New System.Collections.ArrayList
            Dim sql As New System.Text.StringBuilder
            Dim db As New cFWDBConnection
            Dim drow As DataRow
            Dim ARec As New cAuditRecord
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Audiences, curUser.CurrentSubAccountId)

            db.DBOpen(fws, False)


            arlSelected = Session("arlSelected")

            Select Case audienceTable
                Case "contract_audience"
                    db.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb2Flag Then
                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                    Else
                        ARec.ContractNumber = ""
                    End If

                    ARec.DataElementDesc = "CONTRACT AUDIENCE INDIVIDUAL"

                Case "attachment_audience"
                    ARec.ContractNumber = ""
                    ARec.DataElementDesc = "ATTACHMENT AUDIENCE INDIVIDUAL"

                Case Else
                    ARec.ContractNumber = ""
                    ARec.DataElementDesc = "AUDIENCE INDIVIDUAL"
            End Select

            ARec.Action = cFWAuditLog.AUDIT_UPDATE

            ' update any user selections
            'sql.Append("SELECT [User Id],[Full Name] FROM [security] WHERE LOWER([User Name]) <> 'admin' AND [Status] = @logonstatus ORDER BY [Full Name]")
            'db.AddDBParam("logonstatus", LogonStatus.Active, True)
            sql.Append("SELECT DISTINCT e.username, e.employeeId, e.[surname] + ', ' + e.[title] + ' ' + e.[firstname] AS [empname] FROM employees AS e INNER JOIN employeeAccessRoles AS ear ON e.employeeid = ear.employeeID INNER JOIN accessRoleElementDetails AS ared ON ear.accessRoleID = ared.roleID WHERE (LOWER(e.username) NOT LIKE 'admin%') AND e.archived = 0 AND ear.subAccountId = @subAccountId AND ared.elementID = @contractElementID AND ared.viewAccess = 1 ORDER BY [empname]")
            db.AddDBParam("contractElementID", SpendManagementElement.ContractDetails, True)
            db.AddDBParam("subAccountId", curUser.CurrentSubAccountId, False)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            For Each drow In db.glDBWorkA.Tables(0).Rows
                If Not chkPersonList.Items.FindByValue(drow.Item("employeeId")) Is Nothing Then
                    If chkPersonList.Items.FindByValue(drow.Item("employeeId")).Selected = True Then
                        ' currently checked, see if was unchecked
                        If IsSelected(drow.Item("employeeId"), AudienceType.Individual, arlSelected) = False Then
                            ' changed, so update
                            ARec.ElementDesc = chkPersonList.Items.FindByValue(drow.Item("employeeId")).Text.ToUpper
                            ARec.PreVal = "UNCHECKED"
                            ARec.PostVal = "CHECKED"
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, entityID)

                            db.SetFieldValue(entityField, entityID, "N", True)
                            db.SetFieldValue("AudienceType", AudienceType.Individual, "N", False)
                            db.SetFieldValue("AccessId", drow.Item("employeeId"), "N", False)
                            db.FWDb("W", audienceTable, "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    Else
                        ' is not currently checked, was it before update
                        If IsSelected(drow.Item("employeeId"), AudienceType.Individual, arlSelected) = True Then
                            ' changed, so update
                            ARec.ElementDesc = chkPersonList.Items.FindByValue(drow.Item("employeeId")).Text.ToUpper
                            ARec.PreVal = "CHECKED"
                            ARec.PostVal = "UNCHECKED"
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, entityID)

                            db.FWDb("D", audienceTable, entityField, entityID, "AudienceType", AudienceType.Individual, "AccessId", drow.Item("employeeId"), "", "", "", "", "", "")
                        End If
                    End If
                End If
            Next

            ' update any team selections
            Select Case audienceTable
                Case "contract_audience"
                    ARec.DataElementDesc = "CONTRACT AUDIENCE TEAM"
                Case "attachment_audience"
                    ARec.DataElementDesc = "ATTACHMENT AUDIENCE TEAM"
                Case Else
                    ARec.DataElementDesc = "AUDIENCE TEAM"
            End Select

            sql.Remove(0, sql.Length)
            sql.Append("SELECT [teamid],[teamname] FROM [teams] WHERE [subaccountid] = @locId ORDER BY [teamname]")
            db.AddDBParam("locId", curUser.CurrentSubAccountId, True)
            db.RunSQL(sql.ToString, db.glDBWorkB, False, "", False)

            For Each drow In db.glDBWorkB.Tables(0).Rows
                If chkTeamList.Items.FindByValue(drow.Item("teamid")).Selected = True Then
                    ' currently checked, see if was unchecked
                    If IsSelected(drow.Item("teamid"), AudienceType.Team, arlSelected) = False Then
                        ' changed, so update
                        ARec.ElementDesc = chkTeamList.Items.FindByValue(drow.Item("teamid")).Text.ToUpper
                        ARec.PreVal = "UNCHECKED"
                        ARec.PostVal = "CHECKED"
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, entityID)

                        db.SetFieldValue(entityField, entityID, "N", True)
                        db.SetFieldValue("AudienceType", AudienceType.Team, "N", False)
                        db.SetFieldValue("AccessId", drow.Item("teamid"), "N", False)
                        db.FWDb("W", audienceTable, "", "", "", "", "", "", "", "", "", "", "", "")
                    End If
                Else
                    ' is not currently checked, was it before update
                    If IsSelected(drow.Item("teamid"), AudienceType.Team, arlSelected) = True Then
                        ' changed, so update
                        ARec.ElementDesc = chkTeamList.Items.FindByValue(drow.Item("teamid")).Text.ToUpper
                        ARec.PreVal = "CHECKED"
                        ARec.PostVal = "UNCHECKED"
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, entityID)

                        db.FWDb("D", audienceTable, entityField, entityID, "AudienceType", AudienceType.Team, "AccessId", drow.Item("teamid"), "", "", "", "", "", "")
                    End If
                End If
            Next

            updSuccess = True

            Return updSuccess
        End Function

    End Class
End Namespace
