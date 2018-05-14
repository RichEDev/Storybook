Imports System.Collections
Imports System.Collections.Generic
Imports FWClasses
Imports SEL.FeatureFlags
Imports SpendManagementLibrary
Imports SpendManagementLibrary.HelpAndSupport
Imports SpendManagementLibrary.Helpers
Imports Spend_Management
Imports Spend_Management.shared.code.GreenLight

Namespace Framework2006
    Partial Class Home
        Inherits System.Web.UI.Page

        Private myTheme As String
        Private NewStyleReports As Boolean

        Public Property FeatureFlagManager As IFeatureFlagManager

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Me.IsPostBack = False Then
                Dim db As New cFWDBConnection
                If FeatureFlagManager Is Nothing Then
                    NewStyleReports = True
                Else
                    NewStyleReports = FeatureFlagManager.IsEnabled("Syncfusion report viewer")
                End If

                Dim curUser As CurrentUser = cMisc.GetCurrentUser()
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim clsModules As New cModules
                Dim clsModule As cModule = clsModules.GetModuleByID(curUser.CurrentActiveModule)

                Title = "Welcome to " & clsModule.BrandNamePlainText
                Master.title = "Welcome to " & clsModule.BrandNameHTML

                Master.iconSize = fwIconSize.Large

                db.DBOpen(fws, False)

                If Request.QueryString("newloc") <> "" Then
                    ' user must be switching active locations from the menu
                    'uinfo.ActiveLocation = Integer.Parse(Request.QueryString("newloc"))
                    'uinfo.ActiveLocationDesc = Request.QueryString("locdesc")
                    '               uinfo = SecurityRoutines.GetRolePermissions(uinfo, uinfo.ActiveLocation, fws)
                    '               uinfo = SecurityRoutines.GetCountInfo(uinfo, fws)

                End If

                Dim Err As Integer
                Err = Val(Request.QueryString("error"))
                Select Case Err
                    Case 1
                        lblErrorStatus.Text = "** Redirected due no active contract at the current time. **"

                    Case 2
                        lblErrorStatus.Text = "** Access denied to requested repository location. **"

                    Case 3
                        lblErrorStatus.Text = "** Contract record could not be found. **"

                    Case 4
                        lblErrorStatus.Text = "** User not permitted access to the requested contract. **"
                    Case Else

                End Select

                Select Case Request.QueryString("action")
                    Case "callback"
                        Select Case Request.QueryString("type")
                            Case "ck"
                                Dim tmpKey As String
                                Dim res As New System.Text.StringBuilder
                                tmpKey = Request.Form("searchVal")
                                res.Append(GotoCK(curUser, tmpKey))
                                db.DBClose()
                                db = Nothing
                                If res.ToString <> "ERROR" Then
                                    Response.Write(createBroadcastMessage(res, "CK Found"))
                                    Response.Flush()
                                    Response.End()
                                Else
                                    Dim strMsg As New System.Text.StringBuilder
                                    strMsg.Append("<div class=""inputpanel"">" & vbNewLine)
                                    strMsg.Append("<div class=""inputpaneltitle"">Search Error</div>" & vbNewLine)
                                    strMsg.Append("<br /><div>Unable to locate a contract with the Contract Key <b>" & tmpKey & "</b></div>" & vbNewLine)
                                    strMsg.Append("</div>" & vbNewLine)
                                    Response.Write(createBroadcastMessage(strMsg, "Search Results", True))
                                    Response.Flush()
                                    Response.End()
                                    Exit Sub
                                End If
                            Case Else
                                Response.Write(createBroadcastMessage(doSearch(Request.QueryString("type"), Request.Form("searchVal"), Request.Form("status"), Request.Form("iv")), "Search Results", True))
                                Response.Flush()
                                Response.End()
                                Exit Sub
                        End Select
                    Case Else
                End Select

                CreateMenu()

                Master.addStyle = SetStyle(curUser.Employee)

                Dim clsEmployees As New cEmployees(curUser.AccountID)
                Dim lstAccessRoles As List(Of Integer) = curUser.Employee.GetAccessRoles().GetBy(curUser.CurrentSubAccountId)

                If lstAccessRoles.Count > 0 Then

                    AccountInfoPanel.Visible = False
                    If curUser.CurrentActiveModule <> Modules.SmartDiligence Then
                        SetSearchPanel(curUser)
                    Else
                        SearchFieldsPanel.Visible = False
                        Master.showsinglecolumnmenu = False
                    End If
                Else
                    SearchFieldsPanel.Visible = False
                    ' Comment to force the merge
                    If curUser.CurrentActiveModule = Modules.SmartDiligence Then
                        AccountInfoPanel.Visible = False
                    Else
                        SetAccountInfoPanel(curUser)
                    End If
                End If

                SetHeadings(db, curUser)

                SetLinks(db, curUser)
                db.DBClose()
                db = Nothing
            End If
        End Sub

        Private Sub CreateMenu()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties

            Dim clsModules As cModules = New cModules()
            Dim curModule As cModule = clsModules.GetModuleByID(curUser.CurrentActiveModule)

            Dim clsEmployees As New cEmployees(curUser.AccountID)
            Dim lstAccessRoles As List(Of Integer) = curUser.Employee.GetAccessRoles.GetBy(curUser.CurrentSubAccountId)

            ' - Can't see any reason why we need this if statement
            'If curUser.CurrentSubAccountId >= 0 Then
            params = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            'Else
            'params = subaccs.getFirstSubAccount().SubAccountProperties
            'End If

            Dim supplierStr As String = params.SupplierPrimaryTitle

            'If curUser.Employee.username.ToLower <> "admin" Then
            ' create main menu
            If curUser.CurrentActiveModule <> Modules.SmartDiligence Then
                Master.showsinglecolumnmenu = True
            End If

            'If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, False) Then
            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AddNewContract, False) Then
                Master.addMenuItem("form_green", Master.iconSize, "Add New Contract", "Create a new contract entry", "ContractSummary.aspx?id=0&tab=" & SummaryTabs.ContractDetail & "&action=add")
            End If
            'End If
            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierDetails, True) Then
                Master.addMenuItem("handshake", Master.iconSize, supplierStr & " Details", "Search for, add and maintain " & supplierStr.ToLower & " and their associated contact information", "shared/suppliers.aspx")
            End If

            If lstAccessRoles.Count > 0 And curUser.CurrentActiveModule <> Modules.SmartDiligence Then
                Master.addMenuItem("find", Master.iconSize, "Advanced Search", "Use the Advanced Search Portal to locate contracts and " & supplierStr.ToLower & " information", "SummaryPortal.aspx")
            End If

            If params.EnableInternalSupportTickets And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupportTickets, True) Then
                Master.addMenuItem(
                    "lifebelt",
                    48,
                    String.Format("Support Tickets ({0})", SupportTicketInternal.GetCountForAdministrator(curUser)),
                    "Respond to support tickets raised by other users.",
                    "shared/admin/adminHelpAndSupportTickets.aspx")
            End If

            If checkForAdmin(curUser, params) Then
                Master.addMenuItem("businessman2", Master.iconSize, "Administrative Settings", "Access and configure the system set-up", "MenuMain.aspx?menusection=admin")
            End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LogonMessages, True) Then
                Master.addMenuItem("flatscreen_tv", 48, "Marketing Information", "Manage background images, icons and text for the marketing panel on the logon page.", "shared/admin/LogonMessages.aspx")
            End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, True) Then
                Master.addMenuItem("chart_area", 48, "Reports", "Create new reports, edit, delete or view existing ones. Export data to Excel, CSV, flat file or create pivot tables.", "shared/reports/rptlist.aspx")
            End If
            Master.addMenuItem("user_information", Master.iconSize, "My Details", "View information specific to you as a user. This option can also be used to update your profile and change your password", "MenuMain.aspx?menusection=mydetails")

            If lstAccessRoles.Count > 0 Then
                Master.addMenuItem("help2", 48, "Help &amp; Support", "Help &amp; Support is an online service for education, guidance and support that enables you to find the best answers for your " & curModule.BrandNameHTML & " questions.", "shared/helpAndSupport.aspx")
            End If

            Master.addMenuItem("trafficlight_green", 48, "GreenLight", "GreenLight means you can now take any paper based approval and put it online, click here to find out more.", GlobalVariables.GreenLightInfoPage, "_blank")

            GetCustomEntityOptions(curUser)

            Dim exitURL As String = "shared/logon.aspx?action=logout"
            Select Case Me.Page.Theme
                Case "SmartDTheme"
                    exitURL += "&sd=1"
                Case Else
                    exitURL += "&sd=0"
            End Select

            Master.addMenuItem("exit", Master.iconSize, "Log Out", "Log out of " & curModule.BrandNameHTML & " and end current session", "shared/process.aspx?process=1")
        End Sub

        Private Sub SetAccountInfoPanel(ByVal curuser As CurrentUser)
            Dim subaccs As New cAccountSubAccounts(curuser.Account.accountid)

            Dim litStr As New System.Text.StringBuilder
            litStr.Append("You are currently logged in on an account that doesn't have the ability to view or use contract information on this sub account.<br /><br />You may still view any tasks you have been assigned on the system, or view your name and contact details to confirm they are correct, by going into the <i>My Details</i> section.")

            Dim tmpLstSA As ListItem() = subaccs.CreateFilteredDropDown(curuser.Employee, curuser.CurrentSubAccountId)
            If tmpLstSA.Length > 0 Then
                If tmpLstSA.Length > 1 Or (tmpLstSA.Length = 1 And tmpLstSA(0).Value <> curuser.CurrentSubAccountId.ToString) Then
                    litStr.Append("</div><div class=""comment"" style=""width: 94%; margin-top: 5px;"">You have access to view more information on a different sub account, this can be accessed by changing the sub account that you are viewing via the <i>Switch Sub Account</i> link on the menu bar.<br /><br />Additionally, if when you first log in you should start in one of the other sub accounts that you have access to via the link above, you should ask your system administrator to change your <i>Default Sub Account</i> via the form linked in the next paragraph.")
                End If
            End If


            litStr.Append("<br /><br />If you believe you should have access to view information on this sub account of the system please request to have an <i>Access Role</i> added to your account for the <i>Active Sub Account</i> name listed on the title bar above via <a href=""shared/information/changeofdetails.aspx"" alt=""Change Incorrect Information"">this form</a>.")
            litAccountInfo.Text = litStr.ToString
        End Sub

        Private Sub SetSearchPanel(ByVal curuser As CurrentUser)
            Dim subaccs As New cAccountSubAccounts(curuser.Account.accountid)
            Dim params As cAccountProperties
            If curuser.CurrentSubAccountId >= 0 Then
                params = subaccs.getSubAccountById(curuser.CurrentSubAccountId).SubAccountProperties
            Else
                params = subaccs.getFirstSubAccount().SubAccountProperties
            End If

            Dim litStr As New System.Text.StringBuilder
            Dim SearchButton As String
            Dim supplierStr As String = params.SupplierPrimaryTitle

            SearchButton = "<img style=""cursor: hand;"" src=""./icons/16/plain/find.png"" />"

            Dim permitted As Boolean = curuser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, False)

            litStr.Append("<input type=""text"" id=""CKSearch"" value=""" & params.ContractKey & "/?/?"" alt=""Jump directly to a contract matching the specified unique key"" " & IIf(permitted, "", "disabled ") & " />")
            If permitted Then
                litStr.Append("&nbsp;<a onclick=""javascript:doCKSearch();"">")
                litStr.Append(SearchButton)
                litStr.Append("</a>")
            End If

            litCKSearch.Text = litStr.ToString

            litStr.Remove(0, litStr.Length)

            litStr.Append("<input type=""text"" id=""CDSearch"" alt=""Search for contracts containing the search text within it's description"" " & IIf(permitted, "", "disabled ") & " />")
            If permitted Then
                litStr.Append("&nbsp;<a onclick=""javascript:doCDSearch();"">")
                litStr.Append(SearchButton)
                litStr.Append("</a>")
            End If

            litCDSearch.Text = litStr.ToString
            litStr.Remove(0, litStr.Length)

            litStr.Append("<input type=""text"" id=""CNSearch"" alt=""Search for contracts containing the search text it's contract number"" " & IIf(permitted, "", "disabled ") & " />")
            If permitted Then
                litStr.Append("&nbsp;<a onclick=""javascript:doCNSearch();"">")
                litStr.Append(SearchButton)
                litStr.Append("</a>")
            End If
            litCNSearch.Text = litStr.ToString
            litStr.Remove(0, litStr.Length)

            permitted = curuser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, False)

            litStr.Append("<input type=""text"" id=""PNSearch"" alt=""Search for contracts associated with the search product"" " & IIf(permitted, "", "disabled ") & " />")
            If permitted Then
                litStr.Append("&nbsp;<a onclick=""javascript:doPNSearch();"">")
                litStr.Append(SearchButton)
                litStr.Append("</a>")
            End If
            litPNSearch.Text = litStr.ToString
            litStr.Remove(0, litStr.Length)

            permitted = curuser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierDetails, False)

            litStr.Append("<input type=""text"" id=""VNSearch"" alt=""View or amend the details of a " & supplierStr.ToLower & " containing search text in it's name"" " & IIf(permitted, "", "disabled ") & " />")
            If permitted Then
                litStr.Append("&nbsp;<a onclick=""javascript:doVNSearch();"">")
                litStr.Append(SearchButton)
                litStr.Append("</a>")
            End If
            litVNSearch.Text = litStr.ToString
            litStr.Remove(0, litStr.Length)

            litStr.Append("<input type=""radio"" style=""width: 13px;"" name=""rdoStatus"" value=""N"" checked />&nbsp;Live")
            litStr.Append("&nbsp;&nbsp;")
            litStr.Append("<input type=""radio"" style=""width: 13px;"" name=""rdoStatus"" value=""Y"" />&nbsp;Archived")
            litStr.Append("&nbsp;&nbsp;")
            litStr.Append("<input type=""radio"" style=""width: 13px;"" name=""rdoStatus"" value=""B"" />&nbsp;All")
            litStatus.Text = litStr.ToString

            litStr.Remove(0, litStr.Length)
            litStr.Append("<input type=""checkbox"" id=""includevariations"" value=""1"" />")
            litChkIncludeVariations.Text = litStr.ToString
        End Sub

        Private Sub SetLinks(ByVal db As cFWDBConnection, ByVal curUser As CurrentUser)
            Dim sql As New System.Text.StringBuilder

            sql.Append("SELECT ISNULL([lastReport],'') AS [lastReport],[lastContractId],[lastReportId],[reportsview].[reportname],[contract_details].[ContractDescription],lastReportType FROM [subAccountAccess] " & vbNewLine)
            sql.Append("LEFT JOIN [contract_details] ON [contract_details].[ContractId] = subAccountAccess.[lastContractId]" & vbNewLine)
            sql.Append("LEFT JOIN [reportsview] ON [reportsview].[reportid] = subAccountAccess.lastReportId " & vbNewLine)
            sql.Append("WHERE subAccountAccess.employeeId = @empId AND subAccountAccess.subAccountId = @subaccId")
            db.AddDBParam("empId", curUser.EmployeeID, True)
            db.AddDBParam("subaccId", curUser.CurrentSubAccountId, False)
            db.RunSQL(sql.ToString, db.glDBWorkB, False, "", False)

            If db.glNumRowsReturned > 0 Then
                Dim tmpstr As String = ""

                If IsDBNull(db.GetFieldValue(db.glDBWorkB, "lastReportId", 0, 0)) Then
                    hypLastReport.Text = "n/a"
                    hypLastReport.Enabled = False
                Else
                    Dim tmprepname As String = ""
                    If IsDBNull(db.GetFieldValue(db.glDBWorkB, "reportname", 0, 0)) Then
                        tmprepname = "n/a"
                    Else
                        tmprepname = db.GetFieldValue(db.glDBWorkB, "reportname", 0, 0)
                    End If
                    hypLastReport.Text = tmprepname
                    hypLastReport.Attributes.Add("onmouseout", "window.status='Done';")

                    hypLastReport.Attributes.Add("onmouseover", "window.status='Run Report """ & tmprepname.Replace("'", "`") & """';return true;")
                    If NewStyleReports Then
                        hypLastReport.NavigateUrl = "javascript:window.location.href=document.location;window.open('" & cMisc.Path & "/shared/reports/view.aspx?reportid=" & db.GetFieldValue(db.glDBWorkB, "lastReportId", 0, 0).ToString & "','_blank');"
                    Else
                        hypLastReport.NavigateUrl = "javascript:window.location.href=document.location;window.open('" & cMisc.Path & "/shared/reports/reportviewer.aspx?reportid=" & db.GetFieldValue(db.glDBWorkB, "lastReportId", 0, 0).ToString & "','reportviewer','locationbar=no,menubar=no,scrollbars=yes,status=1,resizable=1');"
                    End If

                End If

                If IsDBNull(db.GetFieldValue(db.glDBWorkB, "ContractDescription", 0, 0)) Then
                    tmpstr = "n/a"
                Else
                    If db.GetFieldValue(db.glDBWorkB, "ContractDescription", 0, 0) = "" Then
                        tmpstr = "n/a"
                    Else
                        tmpstr = db.GetFieldValue(db.glDBWorkB, "ContractDescription", 0, 0)
                        If tmpstr.Length > 40 Then
                            tmpstr = tmpstr.Substring(0, 37)
                            tmpstr += "..."
                        End If
                    End If

                End If

                hypLastContract.Text = IIf(tmpstr = "", "n/a", tmpstr)

                'If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, False) Then
                If IsDBNull(db.GetFieldValue(db.glDBWorkB, "lastContractId", 0, 0)) Then
                    tmpstr = ""
                Else
                    If db.GetFieldValue(db.glDBWorkB, "lastContractId", 0, 0) = 0 Then
                        tmpstr = ""
                    Else
                        tmpstr = "ContractSummary.aspx?id=" & Trim(db.GetFieldValue(db.glDBWorkB, "lastContractId", 0, 0)) & "&tab=" & SummaryTabs.ContractDetail
                    End If
                End If

                hypLastContract.NavigateUrl = tmpstr
                hypLastContract.Attributes.Add("onmouseout", "window.status='Done';")
                hypLastContract.Attributes.Add("onmouseover", "window.status='Open the last Contract you viewed';return true;")

                ' ensure that tab always defaults back to ContractDetails

                If tmpstr = "" Then
                    hypLastContract.Enabled = False
                End If
                'Else
                '    hypLastContract.Enabled = False
                'End If
            Else
                hypLastContract.Text = "n/a"
                hypLastContract.Enabled = False
                hypLastReport.Text = "n/a"
                hypLastReport.Enabled = False
            End If

            Dim tasks As New cTasks(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim activeTasks As Integer = tasks.GetTasksByStatus(TaskStatus.InProgress, curUser.Employee.employeeid).Count
            Dim waitingTasks As Integer = tasks.GetTasksByStatus(TaskStatus.NotStarted, curUser.Employee.employeeid).Count
            hypTasks.Text = waitingTasks.ToString & " Not Started; " & activeTasks.ToString & " In Progress"
            hypTasks.NavigateUrl = "~/shared/tasks/MyTasks.aspx"
            Session("TaskRetURL") = ("~/Home.aspx").Base64Encode()
        End Sub

        Private Function doSearch(ByVal searchType As String, ByVal searchVal As String, ByVal searchStatus As String, ByVal includeVariations As String) As System.Text.StringBuilder
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim retStr As New System.Text.StringBuilder

            Select Case searchType.ToLower
                Case "cd"
                    retStr.Append(DescriptionSearch(curUser, searchVal, searchStatus, includeVariations))
                Case "pn"
                    retStr.Append(ProductSearch(curUser, searchVal, searchStatus, includeVariations))
                Case "vn"
                    retStr.Append(VendorDefSearch(curUser, searchVal, searchStatus))
                Case "cn"
                    retStr.Append(ContractNoSearch(curUser, searchVal, searchStatus, includeVariations))
                Case "cc"
                    retStr.Append(ContractCategorySearch(curUser, searchVal, searchStatus, includeVariations))
                Case Else
                    retStr.Append("")
            End Select

            Return retStr
        End Function

        Private Sub SetHeadings(ByVal db As cFWDBConnection, ByVal curUser As CurrentUser)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties
            If curUser.CurrentSubAccountId >= 0 Then
                params = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Else
                params = subaccs.getFirstSubAccount().SubAccountProperties
            End If
            Dim sql As New System.Text.StringBuilder

            If CInt(Session("XRes")) < 1024 Then
                lblContractDesc.Text = params.ContractDescShortTitle
            Else
                lblContractDesc.Text = params.ContractDescTitle
            End If

            lblSupplierName.Text = params.SupplierPrimaryTitle

            'sql.Append("SELECT [Category Id],[Category Description] FROM [codes_contractcategory] ")
            'sql.Append("WHERE [Location Id] = @locId ORDER BY [Category Description]")
            'db.AddDBParam("locId", uinfo.ActiveLocation, True)
            'db.RunSQL(sql.ToString, db.glDBWorkA)

            'sql.Remove(0, sql.Length)

            Dim clsBaseDefs As New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractCategories)
            'Dim cats As New cContractCategories(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim catlist As ListItem() = clsBaseDefs.CreateDropDown(True, 0)

            Dim litStr As New System.Text.StringBuilder
            litStr.Append("<select id=""CCSearch"" onchange=""doCCSearch();"" ")
            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, False) = False Then
                litStr.Append("disabled ")
            End If
            litStr.Append(">" & vbNewLine)
            litStr.Append("<option value=""0"">Select...</option>" & vbNewLine)

            For Each item As ListItem In catlist
                litStr.Append("<option value=""" & item.Value & """>" & item.Text & "</option>" & vbNewLine)
            Next

            litStr.Append("</select>" & vbNewLine)
            litCCSearch.Text = litStr.ToString

            lblCCSearch.Text = params.ContractCategoryTitle
        End Sub

        Private Sub GetCustomEntityOptions(ByVal curuser As CurrentUser)
            Dim centities As New cCustomEntities(curuser)
            Dim menu_list As List(Of cCustomEntityView) = centities.getViewsByMenuId(1)

            If menu_list.Count > 0 Then
                Dim disabledModuleMenuViews As New DisabledModuleMenuViews(curuser.AccountID, curuser.CurrentActiveModule)
                For Each ce As cCustomEntityView In menu_list
                    Dim centity As cCustomEntity = centities.getEntityById(ce.entityid)
                    If Not disabledModuleMenuViews.IsViewDisabled(1, ce.viewid) Then
                        If curuser.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, ce.entityid, ce.viewid, False) Then
                            Dim entityURL As String

                            entityURL = cMisc.Path + "/shared/viewentities.aspx?entityid=" & centity.entityid.ToString & "&viewid=" & ce.viewid.ToString
                            Master.addMenuItem(ce.MenuIcon, fwIconSize.Large, ce.viewname, ce.MenuDescription, True, entityURL)
                        End If
                    End If
                Next
            End If
        End Sub

        Private Function checkForAdmin(ByVal cu As CurrentUser, ByVal params As cAccountProperties) As Boolean
            If cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Colours, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractCategories, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractStatus, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractTypes, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomEntities, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentConfigurations, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InflatorMetrics, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, True) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Locations, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductCategories, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SalesTax, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Sites, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierCategory, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierStatus, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TaskTypes, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TermTypes, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Units, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicenceTypes, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LicenceRenewalTypes, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Locations, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.IPAdressFiltering, False) Or _
            (
                cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, False) AndAlso
                cu.Account.HasLicensedElement(SpendManagementElement.SingleSignOn) _
            ) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSupplierReassignment, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProductReassignment, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, False) Or _
            cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, False) Or _
            (params.EnableRecharge And (cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeClients, False) Or cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAccountCodes, False))) Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class
End Namespace
