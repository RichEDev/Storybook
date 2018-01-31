Imports SpendManagementLibrary
Imports Spend_Management
Imports FWClasses

Namespace Framework2006
    Partial Class ScheduleManage
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
            Dim helpID As String = "#1081"
            Dim action As String
            action = Request.QueryString("action")
            ViewState("ContractId") = Request.QueryString("cid")

            Dim curuser As CurrentUser = cMisc.GetCurrentUser()

            If Me.IsPostBack = False Then
                Dim cId As Integer

                Session("ScheduleAction") = action

                Select Case action
                    Case "merge"
                        curuser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractSchedules, False, True)
                        Title = "Merge Contract Schedules"
                        searchPanel.Visible = False

                        cId = Val(Request.QueryString("cid"))
                        Session("ScheduleContractId") = cId

                        litScheduleHTML.Text = GetSchedulesForMerge(cId)
                        helpID = "#1085"

                    Case "addlinks"
                        curuser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractSchedules, False, True)
                        Title = "Add Schedule Links to Current Contract"

                        cId = Val(Request.QueryString("id"))
                        Session("ScheduleContractId") = cId
                        helpID = "#1086"

                    Case "createlinks"
                        curuser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractSchedules, False, True)
                        Title = "Create New Contract Schedule Links"
                        Session("ScheduleContractId") = Nothing
                        helpID = "#1084"

                    Case Else

                End Select
            End If

            Master.enablenavigation = False
            Master.title = Title

            ' button code
            Select Case action
                Case "merge"
                    cmdOK.AlternateText = "Perform Merge"
                    cmdOK.ToolTip = "Perform Merge"

                Case "addlinks"
                    cmdOK.AlternateText = "Add Links"
                    cmdOK.ToolTip = "Add Links"

                Case "createlinks"
                    cmdOK.AlternateText = "Create Links"
                    cmdOK.ToolTip = "Create Links"

                Case Else
                    cmdOK.AlternateText = "OK"
                    cmdOK.ToolTip = "OK"

            End Select

            cmdOK.Attributes.Add("onmouseover", "window.status='Commit selected updates';return true;")
            cmdOK.Attributes.Add("onmouseout", "window.status='Done';")
            cmdOK.Attributes.Add("style", "padding-right:4px;")

            If action <> "merge" Then
                cmdOK.Visible = False
                cmdCancel.ImageUrl = "~/buttons/page_close.png"
                Master.useCloseNavigationMsg = True
            End If

            cmdCancel.AlternateText = "Cancel"
            cmdCancel.ToolTip = "Abort decision to merge schedules"
            cmdCancel.Attributes.Add("onmouseover", "window.status='Abandon decision to merge schedules';return true;")
            cmdCancel.Attributes.Add("onmouseout", "window.status='Done';")

            If action <> "merge" Then
                cmdSearch.ToolTip = "Search for Contracts"
                cmdSearch.AlternateText = "Search"
                cmdSearch.Attributes.Add("onmouseover", "window.status='Search for contract matches';return true;")
                cmdSearch.Attributes.Add("onmouseout", "window.status='Done';")
            End If
        End Sub

        Private Function GetSchedulesForMerge(ByVal cId As Integer) As String
            Dim db As New cFWDBConnection
            Dim sql As New System.Text.StringBuilder
            Dim strHTML As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim rowClass As String = "row1"
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Try
                Dim supplierStr As String = params.SupplierPrimaryTitle

                db.DBOpen(fws, False)

                sql.Append("SELECT [link_matrix].[LinkId],[link_matrix].[ContractId],[contract_details].[ContractKey],[contract_details].[ContractNumber],[contract_details].[ContractDescription],[contract_details].[ScheduleNumber],[contract_details].[Archived],[supplier_details].[suppliername] ")
                sql.Append("FROM [link_matrix] ")
                sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [link_matrix].[ContractId] ")
                sql.Append("INNER JOIN [supplier_details] ON [supplier_details].[supplierid] = [contract_details].[supplierId] ")
                sql.Append("WHERE [LinkId] = (SELECT [link_matrix].[LinkId] FROM [link_matrix] ")
                sql.Append("INNER JOIN [link_definitions] ON [link_matrix].[LinkId] = [link_definitions].[LinkId] ")
                sql.Append("WHERE [link_definitions].[IsScheduleLink] = 1 AND [ContractId] = @conId)")
                db.AddDBParam("conId", cId, True)
                db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

                ' use hidden text box to store , delimited list of contract id's
                txtSelectCriteria.Text = ""

                strHTML.Append("<table class=""datatbl"">" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<th>Use as Primary</th>" & vbNewLine)
                strHTML.Append("<th>Select for merge</th>" & vbNewLine)
                strHTML.Append("<th>Contract Key</th>" & vbNewLine)
                strHTML.Append("<th>Contract Number</th>" & vbNewLine)
                strHTML.Append("<th>Contract Description</th>" & vbNewLine)
                strHTML.Append("<th>Schedule Number</th>" & vbNewLine)
                strHTML.Append("<th>" & supplierStr & "</th>" & vbNewLine)
                strHTML.Append("<th>Archived Contract?</th>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)

                Dim rowalt As Boolean = False

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    rowalt = (rowalt Xor True)

                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    If Trim(txtSelectCriteria.Text) <> "" Then
                        txtSelectCriteria.Text += ","
                    End If
                    txtSelectCriteria.Text += Trim(Str(drow.Item("ContractId")))
                    strHTML.Append("<tr>" & vbNewLine)

                    ' once contract should be used for primary detail in the new merged contract
                    strHTML.Append("<td class=""" & rowClass & """ align=""center"">")
                    strHTML.Append("<input type=""radio"" name=""Primary"" value=""" & Trim(Str(drow.Item("ContractId"))) & """")
                    ' by default, only the active schedule is checked
                    If drow.Item("ContractId") = cId Then
                        strHTML.Append(" checked")
                    End If
                    strHTML.Append(" /></td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """ align=""center"">")
                    strHTML.Append("<input type=""checkbox"" name=""chk" & Trim(Str(drow.Item("ContractId"))) & """")
                    If drow.Item("ContractId") = cId Then
                        ' active contract, so is checked
                        strHTML.Append(" checked")
                    End If
                    strHTML.Append(" />")
                    strHTML.Append("</td>" & vbNewLine)

                    strHTML.Append("<td class=""" & rowClass & """>")
                    If IsDBNull(drow.Item("ContractKey")) = True Then
                        strHTML.Append("&nbsp;")
                    Else
                        strHTML.Append(Trim(drow.Item("ContractKey")))
                    End If
                    strHTML.Append("</td>" & vbNewLine)

                    strHTML.Append("<td class=""" & rowClass & """>")
                    If IsDBNull(drow.Item("ContractNumber")) = True Then
                        strHTML.Append("&nbsp;")
                    Else
                        strHTML.Append(Trim(drow.Item("ContractNumber")))
                    End If
                    strHTML.Append("</td>" & vbNewLine)

                    ' contract description cannot be null!
                    strHTML.Append("<td class=""" & rowClass & """><a href=""ContractSummary.aspx?id=" & Trim(drow.Item("ContractId")) & """>" & Trim(drow.Item("ContractDescription")) & "</a></td>" & vbNewLine)

                    strHTML.Append("<td class=""" & rowClass & """>")
                    If IsDBNull(drow.Item("ScheduleNumber")) = True Then
                        strHTML.Append("&nbsp;")
                    Else
                        strHTML.Append(Trim(drow.Item("ScheduleNumber")))
                    End If
                    strHTML.Append("</td>" & vbNewLine)

                    strHTML.Append("<td class=""" & rowClass & """>" & Trim(drow.Item("suppliername")) & "</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=""checkbox"" disabled ")
                    If drow.Item("Archived") = "Y" Then
                        strHTML.Append("checked")
                    End If
                    strHTML.Append("/></td>" & vbNewLine)
                    strHTML.Append("</tr>" & vbNewLine)
                Next

                strHTML.Append("</table>")

                db.DBClose()
                db = Nothing

            Catch ex As Exception
                strHTML = New System.Text.StringBuilder
                strHTML.Append("<table class=""datatbl"">" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<th>Error obtaining schedule data for merge</th>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""row1"">" & ex.Message & "</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
                strHTML.Append("</table>")
            End Try

            GetSchedulesForMerge = strHTML.ToString
        End Function

        Private Sub PerformAction()
            Dim drow As DataRow
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim sql As System.Text.StringBuilder

            Try
                Select Case Session("ScheduleAction")
                    Case "merge"
                        ' perform the merge of selected contracts
                        PerformMerge()

                    Case "addlinks"
                        ' adding links to an existing schedule link chain
                        sql = New System.Text.StringBuilder
                        sql.Append("SELECT [Archived],[ContractId],[ContractKey],[ContractDescription],[ContractNumber],[supplier_details].[suppliername] ")
                        sql.Append("FROM [contract_details] ")
                        sql.Append("INNER JOIN [supplier_details] ON [contract_details].[supplierId] = [supplier_details].[supplierid] ")
                        sql.Append("WHERE [contract_details].[subAccountId] = @subAccId AND ")
                        If txtSelectCriteria2.Text.Trim <> "" Then
                            sql.Append("([ContractDescription] LIKE '%" & txtSelectCriteria.Text.Trim & "%' OR [ContractDescription] LIKE '%" & txtSelectCriteria2.Text.Trim & "%') ")
                        Else
                            sql.Append("[ContractDescription] LIKE '%" & txtSelectCriteria.Text.Trim & "%' ")
                        End If
                        db.DBOpen(fws, False)
                        db.AddDBParam("subAccId", curUser.CurrentSubAccountId, True)
                        db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

                        Dim isFirst As Boolean = True
                        Dim linkId, linkCount As Integer

                        linkCount = 0

                        ' get the link id for the current contract
                        sql = New System.Text.StringBuilder
                        sql.Append("SELECT [link_matrix].[LinkId] FROM [link_matrix] ")
                        sql.Append("LEFT OUTER JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] ")
                        sql.Append("WHERE [IsScheduleLink] = 1 AND [ContractId] = " & Trim(Str(Session("ScheduleContractId"))))
                        db.RunSQL(sql.ToString, db.glDBWorkB, False, "", False)
                        If db.glNumRowsReturned > 0 Then
                            linkId = db.GetFieldValue(db.glDBWorkB, "LinkId", 0, 0)
                        Else
                            ' haven't got link, so can't continue
                            litScheduleHTML.Text = "<table class=""datatbl""><tr><td class=""row1"">Could not find existing contract link to associate with.</td></tr></table>"
                            db.DBClose()
                            Exit Select
                        End If

                        For Each drow In db.glDBWorkA.Tables(0).Rows
                            ' new link for all ticked contracts (except the existing one).
                            If Request.Form("CON" & Trim(Str(drow.Item("ContractId")))) = "on" Then
                                db.FWDb("R", "link_matrix", "ContractId", drow.Item("ContractId"), "LinkId", linkId, "", "", "", "", "", "", "", "")
                                If db.FWDbFlag = False Then
                                    ' link for ticked contract doesn't exist, so create
                                    db.SetFieldValue("LinkId", linkId, "N", True)
                                    db.SetFieldValue("ContractId", drow.Item("ContractId"), "N", False)
                                    db.SetFieldValue("IsArchived", IIf(drow.Item("Archived") = "Y", 1, 0), "N", False)
                                    db.FWDb("WX", "link_matrix", "", "", "", "", "", "", "", "", "", "", "", "")
                                End If

                                linkCount += 1
                            End If
                        Next

                        litScheduleHTML.Text = "<table class=""datatbl""><tr><td class=""row1"">" & linkCount.ToString & " Contract(s) added to the schedules chain.</td></tr></table>"
                        cmdOK.Visible = False

                        db.DBClose()

                    Case "createlinks"
                        ' creating a new schedule link chain
                        sql = New System.Text.StringBuilder
                        sql.Append("SELECT [Archived],[ContractId],[ContractKey],[ContractDescription],[ContractNumber],[supplier_details].[suppliername] ")
                        sql.Append("FROM [contract_details] ")
                        sql.Append("INNER JOIN [supplier_details] ON [contract_details].[supplierId] = [supplier_details].[supplierid] ")
                        sql.Append("WHERE [contract_details].[subAccountId] = " & curUser.CurrentSubAccountId.ToString & " AND ")
                        If txtSelectCriteria2.Text.Trim <> "" Then
                            sql.Append("([ContractDescription] LIKE '%" & txtSelectCriteria.Text.Trim & "%' OR [ContractDescription] LIKE '%" & txtSelectCriteria2.Text.Trim & "%') ")
                        Else
                            sql.Append("[ContractDescription] LIKE '%" & txtSelectCriteria.Text.Trim & "%' ")
                        End If
                        db.DBOpen(fws, False)

                        db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

                        Dim isFirst As Boolean = True
                        Dim tmpId, tmpErrorOutput As String
                        Dim linkId, linkCount As Integer

                        linkCount = 0
                        tmpErrorOutput = ""

                        ' first pass to ensure that unable to create link for just one contract - otherwise it will get lost!
                        For Each drow In db.glDBWorkA.Tables(0).Rows
                            If Request.Form("CON" & Trim(Str(drow.Item("ContractId")))) = "on" Then
                                linkCount += 1
                            End If
                        Next

                        If linkCount > 1 Then
                            linkCount = 0

                            For Each drow In db.glDBWorkA.Tables(0).Rows
                                ' see if the checkbox is ticked and create the schedule links assuming not assigned elsewhere
                                tmpId = "CON" & Trim(Str(drow.Item("ContractId")))
                                If Request.Form(tmpId) = "on" Then
                                    ' checkbox is ticked

                                    ' check isn't already assigned as a schedule link
                                    sql = New System.Text.StringBuilder
                                    sql.Append("SELECT [link_matrix].[LinkId] FROM [link_matrix] ")
                                    sql.Append("LEFT OUTER JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] ")
                                    sql.Append("WHERE [IsScheduleLink] = 1 AND [ContractId] = " & Trim(Str(drow.Item("ContractId"))))
                                    db.RunSQL(sql.ToString, db.glDBWorkB, False, "", False)

                                    If db.glNumRowsReturned > 0 Then
                                        ' skip this row as it is already assigned
                                        Dim tmpKey As String
                                        If IsDBNull(drow.Item("ContractKey")) = False Then
                                            tmpKey = drow.Item("ContractKey")
                                        Else
                                            If IsDBNull(drow.Item("ContractNumber")) = False Then
                                                tmpKey = drow.Item("ContractNumber")
                                            Else
                                                tmpKey = "Blank Contract Number"
                                            End If
                                        End If

                                        tmpErrorOutput += "<tr><td class=""row2"">Error creating link for " & tmpKey.Trim & ". Link already exists.</td></tr>"
                                    Else
                                        If isFirst = True Then
                                            ' need to create the initial link definition
                                            db.SetFieldValue("LinkDefinition", Left(drow.Item("ContractDescription"), 50), "S", True)
                                            db.SetFieldValue("IsScheduleLink", 1, "N", False)
                                            db.FWDb("W", "link_definitions", "", "", "", "", "", "", "", "", "", "", "", "")
                                            linkId = db.glIdentity
                                            isFirst = False
                                        End If

                                        db.SetFieldValue("ContractId", drow.Item("ContractId"), "N", True)
                                        db.SetFieldValue("LinkId", linkId, "N", False)
                                        db.SetFieldValue("IsArchived", IIf(drow.Item("Archived") = "Y", 1, 0), "N", False)
                                        db.FWDb("WX", "link_matrix", "", "", "", "", "", "", "", "", "", "", "", "")

                                        linkCount += 1
                                    End If
                                End If
                            Next

                            litScheduleHTML.Text = "<table class=""datatbl""><tr><td class=""row1"">" & linkCount.ToString & " Contract(s) added to the schedules chain.</td></tr>" & tmpErrorOutput & "</table>"
                            cmdOK.Visible = False
                        Else
                            litScheduleHTML.Text = "<table class=""datatbl""><tr><td class=""row1"">Can only create schedule links for 2 or more contracts.</td></tr></table>"
                        End If

                    Case Else

                End Select

            Catch ex As Exception
                litScheduleHTML.Text = "<table class=""datatbl""><tr><td class=""row1""><b>ERROR!&nbsp;</b>An error occurred attempting schedule link creation</td></tr></table>"
                cmdOK.Visible = False
            End Try

            db = Nothing
        End Sub

        Private Sub doSearch()
            Dim tmpStr As String
            Dim sql As System.Text.StringBuilder
            Dim strHTML As New System.Text.StringBuilder
            Dim db As New cFWDBConnection
            Dim drow As DataRow
            Dim hasData As Boolean
            Dim rowClass As String
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim action As String
            Dim supplierStr As String = params.SupplierPrimaryTitle

            Try
                action = Request.QueryString("action")

                sql = New System.Text.StringBuilder
                sql.Append("SELECT [Archived],[ContractId],[ContractKey],[ContractDescription],[ContractNumber],[ScheduleNumber],[supplier_details].[suppliername] ")
                sql.Append("FROM [contract_details] ")
                sql.Append("INNER JOIN [supplier_details] ON [contract_details].[supplierId] = [supplier_details].[supplierid] ")
                sql.Append("WHERE [contract_details].[subAccountId] = " & curUser.CurrentSubAccountId.ToString & " AND ")
                If txtSelectCriteria2.Text.Trim <> "" Then
                    sql.Append("([ContractDescription] LIKE '%" & txtSelectCriteria.Text.Trim & "%' OR [ContractDescription] LIKE '%" & txtSelectCriteria2.Text.Trim & "%') ")
                Else
                    sql.Append("[ContractDescription] LIKE '%" & txtSelectCriteria.Text.Trim & "%' ")
                End If
                sql.Append("AND [ContractId] NOT IN (SELECT [ContractId] FROM [link_matrix] INNER JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] WHERE [link_definitions].[IsScheduleLink] = 1) ")
                db.DBOpen(fws, False)

                db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)
                strHTML.Append("<table class=""datatbl"">" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<th>Add<br>as<br>Schedule</th>" & vbNewLine)
                strHTML.Append("<th>Contract Key</th>" & vbNewLine)
                strHTML.Append("<th>Contract Number</th>" & vbNewLine)
                strHTML.Append("<th>Schedule</th>" & vbNewLine)
                strHTML.Append("<th>Contract Description</th>" & vbNewLine)
                strHTML.Append("<th>" & supplierStr & "</th>" & vbNewLine)
                strHTML.Append("<th>Archived?</th>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)

                hasData = False
                Dim rowalt As Boolean = False

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    rowalt = (rowalt Xor True)
                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=""checkbox"" name=""CON" & Trim(Str(drow.Item("ContractId"))) & """ /></td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("ContractKey") & "</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("ContractNumber") & "</td>" & vbNewLine)
                    If IsDBNull(drow.Item("ScheduleNumber")) = True Then
                        tmpStr = "&nbsp;"
                    Else
                        tmpStr = Trim(drow.Item("ScheduleNumber"))
                    End If
                    strHTML.Append("<td class=""" & rowClass & """>")
                    strHTML.Append(tmpStr)
                    strHTML.Append("</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """><a href=""ContractSummary.aspx?id=" & Trim(Str(drow.Item("ContractId"))) & """ onmouseover=""window.status='Navigate to contract';return true;"" onmouseout=""window.status='Done';"">" & drow.Item("ContractDescription") & "</a></td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("suppliername") & "</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=""checkbox"" disabled ")
                    If drow.Item("Archived") = "Y" Then
                        strHTML.Append("checked ")
                    End If
                    strHTML.Append("/></td>" & vbNewLine)
                    strHTML.Append("</tr>" & vbNewLine)
                    hasData = True
                Next

                If hasData = False Then
                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<td class=""row1"" colspan=""7"" align=""center"">No Contracts returned matching the description.</td>" & vbNewLine)
                    strHTML.Append("</tr>" & vbNewLine)
                Else
                    cmdOK.Visible = True
                    cmdCancel.ImageUrl = "~/buttons/cancel.gif"
                End If
                strHTML.Append("</table>")

                litScheduleHTML.Text = strHTML.ToString

                db.DBClose()

            Catch ex As Exception
                litScheduleHTML.Text = "<table width=""500"" class=""datatbl""><tr><td class=""row1""><b>ERROR!&nbsp;</b>An error occurred while attempting to retrieve the search results.</td></tr></table>"
            End Try

            db = Nothing
        End Sub

        Private Sub PerformMerge()
            Dim arrConList() As String
            Dim primaryID, selectionCount As Integer
            Dim x As Integer
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            Try
                arrConList = txtSelectCriteria.Text.Split(",")
                selectionCount = 0

                If arrConList.Length > 1 Then
                    ' firstly, must check that primary is a selected contract
                    primaryID = Val(Request.Form("Primary"))
                    If Request.Form("chk" & Trim(Str(primaryID))) <> "on" Then
                        ' do not permit to continue. Primary contract must be a selected contract
                        lblErrorMsg.Text = "** ERROR : Primary Contract must be one of those selected for merging **"
                        Exit Sub
                    End If

                    ' secondly, make sure that there is >1 contract selected for merge
                    txtSelectCriteria2.Text = ""
                    For x = 0 To arrConList.Length - 1
                        If Request.Form("chk" & Trim(arrConList(x))) = "on" Then
                            ' add checked contract to the list
                            If Trim(txtSelectCriteria2.Text) <> "" Then
                                txtSelectCriteria2.Text += ","
                            End If
                            txtSelectCriteria2.Text += Trim(arrConList(x))
                            selectionCount += 1
                        End If
                    Next

                    If selectionCount > 1 Then
                        ' core merge functionality!!
                        Dim newConID As Integer
                        Dim db As New cFWDBConnection

                        db.DBOpen(fws, False)

                        arrConList = txtSelectCriteria2.Text.Split(",")

                        ' all contracts selected for merging MUST be checked to see if all products are available
                        ' from the Primary contract's vendor. i.e. Dorana & CIM are available from SEL but if
                        ' Primary contract is Ubiquity, then merge should be denied.
                        Dim PrimaryVendorId As Integer
                        db.FWDb("R", "contract_details", "ContractId", primaryID, "", "", "", "", "", "", "", "", "", "")
                        If db.FWDbFlag = True Then
                            PrimaryVendorId = Val(db.FWDbFindVal("supplierId", 1))
                        End If

                        Dim drow_products, drow_VendorProducts As DataRow
                        Dim primaryProd_sql As New System.Text.StringBuilder
                        Dim sql As New System.Text.StringBuilder
                        Dim tmpContractId As Integer
                        Dim prodFound As Boolean

                        sql.Append("SELECT [contract_details].[ContractId],[contract_details].[ContractKey],[ProductId] FROM [contract_productdetails] ")
                        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId] ")
                        sql.Append("WHERE [contract_details].[ContractId] IN (" & txtSelectCriteria2.Text.Trim & ") ")
                        sql.Append("ORDER BY [contract_details].[ContractId]")
                        db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

                        primaryProd_sql.Append("SELECT [supplierId],[ProductId] FROM [product_suppliers] WHERE [supplierId] = @psId")
                        db.AddDBParam("psId", PrimaryVendorId, True)
                        db.RunSQL(primaryProd_sql.ToString, db.glDBWorkB, False, "", False)

                        For Each drow_products In db.glDBWorkA.Tables(0).Rows
                            tmpContractId = drow_products.Item("ContractId")
                            prodFound = False

                            If drow_products.Item("ContractId") <> primaryID Then
                                For Each drow_VendorProducts In db.glDBWorkB.Tables(0).Rows
                                    If drow_products.Item("ProductId") = drow_VendorProducts.Item("ProductId") Then
                                        ' primary vendor is associated with product
                                        prodFound = True
                                        Exit For
                                    End If
                                Next

                                If prodFound = False Then
                                    ' error - can't merge contract as vendor not associated with a product
                                    Dim tmpKey As String
                                    If IsDBNull(drow_products.Item("ContractKey")) = True Then
                                        tmpKey = ""
                                    Else
                                        tmpKey = Trim(drow_products.Item("ContractKey")) & " "
                                    End If

                                    Dim suppStr As String = params.SupplierPrimaryTitle

                                    lblErrorMsg.Text = "** ERROR - Contract " & tmpKey & " has a Product not associated with Primary " & suppStr & " **"
                                    db.DBClose()
                                    db = Nothing
                                    Exit Sub
                                End If
                            End If
                        Next

                        ' create the new contract from primary selected to merge contracts under.
                        newConID = AddSchedule(Server, primaryID, curUser)
                        For x = 0 To arrConList.Length - 1
                            If Val(arrConList(x)) <> primaryID Then
                                ' assign duplicates to the new contract id
                                DuplicateProducts(curUser, Val(arrConList(x)), newConID, PrimaryVendorId)
                                DuplicateAttachments(Server, curUser, AttachmentArea.CONTRACT, Val(arrConList(x)), newConID)
                                DuplicateNotes(Server, curUser, AttachmentArea.CONTACT_NOTES, arrConList(x), newConID)
                            End If
                        Next

                        db.DBClose()
                        db = Nothing

                        ' must have completed successfully to get here, so display new contract
                        Response.Redirect("ContractSummary.aspx?id=" & Trim(Str(newConID)), True)
                    Else
                        ' do not permit to continue. must have > 1 contract to merge
                        lblErrorMsg.Text = "** ERROR : Must be more than one contract selected for merging **"
                        Exit Sub
                    End If

                End If

            Catch ex As Exception
                Dim msg As New System.Text.StringBuilder

                msg.Append("<table class=""datatbl"" width=""750"">" & vbNewLine)
                msg.Append("<tr>" & vbNewLine)
                msg.Append("<th>An error occurred trying to merge contracts</th>" & vbNewLine)
                msg.Append("</tr>" & vbNewLine)
                msg.Append("<tr>" & vbNewLine)
                msg.Append("<td class=""row1"">" & ex.Message & "</td>" & vbNewLine)
                msg.Append("</tr>" & vbNewLine)
                msg.Append("</table>")

                litScheduleHTML.Text = msg.ToString
            End Try
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Response.Redirect("ScheduleAction.aspx?contractId=" & ViewState("ContractId"), True)
        End Sub

        Protected Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdSearch.Click
            doSearch()
        End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            PerformAction()
        End Sub
    End Class
End Namespace
