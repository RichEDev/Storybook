Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class TieredDefinitions
        Inherits System.Web.UI.Page
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

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Dim FWDb As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim maintitle As String = ""
			Dim tier1Sql, tier2Sql As String
			Dim strDataName As String
			Dim hasdata As Boolean
			Dim action As String
			Dim DuplicateName As String = ""
			Dim redir As String = ""
			Dim IDFieldName As String = ""
            Dim AudName As String = ""
            Dim currentElement As SpendManagementElement

            FWDb.DBOpen(fws, False)
			hasdata = False

			cmdOK.ToolTip = "Commit modified value to the system"
			cmdOK.AlternateText = "Update"
			cmdOK.TabIndex = 3

            If Me.IsPostBack = False Then
                ' Load headings
                strDataName = LCase(Request.QueryString("item"))

                tier1Sql = ""
                tier2Sql = ""

                Select Case strDataName

                    Case "notecategory"
                        currentElement = SpendManagementElement.NoteCategories
                        maintitle = "Note Categories"
                        Session("dbTableName") = "codes_notecategory"
                        tier1Sql = "SELECT [noteCatId],[noteType],[fullDescription],[Description] FROM [" & Trim(Session("dbTableName")) & "] WHERE [noteType] = 0 ORDER BY [fullDescription]"
                        tier2Sql = "SELECT [noteCatId],[noteType],[fullDescription],[Description] FROM [" & Trim(Session("dbTableName")) & "] WHERE [noteType] <> 0 AND [noteType] <> -1"
                    Case Else

                End Select

                curUser.CheckAccessRole(AccessRoleType.View, currentElement, False, True)

                action = Request.QueryString("action")
                Select Case action
                    Case "add"
                        curUser.CheckAccessRole(AccessRoleType.Add, currentElement, False, True)

                        Master.enablenavigation = False
                        panelNewInput.Visible = True
                        lblDefTitle.Text = "Key Description:  "
                        txtDefInput.Text = ""
                        lblFullDesc.Text = "Full Description: "
                        txtFullDesc.Text = ""
                        cmdClose.Visible = False

                        hiddenID.Text = "0"
                        hiddenParentID.Text = Request.QueryString("parent")
                        If hiddenParentID.Text = "0" Then
                            Title = maintitle & " Parent Definition"
                        Else
                            Title = maintitle & " Child Definition"
                        End If

                    Case "delete"
                        curUser.CheckAccessRole(AccessRoleType.Delete, currentElement, False, True)

                        DeleteDefinition(Request.QueryString("id"), Request.QueryString("parent"), FWDb)

                    Case "edit"
                        curUser.CheckAccessRole(AccessRoleType.Edit, currentElement, False, True)

                        Master.enablenavigation = False
                        panelNewInput.Visible = True
                        lblDefTitle.Text = "Key Description:  "
                        lblFullDesc.Text = "Full Description: "
                        txtFullDesc.Text = ""
                        cmdClose.Visible = False

                        hiddenID.Text = Request.QueryString("id")
                        hiddenParentID.Text = Request.QueryString("parent")

                        If hiddenParentID.Text = "0" Then
                            Title = maintitle & " Parent Definition"
                        Else
                            Title = maintitle & " Child Definition"
                        End If

                        GetFieldKeys(IDFieldName, DuplicateName, redir, AudName)

                        FWDb.FWDb("R2", Session("dbTableName"), IDFieldName, hiddenID.Text, "", "", "", "", "", "", "", "", "", "")

                        txtDefInput.Text = FWDb.FWDbFindVal(DuplicateName, 2)
                        Select Case CStr(Session("dbTableName"))
                            Case "codes_notecategory"
                                txtFullDesc.Text = FWDb.FWDbFindVal("Description", 2)
                        End Select

                    Case Else

                End Select

                If action <> "edit" And action <> "add" Then
                    If tier1Sql <> "" Then
                        FWDb.RunSQL(tier1Sql, FWDb.glDBWorkA, False, "", False)
                    End If

                    If tier2Sql <> "" Then
                        FWDb.RunSQL(tier2Sql, FWDb.glDBWorkB, False, "", False)
                    End If

                    hasdata = True

                    litDefinitions.Text = GetSimpleTierData(Session("dbTableName"), FWDb)
                    Title = maintitle
                End If
                Master.title = Title
            End If

            If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.NoteCategories, False) Then
                lnkNew.ToolTip = "Add a new first tier definition"
                lnkNew.Visible = True
            Else
                lnkNew.Visible = False
            End If

            FWDb.DBClose()
            FWDb = Nothing
		End Sub

		Private Sub GetFieldKeys(ByRef IDFieldName As String, ByRef DupName As String, ByRef redir As String, ByRef AudName As String)
			Select Case CStr(Session("dbTableName"))

				Case "codes_notecategory"
                    IDFieldName = "noteCatId"
                    DupName = "fullDescription"
					redir = "TieredDefinitions.aspx?item=notecategory"
					AudName = "NOTE CATEGORY MAINT"
				Case Else
			End Select
		End Sub

		Private Sub UpdateEntry()
			Dim FWDb As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
			Dim IDFieldName As String = ""
			Dim check4duplicateName As String = ""
			Dim redirectStr As String = ""
			Dim AudName As String = ""
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.NoteCategories, curUser.CurrentSubAccountId)

			FWDb.DBOpen(fws, False)

			GetFieldKeys(IDFieldName, check4duplicateName, redirectStr, AudName)

			ARec.DataElementDesc = AudName

			If txtDefInput.Text <> "" Then
				If hiddenID.Text = "0" Then
					' adding
                    If SMRoutines.DefExists(FWDb, Session("dbTableName"), check4duplicateName, Trim(Replace(txtDefInput.Text, "'", "`")), "", curUser.CurrentSubAccountId) = False Then
                        FWDb.SetFieldValue(check4duplicateName, Trim(Replace(txtDefInput.Text, "'", "`")), "S", True)
                        Select Case CStr(Session("dbTableName"))
                            Case "codes_notecategory"
                                ARec.PreVal = ""
                                ARec.PostVal = Left(Trim(txtDefInput.Text), 50)
                                FWDb.SetFieldValue("noteType", hiddenParentID.Text, "N", False)
                                FWDb.SetFieldValue("Description", txtFullDesc.Text, "S", False)

                            Case Else
                                Exit Sub
                        End Select

                        ARec.Action = cFWAuditLog.AUDIT_ADD
                        ARec.PreVal = ""
                        ARec.PostVal = Trim(txtDefInput.Text)
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, hiddenID.Text)

                        FWDb.FWDb("W", Session("dbTableName"), "", "", "", "", "", "", "", "", "", "", "", "")
                    Else
                        ' cannot add as is a duplicate definition
                        lblErrorString.Text = "ERROR! : Duplicate definition"
                        Exit Sub
                    End If
                Else
                    ' amending
                    Dim firstchange As Boolean

                    firstchange = True

                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    FWDb.FWDb("R2", Session("dbTableName"), IDFieldName, hiddenID.Text, "", "", "", "", "", "", "", "", "", "")
                    If FWDb.FWDb2Flag = True Then
                        If FWDb.FWDbFindVal(check4duplicateName, 1) <> txtDefInput.Text Then
                            If SMRoutines.DefExists(FWDb, Session("dbTableName"), check4duplicateName, Trim(Replace(txtDefInput.Text, "'", "`")), "", -1) = False Then
                                ' no duplicate definition, so permit change
                                FWDb.SetFieldValue(check4duplicateName, txtDefInput.Text, "S", firstchange)
                                firstchange = False

                                ARec.ElementDesc = "Key Definition"
                                ARec.PreVal = FWDb.FWDbFindVal(check4duplicateName, 2)
                                ARec.PostVal = Trim(txtDefInput.Text)
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, hiddenID.Text)
                            End If
                        End If

                        Select Case CStr(Session("dbTableName"))
                            Case "code_tables"
                                If FWDb.FWDbFindVal("Full Description", 1) <> txtFullDesc.Text Then
                                    FWDb.SetFieldValue("Full Description", txtFullDesc.Text, "S", firstchange)
                                    firstchange = False

                                    ARec.ElementDesc = "Full Description"
                                    ARec.PreVal = FWDb.FWDbFindVal("Full Description", 2)
                                    ARec.PostVal = Trim(txtFullDesc.Text)
                                    ALog.AddAuditRec(ARec, True)
                                    ALog.CommitAuditLog(curUser.Employee, hiddenID.Text)
                                End If

                            Case "codes_notecategory"
                                If FWDb.FWDbFindVal("Description", 2) <> txtFullDesc.Text Then
                                    FWDb.SetFieldValue("Description", txtFullDesc.Text, "S", firstchange)
                                    firstchange = False

                                    ARec.ElementDesc = "Description"
                                    ARec.PreVal = FWDb.FWDbFindVal("Description", 2)
                                    ARec.PostVal = Trim(txtFullDesc.Text)
                                    ALog.AddAuditRec(ARec, True)
                                    ALog.CommitAuditLog(curUser.Employee, hiddenID.Text)
                                End If

                            Case Else
                                Exit Sub
                        End Select
                    End If

                    If firstchange = False Then
                        FWDb.FWDb("A", Session("dbTableName"), IDFieldName, hiddenID.Text, "", "", "", "", "", "", "", "", "", "")
                    End If
				End If
			End If

			FWDb.DBClose()
			FWDb = Nothing
			Response.Redirect(redirectStr)
		End Sub

		Private Function GetSimpleTierData(ByVal tbl As String, ByVal db As cFWDBConnection) As String
			Dim strHTML As New System.Text.StringBuilder
			Dim DuplicateName As String = ""
			Dim redir As String = ""
			Dim IDFieldName As String = ""
			Dim AudName As String = ""
			Dim drow1, drow2 As DataRow
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim editurl, closeedit, deleteurl, closedelete, addurl, closeadd As String
			Dim rowseq As Boolean = False
			Dim rowClass As String

			rowseq = False

			GetFieldKeys(IDFieldName, DuplicateName, redir, AudName)

			editurl = "<a href=""" & redir & "&action=edit&id="
			closeedit = """>"
			deleteurl = "<a href=""javascript:if(confirm('Are you sure? OK to confirm.')){window.location.href='" & redir & "&action=delete&id="
			closedelete = "';}"">"
			addurl = "<a href=""" & redir & "&action=add&id=0&parent="
			closeadd = """>"

			strHTML.Append("<div class=""inputpaneltitle"">")
			Select Case tbl
				Case "cost_codes"
					strHTML.Append("Cost Code")
				Case "note_categories"
					strHTML.Append("Note Category")
				Case Else

			End Select
			strHTML.Append(" Definition Administration</div>" & vbNewLine)
            'strHTML.Append("<div style=""overflow: auto; height: " & CStr(((CInt(Session("XRes"))) \ 100) * 60) & "px;"" >")
			strHTML.Append("<table class=""datatbl"">" & vbNewLine)

			For Each drow1 In db.glDBWorkA.Tables(0).Rows
				strHTML.Append("<tr>" & vbNewLine)
				strHTML.Append("<th class=""datatbl""><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
				strHTML.Append("<th class=""datatbl""><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
				strHTML.Append("<th class=""datatbl"">Add Child?</th>" & vbNewLine)
				strHTML.Append("<th class=""datatbl"">Key Description</th>" & vbNewLine)
				strHTML.Append("<th class=""datatbl"">Full Description</th>" & vbNewLine)
				strHTML.Append("</tr>")

				rowseq = (rowseq Xor True)
				If rowseq = True Then
					rowClass = "row1"
				Else
					rowClass = "row2"
				End If

				strHTML.Append("<tr>" & vbNewLine)

				Select Case CStr(Session("dbTableName"))

					Case "codes_notecategory"
                        If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.NoteCategories, False) Then
                            strHTML.Append("<td class=""" & rowClass & """>" & editurl & drow1.Item("noteCatId") & closeedit & "<img src=""./icons/edit.gif"" /></a></td>" & vbNewLine)
                        Else
                            strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                        End If

                        If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.NoteCategories, False) Then
                            strHTML.Append("<td class=""" & rowClass & """>" & deleteurl & drow1.Item("noteCatId") & "&parent=" & drow1.Item("noteType") & closedelete & "<img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                        Else
                            strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                        End If

                        If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.NoteCategories, False) Then
                            strHTML.Append("<td class=""" & rowClass & """>" & addurl & drow1.Item("noteCatId") & closeadd & "Add Child</a></td>" & vbNewLine)
                        Else
                            strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                        End If

                        strHTML.Append("<td class=""" & rowClass & """>" & drow1.Item("fullDescription") & "</td>" & vbNewLine)
						strHTML.Append("<td class=""" & rowClass & """>" & drow1.Item("Description") & "</td>" & vbNewLine)

					Case Else
				End Select

				strHTML.Append("</tr>" & vbNewLine)

				' display any 2nd tier definitions
				strHTML.Append("<tr>" & vbNewLine)
				strHTML.Append("<td colspan=""2""></td>" & vbNewLine)
				strHTML.Append("<td colspan=""3"">" & vbNewLine)
				strHTML.Append("<table class=""datatbl""><tr>" & vbNewLine)
				'strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
				strHTML.Append("<th><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
				strHTML.Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
				strHTML.Append("<th>Key Description</th>" & vbNewLine)
				strHTML.Append("<th>Full Description</th>" & vbNewLine)
				strHTML.Append("</tr>" & vbNewLine)

				Dim hasTier2Data As Boolean = False
				Dim rowClass2 As String = "row1"
				Dim rowalt2 As Boolean = rowseq

				For Each drow2 In db.glDBWorkB.Tables(0).Rows
					Select Case CStr(Session("dbTableName"))
						Case "codes_notecategory"
                            If drow2.Item("noteType") = drow1.Item("noteCatId") Then
                                hasTier2Data = True

                                rowalt2 = (rowalt2 Xor True)
                                If rowalt2 Then
                                    rowClass2 = "row1"
                                Else
                                    rowClass2 = "row2"
                                End If

                                strHTML.Append("<tr>" & vbNewLine)
                                'strHTML.Append("<td class=""main"">&nbsp;</td>" & vbNewLine)

                                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.NoteCategories, False) Then
                                    strHTML.Append("<td class=""" & rowClass2 & """>" & editurl & drow2.Item("noteCatId") & closeedit & "<img src=""./icons/edit.gif"" /></a></td>" & vbNewLine)
                                Else
                                    strHTML.Append("<td class=""" & rowClass2 & """>&nbsp;</td>" & vbNewLine)
                                End If

                                If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.NoteCategories, False) Then
                                    strHTML.Append("<td class=""" & rowClass2 & """>" & deleteurl & drow2.Item("noteCatId") & "&parent=" & drow2.Item("noteType") & closedelete & "<img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                                Else
                                    strHTML.Append("<td class=""" & rowClass2 & """>&nbsp;</td>" & vbNewLine)
                                End If

                                strHTML.Append("<td class=""" & rowClass2 & """>" & drow2.Item("fullDescription") & "</td>" & vbNewLine)
                                strHTML.Append("<td class=""" & rowClass2 & """>" & drow2.Item("Description") & "</td>" & vbNewLine)
                                strHTML.Append("</tr>" & vbNewLine)
                            End If

						Case Else

					End Select
				Next

				If hasTier2Data = False Then
					strHTML.Append("<td colspan=""4"" class=""" & rowClass2 & """ align=""center"">None defined</td>" & vbNewLine)
				End If
				strHTML.Append("</table>" & vbNewLine & "</td>" & vbNewLine)
				strHTML.Append("<td colspan=""5"">&nbsp;</td>")
				strHTML.Append("</tr>" & vbNewLine)
			Next

			strHTML.Append("</table>" & vbNewLine)
            'strHTML.Append("</div>")

			GetSimpleTierData = strHTML.ToString
		End Function

		Private Sub AddNewParent()
			Dim DuplicateName As String = ""
			Dim redir As String = ""
			Dim IDFieldName As String = ""
			Dim AudName As String = ""

			GetFieldKeys(IDFieldName, DuplicateName, redir, AudName)

			Response.Redirect(redir & "&action=add&id=0&parent=0", True)
		End Sub

		Private Sub DeleteDefinition(ByVal id As Integer, ByVal parentid As Integer, ByVal fwdb As cFWDBConnection)
			Dim DuplicateName As String = ""
			Dim redir As String = ""
			Dim IDFieldName As String = ""
			Dim AudName As String = ""
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.NoteCategories, curUser.CurrentSubAccountId)

			GetFieldKeys(IDFieldName, DuplicateName, redir, AudName)

			If SMRoutines.CheckRefIntegrity(fwdb, Session("dbTableName"), id) = False Then
				' ok to delete
				fwdb.FWDb("R2", Session("dbTableName"), IDFieldName, id, "", "", "", "", "", "", "", "", "", "")
				If fwdb.FWDb2Flag = True Then
					ARec.Action = cFWAuditLog.AUDIT_DEL
					ARec.DataElementDesc = AudName
					ARec.ElementDesc = fwdb.FWDbFindVal(DuplicateName, 2)
					ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, id)

					fwdb.FWDb("D", Session("dbTableName"), IDFieldName, id, "", "", "", "", "", "", "", "", "", "")
				End If
			Else
				lblErrorString.Text = "ERROR! Cannot perform action as entity is currently assigned."
			End If
		End Sub

        Private Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            UpdateEntry()
        End Sub

        Protected Sub lnkNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNew.Click
            AddNewParent()
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Dim url As String

            Select Case CStr(Session("dbTableName"))
                Case "codes_notecategory"
                    url = "TieredDefinitions.aspx?item=notecategory"
                Case Else
                    url = "MenuMain.aspx?menusection=tailoring"
            End Select

            Response.Redirect(url, True)
        End Sub

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            Response.Redirect("MenuMain.aspx?menusection=tailoring", True)
        End Sub
    End Class
End Namespace
