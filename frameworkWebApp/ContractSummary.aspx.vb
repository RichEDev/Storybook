Imports FWBase
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management
Imports System.Collections.Generic
Imports System.Web.Services
Imports SpendManagementLibrary.Employees

Namespace Framework2006
    Partial Class ContractSummary
        Inherits System.Web.UI.Page
        Private ConProd_Count As Integer = 0
        Private Enum PeriodType
            Months = 0
            Days = 1
        End Enum

#Region "Navigation panel calls"
        Protected Sub lnkCDnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            SetViewTab(SummaryTabs.ContractDetail)
        End Sub

        Protected Sub lnkCAnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            SetViewTab(SummaryTabs.ContractAdditional)
        End Sub

        Protected Sub lnkCPnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            SetViewTab(SummaryTabs.ContractProduct)
        End Sub

        Protected Sub lnkIDnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            SetViewTab(SummaryTabs.InvoiceDetail)
        End Sub

        Protected Sub lnkIFnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            SetViewTab(SummaryTabs.InvoiceForecast)
        End Sub

        Protected Sub lnkNSnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            SetViewTab(SummaryTabs.NotesSummary)
        End Sub

        Protected Sub lnkLCnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            SetViewTab(SummaryTabs.LinkedContracts)
        End Sub

        Protected Sub lnkCHnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Response.Redirect("ContractHistory.aspx?lc=" & IIf(lnkLCnav.Visible, "1", "0"), True)
        End Sub

        Protected Sub lnkRTnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            'SetViewTab(SummaryTabs.RechargeTemplate)
            Response.Redirect("ContractRechargeBreakdown.aspx?id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub lnkRPnav_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Response.Redirect("ContractRechargeData.aspx?id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub lnkTSnav_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkTSnav.Click
            Dim varURL As String = ""

            Select Case CType(ViewTab.ActiveViewIndex, SummaryTabs)
                Case SummaryTabs.ContractDetail, SummaryTabs.ContractAdditional
                    varURL = "?pid=" & Session("ActiveContract") & "&paa=" & AppAreas.CONTRACT_DETAILS
                    varURL += "&ret=" & Server.UrlEncode("~/ContractSummary.aspx?tab=" & ViewTab.ActiveViewIndex & "&id=" & Session("ActiveContract"))
                    Session("TaskRetURL") = Server.UrlEncode("~/ContractSummary.aspx?tab=" & ViewTab.ActiveViewIndex & "&id=" & Session("ActiveContract"))

                Case SummaryTabs.ContractProduct
                    varURL = "?pid=" & Session("ActiveContract") & "&paa=" & AppAreas.CONTRACT_PRODUCTS
                    varURL += "&ret=" & Server.UrlEncode("~/ContractSummary.aspx?tab=" & ViewTab.ActiveViewIndex & "&id=" & Session("ActiveContract"))
                    Session("TaskRetURL") = Server.UrlEncode("~/ContractSummary.aspx?tab=" & ViewTab.ActiveViewIndex & "&id=" & Session("ActiveContract"))
                Case SummaryTabs.InvoiceDetail
                Case SummaryTabs.InvoiceForecast
                Case Else

            End Select

            If varURL <> "" Then
                Response.Redirect("~/shared/tasks/TaskSummary.aspx" & varURL, True)
            End If
        End Sub
#End Region

        Private Sub SetViewTab(ByVal tabId As SummaryTabs, Optional ByVal callSetScreen As Boolean = True)
            Dim doRefresh As Boolean = True

            Select Case tabId
                Case SummaryTabs.ContractDetail
                    ViewTab.ActiveViewIndex = ViewTab.Views.IndexOf(vwContractDetails)
                Case SummaryTabs.ContractAdditional
                    ViewTab.ActiveViewIndex = ViewTab.Views.IndexOf(vwContractAdditional)
                Case SummaryTabs.ContractProduct
                    ViewTab.ActiveViewIndex = ViewTab.Views.IndexOf(vwContractProducts)
                Case SummaryTabs.InvoiceDetail
                    ViewTab.ActiveViewIndex = ViewTab.Views.IndexOf(vwInvoiceDetails)
                Case SummaryTabs.InvoiceForecast
                    ViewTab.ActiveViewIndex = ViewTab.Views.IndexOf(vwInvoiceForecasts)
                Case SummaryTabs.NotesSummary
                    ViewTab.ActiveViewIndex = ViewTab.Views.IndexOf(vwNoteSummary)
                Case SummaryTabs.LinkedContracts
                    ViewTab.ActiveViewIndex = ViewTab.Views.IndexOf(vwLinkedContracts)
                Case Else
                    doRefresh = False
            End Select

            If doRefresh Then
                Session("CurTab") = tabId

                If callSetScreen Then
                    Dim curUser As CurrentUser = cMisc.GetCurrentUser()

                    RenderUFields()
                    Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                    SetScreen(tabId, curUser.Account.accountid, curUser.Employee.employeeid)
                End If
            End If
        End Sub

        Private Sub SetNavActiveState(ByVal bState As Boolean)
            lnkCDnav.Enabled = bState
            lnkCAnav.Enabled = bState
            lnkCPnav.Enabled = bState
            lnkIDnav.Enabled = bState
            lnkIFnav.Enabled = bState
            lnkNSnav.Enabled = bState
            lnkCHnav.Enabled = bState
            lnkLCnav.Enabled = bState
            lnkRTnav.Enabled = bState
            lnkRPnav.Enabled = bState
            lnkTSnav.Enabled = bState
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            RenderUFields()
        End Sub

        Private Sub RenderUFields()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim employees As New cEmployees(curUser.Account.accountid)
            Dim ufields As New cUserdefinedFields(curUser.Account.accountid)
            Dim tables As New cTables(curUser.Account.accountid)

            Dim jscript As New StringBuilder
            Dim contrCatID As Integer

            If IsPostBack = False Then
                Dim ActiveViewTab As SummaryTabs

                ActiveViewTab = CType(Request.QueryString("tab"), SummaryTabs)
                If ActiveViewTab <> Session("CurTab") Then
                    Session("CurTab") = ActiveViewTab
                End If

                If Not Request.QueryString("id") Is Nothing Then
                    Dim curContractId As Integer = Request.QueryString("id")
                    If Session("ActiveContract") <> curContractId Then
                        Session("ActiveContract") = curContractId
                    End If
                End If
            End If

            Select Case CType(Session("CurTab"), SummaryTabs)
                Case SummaryTabs.ContractDetail
                    Dim cdTable As cTable = tables.getTableByName("contract_details")
                    phCDUFields.Controls.Clear()
                    Dim udTable As cTable = tables.getTableById(cdTable.UserDefinedTableID)
                    ufields.createFieldPanel(phCDUFields, udTable, "cddetails", jscript, Nothing, groupType:=GroupingOutputType.UnGroupedOnly)

                Case SummaryTabs.ContractAdditional
                    'Dim db As New cFWDBConnection
                    'db.DBOpen(fws, False)
                    'GetContractAdditionalData(db, uinfo, fws, Session("HasCAFields"))
                    'db.DBClose()

                    'Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
                    'Dim contrID As Integer? = Session("ActiveContract")
                    'Dim sql As String = "select [categoryId] from contract_details where [contractId] = @conId"
                    'db.sqlexecute.Parameters.AddWithValue("@conId", contrID)

                    Integer.TryParse(lstContractCategory.SelectedValue, contrCatID) 'db.getIntSum(sql)

                    If lstContractCategory.SelectedValue = String.Empty Then
                        Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
                        Dim contrID As Integer? = Session("ActiveContract")
                        Dim sql As String = "select ISNULL([categoryId],0) from contract_details where [contractId] = @conId"
                        db.sqlexecute.Parameters.AddWithValue("@conId", contrID)
                        contrCatID = db.getcount(sql)
                    End If

                    phCAUFields.Controls.Clear()
                    Dim cdTable As cTable = tables.getTableByName("contract_details")
                    Dim udTable As cTable = tables.getTableById(cdTable.UserDefinedTableID)
                    If contrCatID = 0 Then
                        ufields.createFieldPanel(phCAUFields, udTable, "cadetails", jscript, Nothing, groupType:=GroupingOutputType.GroupedOnly)
                    Else
                        ufields.createFieldPanel(phCAUFields, udTable, "cadetails", jscript, contrCatID, groupType:=GroupingOutputType.GroupedOnly)
                    End If

                Case SummaryTabs.ContractProduct
                    ' get parent contract category for udf grouping filter
                    Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
                    Dim contrID As Integer? = Session("ActiveContract")
                    Dim sql As String = "select ISNULL([categoryId],0) from contract_details where [contractId] = @conId"
                    db.sqlexecute.Parameters.AddWithValue("@conId", contrID)
                    contrCatID = db.getcount(sql)

                    phCPUFields.Controls.Clear()
                    Dim cpTable As cTable = tables.getTableByName("contract_productdetails")
                    Dim udTable As cTable = tables.getTableById(cpTable.UserDefinedTableID)
                    If contrCatID = 0 Then
                        ufields.createFieldPanel(phCPUFields, udTable, "cpdetails", jscript, Nothing)
                    Else
                        ufields.createFieldPanel(phCPUFields, udTable, "cpdetails", jscript, contrCatID)
                    End If

                Case Else
            End Select

            If jscript.Length > 0 Then
                ScriptManager.RegisterStartupScript(Me, Page.GetType, "udfscript_" & Session("CurTab"), jscript.ToString, True)
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim ActiveViewTab As SummaryTabs

            Response.Expires = 0
            Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1)
            Response.AddHeader("pragma", "no-cache")
            Response.AddHeader("cache-control", "private")
            Response.CacheControl = "no-cache"

            If Me.IsPostBack = False Then
                ActiveViewTab = CType(Request.QueryString("tab"), SummaryTabs)
                If ActiveViewTab <> Session("CurTab") Then
                    Session("CurTab") = ActiveViewTab
                End If

                If Request.QueryString("afs") = 1 Then
                    ' check if contract accessed from supplier screen, if so return to supplier contract list for the current supplier
                    Session("ContractAFS") = 1
                Else
                    Session("ContractAFS") = 0
                End If

                Title = "Contract Summary"
                Master.title = Title
                'Master.enablenavigation = False

                SetViewTab(Session("CurTab"), False)

                ContractSummary_Load(Session("CurTab"))

                SetScreen(Session("CurTab"), curUser.Account.accountid, curUser.Employee.employeeid)

                Dim curContract As Integer
                If Not Session("ActiveContract") Is Nothing Then
                    curContract = Session("ActiveContract")
                Else
                    curContract = 0
                End If

                Dim onloadFunctionAppend As String = ""
                Dim allowDelete As Boolean = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractDetails, False)

                Select Case CType(Session("CurTab"), SummaryTabs)
                    Case SummaryTabs.ContractAdditional, SummaryTabs.ContractDetail
                        Dim sql As String
                        Dim db As New cFWDBConnection
                        db.DBOpen(fws, False)
                        sql = "SELECT dbo.IsVariation(@conId) AS [IsVariation]"
                        db.AddDBParam("conId", Session("ActiveContract"), True)
                        db.RunSQL(sql, db.glDBWorkA, False, "", False)

                        Dim CDLocked As Boolean = cLocks.IsLocked(Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.EmployeeID)
                        Dim CALocked As Boolean = cLocks.IsLocked(Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.EmployeeID)

                        If db.GetFieldValue(db.glDBWorkA, "IsVariation", 0, 0) > 0 Or curContract = 0 Then
                            lnkNewVariation.Visible = False
                            allowDelete = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractVariations, True)
                            onloadFunctionAppend = "SetIsVariation(true);"
                        Else

                            ' check for locked contract. If locked, don't permit actions
                            allowDelete = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractDetails, False) And Not (CDLocked Or CALocked)
                        End If

                        lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractDetails, False) And Not (CDLocked Or CALocked)
                        If lnkNewVariation.Visible Then
                            lnkNewVariation.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractVariations, False) And Not (CDLocked Or CALocked)
                        End If

                        lnkDelete.Visible = allowDelete

                        db.DBClose()
                        db = Nothing
                    Case Else

                End Select

                Master.addStyle = SetStyle(curUser.Employee, False)
                Master.onloadfunc = "SetContractId(" & curContract.ToString & ");" & onloadFunctionAppend '"ChangeCSPage(" & ActiveViewTab & ",false," & curContract.ToString & ");SetCSPermissions(" & uinfo.permInsert.ToString.ToLower & "," & allowDelete.ToString.ToLower & "," & uinfo.permNotes.ToString.ToLower & "," & fws.glUseSavings.ToString.ToLower & ");SetCPCount(" & Session("CPCount") & ");" & onloadFunctionAppend
            End If

            'Dim dummyRelTxt As New relationshipTextbox
            'dummyRelTxt.ID = "dummyrtb"
            'dummyRelTxt.TextBox.Style.Add("display", "none")
            'pnlDummyRTB.Controls.Add(dummyRelTxt)
        End Sub

        Private Sub SetViewPermissions(ByVal ActiveViewTab As SummaryTabs)
            Dim helpID As String = "#0"
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            Select Case ActiveViewTab
                Case SummaryTabs.ContractDetail
                    If Session("ActiveContract") = 0 Then
                        helpID = "#1073"
                    Else
                        helpID = "#1074"
                    End If
                    cmdCDUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractDetails, False)
                    cmdCDUpdate.Attributes.Add("onclick", "javascript:if(validateform('cddetails') == false) { return false; }")

                Case SummaryTabs.ContractAdditional
                    helpID = "#1077"
                    cmdCAUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractAdditional, False)
                    cmdCAUpdate.Attributes.Add("onclick", "javascript:if(validateform('cadetails') == false) { return false; }")

                Case SummaryTabs.ContractProduct
                    helpID = "#1078"

                    'If Session("CPAction") = "add" Then
                    '    cmdCPUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractProducts, False)
                    'ElseIf Session("CPAction") = "edit" Then
                    '    cmdCPUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False)
                    'End If


                Case SummaryTabs.InvoiceDetail
                    helpID = "#1093"

                Case SummaryTabs.InvoiceForecast
                    helpID = "#1097"
                    cmdIFUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False)

                Case SummaryTabs.LinkedContracts
                    helpID = "#1163"

                Case SummaryTabs.NotesSummary
                    helpID = "#1079"

                Case SummaryTabs.RechargeTemplate
                    helpID = "#1120"

                Case SummaryTabs.RechargePayments
                    helpID = "#1118"

                Case Else
            End Select
            
        End Sub

        Private Sub SetScreen(ByVal ActiveViewTab As SummaryTabs, ByVal accountid As Integer, ByVal employeeid As Integer)
            Dim connStr As String = cAccounts.getConnectionString(accountid)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            'ChangeCSPage(ActiveViewTab, Session("ActiveContract"), employeeid)

            'SetTabOptions()
            'SetViewPermissions(ActiveViewTab)
            'SetHeadings(ActiveViewTab)

            ChangeCSPage(ActiveViewTab, Session("ActiveContract"), curUser.EmployeeID)

            SetTabOptions()
            SetViewPermissions(ActiveViewTab)
            SetHeadings(ActiveViewTab)

            Select Case ActiveViewTab
                Case SummaryTabs.ContractDetail
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    ContractDetails_Load()

                Case SummaryTabs.ContractAdditional
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    ContractAdditional_Load()

                Case SummaryTabs.ContractProduct
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    ContractProduct_Load()

                Case SummaryTabs.InvoiceDetail
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    InvoiceDetails_Load()

                Case SummaryTabs.InvoiceForecast
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    InvoiceForecast_Load()

                Case SummaryTabs.NotesSummary
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    NoteSummary_Load()

                Case SummaryTabs.LinkedContracts
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    LinkedContracts_Load()

                Case SummaryTabs.RechargeTemplate
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)

                Case SummaryTabs.RechargePayments
                    ' release any lock for user in other tabs
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)
                    cLocks.RemoveLockItem(accountid, connStr, Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), employeeid)

                Case Else

            End Select
        End Sub

#Region "SetHeadings"
        Private Sub SetHeadings(ByVal ActiveViewTab As SummaryTabs)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim clsBaseDefs As cBaseDefinitions
            Dim accProperties As cAccountProperties
            If curUser.CurrentSubAccountId >= 0 Then
                accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Else
                accProperties = subaccs.getFirstSubAccount().SubAccountProperties
            End If

            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection()
            db.DBOpen(fws, True)

            Select Case ActiveViewTab
                Case SummaryTabs.ContractDetail
                    Dim sql As String
                    'Dim vlist As New Infragistics.WebUI.UltraWebGrid.ValueList

                    'If Session("XRes") < 1000 Then
                    '	lblUniqueKey.Text = "Key"
                    '	lblContractNumber.Text = "Contract No."
                    '	lblSupersedesContract.Text = "Supersedes"

                    '	lblDescription.Text = accProperties.ContractDescShortTitle

                    '	lblSchedule.Text = "Schedule"
                    '	lblCategory.Text = accProperties.ContractCategoryTitle

                    '	lblContractType.Text = "Contract Type"
                    '	lblTermType.Text = "Term Type"
                    '	litVendor.Text = accProperties.SupplierPrimaryTitle
                    '	lblSupplierCode.Text = accProperties.SupplierPrimaryTitle & " Code"
                    '	lblCDCurrency.Text = "Currency"
                    '	lblInvoiceFreq.Text = "Inv. Freq."
                    '	lblMaintenanceType.Text = "Incr. Type"
                    '	lblContractDate.Text = "Start Date (*)"
                    '	lblRenewalDate.Text = "End Date (*)"
                    '	lblCancellationPeriod.Text = "Cancel Pd"
                    '	lblCancellationDate.Text = "Cancel Date"
                    '	lblReviewPeriod.Text = "Review Pd"
                    '	lblReviewDate.Text = "Review Date"
                    '	lblReviewCompleteDate.Text = "Review Done"
                    '	lblContractValue.Text = "Con. Value"
                    '	lblTotalContractValue.Text = "Total Con.Value"
                    '	lblContractStatus.Text = "Status"
                    '	lblContractOwner.Text = "Owner"
                    '	'lblNotify.Text = GetLang(422) & ":"
                    '	lblForecastType.Text = "Forecast Type"
                    '	lblPenaltyClause.Text = accProperties.PenaltyClauseTitle

                    '	lblAnnualContractValue.Text = "Annual Value"
                    '	lblAttachments.Text = "Attachments"
                    'Else
                    lblUniqueKey.Text = "Contract Key"
                    lblContractNumber.Text = "Contract No."
                    lblSupersedesContract.Text = "Supersedes Contract"
                    lblDescription.Text = accProperties.ContractDescTitle & " *"
                    lblSchedule.Text = "Schedule"
                    lblCategory.Text = accProperties.ContractCategoryTitle

                    lblContractType.Text = "Contract Type"
                    lblTermType.Text = "Term Type"
                    lblVendor.Text = accProperties.SupplierPrimaryTitle & " *"
                    lblSupplierCode.Text = accProperties.SupplierPrimaryTitle & " Code"
                    lblCDCurrency.Text = "Currency"
                    lblInvoiceFreq.Text = "Invoice Frequency"
                    lblMaintenanceType.Text = "Increase Type"
                    lblContractDate.Text = "Start Date"
                    lblRenewalDate.Text = "End Date"
                    lblCancellationPeriod.Text = "Cancellation Period"
                    lblCancellationDate.Text = "Cancellation Date"
                    lblReviewPeriod.Text = "Review Period"
                    lblReviewDate.Text = "Review Date"
                    lblReviewCompleteDate.Text = "Review Complete"
                    lblContractValue.Text = "Contract Value"
                    lblTotalContractValue.Text = "Total Contract Value"
                    lblContractStatus.Text = "Contract Status"
                    lblContractOwner.Text = "Contract Owner"
                    lblForecastType.Text = "Forecast Type"
                    lblPenaltyClause.Text = accProperties.PenaltyClauseTitle

                    lblAnnualContractValue.Text = "Annual Value"
                    lblAttachments.Text = "Attachments"
                    'End If

                    txtSchedule.Text = accProperties.ContractScheduleDefault
                    lblAnnualContractValue.Text += " " & accProperties.ValueComments
                    lblContractValue.Text += " " & accProperties.ValueComments

                    If accProperties.ContractCatMandatory = False Then
                        cmpContractCategory.Enabled = False
                    Else
                        cmpContractCategory.Enabled = True
                        cmpContractCategory.ErrorMessage = lblCategory.Text & " field is mandatory"
                    End If

                    ' populate the review maintenance type drop down list
                    lstMaintenanceType.Items.Clear()
                    lstMaintenanceType.Items.Insert(MaintType.SingleInflator, New ListItem("Single inflator (x entry only)", MaintType.SingleInflator))
                    lstMaintenanceType.Items.Insert(MaintType.GreaterXY, New ListItem("Greater inflator of x & y", MaintType.GreaterXY))
                    lstMaintenanceType.Items.Insert(MaintType.LesserXY, New ListItem("Lesser inflator of x & y", MaintType.LesserXY))

                    ' populate the forecast type ddlist
                    lstForecastType.Items.Clear()
                    lstForecastType.Items.Insert(ForecastType.Prod_v_Inflator, New ListItem("Product £ v Inflator", ForecastType.Prod_v_Inflator))
                    lstForecastType.Items.Insert(ForecastType.InflatorOnly, New ListItem("Inflator Only", ForecastType.InflatorOnly))
                    'lstForecastType.Items.Insert(ApplicationProperties.ForecastType.Staged, New ListItem("Staged Maint.", ApplicationProperties.ForecastType.Staged))

                    'sql = "SELECT * FROM [codes_inflatormetrics] WHERE [Location Id] = @LocationId ORDER BY [Name]"
                    '               db.AddDBParam("LocationId", curUser.CurrentSubAccountId.Value, True)
                    'db.RunSQL(sql, db.glDBWorkA, False, "", False)

                    ''vlist.ValueListItems.Insert(0, "0", "%")
                    'lstMaintParam1.Items.Clear()
                    'lstMaintParam1.DataSource = db.glDBWorkA
                    'lstMaintParam1.DataTextField = "Name"
                    'lstMaintParam1.DataValueField = "Metric Id"
                    'lstMaintParam1.DataBind()

                    clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.InflatorMetrics)

                    lstMaintParam1.Items.AddRange(clsBaseDefs.CreateDropDown(False, 0))
                    lstMaintParam1.Items.Insert(0, New ListItem("%", "0"))
                    lstMaintParam1.ClearSelection()
                    lstMaintParam1.Items.FindByText("%").Selected = True

                    'lstMaintParam2.Items.Clear()
                    'lstMaintParam2.DataSource = db.glDBWorkA
                    'lstMaintParam2.DataTextField = "Name"
                    'lstMaintParam2.DataValueField = "Metric Id"
                    'lstMaintParam2.DataBind()

                    lstMaintParam2.Items.AddRange(clsBaseDefs.CreateDropDown(False, 0))
                    lstMaintParam2.Items.Insert(0, New ListItem("%", "0"))
                    lstMaintParam2.ClearSelection()
                    lstMaintParam2.Items.FindByText("%").Selected = True

                    ' populate the invoice frequency type ddlist

                    ' populate term type ddlist


                    ' Populate the currency ddlist
                    Dim currencies As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
                    lstCDCurrency.Items.Clear()
                    lstCDCurrency.Items.AddRange(currencies.CreateDropDown.ToArray)

                    lstCDCurrency.Items.Insert(0, New ListItem("[None]", "0"))

                    ' populate the vendor ddlist
                    Dim suppliers As New cSuppliers(curUser.Account.accountid, curUser.CurrentSubAccountId)

                    lstVendor.Items.Clear()
                    lstVendor.Items.AddRange(suppliers.getListItemsForContractAdd(True))

                    ' populate the contract type ddlist


                    ' populate the contract status

                    '               sql = "SELECT [StatusId],[StatusDescription] FROM [codes_contractstatus] WHERE [subAccountId] = @subAccId ORDER BY [StatusDescription]"
                    '               db.AddDBParam("subAccId", curUser.CurrentSubAccountId.Value, True)
                    'db.RunSQL(sql, db.glDBWorkA, False, "", False)
                    'lstContractStatus.DataSource = db.glDBWorkA
                    '               lstContractStatus.DataTextField = "StatusDescription"
                    '               lstContractStatus.DataValueField = "StatusId"
                    'lstContractStatus.DataBind()

                    'populate the contract owner list
                    Dim emps As New cEmployees(curUser.AccountID)
                    lstContractOwner.Items.Clear()
                    lstContractOwner.Items.AddRange(emps.CreateDropDown(0, False))
                    lstContractOwner.Items.Insert(0, New ListItem("[None]", "0"))

                    'sql = "SELECT [Staff Id],[Staff Name] FROM [staff_details] WHERE [Location Id] = @Location_Id ORDER BY [Staff Name]"
                    'db.AddDBParam("Location Id", uinfo.ActiveLocation, True)
                    'db.RunSQL(sql, db.glDBWorkA)
                    'lstContractOwner.DataSource = db.glDBWorkA
                    'lstContractOwner.DataTextField = "Staff Name"
                    'lstContractOwner.DataValueField = "Staff Id"
                    'lstContractOwner.DataBind()

                    'lstContractOwner.Items.Insert(0, New ListItem("[None]", "0"))

                    ' populate available links to ddlist
                    sql = "SELECT DISTINCT [linkId],[linkDefinition] FROM [link_definitions] "
                    'sql += "INNER JOIN [Link Matrix] ON [Link Matrix].[Link Id] = [Link Definitions].[Link Id] "
                    'sql += "INNER JOIN [Contract Details] ON [Contract Details].[Contract Id] = [Link Matrix].[Contract Id] "
                    'sql += "WHERE [Contract Details].[Location Id] = 0 "
                    sql += "WHERE [isScheduleLink] = 0 AND [subAccountId] = @subAccId "
                    sql += "ORDER BY [linkDefinition]"
                    db.AddDBParam("subAccId", curUser.CurrentSubAccountId, True)
                    db.RunSQL(sql, db.glDBWorkA, False, "", False)
                    lstLinkManage.Items.Clear()

                    If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractLinks, True) Then
                        lstLinkManage.DataSource = db.glDBWorkA
                        lstLinkManage.DataTextField = "LinkDefinition"
                        lstLinkManage.DataValueField = "LinkId"
                        lstLinkManage.DataBind()
                    End If

                    If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractLinks, True) Then
                        'lstLinkManage.Items.Insert(0, New ListItem("<new link definition>", "0"))
                        lstLinkManage.Items.Insert(0, New ListItem("[None]", "-1"))
                    End If

                    If Not curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractLinks, True) And Not curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractLinks, True) Then

                        imgLinkManage.Visible = False
                        lstLinkManage.Enabled = False
                    End If

                    lstSchedules.ToolTip = "** = you are not granted access to this contract"
                    lstNotify.ToolTip = "** = Team notification"

                    'panelMaintParams.Visible = accProperties.InflatorActive
                    ' change from hiding 1 panel to the separate cells as the panel also holds term types which should stay
                    panelMaintParams1.Visible = accProperties.InflatorActive
                    panelMaintParams2.Visible = accProperties.InflatorActive
                    lstInvoiceFreq.Enabled = accProperties.InvoiceFreqActive
                    lstTermType.Enabled = accProperties.TermTypeActive

                    imgAudience.Attributes.Add("onmouseover", "window.status='Define the audience of users permitted to access this contract';return true;")
                    imgAudience.Attributes.Add("onmouseout", "window.status='Done';")
                    imgAudience.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractAudience, False)

                Case SummaryTabs.ContractAdditional

                Case SummaryTabs.ContractProduct
                    Dim sql As String

                    lblProductName.Text = "Product Name"
                    lblProductValue.Text = "Product Value"
                    lblCPCurrency.Text = "Currency"
                    lblMaintValue.Text = "Annual Cost"
                    lblMaintPercent.Text = "Annual Cost %"
                    lblUnits.Text = "Unit Description"
                    lblProjectedSaving.Text = "Projected Saving"
                    lblUnitCost.Text = "Unit Cost"
                    lblCPSalesTax.Text = "Sales Tax"
                    lblQuantity.Text = "Quantity"
                    lblPricePaid.Text = "Price Paid"
                    lblProductCategory.Text = "Product Category"

                    'If Session("CP_AddMulti") = 0 Then
                    ' populate the available products
                    sql = "SELECT DISTINCT [productDetails].[ProductId],[productDetails].[ProductName]" & vbNewLine & _
                     "FROM (([contract_details]" & vbNewLine & _
                     "INNER JOIN [product_suppliers] " & vbNewLine & _
                     "ON [contract_details].[supplierId] = [product_suppliers].[supplierId])" & vbNewLine & _
                     "LEFT OUTER JOIN [productDetails] " & vbNewLine & _
                     "ON [product_suppliers].[ProductId] = [productDetails].[ProductId]) " & vbNewLine & _
                     "WHERE [contract_details].[ContractId] = @conId " & _
                     "ORDER BY [ProductName]"

                    db.AddDBParam("conId", Session("ActiveContract"), True)
                    db.RunSQL(sql, db.glDBWorkB, False, "Products", False)
                    lstProductName.Items.Clear()
                    lstProductName.DataSource = db.glDBWorkB
                    lstProductName.DataMember = "Products"
                    lstProductName.DataValueField = "ProductId"
                    lstProductName.DataTextField = "ProductName"
                    lstProductName.DataBind()

                    lstProductName.Items.Insert(0, New ListItem("[None]", "0"))
                    'Else
                    If Session("CP_AddMulti") <> 0 Then
                        ' if adding multi, then need to populate and activate the product category list box
                        lstProductCategory.Enabled = True
                        lstProductCategory.Visible = True
                        lblProductCategory.Visible = True

                        clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories)

                        lstProductCategory.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))
                    End If

                    ' populate the available units
                    'sql = "SELECT [Unit Id],[Unit Description] FROM [codes_units] WHERE [Location Id] = @locId"
                    'db.AddDBParam("locId", curUser.CurrentSubAccountId.Value, True)
                    'db.RunSQL(sql, db.glDBWorkB, False, "Units", False)

                    'lstUnits.DataSource = db.glDBWorkB
                    'lstUnits.DataMember = "Units"
                    'lstUnits.DataTextField = "Unit Description"
                    'lstUnits.DataValueField = "Unit Id"
                    'lstUnits.DataBind()

                    'lstUnits.Items.Insert(0, New ListItem("[None]", "0"))

                    'sql = "SELECT [salesTaxId],[description] FROM [codes_salestax] WHERE [archived] = @archived AND [subAccountId] = @locId"
                    'db.AddDBParam("locId", curUser.CurrentSubAccountId, True)
                    'db.AddDBParam("archived", 0, False)
                    'db.RunSQL(sql, db.glDBWorkB, False, "Tax", False)
                    'lstCPSalesTax.DataSource = db.glDBWorkB
                    'lstCPSalesTax.DataMember = "Tax"
                    'lstCPSalesTax.DataTextField = "description"
                    'lstCPSalesTax.DataValueField = "salesTaxId"
                    'lstCPSalesTax.DataBind()

                    'lstCPSalesTax.Items.Insert(0, New ListItem("[None]", "0"))
                    clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.SalesTax)
                    lstCPSalesTax.Items.Clear()
                    lstCPSalesTax.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                    Dim currencies As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
                    lstCPCurrency.Items.Clear()
                    lstCPCurrency.Items.AddRange(currencies.CreateDropDown().ToArray)
                    'sql = "SELECT [Currency Id],[Name] FROM [codes_currency] WHERE [Location Id] = @locId"
                    'db.AddDBParam("locId", uinfo.ActiveLocation, True)
                    'db.RunSQL(sql, db.glDBWorkB, False, "Currency")
                    'lstCPCurrency.DataSource = db.glDBWorkB
                    'lstCPCurrency.DataMember = "Currency"
                    'lstCPCurrency.DataTextField = "Name"
                    'lstCPCurrency.DataValueField = "Currency Id"
                    'lstCPCurrency.DataBind()

                    lstCPCurrency.Items.Insert(0, New ListItem("[None]", "0"))

                Case SummaryTabs.InvoiceDetail
                    Dim sql As String

                    lblInvoiceNumber.Text = "Invoice Number"
                    lblIDAmount.Text = "Amount"
                    lblInvSalesTax.Text = "Sales Tax"
                    lblStatus.Text = "Invoice Status"
                    lblDueDate.Text = "Due Date"
                    lblReceivedDate.Text = "Received Date"
                    lblInvoicePaidDate.Text = "Paid Date"
                    lblPaymentRef.Text = "Payment Ref"
                    lblIDComment.Text = "Comment"
                    lblIDPONumber.Text = "PO Number"
                    lblIDPOMaxValue.Text = "PO Max Value"
                    lblIDPOStart.Text = "PO Start Date"
                    lblIDPOExpiry.Text = "PO Expiry Date"
                    lblIDCoverEnd.Text = "Cover Period Ends"

                    ' populate ddlists
                    'sql = "SELECT * FROM [codes_salestax] WHERE [subAccountId] = @subAccId ORDER BY [description]"
                    'db.AddDBParam("subAccId", curUser.CurrentSubAccountId, True)
                    'db.RunSQL(sql, db.glDBWorkB, False, "Tax", False)

                    'lstInvSalesTax.DataSource = db.glDBWorkB
                    'lstInvSalesTax.DataMember = "Tax"
                    'lstInvSalesTax.DataTextField = "description"
                    'lstInvSalesTax.DataValueField = "salesTaxId"
                    'lstInvSalesTax.DataBind()

                    clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.SalesTax)
                    lstInvSalesTax.Items.Clear()
                    lstInvSalesTax.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                    'sql = "SELECT * FROM [invoiceStatusType] WHERE [subAccountId] = @subAccId ORDER BY [description]"
                    'db.AddDBParam("subAccId", curUser.CurrentSubAccountId, True)
                    'db.RunSQL(sql, db.glDBWorkB, False, "Status", False)

                    'lstStatus.DataSource = db.glDBWorkB
                    'lstStatus.DataMember = "Status"
                    'lstStatus.DataTextField = "description"
                    'lstStatus.DataValueField = "invoiceStatusTypeID"
                    'lstStatus.DataBind()

                    '' insert a blank entry at pos 0
                    ''lstInvSalesTax.Items.Insert(0, New ListItem("[None]", "0"))
                    'lstStatus.Items.Insert(0, New ListItem("[None]", "0"))

                    clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.InvoiceStatus)
                    lstStatus.Items.Clear()
                    lstStatus.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))


                Case SummaryTabs.InvoiceForecast
                    lblForecastAmount.Text = "Forecast Amount"
                    lblForecastDate.Text = "Forecast Date"
                    lblProductAmount.Text = "Product / Amount"
                    lblIFComment.Text = "Comment"
                    lblIFPONumber.Text = "PO Number"
                    lblIFPOStart.Text = "PO Start"
                    lblIFPOExpiry.Text = "PO Expiry"
                    lblIFPOMaxValue.Text = "PO Max.Value"
                    lblIFCoverEnd.Text = "Cover Period Ends"
                Case Else

            End Select
        End Sub
#End Region

#Region "Contract Summary Code"
        Private Sub ContractSummary_Load(ByVal ActiveTab As SummaryTabs)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim FWDb As New cFWDBConnection
            Dim rscoll As cRechargeSettings
            Dim rs As cRechargeSetting
            Dim cId As Integer = 0
            Dim action As String = ""
            Dim loc As String = ""
            Dim ARec As New cAuditRecord
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim accProperties As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            Dim clsModules As New cModules
            Dim clsModule As cModule = clsModules.GetModuleByID(CInt(curUser.CurrentActiveModule))
            Dim brandName As String = clsModule.BrandNameHTML

            'Set the element for the audit log
            Dim element As SpendManagementElement

            Select Case ActiveTab
                Case SummaryTabs.ContractDetail
                    element = SpendManagementElement.ContractDetails
                Case SummaryTabs.ContractAdditional
                    element = SpendManagementElement.ContractAdditional
                Case SummaryTabs.ContractProduct
                    element = SpendManagementElement.ContractProducts
                Case SummaryTabs.InvoiceDetail
                    element = SpendManagementElement.Invoices
                Case SummaryTabs.InvoiceForecast
                    element = SpendManagementElement.InvoiceForecasts
            End Select

            Dim ALog As New cFWAuditLog(fws, element, curUser.CurrentSubAccountId)

            If fws.glUseRechargeFunction Then
                rscoll = New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
                rs = rscoll.getSettings
            End If

            If Me.IsPostBack = False Then
                FWDb.DBOpen(fws, False)

                'Master.isContractScreen = True

                action = Request.QueryString("action")
                If Not Request.QueryString("id") Is Nothing Then
                    Integer.TryParse(Request.QueryString("id"), cId)

                    ' check access permissions for user to the contract
                    If ContractRoutines.CheckAudienceAccess(curUser.AccountID, curUser.Employee, cAccounts.getConnectionString(curUser.AccountID), "contract_audience", cId, curUser.CurrentSubAccountId) = False Then
                        ' permission denied
                        FWDb.DBClose()
                        Response.Redirect("Home.aspx?error=4", True)
                    End If
                    Session("ActiveContract") = cId
                End If
                loc = Request.QueryString("loc")

                ActiveTab = Request.QueryString("tab")

                If Not loc Is Nothing Then
                    If Integer.Parse(loc) <> curUser.CurrentSubAccountId Then
                        Throw New NotImplementedException("Need to refresh user permissions - not implemented")
                        'newUInfo = SecurityRoutines.GetRolePermissions(uinfo, Integer.Parse(loc), fws)

                        '' check that user has access permissions for this location
                        'If newUInfo.ActiveLocation > -1 Then
                        '	uinfo = newUInfo
                        '	uinfo = SecurityRoutines.GetCountInfo(uinfo, fws)
                        'Else
                        '	'access to requested new location has been rejected
                        '	FWDb.DBClose()
                        '	FWDb = Nothing
                        '	Response.Redirect("Home.aspx?error=2", True)
                        'End If
                    End If
                End If

                Select Case action
                    Case "addschedule"
                        Dim origKey As String = ""

                        FWDb.FWDb("R3", "contract_details", "contractId", cId, "", "", "", "", "", "", "", "", "", "")
                        If FWDb.FWDb3Flag = True Then
                            origKey = FWDb.FWDbFindVal("contractKey", 3)
                            If Trim(origKey) = "" Then
                                origKey = FWDb.FWDbFindVal("contractNumber", 3)
                            End If
                        End If

                        cId = AddSchedule(Server, cId, curUser)
                        If cId < 1 Then
                            ' invalid creation of the new schedule
                            cId = Val(Request.QueryString("id"))
                        Else
                            ' must have validly created the new schedule
                            FWDb.FWDb("R2", "contract_details", "contractId", cId, "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDb2Flag = True Then
                                Session("ActiveContract") = cId
                                ARec.Action = cFWAuditLog.AUDIT_ADD
                                If Trim(FWDb.FWDbFindVal("contractKey", 2)) <> "" Then
                                    ARec.ContractNumber = FWDb.FWDbFindVal("contractKey", 2)
                                Else
                                    ARec.ContractNumber = FWDb.FWDbFindVal("contractNumber", 2)
                                End If
                                ARec.DataElementDesc = "CONTRACT SCHEDULE"
                                ARec.ElementDesc = "ADD SCHEDULE"
                                ARec.PreVal = origKey
                                ARec.PostVal = ARec.ContractNumber
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, cId)
                            End If
                        End If

                    Case "mergeschedule"
                        FWDb.DBClose()
                        FWDb = Nothing
                        Response.Redirect("ScheduleManage.aspx?action=merge&cid=" & cId.ToString.Trim, True)

                    Case "updateschedule"
                        Dim origKey As String = ""

                        FWDb.FWDb("R3", "contract_details", "contractId", cId, "", "", "", "", "", "", "", "", "", "")
                        If FWDb.FWDb3Flag = True Then
                            origKey = FWDb.FWDbFindVal("contractKey", 3)
                            If Trim(origKey) = "" Then
                                origKey = FWDb.FWDbFindVal("contractNumber", 3)
                            End If
                        End If

                        cId = UpdateSchedule(Server, cId, curUser)
                        If cId < 1 Then
                            ' invalid creation of the new schedule
                            cId = Val(Request.QueryString("id"))
                        Else
                            ' must have validly created the new schedule
                            Session("ActiveContract") = cId
                            FWDb.FWDb("R2", "contract_details", "contractId", cId, "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDb2Flag = True Then
                                ARec.Action = cFWAuditLog.AUDIT_ADD
                                If Trim(FWDb.FWDbFindVal("contractKey", 2)) <> "" Then
                                    ARec.ContractNumber = FWDb.FWDbFindVal("contractKey", 2)
                                Else
                                    ARec.ContractNumber = FWDb.FWDbFindVal("contractNumber", 2)
                                End If
                                ARec.DataElementDesc = "CONTRACT SCHEDULE"
                                ARec.ElementDesc = "UPDATE SCHEDULE"
                                ARec.PreVal = origKey
                                ARec.PostVal = ARec.ContractNumber
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, cId)
                            End If
                        End If

                    Case "rechargepayment"
                        Dim rp_id As Integer
                        rp_id = Request.QueryString("rpid")
                        If rp_id > 0 Then
                            FWDb.FWDb("R", "contract_productdetails_recharge", "rechargeItemId", rp_id, "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDbFlag = True Then
                                Dim tmpSQL As New System.Text.StringBuilder
                                tmpSQL.Append("SELECT [contract_details].[contractId] FROM [contract_details] ")
                                tmpSQL.Append("INNER JOIN [contract_productdetails] ON [contract_details].[contractId] = [contract_productdetails].[contractId] ")
                                tmpSQL.Append("INNER JOIN [recharge_associations] ON [recharge_associations].[contractProductId] = [contract_productdetails].[contractProductId] ")
                                tmpSQL.Append("WHERE [rechargeId] = @rcid")
                                FWDb.AddDBParam("rcid", FWDb.FWDbFindVal("Recharge Id", 1), True)
                                FWDb.RunSQL(tmpSQL.ToString, FWDb.glDBWorkL, False, "", False)
                                If FWDb.glNumRowsReturned > 0 Then
                                    cId = FWDb.GetFieldValue(FWDb.glDBWorkL, "contractId", 0, 0)
                                    Session("tmpFromDate") = FWDb.FWDbFindVal("rechargePeriod", 1)
                                End If
                            End If
                        End If

                    Case "email"
                        ' secure attachment request from ddlist
                        Dim att_id As Integer

                        att_id = Request.QueryString("attid")

                        If fws.glMailServer <> "" Then
                            'If SMRoutines.SendAttachmentRequest(FWDb, fws, curUser.Employee, att_id, AttachmentArea.CONTRACT, Session("ActiveContract"), Server) = False Then
                            lblErrorString.Text = "ERROR! Secure Attachment email request failed. Contact your " & brandName & " Administrator."
                            'End If
                        End If
                    Case Else

                End Select

                If cId > 0 Then
                    ' check to make sure that the contract actually exists
                    FWDb.FWDb("R2", "contract_details", "contractId", cId, "subAccountId", curUser.CurrentSubAccountId, "", "", "", "", "", "", "", "")
                    If FWDb.FWDb2Flag = False Then
                        ' contract wasn't found in the active location, redirect to home page
                        FWDb.DBClose()
                        FWDb = Nothing
                        Response.Redirect("Home.aspx?error=3", True)
                    End If

                    FWDb.FWDb("R3", "subAccountAccess", "employeeId", curUser.EmployeeID, "subAccountId", curUser.CurrentSubAccountId, "", "", "", "", "", "", "", "")
                    If FWDb.FWDb3Flag Then
                        FWDb.SetFieldValue("lastContractId", cId, "N", True)
                        FWDb.FWDb("A", "subAccountAccess", "employeeId", curUser.EmployeeID, "subAccountId", curUser.CurrentSubAccountId, "", "", "", "", "", "", "", "")
                    Else
                        FWDb.SetFieldValue("lastContractId", cId, "N", True)
                        FWDb.SetFieldValue("subAccountId", curUser.CurrentSubAccountId, "N", False)
                        FWDb.SetFieldValue("employeeId", curUser.EmployeeID, "N", False)
                        FWDb.FWDb("W", "subAccountAccess", "", "", "", "", "", "", "", "", "", "", "", "")
                    End If

                    Dim strSQL As String
                    strSQL = "SELECT COUNT(*) AS [CPCount] FROM [contract_productdetails] WHERE [contractId] = @conId"
                    FWDb.AddDBParam("conId", cId, True)
                    FWDb.RunSQL(strSQL, FWDb.glDBWorkA, False, "", False)
                    Dim tmpCount As Integer = FWDb.GetFieldValue(FWDb.glDBWorkA, "CPCount", 0, 0)
                    Session("CPCount") = tmpCount

                    If fws.glUseRechargeFunction Then
                        ' instigate the caching of templates and payments for this contract
                        Dim cc As New cCacheCollections(curUser, curUser.Employee)

                        If Cache("RCT_" & Session("ActiveContract")) Is Nothing Then
                            ' only do background cache, if it not already being done
                            Dim crt As New cCacheCollections.CacheRTCollection(AddressOf cc.CacheTemplateCollection)
                            Dim rtsyncres As IAsyncResult

                            Cache("RCT_" & Session("ActiveContract")) = 1
                            rtsyncres = crt.BeginInvoke(cId, New AsyncCallback(AddressOf CSRTCallBack), crt)
                        End If

                        If Cache("RCP_" & Session("ActiveContract")) Is Nothing And Cache("RCG_" & Session("ActiveContract")) Is Nothing Then
                            Dim rpsyncres As IAsyncResult
                            Dim cp_info As New cCPFieldInfo(fws.MetabaseCustomerId, curUser.CurrentSubAccountId, fws.getConnectionString, curUser.EmployeeID, cId)
                            Dim crp As New cCacheCollections.CacheRPCollection(AddressOf cc.CachePaymentCollection)

                            Cache("RCP_" & Session("ActiveContract")) = 1
                            rpsyncres = crp.BeginInvoke(cp_info, cId, New AsyncCallback(AddressOf CSRPCallBack), crp)
                        End If
                    End If
                End If

                FWDb.DBClose()
                FWDb = Nothing
            End If
        End Sub

        Private Sub CSRTCallBack(ByVal result As IAsyncResult)
            Dim rt As cCacheCollections.CacheRTCollection = CType(result.AsyncState, cCacheCollections.CacheRTCollection)
            rt.EndInvoke(result)

            If Not Cache("RCT_" & Session("ActiveContract")) Is Nothing Then
                Cache.Remove("RCT_" & Session("ActiveContract"))
            End If
        End Sub

        Private Sub CSRPCallBack(ByVal result As IAsyncResult)
            Dim rp As cCacheCollections.CacheRPCollection = CType(result.AsyncState, cCacheCollections.CacheRPCollection)
            rp.EndInvoke(result)

            If Not Cache("RCP_" & Session("ActiveContract")) Is Nothing Then
                Cache.Remove("RCP_" & Session("ActiveContract"))
            End If
        End Sub

        Private Sub SetTabOptions()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim properties As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            If Session("ActiveContract") Is Nothing Then
                Session("ActiveContract") = 0
            End If

            Dim omout As String = "window.status='Done';"
            Dim omover As String = "window.status='%msg%';return true;"

            If Session("ActiveContract") > 0 Then
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractAdditional, False) = False Then
                    lnkCAnav.Visible = False
                Else
                    lnkCAnav.ToolTip = "Display additional contract details"
                    lnkCAnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCAnav.ToolTip))
                    lnkCAnav.Attributes.Add("onmouseout", omout)
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProducts, False) = False Then
                    lnkCPnav.Visible = False
                Else
                    lnkCPnav.ToolTip = "Display contract products associated with this contract"
                    lnkCPnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCPnav.ToolTip))
                    lnkCPnav.Attributes.Add("onmouseout", omout)
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Invoices, False) = False Then
                    lnkIDnav.Visible = False
                Else
                    lnkIDnav.ToolTip = "Display invoice details associated with the active contract"
                    lnkIDnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkIDnav.ToolTip))
                    lnkIDnav.Attributes.Add("onmouseout", omout)
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceForecasts, False) = False Then
                    lnkIFnav.Visible = False
                Else
                    lnkIFnav.ToolTip = "Display forecast payments associated with the active contract"
                    lnkIFnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkIFnav.ToolTip))
                    lnkIFnav.Attributes.Add("onmouseout", omout)
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractNotes, False) = False And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierNotes, False) = False And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierContactNotes, False) = False And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceNotes, False) = False And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductNotes, False) = False Then
                    lnkNSnav.Visible = False
                Else
                    lnkNSnav.ToolTip = "Display collated summary of free text notes associated with the active contract"
                    lnkNSnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNSnav.ToolTip))
                    lnkNSnav.Attributes.Add("onmouseout", omout)
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractHistory, False) = False Then
                    lnkCHnav.Visible = False
                Else
                    lnkCHnav.ToolTip = "Display amendment history of the active contract"
                    lnkCHnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkCHnav.ToolTip))
                    lnkCHnav.Attributes.Add("onmouseout", omout)
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractLinks, False) = True Then
                    If Session("ActiveContract") > 0 Then
                        ' contract was found previously
                        Dim Sql As New System.Text.StringBuilder
                        Sql.Append("SELECT COUNT(*) AS [LnkCnt] FROM [link_matrix] ")
                        Sql.Append("INNER JOIN [link_definitions] ON [link_definitions].[linkId] = [link_matrix].[linkId] ")
                        Sql.Append("WHERE [link_definitions].[isScheduleLink] = 0 AND [contractId] = @cId")
                        db.sqlexecute.Parameters.AddWithValue("@cId", Session("ActiveContract"))
                        Dim count As Integer = db.getcount(Sql.ToString)

                        If count > 0 Then
                            ' links with the contract exist, so display the tab and separator image
                            lnkLCnav.Visible = True
                            lnkLCnav.ToolTip = "Display contracts linked via definitions"
                            lnkLCnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkLCnav.ToolTip))
                            lnkLCnav.Attributes.Add("onmouseout", omout)
                        End If
                    End If
                End If

                If properties.EnableRecharge And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAssociations, False) = True Then
                    Dim rscoll As New cRechargeSettings(curUser.Account.accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.Account.accountid))
                    Dim rs As cRechargeSetting = rscoll.getSettings

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargePayments, False) = True Then
                        lnkRPnav.Visible = True
                        lnkRPnav.ToolTip = "View recharge payments defined against contract products"
                        lnkRPnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkRPnav.ToolTip))
                        lnkRPnav.Attributes.Add("onmouseout", omout)
                    End If

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAssociations, False) = True Then
                        lnkRTnav.Visible = True
                        lnkRTnav.ToolTip = "Define / Modify recharge apportioning between " & rs.ReferenceAs & "s"
                        lnkRTnav.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkRTnav.ToolTip))
                        lnkRTnav.Attributes.Add("onmouseout", omout)
                    End If
                End If
            Else
                ' must be adding a contract, so hide other tabs other than contract detail
                lnkCAnav.Visible = False
                lnkCPnav.Visible = False
                lnkIDnav.Visible = False
                lnkIFnav.Visible = False
                lnkNSnav.Visible = False
                lnkLCnav.Visible = False
                lnkRTnav.Visible = False
                lnkRPnav.Visible = False
                lnkCHnav.Visible = False
                lnkTSnav.Visible = False
            End If
        End Sub

        Private Sub ChangeCSPage(ByVal viewId As SummaryTabs, ByVal contractId As Integer, ByVal employeeid As Integer)
            'lnkVRegistry.ToolTip = "Open the Version Registry screen for product / services"

            Dim omout As String = "window.status='Done';"
            Dim omover As String = "window.status='%msg%';return true;"
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim accProperties As cAccountProperties
            If curUser.CurrentSubAccountId >= 0 Then
                accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Else
                accProperties = subaccs.getFirstSubAccount().SubAccountProperties
            End If

            Select Case viewId
                Case SummaryTabs.ContractDetail
                    lnkNew.Visible = False 'uinfo.permInsert
                    'lnkNew.ToolTip = "Add a new contract"
                    'lnkNew.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNew.ToolTip))
                    'lnkNew.Attributes.Add("onmouseout", omout)
                    lnkNewDefine.Visible = False
                    lnkNewVariation.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractVariations, False)
                    lnkNewVariation.ToolTip = "Add a new variation to the current contract"
                    lnkNewVariation.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNewVariation.ToolTip))
                    lnkNewVariation.Attributes.Add("onmouseout", omout)

                    If contractId = 0 Then
                        lnkDelete.Visible = False
                        lnkNotes.Visible = False
                        lnkSaving.Visible = False
                        lnkAddCDTask.Visible = False

                        ' Task Summary Link
                        lnkTSnav.Visible = False
                    Else
                        lnkDelete.Visible = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractDetails, False)
                        lnkDelete.Attributes.Add("onclick", "javascript:DeleteContract(" & contractId.ToString & ");")
                        lnkDelete.ToolTip = "Delete the active contract"
                        lnkDelete.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkDelete.ToolTip))
                        lnkDelete.Attributes.Add("onmouseout", omout)
                        lnkNotes.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractNotes, False)
                        lnkNotes.ToolTip = "View free text notes for the active contract"
                        lnkNotes.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNotes.ToolTip))
                        lnkNotes.Attributes.Add("onmouseout", omout)
                        lnkSaving.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSavings, False)
                        lnkSaving.ToolTip = "View or register saving made against the current contract"
                        lnkSaving.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkSaving.ToolTip))
                        lnkSaving.Attributes.Add("onmouseout", omout)
                        lnkAddCDTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)
                        lnkAddCDTask.ToolTip = "Create a task for the current contract"
                        lnkAddCDTask.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkAddCDTask.ToolTip))
                        lnkAddCDTask.Attributes.Add("onmouseout", omout)
                        lnkAddCDTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)
                        lnkTSnav.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tasks, False)
                    End If
                    lnkBulkUpdate.Visible = False
                    lnkGenerate.Visible = False
                    'lnkVRegistry.Visible = False
                    lnkAddIFTask.Visible = False
                    lnkAddCPTask.Visible = False
                    lnkAddIDTask.Visible = False

                Case SummaryTabs.ContractAdditional
                    lnkNew.Visible = False
                    lnkNewDefine.Visible = False
                    lnkNewVariation.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractDetails, False)
                    lnkNewVariation.ToolTip = "Add a new variation to the current contract"
                    lnkNewVariation.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNewVariation.ToolTip))
                    lnkNewVariation.Attributes.Add("onmouseout", omout)

                    If contractId = 0 Then
                        lnkNotes.Visible = False
                        lnkAddCDTask.Visible = False
                        lnkTSnav.Visible = False
                    Else
                        lnkNotes.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractNotes, False)
                        lnkNotes.ToolTip = "View free text notes for the active contract"
                        lnkNotes.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNotes.ToolTip))
                        lnkNotes.Attributes.Add("onmouseout", omout)
                        lnkAddCDTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)
                        lnkAddCDTask.ToolTip = "Create a task for the current contract"
                        lnkAddCDTask.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkAddCDTask.ToolTip))
                        lnkAddCDTask.Attributes.Add("onmouseout", omout)
                        lnkAddCDTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)
                        lnkTSnav.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tasks, False)
                    End If
                    lnkDelete.Visible = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractDetails, False)
                    lnkDelete.Attributes.Add("onclick", "javascript:DeleteContract(" & contractId.ToString & ");")
                    lnkDelete.ToolTip = "Delete the active contract"
                    lnkDelete.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkDelete.ToolTip))
                    lnkDelete.Attributes.Add("onmouseout", omout)
                    lnkBulkUpdate.Visible = False
                    lnkGenerate.Visible = False
                    'lnkVRegistry.Visible = False
                    If contractId = 0 Then
                        lnkSaving.Visible = False
                    Else
                        lnkSaving.Visible = (curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSavings, False) And accProperties.EnableContractSavings)
                        lnkSaving.ToolTip = "View or register saving made against the current contract"
                        lnkSaving.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkSaving.ToolTip))
                        lnkSaving.Attributes.Add("onmouseout", omout)
                    End If
                    lnkAddIFTask.Visible = False
                    lnkAddCPTask.Visible = False
                    lnkAddIDTask.Visible = False

                Case SummaryTabs.ContractProduct
                    lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractProducts, False)
                    lnkNew.ToolTip = "Add a new contract product to the active contract"
                    lnkNew.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNew.ToolTip))
                    lnkNew.Attributes.Add("onmouseout", omout)
                    lnkNewDefine.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractProducts, False)
                    lnkNewDefine.ToolTip = "Define and Add a new contract product to the active contract"
                    lnkNewDefine.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNewDefine.ToolTip))
                    lnkNewDefine.Attributes.Add("onmouseout", omout)
                    lnkNewVariation.Visible = False
                    lnkNotes.Visible = False
                    lnkDelete.Visible = False
                    lnkBulkUpdate.Visible = False
                    lnkGenerate.Visible = False
                    'If contractId = 0 Then
                    '    lnkVRegistry.Visible = False
                    'Else
                    '    If Session("CPCount") > 0 Then
                    '        lnkVRegistry.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VersionRegistry, False)
                    '        lnkVRegistry.ToolTip = "Access the version registry for products on the active contract"
                    '        lnkVRegistry.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkVRegistry.ToolTip))
                    '        lnkVRegistry.Attributes.Add("onmouseout", omout)
                    '    Else
                    '        lnkVRegistry.Visible = False
                    '    End If
                    'End If
                    lnkSaving.Visible = False
                    lnkAddCDTask.Visible = False
                    lnkAddIFTask.Visible = False
                    lnkAddCPTask.Visible = False
                    lnkAddIDTask.Visible = False

                    lnkTSnav.Visible = False

                Case SummaryTabs.InvoiceDetail
                    lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Invoices, False)
                    lnkNew.ToolTip = "Add a new invoice to the active contract"
                    lnkNew.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNew.ToolTip))
                    lnkNew.Attributes.Add("onmouseout", omout)
                    lnkNewDefine.Visible = False
                    lnkNewVariation.Visible = False
                    lnkNotes.Visible = False
                    lnkDelete.Visible = False
                    lnkBulkUpdate.Visible = False
                    lnkGenerate.Visible = False
                    'lnkVRegistry.Visible = False
                    lnkSaving.Visible = False
                    lnkAddCDTask.Visible = False
                    lnkAddIFTask.Visible = False
                    lnkAddCPTask.Visible = False
                    lnkTSnav.Visible = False

                Case SummaryTabs.InvoiceForecast
                    lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceForecasts, False)
                    lnkNew.ToolTip = "Add a new forecast payment to the active contract"
                    lnkNew.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkNew.ToolTip))
                    lnkNew.Attributes.Add("onmouseout", omout)
                    lnkNewDefine.Visible = False
                    lnkNewVariation.Visible = False
                    lnkNotes.Visible = False
                    lnkDelete.Visible = False
                    lnkBulkUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False)
                    lnkBulkUpdate.ToolTip = "Perform a bulk update to forecast entries"
                    lnkBulkUpdate.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkBulkUpdate.ToolTip))
                    lnkBulkUpdate.Attributes.Add("onmouseout", omout)
                    lnkGenerate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False)
                    lnkGenerate.ToolTip = "Generate forecast payments for the active contract according to parameters supplied"
                    lnkGenerate.Attributes.Add("onmouseover", omover.Replace("%msg%", lnkGenerate.ToolTip))
                    lnkGenerate.Attributes.Add("onmouseout", omout)
                    'lnkVRegistry.Visible = False
                    lnkSaving.Visible = False
                    lnkAddCDTask.Visible = False
                    lnkAddIFTask.Visible = False
                    lnkAddCPTask.Visible = False
                    lnkAddIDTask.Visible = False

                    lnkTSnav.Visible = False

                Case SummaryTabs.NotesSummary
                    lnkNew.Visible = False
                    lnkNewDefine.Visible = False
                    lnkNewVariation.Visible = False
                    lnkNotes.Visible = False
                    lnkDelete.Visible = False
                    lnkBulkUpdate.Visible = False
                    lnkGenerate.Visible = False
                    'lnkVRegistry.Visible = False
                    lnkSaving.Visible = False
                    lnkAddCDTask.Visible = False
                    lnkAddIFTask.Visible = False
                    lnkAddCPTask.Visible = False
                    lnkAddIDTask.Visible = False

                    lnkTSnav.Visible = False

                Case SummaryTabs.LinkedContracts
                    lnkNew.Visible = False
                    lnkNewDefine.Visible = False
                    lnkNewVariation.Visible = False
                    lnkNotes.Visible = False
                    lnkDelete.Visible = False
                    lnkBulkUpdate.Visible = False
                    lnkGenerate.Visible = False
                    'lnkVRegistry.Visible = False
                    lnkSaving.Visible = False
                    lnkAddCDTask.Visible = False
                    lnkAddIFTask.Visible = False
                    lnkAddCPTask.Visible = False
                    lnkAddIDTask.Visible = False

                    lnkTSnav.Visible = False

                Case Else
                    lnkNew.Visible = False
                    lnkNewDefine.Visible = False
                    lnkNewVariation.Visible = False
                    lnkNotes.Visible = False
                    lnkDelete.Visible = False
                    lnkBulkUpdate.Visible = False
                    lnkGenerate.Visible = False
                    'lnkVRegistry.Visible = False
                    lnkSaving.Visible = False
                    lnkAddCDTask.Visible = False
                    lnkAddIFTask.Visible = False
                    lnkAddCPTask.Visible = False
                    lnkAddIDTask.Visible = False
                    lnkTSnav.Visible = False
            End Select
        End Sub

        Protected Sub lnkNew_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim url As String = ""
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))

            Select Case CType(ViewTab.ActiveViewIndex, SummaryTabs)
                Case SummaryTabs.ContractDetail, SummaryTabs.ContractAdditional
                    Session("CDAction") = "add"
                    url = "ContractSummary.aspx?cdaction=add&id=0&tab=" & SummaryTabs.ContractDetail
                Case SummaryTabs.ContractProduct
                    Dim ConCategoryId As Integer = 0

                    Session("CP_AddMulti") = "0"
                    Session("CPAction") = "add"

                    Dim sql As String = "select [categoryId] from contract_details where [contractId] = @conId"
                    db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))

                    ConCategoryId = db.getIntSum(sql)

                    NewCPEntry(ConCategoryId)

                    url = "" '"ContractSummary.aspx?cpaction=add&tab=" & SummaryTabs.ContractProduct
                Case SummaryTabs.InvoiceDetail
                    Session("IDAction") = "add"
                    url = "ContractSummary.aspx?idaction=add&tab=" & SummaryTabs.InvoiceDetail
                Case SummaryTabs.InvoiceForecast
                    Session("IFAction") = "add"
                    url = "ContractSummary.aspx?ifaction=add&tab=" & SummaryTabs.InvoiceForecast
                Case Else

            End Select

            If url <> "" Then
                Response.Redirect(url, True)
            End If
        End Sub

        Protected Sub lnkNewVariation_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Session("CDAction") = "addvariation"
            Response.Redirect("ContractSummary.aspx?tab=0&id=" & Session("ActiveContract") & "&cdaction=addvariation", True)
        End Sub

        Protected Sub lnkNotes_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim url As String = ""

            Select Case CType(ViewTab.ActiveViewIndex, SummaryTabs)
                Case SummaryTabs.ContractDetail, SummaryTabs.ContractAdditional
                    url = "ViewNotes.aspx?notetype=Contract&id=-1&contractid=" & Session("ActiveContract") & "&item=" & Replace(Replace(Session("CurContractDesc"), "&", "%26"), vbNewLine, "")
                Case Else

            End Select

            If url <> "" Then
                Response.Redirect(url, True)
            End If
        End Sub

        Protected Sub lnkNewDefine_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim ConCategoryId As Integer = 0
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            Session("CP_AddandDefine") = "1"
            Session("CP_AddMulti") = "1"
            Session("CPAction") = "add"

            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))

            Dim sql As String = "select [categoryId] from contract_details where [contractId] = @conId"
            db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))

            ConCategoryId = db.getIntSum(sql)

            NewCPEntry(ConCategoryId)

            'Response.Redirect("ContractSummary.aspx?cpaction=add&am=1&tab=" & SummaryTabs.ContractProduct, True)
        End Sub

        'Protected Sub lnkVRegistry_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        '    Response.Redirect("VersionRegistry.aspx?cid=" & Session("ActiveContract"), True)
        'End Sub

        Protected Sub lnkBulkUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            CallBulkUpdates()
        End Sub

        Protected Sub lnkGenerate_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Session("IFAction") = "generate"
            Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast, True)
        End Sub

        Protected Sub lnkSaving_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Response.Redirect("ContractSavings.aspx", True)
        End Sub

#End Region

#Region "Contract Details Code"
        Private Sub ContractDetails_Load()
            Dim IsArchived As Boolean
            Dim IsLocked As Boolean = False
            Dim newNum As String = ""
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim colParams As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, False, True)

            Dim action As String
            action = Session("CDAction")
            Master.useCloseNavigationMsg = False
            'Master.enablenavigation = False

            Select Case action
                Case "add"
                    Session("ActiveContract") = 0
                    Session("ActiveContractDesc") = ""

                Case "addvariation"
                    Dim origKey As String = ""
                    Dim cId As Integer = Session("ActiveContract")
                    Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
                    Dim sql As String = "select [contractKey] from contract_details where [contractId] = @conId"
                    Dim ARec As New cAuditRecord
                    Dim ALog As New cAuditLog(curUser.Account.accountid, curUser.Employee.employeeid)
                    Dim origContractId As Integer = cId

                    cId = AddVariation(Server, curUser, cId, colParams)
                    If cId < 1 Then
                        ' invalid creation of the new schedule
                        cId = Val(Request.QueryString("id"))
                    Else
                        ' must have validly created the new schedule
                        db.sqlexecute.Parameters.Clear()
                        db.sqlexecute.Parameters.AddWithValue("@conId", cId)

                        origKey = db.getStringValue(sql)
                        ARec.Action = "I"
                        ARec.ContractNumber = origKey
                        ARec.DataElementDesc = "NEW CONTRACT VARIATION"
                        ARec.ElementDesc = ""
                        ALog.addRecord(SpendManagementElement.ContractDetails, "Contract Variation", cId)

                        Session("ActiveContract") = cId

                        ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.ContractDetail, origContractId, ARec)
                    End If

                    ' remove the addvariation now complete otherwise keeps adding variations!!!
                    Session("CDAction") = Nothing

                Case "vopen"
                    ' re-open a variation
                    Dim vId As Integer = Request.QueryString("vid")
                    SetVariationState(curUser, "open", vId)

                Case "vclose"
                    ' close a variation
                    Dim vId As Integer = Request.QueryString("vid")
                    SetVariationState(curUser, "close", vId)

                Case Else

            End Select

            reqStartDate.Enabled = colParams.ContractDatesMandatory
            reqEndDate.Enabled = colParams.ContractDatesMandatory
            reqCancellationDate.Enabled = colParams.ContractDatesMandatory
            reqReviewDate.Enabled = colParams.ContractDatesMandatory

            Dim fwdb As New cFWDBConnection
            fwdb.DBOpen(fws, False)

            If Not curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractAudience, False) Then
                imgAudience.Visible = False
            Else
                imgAudience.Visible = True
            End If

            ' Get the details of the active contract (if there is one)
            If Session("ActiveContract") > 0 Then
                IsArchived = GetContractData(Session("ActiveContract"))
                LoadCDUFData(fws, curUser, fwdb, Session("ActiveContract"))

                ' lock contract to this user, unless it is already locked
                Dim cacheHit As Boolean = False
                Dim LockedBy As String = "Unknown User"
                Dim cacheKey As String = "CD_" & curUser.AccountID.ToString() & "_" & Session("ActiveContract")

                If Not Cache(cacheKey) Is Nothing Then
                    Dim lockEmployeeId As Integer = CInt(Cache(cacheKey))
                    cacheHit = True

                    If lockEmployeeId <> curUser.EmployeeID Then
                        IsLocked = True
                        Dim emps As New cEmployees(curUser.AccountID)
                        Dim lockEmployee As Employee = emps.GetEmployeeById(lockEmployeeId)
                        If Not lockEmployee Is Nothing Then
                            LockedBy = lockEmployee.FullName
                        End If
                    End If
                End If

                If IsLocked = False And cacheHit = False Then
                    Dim cacheTimeout As Double = colParams.CacheTimeout

                    ' lock contract and any variations or if it is a variation, other variations and the primary contract
                    cLocks.LockContract(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), cacheTimeout, curUser.EmployeeID)
                    litLockStatus.Text = ""
                ElseIf IsLocked = True Then
                    ' display notification at the top of the screen to indicate that it is locked and by whom
                    litLockStatus.Text = "<img src=""./icons/16/plain/lock.gif"" alt=""Locked"" />&nbsp;The contract is currently locked by " & LockedBy
                End If

                'litUserFields.Text = GetUserDefinedFields(db, False, True)
                Session("ActiveContractDesc") = txtDescription.Text

                Dim schedSQL As New System.Text.StringBuilder
                schedSQL.Append("SELECT dbo.CheckContractAccess(@userId,[link_matrix].[contractId], @subAccountId) AS [PermitVal],[link_matrix].[contractId],[contract_details].[contractKey],ISNULL([contract_details].[scheduleNumber],'n/a') AS [scheduleNumber] FROM [link_matrix] " & vbNewLine)
                schedSQL.Append("INNER JOIN [contract_details] ON [link_matrix].[contractId] = [contract_details].[contractId] " & vbNewLine)
                schedSQL.Append("WHERE [linkId] IN (" & vbNewLine)
                schedSQL.Append("SELECT [link_definitions].[linkId] FROM [link_definitions] " & vbNewLine)
                schedSQL.Append("INNER JOIN [link_matrix] ON [link_matrix].[linkId] = [link_definitions].[linkId] " & vbNewLine)
                schedSQL.Append("WHERE [link_matrix].[contractId] = @conId AND [isScheduleLink] = 1) " & vbNewLine)
                schedSQL.Append("ORDER BY [startDate],[endDate] ASC" & vbNewLine)
                fwdb.AddDBParam("conId", Session("ActiveContract"), True)
                fwdb.AddDBParam("userId", curUser.EmployeeID, False)
                fwdb.AddDBParam("subAccountId", curUser.CurrentSubAccountId, False)
                fwdb.RunSQL(schedSQL.ToString, fwdb.glDBWorkD, False, "", False)

                If fwdb.glNumRowsReturned > 0 Then
                    Dim drow As DataRow
                    Dim locked As String = ""
                    For Each drow In fwdb.glDBWorkD.Tables(0).Rows
                        If drow.Item("contractId") <> Session("ActiveContract") Then
                            If drow.Item("PermitVal") = 0 Then
                                locked = "** "
                            Else
                                locked = ""
                            End If
                            lstSchedules.Items.Add(New ListItem(locked & drow.Item("scheduleNumber") & "->" & CStr(drow.Item("contractKey")).Replace(fws.KeyPrefix & "/", ""), drow.Item("contractId")))
                        End If
                    Next
                    lstSchedules.Items.Insert(0, New ListItem("Select schedule to view", 0))
                Else
                    lstSchedules.Items.Insert(0, New ListItem("None Defined", 0))
                    lstSchedules.Enabled = False
                End If
            Else
                ' adding a new contract, so don't permit delete,notes etc.
                'lnkDelete.Visible = False
                cmdNotes.Visible = False
                imgSchedules.Visible = False

                imgAudience.Visible = False
                lnkCHnav.Visible = False

                Dim clsBaseDefs As cBaseDefinitions

                'Populate Base Information dropdowns
                lstContractCategory.Items.Clear()
                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractCategories)
                lstContractCategory.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractTypes)
                lstContractType.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.TermTypes)
                lstTermType.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.InvoiceFrequencyTypes)
                lstInvoiceFreq.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractStatus)
                lstContractStatus.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                ' if auto-generating contract id, then get next available contract id and insert into contract number field
                txtContractNumber.Text = colParams.ContractNumSeq.ToString("000000")
            End If

            If fws.glAutoUpdateCV = True Then
                fteACV.Enabled = False
                txtAnnualContractValue.Enabled = False
            End If

            ' check to see if we are adding new contract for a particular vendor
            Dim tmpStr As String

            tmpStr = Request.QueryString("supplierid")
            If tmpStr <> "" Then
                lstVendor.ClearSelection()
                lstVendor.Items.FindByValue(tmpStr).Selected = True
                lstVendor.Enabled = False
            End If

            'litUserFields.Text = GetUserDefinedFields(db, False, False)

            ' THIS IS NOW DONE IN THE PAGE_INIT
            'Dim ufields As New cUserDefinedFields(fws, uinfo)
            'CDUFPanel.Controls.Add(ufields.GetUserFieldDisplayTable(AppAreas.CONTRACT_DETAILS, Session("ActiveContract"), 0))

            UpdateContractLinks(fwdb, Session("ActiveContract"), curUser)
            'Else
            '' update any uf fields into viewstate
            'UpdateUFViewState(db)
            'End If

            'If uinfo.permDelete = True Then
            '    If Session("ActiveContract") = 0 Then
            '        lnkDelete.Visible = False
            '    Else
            '        If IsArchived = True And uinfo.permAdmin = False Then
            '            lnkDelete.Visible = False
            '        Else
            '            lnkDelete.Attributes.Add("onclick", "javascript:if(confirm('Click OK to confirm deletion')){window.navigate('ContractSummary.aspx?action=delete&id=" & Session("ActiveContract") & "');}")
            '            lnkDelete.Attributes.Add("onmouseover", "window.status='Delete this contract';return true;")
            '            lnkDelete.Attributes.Add("onmouseout", "window.status='Done';")
            '        End If
            '    End If
            'End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractNotes, False) Then
                If Session("ActiveContract") > 0 Then
                    If SMRoutines.Check4Notes(fws, AttachmentArea.CONTRACT_NOTES, Session("ActiveContract")) = True Then
                        cmdNotes.Visible = True
                    End If
                    Session("NoteReturnURL") = "ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract")
                End If

                cmdNotes.Attributes.Add("onmouseover", "window.status='Add / Edit / Delete notes for the current contract';return true;")
                cmdNotes.Attributes.Add("onmouseout", "window.status='Done';")
                cmdNotes.AlternateText = "Notes"
                cmdNotes.CausesValidation = False

                If Session("ActiveContract") = 0 Then
                    cmdNotes.Visible = False
                End If
            End If

            lnkNotify.Enabled = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractDetails, False)
            lnkNotify.CausesValidation = False

            If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractDetails, False) Then
                lnkNotify.ToolTip = "Modify nominations for email notifications"

                If Session("ActiveContract") < 1 Or IsLocked Then
                    lnkNotify.Enabled = False
                End If
                lnkNotify.Attributes.Add("onmouseover", "window.status='Add / Remove nominees for email notifications';return true;")
                lnkNotify.Attributes.Add("onmouseout", "window.status='Done';")
            End If

            'If uinfo.permExport = True Then
            '    litPrint.Text = "<a onmouseover=""window.status='Open window with details that are printer friendly';return true;"" onmouseout=""window.status='Done';"" href=""javascript:window.navigate(window.location);"" onclick=""javascript:window.open('ContractMain.aspx?action=print&id=" & Trim(Session("ActiveContract")) & "');""><img src=""./buttons/printer.gif"" /></a>"
            '    holderPrintButton.Controls.Add(litPrint)
            'End If

            cmdCDUpdate.ToolTip = "Update any changes to the system"
            cmdCDUpdate.AlternateText = "Update"

            If IsArchived = True Then
                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractDetails, False) = False Or IsLocked = True Then
                    cmdCDUpdate.Visible = False
                End If
            Else
                cmdCDUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractDetails, False) And Not IsLocked
            End If

            If cmdCDUpdate.Visible = False Then
                Master.useCloseNavigationMsg = True
                cmdCDCancel.ImageUrl = "./buttons/page_close.png"
            End If

            cmpAnnualValue.Enabled = Not fws.glAutoUpdateCV

            ' Add in the inflator help
            litInflatorHelp.Text = "" '"<a onmouseover=""window.status='Open inflator specific help document';return true;"" onmouseout=""window.status='Done';"" onclick=""javascript:window.open('./help/cd_inflator_metric.htm')""><img src=""./buttons/information.gif"" alt=""Open help text for use of the inflator metrics"" /></a>"

            lblContractLink.ToolTip = "* = already linked to group"
            lblContractLink.Text = "Create / Manage Links"

            If Session("ActiveContract") < 1 Then
                imgLinkManage.Visible = False
            End If
            imgLinkManage.Attributes.Add("onmouseover", "window.status='Create new / add to existing contract link group';return true;")
            imgLinkManage.Attributes.Add("onmouseout", "window.status='Done';")

            Master.RefreshBreadcrumbInfo()

            Dim uhd As UserHelpDocs
            Dim statusbar As String

            statusbar = "onmouseover=""window.status='Open Field Guidance Notes';return true;"" onmouseout=""window.status='Done';"" "
            uhd = Session("UserHelpDocs")
            If uhd.Supercedes <> "" Then
                txtSupersedesContract.Width = Unit.Pixel(140)
                litUH1.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.Supercedes) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If

            If uhd.ConType <> "" Then
                lstContractType.Width = Unit.Pixel(140)
                litUH2.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.ConType) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If

            If uhd.ConCategory <> "" Then
                lstContractCategory.Width = Unit.Pixel(140)
                litUH3.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.ConCategory) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If

            If uhd.ConStatus <> "" Then
                lstContractStatus.Width = Unit.Pixel(140)
                litUH4.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.ConStatus) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If

            If uhd.ConOwner <> "" Then
                lstContractOwner.Width = Unit.Pixel(140)
                litUH5.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.ConOwner) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If

            If uhd.ConValue <> "" Then
                txtContractValue.Width = Unit.Pixel(80)
                litUH6.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.ConValue) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If

            If uhd.ConAnnualValue <> "" Then
                txtAnnualContractValue.Width = Unit.Pixel(80)
                litUH7.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.ConAnnualValue) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If

            If uhd.ConTermType <> "" Then
                lstTermType.Width = Unit.Pixel(120)
                litUH8.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.ConTermType) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If

            If uhd.ConInvFreq <> "" Then
                lstInvoiceFreq.Width = Unit.Pixel(120)
                litUH9.Text = "<a " & statusbar & "onclick=""javascript:window.open('ViewAttachment.aspx?type=udh&udhp=" & Trim(uhd.ContractHelpDir) & "&udhf=" & Trim(uhd.ConInvFreq) & "')""><img src=""./buttons/information.gif"" alt=""Open Field Guidance Notes"" /></a>"
            End If
        End Sub

        Private Sub LoadCDUFData(ByVal fws As cFWSettings, ByVal curUser As CurrentUser, ByVal db As cFWDBConnection, ByVal curContractId As Integer)
            'db.FWDb("R3", "contract_details", "contractId", curContractId, "", "", "", "", "", "", "", "", "", "")
            ''If db.FWDb3Flag Then
            Dim tables As New cTables(fws.MetabaseCustomerId)
            Dim fields As New cFields(fws.MetabaseCustomerId)
            Dim ufields As New cUserdefinedFields(fws.MetabaseCustomerId)
            Dim cdTable As cTable = tables.getTableByName("contract_details")
            Dim udTable As cTable = tables.getTableById(cdTable.UserDefinedTableID)
            ufields.populateRecordDetails(phCDUFields, udTable, ufields.GetRecord(udTable, curContractId, tables, fields), GroupingOutputType.UnGroupedOnly)
        End Sub

        Private Function GetContractData(ByVal id As Integer) As Boolean
            Dim sql As String
            Dim newIdx As Integer
            Dim tmpStr As String
            Dim drow As DataRow
            Dim isArchived As Boolean
            Dim curCurrencyId, baseCurrencyId As Integer
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection

            db.DBOpen(fws, False)
            isArchived = False

            Dim clsBaseDefs As cBaseDefinitions
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim ContractCurrency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim baseCurrency As cCurrency
            If params.BaseCurrency.HasValue Then
                baseCurrency = ContractCurrency.getCurrencyById(params.BaseCurrency.Value)
            End If

            db.FWDb("R", "contract_details", "contractId", id, "", "", "", "", "", "", "", "", "", "")
            If db.FWDbFlag = True Then
                Dim startDate As Date
                Date.TryParse(db.FWDbFindVal("startDate", 1), startDate)

                curCurrencyId = Val(db.FWDbFindVal("contractCurrency", 1))
                If params.BaseCurrency.HasValue Then
                    baseCurrencyId = params.BaseCurrency.Value
                    baseCurrency = ContractCurrency.getCurrencyById(baseCurrencyId)

                    If curCurrencyId = 0 Then
                        curCurrencyId = baseCurrencyId
                    End If
                End If

                Session("CurContractDesc") = db.FWDbFindVal("contractDescription", 1)

                txtContractNumber.Text = db.FWDbFindVal("contractNumber", 1)
                Session("CurContractNumber") = txtContractNumber.Text
                txtSupersedesContract.Text = db.FWDbFindVal("supercedeContract", 1)
                txtDescription.Text = db.FWDbFindVal("contractDescription", 1)
                txtSupplierCode.Text = db.FWDbFindVal("supplierCode", 1)
                txtSchedule.Text = db.FWDbFindVal("scheduleNumber", 1)

                'oldIdx = lstVendor.SelectedItem.Value
                newIdx = SMRoutines.CheckListIndex(Integer.Parse(db.FWDbFindVal("supplierId", 1)))

                lstVendor.ClearSelection()

                If Not lstVendor.Items.FindByValue(newIdx.ToString()) Is Nothing Then
                    lstVendor.Items.FindByValue(newIdx).Selected = True
                Else
                    If newIdx > 0 Then
                        Dim clsSuppliers As New cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId)
                        Dim supp As cSupplier = clsSuppliers.getSupplierById(newIdx)
                        Dim addItem As Boolean = False

                        If Not IsNothing(supp) Then
                            If Not IsNothing(supp.SupplierStatus) Then
                                If supp.SupplierStatus.DenyContractAdd Then
                                    addItem = True
                                End If
                            End If

                            If addItem Then
                                lstVendor.Items.Add(New ListItem(supp.SupplierName, supp.SupplierId.ToString))
                            End If

                            If Not lstVendor.Items.FindByValue(newIdx) Is Nothing Then
                                lstVendor.Items.FindByValue(newIdx).Selected = True
                            End If
                        End If
                    End If
                End If

                ' if vendor is selected, set label as hyperlink to the vendor screen
                If newIdx > 0 Then
                    Dim tmpSupplierStr As String = params.SupplierPrimaryTitle
                    litVendorLink.Text = String.Format("<a href='shared/supplier_details.aspx?redir=1&sid={0}'><img src='shared/images/icons/view.png' title='View {1} details' /></a>", lstVendor.SelectedItem.Value.Trim, tmpSupplierStr)
                End If

                ' will destroy ref integrity if vendor can be changed (if conprods have been assigned).
                db.FWDb("R3", "contract_productdetails", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                If db.FWDb3Flag = True Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AddNewContract, False) = False Then
                    lstVendor.Enabled = False
                End If

                'oldIdx = lstContractCategory.SelectedItem.Value
                If db.FWDbFindVal("categoryId", 1) = "" Then
                    newIdx = 0
                Else
                    newIdx = SMRoutines.CheckListIndex(Integer.Parse(db.FWDbFindVal("categoryId", 1)))
                End If

                ' populate the contract category ddlist
                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractCategories)
                lstContractCategory.Items.Clear()
                lstContractCategory.Items.AddRange(clsBaseDefs.CreateDropDown(True, newIdx))
                lstContractCategory.ClearSelection()
                If lstContractCategory.Items.FindByValue(newIdx) IsNot Nothing Then
                    lstContractCategory.Items.FindByValue(newIdx).Selected = True
                End If
                'If oldIdx <> newIdx Then
                '    lstContractCategory.Items.FindByValue(oldIdx).Selected = False
                'End If

                'oldIdx = lstContractType.SelectedItem.Value
                If db.FWDbFindVal("contractTypeId", 1) = "" Then
                    newIdx = 0
                Else
                    newIdx = SMRoutines.CheckListIndex(Integer.Parse(db.FWDbFindVal("contractTypeId", 1)))
                End If

                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractTypes)
                lstContractType.Items.Clear()

                lstContractType.Items.AddRange(clsBaseDefs.CreateDropDown(True, newIdx))

                lstContractType.ClearSelection()
                lstContractType.Items.FindByValue(newIdx).Selected = True

                'If oldIdx <> newIdx Then
                '    lstContractType.Items.FindByValue(oldIdx).Selected = False
                'End If

                'oldIdx = lstCDCurrency.SelectedItem.Value
                If db.FWDbFindVal("contractCurrency", 1) = "" Then
                    newIdx = 0
                Else
                    newIdx = SMRoutines.CheckListIndex(Integer.Parse(db.FWDbFindVal("contractCurrency", 1)))
                End If

                lstCDCurrency.ClearSelection()
                If Not lstCDCurrency.Items.FindByValue(newIdx) Is Nothing Then
                    lstCDCurrency.Items.FindByValue(newIdx).Selected = True
                End If
                'If oldIdx <> newIdx Then
                '    lstCDCurrency.Items.FindByValue(oldIdx).Selected = False
                'End If

                'oldIdx = lstTermType.SelectedItem.Value
                If db.FWDbFindVal("termTypeId", 1) = "" Then
                    newIdx = 0
                Else
                    newIdx = SMRoutines.CheckListIndex(Integer.Parse(db.FWDbFindVal("termTypeId", 1)))
                End If

                lstTermType.Items.Clear()
                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.TermTypes)
                lstTermType.Items.AddRange(clsBaseDefs.CreateDropDown(True, newIdx))

                lstTermType.ClearSelection()
                lstTermType.Items.FindByValue(newIdx).Selected = True
                'If oldIdx <> newIdx Then
                '    lstTermType.Items.FindByValue(oldIdx).Selected = False
                'End If

                'oldIdx = lstInvoiceFreq.SelectedItem.Value

                newIdx = SMRoutines.CheckListIndex(Val(db.FWDbFindVal("invoiceFrequencyTypeId", 1)))

                lstInvoiceFreq.Items.Clear()
                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.InvoiceFrequencyTypes)
                lstInvoiceFreq.Items.AddRange(clsBaseDefs.CreateDropDown(True, newIdx))

                lstInvoiceFreq.ClearSelection()
                lstInvoiceFreq.Items.FindByValue(newIdx).Selected = True
                'If oldIdx <> newIdx Then
                '    lstInvoiceFreq.Items.FindByValue(oldIdx).Selected = False
                'End If

                'oldIdx = lstMaintenanceType.SelectedItem.Value
                newIdx = SMRoutines.CheckListIndex(Val(db.FWDbFindVal("maintenanceType", 1)))

                lstMaintenanceType.ClearSelection()
                lstMaintenanceType.Items.FindByValue(newIdx).Selected = True
                'If oldIdx <> newIdx Then
                '    lstMaintenanceType.Items.FindByValue(oldIdx).Selected = False
                'End If

                'oldIdx = lstMaintParam1.SelectedItem.Value
                newIdx = SMRoutines.CheckListIndex(db.FWDbFindVal("maintenanceInflatorX", 1))

                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.InflatorMetrics)

                lstMaintParam1.Items.Clear()
                lstMaintParam1.Items.AddRange(clsBaseDefs.CreateDropDown(False, newIdx))
                lstMaintParam1.Items.Insert(0, New ListItem("%", "0"))
                'lstMaintParam1.ClearSelection()
                'lstMaintParam1.Items.FindByText("%").Selected = True
                lstMaintParam1.ClearSelection()
                lstMaintParam1.Items.FindByValue(newIdx).Selected = True
                'If oldIdx <> newIdx Then
                '    lstMaintParam1.Items.FindByValue(oldIdx).Selected = False
                'End If

                'oldIdx = lstForecastType.SelectedItem.Value

                newIdx = SMRoutines.CheckListIndex(Val(db.FWDbFindVal("forecastTypeId", 1)))

                lstForecastType.ClearSelection()
                lstForecastType.Items.FindByValue(newIdx).Selected = True
                'If oldIdx <> newIdx Then
                '    lstForecastType.Items.FindByValue(oldIdx).Selected = False
                'End If

                'db.FWDb("R2", "Codes - Inflator Metrics", "Metric Id", lstMaintParam1.SelectedItem.Value)
                'If db.FWDb2Flag = True Then
                '    If db.FWDbFindVal("requires Extra Pct", 2) = 1 Then
                txtMaintParam1.Text = db.FWDbFindVal("maintenancePercentX", 1)
                If txtMaintParam1.Text Is String.Empty Then
                    txtMaintParam1.Text = 0
                End If
                '    Else
                '        txtMaintParam1.Text = "0"
                '    End If
                'Else
                '    If lstMaintParam1.SelectedItem.Text = "%" Then
                '        txtMaintParam1.Text = db.FWDbFindVal("Maintenance Percent X")
                '    End If
                'End If

                'oldIdx = lstMaintParam2.SelectedItem.Value
                newIdx = SMRoutines.CheckListIndex(Val(db.FWDbFindVal("maintenanceInflatorY", 1)))

                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.InflatorMetrics)

                lstMaintParam2.Items.Clear()
                lstMaintParam2.Items.AddRange(clsBaseDefs.CreateDropDown(False, newIdx))
                lstMaintParam2.Items.Insert(0, New ListItem("%", "0"))
                'lstMaintParam2.ClearSelection()
                'lstMaintParam2.Items.FindByText("%").Selected = True
                lstMaintParam2.ClearSelection()
                lstMaintParam2.Items.FindByValue(newIdx).Selected = True
                'If oldIdx <> newIdx Then
                '    lstMaintParam2.Items.FindByValue(oldIdx).Selected = False
                'End If

                'If lstContractStatus.Items.Count > 0 Then
                'oldIdx = lstContractStatus.SelectedItem.Value
                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractStatus)

                newIdx = SMRoutines.CheckListIndex(Val(db.FWDbFindVal("contractStatusId", 1)))
                lstContractStatus.Items.Clear()
                lstContractStatus.Items.AddRange(clsBaseDefs.CreateDropDown(True, newIdx))
                lstContractStatus.ClearSelection()

                If Not IsNothing(lstContractStatus.Items.FindByValue(newIdx)) Then
                    lstContractStatus.Items.FindByValue(newIdx).Selected = True
                End If
                'If oldIdx <> newIdx Then
                '    lstContractStatus.Items.FindByValue(oldIdx).Selected = False
                'End If

                If db.FWDbFindVal("Archived", 1) = "Y" Then
                    lstContractStatus.Enabled = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractDetails, False)
                    isArchived = True
                End If
                'End If
                tmpStr = db.FWDbFindVal("archiveDate", 1)
                If Trim(tmpStr) <> "" Then
                    lblContractStatus.Text = "Contract Status (on " & Format(CDate(tmpStr), cDef.DATE_FORMAT) & ")"
                End If

                'db.FWDb("R2", "Codes - Inflator Metrics", "Metric Id", lstMaintParam2.SelectedItem.Value)
                'If db.FWDb2Flag = True Then
                '    If db.FWDbFindVal("requires Extra Pct", 2) = 1 Then
                txtMaintParam2.Text = db.FWDbFindVal("maintenancePercentY", 1)
                If txtMaintParam2.Text Is String.Empty Then
                    txtMaintParam2.Text = 0
                End If
                '    Else
                '        txtMaintParam2.Text = "0"
                '    End If
                'Else
                '    If lstMaintParam2.SelectedItem.Text = "%" Then
                '        txtMaintParam2.Text = db.FWDbFindVal("Maintenance Percent Y")
                '    End If
                'End If

                If db.FWDbFindVal("contractValue", 1) = "" Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False) = False Then
                    tmpStr = "0"
                Else
                    tmpStr = db.FWDbFindVal("contractValue", 1)
                End If

                Dim currency As cCurrency = ContractCurrency.getCurrencyById(curCurrencyId)
                txtContractValue.Text = ContractCurrency.FormatCurrency(Double.Parse(tmpStr), currency, True)
                'otxtContractValue.Text = tmpStr

                If curCurrencyId <> baseCurrencyId Then
                    txtContractValue.ToolTip = "Base currency value is " & ContractCurrency.FormatCurrency(ContractCurrency.convertToBase(curCurrencyId, Decimal.Parse(tmpStr), startDate), baseCurrency, False)
                    txtContractValue.CssClass = "not_base_currency_border"
                End If

                If db.FWDbFindVal("annualContractValue", 1) = "" Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False) = False Then
                    tmpStr = "0"
                Else
                    tmpStr = db.FWDbFindVal("annualContractValue", 1)
                End If

                txtAnnualContractValue.Text = ContractCurrency.FormatCurrency(Double.Parse(tmpStr), currency, False)
                If fws.glAutoUpdateCV = False Then
                    Do While IsNumeric(Left(txtAnnualContractValue.Text, 1)) = False
                        txtAnnualContractValue.Text = Mid(txtAnnualContractValue.Text, 2)
                    Loop
                Else
                    fteACV.Enabled = False
                    txtAnnualContractValue.Enabled = False
                    txtAnnualContractValue.ToolTip = "ACV is set to be automatically updated as sum of Contract Products' Maintenance Values"
                End If

                If curCurrencyId <> baseCurrencyId Then
                    txtAnnualContractValue.ToolTip = "Base currency value is " & ContractCurrency.FormatCurrency(ContractCurrency.convertToBase(curCurrencyId, Decimal.Parse(tmpStr), startDate), baseCurrency, False)
                    txtAnnualContractValue.CssClass = "not_base_currency_border"
                End If

                txtContractDate.Text = FormatDate(db.FWDbFindVal("startDate", 1))
                txtRenewalDate.Text = FormatDate(db.FWDbFindVal("endDate", 1))
                txtCancellationPeriod.Text = db.FWDbFindVal("noticePeriod", 1)
                lstCancellationPeriodType.ClearSelection()
                lstCancellationPeriodType.Items.FindByValue(db.FWDbFindVal("noticePeriodType", 1)).Selected = True
                txtCancellationDate.Text = FormatDate(db.FWDbFindVal("cancellationDate", 1))
                txtReviewPeriod.Text = db.FWDbFindVal("reviewPeriod", 1)
                'oldIdx = lstReviewPeriodType.SelectedItem.Value
                newIdx = SMRoutines.CheckListIndex(db.FWDbFindVal("reviewPeriodType", 1))
                'If oldIdx <> newIdx Then
                '    lstReviewPeriodType.Items.FindByValue(oldIdx).Selected = False
                'End If

                lstReviewPeriodType.ClearSelection()
                lstReviewPeriodType.Items.FindByValue(newIdx).Selected = True
                txtReviewDate.Text = FormatDate(db.FWDbFindVal("reviewDate", 1))
                txtReviewCompleteDate.Text = FormatDate(db.FWDbFindVal("reviewComplete", 1))
                chkPenaltyClause.Checked = IIf(db.FWDbFindVal("cancellationPenalty", 1) = "1", True, False)

                'oldIdx = lstContractOwner.SelectedItem.Value
                Dim contractOwner As String = db.FWDbFindVal("contractOwner", 1)

                If Integer.TryParse(contractOwner, newIdx) Then
                    newIdx = SMRoutines.CheckListIndex(contractOwner)
                Else
                    newIdx = 0
                End If



                lstContractOwner.ClearSelection()

                If Not lstContractOwner.Items.FindByValue(newIdx) Is Nothing Then
                    lstContractOwner.Items.FindByValue(newIdx).Selected = True
                Else
                    Dim clsEmps As New cEmployees(curUser.AccountID)
                    Dim emp As Employee = clsEmps.GetEmployeeById(newIdx)

                    If Not emp Is Nothing Then
                        If emp.archived Then
                            lstContractOwner.Items.Add(New ListItem(emp.Surname & " " & emp.Title & " (" & emp.Username & ") (Archived)", emp.EmployeeID))
                            lstContractOwner.Items.FindByValue(emp.employeeid).Selected = True
                        End If

                    End If
                End If

                'If oldIdx <> newIdx Then
                '    lstContractOwner.Items.FindByValue(oldIdx).Selected = False
                'End If


                Dim notifySQL As New System.Text.StringBuilder
                ' Identify which staff / teams are on the notify list
                notifySQL.Append("SELECT [contract_notification].[employeeId],[IsTeam],[employees].firstname + ' ' + employees.surname AS [NotifyName] FROM [contract_notification] " & vbNewLine)
                notifySQL.Append("INNER JOIN [employees] ON [contract_notification].[employeeId] = [employees].[employeeId] " & vbNewLine)
                notifySQL.Append("WHERE [contractId] = @conId AND [IsTeam] = @NoTeam " & vbNewLine)
                notifySQL.Append("UNION " & vbNewLine)
                notifySQL.Append("SELECT [teamid],[IsTeam],[teamname] AS [NotifyName] FROM [contract_notification] " & vbNewLine)
                notifySQL.Append("INNER JOIN [teams] ON [teams].[teamid] = [contract_notification].[employeeId] " & vbNewLine)
                notifySQL.Append("WHERE [contract_notification].[contractId] = @conId AND [IsTeam] = @YesTeam" & vbNewLine)
                db.AddDBParam("conId", Session("ActiveContract"), True)
                db.AddDBParam("YesTeam", AudienceType.Team, False)
                db.AddDBParam("NoTeam", AudienceType.Individual, False)
                'sql = "SELECT [contract_notification].[Staff Id],[IsTeam],[staff_details].[Staff Name] FROM [contract_notification] " & _
                '    "INNER JOIN [staff_details] ON [contract_notification].[Staff Id] = [staff_details].[Staff Id] " & _
                '    "WHERE [Contract Id] = " & Trim(Session("ActiveContract")) & " ORDER BY [Staff Name]"
                db.RunSQL(notifySQL.ToString, db.glDBWorkA, False, "", False)

                Dim tmpNotify As New System.Text.StringBuilder

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    If Not drow.Item("NotifyName") Is Nothing Then
                        If CType(drow.Item("IsTeam"), AudienceType) = AudienceType.Team Then
                            tmpNotify.Append("**")
                        End If
                        tmpNotify.Append(drow.Item("NotifyName"))
                        tmpNotify.Append(vbNewLine)
                    End If
                Next
                lstNotify.Text = tmpNotify.ToString

                ' count the number of contract notes
                sql = "SELECT COUNT(*) AS [NumNotes] FROM [contractNotes] WHERE [contractId] = @conId"
                db.AddDBParam("conId", Session("ActiveContract"), True)
                db.RunSQL(sql, db.glDBWorkA, False, "", False)
                Dim numNotes As Integer
                numNotes = db.GetFieldValue(db.glDBWorkA, "NumNotes", 0, 0)
                txtNotes.Text = numNotes.ToString & " Notes"
                If params.EnableFlashingNotesIcon = True And numNotes > 0 Then
                    cmdNotes.ImageUrl = "~/images/attachment-flashing.gif"
                End If

                lblLastChangedValue.Text = Trim(db.FWDbFindVal("lastChangeDate", 1)) & " [" & Trim(db.FWDbFindVal("lastChangedBy", 1)) & "]"

                If Trim(db.FWDbFindVal("contractKey", 1)) <> "" Then
                    txtUniqueKeyValue.Text = Trim(db.FWDbFindVal("contractKey", 1))
                End If

                txtAudience.Text = GetContractAudience(db, id)
                sql = "SELECT dbo.VariationCount(@conId) AS [VarCount], dbo.IsVariation(@conId) AS [IsVariation], dbo.GetContractValue(@conId,1) AS [Total Contract Value]"
                db.AddDBParam("conId", id, True)
                db.RunSQL(sql, db.glDBWorkA, False, "", False)

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                    Dim totalValue As Double = 0
                    If IsDBNull(db.GetFieldValue(db.glDBWorkA, "Total Contract Value", 0, 0)) = False Then
                        totalValue = db.GetFieldValue(db.glDBWorkA, "Total Contract Value", 0, 0)
                    End If

                    txtTotalContractValue.Text = ContractCurrency.FormatCurrency(totalValue, currency, False)
                    If curCurrencyId <> baseCurrencyId Then
                        txtTotalContractValue.ToolTip += vbNewLine & "Base currency value is " & ContractCurrency.FormatCurrency(ContractCurrency.convertToBase(curCurrencyId, db.GetFieldValue(db.glDBWorkA, "Total Contract Value", 0, 0), startDate), baseCurrency, False)
                        txtTotalContractValue.CssClass = "not_base_currency_border"
                    End If
                Else
                    txtTotalContractValue.Text = ContractCurrency.FormatCurrency(0, currency, False)
                End If
                txtTotalContractValue.Enabled = False

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractVariations, True) Then
                    If db.GetFieldValue(db.glDBWorkA, "VarCount", 0, 0) > 0 Then
                        ' display variations
                        igVariationsPanel.Header.Text = CStr(db.GetFieldValue(db.glDBWorkA, "VarCount", 0, 0)) & " Variation(s) for this contract"
                        igVariationsPanel.Width = Unit.Percentage(85) '(CInt(Session("XRes")) / 100) * 82

                        litCDVariations.Text = GetVariationList(db, curUser, id)
                        panelVariations.Visible = True
                        If Request.QueryString("expand") = "1" Then
                            igVariationsPanel.Expanded = True
                        End If

                    ElseIf db.GetFieldValue(db.glDBWorkA, "IsVariation", 0, 0) = "1" Then
                        litCDVariations.Text = ReturnToPrimaryContract(db, id)
                        igVariationsPanel.Header.Text = "Contract tagged as a variation"
                        igVariationsPanel.Expanded = True
                        igVariationsPanel.Width = Unit.Percentage(85) '(CInt(Session("XRes")) / 100) * 82
                        panelVariations.Visible = True
                        lblSchedule.Text = "Variation/Extension"
                        lblContractValue.Text = "Variation Value"
                        panelMaintParams1.Visible = False
                        panelMaintParams2.Visible = False
                    End If
                End If

                If params.EnableContractNumUpdate = False Then
                    txtContractNumber.Enabled = False
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Attachments, True) Then
                    sql = "SELECT * FROM [attachments] WHERE [ReferenceNumber] = @RefNum AND [AttachmentArea] = @Attachment_Area AND dbo.CheckAttachmentAccess([AttachmentId],@userId) > 0"
                    db.AddDBParam("RefNum", Session("ActiveContract"), True)
                    db.AddDBParam("Attachment_Area", AttachmentArea.CONTRACT, False)
                    db.AddDBParam("userId", curUser.EmployeeID, False)
                    db.RunSQL(sql, db.glDBWorkA, False, "", False)

                    If Request.QueryString("cdaction") = "print" Then
                        If db.glNumRowsReturned > 0 Then
                            litAttachments.Text = "<i>" & db.glNumRowsReturned.ToString & " Attachments</i>"
                        Else
                            litAttachments.Text = "<i>No Attachments</i>"
                        End If
                    Else
                        Dim strHTML As New System.Text.StringBuilder
                        Dim crypt As New cSecureData

                        strHTML.Append("<select name=""attachments"" id=""attachments"" tabindex=""11"" onchange=""OpenAttachment();"">" & vbNewLine)

                        If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                            strHTML.Append("<option value=""0"" selected>[select attachment]</option>" & vbNewLine)

                            For Each drow In db.glDBWorkA.Tables(0).Rows
                                tmpStr = ""
                                If IsDBNull(drow.Item("Description")) = True Then
                                    If CType(drow.Item("AttachmentType"), AttachmentType) = AttachmentType.Hyperlink Then
                                        tmpStr = drow.Item("Directory")
                                    Else
                                        tmpStr = Trim(drow.Item("Directory")) & Trim(drow.Item("Filename"))
                                    End If
                                Else
                                    If Trim(drow.Item("Description")) = "" Then
                                        If CType(drow.Item("AttachmentType"), AttachmentType) = AttachmentType.Hyperlink Then
                                            tmpStr = Trim(drow.Item("Directory"))
                                        Else
                                            tmpStr = Trim(drow.Item("Directory")) & Trim(drow.Item("Filename"))
                                        End If
                                    Else
                                        tmpStr = Trim(drow.Item("Description"))
                                    End If

                                End If

                                Select Case CType(drow.Item("AttachmentType"), AttachmentType)
                                    Case AttachmentType.Hyperlink
                                        strHTML.Append("<option value=""" & Trim(drow.Item("AttachmentType")) & drow.Item("Directory") & """>" & tmpStr & "</option>" & vbNewLine)
                                    Case Else
                                        strHTML.Append("<option value=""" & Trim(drow.Item("AttachmentType")) & Server.UrlEncode(crypt.Encrypt(drow.Item("AttachmentId"))) & """>" & tmpStr & "</option>" & vbNewLine)
                                End Select

                            Next
                        Else
                            strHTML.Append("<option value=""0"">[No Links]</option>" & vbNewLine)
                        End If
                        strHTML.Append("</select>")
                        strHTML.Append("<script language=""javascript"" type=""text/javascript"">" & vbNewLine)
                        strHTML.Append("function OpenAttachment()" & vbNewLine & "{" & vbNewLine)
                        strHTML.Append("var path;" & vbNewLine)
                        strHTML.Append("var att_list = document.getElementById('attachments');" & vbNewLine)
                        strHTML.Append("path = att_list.options[att_list.selectedIndex].value;" & vbNewLine)
                        strHTML.Append("if(path != '0')" & vbNewLine & "{" & vbNewLine)
                        strHTML.Append("var att_type = path.substring(0,1);" & vbNewLine)
                        strHTML.Append("var att_path = path.substring(1);" & vbNewLine)
                        strHTML.Append("if(att_type == '" & AttachmentType.Open & "' || att_type == '" & AttachmentType.Audience & "')" & vbNewLine & "{" & vbNewLine)
                        strHTML.Append("window.open('ViewAttachment.aspx?id=' + att_path);" & vbNewLine)
                        strHTML.Append("}" & vbNewLine)
                        strHTML.Append("else if(att_type == '" & AttachmentType.Hyperlink & "')" & vbNewLine)
                        strHTML.Append("{" & vbNewLine)
                        strHTML.Append("window.open(att_path);" & vbNewLine)
                        strHTML.Append("}" & vbNewLine)
                        strHTML.Append("else" & vbNewLine)
                        strHTML.Append("{" & vbNewLine)
                        strHTML.Append("if(confirm('\tThis attachment is in the secure area.\nDo you wish to issue an email request for this attachment to the Contract Owner?'))" & vbNewLine)
                        strHTML.Append("{" & vbNewLine)
                        strHTML.Append("window.location.href='ContractSummary.aspx?action=email&attid=' + att_path;" & vbNewLine)
                        strHTML.Append("}" & vbNewLine)
                        strHTML.Append("}" & vbNewLine)
                        strHTML.Append("}" & vbNewLine)
                        strHTML.Append("}" & vbNewLine & "</script>" & vbNewLine)
                        litAttachmentIcon.Text = "<a onmouseover=""window.status='Open the file attachments screen';return true;"" onmouseout=""window.status='Done';"" href=""Attachments.aspx?attarea=" & Trim(AttachmentArea.CONTRACT) & "&ref=" & Trim(Session("ActiveContract")) & """><img alt=""Open the file attachments screen"" src=""./icons/16/plain/paperclip.png"" /></a>"


                        litAttachments.Text = strHTML.ToString

                    End If
                Else
                    litAttachments.Text = "" '"No access to view attachments"

                End If
            End If

            GetContractData = isArchived
        End Function

        Private Sub UpdateContractLinks(ByVal db As cFWDBConnection, ByVal contractId As Integer, ByVal curUser As CurrentUser)
            Dim drow As DataRow
            Dim li As ListItem
            Dim sql As New System.Text.StringBuilder

            sql.Append("SELECT [link_definitions].[linkId] FROM [link_matrix] ")
            sql.Append("INNER JOIN [link_definitions] ON [link_definitions].[linkId] = [link_matrix].[linkId] ")
            sql.Append("WHERE [contractId] = @Contract_Id AND [subAccountId] = @Location_Id")
            sql.Append(" ORDER BY [link_matrix].[linkId]")
            db.AddDBParam("Contract Id", contractId.ToString, True)
            db.AddDBParam("Location Id", curUser.CurrentSubAccountId, False)
            db.RunSQL(sql.ToString, db.glDBWorkB, False, "", False)

            For Each drow In db.glDBWorkB.Tables(0).Rows
                For Each li In lstLinkManage.Items
                    If Val(li.Value) = drow.Item("linkId") Then
                        li.Text = "* " & li.Text
                    Else
                        li.Text = li.Text
                    End If
                Next
            Next
        End Sub

        Protected Sub imgLinkManage_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgLinkManage.Click
            Select Case lstLinkManage.SelectedItem.Value
                Case -1
                    Response.Redirect("LinkManage.aspx?ret=0&action=add&cid=" & Trim(Session("ActiveContract")) & "&fid=" & lstLinkManage.SelectedValue, True)
                Case Else
                    Response.Redirect("LinkManage.aspx?ret=0&action=add&cid=" & Trim(Session("ActiveContract")) & "&fid=" & lstLinkManage.SelectedValue, True)
            End Select
        End Sub

        Protected Sub txtRenewalDate_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            CalcDate("RENEW")
        End Sub

        Protected Sub cmdCDUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCDUpdate.Click
            cmdCDUpdate.Enabled = False
            UpdateContract()
            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub cmdCDCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCDCancel.Click
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            Dim NavURL As String = "Home.aspx"

            db.DBOpen(fws, False)
            If Session("ContractAFS") = 1 Then
                ' retrieve the supplier id and return to supplier screen
                db.FWDb("R", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                If db.FWDbFlag Then
                    NavURL = "~/shared/supplier_details.aspx?&xcon=1&sid=" & db.FWDbFindVal("supplierId", 1)
                End If
            End If
            cLocks.RemoveLockItem(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.EmployeeID)
            db.DBClose()

            Session("CDAction") = Nothing
            Response.Redirect(NavURL, True)
        End Sub

        Protected Sub txtCancellationPeriod_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            CalcDate("CANCEL")
        End Sub

        Private Sub Notifications()
            Response.Redirect("Associations.aspx?frompage=contractdetails", True)
        End Sub

        Protected Sub txtReviewPeriod_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            If txtRenewalDate.Text <> "" Then
                CalcDate("REVIEW")
            End If
        End Sub

        Private Sub CalcDate(ByVal trigger As String)
            Dim tmpDate As String

            If txtRenewalDate.Text = "" Then
                Exit Sub
            End If

            Select Case trigger
                Case "CANCEL"
                    If txtCancellationPeriod.Text.Trim <> "" Then
                        If IsNumeric(txtCancellationPeriod.Text) And IsDate(txtRenewalDate.Text) Then
                            If lstCancellationPeriodType.SelectedItem.Value = PeriodType.Months Then
                                txtCancellationDate.Text = FormatDate(DateAdd(DateInterval.Month, 0 - Integer.Parse(txtCancellationPeriod.Text), CDate(txtRenewalDate.Text)).ToShortDateString)
                            Else
                                txtCancellationDate.Text = FormatDate(DateAdd(DateInterval.Day, 0 - Integer.Parse(txtCancellationPeriod.Text), CDate(txtRenewalDate.Text)).ToShortDateString)
                            End If
                        End If
                    End If

                    If txtCancellationDate.Text.Trim <> "" Then
                        tmpDate = txtCancellationDate.Text
                    Else
                        tmpDate = txtRenewalDate.Text
                    End If

                    If txtReviewPeriod.Text.Trim <> "" Then
                        If IsNumeric(txtReviewPeriod.Text) And IsDate(tmpDate) Then
                            If lstReviewPeriodType.SelectedItem.Value = PeriodType.Months Then
                                txtReviewDate.Text = FormatDate(DateAdd(DateInterval.Month, 0 - Integer.Parse(txtReviewPeriod.Text), CDate(tmpDate)).ToShortDateString)
                            Else
                                txtReviewDate.Text = FormatDate(DateAdd(DateInterval.Day, 0 - Integer.Parse(txtReviewPeriod.Text), CDate(tmpDate)).ToShortDateString)
                            End If
                        End If
                    End If

                    Page.SetFocus(lstCancellationPeriodType)

                Case "RENEW"
                    If txtCancellationPeriod.Text.Trim <> "" Then
                        ' recalculate the cancellation date
                        If IsNumeric(txtCancellationPeriod.Text) And IsDate(txtRenewalDate.Text) Then
                            If lstCancellationPeriodType.SelectedItem.Value = PeriodType.Months Then
                                txtCancellationDate.Text = FormatDate(DateAdd(DateInterval.Month, 0 - Integer.Parse(txtCancellationPeriod.Text), CDate(txtRenewalDate.Text)).ToShortDateString)
                            Else
                                txtCancellationDate.Text = FormatDate(DateAdd(DateInterval.Day, 0 - Integer.Parse(txtCancellationPeriod.Text), CDate(txtRenewalDate.Text)).ToShortDateString)
                            End If
                        End If
                    End If

                    If txtCancellationDate.Text.Trim <> "" Then
                        tmpDate = txtCancellationDate.Text
                    Else
                        tmpDate = txtRenewalDate.Text
                    End If

                    If txtReviewPeriod.Text.Trim <> "" Then
                        If txtCancellationPeriod.Text.Trim <> "" Then
                            If IsNumeric(txtReviewPeriod.Text) And IsDate(tmpDate) Then
                                If lstReviewPeriodType.SelectedItem.Value = PeriodType.Months Then
                                    txtReviewDate.Text = FormatDate(DateAdd(DateInterval.Month, 0 - Integer.Parse(txtReviewPeriod.Text), CDate(tmpDate)).ToShortDateString)
                                Else
                                    txtReviewDate.Text = FormatDate(DateAdd(DateInterval.Day, 0 - Integer.Parse(txtReviewPeriod.Text), CDate(tmpDate)).ToShortDateString)
                                End If
                            End If
                        End If
                    End If

                    Page.SetFocus(lstReviewPeriodType)
                Case "REVIEW"
                    If txtCancellationDate.Text.Trim <> "" Then
                        tmpDate = txtCancellationDate.Text.Trim
                    Else
                        tmpDate = txtRenewalDate.Text.Trim
                    End If

                    If txtReviewPeriod.Text.Trim <> "" Then
                        If IsNumeric(txtReviewPeriod.Text) And IsDate(tmpDate) Then
                            If lstReviewPeriodType.SelectedItem.Value = PeriodType.Months Then
                                txtReviewDate.Text = FormatDate(DateAdd(DateInterval.Month, 0 - Integer.Parse(txtReviewPeriod.Text), CDate(tmpDate)).ToShortDateString)
                            Else
                                txtReviewDate.Text = FormatDate(DateAdd(DateInterval.Day, 0 - Integer.Parse(txtReviewPeriod.Text), CDate(tmpDate)).ToShortDateString)
                            End If
                        End If
                    End If
                Case Else
            End Select

            ' must refresh the user fields from viewstate as postback update has taken place
            'Dim db As New cFWDBConnection
            'db.DBOpen(fws,false)
            'litUserFields.Text = GetUserDefinedFields(db, True, True)
            'db.DBClose()
            'db = Nothing
        End Sub

        <WebMethod()> _
        Public Shared Function DeleteContract(ByVal id As Integer) As String
            ' Check referential integrity. Cannot delete if it has contract products or financials etc.
            If id = 0 Then
                Return "ERROR! No Contract reference provided for deletion"
                Exit Function
            End If

            Dim db As New cFWDBConnection
            Dim appinfo As HttpApplication = CType(HttpContext.Current.ApplicationInstance, HttpApplication)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            db.DBOpen(fws, False)

            If SMRoutines.CheckRefIntegrity(db, "contract_details", id) = True Then
                Return "ERROR! : Cannot perform action as entity is currently assigned."
                Exit Function
            End If

            Dim sql As New System.Text.StringBuilder
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractDetails, curUser.CurrentSubAccountId)

            db.FWDb("R", "contract_details", "contractId", id, "", "", "", "", "", "", "", "", "", "")
            If db.FWDbFlag Then
                Dim ARec As New cAuditRecord

                ARec.Action = cFWAuditLog.AUDIT_DEL
                If db.FWDbFindVal("Contract Key", 1).Trim <> "" Then
                    ARec.ContractNumber = db.FWDbFindVal("contractKey", 1)
                Else
                    ARec.ContractNumber = db.FWDbFindVal("contractNumber", 1)
                End If
                ARec.DataElementDesc = "CONTRACT DELETION"
                ARec.ElementDesc = db.FWDbFindVal("contractDescription", 1)
                ARec.PostVal = ""
                ARec.PreVal = ""
                ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, id)

                db.FWDb("D", "contract_notification", "contractId", id, "", "", "", "", "", "", "", "", "", "")
                db.FWDb("D", "link_variations", "variationContractId", id, "", "", "", "", "", "", "", "", "", "")

                ' remove any contract links
                sql.Append("SELECT COUNT(*) AS [LinkCount],[link_matrix].[linkId] FROM [link_matrix] ")
                sql.Append("INNER JOIN [link_definitions] ON [link_matrix].[linkId] = [link_definitions].[linkId] ")
                sql.Append("WHERE [link_definitions].[isScheduleLink] = 1 AND [link_matrix].[linkId] IN (")
                sql.Append("SELECT [linkId] FROM [link_matrix] WHERE [contractId] = @Contract_Id) ")
                sql.Append("GROUP BY [link_matrix].[linkId]")
                db.AddDBParam("Contract Id", Trim(Str(id)), True)
                db.RunSQL(sql.ToString, db.glDBWorkL, False, "", False)

                If db.glNumRowsReturned > 0 Then
                    If db.GetFieldValue(db.glDBWorkL, "LinkCount", 0, 0) <= 2 Then
                        ' removal will leave single link chain, so delete definition for schedule link as well
                        db.FWDb("D", "link_definitions", "linkId", db.GetFieldValue(db.glDBWorkL, "linkId", 0, 0), "", "", "", "", "", "", "", "", "", "")
                        db.FWDb("D", "link_matrix", "linkId", db.GetFieldValue(db.glDBWorkL, "linkId", 0, 0), "", "", "", "", "", "", "", "", "", "")
                    End If
                End If

                db.FWDb("D", "link_matrix", "contractId", id, "", "", "", "", "", "", "", "", "", "")
                db.FWDb("D", "contract_audience", "contractId", id, "", "", "", "", "", "", "", "", "", "")
                db.FWDb("D", "contract_history", "contractId", id, "", "", "", "", "", "", "", "", "", "")

                db.SetFieldValue("lastContractId", DBNull.Value, "#", True)
                db.FWDb("A", "subAccountAccess", "lastContractId", id, "", "", "", "", "", "", "", "", "", "")

                db.FWDb("D", "contract_details", "contractId", id, "", "", "", "", "", "", "", "", "", "")

                appinfo.Session("ActiveContract") = 0

                db.DBClose()
                db = Nothing

                Return "Contract Deleted Successfully"
                'Response.Redirect("Home.aspx", True)
            Else
                Return "ERROR! Contract record not found. Deletion aborted."
            End If
        End Function

        Private Sub UpdateContract()
            Dim FWDb As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ARec As New cAuditRecord
            Dim sql As String
            Dim drow As DataRow
            Dim redir As String = String.Empty
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim supplierStr As String = params.SupplierPrimaryTitle

            Dim employees As New cEmployees(curUser.AccountID)
            Dim ufields As New cUserdefinedFields(curUser.AccountID)
            Dim tables As New cTables(curUser.AccountID)
            Dim fields As New cFields(curUser.AccountID)
            Dim cdTable As cTable = tables.getTableByName("contract_details")
            Dim cdUFields As List(Of cUserDefinedField) = ufields.getFieldsByTable(cdTable)

            FWDb.DBOpen(fws, False)
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractDetails, curUser.CurrentSubAccountId)

            'If CheckMandatoryUF(FWDb, AppAreas.CONTRACT_DETAILS) = True Then
            '    lblErrorString.Text = "A mandatory user field has not been specified"
            '    FWDb.DBClose()
            '    Exit Sub
            'End If

            ARec.ContractNumber = txtContractNumber.Text
            ARec.DataElementDesc = "CONTRACT DETAIL"
            ARec.ElementDesc = Left(Replace(txtDescription.Text, "'", "`"), 50)

            ' Check each field to see if a field has changed. Get original record
            If Session("ActiveContract") = 0 Then
                ' must be inserting a new contract
                ' only permit the addition if the supplier status permits it

                ARec.Action = cFWAuditLog.AUDIT_ADD
                ARec.PostVal = ""
                ARec.PreVal = ""

                FWDb.SetFieldValue("subAccountId", curUser.CurrentSubAccountId, "N", True)
                FWDb.SetFieldValue("contractKey", "xx/0/0", "S", False)
                FWDb.SetFieldValue("contractNumber", txtContractNumber.Text, "S", False)
                FWDb.SetFieldValue("supercedeContract", txtSupersedesContract.Text, "S", False)
                If txtDescription.Text.Length > 250 Then
                    FWDb.SetFieldValue("contractDescription", txtDescription.Text.Replace("'", "`").Substring(0, 250), "S", False)
                Else
                    FWDb.SetFieldValue("contractDescription", txtDescription.Text.Replace("'", "`"), "S", False)
                End If
                FWDb.SetFieldValue("supplierCode", txtSupplierCode.Text, "S", False)
                FWDb.SetFieldValue("scheduleNumber", txtSchedule.Text, "S", False)
                FWDb.SetFieldValue("supplierId", lstVendor.SelectedItem.Value, "N", False)
                If lstContractCategory.SelectedItem.Value <> "0" Then
                    FWDb.SetFieldValue("categoryId", lstContractCategory.SelectedItem.Value, "N", False)
                End If
                If lstContractType.SelectedItem.Value <> "0" Then
                    FWDb.SetFieldValue("contractTypeId", lstContractType.SelectedItem.Value, "N", False)
                End If
                If lstTermType.SelectedItem.Value <> "0" Then
                    FWDb.SetFieldValue("termTypeId", lstTermType.SelectedItem.Value, "N", False)
                End If
                FWDb.SetFieldValue("forecastTypeId", lstForecastType.SelectedItem.Value, "N", False)

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                    FWDb.SetFieldValue("contractValue", Double.Parse(txtContractValue.Text), "N", False)
                    If fws.glAutoUpdateCV = False Then
                        FWDb.SetFieldValue("annualContractValue", txtAnnualContractValue.Text, "N", False)
                    End If
                End If
                If lstCDCurrency.SelectedItem.Value <> "0" Then
                    FWDb.SetFieldValue("contractCurrency", lstCDCurrency.SelectedItem.Value, "N", False)
                End If
                If lstInvoiceFreq.SelectedItem.Value <> "0" Then
                    FWDb.SetFieldValue("invoiceFrequencyTypeId", lstInvoiceFreq.SelectedItem.Value, "N", False)
                End If
                If txtContractDate.Text <> "" Then
                    FWDb.SetFieldValue("startDate", txtContractDate.Text, "D", False)
                End If
                If txtRenewalDate.Text <> "" Then
                    FWDb.SetFieldValue("endDate", txtRenewalDate.Text, "D", False)
                End If
                FWDb.SetFieldValue("cancellationPenalty", IIf(chkPenaltyClause.Checked, "1", "0"), "N", False)

                If txtCancellationPeriod.Text <> "" Then
                    FWDb.SetFieldValue("noticePeriod", txtCancellationPeriod.Text, "N", False)
                Else
                    FWDb.SetFieldValue("noticePeriod", 0, "N", False)
                End If

                FWDb.SetFieldValue("noticePeriodType", lstCancellationPeriodType.SelectedItem.Value, "N", False)
                If txtCancellationDate.Text <> "" Then
                    FWDb.SetFieldValue("cancellationDate", txtCancellationDate.Text, "D", False)
                End If
                If txtReviewPeriod.Text <> "" Then
                    FWDb.SetFieldValue("reviewPeriod", txtReviewPeriod.Text, "N", False)
                Else
                    FWDb.SetFieldValue("reviewPeriod", 0, "N", False)
                End If

                FWDb.SetFieldValue("reviewPeriodType", lstReviewPeriodType.SelectedItem.Value, "N", False)
                If txtReviewDate.Text <> "" Then
                    FWDb.SetFieldValue("reviewDate", txtReviewDate.Text, "D", False)
                End If

                If txtReviewCompleteDate.Text <> "" Then
                    FWDb.SetFieldValue("reviewComplete", txtReviewCompleteDate.Text, "D", False)
                End If

                FWDb.SetFieldValue("maintenanceType", lstMaintenanceType.SelectedItem.Value, "N", False)
                FWDb.SetFieldValue("maintenanceInflatorX", lstMaintParam1.SelectedItem.Value, "N", False)
                If SMRoutines.hasExtraPercent(fws, lstMaintParam1.SelectedItem.Value) Or lstMaintParam2.SelectedItem.Text = "%" Then
                    If txtMaintParam1.Text <> "" Then
                        FWDb.SetFieldValue("maintenancePercentX", txtMaintParam1.Text, "N", False)
                    Else
                        FWDb.SetFieldValue("maintenancePercentX", "0", "N", False)
                    End If
                End If

                FWDb.SetFieldValue("maintenanceInflatorY", lstMaintParam2.SelectedItem.Value, "N", False)
                If SMRoutines.hasExtraPercent(fws, lstMaintParam2.SelectedItem.Value) Or lstMaintParam2.SelectedItem.Text = "%" Then
                    If txtMaintParam2.Text <> "" Then
                        FWDb.SetFieldValue("maintenancePercentY", txtMaintParam2.Text, "N", False)
                    Else
                        FWDb.SetFieldValue("maintenancePercentY", "0", "N", False)
                    End If
                End If

                If lstContractStatus.SelectedItem.Value <> "0" Then
                    FWDb.SetFieldValue("contractStatusId", lstContractStatus.SelectedItem.Value, "N", False)

                    FWDb.FWDb("R2", "codes_contractstatus", "statusId", lstContractStatus.SelectedItem.Value, "", "", "", "", "", "", "", "", "", "")
                    If FWDb.FWDb2Flag = True Then
                        If FWDb.FWDbFindVal("IsArchive", 2) = "0" Then
                            FWDb.SetFieldValue("Archived", "N", "S", False)
                        Else
                            FWDb.SetFieldValue("Archived", "Y", "S", False)
                            FWDb.SetFieldValue("archiveDate", Today(), "D", False)
                        End If
                    Else
                        FWDb.SetFieldValue("Archived", "N", "S", False)
                    End If
                Else
                    FWDb.SetFieldValue("Archived", "N", "S", False)
                End If

                If lstContractOwner.SelectedItem.Value = 0 Then
                    FWDb.SetFieldValue("contractOwner", DBNull.Value, "#", False)
                Else
                    FWDb.SetFieldValue("contractOwner", lstContractOwner.SelectedItem.Value, "N", False)
                End If


                '' Update any user defined fields
                'Dim newrec As SortedList(Of Integer, Object) = ufields.getItemsFromPanel(phCDUFields, cdTable)

                'For Each uf As cUserDefinedField In cdUFields
                '    Select Case uf.fieldtype
                '        Case FieldType.AutoCompleteTextbox, FieldType.Hyperlink, FieldType.LargeText, FieldType.RelationshipTextbox, FieldType.Text
                '            FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "S", False)

                '        Case FieldType.Currency

                '            'Only save the amount if the user is allowed to view the financials
                '            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                '                FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "N", False)
                '            End If
                '        Case FieldType.Integer, FieldType.List, FieldType.Number, FieldType.Relationship
                '            FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "N", False)

                '        Case FieldType.DateTime
                '            FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "D", False)

                '        Case FieldType.TickBox, FieldType.RunWorkflow
                '            FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "X", False)
                '        Case Else

                '    End Select
                'Next

                FWDb.SetFieldValue("lastChangedBy", curUser.Employee.Forename & " " & curUser.Employee.Surname, "S", False)
                FWDb.SetFieldValue("lastChangeDate", Now(), "D", False)
                If cDef.DBVERSION >= 18 Then
                    FWDb.SetFieldValue("createdOn", Now(), "D", False)
                End If
                ' write record to the database
                FWDb.FWDb("W", "contract_details", "", "", "", "", "", "", "", "", "", "", "", "")

                If FWDb.glIdentity <> 0 Then
                    FWDb.FWDb("R2", "contract_details", "contractId", FWDb.glIdentity, "", "", "", "", "", "", "", "", "", "")
                Else
                    FWDb.FWDb("R2", "contract_details", "subAccountId", curUser.CurrentSubAccountId, "contractDescription", Replace(txtDescription.Text, "'", "`"), "supplierId", lstVendor.SelectedItem.Value, "", "", "", "", "", "")
                End If

                If params.ContractNumGen Then
                    subaccs = New cAccountSubAccounts(curUser.AccountID) ' need to force refresh
                    params = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
                    Dim curSeq As Integer = params.ContractNumSeq
                    Dim cnum As Integer = 0
                    Integer.TryParse(txtContractNumber.Text.Trim, cnum)

                    If curSeq <> cnum Then
                        ' the sequence must have been updated by another, so use new current value
                        params.ContractNumSeq = cnum
                        FWDb.SetFieldValue("contractNumber", cnum, "N", True)
                        FWDb.FWDb("A", "contract_details", "contractId", FWDb.glIdentity, "", "", "", "", "", "", "", "", "", "")
                    End If

                    Dim accProperties As New cAccountSubAccounts(curUser.AccountID)

                    params.ContractNumSeq += 1
                    accProperties.SaveAccountProperties(params, curUser.EmployeeID, Nothing)
                End If

                If FWDb.FWDb2Flag = True Then
                    Dim newKey As String
                    newKey = params.ContractKey & "/" & Trim(FWDb.FWDbFindVal("contractId", 2)) & "/" & Trim(FWDb.FWDbFindVal("supplierId", 2))

                    Session("ActiveContract") = Val(FWDb.FWDbFindVal("contractId", 2))
                    FWDb.SetFieldValue("contractKey", newKey, "S", True)
                    FWDb.FWDb("A", "contract_details", "contractId", FWDb.FWDbFindVal("contractId", 2), "", "", "", "", "", "", "", "", "", "")

                    ARec.ContractNumber = newKey
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, FWDb.glIdentity)

                    ' create contract history entry
                    ARec.PostVal = "Contract Created"
                    ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.ContractDetail, Session("ActiveContract"), ARec)

                    cmdNotes.Enabled = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractNotes, False)

                    ' update the notification selections
                    'For x = 0 To chkNotifyList.Items.Count - 1
                    '    If chkNotifyList.Items(x).Selected = True Then
                    '        FWDb.SetFieldValue("Staff Id", chkNotifyList.Items(x).Value, "N", True)
                    '        FWDb.SetFieldValue("Contract Id", Session("ActiveContract"), "N", False)
                    '        FWDb.SetFieldValue("Location Id", userinfo.ActiveLocation, "N", False)
                    '        FWDb.FWDb("W", "Contract - Notification")
                    '    End If
                    'Next

                    redir = "ContractSummary.aspx?id=" & Trim(Str(Session("ActiveContract")))
                Else
                    redir = "Home.aspx"
                End If
                'redir = "FWMain.aspx?viewname=current"
            Else
                ' updating an existing record
                Dim firstchange As Boolean = True
                Dim updateVariationsInvFreq As Boolean = False
                Dim NewContractStatus As Integer = -1
                Dim tmpval As String
                Dim ntmpval As Double
                Dim arrayAuditRecs As New System.Collections.Generic.List(Of cAuditRecord)
                Dim contrID As String


                ' get the original record for comparison
                FWDb.FWDb("R2", "contract_details", "subAccountId", curUser.CurrentSubAccountId, "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "")
                If FWDb.FWDb2Flag = True Then
                    tmpval = FWDb.FWDbFindVal("contractKey", 2)
                    If tmpval = "" Then
                        tmpval = FWDb.FWDbFindVal("contractNumber", 2)
                    End If
                    contrID = tmpval
                    ARec = New cAuditRecord
                    ARec.ContractNumber = contrID
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE

                    If txtContractNumber.Text <> FWDb.FWDbFindVal("contractNumber", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblContractNumber.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("contractNumber", 2)
                        ARec.PostVal = txtContractNumber.Text

                        FWDb.SetFieldValue("contractNumber", txtContractNumber.Text, "S", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If txtSupersedesContract.Text <> FWDb.FWDbFindVal("supercedeContract", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblSupersedesContract.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("supercedeContract", 2)
                        ARec.PostVal = txtSupersedesContract.Text

                        FWDb.SetFieldValue("supercedeContract", txtSupersedesContract.Text, "S", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    'tmpval = Replace(txtDescription.Text, "'", "`")
                    If txtDescription.Text.Trim <> FWDb.FWDbFindVal("contractDescription", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblDescription.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("contractDescription", 2)
                        ARec.PostVal = txtDescription.Text.Trim

                        FWDb.SetFieldValue("contractDescription", txtDescription.Text.Trim, "S", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If txtSchedule.Text <> FWDb.FWDbFindVal("scheduleNumber", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblSchedule.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("scheduleNumber", 2)
                        ARec.PostVal = txtSchedule.Text

                        FWDb.SetFieldValue("scheduleNumber", txtSchedule.Text, "S", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstVendor.SelectedItem.Value <> FWDb.FWDbFindVal("supplierId", 2) Then
                        If ValidateContractProducts(lstVendor.SelectedItem.Value) = True Then
                            ARec = New cAuditRecord
                            ARec.ContractNumber = contrID
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ElementDesc = params.SupplierPrimaryTitle

                            ARec.PreVal = lstVendor.Items.FindByValue(FWDb.FWDbFindVal("supplierId", 2)).Text
                            If lstVendor.SelectedItem.Text.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                                ARec.PostVal = lstVendor.SelectedItem.Text.Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                            Else
                                ARec.PostVal = lstVendor.SelectedItem.Text
                            End If

                            FWDb.SetFieldValue("supplierId", lstVendor.SelectedItem.Value, "N", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        Else
                            ' cannot modify vendor because contract products associated
                            ' are not available from the newly selected vendor.
                            lblErrorString.Text = "ERROR! " & supplierStr & " - Product Association invalid"
                            FWDb.DBClose()
                            FWDb = Nothing
                            Exit Sub
                        End If
                    End If

                    If lstContractCategory.SelectedItem.Value <> FWDb.FWDbFindVal("categoryId", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblCategory.Text)
                        If FWDb.FWDbFindVal("categoryId", 2) <> "" Then
                            ARec.PreVal = lstContractCategory.Items.FindByValue(FWDb.FWDbFindVal("categoryId", 2)).Text
                        Else
                            ARec.PreVal = ""
                        End If
                        ARec.PostVal = lstContractCategory.SelectedItem.Text

                        If lstContractCategory.SelectedItem.Value = "0" Then
                            FWDb.SetFieldValue("categoryId", DBNull.Value, "#", firstchange)
                        Else
                            FWDb.SetFieldValue("categoryId", lstContractCategory.SelectedItem.Value, "N", firstchange)
                        End If

                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstContractType.SelectedItem.Value <> FWDb.FWDbFindVal("contractTypeId", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblContractType.Text)

                        If FWDb.FWDbFindVal("contractTypeId", 2) <> "" Then
                            ARec.PreVal = lstContractType.Items.FindByValue(FWDb.FWDbFindVal("contractTypeId", 2)).Text
                        End If
                        ARec.PostVal = lstContractType.SelectedItem.Text

                        If lstContractType.SelectedItem.Value = "0" Then
                            FWDb.SetFieldValue("contractTypeId", DBNull.Value, "#", firstchange)
                        Else
                            FWDb.SetFieldValue("contractTypeId", lstContractType.SelectedItem.Value, "N", firstchange)
                        End If

                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstForecastType.SelectedItem.Value <> FWDb.FWDbFindVal("forecastTypeId", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblForecastType.Text)
                        If FWDb.FWDbFindVal("forecastTypeId", 2) = "" Then
                            ARec.PreVal = ""
                        Else
                            ARec.PreVal = lstForecastType.Items.FindByValue(FWDb.FWDbFindVal("forecastTypeId", 2)).Text
                        End If

                        ARec.PostVal = lstForecastType.SelectedItem.Text

                        FWDb.SetFieldValue("forecastTypeId", lstForecastType.SelectedItem.Value, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstTermType.SelectedItem.Value <> FWDb.FWDbFindVal("termTypeId", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblTermType.Text)
                        If FWDb.FWDbFindVal("termTypeId", 2) = "" Then
                            ARec.PreVal = ""
                        Else
                            ARec.PreVal = lstTermType.Items.FindByValue(FWDb.FWDbFindVal("termTypeId", 2)).Text
                        End If

                        ARec.PostVal = lstTermType.SelectedItem.Text

                        If lstTermType.SelectedItem.Value = "0" Then
                            FWDb.SetFieldValue("termTypeId", DBNull.Value, "#", firstchange)
                        Else
                            FWDb.SetFieldValue("termTypeId", lstTermType.SelectedItem.Value, "N", firstchange)
                        End If
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If txtSupplierCode.Text <> FWDb.FWDbFindVal("supplierCode", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblSupplierCode.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("supplierCode", 2)
                        ARec.PostVal = txtSupplierCode.Text

                        FWDb.SetFieldValue("supplierCode", txtSupplierCode.Text, "S", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    ' if no permissions to view financial, then don't want to overwrite hidden contract value
                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE

                        ntmpval = Val(Replace(txtContractValue.Text, ",", ""))
                        If ntmpval <> Val(FWDb.FWDbFindVal("contractValue", 2)) Then
                            ARec.ElementDesc = UCase(lblContractValue.Text)
                            ARec.PreVal = FWDb.FWDbFindVal("contractValue", 2)
                            ARec.PostVal = Replace(txtContractValue.Text, ",", "")

                            FWDb.SetFieldValue("contractValue", Replace(txtContractValue.Text, ",", ""), "N", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        End If

                        If fws.glAutoUpdateCV = False Then
                            ntmpval = Val(Replace(txtAnnualContractValue.Text, ",", ""))
                            If ntmpval <> Val(FWDb.FWDbFindVal("annualContractValue", 2)) Then
                                ARec.ElementDesc = UCase(lblAnnualContractValue.Text)
                                ARec.PreVal = FWDb.FWDbFindVal("annualContractValue", 2)
                                ARec.PostVal = Replace(txtAnnualContractValue.Text, ",", "")

                                FWDb.SetFieldValue("annualContractValue", Replace(txtAnnualContractValue.Text, ",", ""), "N", firstchange)
                                arrayAuditRecs.Add(ARec)
                                firstchange = False
                            End If
                        End If
                    End If

                    If lstCDCurrency.SelectedItem.Value <> FWDb.FWDbFindVal("contractCurrency", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblCDCurrency.Text)
                        If FWDb.FWDbFindVal("contractCurrency", 2) = "" Then
                            ARec.PreVal = ""
                        Else
                            ARec.PreVal = lstCDCurrency.Items.FindByValue(FWDb.FWDbFindVal("contractCurrency", 2)).Text
                        End If

                        ARec.PostVal = lstCDCurrency.SelectedItem.Text

                        If lstCDCurrency.SelectedItem.Value = "0" Then
                            FWDb.SetFieldValue("contractCurrency", DBNull.Value, "#", firstchange)
                        Else
                            FWDb.SetFieldValue("contractCurrency", lstCDCurrency.SelectedItem.Value, "N", firstchange)
                        End If
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstInvoiceFreq.SelectedItem.Value <> FWDb.FWDbFindVal("invoiceFrequencyTypeId", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblInvoiceFreq.Text)
                        If FWDb.FWDbFindVal("invoiceFrequencyTypeId", 2) = "" Then
                            ARec.PreVal = ""
                        Else
                            ARec.PreVal = lstInvoiceFreq.Items.FindByValue(FWDb.FWDbFindVal("invoiceFrequencyTypeId", 2)).Text
                        End If
                        ARec.PostVal = lstInvoiceFreq.SelectedItem.Text

                        If lstInvoiceFreq.SelectedItem.Value = "0" Then
                            FWDb.SetFieldValue("invoiceFrequencyTypeId", DBNull.Value, "#", firstchange)
                        Else
                            FWDb.SetFieldValue("invoiceFrequencyTypeId", lstInvoiceFreq.SelectedItem.Value, "N", firstchange)
                        End If
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                        updateVariationsInvFreq = True
                    End If

                    tmpval = FWDb.FWDbFindVal("startDate", 2)

                    If IsDate(tmpval) = True Then
                        tmpval = Format(Date.Parse(tmpval), cDef.DATE_FORMAT)
                    End If

                    If txtContractDate.Text <> tmpval Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblContractDate.Text)
                        ARec.PreVal = tmpval
                        ARec.PostVal = FormatDate(txtContractDate.Text)

                        If IsDate(txtContractDate.Text) Then
                            FWDb.SetFieldValue("startDate", txtContractDate.Text, "D", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        Else
                            FWDb.SetFieldValue("startDate", DBNull.Value, "#", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        End If
                    End If

                    tmpval = FWDb.FWDbFindVal("endDate", 2)

                    If IsDate(tmpval) = True Then
                        tmpval = Format(Date.Parse(tmpval), cDef.DATE_FORMAT)
                    End If

                    If txtRenewalDate.Text <> tmpval Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblRenewalDate.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("endDate", 2)
                        ARec.PostVal = FormatDate(txtRenewalDate.Text)

                        If IsDate(txtRenewalDate.Text) Then
                            FWDb.SetFieldValue("endDate", txtRenewalDate.Text, "D", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        Else
                            FWDb.SetFieldValue("endDate", DBNull.Value, "#", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        End If
                    End If

                    If txtCancellationPeriod.Text.Trim = "" Then
                        txtCancellationPeriod.Text = "0"
                    End If

                    If txtCancellationPeriod.Text <> FWDb.FWDbFindVal("noticePeriod", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblCancellationPeriod.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("noticePeriod", 2)
                        ARec.PostVal = txtCancellationPeriod.Text

                        FWDb.SetFieldValue("noticePeriod", txtCancellationPeriod.Text, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstCancellationPeriodType.SelectedItem.Value <> FWDb.FWDbFindVal("noticePeriodType", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = "NOTICE PERIOD TYPE"
                        ARec.PreVal = IIf(FWDb.FWDbFindVal("noticePeriodType", 2) = PeriodType.Months, "mths", "days")
                        ARec.PostVal = lstCancellationPeriodType.SelectedItem.Text

                        FWDb.SetFieldValue("noticePeriodType", lstCancellationPeriodType.SelectedItem.Value, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    tmpval = FWDb.FWDbFindVal("cancellationDate", 2)
                    If IsDate(tmpval) = True Then
                        tmpval = Format(Date.Parse(tmpval), cDef.DATE_FORMAT)
                    End If

                    If txtCancellationDate.Text <> tmpval Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblCancellationDate.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("cancellationDate", 2)
                        ARec.PostVal = FormatDate(txtCancellationDate.Text)

                        If IsDate(txtCancellationDate.Text) = True Then
                            If Format(Date.Parse(txtCancellationDate.Text), cDef.DATE_FORMAT) <> tmpval Then
                                FWDb.SetFieldValue("cancellationDate", txtCancellationDate.Text, "D", firstchange)
                                arrayAuditRecs.Add(ARec)
                                firstchange = False
                            End If
                        Else
                            FWDb.SetFieldValue("cancellationDate", DBNull.Value, "#", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        End If
                    End If

                    If IIf(chkPenaltyClause.Checked, "1", "0") <> FWDb.FWDbFindVal("cancellationPenalty", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(chkPenaltyClause.Text)
                        ARec.PreVal = IIf(FWDb.FWDbFindVal("cancellationPenalty", 2) = "1", "YES", "NO")
                        ARec.PostVal = IIf(chkPenaltyClause.Checked, "YES", "NO")

                        FWDb.SetFieldValue("cancellationPenalty", IIf(chkPenaltyClause.Checked, "1", "0"), "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If txtReviewPeriod.Text.Trim = "" Then
                        txtReviewPeriod.Text = "0"
                    End If

                    If txtReviewPeriod.Text <> FWDb.FWDbFindVal("reviewPeriod", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblReviewPeriod.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("reviewPeriod", 2)
                        ARec.PostVal = txtReviewPeriod.Text

                        FWDb.SetFieldValue("reviewPeriod", txtReviewPeriod.Text, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstReviewPeriodType.SelectedItem.Value <> FWDb.FWDbFindVal("reviewPeriodType", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = "REVIEW PERIOD TYPE"
                        ARec.PreVal = IIf(FWDb.FWDbFindVal("reviewPeriodType", 2) = PeriodType.Months, "mths", "days")
                        ARec.PostVal = lstReviewPeriodType.SelectedItem.Text

                        FWDb.SetFieldValue("reviewPeriodType", lstReviewPeriodType.SelectedItem.Value, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    tmpval = FWDb.FWDbFindVal("reviewDate", 2)
                    If IsDate(tmpval) = True Then
                        tmpval = Format(Date.Parse(tmpval), cDef.DATE_FORMAT)
                    End If

                    If txtReviewDate.Text <> tmpval Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblReviewDate.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("reviewDate", 2)
                        ARec.PostVal = FormatDate(txtReviewDate.Text)

                        If IsDate(txtReviewDate.Text) = True Then
                            FWDb.SetFieldValue("reviewDate", txtReviewDate.Text, "D", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        Else
                            FWDb.SetFieldValue("reviewDate", DBNull.Value, "#", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        End If
                    End If

                    tmpval = FWDb.FWDbFindVal("reviewComplete", 2)
                    If IsDate(tmpval) = True Then
                        tmpval = Format(CDate(tmpval), cDef.DATE_FORMAT)
                    End If

                    If txtReviewCompleteDate.Text.Trim <> tmpval Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblReviewCompleteDate.Text)
                        ARec.PreVal = FWDb.FWDbFindVal("reviewComplete", 2)
                        ARec.PostVal = FormatDate(txtReviewCompleteDate.Text)

                        If IsDate(txtReviewCompleteDate.Text) Then
                            FWDb.SetFieldValue("reviewComplete", txtReviewCompleteDate.Text, "D", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        Else
                            FWDb.SetFieldValue("reviewComplete", DBNull.Value, "#", firstchange)
                            arrayAuditRecs.Add(ARec)
                            firstchange = False
                        End If
                    End If

                    If lstContractStatus.SelectedItem.Value <> FWDb.FWDbFindVal("contractStatusId", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblContractStatus.Text)

                        If FWDb.FWDbFindVal("contractStatusId", 2) <> "" Then
                            ARec.PreVal = lstContractStatus.Items.FindByValue(FWDb.FWDbFindVal("contractStatusId", 2)).Text
                        End If

                        ARec.PostVal = lstContractStatus.SelectedItem.Text

                        If lstContractStatus.SelectedItem.Value = "0" Then
                            FWDb.SetFieldValue("contractStatusId", DBNull.Value, "#", firstchange)
                        Else
                            FWDb.SetFieldValue("contractStatusId", lstContractStatus.SelectedItem.Value, "N", firstchange)
                        End If
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                        NewContractStatus = lstContractStatus.SelectedItem.Value
                    End If

                    If lstMaintenanceType.SelectedItem.Value <> FWDb.FWDbFindVal("maintenanceType", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblMaintenanceType.Text)
                        If FWDb.FWDbFindVal("maintenanceType", 2) = "" Then
                            ARec.PreVal = ""
                        Else
                            ARec.PreVal = lstMaintenanceType.Items.FindByValue(FWDb.FWDbFindVal("maintenanceType", 2)).Text
                        End If

                        ARec.PostVal = lstMaintenanceType.SelectedItem.Text

                        FWDb.SetFieldValue("maintenanceType", lstMaintenanceType.SelectedItem.Value, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstContractOwner.SelectedItem.Value <> FWDb.FWDbFindVal("contractOwner", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = UCase(lblContractOwner.Text)
                        If FWDb.FWDbFindVal("contractOwner", 2) = "" Then
                            ARec.PreVal = ""
                        Else
                            ARec.PreVal = lstContractOwner.Items.FindByValue(FWDb.FWDbFindVal("contractOwner", 2)).Text
                        End If
                        ARec.PostVal = lstContractOwner.SelectedItem.Text

                        If lstContractOwner.SelectedItem.Value = "0" Then
                            FWDb.SetFieldValue("contractOwner", DBNull.Value, "#", firstchange)
                        Else
                            FWDb.SetFieldValue("contractOwner", lstContractOwner.SelectedItem.Value, "N", firstchange)
                        End If

                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstMaintParam1.SelectedItem.Value <> FWDb.FWDbFindVal("maintenanceInflatorX", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = "MAINT TYPE X"
                        If FWDb.FWDbFindVal("maintenanceInflatorX", 2) = "" Then
                            ARec.PreVal = ""
                        Else
                            ARec.PreVal = lstMaintParam1.Items.FindByValue(FWDb.FWDbFindVal("maintenanceInflatorX", 2)).Text
                        End If

                        ARec.PostVal = lstMaintParam1.SelectedItem.Text

                        FWDb.SetFieldValue("maintenanceInflatorX", lstMaintParam1.SelectedItem.Value, "N", firstchange)
                        If SMRoutines.hasExtraPercent(fws, lstMaintParam1.SelectedItem.Value) = False Or lstMaintParam1.SelectedItem.Text = "%" Then
                            txtMaintParam1.Text = "0"
                        End If
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If lstMaintParam2.SelectedItem.Value <> FWDb.FWDbFindVal("maintenanceInflatorY", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = "MAINT TYPE Y"
                        If FWDb.FWDbFindVal("maintenanceInflatorY", 2) = "" Then
                            ARec.PreVal = ""
                        Else
                            ARec.PreVal = lstMaintParam2.Items.FindByValue(FWDb.FWDbFindVal("maintenanceInflatorY", 2)).Text
                        End If

                        ARec.PostVal = lstMaintParam2.SelectedItem.Text

                        FWDb.SetFieldValue("maintenanceInflatorY", lstMaintParam2.SelectedItem.Value, "N", firstchange)
                        If SMRoutines.hasExtraPercent(fws, lstMaintParam2.SelectedItem.Value) = False Or lstMaintParam2.SelectedItem.Text = "%" Then
                            txtMaintParam2.Text = "0"
                        End If
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If txtMaintParam1.Text <> FWDb.FWDbFindVal("maintenancePercentX", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = "MAINT % X"
                        ARec.PreVal = FWDb.FWDbFindVal("maintenancePercentX", 2)
                        ARec.PostVal = txtMaintParam1.Text

                        FWDb.SetFieldValue("maintenancePercentX", txtMaintParam1.Text, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    If txtMaintParam2.Text <> FWDb.FWDbFindVal("maintenancePercentY", 2) Then
                        ARec = New cAuditRecord
                        ARec.ContractNumber = contrID
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ElementDesc = "MAINT % Y"
                        ARec.PreVal = FWDb.FWDbFindVal("maintenancePercentY", 2)
                        ARec.PostVal = txtMaintParam2.Text

                        FWDb.SetFieldValue("maintenancePercentY", txtMaintParam2.Text, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If

                    ' update the user defined fields
                    'Dim oldrec As SortedList(Of Integer, Object) = CType(ViewState("record"), SortedList(Of Integer, Object))
                    'Dim newrec As SortedList(Of Integer, Object) = ufields.getItemsFromPanel(phCDUFields, cdTable)

                    'For Each uf As cUserDefinedField In cdUFields
                    '    ARec.ElementDesc = uf.label.ToUpper
                    '    ARec.PreVal = CStr(oldrec(uf.userdefineid))
                    '    ARec.PostVal = CStr(newrec(uf.userdefineid))

                    '    Select Case uf.fieldtype
                    '        Case FieldType.AutoCompleteTextbox, FieldType.Hyperlink, FieldType.LargeText, FieldType.RelationshipTextbox, FieldType.Text
                    '            If oldrec(uf.userdefineid) <> newrec(uf.userdefineid) Then
                    '                FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "S", firstchange)
                    '                arrayAuditRecs.Add(ARec)

                    '                firstchange = False
                    '            End If
                    '        Case FieldType.Currency
                    '            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                    '                If oldrec(uf.userdefineid) <> newrec(uf.userdefineid) Then
                    '                    FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "N", firstchange)
                    '                    arrayAuditRecs.Add(ARec)

                    '                    firstchange = False
                    '                End If
                    '            End If
                    '        Case FieldType.Integer, FieldType.List, FieldType.Number, FieldType.Relationship
                    '            If oldrec(uf.userdefineid) <> newrec(uf.userdefineid) Then
                    '                FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "N", firstchange)
                    '                arrayAuditRecs.Add(ARec)

                    '                firstchange = False
                    '            End If

                    '        Case FieldType.DateTime
                    '            If oldrec(uf.userdefineid) <> newrec(uf.userdefineid) Then
                    '                FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "D", firstchange)
                    '                arrayAuditRecs.Add(ARec)

                    '                firstchange = False
                    '            End If

                    '        Case FieldType.TickBox, FieldType.RunWorkflow
                    '            If oldrec(uf.userdefineid) <> newrec(uf.userdefineid) Then
                    '                FWDb.SetFieldValue("udf" & uf.userdefineid.ToString, newrec(uf.userdefineid), "X", firstchange)
                    '                arrayAuditRecs.Add(ARec)

                    '                firstchange = False
                    '            End If
                    '        Case Else

                    '    End Select
                    'Next

                    If firstchange = False Then
                        If Trim(txtUniqueKeyValue.Text) = "" Then
                            FWDb.SetFieldValue("contractKey", params.ContractKey & "/" & Trim(Session("ActiveContract")) & "/" & Trim(lstVendor.SelectedItem.Value), "S", False)
                        End If

                        FWDb.SetFieldValue("lastChangedBy", curUser.Employee.Forename & " " & curUser.Employee.Surname, "S", False)
                        FWDb.SetFieldValue("lastChangeDate", Now(), "D", False)

                        If NewContractStatus <> -1 Then
                            ' to simplify reporting, keep the Y/N of the old archive flag - update according to contract status
                            FWDb.FWDb("R2", "codes_contractstatus", "statusId", NewContractStatus, "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDb2Flag = True Then
                                If FWDb.FWDbFindVal("IsArchive", 2) = "0" Then
                                Else
                                    FWDb.SetFieldValue("Archived", "Y", "S", False)
                                    FWDb.SetFieldValue("archiveDate", Today(), "D", False)
                                End If
                            End If
                        End If

                        FWDb.FWDb("A", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                        ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.ContractDetail, Session("ActiveContract"), arrayAuditRecs)
                        For auditLoop As Integer = 0 To arrayAuditRecs.Count - 1
                            ALog.AddAuditRec(arrayAuditRecs(auditLoop), True)
                            ALog.CommitAuditLog(curUser.Employee, Session("ActiveContract"))
                        Next

                        If NewContractStatus <> -1 Then
                            FWDb.FWDb("R2", "codes_contractstatus", "statusId", NewContractStatus, "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDb2Flag Then
                                If FWDb.FWDbFindVal("IsArchive", 2) = "1" Then
                                    ' archive any variations
                                    sql = "SELECT [contractId],[contractKey] FROM [contract_details] WHERE [contractId] IN (SELECT [variationContractId] FROM [link_variations] WHERE [primaryContractId] = @contractId)"
                                    FWDb.AddDBParam("contractId", Session("ActiveContract"), True)
                                    FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "", False)

                                    For Each drow In FWDb.glDBWorkA.Tables(0).Rows
                                        ' archive all variations and log in contract history and audit log
                                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                                        ARec.ContractNumber = drow.Item("contractKey")
                                        ARec.DataElementDesc = "CONTRACT VARIATION"
                                        ARec.PreVal = "LIVE"
                                        ARec.PostVal = "ARCHIVED"
                                        ALog.AddAuditRec(ARec, True)
                                        ALog.CommitAuditLog(curUser.Employee, Session("ActiveContract"))

                                        FWDb.SetFieldValue("contractStatusId", NewContractStatus, "N", True)
                                        FWDb.SetFieldValue("Archived", "Y", "S", False)
                                        FWDb.FWDb("A", "contract_details", "contractId", drow("contractId"), "", "", "", "", "", "", "", "", "", "")
                                    Next
                                Else
                                    ' blank the Archive Date
                                    FWDb.AddDBParam("conId", Session("ActiveContract"), True)
                                    FWDb.ExecuteSQL("UPDATE contract_details SET [archiveDate] = NULL, [Archived] = 'N' WHERE [contractId] = @conId")
                                End If
                            End If
                        End If

                        If updateVariationsInvFreq Then
                            sql = "SELECT [contractId] FROM [contract_details] WHERE [contractId] IN (SELECT [variationContractId] FROM [link_variations] WHERE [primaryContractId] = @contractId)"
                            FWDb.AddDBParam("contractId", Session("ActiveContract"), True)
                            FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "", False)

                            If FWDb.glNumRowsReturned > 0 Then
                                Dim comma As String = ""
                                Dim csvIds As String = ""

                                For Each drow In FWDb.glDBWorkA.Tables(0).Rows
                                    csvIds += comma & CStr(drow("contractId"))
                                    comma = ","
                                Next

                                sql = "UPDATE contract_details SET [invoiceFrequencyTypeId] = @InvFreq WHERE [contractId] IN (" & csvIds & ")"
                                If lstInvoiceFreq.SelectedItem.Value = "0" Then
                                    FWDb.AddDBParam("InvFreq", DBNull.Value, True)
                                Else
                                    FWDb.AddDBParam("InvFreq", lstInvoiceFreq.SelectedItem.Value, True)
                                End If
                                FWDb.ExecuteSQL(sql)
                            End If
                        End If
                    End If
                End If

                arrayAuditRecs = Nothing
            End If

            ' update ungrouped udfs
            Dim udTable As cTable = tables.getTableById(cdTable.UserDefinedTableID)
            Dim newrec As SortedList(Of Integer, Object) = ufields.getItemsFromPanel(phCDUFields, udTable, groupType:=GroupingOutputType.UnGroupedOnly)
            Dim record As String = params.ContractKey & "/" & Trim(Session("ActiveContract")) & "/" & Trim(lstVendor.SelectedItem.Value)

            ufields.SaveValues(udTable, Session("ActiveContract"), newrec, tables, fields, curUser, groupType:=GroupingOutputType.UnGroupedOnly, elementId:=SpendManagementElement.ContractDetails, record:=record)

            FWDb.DBClose()
            FWDb = Nothing

            Session("CDAction") = Nothing

            If redir = String.Empty Then
                redir = "ContractSummary.aspx?id=" & Trim(Session("ActiveContract"))
            End If

            Response.Redirect(redir, True)
        End Sub

        Private Function ValidateContractProducts(ByVal SupplierId As Integer) As Boolean
            Dim db As New cFWDBConnection
            Dim sql As String
            Dim curRow, curVendorRow As DataRow
            Dim curConProdId As Integer
            Dim result, match As Boolean
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As cAccountSubAccounts = New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)

            db.DBOpen(fws, False)

            sql = "SELECT [productId] FROM [contract_productdetails] WHERE [contractId] = @contractId"
            db.AddDBParam("contractId", Trim(Session("ActiveContract")), True)
            db.RunSQL(sql, db.glDBWorkA, False, "", False)

            sql = "SELECT [productId] FROM [product_suppliers] WHERE [supplierId] = @supplierId"
            db.AddDBParam("supplierId", SupplierId.ToString, True)
            db.RunSQL(sql, db.glDBWorkB, False, "", False)

            result = True

            If db.GetRowCount(db.glDBWorkA, 0) = 0 Then
                ' no contract products so ok
                ValidateContractProducts = True
                Exit Function
            End If

            If db.GetRowCount(db.glDBWorkB, 0) = 0 Then
                ' no products associated with the new vendor, so invalid
                ValidateContractProducts = False
                Exit Function
            End If

            For Each curRow In db.glDBWorkA.Tables(0).Rows
                curConProdId = curRow.Item("productId")
                match = False
                For Each curVendorRow In db.glDBWorkB.Tables(0).Rows
                    If curVendorRow.Item("productId") = curConProdId Then
                        match = True
                        Exit For
                    End If
                Next

                If match = False Then
                    ' match not found for a particular vendor - contract product
                    result = False
                    Exit For
                End If
            Next

            db.DBClose()
            db = Nothing
            ValidateContractProducts = result
        End Function

        Private Sub GetCDNotes()
            Dim tmpstr As String

            If Session("ActiveContract") <> 0 Then
                Session("NoteReturnURL") = "ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Trim(Session("ActiveContract"))

                tmpstr = "ViewNotes.aspx?id=-1&notetype=Contract&contractid=" & Trim(Session("ActiveContract")) & "&item=" & Trim(Replace(Replace(txtDescription.Text, "&", "%26"), vbNewLine, ""))
                Response.Redirect(tmpstr, True)
            End If
        End Sub

        Protected Sub cmdNotes_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            GetCDNotes()
        End Sub

        Protected Sub lnkNotify_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Response.Redirect("Associations.aspx?frompage=contractdetails", True)
        End Sub

        Protected Sub lnkSchedules_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSchedules.Click
            Response.Redirect("ScheduleAction.aspx", True)
        End Sub

        Protected Sub lstSchedules_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            If lstSchedules.SelectedItem.Value > 0 Then
                Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & lstSchedules.SelectedItem.Value, True)
            End If
        End Sub

        Protected Sub imgAudience_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAudience.Click
            Session("AudienceReturnURL") = "ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract")
            Response.Redirect("SetAudience.aspx?type=contract&id=" & Session("ActiveContract"), True)
        End Sub

        Private Function GetContractAudience(ByVal db As cFWDBConnection, ByVal id As Integer) As String
            Dim audStr As New System.Text.StringBuilder
            Dim sql As New System.Text.StringBuilder
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            sql.Append("SELECT employees.[surname] + ', ' + employees.[title] + ' ' + employees.firstname + ' (' + [username] + ')' AS Audience, [audienceType] FROM [contract_audience] " & vbNewLine)
            sql.Append("INNER JOIN [employees] ON [contract_audience].[accessId] = [employees].[employeeId] " & vbNewLine)
            sql.Append("WHERE [contractId] = @conId AND [audienceType] = @IndAudience " & vbNewLine)
            sql.Append("UNION " & vbNewLine)
            sql.Append("SELECT [teamname] AS Audience, [audienceType] FROM [contract_audience] " & vbNewLine)
            sql.Append("INNER JOIN [teams] ON [contract_audience].[accessId] = [teams].[teamid] " & vbNewLine)
            sql.Append("WHERE [audienceType] = @TeamAudience AND [contractId] = @conId" & vbNewLine)

            db.AddDBParam("conId", id, True)
            db.AddDBParam("TeamAudience", AudienceType.Team, False)
            db.AddDBParam("IndAudience", AudienceType.Individual, False)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            Dim drow As DataRow
            For Each drow In db.glDBWorkA.Tables(0).Rows
                If CType(drow.Item("audienceType"), AudienceType) = AudienceType.Team Then
                    audStr.Append("*")
                End If
                audStr.Append(drow.Item("Audience"))
                audStr.Append(vbNewLine)
            Next
            Return audStr.ToString
        End Function

        Private Function GetVariationList(ByVal db As cFWDBConnection, ByVal curUser As CurrentUser, ByVal cId As Integer) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim sql As String
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)

            sql = "SELECT [contractId],[contractKey],[contractNumber],[contractDescription],ISNULL([contractValue],0) AS [contractValue],[contractCurrency],[reviewComplete],ISNULL([scheduleNumber],'') AS [scheduleNumber] FROM [contract_details] WHERE [contractId] IN (SELECT [variationContractId] FROM [link_variations] WHERE [primaryContractId] = @primaryContractId) AND [subAccountId] = @locId"
            db.AddDBParam("locId", curUser.CurrentSubAccountId, True)
            db.AddDBParam("primaryContractId", cId, False)
            db.RunSQL(sql, db.glDBWorkL, False, "", False)

            Dim drow As DataRow
            Dim rowClass As String = "row1"
            Dim rowalt As Boolean = False

            With strHTML
                '.Append("<div class=""inputpanel"">" & vbNewLine)
                .Append("<table class=""datatbl"">" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractVariations, True) Then
                    .Append("<th style=""width: 30px""><img src=""./icons/16/plain/form_green.png"" alt=""Open Variation"" /></th>" & vbNewLine)
                End If

                'If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractVariations, True) Then
                .Append("<th style=""width: 30px""><img src=""./icons/16/plain/folder_lock.png"" alt=""Close/Reinstate variation"" /></th>" & vbNewLine)
                'End If

                .Append("<th>Contract Key</th>" & vbNewLine)
                .Append("<th>Contract Number</th>" & vbNewLine)
                .Append("<th>Variation / Extension</th>" & vbNewLine)
                .Append("<th>Description</th>" & vbNewLine)
                .Append("<th>Contract Value</th>" & vbNewLine)
                .Append("</tr>" & vbNewLine)

                For Each drow In db.glDBWorkL.Tables(0).Rows
                    rowalt = (rowalt Xor True)
                    If rowalt Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    .Append("<tr>" & vbNewLine)
                    If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractVariations, True) Then
                        .Append("<td class=""" & rowClass & """><a href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & drow.Item("contractId") & """ title=""Open contract variation"" onmouseover=""window.status='Open contract variation';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/16/plain/form_green.png"" alt=""Open Variation"" /></a></td>" & vbNewLine)
                    End If

                    If IsDBNull(drow.Item("reviewComplete")) = True Then
                        ' variation is open. provide icon to close
                        .Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Close the variation';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract") & "&cdaction=vclose&expand=1&vid=" & drow.Item("contractId") & """ title=""Close the variation""><img src=""./icons/16/plain/folder_lock.png"" alt=""Close"" /></a></td>" & vbNewLine)
                    Else
                        ' variation is closed. provide icon to re-open
                        .Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Reinstate the variation';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract") & "&cdaction=vopen&expand=1&vid=" & drow.Item("contractId") & """ title=""Reinstate the variation""><img src=""./icons/16/plain/folder_out.png"" alt=""Reinstate"" /></a></td>" & vbNewLine)
                    End If

                    .Append("<td class=""" & rowClass & """>" & drow.Item("contractKey") & "</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & drow.Item("contractNumber") & "</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & drow.Item("scheduleNumber") & "</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & drow.Item("contractDescription") & "</td>" & vbNewLine)

                    Dim currencyStr As String = ""

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                        If IsDBNull(drow.Item("contractCurrency")) Then
                            currencyStr = "0"
                        Else
                            currencyStr = currency.FormatCurrency(drow.Item("contractValue"), currency.getCurrencyById(drow.Item("contractCurrency")), False)
                        End If
                    Else
                        currencyStr = "N/A"
                    End If
                    .Append("<td class=""" & rowClass & """ align=""right"">" & currencyStr & "</td>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                Next

                .Append("</table>" & vbNewLine)
                '.Append("</div>" & vbNewLine)
            End With

            Return strHTML.ToString
        End Function

        Private Function ReturnToPrimaryContract(ByVal db As cFWDBConnection, ByVal conId As Integer) As String
            Dim strHTML As New System.Text.StringBuilder

            db.FWDb("R2", "link_variations", "variationContractId", conId, "", "", "", "", "", "", "", "", "", "")
            With strHTML
                '.Append("<div class=""inputpanel"">" & vbNewLine)
                .Append("<table class=""datatbl"">" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                .Append("<th>Current Contract tagged as a Variation</th>" & vbNewLine)
                .Append("</tr>" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                If db.FWDbFlag Then
                    .Append("<td class=""row1""><a href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & db.FWDbFindVal("primaryContractId", 2).Trim & """>Return to Primary Contract</a></td>" & vbNewLine)
                Else
                    .Append("<td class=""row1"">Link to Primary contract could not be found.</td>" & vbNewLine)
                End If
                .Append("</tr>" & vbNewLine)

                .Append("</table>" & vbNewLine)
                '.Append("</div>" & vbNewLine)
            End With

            Return strHTML.ToString
        End Function

        Private Sub SetVariationState(ByVal curUser As CurrentUser, ByVal state As String, ByVal vId As Integer)
            Try
                Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim ARec As New cAuditRecord
                Dim db As New cFWDBConnection

                db.DBOpen(fws, False)

                ARec.Action = cFWAuditLog.AUDIT_UPDATE
                ARec.DataElementDesc = "VARIATION STATE"

                db.FWDb("R2", "contract_details", "contractId", vId, "", "", "", "", "", "", "", "", "", "")
                If db.FWDb2Flag Then
                    ARec.ContractNumber = db.FWDbFindVal("contractKey", 2)
                End If

                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractVariations, curUser.CurrentSubAccountId)

                Select Case state
                    Case "open"
                        ' re-instate a variation
                        ARec.ElementDesc = "REINSTATE"
                        ARec.PreVal = "Closed"
                        ARec.PostVal = "Active"

                        db.AddDBParam("vconId", vId, True)
                        db.ExecuteSQL("UPDATE [contract_details] SET [reviewComplete] = NULL WHERE [contractId] = @vconId")

                        db.SetFieldValue("Closed", 0, "N", True)
                        db.FWDb("A", "link_variations", "variationContractId", vId, "", "", "", "", "", "", "", "", "", "")

                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, vId)

                        ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.ContractDetail, vId, ARec)

                    Case "close"
                        ' close a variation
                        ARec.ElementDesc = "CLOSE"
                        ARec.PreVal = "Active"
                        ARec.PostVal = "Closed"

                        db.SetFieldValue("reviewComplete", Now, "D", True)
                        db.FWDb("A", "contract_details", "contractId", vId, "", "", "", "", "", "", "", "", "", "")

                        db.SetFieldValue("Closed", 1, "N", True)
                        db.FWDb("A", "link_variations", "variationContractId", vId, "", "", "", "", "", "", "", "", "", "")

                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, vId)

                        ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.ContractDetail, vId, ARec)
                    Case Else

                End Select

                db.DBClose()
            Catch ex As Exception

            End Try
        End Sub

        Protected Sub lnkAddCDTask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddCDTask.Click
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)
            cLocks.RemoveLockItem(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), Cache, "CD_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.EmployeeID)
            cLocks.RemoveLockItem(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), curUser.EmployeeID)
            db.DBClose()

            Dim varURL As String
            varURL = "tid=0&rid=" & Session("ActiveContract") & "&rtid=" & AppAreas.CONTRACT_DETAILS
            Session("TaskRetURL") = Server.UrlEncode("~/ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Session("ActiveContract"))
            Response.Redirect("~/shared/tasks/ViewTask.aspx?" & varURL, True)
        End Sub

        Protected Sub lstCancellationPeriodType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstCancellationPeriodType.SelectedIndexChanged
            CalcDate("CANCEL")
        End Sub

        Protected Sub lstReviewPeriodType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstReviewPeriodType.SelectedIndexChanged
            CalcDate("REVIEW")
        End Sub
#End Region

#Region "Contract Additional Code"
        Private Sub ContractAdditional_Load()
            Dim uhd As UserHelpDocs
            'Dim hasAnyFields As Boolean = False
            Dim IsLocked As Boolean = False
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim colParams As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim ufields As New cUserdefinedFields(curUser.AccountID)

            ' get a dataset containing the different additional field groupings, with only those allocated for
            ' display with the current contract category

            ' THIS IS CALLED IN THE PAGE_INIT instead
            'GetContractAdditionalData(db, uinfo, fws, Session("HasCAFields"))

            ' lock contract to this user, unless it is already locked
            Dim cacheHit As Boolean = False
            Dim LockedBy As String = "Unknown User"
            Dim cacheKey As String = "CA_" & curUser.AccountID.ToString() & "_" & Session("ActiveContract")

            If Not Cache(cacheKey) Is Nothing Then
                Dim lockEmployeeId As Integer
                lockEmployeeId = CInt(Cache(cacheKey))
                cacheHit = True

                If lockEmployeeId <> curUser.EmployeeID Then
                    IsLocked = True
                    Dim emps As New cEmployees(curUser.AccountID)
                    Dim emp As Employee = emps.GetEmployeeById(lockEmployeeId)
                    If Not emp Is Nothing Then
                        LockedBy = emp.FullName
                    End If
                End If
            End If

            If Session("ActiveContract") > 0 Then
                Dim tables As New cTables(curUser.AccountID)
                Dim fields As New cFields(curUser.AccountID)
                Dim cdtable As cTable = tables.GetTableByName("contract_details")
                Dim udTable As cTable = tables.GetTableByID(cdtable.UserDefinedTableID)
                ufields.populateRecordDetails(phCAUFields, udTable, ufields.GetRecord(udTable, Session("ActiveContract"), tables, fields), groupType:=GroupingOutputType.GroupedOnly)
            End If

            If IsLocked = False And cacheHit = False Then
                Dim cacheTimeout As Double = 5
                cacheTimeout = colParams.CacheTimeout

                cLocks.LockContract(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), Cache, "CA_" & curUser.AccountID.ToString, Session("ActiveContract"), cacheTimeout, curUser.EmployeeID)
                litCALockMsg.Text = ""
            ElseIf IsLocked = True Then
                ' display notification at the top of the screen to indicate that it is locked and by whom
                litCALockMsg.Text = "<img src=""./icons/16/plain/lock.gif"" alt=""Locked"" />&nbsp;The contract is currently locked by " & LockedBy
            End If

            ' place update buttons etc.

            cmdCAUpdate.AlternateText = "Update"
            cmdCAUpdate.ToolTip = "Save any changes to the database"
            cmdCAUpdate.Attributes.Add("onmouseover", "window.status='Save any changes to the database';return true;")
            cmdCAUpdate.Attributes.Add("onmouseout", "window.status='Done';")

            cmdCAUpdate.Visible = Not IsLocked And curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractDetails, False)

            If cmdCAUpdate.Visible = False Then
                Master.useCloseNavigationMsg = True
                'Master.enablenavigation = False
                cmdCACancel.ImageUrl = "./buttons/page_close.png"

                Master.RefreshBreadcrumbInfo()
            End If

            ' display bespoke help link if it exists
            uhd = Session("UserHelpDocs")
            If uhd.ContractAdditional <> "" Then
                litUDH.Text = "<a class=""submenuitem"" onmouseover=""window.status='Open Field Guidance Notes';return true;"" onmouseout=""window.status='Done';"" onclick=""javascript:window.open('file://" & Trim(uhd.ContractHelpDir) & Trim(uhd.ContractAdditional) & "');"">Additional Help</a>"
            End If
        End Sub

        Private Sub Update_ContractAdditional()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            'Dim arrARecs As New ArrayList

            If Session("ActiveContract") = 0 Or Session("ActiveContract") Is Nothing Then
                Exit Sub
            End If

            Dim ALog As New cAuditLog(curUser.AccountID, curUser.EmployeeID)
            Dim employees As New cEmployees(curUser.AccountID)
            Dim ARec As New cAuditRecord
            Dim ARecs As New List(Of cAuditRecord)
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.AccountID))
            Dim dbParams As New Dictionary(Of String, Object)

            ARec.Action = cFWAuditLog.AUDIT_UPDATE
            'ARec.ContractNumber = db.FWDbFindVal("Contract Key", 2)
            ARec.DataElementDesc = "CONTRACT ADDITIONAL"

            Dim ufields As New cUserdefinedFields(curUser.Account.accountid)
            Dim tables As New cTables(curUser.Account.accountid)
            Dim fields As New cFields(curUser.AccountID)
            Dim cdTable As cTable = tables.getTableByName("contract_details")
            Dim udTable As cTable = tables.getTableById(cdTable.UserDefinedTableID)
            Dim cdUFields As List(Of cUserDefinedField) = ufields.GetFieldsByTable(udTable)

            Dim sql As String = "select [contractKey] from contract_details where [contractId] = " + Convert.ToString(Session("ActiveContract"))
            Dim contractKey As String = db.getStringValue(sql)

                ' update the user defined fields
                Dim newrec As SortedList(Of Integer, Object) = ufields.getItemsFromPanel(phCAUFields, udTable, groupType:=GroupingOutputType.GroupedOnly)
            ufields.SaveValues(udTable, Session("ActiveContract"), newrec, tables, fields, curUser, groupType:=GroupingOutputType.GroupedOnly, elementId:=SpendManagementElement.ContractAdditional, record:=contractKey)
        End Sub

        Protected Sub cmdCAUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCAUpdate.Click
            cmdCAUpdate.Enabled = False
            Update_ContractAdditional()
            Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractAdditional & "&id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub cmdCACancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCACancel.Click
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim cacheEnum As IDictionaryEnumerator = Cache.GetEnumerator
            Dim tmpEmployeeId As Integer

            While cacheEnum.MoveNext
                If cacheEnum.Key = "CA" & Session("ActiveContract") Then
                    tmpEmployeeId = cacheEnum.Value
                    If tmpEmployeeId = curUser.Employee.employeeid Then
                        Cache.Remove("CA" & Session("ActiveContract"))
                    End If
                    Exit While
                End If
            End While

            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
            Dim navURL As String = "Home.aspx"

            If Session("ContractAFS") = 1 Then
                ' retrieve the supplier id and return to supplier screen
                Dim sql As String = "select [supplierId] from contract_details where [contractid] = @conId"
                db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))
                Dim supplierId As Integer = db.getcount(sql)

                If supplierId > 0 Then
                    navURL = "~/shared/supplier_details.aspx?xcon=1&sid=" & supplierId.ToString
                End If
            End If

            Response.Redirect(navURL, True)
        End Sub

#End Region

#Region "Contract Product Code"
        Private Sub ContractProduct_Load()
            Dim action As String = ""
            Dim actionid As Integer = 0
            Dim hasData As Boolean
            Dim ConCategoryId As Integer = 0
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
            Dim sql As String



            hasData = False
            If lstContractCategory.SelectedValue <> "0" And lstContractCategory.SelectedValue <> "" Then
                sql = "select [categoryId] from contract_details where [contractId] = @conId"
                db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))
                ConCategoryId = db.getcount(sql)
            End If
            action = Request.QueryString("cpaction")

            actionid = Request.QueryString("cpid")
            Session("CPAction") = LCase(action)


            Select Case action
                Case "add"
                    NewCPEntry(ConCategoryId)

                Case "edit"
                    DisplayCPForEdit(actionid, ConCategoryId)

                    'Case "callproduct"
                    '    Dim strHTML As New System.Text.StringBuilder
                    '    strHTML.Append("<table>" & vbNewLine)
                    '    strHTML.Append(GetProduct(db, actionid))
                    '    strHTML.Append("</table>" & vbNewLine)
                    '    Response.Write(strHTML.ToString)
                    '    Response.Flush()
                    '    Response.End()
                    '    Exit Sub

                    'Case "callback"
                    '    UFieldlist = GetUFieldList(db, ConCategoryId)
                    '    Response.Write(GetSingleUFData(db, actionid, UFieldlist))
                    '    Response.Flush()
                    '    Response.End()
                    '    Exit Sub

                Case "cpnotes"
                    Dim cpId As Integer
                    cpId = Request.QueryString("cpid")
                    GetCPNotes(curUser, cpId)

                Case Else
                    GetCPRecs(ConCategoryId)
            End Select

            Sql = "select [contractDescription] from contract_details where [contractId] = @conId"
            db.sqlexecute.Parameters.Clear()
            db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))

            lblCPTitle.Text = " - " & db.getStringValue(Sql)

            'If uinfo.permDelete = True Then
            '    lnkDelete.ToolTip = "Delete the currently selected entry"
            'End If

            If cmdCPUpdate.Visible = False Then
                cmdCPCancel.ImageUrl = "~/buttons/page_close.png"
            Else
                cmdCPCancel.ImageUrl = "~/buttons/cancel.gif"
            End If

            cmdCPCancel.AlternateText = "Cancel"
            cmdCPCancel.ToolTip = "Abandon add/edit and return to product list"
            cmdCPCancel.Attributes.Add("onmouseover", "window.status='Abandon add/edit and return to product list';return true;")
            cmdCPCancel.Attributes.Add("onmouseout", "window.status='Done';")

        End Sub

        Private Sub GetCPRecs(ByVal contractCategoryId As Integer)
            ' get contract maintenance parameters
            Dim UFieldlist As String = ""
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            db.DBOpen(FWS, False)

            Master.enablenavigation = False
            Master.useCloseNavigationMsg = True
            Master.RefreshBreadcrumbInfo()

            Session("MaintParams") = SMRoutines.GetMaintParams(db, Session("ActiveContract"))

            UFieldlist = GetUFieldList(curUser, contractCategoryId)

            GetContractProducts(db, UFieldlist)
            Session("CPCount") = ConProd_Count

            db.DBClose()
        End Sub

        Private Sub UpdateCPRecord()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
            Dim tmpVal, preVal As Object
            Dim ErrStr As String = ""
            Dim hasQty, hasProductVal, hasMaintPct, hasMaintVal As Boolean
            Dim ARec As New cAuditRecord
            Dim ARecs As New List(Of cAuditRecord)
            Dim ARecContrID As String
            Dim ARecEleDesc As String
            Dim ARecAction As Char
            Dim firstEntry, isAmend, isAddMulti As Boolean
            Dim MaintChanged, QtyChanged As Boolean
            Dim isAddAndDefine As Boolean
            Dim sql As System.Text.StringBuilder
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim contractcategoryid As Integer = 0
            Dim ALog As New cAuditLog(curUser.Account.accountid, curUser.Employee.employeeid)
            Dim dsetA As New DataSet

            ARecContrID = String.Empty
            ARecEleDesc = String.Empty
            hasProductVal = False
            hasQty = False
            QtyChanged = False
            hasMaintPct = False
            hasMaintVal = False
            MaintChanged = False
            firstEntry = True
            isAddMulti = False
            isAddAndDefine = False

            lblErrorString.Text = String.Empty

            ErrStr = "ERROR! Invalid entry "

            Select Case Session("CPAction")
                Case "edit"
                    sql = New System.Text.StringBuilder
                    sql.Append(Session("PreRec"))
                    dsetA = db.GetDataSet(sql.ToString)
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARecAction = cFWAuditLog.AUDIT_UPDATE
                    isAmend = True

                Case "add"
                    If Session("ActiveContract") = Nothing Or Session("ActiveContract") < 1 Then
                        lblErrorString.Text = "ERROR! Unknown contract id. Cannot add product at this time."
                        cmdCPUpdate.Enabled = True
                        Exit Sub
                    End If

                    If Request.QueryString("am") = "1" Or Session("CP_AddMulti") = "1" Then
                        isAddMulti = True
                        Session("CP_AddMulti") = "1"
                    End If

                    If Session("CP_AddandDefine") = "1" Then
                        isAddAndDefine = True
                    End If

                    isAmend = False

                Case Else
                    lblErrorString.Text = "Unknown action trying to add/update record"
                    cmdCPUpdate.Enabled = True
                    Exit Sub
            End Select

            If isAddAndDefine = False Then
                If lstProductName.SelectedItem.Value = 0 Then
                    lblErrorString.Text = "ERROR! Valid Product must be selected"
                    cmdCPUpdate.Enabled = True
                    Exit Sub
                End If
            Else
                If txtProductName.Text.Trim = "" Then
                    lblErrorString.Text = "ERROR! A Product Name must be provided"
                    cmdCPUpdate.Enabled = True
                    Exit Sub
                End If
            End If

            If lstCPCurrency.SelectedItem.Value = 0 Then
                lblErrorString.Text = "ERROR! Valid Currency must be selected"
                cmdCPUpdate.Enabled = True
                Exit Sub
            End If

            Dim catSQL As String = "select [contractKey], [categoryId] from contract_details where [contractId] = @conId"
            db.sqlexecute.Parameters.Clear()
            db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))
            Dim dsetTmp As DataSet = db.GetDataSet(catSQL)

            For Each drow As DataRow In dsetTmp.Tables(0).Rows
                If Not IsDBNull(drow("categoryId")) Then
                    contractcategoryid = CInt(drow("categoryId"))
                End If
                ARec.ContractNumber = CStr(drow("contractKey"))
                ARecContrID = CStr(drow("contractKey"))
            Next

            Dim tmpDE As String
            If isAmend = True Then
                tmpDE = CStr(dsetA.Tables(0).Rows(0).Item("productName"))
            Else
                If isAddAndDefine Then
                    tmpDE = txtProductName.Text
                Else
                    tmpDE = lstProductName.SelectedItem.Text
                End If
            End If

            Dim dbParams As New Dictionary(Of String, Object)
            ARec.DataElementDesc = "CON.PROD:" & tmpDE
            ARecEleDesc = "CON.PROD:" & tmpDE

            ' validate information input
            If isAmend Then
                tmpVal = lstProductName.SelectedItem.Value
                preVal = CInt(dsetA.Tables(0).Rows(0).Item("productId"))
                If tmpVal <> preVal Then
                    ARec.PreVal = lstProductName.Items.FindByValue(SMRoutines.CheckListIndex(preVal)).Text
                    ARec.PostVal = lstProductName.SelectedItem.Text
                    ARec.ElementDesc = "PRODUCT NAME"
                    ARecs.Add(ARec)

                    dbParams.Add("productId", tmpVal)

                    firstEntry = False
                End If
            Else
                ARec.Action = cFWAuditLog.AUDIT_ADD

                If isAddAndDefine = True Then
                    ' must be using the txtProductName input box instead
                    Dim newProdID As Integer
                    newProdID = ValidateNewProduct()

                    If newProdID > 0 Then
                        ' product has been added and associated with the vendor
                        dbParams.Add("contractId", Session("ActiveContract"))
                        firstEntry = False

                        dbParams.Add("productId", newProdID)
                    Else
                        Dim tmpSuppStr As String = params.SupplierPrimaryTitle
                        lblErrorString.Text = "ERROR! Product Name provided already exists. Use standard Add option or check Product Association with " & tmpSuppStr
                        cmdCPUpdate.Enabled = True
                        Exit Sub
                    End If
                Else
                    tmpVal = lstProductName.SelectedItem.Value
                    dbParams.Add("contractId", Session("ActiveContract"))
                    firstEntry = False

                    dbParams.Add("productId", tmpVal)
                End If
            End If

            tmpVal = lstUnits.SelectedItem.Value
            If isAmend Then
                If Not dsetA.Tables(0).Rows(0).Item("unitId") Is DBNull.Value Then
                    preVal = dsetA.Tables(0).Rows(0).Item("unitId")
                    If tmpVal <> preVal Then
                        ARec = New cAuditRecord
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ARecContrID
                        ARec.DataElementDesc = ARecEleDesc
                        ARec.PreVal = lstUnits.Items.FindByValue(SMRoutines.CheckListIndex(preVal)).Text
                        ARec.PostVal = lstUnits.SelectedItem.Text
                        ARec.ElementDesc = "UNITS"
                        ARecs.Add(ARec)

                        If tmpVal = "0" Then
                            dbParams.Add("unitId", DBNull.Value)
                        Else
                            dbParams.Add("unitId", tmpVal)
                        End If

                        firstEntry = False
                    End If
                Else
                    If tmpVal = "0" Then
                        dbParams.Add("unitId", DBNull.Value)
                    Else
                        dbParams.Add("unitId", tmpVal)
                    End If

                    firstEntry = False
                End If


            Else
                If tmpVal > 0 Then
                    dbParams.Add("unitId", tmpVal)
                    firstEntry = False
                End If
            End If

            tmpVal = lstCPSalesTax.SelectedItem.Value
            If isAmend Then
                If Not dsetA.Tables(0).Rows(0).Item("salesTaxRate") Is DBNull.Value Then
                    preVal = dsetA.Tables(0).Rows(0).Item("salesTaxRate")
                    If tmpVal <> preVal Then
                        ARec = New cAuditRecord
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ARecContrID
                        ARec.DataElementDesc = ARecEleDesc
                        ARec.PreVal = lstCPSalesTax.Items.FindByValue(SMRoutines.CheckListIndex(preVal)).Text
                        ARec.PostVal = lstCPSalesTax.SelectedItem.Text
                        ARec.ElementDesc = "SALES TAX"
                        ARecs.Add(ARec)

                        If tmpVal = "0" Then
                            dbParams.Add("salesTaxRate", DBNull.Value)
                        Else
                            dbParams.Add("salesTaxRate", tmpVal)
                        End If

                        firstEntry = False
                    End If
                Else
                    If tmpVal = "0" Then
                        dbParams.Add("salesTaxRate", DBNull.Value)
                    Else
                        dbParams.Add("salesTaxRate", tmpVal)
                    End If

                    firstEntry = False
                End If
            Else
                If tmpVal > 0 Then
                    dbParams.Add("salesTaxRate", tmpVal)
                    firstEntry = False
                End If
            End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                ' only allow update if they have financial viewing permissions
                tmpVal = txtProductValue.Text
                If tmpVal <> "" Then
                    If isAmend Then
                        preVal = dsetA.Tables(0).Rows(0).Item("productValue")
                        If tmpVal <> preVal Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = ARecEleDesc
                            ARec.PreVal = preVal
                            ARec.PostVal = tmpVal
                            ARec.ElementDesc = "PRODUCT VALUE"
                            ARecs.Add(ARec)

                            dbParams.Add("productValue", tmpVal)

                            firstEntry = False
                        End If
                    Else
                        dbParams.Add("productValue", tmpVal)
                        firstEntry = False
                    End If

                    If tmpVal > 0 Then
                        hasProductVal = True
                    End If
                Else
                    dbParams.Add("productValue", 0)
                    firstEntry = False
                End If

                tmpVal = txtPricePaid.Text
                If tmpVal <> "" Then
                    If isAmend Then
                        preVal = dsetA.Tables(0).Rows(0).Item("pricePaid")
                        If tmpVal <> preVal Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = ARecEleDesc
                            ARec.PreVal = preVal
                            ARec.PostVal = tmpVal
                            ARec.ElementDesc = "PRICE PAID"
                            ARecs.Add(ARec)

                            dbParams.Add("pricePaid", tmpVal)

                            firstEntry = False
                        End If
                    Else
                        dbParams.Add("pricePaid", tmpVal)
                        firstEntry = False
                    End If
                Else
                    dbParams.Add("pricePaid", 0)
                    firstEntry = False
                End If
            End If

            tmpVal = txtQuantity.Text
            If tmpVal <> "" Then
                If isAmend Then
                    preVal = dsetA.Tables(0).Rows(0).Item("Quantity")
                    If tmpVal <> preVal Then
                        ARec = New cAuditRecord
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ARecContrID
                        ARec.DataElementDesc = ARecEleDesc
                        ARec.PreVal = preVal
                        ARec.PostVal = tmpVal
                        ARec.ElementDesc = "QUANTITY"
                        ARecs.Add(ARec)

                        dbParams.Add("Quantity", tmpVal)
                        QtyChanged = True

                        firstEntry = False
                    End If
                Else
                    dbParams.Add("Quantity", tmpVal)
                    firstEntry = False
                End If

                If tmpVal > 0 Then
                    hasQty = True
                End If
            Else
                txtQuantity.Text = "0"
                dbParams.Add("Quantity", 0)
                firstEntry = False
            End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False) Then
                tmpVal = txtProjectedSaving.Text
                If tmpVal <> "" Then
                    If isAmend Then
                        preVal = dsetA.Tables(0).Rows(0).Item("projectedSaving")
                        If tmpVal <> preVal Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = ARecEleDesc
                            ARec.PreVal = preVal
                            ARec.PostVal = tmpVal
                            ARec.ElementDesc = "PROJ.SAVING"
                            ARecs.Add(ARec)

                            dbParams.Add("projectedSaving", tmpVal)

                            firstEntry = False
                        End If
                    Else
                        dbParams.Add("projectedSaving", tmpVal)
                        firstEntry = False
                    End If
                Else
                    dbParams.Add("projectedSaving", 0)
                    firstEntry = False
                End If
            End If

            tmpVal = txtMaintPercent.Text
            If tmpVal <> "" Then
                If isAmend Then
                    preVal = dsetA.Tables(0).Rows(0).Item("maintenancePercent")
                    If tmpVal <> preVal Then
                        ARec = New cAuditRecord
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ARecContrID
                        ARec.DataElementDesc = ARecEleDesc
                        ARec.PreVal = preVal
                        ARec.PostVal = tmpVal
                        ARec.ElementDesc = "MAINT %"
                        ARecs.Add(ARec)

                        dbParams.Add("maintenancePercent", tmpVal)

                        firstEntry = False
                    End If
                Else
                    dbParams.Add("maintenancePercent", tmpVal)
                    firstEntry = False
                End If

                If tmpVal > 0 Then
                    hasMaintPct = True
                End If
            Else
                dbParams.Add("maintenancePercent", 0)
                firstEntry = False
            End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                tmpVal = txtMaintValue.Text
                If isAmend Then
                    If tmpVal <> "" Then
                        preVal = dsetA.Tables(0).Rows(0).Item("maintenanceValue")
                        If tmpVal <> preVal Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = ARecEleDesc
                            ARec.PreVal = preVal
                            ARec.PostVal = tmpVal
                            ARec.ElementDesc = "MAINT VALUE"
                            ARecs.Add(ARec)

                            dbParams.Add("maintenanceValue", tmpVal)
                            MaintChanged = True

                            firstEntry = False
                        End If

                        If tmpVal > 0 Then
                            hasMaintVal = True
                        End If
                    Else
                        txtMaintValue.Text = "0"
                        dbParams.Add("maintenanceValue", 0)
                        firstEntry = False
                        hasMaintVal = False
                    End If
                Else
                    If tmpVal = "" Then
                        If hasMaintPct And hasProductVal Then
                            ' calculate the maintenance value
                            tmpVal = Val(txtProductValue.Text) * (Val(txtMaintPercent.Text) / 100)
                            dbParams.Add("maintenanceValue", tmpVal)
                            txtMaintValue.Text = CStr(tmpVal)
                            firstEntry = False

                            If tmpVal > 0 Then
                                hasMaintVal = True
                            End If
                        Else
                            dbParams.Add("maintenanceValue", 0)
                            txtMaintValue.Text = "0"
                            firstEntry = False
                        End If
                    Else
                        dbParams.Add("maintenanceValue", tmpVal)
                        If tmpVal > 0 Then
                            hasMaintVal = True
                        End If
                        firstEntry = False
                    End If
                End If

                tmpVal = txtUnitCost.Text
                If isAmend Then
                    If tmpVal <> "" Then
                        preVal = dsetA.Tables(0).Rows(0).Item("unitCost")
                        If tmpVal <> preVal Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = ARecEleDesc
                            ARec.PreVal = preVal
                            ARec.PostVal = tmpVal
                            ARec.ElementDesc = "UNIT COST"
                            ARecs.Add(ARec)

                            dbParams.Add("unitCost", tmpVal)

                            firstEntry = False
                        End If
                    Else
                        dbParams.Add("unitCost", 0)
                        firstEntry = False
                    End If
                Else
                    If hasQty And hasMaintVal Then
                        ' calculate the unit cost
                        Dim tmpMV, tmpQ As Double

                        tmpQ = Double.Parse(txtQuantity.Text)
                        tmpMV = Double.Parse(txtMaintValue.Text)
                        tmpVal = Math.Round((tmpMV / tmpQ), 2, MidpointRounding.AwayFromZero)
                        dbParams.Add("unitCost", tmpVal)
                        firstEntry = False
                    Else
                        If tmpVal <> "" Then
                            dbParams.Add("unitCost", tmpVal)
                        Else
                            dbParams.Add("unitCost", 0)
                        End If
                        firstEntry = False
                    End If
                End If
            End If

            tmpVal = lstCPCurrency.SelectedValue
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            If isAmend = True Then
                If IsDBNull(dsetA.Tables(0).Rows(0).Item("currencyId")) = False Then
                    preVal = dsetA.Tables(0).Rows(0).Item("currencyId")
                Else
                    preVal = "0"
                End If

                If tmpVal <> preVal Then
                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = ARecEleDesc
                    ARec.PreVal = lstCPCurrency.Items.FindByValue(preVal).Text
                    ARec.PostVal = lstCPCurrency.SelectedItem.Text
                    ARec.ElementDesc = "CURRENCY"
                    ARecs.Add(ARec)

                    If tmpVal = "0" Then
                        dbParams.Add("currencyId", DBNull.Value)
                    Else
                        dbParams.Add("currencyId", tmpVal)
                    End If

                    firstEntry = False
                End If
            Else
                If tmpVal > 0 Then
                    dbParams.Add("currencyId", tmpVal)
                    firstEntry = False
                End If
            End If

            ' update UF fields
            'Dim drow As DataRow
            'Dim prevalStr As String = ""
            'Dim postvalStr As String = ""
            'Dim prevalNum, postvalNum As Double
            Dim fieldName As String = ""

            Dim ufields As New cUserdefinedFields(curUser.Account.accountid)
            Dim tables As New cTables(curUser.Account.accountid)
            Dim fields As New cFields(curUser.AccountID)
            Dim cpTable As cTable = tables.getTableByName("contract_productdetails")
            Dim udTable As cTable = tables.getTableById(cpTable.UserDefinedTableID)
            Dim cpUFields As List(Of cUserDefinedField) = ufields.GetFieldsByTable(udTable)

            ' update the user defined fields
            Dim employees As New cEmployees(curUser.Account.accountid)

            If firstEntry = False Then
                ' make sure something changed!
                If isAmend Then
                    db.sqlexecute.Parameters.Clear()

                    Dim comma As String = ""
                    Dim strsql As String = "update contract_productdetails set "
                    For Each p As KeyValuePair(Of String, Object) In dbParams
                        strsql += comma & " [" & p.Key & "] = @" & p.Key.Replace(" ", "_")

                        db.sqlexecute.Parameters.AddWithValue("@" & p.Key.Replace(" ", "_"), p.Value)
                        comma = ","
                    Next
                    strsql += " where [contractProductId] = @cpId"
                    db.sqlexecute.Parameters.AddWithValue("@cpId", Session("ActiveContractProduct"))
                    db.ExecuteSQL(strsql)

                    'For Each a As cAuditRecord In ARecs
                    '    ALog.editRecord(CInt(Session("ActiveContractProduct")), a.ContractNumber, a.ElementDesc, a.DataElementDesc, a.PreVal, a.PostVal)
                    'Next
                    ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.ContractProduct, Session("ActiveContract"), ARecs)
                Else
                    db.sqlexecute.Parameters.Clear()
                    Dim comma As String = ""
                    Dim strsql As String = "insert into contract_productdetails ("
                    Dim sqlValues As String = " values ("

                    For Each p As KeyValuePair(Of String, Object) In dbParams
                        strsql += comma & "[" & p.Key & "]"
                        sqlValues += comma & "@" & p.Key.Replace(" ", "_")

                        db.sqlexecute.Parameters.AddWithValue("@" & p.Key.Replace(" ", "_"), p.Value)
                        comma = ","
                    Next
                    strsql += comma & "[createdDate],[lastModified]) "
                    sqlValues += comma & "@Created_Date, @Last_Modified)"
                    db.sqlexecute.Parameters.AddWithValue("@Created_Date", Now)
                    db.sqlexecute.Parameters.AddWithValue("@Last_Modified", Now)
                    db.sqlexecute.Parameters.Add("@newId", SqlDbType.Int)
                    db.sqlexecute.Parameters("@newId").Direction = ParameterDirection.Output
                    db.ExecuteSQL(strsql & sqlValues & " select @newId = scope_identity();")
                    Session("ActiveContractProduct") = db.sqlexecute.Parameters("@newId").Value

                    ALog.addRecord(SpendManagementElement.ContractProducts, "New Contract Product", tmpVal)
                    'If Not db.glDBWorkI Is Nothing Then
                    '    ARec.PostVal = tmpDE
                    'End If
                    ARec.ElementDesc = "NEW CON.PRODUCT"
                    ARec.PreVal = ""
                    ARec.PostVal = tmpDE

                    ContractRoutines.AddContractHistory(curUser.Account.accountid, cAccounts.getConnectionString(curUser.Account.accountid), curUser.Employee, SummaryTabs.ContractProduct, Session("ActiveContract"), ARec)
                End If

                ' update the Total Maintenance Value field in the Contract Details table
                Dim strUpdateCPfunction As String = "EXECUTE dbo.UpdateCPAnnualCost @conId, @acv"
                If params.AutoUpdateCVRechargeLive Then
                    strUpdateCPfunction = "EXECUTE dbo.UpdateRechargeCPAnnualCost @conId, @acv"
                End If

                db.sqlexecute.Parameters.Clear()

                db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))
                Dim bVal As Integer = 0
                If params.AutoUpdateAnnualContractValue Then
                    bVal = 1
                End If
                db.sqlexecute.Parameters.AddWithValue("@acv", bVal)
                db.ExecuteSQL(strUpdateCPfunction)

                If params.EnableRecharge Then
                    If hasMaintVal And isAmend Then
                        ' maintenance value changed, so need to update value of cp in all recharge templates
                        Dim rt As New cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, Session("ActiveContract"), cAccounts.getConnectionString(curUser.Account.accountid), params)

                        rt.UpdateCPAnnualCost(Session("ActiveContractProduct"), Double.Parse(txtMaintValue.Text))
                    End If
                End If
            End If

            Dim newrec As SortedList(Of Integer, Object) = ufields.getItemsFromPanel(phCPUFields, udTable)
            ufields.SaveValues(udTable, Session("ActiveContractProduct"), newrec, tables, fields, curUser, elementId:=SpendManagementElement.ContractProducts, record:=tmpDE)

            Session("PreRec") = Nothing

            If chkAddAnother.Checked = True Then
                Session("CP_AddMulti") = "1"
                Response.Redirect("ContractSummary.aspx?cpaction=add&am=1&tab=" & SummaryTabs.ContractProduct, True)
                'ShowCPDetail(True, isAddMulti, isAddAndDefine)
            Else
                ' must refresh the contract details tab as Vendor will now not be changeable
                SetNavActiveState(True)
                ShowCPDetail(False)
                GetCPRecs(contractcategoryid)
                Session("CPAction") = Nothing
                Session("CP_AddandDefine") = Nothing
                Session("CP_AddMulti") = Nothing
                cmdCPUpdate.Enabled = True
            End If
        End Sub

        '   Private Sub UpdateCPUFField(ByRef db As cFWDBConnection, ByRef src_dset As DataSet, ByVal curField As cUserField, ByRef firstentry As Boolean, ByRef ALog As cFWAuditLog, ByRef ARec As cAuditRecord, ByVal isAmend As Boolean)
        '       Dim dbFieldName As String = "UF" & curField.FieldId.ToString
        '       Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
        '       Dim fws As cFWSettings = curUser.UserFWS
        '       Dim uinfo As UserInfo = curUser.currentUser.userInfo

        '       ARec.ElementDesc = curField.FieldName.ToUpper

        '       Select Case curField.FieldType
        '           Case UserFieldType.Character, UserFieldType.Text, UserFieldType.Hyperlink
        '               Dim txt As TextBox = CType(CPUFPanel.FindControl(dbFieldName), TextBox)
        '               If Not txt Is Nothing Then
        '                   If isAmend Then
        '                       If txt.Text <> db.GetFieldValue(src_dset, dbFieldName, 0, 0) Then
        '                           ARec.PreVal = db.GetFieldValue(src_dset, dbFieldName, 0, 0)
        '                           ARec.PostVal = txt.Text
        '                           ALog.AddAuditRec(ARec, firstentry)

        '                           db.SetFieldValue(dbFieldName, txt.Text, "S", firstentry)
        '                           firstentry = False
        '                       End If
        '                   Else
        '                       db.SetFieldValue(dbFieldName, txt.Text, "S", firstentry)
        '                       firstentry = False
        '                   End If
        '               End If
        '           Case UserFieldType.Checkbox
        '               Dim chk As CheckBox = CType(CPUFPanel.FindControl(dbFieldName), CheckBox)
        '               If Not chk Is Nothing Then
        '                   If isAmend Then
        '                       If db.GetFieldValue(src_dset, dbFieldName, 0, 0) = "1" Then
        '                           ARec.PreVal = "CHECKED"
        '                       Else
        '                           ARec.PreVal = "UNCHECKED"
        '                       End If

        '                       If chk.Checked Then
        '                           ARec.PostVal = "CHECKED"
        '                       Else
        '                           ARec.PostVal = "UNCHECKED"
        '                       End If
        '                       ALog.AddAuditRec(ARec, firstentry)

        '                       db.SetFieldValue(dbFieldName, IIf(chk.Checked, 1, 0), "N", firstentry)
        '                       firstentry = False
        '                   Else
        '                       db.SetFieldValue(dbFieldName, IIf(chk.Checked, 1, 0), "N", firstentry)
        '                       firstentry = False
        '                   End If
        '               End If
        '           Case UserFieldType.DateField
        '               Dim txt As TextBox = CType(CPUFPanel.FindControl(dbFieldName), TextBox)
        '               If Not txt Is Nothing Then
        '                   If isAmend Then
        '                       If IsDBNull(db.GetFieldValue(src_dset, dbFieldName, 0, 0)) = False Then
        '                           If db.GetFieldValue(src_dset, dbFieldName, 0, 0) <> txt.Text.Trim Then
        '                               ARec.PreVal = Format(db.GetFieldValue(src_dset, dbFieldName, 0, 0), cDef.DATE_FORMAT)
        '                           Else
        '                               ARec.PreVal = ""
        '                           End If
        '                           ALog.AddAuditRec(ARec, firstentry)

        '                           db.SetFieldValue(dbFieldName, txt.Text, "D", firstentry)
        '                           firstentry = False
        '                       End If
        '                   Else
        '                       If txt.Text.Trim <> "" Then
        '                           db.SetFieldValue(dbFieldName, txt.Text, "D", firstentry)
        '                           firstentry = False
        '                       End If
        '                   End If
        '               End If
        '           Case UserFieldType.DDList
        '               Dim lst As DropDownList = CType(CPUFPanel.FindControl(dbFieldName), DropDownList)
        '               If Not lst Is Nothing Then
        '                   If isAmend Then
        '                       If db.GetFieldValue(src_dset, dbFieldName, 0, 0) <> lst.SelectedItem.Text Then
        '                           If IsDBNull(db.GetFieldValue(src_dset, dbFieldName, 0, 0)) Then
        '                               ARec.PreVal = db.GetFieldValue(src_dset, dbFieldName, 0, 0)
        '                           Else
        '                               ARec.PreVal = ""
        '                           End If

        '                           ARec.PostVal = lst.SelectedItem.Text
        '                           ALog.AddAuditRec(ARec, firstentry)

        '                           db.SetFieldValue(dbFieldName, lst.SelectedItem.Text, "S", firstentry)
        '                           firstentry = False
        '                       End If
        '                   Else
        '                       db.SetFieldValue(dbFieldName, lst.SelectedItem.Text, "S", firstentry)
        '                       firstentry = False
        '                   End If
        '               End If
        '           Case UserFieldType.Float, UserFieldType.Number
        '               Dim txt As TextBox = CType(CPUFPanel.FindControl(dbFieldName), TextBox)
        '               If Not txt Is Nothing Then
        '                   If isAmend Then
        '                       If txt.Text <> db.GetFieldValue(src_dset, dbFieldName, 0, 0) Then
        '                           ARec.PreVal = db.GetFieldValue(src_dset, dbFieldName, 0, 0)
        '                           ARec.PostVal = txt.Text
        '                           ALog.AddAuditRec(ARec, firstentry)

        '                           db.SetFieldValue(dbFieldName, txt.Text, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   Else
        '                       db.SetFieldValue(dbFieldName, txt.Text, "N", firstentry)
        '                       firstentry = False
        '                   End If
        '               End If

        '           Case UserFieldType.RechargeAcc_Code
        'Dim rca As New FWClasses.cRechargeAccountCodes(uinfo, fws)
        '               If rca.Count > cDef.UF_MAXCOUNT Then
        '                   ' displays text box for look up
        '                   Dim txt As TextBox = CType(CPUFPanel.FindControl(dbFieldName & "_TXT"), TextBox)
        '                   Dim txt_id As HiddenField = CType(CPUFPanel.FindControl(dbFieldName), HiddenField)
        '                   If txt_id.Value <> "" Then
        '                       If isAmend Then
        '                           If txt_id.Value <> CStr(db.GetFieldValue(src_dset, dbFieldName, 0, 0)) Then
        '                               If db.GetFieldValue(src_dset, dbFieldName, 0, 0) > 0 Then
        '                                   ARec.PreVal = rca.GetCodeById(CInt(db.GetFieldValue(src_dset, dbFieldName, 0, 0))).AccountCode
        '                               Else
        '                                   ARec.PreVal = ""
        '                               End If

        '                               ARec.PostVal = rca.GetCodeById(CInt(txt_id.Value)).AccountCode
        '                               ALog.AddAuditRec(ARec, firstentry)

        '                               db.SetFieldValue(dbFieldName, txt_id.Value, "N", firstentry)
        '                               firstentry = False
        '                           End If
        '                       Else
        '                           db.SetFieldValue(dbFieldName, txt_id.Value, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   End If
        '               Else
        '                   Dim lst As DropDownList = CType(CPUFPanel.FindControl(dbFieldName), DropDownList)

        '                   If Not lst Is Nothing Then
        '                       If isAmend Then
        '                           If db.GetFieldValue(src_dset, dbFieldName, 0, 0) > 0 Then
        '                               ARec.PreVal = rca.GetCodeById(db.GetFieldValue(src_dset, dbFieldName, 0, 0)).CodeId
        '                           Else
        '                               ARec.PreVal = ""
        '                           End If

        '                           ARec.PostVal = lst.SelectedItem.Text
        '                           ALog.AddAuditRec(ARec, firstentry)

        '                           db.SetFieldValue(dbFieldName, lst.SelectedItem.Value, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   Else
        '                       db.SetFieldValue(dbFieldName, lst.SelectedItem.Value, "N", firstentry)
        '                       firstentry = False
        '                   End If
        '               End If
        '           Case UserFieldType.RechargeClient_Ref
        'Dim rcc As New FWClasses.cRechargeClientList(uinfo, fws)
        '               If rcc.Count > cDef.UF_MAXCOUNT Then
        '                   ' displays text box for look up
        '                   Dim txt As TextBox = CType(CPUFPanel.FindControl(dbFieldName & "_TXT"), TextBox)
        '                   Dim txt_id As HiddenField = CType(CPUFPanel.FindControl(dbFieldName), HiddenField)
        '                   If txt_id.Value <> "" Then
        '                       If isAmend Then
        '                           If txt_id.Value <> CStr(db.GetFieldValue(src_dset, dbFieldName, 0, 0)) Then
        '                               If db.GetFieldValue(src_dset, dbFieldName, 0, 0) > 0 Then
        '                                   ARec.PreVal = rcc.GetClientById(CInt(db.GetFieldValue(src_dset, dbFieldName, 0, 0))).ClientName
        '                               Else
        '                                   ARec.PreVal = ""
        '                               End If

        '                               ARec.PostVal = rcc.GetClientById(CInt(txt_id.Value)).ClientName
        '                               ALog.AddAuditRec(ARec, firstentry)

        '                               db.SetFieldValue(dbFieldName, txt_id.Value, "N", firstentry)
        '                               firstentry = False
        '                           End If
        '                       Else
        '                           db.SetFieldValue(dbFieldName, txt_id.Value, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   End If
        '               Else
        '                   Dim lst As DropDownList = CType(CPUFPanel.FindControl(dbFieldName), DropDownList)

        '                   If Not lst Is Nothing Then
        '                       If isAmend Then
        '                           If db.GetFieldValue(src_dset, dbFieldName, 0, 0) > 0 Then
        '                               ARec.PreVal = rcc.GetClientById(db.GetFieldValue(src_dset, dbFieldName, 0, 0)).ClientName
        '                           Else
        '                               ARec.PreVal = ""
        '                           End If

        '                           ARec.PostVal = lst.SelectedItem.Text
        '                           ALog.AddAuditRec(ARec, firstentry)

        '                           db.SetFieldValue(dbFieldName, lst.SelectedItem.Value, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   Else
        '                       db.SetFieldValue(dbFieldName, lst.SelectedItem.Value, "N", firstentry)
        '                       firstentry = False
        '                   End If
        '               End If

        '           Case UserFieldType.Site_Ref
        '               Dim sites As New cSites(fws, uinfo)
        '               If sites.Count > cDef.UF_MAXCOUNT Then
        '                   ' displays text box for look up
        '                   Dim txt As TextBox = CType(CPUFPanel.FindControl(dbFieldName & "_TXT"), TextBox)
        '                   Dim txt_id As HiddenField = CType(CPUFPanel.FindControl(dbFieldName), HiddenField)
        '                   If txt_id.Value <> "" Then
        '                       If isAmend Then
        '                           If txt_id.Value <> CStr(db.GetFieldValue(src_dset, dbFieldName, 0, 0)) Then
        '                               If db.GetFieldValue(src_dset, dbFieldName, 0, 0) > 0 Then
        '                                   ARec.PreVal = sites.GetSiteById(CInt(db.GetFieldValue(src_dset, dbFieldName, 0, 0))).SiteCode
        '                               Else
        '                                   ARec.PreVal = ""
        '                               End If

        '                               ARec.PostVal = sites.GetSiteById(CInt(txt_id.Value)).SiteCode
        '                               ALog.AddAuditRec(ARec, firstentry)

        '                               db.SetFieldValue(dbFieldName, txt_id.Value, "N", firstentry)
        '                               firstentry = False
        '                           End If
        '                       Else
        '                           db.SetFieldValue(dbFieldName, txt_id.Value, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   End If
        '               Else
        '                   Dim lst As DropDownList = CType(CPUFPanel.FindControl(dbFieldName), DropDownList)

        '                   If Not lst Is Nothing Then
        '                       If isAmend Then
        '                           If db.GetFieldValue(src_dset, dbFieldName, 0, 0) > 0 Then
        '                               ARec.PreVal = sites.GetSiteById(db.GetFieldValue(src_dset, dbFieldName, 0, 0)).SiteCode
        '                           Else
        '                               ARec.PreVal = ""
        '                           End If

        '                           ARec.PostVal = lst.SelectedItem.Text
        '                           ALog.AddAuditRec(ARec, firstentry)

        '                           db.SetFieldValue(dbFieldName, lst.SelectedItem.Value, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   Else
        '                       db.SetFieldValue(dbFieldName, lst.SelectedItem.Value, "N", firstentry)
        '                       firstentry = False
        '                   End If
        '               End If

        '           Case UserFieldType.StaffName_Ref
        'Dim emps As New cEmployees(fws.MetabaseCustomerId)
        '               If emps.Count > cDef.UF_MAXCOUNT Then
        '                   ' displays text box for look up
        '                   Dim txt As TextBox = CType(CPUFPanel.FindControl(dbFieldName & "_TXT"), TextBox)
        '                   Dim txt_id As HiddenField = CType(CPUFPanel.FindControl(dbFieldName), HiddenField)
        '                   If txt_id.Value <> "" Then
        '                       If isAmend Then
        '                           If txt_id.Value <> CStr(db.GetFieldValue(src_dset, dbFieldName, 0, 0)) Then
        '                               If db.GetFieldValue(src_dset, dbFieldName, 0, 0) > 0 Then
        '                                   ARec.PreVal = emps.GetEmployeeById(CInt(db.GetFieldValue(src_dset, dbFieldName, 0, 0))).EmployeeName
        '                               Else
        '                                   ARec.PreVal = ""
        '                               End If

        '                               ARec.PostVal = emps.GetEmployeeById(CInt(txt_id.Value)).EmployeeName
        '                               ALog.AddAuditRec(ARec, firstentry)

        '                               db.SetFieldValue(dbFieldName, txt_id.Value, "N", firstentry)
        '                               firstentry = False
        '                           End If
        '                       Else
        '                           db.SetFieldValue(dbFieldName, txt_id.Value, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   End If
        '               Else
        '                   Dim lst As DropDownList = CType(CPUFPanel.FindControl(dbFieldName), DropDownList)

        '                   If Not lst Is Nothing Then
        '                       If isAmend Then
        '                           If db.GetFieldValue(src_dset, dbFieldName, 0, 0) > 0 Then
        '                               ARec.PreVal = emps.GetEmployeeById(db.GetFieldValue(src_dset, dbFieldName, 0, 0)).EmployeeName
        '                           Else
        '                               ARec.PreVal = ""
        '                           End If

        '                           ARec.PostVal = lst.SelectedItem.Text
        '                           ALog.AddAuditRec(ARec, firstentry)

        '                           db.SetFieldValue(dbFieldName, lst.SelectedItem.Value, "N", firstentry)
        '                           firstentry = False
        '                       End If
        '                   Else
        '                       db.SetFieldValue(dbFieldName, lst.SelectedItem.Value, "N", firstentry)
        '                       firstentry = False
        '                   End If
        '               End If
        '           Case Else

        '       End Select
        '   End Sub

        <WebMethod(enableSession:=True)> _
        Public Shared Function DeleteContractProduct(ByVal CPid As Integer) As String
            Dim appinfo As HttpApplication = CType(HttpContext.Current.ApplicationInstance, HttpApplication)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New cFWDBConnection
            Dim ARec As New cAuditRecord
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim curContractId As Integer = 0 'appinfo.Session("ActiveContract")
            Dim ALog As New cAuditLog(curUser.AccountID, curUser.EmployeeID)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            db.DBOpen(fws, False)

            If SMRoutines.CheckRefIntegrity(db, "contract_productdetails", CPid) = False Then
                ' remove entries in the Contract - Product-History table for record
                db.FWDb("R", "contract_productdetails", "ContractProductId", CPid, "", "", "", "", "", "", "", "", "", "")
                curContractId = CInt(db.FWDbFindVal("contractId", 1))

                If db.FWDbFlag = True Then
                    db.FWDb("R2", "productDetails", "productId", db.FWDbFindVal("ProductId", 1), "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("R", "contract_details", "contractId", curContractId, "", "", "", "", "", "", "", "", "", "")
                    If db.FWDbFlag = True Then
                        Dim tmpKey As String
                        tmpKey = db.FWDbFindVal("contractKey", 1)
                        If Trim(tmpKey) <> "" Then
                            ARec.ContractNumber = tmpKey
                        Else
                            ARec.ContractNumber = db.FWDbFindVal("contractNumber", 1)
                        End If
                    End If

                    db.FWDb("D", "contract_productdetails_recharge", "contractProductId", CPid, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "contract_productdetails_calloff", "contractProductId", CPid, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "contract_producthistory", "contractProductId", CPid, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "contract_productplatforms", "contractProductId", CPid, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "contract_productdetails", "contractProductId", CPid, "", "", "", "", "", "", "", "", "", "")

                    ' log deletion in the audit log
                    ARec.Action = cFWAuditLog.AUDIT_DEL
                    ARec.ContractNumber = ""
                    ARec.DataElementDesc = "CONTRACT PRODUCTS"
                    ARec.ElementDesc = IIf(db.FWDb2Flag = True, db.FWDbFindVal("productName", 2), "")
                    ARec.PreVal = ""
                    ARec.PostVal = ""

                    ALog.deleteRecord(SpendManagementElement.ContractProducts, CPid, ARec.ElementDesc)

                    ContractRoutines.AddContractHistory(curUser.Account.accountid, cAccounts.getConnectionString(curUser.Account.accountid), curUser.Employee, SummaryTabs.ContractProduct, curContractId, ARec)

                    ' update the Total Maintenance Value field in the Contract Details table
                    Dim strUpdateCPfunction As String = "EXECUTE dbo.UpdateCPAnnualCost @conId, @acv"
                    If params.EnableRecharge Then
                        If params.AutoUpdateCVRechargeLive Then
                            strUpdateCPfunction = "EXECUTE dbo.UpdateRechargeCPAnnualCost @conId, @acv"
                        End If
                    End If
                    db.AddDBParam("conId", curContractId, True)
                    db.AddDBParam("acv", IIf(fws.glAutoUpdateCV, 1, 0), False)
                    db.ExecuteSQL(strUpdateCPfunction)

                    db.DBClose()
                End If
            Else
                Return "ERROR! Cannot perform action as entity is currently assigned."
                Exit Function
            End If
            Return "OK"
        End Function

        Private Sub GetCPNotes(ByVal curUser As CurrentUser, ByVal cpId As Integer)
            Dim prodID As Integer
            Dim prodText As String
            Dim sql As New System.Text.StringBuilder
            Dim db As New cFWDBConnection
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            db.DBOpen(fws, False)

            'db.FWDb("R2", "contract_productdetails", "Contract-Product Id", conProdID)
            sql.Append("SELECT [productDetails].[ProductName],[contract_productdetails].[productId] FROM [contract_productdetails] ")
            sql.Append("INNER JOIN [productDetails] ON [productDetails].[productId] = [contract_productdetails].[productId] ")
            sql.Append("WHERE [contractProductId] = @cpID")
            db.AddDBParam("cpID", cpId, True)
            db.RunSQL(sql.ToString, db.glDBWorkL, False, "", False)

            If db.glNumRowsReturned > 0 Then
                prodID = db.GetFieldValue(db.glDBWorkL, "ProductId", 0, 0)
                prodText = db.GetFieldValue(db.glDBWorkL, "ProductName", 0, 0)
            Else
                prodID = 0
                prodText = "**Unknown Product**"
            End If

            db.DBClose()

            db = Nothing

            If prodID = 0 Then
                ' no current note
                Exit Sub
            End If
            Session("NoteReturnURL") = "ContractSummary.aspx?tab=2&id=" & Trim(Session("ActiveContract"))

            Response.Redirect("ViewNotes.aspx?notetype=Product&id=-1&productid=" & prodID.ToString.Trim & "&item=" & Replace(Replace(prodText.Trim, "&", "%26"), vbNewLine, ""), True)
        End Sub

        Private Sub GetContractProducts(ByVal db As cFWDBConnection, Optional ByVal UFieldlist As String = "")
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim strHTML As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim firstrec As Boolean
            Dim reqGrpUFields As String = ""
            Dim GrpUFields(100) As UFCollection
            Dim cCOLL As cCPFieldInfo
            Dim CP_Count As Integer = 0
            Dim sql As New System.Text.StringBuilder
            Dim subacss As New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subacss.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subacss.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim useCPStatus As Boolean = False
            Dim employees As New cEmployees(curUser.AccountID)

            If params.RechargeSettings.CP_Delete_Action = 1 Then
                panelCPViewStatus.Visible = True
                useCPStatus = True

                If Session("CPStatus") Is Nothing Then
                    Session("CPStatus") = "0"
                End If
                rdoCPStatus.ClearSelection()
                rdoCPStatus.Items.FindByValue(Session("CPStatus")).Selected = True
            End If

            firstrec = True

            sql.Append("SELECT COUNT(*) AS [ConProdCount] ")
            sql.Append(" FROM [contract_productdetails]")
            sql.Append(" WHERE [contract_productdetails].[contractId] = @conId")
            db.AddDBParam("conId", Session("ActiveContract"), True)
            db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)

            CP_Count = db.GetFieldValue(db.glDBWorkD, "ConProdCount", 0, 0)
            ConProd_Count = CP_Count

            cCOLL = New cCPFieldInfo(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID), curUser.EmployeeID, Session("ActiveContract"))

            If CP_Count > cDef.MAX_CP_DISPLAY Then
                CP_FilterPanel.Visible = True

                If Session("CP_Filter") Is Nothing Then
                    Session("CP_Filter") = ""
                Else
                    txtFilter.Text = Session("CP_Filter")
                End If
            Else
                CP_FilterPanel.Visible = False
            End If

            sql = New System.Text.StringBuilder
            Dim whereAND As String = ""
            Dim UF1_Key As String = ""
            Dim UF2_Key As String = ""
            Dim moreJoins As New System.Collections.SortedList
            Dim UF_DisplayFields As New System.Collections.SortedList

            sql.Append("SELECT ")
            If CP_Count > cDef.MAX_CP_DISPLAY And Session("CP_Filter") = "" Then
                sql.Append("TOP " & cDef.MAX_CP_DISPLAY.ToString & " ")
            End If
            sql.Append("[contractProductId],[contractId],[contract_productdetails].[productId], ISNULL([currencyId],0) AS [currencyId],[codes_salestax].[description],[createdDate],")
            sql.Append("ISNULL([Quantity],0) AS [Quantity],ISNULL([codes_units].[description],'') AS [unitDescription],ISNULL([unitCost],0) AS [unitCost],ISNULL([projectedSaving],0) AS [projectedSaving],[productDetails].[ProductName],ISNULL([productValue],0) AS [productValue],ISNULL([pricePaid],0) AS [pricePaid],ISNULL([maintenanceValue],0) AS [maintenanceValue],ISNULL([maintenancePercent],0) AS [maintenancePercent],[archiveStatus]<**UF**><**UFDISPFIELDS**>" & vbNewLine)
            sql.Append("FROM [contract_productdetails] " & vbNewLine)
            sql.Append("LEFT OUTER JOIN [codes_units] ON [contract_productdetails].[unitId] = [codes_units].[unitId]" & vbNewLine)
            sql.Append("LEFT OUTER JOIN [codes_salestax] ON [contract_productdetails].[salesTaxRate] = [codes_salestax].[salesTaxId]" & vbNewLine)
            sql.Append("LEFT OUTER JOIN [productDetails] ON [contract_productdetails].[productId] = [productDetails].[ProductId]" & vbNewLine)
            sql.Append("<**JOINS**>")
            sql.Append("WHERE ")

            If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                Select Case cCOLL.CPFieldInfoItem.CP_UF1_Type
                    Case UserFieldType.RechargeAcc_Code
                        UF1_Key = "ACCOUNT"
                        moreJoins.Add(UF1_Key, "LEFT JOIN [codes_accountcodes] ON [codes_accountcodes].[codeId] = [contract_productdetails].[UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString & "] " & vbNewLine)
                        UF_DisplayFields.Add(UF1_Key, "[codes_accountcodes].[accountCode]")

                    Case UserFieldType.RechargeClient_Ref
                        UF1_Key = "CLIENT"
                        moreJoins.Add(UF1_Key, "LEFT JOIN [codes_rechargeentity] ON [codes_rechargeentity].[entityId] = [contractProductDetails].[UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString & "] " & vbNewLine)
                        UF_DisplayFields.Add(UF1_Key, "[codes_rechargeentity].[Name]")

                    Case UserFieldType.Site_Ref
                        UF1_Key = "SITE"
                        moreJoins.Add(UF1_Key, "LEFT JOIN [codes_sites] ON [codes_sites].[siteId] = [contract_productdetails].[UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString & "] " & vbNewLine)
                        UF_DisplayFields.Add(UF1_Key, "[codes_sites].[siteCode]")

                    Case UserFieldType.StaffName_Ref
                        UF1_Key = "STAFF"
                        moreJoins.Add(UF1_Key, "LEFT JOIN [employees] ON [employees].[employeeId] = [contract_productdetails].[UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString & "] " & vbNewLine)
                        UF_DisplayFields.Add(UF1_Key, "[employees].firstname + employees.surname AS employeeName")

                    Case Else
                        UF1_Key = ""
                End Select
            End If

            If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                Select Case cCOLL.CPFieldInfoItem.CP_UF2_Type
                    Case UserFieldType.RechargeAcc_Code
                        If moreJoins.ContainsKey("ACCOUNT") = False Then
                            UF2_Key = "ACCOUNT"
                            moreJoins.Add(UF2_Key, "LEFT JOIN [codes_accountcodes] ON [codes_accountcodes].[codeId] = [contract-productdetails].[UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString & "] " & vbNewLine)
                            UF_DisplayFields.Add(UF2_Key, "[codes_accountcodes].[accountCode]")
                        End If

                    Case UserFieldType.RechargeClient_Ref
                        If moreJoins.ContainsKey("CLIENT") = False Then
                            UF2_Key = "CLIENT"
                            moreJoins.Add(UF2_Key, "LEFT JOIN [codes_rechargeentity] ON [codes_rechargeentity].[Entity Id] = [contract_productdetails].[UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString & "] " & vbNewLine)
                            UF_DisplayFields.Add(UF2_Key, "[codes_rechargeentity].[Name]")
                        End If

                    Case UserFieldType.Site_Ref
                        If moreJoins.ContainsKey("SITE") = False Then
                            UF2_Key = "SITE"
                            moreJoins.Add(UF2_Key, "LEFT JOIN [codes_sites] ON [codes_sites].[siteId] = [contract_productdetails].[UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString & "] " & vbNewLine)
                            UF_DisplayFields.Add(UF2_Key, "[codes_sites].[siteCode]")
                        End If

                    Case UserFieldType.StaffName_Ref
                        If moreJoins.ContainsKey("STAFF") = False Then
                            UF2_Key = "STAFF"
                            moreJoins.Add(UF2_Key, "LEFT JOIN [employees] ON [employees].[employeeId] = [contract_productdetails].[UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString & "] " & vbNewLine)
                            UF_DisplayFields.Add(UF2_Key, "[employees].firstname + employees.surname AS employeeName")
                        End If

                    Case Else
                        UF2_Key = ""
                End Select
            End If

            If CP_Count > cDef.MAX_CP_DISPLAY Then
                If Session("CP_Filter") <> "" And Session("CP_Filter") <> "*" Then
                    sql.Append(" (")
                    sql.Append("[productName] LIKE '%' + @CP_Filter + '%'")

                    If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                        Select Case cCOLL.CPFieldInfoItem.CP_UF1_Type
                            Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref
                                sql.Append(" OR " & UF_DisplayFields(UF1_Key) & " LIKE '%' + @CP_Filter + '%'")
                            Case Else
                                sql.Append(" OR [UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim & "] LIKE '%' + @CP_Filter + '%'")
                        End Select

                    End If

                    If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                        Select Case cCOLL.CPFieldInfoItem.CP_UF2_Type
                            Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref
                                sql.Append(" OR " & UF_DisplayFields(UF2_Key) & " LIKE '%' + @CP_Filter + '%'")
                            Case Else
                                sql.Append(" OR [UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim & "] LIKE '%' + @CP_Filter + '%'")
                        End Select
                    End If

                    sql.Append(")")
                    whereAND = " AND "
                End If
            End If

            If useCPStatus Then
                sql.Append(whereAND & "[archiveStatus] = @CPstatus ")
                whereAND = " AND "
            End If

            sql.Append(whereAND & "[contractId] = @conId " & vbNewLine & " ")
            sql.Append("ORDER BY ")

            Dim comma As String = ""

            If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                Select Case cCOLL.CPFieldInfoItem.CP_UF1_Type
                    Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref
                        sql.Append(UF_DisplayFields(UF1_Key))
                        comma = ","
                    Case UserFieldType.Text
                        ' can't sort by a text field
                    Case Else
                        sql.Append(" [UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim & "]")
                        comma = ","
                End Select
            End If

            If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                Select Case cCOLL.CPFieldInfoItem.CP_UF2_Type
                    Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref
                        sql.Append(comma & UF_DisplayFields(UF2_Key))
                        comma = ","
                    Case UserFieldType.Text
                        ' can't sort by a text field
                    Case Else
                        sql.Append(comma & " [UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim & "]")
                        comma = ","
                End Select
            End If
            sql.Append(comma & " [productName]")

            sql.Replace("<**UF**>", UFieldlist)

            Dim joinSQL As New System.Text.StringBuilder
            Dim x As Integer
            For x = 0 To moreJoins.Count - 1
                joinSQL.Append(moreJoins.GetByIndex(x))
            Next
            sql.Replace("<**JOINS**>", joinSQL.ToString)

            joinSQL = New System.Text.StringBuilder
            For x = 0 To UF_DisplayFields.Count - 1
                joinSQL.Append("," & UF_DisplayFields.GetByIndex(x))
            Next

            sql.Replace("<**UFDISPFIELDS**>", joinSQL.ToString)

            db.AddDBParam("conId", Session("ActiveContract"), True)
            If useCPStatus Then
                db.AddDBParam("CPstatus", Session("CPStatus"), False)
            End If
            If CP_Count > cDef.MAX_CP_DISPLAY Then
                db.AddDBParam("CP_Filter", Session("CP_Filter"), False)
            End If
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)
            ConProd_Count = db.glNumRowsReturned

            If CP_Count > cDef.MAX_CP_DISPLAY And Session("CP_Filter") = "" Then
                strHTML.Append("<div class=""inputpaneltitle"">** TOP " & cDef.MAX_CP_DISPLAY.ToString & " displayed only - use Filter or '*' to display all</div>" & vbNewLine)
            End If

            Dim CPTable As New Table
            CPTable.CssClass = "datatbl"
            Dim CPHRow As New TableHeaderRow
            Dim CPHCell As New TableHeaderCell
            Dim CPHImg As New System.Web.UI.WebControls.Image

            If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False) Then
                CPHImg.ImageUrl = "~/icons/edit.gif"
                CPHCell.Controls.Add(CPHImg)
                CPHCell.Attributes.Add("width", "30px")
                CPHRow.Cells.Add(CPHCell)
            End If

            If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractProducts, False) Then
                CPHCell = New TableHeaderCell
                CPHImg = New System.Web.UI.WebControls.Image
                CPHImg.ImageUrl = "~/icons/delete2.gif"
                CPHCell.Controls.Add(CPHImg)
                CPHCell.Attributes.Add("width", "30px")
                CPHRow.Cells.Add(CPHCell)
            End If

            If useCPStatus Then
                CPHCell = New TableHeaderCell
                CPHImg = New System.Web.UI.WebControls.Image
                CPHImg.ImageUrl = "~/icons/16/plain/lock_open.gif"
                CPHCell.Attributes.Add("width", "30px")
                CPHCell.Controls.Add(CPHImg)
                CPHRow.Cells.Add(CPHCell)
            End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProducts, False) And curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False) = False Then
                CPHCell = New TableHeaderCell
                CPHImg = New System.Web.UI.WebControls.Image
                With CPHImg
                    .ImageUrl = "~/icons/16/plain/view.png"
                End With
                CPHCell.Attributes.Add("width", "30px")
                CPHCell.Controls.Add(CPHImg)
                CPHRow.Cells.Add(CPHCell)
            End If

            CPHCell = New TableHeaderCell
            CPHImg = New System.Web.UI.WebControls.Image
            CPHImg.ImageUrl = "~/icons/16/plain/document_attachment.png"
            CPHCell.Attributes.Add("width", "30px")
            CPHCell.Controls.Add(CPHImg)
            CPHRow.Cells.Add(CPHCell)

            CPHCell = New TableHeaderCell
            CPHImg = New System.Web.UI.WebControls.Image
            CPHImg.ImageUrl = "~/icons/16/plain/clipboard.png"
            CPHCell.Attributes.Add("width", "30px")
            CPHCell.Controls.Add(CPHImg)
            CPHRow.Cells.Add(CPHCell)

            CPHCell = New TableHeaderCell
            CPHImg = New System.Web.UI.WebControls.Image
            CPHImg.ImageUrl = "~/icons/16/plain/preferences.png"
            CPHCell.Attributes.Add("width", "30px")
            CPHCell.Controls.Add(CPHImg)
            CPHRow.Cells.Add(CPHCell)

            CPHCell = New TableHeaderCell
            CPHCell.Text = "Product Name"
            CPHRow.Cells.Add(CPHCell)

            CPHCell = New TableHeaderCell
            CPHCell.Text = "Product Value"
            CPHRow.Cells.Add(CPHCell)

            CPHCell = New TableHeaderCell
            CPHCell.Text = "Annual Cost"
            CPHRow.Cells.Add(CPHCell)

            CPHCell = New TableHeaderCell
            CPHCell.Text = "Next Period Annual Cost"
            CPHRow.Cells.Add(CPHCell)

            CPHCell = New TableHeaderCell
            CPHCell.Text = "Quantity"
            CPHRow.Cells.Add(CPHCell)

            CPHCell = New TableHeaderCell
            CPHCell.Text = "Unit Description"
            CPHRow.Cells.Add(CPHCell)

            If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                CPHCell = New TableHeaderCell
                CPHCell.Text = cCOLL.CPFieldInfoItem.CP_UF1_Title
                CPHRow.Cells.Add(CPHCell)
            End If

            If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                CPHCell = New TableHeaderCell
                CPHCell.Text = cCOLL.CPFieldInfoItem.CP_UF2_Title
                CPHRow.Cells.Add(CPHCell)
            End If

            CPTable.Rows.Add(CPHRow)

            Dim rowClass As String = "row1"
            Dim rowalt As Boolean = False
            Dim cntlSeq As Integer = 0
            Dim hasdata As Boolean = False
            Dim viewFinancial = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False)

            For Each drow In db.glDBWorkA.Tables(0).Rows
                rowalt = (rowalt Xor True)
                If rowalt Then
                    rowClass = "row1"
                Else
                    rowClass = "row2"
                End If

                cntlSeq += 1
                Dim CPRow As New TableRow
                Dim CPCell As New TableCell

                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False) Then
                    Dim litEdit As New Literal
                    litEdit.Text = "<a onmouseover=""window.status='Edit/View this Product';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractProduct & "&cpaction=edit&cpid=" & drow.Item("contractProductId") & """ title=""Edit/View this Product""><img src=""./icons/edit.gif"" /></a>"
                    CPCell.Controls.Add(litEdit)

                    CPCell.CssClass = rowClass
                    CPRow.Cells.Add(CPCell)
                End If

                CPCell = New TableCell
                If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ContractProducts, False) Then
                    Dim litDelete As New Literal
                    litDelete.Text = "<a onmouseover=""window.status='Delete this product';return true;"" onmouseout=""window.status='Done';"" href=""javascript:DeleteConProd(" & drow.Item("contractProductId") & ");""><img src=""./icons/delete2.gif"" /></a>"
                    CPCell.Controls.Add(litDelete)

                    CPCell.CssClass = rowClass
                    CPRow.Cells.Add(CPCell)
                End If

                If useCPStatus Then
                    CPCell = New TableCell
                    Dim cpArchive As New System.Web.UI.WebControls.Image
                    Dim cpSwitchStatus As String
                    With cpArchive
                        If drow.Item("archiveStatus") = 0 Then
                            .ImageUrl = "~/icons/16/plain/lock_open.gif"
                            cpSwitchStatus = "1"
                            .Attributes.Add("onmouseover", "window.status='Archive the active contract product';return true;")
                        Else
                            .ImageUrl = "~/icons/16/plain/lock.gif"
                            cpSwitchStatus = "0"
                            .Attributes.Add("onmouseover", "window.status='Reinstate the archived contract product';return true;")
                        End If
                        .ToolTip = "Archive the active contract product"
                        .Attributes.Add("onmouseout", "window.status='Done';")
                        .Attributes.Add("onclick", "SetCPArchiveStatus(" & Session("ActiveContract") & "," & CStr(drow.Item("contractProductId")) & "," & cpSwitchStatus & ");")
                        .Attributes.Add("style", "cursor: hand;")
                    End With
                    CPCell.Controls.Add(cpArchive)
                    CPCell.CssClass = rowClass
                    CPRow.Cells.Add(CPCell)
                End If

                CPCell = New TableCell
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProducts, False) And curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False) = False Then
                    Dim CPViewImg As New System.Web.UI.WebControls.Image
                    With CPViewImg
                        .ImageUrl = "~/icons/16/plain/view.gif"
                        .ToolTip = "Expand full details for this contract details"
                        .Attributes.Add("onmouseover", "window.status='Expand full details for this contract details';return true;")
                        .Attributes.Add("onmouseout", "window.status='Done';")
                        .Attributes.Add("onclick", "window.location.href='ContractSummary.aspx?tab=" & SummaryTabs.ContractProduct & "&cpaction=edit&cpid=" & drow.Item("contractProductId") & "';") ' "GetCPDetail(" & CStr(drow.Item("contractProductId")) & ");")
                        .Attributes.Add("style", "cursor: hand;")
                    End With
                    CPCell.Controls.Add(CPViewImg)

                    CPCell.CssClass = rowClass
                    CPRow.Cells.Add(CPCell)
                End If
                ' contract product notes
                CPCell = New TableCell
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductNotes, False) Then
                    Dim CPNotesImg As New System.Web.UI.WebControls.Image
                    With CPNotesImg
                        .ImageUrl = "~/icons/16/plain/document_attachment.gif"
                        .ToolTip = "Open contract product notes"
                        .Attributes.Add("onmouseover", "window.status='Open contract product notes';return true;")
                        .Attributes.Add("onmouseout", "window.status='Done';")
                        .Attributes.Add("onclick", "window.location.href='ContractSummary.aspx?tab=" & SummaryTabs.ContractProduct & "&cpaction=cpnotes&cpid=" & drow.Item("contractProductId") & "';")
                        .Attributes.Add("style", "cursor: hand;")
                    End With
                    CPCell.Controls.Add(CPNotesImg)
                End If
                CPCell.CssClass = rowClass
                CPRow.Cells.Add(CPCell)

                ' product information
                CPCell = New TableCell
                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False) Then
                    Dim CPInfoImg As New System.Web.UI.WebControls.Image
                    With CPInfoImg
                        .AlternateText = "Information"
                        .ImageUrl = "~/icons/16/plain/clipboard.gif"
                        .Attributes.Add("onmouseover", "window.status='View Product Information';return true;")
                        .Attributes.Add("onmouseout", "window.status='Done';")
                        .Attributes.Add("onclick", "window.open('ContractProductInfo.aspx?cpid=" & CStr(drow.Item("contractProductId")).Trim & "');")
                        .Attributes.Add("style", "cursor: hand;")
                        .ToolTip = "View Product Information"
                    End With
                    CPCell.Controls.Add(CPInfoImg)
                End If
                CPCell.CssClass = rowClass
                CPRow.Cells.Add(CPCell)

                CPCell = New TableCell
                CPCell.CssClass = rowClass
                Dim taskImg As New System.Web.UI.WebControls.Image
                taskImg.ID = "task" & drow.Item("contractProductId")

                Dim retURL As String = "~/ContractSummary.aspx?tab=" & SummaryTabs.ContractProduct & "&id=" & Session("ActiveContract")
                retURL = Server.UrlEncode(retURL)

                If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False) Then
                    With taskImg
                        .AlternateText = "Task Options"
                        .ImageUrl = "~/icons/16/plain/preferences.gif"
                        .Attributes.Add("onmouseover", "window.status='View Task Options for Contract Product';return true;")
                        .Attributes.Add("onmouseout", "window.status='Done';")
                        '.Attributes.Add("onclick", "window.location.href='tasks/ViewTask.aspx?tid=0&rid=" & CStr(drow("Contract-Product Id")) & "&rtid=" & AppAreas.CONTRACT_PRODUCTS & "&ret=" & retURL & "';")
                        .Attributes.Add("onclick", "showCPTaskOptions(event,'" & taskImg.ClientID & "'," & drow.Item("contractProductId") & ",'" & retURL & "');")
                        .Attributes.Add("style", "cursor: hand;")
                        .ToolTip = "View Task Options for Contract Product"
                    End With
                    CPCell.Controls.Add(taskImg)
                End If
                CPRow.Cells.Add(CPCell)

                CPCell = New TableCell
                CPCell.CssClass = rowClass
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, False) Then
                    Dim hyp As New HyperLink
                    hyp.ID = "hypProduct" & CStr(drow("productId")) & "_" & cntlSeq.ToString
                    hyp.Text = drow("productName")
                    hyp.Attributes.Add("style", "cursor: hand; text-decoration: underline;")
                    hyp.Attributes.Add("onclick", "javascript:window.location.href='ProductDetails.aspx?action=edit&id=" & CStr(drow("productId")) & "&item=" & drow.Item("productName") & "&ret=1&cid=" & Session("ActiveContract") & "';")
                    CPCell.Controls.Add(hyp)
                Else
                    CPCell.Text = CStr(drow("productName"))
                End If

                CPRow.Cells.Add(CPCell)

                Dim currencyId As Integer = drow.Item("currencyId")
                Dim curCurrency As cCurrency = currency.getCurrencyById(currencyId)

                Dim tmpTYM, tmpPV As String
                tmpPV = ""
                tmpTYM = ""
                If params.BaseCurrency.HasValue Then
                    If drow.Item("CurrencyId") <> params.BaseCurrency.Value Then
                        If IsDBNull(drow.Item("createdDate")) = False Then
                            tmpPV = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(currencyId, drow.Item("productValue"), drow.Item("createdDate")), curCurrency, False)
                            tmpTYM = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(currencyId, drow("maintenanceValue"), drow.Item("createdDate")), curCurrency, False)
                        Else
                            tmpPV = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(currencyId, drow.Item("productValue"), Nothing), curCurrency, False)
                            tmpTYM = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(currencyId, drow("maintenanceValue"), Nothing), curCurrency, False)
                        End If
                    End If
                End If

                CPCell = New TableCell
                CPCell.CssClass = rowClass
                If viewFinancial Then
                    CPCell.Text = currency.FormatCurrency(drow.Item("productValue"), currency.getCurrencyById(drow.Item("currencyId")), False)
                    CPCell.ToolTip = tmpTYM
                Else
                    CPCell.Text = "n/a"
                End If
                CPRow.Cells.Add(CPCell)

                CPCell = New TableCell
                CPCell.CssClass = rowClass
                If viewFinancial Then
                    CPCell.Text = currency.FormatCurrency(drow("maintenanceValue"), currency.getCurrencyById(drow.Item("currencyId")), False)
                Else
                    CPCell.Text = "n/a"
                End If
                CPRow.Cells.Add(CPCell)

                Dim tmpNYM As String = ""
                Dim hypNYM As String = "n/a"
                Dim maintparams As MaintParams
                Dim newmaint As NYMResult

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False) Then
                    ' calculate the next period maintenance
                    maintparams = Session("MaintParams")
                    maintparams.CurMaintVal = drow("maintenanceValue")

                    maintparams.ListPrice = drow.Item("productValue")

                    If IsDBNull(drow.Item("maintenancePercent")) = True Then
                        maintparams.PctOfLP = 0
                    Else
                        maintparams.PctOfLP = drow.Item("maintenancePercent")
                    End If

                    newmaint = SMRoutines.CalcNYM(maintparams, 0)
                    Session("CalcText" & Trim(drow.Item("contractProductId"))) = newmaint.NYMCalculation

                    Dim currObj As cCurrency = currency.getCurrencyById(drow.Item("currencyId"))

                    tmpNYM = ""
                    If params.BaseCurrency.HasValue Then
                        If drow.Item("currencyId") <> params.BaseCurrency.Value And Not currObj Is Nothing Then
                            If IsDBNull(drow.Item("createdDate")) = True Then
                                tmpNYM = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(currencyId, newmaint.NYMValue, Nothing), curCurrency, False)
                            Else
                                tmpNYM = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(currencyId, newmaint.NYMValue, drow.Item("createdDate")), curCurrency, False)
                            End If

                        End If
                    End If

                    If Not currObj Is Nothing Then
                        hypNYM = "<a href=""MaintCalc.aspx?id=" & CStr(drow.Item("contractProductId")).Trim & """ target=_blank>" & currency.FormatCurrency(newmaint.NYMValue, currObj, False).Trim & "</a>"
                    Else
                        hypNYM = ""
                    End If

                End If

                CPCell = New TableCell
                CPCell.CssClass = rowClass
                CPCell.ToolTip = tmpNYM
                Dim litNYM As New Literal
                litNYM.Text = hypNYM
                CPCell.Controls.Add(litNYM)
                CPRow.Cells.Add(CPCell)

                CPCell = New TableCell
                CPCell.CssClass = rowClass
                CPCell.Text = CStr(drow.Item("Quantity"))
                CPRow.Cells.Add(CPCell)

                CPCell = New TableCell
                CPCell.CssClass = rowClass
                CPCell.Text = drow.Item("unitDescription")
                CPRow.Cells.Add(CPCell)

                If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                    Try
                        CPCell = New TableCell
                        CPCell.CssClass = rowClass

                        If IsDBNull(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim)) = False Then
                            Dim tmpVal As String
                            tmpVal = CStr(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim))

                            If tmpVal <> "" And tmpVal <> "0" Then
                                Select Case cCOLL.CPFieldInfoItem.CP_UF1_Type
                                    Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref
                                        CPCell.Text = cCOLL.CPFieldInfoItem.CP_UF1_Coll(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim))

                                    Case Else
                                        CPCell.Text = drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF1.ToString.Trim)
                                End Select
                            End If
                        Else
                            CPCell.Text = ""
                        End If

                        CPRow.Cells.Add(CPCell)

                    Catch ex As Exception
                        ' uf selected may not be listed (due to grouping etc.)
                    End Try
                End If

                If cCOLL.CPFieldInfoItem.CP_UF2 > 0 Then
                    Try
                        CPCell = New TableCell
                        CPCell.CssClass = rowClass

                        If IsDBNull(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim)) = False Then
                            Dim tmpVal As String
                            tmpVal = CStr(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim))
                            If tmpVal <> "" And tmpVal <> "0" Then
                                Select Case cCOLL.CPFieldInfoItem.CP_UF2_Type
                                    Case UserFieldType.RechargeAcc_Code, UserFieldType.RechargeClient_Ref, UserFieldType.Site_Ref, UserFieldType.StaffName_Ref
                                        CPCell.Text = cCOLL.CPFieldInfoItem.CP_UF2_Coll(drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim))

                                    Case Else
                                        CPCell.Text = drow.Item("UF" & cCOLL.CPFieldInfoItem.CP_UF2.ToString.Trim)
                                End Select
                            End If
                        Else
                            CPCell.Text = ""
                        End If

                        CPRow.Cells.Add(CPCell)

                    Catch ex As Exception
                        ' uf selected may not be listed (due to grouping etc.)
                    End Try
                End If

                CPTable.Rows.Add(CPRow)
                hasdata = True
            Next

            If Not hasdata Then
                Dim tbrow As New TableRow
                Dim tbcell As New TableCell
                tbcell.CssClass = "row1"
                Dim cols As Integer = 13
                If cCOLL.CPFieldInfoItem.CP_UF1 > 0 Then
                    cols += 1
                End If
                If cCOLL.CPFieldInfoItem.CP_UF1 Then
                    cols += 1
                End If
                tbcell.HorizontalAlign = HorizontalAlign.Center
                tbcell.Text = "No contract products currently defined"
                tbcell.ColumnSpan = cols
                tbrow.Cells.Add(tbcell)
                CPTable.Rows.Add(tbrow)
            End If

            CPData.Controls.Add(CPTable)
            'End If
            Dim litSpacer As New Literal
            litSpacer.Text = "<div>&nbsp;</div>"
            CPData.Controls.Add(litSpacer)

            Dim CP_PageClose As New ImageButton

            With CP_PageClose
                .ImageUrl = "./buttons/page_close.png"
                .CausesValidation = False
                .ToolTip = "Exit the active contract and return to the home page"
                .Attributes.Add("onclick", "document.location=""Home.aspx""")
                .Attributes.Add("onmouseover", "window.status='" & .ToolTip & "';return true;")
                .Attributes.Add("onmouseout", "window.status='Done';")
                CPData.Controls.Add(CP_PageClose)
            End With
        End Sub

        Public Shared Function GetCPData(ByVal db As cFWDBConnection, ByVal drow As DataRow, ByVal appinfo As HttpApplication) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim mainClass As String = ""
            Dim tmpmaintval As Integer = 0
            Dim tmpStr As String = ""
            Dim tmpPP, tmpPS, tmpUC As String
            Dim baseCurrencyId As Integer = 0
            Dim maintparams As MaintParams
            Dim newmaint As NYMResult
            Dim hypNYM As String
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim baseCurrency As cCurrency

            baseCurrencyId = params.BaseCurrency.Value
            baseCurrency = currency.getCurrencyById(baseCurrencyId)

            If IsDBNull(drow.Item("maintenanceValue")) = True Then 'Or uinfo.permFinancial = False
                tmpmaintval = 0
            Else
                tmpmaintval = drow.Item("maintenanceValue")
            End If

            With strHTML
                .Append("<table>" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                .Append("<td class=""labeltd"">Product Name</td>" & vbNewLine)
                .Append("<td class=""inputtd""><input type=""text"" readonly value=""" & drow.Item("productName") & """ /></td>" & vbNewLine)
                .Append("<td class=""labeltd"">Product Cost</td>" & vbNewLine)

                Dim tmpTYM, tmpPV As String
                If drow.Item("currencyId") <> baseCurrencyId Then
                    tmpPV = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(CInt(drow.Item("CurrencyId")), drow.Item("productValue"), Now), currency.getCurrencyById(baseCurrencyId), False)
                    tmpTYM = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(drow.Item("currencyId"), tmpmaintval, Now), currency.getCurrencyById(baseCurrencyId), False)
                    mainClass = "not_base_currency_inputtd"
                Else
                    tmpTYM = ""
                    tmpPV = ""
                    mainClass = "inputtd"
                End If
                .Append("<td class=""" & mainClass & """ title=""" & tmpPV & """><input type=""text"" readonly value=""" & currency.FormatCurrency(drow.Item("productValue"), currency.getCurrencyById(CInt(drow.Item("currencyId"))), False) & """ /></td>" & vbNewLine)
                .Append("</tr>" & vbNewLine)

                .Append("<tr>" & vbNewLine)
                .Append("<td class=""labeltd"">Annual % of PC</td>" & vbNewLine)
                .Append("<td class=""inputtd""><input type=""text"" readonly value=""" & drow.Item("maintenancePercent") & """ /></td>" & vbNewLine)
                .Append("<td class=""labeltd"">Annual Cost (£)</td>" & vbNewLine)
                .Append("<td class=""" & mainClass & """ title=""" & tmpTYM & """><input type=""text"" readonly value=""" & currency.FormatCurrency(tmpmaintval, currency.getCurrencyById(CInt(drow.Item("currencyId"))), False) & """ /></td>" & vbNewLine)
                .Append("</tr>" & vbNewLine)

                .Append("<tr>" & vbNewLine)
                ' calculate the next period maintenance
                maintparams = appinfo.Session("MaintParams")
                maintparams.CurMaintVal = tmpmaintval
                If IsDBNull(drow.Item("productValue")) = True Then
                    maintparams.ListPrice = 0
                Else
                    maintparams.ListPrice = drow.Item("productValue")
                End If

                If IsDBNull(drow.Item("maintenancePercent")) = True Then
                    maintparams.PctOfLP = 0
                Else
                    maintparams.PctOfLP = drow.Item("maintenancePercent")
                End If

                newmaint = SMRoutines.CalcNYM(maintparams, 0)
                appinfo.Session("CalcText" & Trim(drow.Item("contractProductId"))) = newmaint.NYMCalculation

                Dim tmpNYM As String = ""
                'If uinfo.permFinancial Then
                If drow.Item("CurrencyId") <> baseCurrencyId Then
                    tmpNYM = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(newmaint.NYMValue, CDec(drow.Item("currencyId")), Now), baseCurrency, False)
                    mainClass = "not_base_currency_inputtd"
                Else
                    tmpNYM = ""
                    mainClass = "inputtd"
                End If

                hypNYM = "<a href=""MaintCalc.aspx?id=" & Trim(Str(drow.Item("contractProductId"))) & """ target=_blank>" & currency.FormatCurrency(newmaint.NYMValue, drow.Item("currencyId"), False).Trim & "</a>"
                'Else
                'hypNYM = "n/a"
                'End If
                .Append("<td class=""labeltd"">Next Period Cost</td>" & vbNewLine)
                .Append("<td class=""" & mainClass & """ title=""" & tmpNYM & """>" & hypNYM & "</td>" & vbNewLine)
                .Append("<td class=""labeltd"">Projected Saving</td>" & vbNewLine)
                'If uinfo.permFinancial = True Then
                If IsDBNull(drow.Item("projectedSaving")) = True Then
                    tmpStr = currency.FormatCurrency(0, drow.Item("CurrencyId"), False)
                Else
                    tmpStr = currency.FormatCurrency(drow.Item("projectedSaving"), currency.getCurrencyById(CInt(drow.Item("currencyId"))), False)
                End If
                'Else
                'tmpStr = "n/a"
                'End If

                If baseCurrencyId <> drow.Item("currencyId") Then 'And uinfo.permFinancial Then
                    If IsDBNull(drow.Item("ProjectedSaving")) = True Then
                        tmpPS = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(0, CInt(drow.Item("currencyId")), Now), baseCurrency, False)
                    Else
                        tmpPS = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(CDec(drow.Item("projectedSaving")), CInt(drow.Item("currencyId")), Now), baseCurrency, False)
                    End If

                    mainClass = "not_base_currency_inputtd"
                Else
                    tmpPS = ""
                    mainClass = "inputtd"
                End If

                strHTML.Append("<td class=""" & mainClass & """ title=""" & tmpPS & """><input type=""text"" readonly value=""" & tmpStr & """ /></td>" & vbNewLine)
                .Append("</tr>" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                .Append("<td class=""labeltd"">Price Paid</td>" & vbNewLine)
                'If uinfo.permFinancial = True Then
                If IsDBNull(drow.Item("pricePaid")) = True Then
                    tmpStr = currency.FormatCurrency(0, drow.Item("currencyId"), False)
                Else
                    tmpStr = currency.FormatCurrency(drow.Item("pricePaid"), drow.Item("currencyId"), False)
                End If
                'Else
                'tmpStr = "n/a"
                'End If

                If baseCurrencyId <> drow.Item("currencyId") Then ' And uinfo.permFinancial Then
                    If IsDBNull(drow.Item("pricePaid")) = True Then
                        tmpPP = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(0, CInt(drow.Item("currencyId")), Now), baseCurrency, False)
                    Else
                        tmpPP = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(CDec(drow.Item("pricePaid")), CInt(drow.Item("currencyId")), Now), baseCurrency, False)
                    End If

                    mainClass = "not_base_currency_inputtd"
                Else
                    tmpPP = ""
                    mainClass = "inputtd"
                End If

                .Append("<td class=""" & mainClass & """ title=""" & tmpPP & """><input type=""text"" readonly value=""" & tmpStr & """ /></td>" & vbNewLine)
                .Append("<td class=""labeltd"">Sales Tax</td>" & vbNewLine)
                .Append("<td class=""inputtd""><input type=""text"" readonly value=""" & drow.Item("salesTaxDescription") & """ /></td>" & vbNewLine)
                .Append("</tr>" & vbNewLine)

                .Append("<tr>" & vbNewLine)
                .Append("<td class=""labeltd"">Quantity</td>" & vbNewLine)
                .Append("<td class=""inputtd""><input type=""text"" readonly value=""" & drow.Item("Quantity") & """ /></td>" & vbNewLine)
                .Append("<td class=""labeltd"">Unit Description</td>" & vbNewLine)
                .Append("<td class=""inputtd""><input type=""text"" readonly value=""" & drow.Item("unitDescription") & """ /></td>" & vbNewLine)
                .Append("</tr>" & vbNewLine)

                .Append("<tr>" & vbNewLine)
                .Append("<td class=""labeltd"">Unit Cost</td>" & vbNewLine)
                'If uinfo.permFinancial = True Then
                If IsDBNull(drow.Item("unitCost")) = True Then
                    tmpStr = currency.FormatCurrency(0, drow.Item("currencyId"), False)
                Else
                    tmpStr = currency.FormatCurrency(drow.Item("unitCost"), currency.getCurrencyById(CInt(drow.Item("currencyId"))), False)
                End If
                'Else
                'tmpStr = "n/a"
                'End If

                If baseCurrencyId <> drow.Item("currencyId") Then 'And uinfo.permFinancial Then
                    If IsDBNull(drow.Item("unitCost")) = True Then
                        tmpUC = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(0, CInt(drow.Item("currencyId")), Now), baseCurrency, False)
                    Else
                        tmpUC = "Value in Base Currency is " & currency.FormatCurrency(currency.convertToBase(CDec(drow.Item("unitCost")), CInt(drow.Item("currencyId")), Now), currency.getCurrencyById(CInt(drow.Item("currencyId"))), False)
                    End If

                    mainClass = "not_base_currency_inputtd"
                Else
                    tmpUC = ""
                    mainClass = "inputtd"
                End If

                .Append("<td class=""" & mainClass & """ title=""" & tmpUC & """><input type=""text"" readonly value=""" & tmpStr & """ /></td>" & vbNewLine)
                .Append("<td colspan=""2""></td>" & vbNewLine)
                .Append("</tr>" & vbNewLine)
                .Append("<tr><td colspan=""4""><hr /></td></tr>" & vbNewLine)
                .Append("</table>" & vbNewLine)
            End With

            Return strHTML.ToString
        End Function

        Private Sub DisplayCPForEdit(Optional ByVal id As Integer = 0, Optional ByVal conCatId As Integer = 0)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim sql As String
            Dim tmpId, curId As Integer
            Dim AddMulti As Boolean = IIf(Session("CP_AddMulti") = "1", True, False)
            Dim isAddandDefine As Boolean = IIf(Session("CP_AddandDefine") = "1", True, False)
            Dim UFields As String = ""
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim accproperties As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim db As New cFWDBConnection
            Dim clsBaseDefs As cBaseDefinitions

            db.DBOpen(fws, False)

            ' disable the nav options while editing / adding
            'SetNavActiveState(False)

            ShowCPDetail(True, AddMulti, isAddandDefine)

            lnkNew.Visible = False
            lnkNewDefine.Visible = False
            lnkGenerate.Visible = False
            'lnkVRegistry.Visible = False

            cmdCPCancel.Visible = True

            Master.enablenavigation = False
            Master.useCloseNavigationMsg = False
            Master.RefreshBreadcrumbInfo()

            If id <> 0 Then
                panelAddAnother.Visible = False

                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ContractProducts, False) Then
                    cmdCPUpdate.ToolTip = "Save changes made to current selection"
                    cmdCPUpdate.AlternateText = "Update"

                    If Session("CPAction") = "add" Or Session("CPAction") = "edit" Then
                        If lblCPMessage.Text = "" Then
                            cmdCPUpdate.Visible = True
                        End If
                        cmdCPCancel.Visible = True
                    Else
                        cmdCPUpdate.Visible = False
                        cmdCPCancel.Visible = False
                    End If
                    cmdCPUpdate.Attributes.Add("onmouseover", "window.status='Save any changes to the database';return true;")
                    cmdCPUpdate.Attributes.Add("onmouseout", "window.status='Done';")
                    cmdCPUpdate.Attributes.Add("onclick", "javascript:if(validateform('cpdetails') == false) { return false; }")
                End If

                Session("ActiveContractProduct") = id

                ' Get data for active record
                sql = "SELECT [contractProductId],[contract_productdetails].[productId],[productDetails].[ProductName],[currencyId],[salesTaxRate]," & _
                 "[Quantity],[unitId],[productValue],[maintenanceValue],[maintenancePercent],[unitCost],[pricePaid],[projectedSaving]<**UF**> " & vbNewLine & _
                 "FROM [contract_productdetails] " & _
                 "LEFT JOIN [productDetails] ON [contract_productdetails].[productId] = [productDetails].[ProductId]" & vbNewLine & _
                 "WHERE [contractProductId] = " & id.ToString

                UFields = GetUFieldList(curUser, conCatId)

                sql = Replace(sql, "<**UF**>", UFields)

                db.RunSQL(sql, db.glDBWorkA, False, "", False)

                Session("PreRec") = sql

                lblCPTitle.Text = " - " & db.GetFieldValue(db.glDBWorkA, "productName", 0, 0)

                If Session("CP_AddMulti") = 0 Then
                    tmpId = SMRoutines.CheckListIndex(db.GetFieldValue(db.glDBWorkA, "productId", 0, 0))
                    If lstProductName.SelectedItem.Value Is Nothing Then
                        curId = 0
                    Else
                        curId = lstProductName.SelectedItem.Value
                    End If

                    If tmpId <> curId Then
                        Try
                            ' if this fails, product-vendor association not present
                            lstProductName.ClearSelection()
                            lstProductName.Items.FindByValue(tmpId).Selected = True
                        Catch ex As Exception
                            lblCPMessage.Text = "** WARNING - Could not display product. Check " & accproperties.SupplierPrimaryTitle & " - Product Association"
                            cmdCPCancel.ImageUrl = "~/buttons/page_close.png"
                        End Try

                        'lstProductName.Items.FindByValue(curId).Selected = False
                    End If
                End If


                If Not db.GetFieldValue(db.glDBWorkA, "salesTaxRate", 0, 0) Is DBNull.Value Then
                    tmpId = SMRoutines.CheckListIndex(db.GetFieldValue(db.glDBWorkA, "salesTaxRate", 0, 0))

                    clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.SalesTax)
                    lstCPSalesTax.Items.Clear()
                    lstCPSalesTax.Items.AddRange(clsBaseDefs.CreateDropDown(True, tmpId))
                    lstCPSalesTax.SelectedIndex = lstCPSalesTax.Items.IndexOf(lstCPSalesTax.Items.FindByValue(tmpId))

                    'If lstCPSalesTax.SelectedItem.Value Is Nothing Then
                    '    curId = 0
                    'Else
                    '    curId = lstCPSalesTax.SelectedItem.Value
                    'End If

                    'If tmpId <> curId Then
                    '    lstCPSalesTax.ClearSelection()
                    '    lstCPSalesTax.Items.FindByValue(tmpId).Selected = True
                    '    'lstCPSalesTax.Items.FindByValue(curId).Selected = False
                    'End If
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                    If IsDBNull(db.GetFieldValue(db.glDBWorkA, "productValue", 0, 0)) Then
                        txtProductValue.Text = ""
                    Else
                        txtProductValue.Text = db.GetFieldValue(db.glDBWorkA, "productValue", 0, 0)
                    End If
                    If IsDBNull(db.GetFieldValue(db.glDBWorkA, "pricePaid", 0, 0)) Then
                        txtPricePaid.Text = ""
                    Else
                        txtPricePaid.Text = db.GetFieldValue(db.glDBWorkA, "pricePaid", 0, 0)
                    End If
                    If IsDBNull(db.GetFieldValue(db.glDBWorkA, "maintenanceValue", 0, 0)) Then
                        txtMaintValue.Text = ""
                    Else
                        txtMaintValue.Text = db.GetFieldValue(db.glDBWorkA, "maintenanceValue", 0, 0)
                    End If
                    If IsDBNull(db.GetFieldValue(db.glDBWorkA, "projectedSaving", 0, 0)) Then
                        txtProjectedSaving.Text = ""
                    Else
                        txtProjectedSaving.Text = db.GetFieldValue(db.glDBWorkA, "projectedSaving", 0, 0)
                    End If
                Else
                    txtProductValue.Text = "0"
                    txtPricePaid.Text = "0"
                    txtMaintValue.Text = "0"
                    txtProjectedSaving.Text = "0"
                End If

                If IsDBNull(db.GetFieldValue(db.glDBWorkA, "maintenancePercent", 0, 0)) Then
                    txtMaintPercent.Text = ""
                Else
                    txtMaintPercent.Text = db.GetFieldValue(db.glDBWorkA, "maintenancePercent", 0, 0)
                End If
                If IsDBNull(db.GetFieldValue(db.glDBWorkA, "Quantity", 0, 0)) Then
                    txtQuantity.Text = ""
                Else
                    txtQuantity.Text = db.GetFieldValue(db.glDBWorkA, "Quantity", 0, 0)
                End If


                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.Units)

                If Not db.GetFieldValue(db.glDBWorkA, "unitId", 0, 0) Is DBNull.Value Then
                    tmpId = SMRoutines.CheckListIndex(db.GetFieldValue(db.glDBWorkA, "unitId", 0, 0))

                    lstUnits.Items.Clear()
                    lstUnits.Items.AddRange(clsBaseDefs.CreateDropDown(True, tmpId))
                    lstUnits.ClearSelection()
                    lstUnits.Items.FindByValue(tmpId).Selected = True
                Else
                    lstUnits.Items.Clear()
                    lstUnits.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))
                End If

                If IsDBNull(db.GetFieldValue(db.glDBWorkA, "unitCost", 0, 0)) Then
                    txtUnitCost.Text = ""
                Else
                    txtUnitCost.Text = db.GetFieldValue(db.glDBWorkA, "unitCost", 0, 0)
                End If

                tmpId = SMRoutines.CheckListIndex(db.GetFieldValue(db.glDBWorkA, "currencyId", 0, 0))
                lstCPCurrency.ClearSelection()
                lstCPCurrency.Items.FindByValue(tmpId).Selected = True
                'If lstCPCurrency.SelectedItem.Value Is Nothing Then
                '    curId = 0
                'Else
                '    curId = lstCPCurrency.SelectedItem.Value
                'End If

                'If tmpId <> curId Then
                '    lstCPCurrency.Items.FindByValue(curId).Selected = False
                'End If

                LoadCPUFData(Session("ActiveContract"), id)

                Session("CPAction") = "edit"
                'litProductUF.Text = GetProductUFData(db, True, id, conCatId)
            Else

                If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractProducts, False) Then
                    If Session("CPAction") = "edit" Then
                        lnkAddCPTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)
                    End If
                    cmdCPUpdate.Visible = True
                    lnkAddCPTask.ToolTip = "Create a task associated with the current contract product"
                    lnkAddCDTask.Attributes.Add("onmouseover", "window.status='Create a task associated with the current contract product';return true;")
                    lnkAddCPTask.Attributes.Add("onmouseout", "window.status='Done';")
                End If

                lblCPTitle.Text = " - New Contract Product"
                If Session("CP_AddMulti") = 0 Then
                    If Not lstProductName.SelectedItem Is Nothing Then
                        'lstProductName.SelectedItem.Selected = False
                    End If
                    lstProductName.ClearSelection()
                    lstProductName.Items(0).Selected = True
                Else
                    txtProductName.Text = ""
                End If

                'If Not lstCPSalesTax Is Nothing Then
                'lstCPSalesTax.SelectedItem.Selected = False
                'End If

                lstCPSalesTax.ClearSelection()
                lstCPSalesTax.Items(0).Selected = True

                txtProductValue.Text = "0"
                txtPricePaid.Text = "0"
                txtMaintValue.Text = "0"
                txtProjectedSaving.Text = "0"
                txtMaintPercent.Text = "0"
                txtQuantity.Text = "0"

                'If Not lstUnits.SelectedItem Is Nothing Then
                '    lstUnits.SelectedItem.Selected = False
                'End If
                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.Units)
                lstUnits.Items.Clear()
                lstUnits.Items.AddRange(clsBaseDefs.CreateDropDown(True, tmpId))
                lstUnits.ClearSelection()
                lstUnits.Items(0).Selected = True


                txtUnitCost.Text = "0"

                'If Not lstCPCurrency.SelectedItem Is Nothing Then
                '    lstCPCurrency.SelectedItem.Selected = False
                'End If

                lstCPCurrency.ClearSelection()
                lstCPCurrency.Items(0).Selected = False

                LoadCPUFData(Session("ActiveContract"), 0)
                'litProductUF.Text = GetProductUFData(db, False, 0, conCatId)
            End If

            db.DBClose()
        End Sub

        Private Sub ShowCPDetail(ByVal x As Boolean, Optional ByVal AddMultiple As Boolean = False, Optional ByVal AddandDefine As Boolean = False)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            CP_EditFieldsPanel.Visible = x
            panelAddAnother.Visible = x
            CP_FilterPanel.Visible = Not x

            Dim clsBaseDefs As New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories)

            lstProductCategory.Items.Clear()
            lstProductCategory.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))
            lstProductName.Visible = Not AddandDefine
            lstProductCategory.Visible = AddandDefine
            CPproductcategorypanel.Visible = AddandDefine
            txtProductName.Visible = AddandDefine
            reqProductName.Visible = AddandDefine
            txtProductName.Enabled = AddandDefine
            reqProductName.Enabled = AddandDefine
            chkAddAnother.Checked = AddMultiple
        End Sub

        Private Sub GetConProductUFFields(ByVal contractId As Integer, Optional ByVal ConProdId As Integer = 0)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))

            'Dim employees As New cEmployees(fws.MetabaseCustomerId)
            Dim ufcoll As New cUserdefinedFields(curUser.Account.accountid)
            'Dim ufgrp As New cUserFieldGroupingCollection(fws, uinfo, AppAreas.CONPROD_GROUPING, ConProdId, employees)
            Dim contractCategoryId As Integer = 0

            Dim ufields As New cUserdefinedFields(curUser.Account.accountid)
            Dim tables As New cTables(curUser.Account.accountid)
            Dim cpTable As cTable = tables.GetTableByName("contract_productdetails")
            Dim cpUFields As List(Of cUserDefinedField) = ufields.GetFieldsByTable(cpTable)

            Dim sql As String = "select [categoryId] from contract_details where [contractId] = @conId"
            db.sqlexecute.Parameters.AddWithValue("@conId", contractId)

            contractCategoryId = db.getIntSum(sql)

            ViewState("record") = cUFInterim.GetUFRecordValues(curUser.Account.accountid, ConProdId, cpUFields)
            phCPUFields.Controls.Clear()
            ufields.createFieldPanel(phCPUFields, cpTable, "cpdetails", New StringBuilder)
        End Sub

        Private Sub LoadCPUFData(ByVal contractId As Integer, ByVal conProdId As Integer)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim ufields As New cUserdefinedFields(curUser.Account.accountid)
            Dim tables As New cTables(curUser.Account.accountid)
            Dim fields As New cFields(curUser.AccountID)
            Dim cpTable As cTable = tables.GetTableByName("contract_productdetails")
            Dim udTable As cTable = tables.GetTableByID(cpTable.UserDefinedTableID)
            Dim cdUFields As List(Of cUserDefinedField) = ufields.GetFieldsByTable(udTable)
            Dim record As SortedList(Of Integer, Object) = ufields.GetRecord(udTable, conProdId, tables, fields)
            ufields.populateRecordDetails(phCPUFields, udTable, record)
        End Sub

        Public Shared Function CreateCP_UFBody(ByVal curUser As CurrentUser, ByVal drow As DataRow) As String
            Dim sql As String
            'Dim tmpHeaderRow As New System.Text.StringBuilder
            Dim tmpDataRow As New System.Text.StringBuilder
            Dim tmpResult As New System.Text.StringBuilder
            Dim ufrow As DataRow
            Dim ufieldname As String
            Dim hasdata As Boolean
            Dim fieldcount As Integer
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
            fieldcount = 0
            hasdata = False

            sql = "SELECT * FROM [user_fields] WHERE [AppArea] = @appArea ORDER BY [Field Sequence]"
            db.sqlexecute.Parameters.AddWithValue("@appArea", AppAreas.CONTRACT_PRODUCTS)
            Dim dset As New DataSet
            dset = db.GetDataSet(sql)

            'tmpHeaderRow.Append("<tr><td colspan=""2"">&nbsp;</td>" & vbNewLine)
            tmpDataRow.Append("<div id=""broadcasttitle"">User Defined Fields</div>" & vbNewLine)
            tmpDataRow.Append("<table>" & vbNewLine)
            tmpDataRow.Append("<tr>" & vbNewLine)

            For Each ufrow In dset.Tables(0).Rows
                ufieldname = "UF" & Trim(Str(ufrow.Item("User Field Id")))

                tmpDataRow.Append("<td class=""labeltd"">" & Trim(ufrow.Item("Field Name")) & "</td>")
                tmpDataRow.Append("<td class=""inputtd""")

                Select Case CType(ufrow.Item("Field Type"), UserFieldType)
                    Case UserFieldType.Character, UserFieldType.DDList
                        If IsDBNull(drow.Item(ufieldname)) = True Then
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append(">" & Trim(drow.Item(ufieldname)) & "</td>" & vbNewLine)
                        End If

                    Case UserFieldType.Hyperlink
                        If IsDBNull(drow.Item(ufieldname)) = True Then
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append("><a target=""_blank"" href=""" & Trim(drow.Item(ufieldname)) & """>" & Trim(drow.Item(ufieldname)) & "</a></td>" & vbNewLine)
                        End If

                    Case UserFieldType.Number, UserFieldType.Float
                        If IsDBNull(drow.Item(ufieldname)) = False Then
                            tmpDataRow.Append(">" & Trim(drow.Item(ufieldname)) & "</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        End If

                    Case UserFieldType.Checkbox
                        If IsDBNull(drow.Item(ufieldname)) = False Then
                            tmpDataRow.Append(">")
                            If drow.Item(ufieldname) = "1" Then
                                tmpDataRow.Append("<input type=""checkbox"" disabled checked /></td>" & vbNewLine)
                            Else
                                tmpDataRow.Append("<input type=""checkbox"" disabled /></td>" & vbNewLine)
                            End If
                        Else
                            tmpDataRow.Append(">")
                            tmpDataRow.Append("<input type=""checkbox"" disabled /></td>" & vbNewLine)
                        End If

                    Case UserFieldType.Text
                        If IsDBNull(drow.Item(ufieldname)) = True Then
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append("><textarea readonly>")
                            tmpDataRow.Append(drow.Item(ufieldname))
                            tmpDataRow.Append("</textarea></td>")
                        End If

                    Case UserFieldType.DateField
                        If IsDBNull(drow.Item(ufieldname)) = False Then
                            tmpDataRow.Append(">" & Format(drow.Item(ufieldname), cDef.DATE_FORMAT) & "</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        End If

                    Case UserFieldType.StaffName_Ref
                        Dim tmpStr As String

                        If IsDBNull(drow.Item(ufieldname)) = False Then
                            Dim emps As New cEmployees(curUser.AccountID)
                            Dim emp As Employee = emps.GetEmployeeById(drow.Item(ufieldname))

                            If Not emp Is Nothing Then
                                tmpStr = emp.Forename & " " & emp.Surname
                            Else
                                tmpStr = "&nbsp;"
                            End If
                            tmpDataRow.Append(">" & tmpStr & "</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        End If

                    Case UserFieldType.Site_Ref
                        Dim tmpStr As String

                        If IsDBNull(drow.Item(ufieldname)) = False Then
                            Dim sites As New cSites(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, cAccounts.getConnectionString(curUser.AccountID))
                            Dim site As cSite = sites.GetSiteById(drow.Item(ufieldname))

                            If Not site Is Nothing Then
                                tmpStr = site.SiteCode & ":" & site.SiteDescription
                            Else
                                tmpStr = "&nbsp;"
                            End If
                            tmpDataRow.Append(">" & tmpStr & "</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        End If

                    Case UserFieldType.RechargeClient_Ref
                        Dim tmpStr As String

                        If IsDBNull(drow.Item(ufieldname)) = False Then
                            Dim rclients As New cRechargeClientList(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID))
                            Dim client As cRechargeClient = rclients.GetClientById(drow.Item(ufieldname))

                            If Not client Is Nothing Then
                                tmpStr = client.ClientName
                            Else
                                tmpStr = "&nbsp;"
                            End If
                            tmpDataRow.Append(">" & tmpStr & "</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        End If

                    Case UserFieldType.RechargeAcc_Code
                        Dim tmpStr As String

                        If IsDBNull(drow.Item(ufieldname)) = False Then
                            Dim raccs As New cRechargeAccountCodes(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID))
                            Dim rac As cRechargeAccountCode = raccs.GetCodeById(drow.Item(ufieldname))

                            If Not rac Is Nothing Then
                                tmpStr = rac.AccountCode
                            Else
                                tmpStr = "&nbsp;"
                            End If
                            tmpDataRow.Append(">" & tmpStr & "</td>" & vbNewLine)
                        Else
                            tmpDataRow.Append(">&nbsp;</td>" & vbNewLine)
                        End If

                    Case Else

                End Select

                fieldcount += 1
                If fieldcount >= 2 Then
                    ' start a new line
                    tmpDataRow.Append("</tr>" & vbNewLine)
                    tmpResult.Append(tmpDataRow)

                    tmpDataRow.Remove(0, tmpDataRow.Length)
                    tmpDataRow.Append("<tr>" & vbNewLine)

                    fieldcount = 0
                End If

                hasdata = True
            Next

            ' finish off the remaining fields for the row
            If fieldcount = 1 Then
                tmpDataRow.Append("<td colspan=""2"">&nbsp;</td></tr>" & vbNewLine)
            End If

            If fieldcount = 0 Then
                ' blank these as full row was completed and this then prevents 
                ' unnecessary row between product being inserted
                tmpDataRow.Remove(0, tmpDataRow.Length)
            End If

            tmpDataRow.Append("<tr><td colspan=""4""><hr /></td></tr>" & vbNewLine)
            tmpDataRow.Append("</table>" & vbNewLine)

            tmpResult.Append(tmpDataRow)
            Return tmpResult.ToString
        End Function

        Public Shared Function GetUFieldList(ByVal curUser As CurrentUser, Optional ByVal ConCatId As Integer = 0) As String
            'Dim sql As New System.Text.StringBuilder
            Dim retStr As New System.Text.StringBuilder

            'Dim drow As DataRow

            'sql.Append("SELECT [User Field Id],[codes_userfieldgrouping].[Grouping Description] FROM [user_fields] ")
            'sql.Append("LEFT OUTER JOIN [codes_userfieldgrouping] ON [codes_userfieldgrouping].[UF Group Id] = [user_fields].[Group Id] ")
            'If ConCatId > 0 Then
            '	sql.Append("LEFT OUTER JOIN [uf_groupallocation] ON [codes_userfieldgrouping].[UF Group Id] = [uf_groupallocation].[UF Grouping Id]")
            'End If
            'sql.Append("WHERE [AppArea] = " & AppAreas.CONTRACT_PRODUCTS & " OR ")
            'If ConCatId > 0 Then
            '	sql.Append("(")
            'End If
            'sql.Append("[AppArea] = " & AppAreas.CONPROD_GROUPING)
            'If ConCatId > 0 Then
            '	sql.Append(" AND [uf_groupallocation].[Category Id] = " & ConCatId.ToString & ")")
            'End If
            'sql.Append(" ORDER BY [Grouping Sequence],[Field Sequence]")
            'db.RunSQL(sql.ToString, db.glDBWorkL, False, "", False)
            Dim ufields As New cUserdefinedFields(curUser.Account.accountid)
            Dim tables As New cTables(curUser.AccountID)
            Dim cdTable As cTable = tables.GetTableByName("contract_details")
            Dim cdUFields As List(Of cUserDefinedField) = ufields.GetFieldsByTable(cdTable)
            Dim comma As String = ""

            For Each uf As cUserDefinedField In cdUFields
                retStr.Append(comma)
                retStr.Append(" udf" + uf.userdefineid.ToString())
                comma = ","
            Next

            'For Each drow In db.glDBWorkL.Tables(0).Rows
            '	retStr.Append(",[UF" & CStr(drow.Item("User Field Id")).Trim & "]")
            'Next

            Return retStr.ToString
        End Function

        'Public Shared Function CreateCP_UFGroupingBody(ByVal db As cFWDBConnection, ByVal cpId As Integer, ByVal uFieldList As String, ByVal appinfo As HttpApplication) As String
        '	Dim grpUFields(100) As UFCollection
        '	Dim categoryId As Integer
        '	Dim reqUFields As String
        '	Dim sql As String
        '	Dim retStr As String = ""

        '	reqUFields = ""
        '	db.FWDb("R3", "contract_details", "Contract Id", appinfo.Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
        '	If db.FWDb3Flag = True Then
        '		categoryId = Val(db.FWDbFindVal("Category Id", 3))

        '		GetUFCollection(db, categoryId, reqUFields, grpUFields)

        '		sql = "SELECT [Contract-Product Id]" & reqUFields & " FROM [contract_productdetails] " & vbNewLine
        '		sql += "WHERE [Contract-Product Id] = @cpId"
        '		db.AddDBParam("cpId", cpId, True)
        '		db.RunSQL(sql, db.glDBWorkL, False, "", False)
        '		If db.glNumRowsReturned > 0 Then
        '			' should only ever return one row!
        '			Try
        '				retStr = SMRoutines.GetGroupedUFData(db, db.glDBWorkL.Tables(0).Rows(0), grpUFields)
        '			Catch ex As Exception
        '				retStr = ""
        '			End Try
        '		End If
        '	End If

        '	Return retStr
        'End Function

        'Public Shared Sub GetUFCollection(ByVal db As cFWDBConnection, ByVal catId As Integer, ByRef reqUF As String, ByRef GrpUFields As UFCollection())
        '	Dim sql As New System.Text.StringBuilder
        '	Dim drow As DataRow

        '	sql.Append("SELECT [user_fields].*,[codes_userfieldgrouping].[Grouping Sequence],[codes_userfieldgrouping].[Grouping Description] " & vbNewLine)
        '	sql.Append("FROM [user_fields] " & vbNewLine)
        '	sql.Append("INNER JOIN [codes_userfieldgrouping] ON [codes_userfieldgrouping].[UF Group Id] = [user_fields].[Group Id] " & vbNewLine)
        '	sql.Append("WHERE [AppArea] = " & Trim(Str(AppAreas.CONPROD_GROUPING)) & " AND [Group Id] IN (SELECT [UF Grouping Id] FROM [uf_groupallocation] WHERE [Category Id] = " & catId.ToString & ") " & vbNewLine)
        '	sql.Append("ORDER BY [Grouping Sequence],[Field Sequence]")
        '	db.RunSQL(sql.ToString, db.glDBWorkI, False, "", False)

        '	Dim idx As Integer
        '	idx = 0
        '	reqUF = ""

        '	For Each drow In db.glDBWorkI.Tables(0).Rows
        '		reqUF += ",[UF" & Trim(Str(drow.Item("User Field Id"))) & "]"
        '		GrpUFields(idx).UF_DBFieldName = "UF" & Trim(Str(drow.Item("User Field Id")))
        '		GrpUFields(idx).UF_FieldName = drow.Item("Field Name")
        '		GrpUFields(idx).UF_FieldNumber = drow.Item("User Field Id")
        '		GrpUFields(idx).UF_FieldGroupingName = Trim(drow.Item("Grouping Description"))
        '		GrpUFields(idx).UF_FieldType = Trim(Str(drow.Item("Field Type")))
        '		idx += 1
        '	Next
        '	ReDim Preserve GrpUFields(idx - 1)
        'End Sub

        Public Shared Function GetProduct(ByVal db As cFWDBConnection, ByVal CPid As Integer, ByVal appinfo As HttpApplication, ByVal curUser As CurrentUser) As String
            Dim UFieldList As String
            Dim strHTML As New System.Text.StringBuilder
            Dim sql As New System.Text.StringBuilder
            Dim reqGrpUFields As String = ""
            Dim GrpUFields(100) As UFCollection
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)

            ' get dataset of user fields for the recharge grouping
            db.FWDb("R3", "contract_details", "ContractId", appinfo.Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDb3Flag = True Then
                'GetUFCollection(db, Val(db.FWDbFindVal("Category Id", 3)), reqGrpUFields, GrpUFields)
            End If

            UFieldList = GetUFieldList(curUser, Val(db.FWDbFindVal("CategoryId", 3)))

            'sql = "SELECT * FROM [Contract - Product Details] WHERE [Contract-Product Id] = " & Trim(Str(CPid))
            sql.Append("SELECT [ContractProductId],[ContractId],[contract_productdetails].[ProductId] AS [ProdID],ISNULL([CurrencyId],0) AS [CurrencyId],[codes_salestax].[description],")
            sql.Append("[Quantity],[codes_units].[description],[UnitCost],ISNULL([ProjectedSaving],0) AS [ProjectedSaving],[productDetails].[ProductName],ISNULL([ProductValue],0) AS [ProductValue],ISNULL([PricePaid],0) AS [PricePaid],ISNULL([MaintenanceValue],0) AS [MaintenanceValue],[MaintenancePercent]<**UF**>" & vbNewLine)
            sql.Append("FROM [contract_productdetails] " & vbNewLine)
            sql.Append("LEFT OUTER JOIN [codes_units] ON [contract_productdetails].[UnitId] = [codes_units].[UnitId]" & vbNewLine)
            sql.Append("LEFT OUTER JOIN [codes_salestax] ON [contract_productdetails].[SalesTaxRate] = [codes_salestax].[SalesTaxId]" & vbNewLine)
            sql.Append("LEFT OUTER JOIN [productDetails] ON [contract_productdetails].[ProductId] = [productDetails].[ProductId]" & vbNewLine)
            sql.Append("WHERE [ContractProductId] = @CPid")

            sql.Replace("<**UF**>", UFieldList)
            db.AddDBParam("CPid", CPid, True)
            db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)

            If db.glNumRowsReturned > 0 Then
                strHTML.Append(GetCPData(db, db.glDBWorkD.Tables(0).Rows(0), appinfo))
                strHTML.Append(CreateCP_UFBody(curUser, db.glDBWorkD.Tables(0).Rows(0)))
                'strHTML.Append(CreateCP_UFGroupingBody(db, CPid, reqGrpUFields, appinfo))
            Else
                strHTML.Append("")
            End If

            GetProduct = strHTML.ToString
        End Function

        Private Sub NewCPEntry(Optional ByVal conCatId As Integer = 0)
            DisplayCPForEdit(0, conCatId)
        End Sub

        Private Function ValidateNewProduct() As Integer
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection()

            ' check whether product name specified is duplicate or not
            Dim tmpNewProductId As Integer

            db.DBOpen(fws, False)
            db.FWDb("R2", "productDetails", "ProductName", txtProductName.Text, "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag = False Then
                ' doesn't exist, so create it and associate it with the active vendor
                Dim tmpVendorId As Integer

                db.FWDb("R3", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                If db.FWDb3Flag = True Then
                    tmpVendorId = Val(db.FWDbFindVal("supplierId", 3))

                    db.SetFieldValue("ProductName", Trim(txtProductName.Text), "S", True)
                    If lstProductCategory.SelectedValue > 0 Then
                        db.SetFieldValue("ProductCategoryId", lstProductCategory.SelectedValue, "N", False)
                    End If
                    db.SetFieldValue("subAccountId", curUser.CurrentSubAccountId, "N", False)
                    db.SetFieldValue("createdOn", Date.UtcNow, "D", False)
                    db.SetFieldValue("createdBy", curUser.EmployeeID, "N", False)
                    db.SetFieldValue("Archived", False, "N", False)
                    db.FWDb("W", "productDetails", "", "", "", "", "", "", "", "", "", "", "", "")
                    tmpNewProductId = db.glIdentity

                    If tmpNewProductId > 0 Then
                        ' create the product vendor association entry
                        db.SetFieldValue("productId", tmpNewProductId, "N", True)
                        db.SetFieldValue("supplierId", tmpVendorId, "N", False)
                        db.FWDb("W", "product_suppliers", "", "", "", "", "", "", "", "", "", "", "", "")
                    End If

                    ' add it to the drop down list
                    lstProductName.Items.Add(New ListItem(txtProductName.Text, tmpNewProductId))
                Else
                    ValidateNewProduct = 0
                    Exit Function
                End If
            Else
                ValidateNewProduct = 0
                Exit Function
            End If

            db.DBClose()
            ValidateNewProduct = tmpNewProductId
        End Function

        Private Sub ExitCP_AddEdit()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim cacheEnum As IDictionaryEnumerator = Cache.GetEnumerator
            Dim tmpEmpId As Integer

            lblCPMessage.Text = ""

            While cacheEnum.MoveNext
                If cacheEnum.Key = "CP" & Session("ActiveContractProduct") Then
                    tmpEmpId = cacheEnum.Value
                    If tmpEmpId = curUser.Employee.EmployeeID Then
                        Cache.Remove("CP" & Session("ActiveContractProduct"))
                    End If
                    Exit While
                End If
            End While

            Session("CPAction") = Nothing
            Session("CP_AddandDefine") = Nothing
            Session("CP_AddMulti") = Nothing

            SetNavActiveState(True)
            ShowCPDetail(False)
            lnkAddCPTask.Visible = False
            lnkNew.Visible = False
            lnkGenerate.Visible = False
            lnkNewDefine.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractProducts, False)
            lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractProducts, False)

            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))

            Dim catId As Integer = 0

            If lstContractCategory.SelectedValue <> "0" And lstContractCategory.SelectedValue <> "" Then
                Dim sql As String = "select [categoryId] from contract_details where [contractId] = @conId"
                db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))
                catId = db.getIntSum(sql)
            End If

            lblErrorString.Text = String.Empty
            GetCPRecs(catId)

            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractProduct & "&id=" & Session("ActiveContract") & "&title=" & lblCPTitle.Text.Replace(vbNewLine, ""), True)
        End Sub

        Protected Sub cmdCPCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCPCancel.Click
            ExitCP_AddEdit()

        End Sub

        Protected Sub cmdCPUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCPUpdate.Click
            cmdCPUpdate.Enabled = False
            UpdateCPRecord()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            ChangeCSPage(SummaryTabs.ContractProduct, Session("ActiveContract"), curUser.EmployeeID)
            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractProduct & "&id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub cmdRefresh_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Session("CP_Filter") = txtFilter.Text

            doCPRefresh()
        End Sub

        Private Sub doCPRefresh()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
            Dim sql As String = "select [categoryId] from contract_details where [contractId] = @conId"

            db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))
            Dim catId As Integer = db.getIntSum(sql)

            GetCPRecs(catId)
        End Sub

        <WebMethod()> _
        Public Shared Function SetCPStatus(ByVal CPId As Integer, ByVal CPStatus As Integer) As String
            Dim appinfo As HttpApplication = CType(HttpContext.Current.ApplicationInstance, HttpApplication)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            Dim sql As New System.Text.StringBuilder
            Dim ARec As New cAuditRecord
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.ContractProducts, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection

            db.DBOpen(fws, False)

            ARec.Action = cFWAuditLog.AUDIT_UPDATE
            ARec.DataElementDesc = "CON.PROD.ARCHIVE STATUS"

            sql.Append("SELECT ")
            sql.Append("[contract_details].[Contract Key], ")
            sql.Append("[productDetails].[ProductName], ")
            sql.Append("[contract_productdetails].* ")
            sql.Append("FROM [contract_productdetails] ")
            sql.Append("LEFT JOIN [contract_details] ON [contract_details].[contractId] = [contract_productdetails].[contractId] ")
            sql.Append("LEFT JOIN [productDetails] ON [productDetails].[productId] = [contract_productdetails].[productId] ")
            sql.Append("WHERE [contractProductId] = @cpId")
            db.AddDBParam("cpId", CPId, True)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            If db.glNumRowsReturned > 0 Then
                ARec.ContractNumber = db.GetFieldValue(db.glDBWorkA, "contractKey", 0, 0)
                ARec.ElementDesc = db.GetFieldValue(db.glDBWorkA, "productName", 0, 0)

                sql.Remove(0, sql.Length)

                If CPStatus = 1 Then
                    ' archive the contract product
                    ARec.PreVal = "LIVE"
                    ARec.PostVal = "ARCHIVED"
                    sql.Append("UPDATE [contract_productdetails] SET [archiveStatus] = 1, [archiveDate] = getdate() WHERE [contractProductId] = @cpId")
                Else
                    ' un-archive the contract product
                    ARec.PostVal = "LIVE"
                    ARec.PreVal = "ARCHIVED"
                    sql.Append("UPDATE [contract_productdetails] SET [archiveStatus] = 0, [archiveDate] = NULL WHERE [contractProductId] = @cpId")
                End If

                db.AddDBParam("cpId", CPId, True)
                db.ExecuteSQL(sql.ToString)

                ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, CPId)

                ' update the Total Maintenance Value field in the Contract Details table
                sql.Remove(0, sql.Length)
                Dim curContractId As Integer = db.GetFieldValue(db.glDBWorkA, "ContractId", 0, 0)

                Dim strUpdateCPfunction As String = "EXECUTE dbo.UpdateCPAnnualCost @conId, @acv"
                If params.EnableRecharge Then
                    If params.AutoUpdateCVRechargeLive Then
                        strUpdateCPfunction = "EXECUTE dbo.UpdateRechargeCPAnnualCost @conId, @acv"
                    End If
                End If

                db.AddDBParam("conId", curContractId, True)
                db.AddDBParam("acv", IIf(fws.glAutoUpdateCV, 1, 0), False)
                db.ExecuteSQL(strUpdateCPfunction)
            End If

            db.DBClose()
            db = Nothing

            Return "OK"
        End Function

        Protected Sub rdoCPStatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Session("CPStatus") = rdoCPStatus.SelectedValue

            doCPRefresh()
        End Sub

        <WebMethod()> _
        Public Shared Function GetCPDetail(ByVal CPId As Integer) As String
            Dim strContent As New System.Text.StringBuilder
            Dim db As New cFWDBConnection
            Dim appinfo As HttpApplication = CType(HttpContext.Current.ApplicationInstance, HttpApplication)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            Dim CPDetailTitle As String = ""

            db.DBOpen(fws, False)
            Dim sql As New System.Text.StringBuilder
            sql.Append("SELECT ")
            'sql.Append("[contract_details].[ContractKey], ")
            sql.Append("[productDetails].[ProductName] ")
            'sql.Append("[contract_productdetails].* ")
            sql.Append("FROM [contract_productdetails] ")
            'sql.Append("LEFT JOIN [contract_details] ON [contract_details].[Contract Id] = [contract_productdetails].[Contract Id] ")
            sql.Append("LEFT JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[ProductId] ")
            sql.Append("WHERE [ContractProductId] = @cpId")
            db.AddDBParam("cpId", CPId, True)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            If db.glNumRowsReturned > 0 Then
                CPDetailTitle = db.GetFieldValue(db.glDBWorkA, "ProductName", 0, 0)
            End If

            strContent.Append(GetProduct(db, CPId, appinfo, curUser))

            Dim retStr As New System.Text.StringBuilder
            retStr.Append(createBroadcastMessage(strContent, CPDetailTitle))

            db.DBClose()
            db = Nothing
            Return retStr.ToString
        End Function

        Protected Sub lnkAddCPTask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddCPTask.Click
            Dim varURL As String
            varURL = "tid=0&rid=" & Session("ActiveContractProduct") & "&rtid=" & AppAreas.CONTRACT_PRODUCTS
            Session("TaskRetURL") = Server.UrlEncode("~/ContractSummary.aspx?cpaction=edit&tab=" & SummaryTabs.ContractProduct & "&cpid=" & Session("ActiveContractProduct"))
            Response.Redirect("~/shared/tasks/ViewTask.aspx?" & varURL, True)
        End Sub

#End Region

#Region "Invoice Details Code"
        Private Sub InvoiceDetails_Load()
            Dim tmpId As Integer
            Dim action As String
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            tmpId = 0
            action = Session("IDAction")
            If action Is Nothing Then
                action = Request.QueryString("idaction")
                Session("IDAction") = action
            End If

            Select Case action
                Case "edit"
                    tmpId = Request.QueryString("invid")
                    invID.Text = tmpId.ToString.Trim
                    ShowInvDetail(curUser, tmpId)

                Case "add"
                    ShowInvDetail(curUser)

                Case "move"
                    tmpId = Integer.Parse(Request.QueryString("fid"))
                    ' bring in the forecast information and pre-populate the invoice
                    Session("IDAction") = "move"
                    invID.Text = tmpId.ToString.Trim
                    ShowInvDetail(curUser)
                    PopulateFromForecast(curUser, tmpId)

                Case "notes"
                    Session("NoteReturnURL") = "ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & Session("ActiveContract")
                    Session("IDAction") = Nothing
                    Response.Redirect("ViewNotes.aspx?notetype=Invoice&invoiceid=" & Request.QueryString("invid") & "&item=" & Request.QueryString("invnum") & "&id=-1")

                Case Else
                    'Master.enablenavigation = False
                    Master.useCloseNavigationMsg = True
                    Master.RefreshBreadcrumbInfo()

                    GetIDTable(curUser)
            End Select

            lblIDTitle.Text = Session("contractDescription")

            If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Invoices, False) Then
                lnkNew.ToolTip = "Insert a new invoice to the contract"
                lnkNew.Attributes.Add("onmouseover", "window.status='Insert a new invoice to the contract';return true;")
                lnkNew.Attributes.Add("onmouseout", "window.status='Done';")
            End If

            If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Invoices, False) Then
                cmdIDUpdate.ToolTip = "Amend any changes to the current invoice"
                cmdIDUpdate.Attributes.Add("onmouseover", "window.status='Amend any changes to the current invoice';return true;")
                cmdIDUpdate.Attributes.Add("onmouseout", "window.status='Done';")
            End If

            If Session("IDAction") = "edit" Or Session("IDAction") = "add" Then
                ' put a cancel button on the screen
                cmdIDCancel.ToolTip = "Abandon add/edit and return to invoice list"
                cmdIDCancel.Attributes.Add("onmouseover", "window.status='Abandon add/edit and return to invoice list';return true;")
                cmdIDCancel.Attributes.Add("onmouseout", "window.status='Done';")
            End If

            cmdIDClose.ToolTip = "Exit and return to Home page"
            cmdIDClose.Attributes.Add("onclick", "javascript:GoHome();")
            cmdIDClose.Attributes.Add("onmouseover", "window.status='Exit and return to Home page';return true;")
            cmdIDClose.Attributes.Add("onmouseout", "window.status='Done';")

            If Session("IDAction") = "edit" Or Session("IDAction") = "add" Or Session("IDAction") = "move" Then
                lnkNew.Visible = False
            End If
        End Sub

        <WebMethod()> _
        Public Shared Function DeleteInvRec(ByVal invId As Integer, ByVal curContractId As Integer) As String
            Dim db As New cFWDBConnection
            Dim appinfo As HttpApplication = CType(HttpContext.Current.ApplicationInstance, HttpApplication)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim ARec As New cAuditRecord
            Dim ALog As New cAuditLog(curUser.AccountID, curUser.EmployeeID)

            Try
                db.DBOpen(fws, False)
                db.FWDb("R2", "invoices", "InvoiceId", invId, "", "", "", "", "", "", "", "", "", "")

                If db.FWDb2Flag = True Then
                    db.FWDb("R3", "contract_details", "contractId", db.FWDbFindVal("contractId", 2), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        ARec.ContractNumber = db.FWDbFindVal("Contract Key", 3)
                    Else
                        ARec.ContractNumber = ""
                    End If
                    ARec.Action = cFWAuditLog.AUDIT_DEL
                    ARec.DataElementDesc = "INVOICE DETAILS"
                    ARec.ElementDesc = "INV:" & db.FWDbFindVal("invoiceNumber", 2)
                    ARec.PreVal = ""
                    ARec.PostVal = ""

                    db.FWDb("D", "invoiceLog", "invoiceId", invId, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "invoiceNotes", "invoiceId", invId, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "invoiceProductDetails", "invoiceId", invId, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "invoices", "invoiceId", invId, "", "", "", "", "", "", "", "", "", "")

                    ALog.deleteRecord(SpendManagementElement.Invoices, ARec.DataElementDesc, ARec.ElementDesc)

                    ContractRoutines.AddContractHistory(curUser.Account.accountid, cAccounts.getConnectionString(curUser.Account.accountid), curUser.Employee, SummaryTabs.InvoiceDetail, curContractId, ARec)
                End If
                db.DBClose()
                db = Nothing
            Catch ex As Exception

            End Try

            Return "OK"
        End Function

        Private Sub LoadIDFilter()
            Dim x As Integer

            lstInvArchived.Items.Clear()
            lstInvArchived.Items.Add(New ListItem("Live", 0))
            lstInvArchived.Items.Add(New ListItem("Archived", 1))
            lstInvArchived.Items.Add(New ListItem("Both", 2))

            If Not Session("InvArchiveFlag") Is Nothing Then
                x = lstInvArchived.SelectedItem.Value
                lstInvArchived.ClearSelection()
                lstInvArchived.Items.FindByValue(Session("InvArchiveFlag")).Selected = True

                'If Session("InvArchiveFlag") <> x Then
                '    lstInvArchived.Items.FindByValue(x).Selected = False
                'End If
            Else
                Session("InvArchiveFlag") = lstInvArchived.SelectedItem.Value
            End If
        End Sub

        Private Sub GetIDTable(ByVal curUser As CurrentUser)
            Dim hasdata As Boolean = False
            Dim currencyId As Integer
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fwparams As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            db.FWDb("R2", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag = True Then
                lblIDTitle.Text = " - " & db.FWDbFindVal("contractDescription", 2)
                Session("contractDescription") = lblIDTitle.Text
                If db.FWDbFindVal("contractCurrency", 2) <> String.Empty Then
                    currencyId = Integer.Parse(db.FWDbFindVal("contractCurrency", 2))
                Else
                    currencyId = 0
                End If
            Else
                lblIDTitle.Text = "Invoice Details"
                currencyId = 0
            End If

            LoadIDFilter()

            db.RunSQL(GetInvSQL(Session("InvArchiveFlag")), db.glDBWorkA, False, "", False)

            cLocks.UpdateCacheExpireItem(Cache, "InvTable", fwparams.CacheTimeout, GetInvoiceTable(curUser, db.glDBWorkA, currencyId))
            litInvoiceData.Text = Cache("InvTable")

            db.DBClose()
        End Sub

        Private Function GetInvSQL(ByVal invStatus As Integer) As String
            Dim sql As New System.Text.StringBuilder
            sql.Append("SELECT [invoiceId],[contractId],ISNULL([invoiceNumber],'') AS [invoiceNumber],[poNumber],[poMaxValue],[poStartDate],[poExpiryDate],ISNULL([totalAmount],0) AS [totalAmount]")
            sql.Append(",ISNULL([Comment],'') AS [Comment],[invoicestatustype].[description],[invoiceStatusType].[isarchive],[receivedDate],[dueDate],[paidDate],[paymentReference]" & vbNewLine)
            sql.Append("FROM [invoices]" & vbNewLine)
            sql.Append("LEFT OUTER JOIN [invoiceStatusType] ON [invoiceStatusType].[invoiceStatusTypeId] = [invoices].[invoiceStatus] ")
            sql.Append(" WHERE [contractId] = " & Session("ActiveContract"))

            If lstInvArchived.SelectedItem.Value <> 2 Then
                sql.Append(" AND [invoiceStatusType].[isarchive] = " & invStatus.ToString)
            End If

            sql.Append(" ORDER BY [receivedDate] DESC")

            Return sql.ToString
        End Function

        Protected Sub cmdIDUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fwparams As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            cmdIDUpdate.Enabled = False

            UpdateInvEntry(curUser)

            invEditFieldsPanel.Visible = False
            panelIDFilter.Visible = True
            cmdIDClose.Visible = True
            cmdIDUpdate.Visible = False
            cmdIDCancel.Visible = False
            lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Invoices, False)

            Session("IDAction") = Nothing

            GetIDTable(curUser)

            db.DBClose()
            db = Nothing
        End Sub

        Protected Sub cmdIDCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            invEditFieldsPanel.Visible = False
            panelIDFilter.Visible = True
            cmdIDClose.Visible = True
            cmdIDUpdate.Visible = False
            cmdIDCancel.Visible = False
            lnkAddIDTask.Visible = False

            Session("IDAction") = Nothing

            ChangeCSPage(SummaryTabs.InvoiceDetail, Session("ActiveContract"), curUser.EmployeeID)
            LoadIDFilter()
            litInvoiceData.Text = Cache("InvTable")

            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & Session("ActiveContract"), True)
        End Sub

        Private Sub ShowInvDetail(ByVal curUser As CurrentUser, Optional ByVal InvId As Integer = 0)
            invEditFieldsPanel.Visible = True
            panelIDFilter.Visible = False
            cmdIDClose.Visible = False
            cmdIDUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Invoices, False)
            cmdIDCancel.Visible = True
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            Master.enablenavigation = False
            Master.useCloseNavigationMsg = False
            Master.RefreshBreadcrumbInfo()

            If InvId = 0 Then
                txtInvoiceNumber.Text = ""
                txtIDAmount.Text = "0"
                txtReceivedDate.Text = ""
                txtDueDate.Text = ""
                txtInvComment.Text = ""
                txtInvoicePaidDate.Text = ""
                txtPaymentRef.Text = ""
                txtIDCoverEnd.Text = ""
                txtIDPONumber.Text = ""
                dateIDPOStart.Text = ""
                dateIDPOExpiry.Text = ""
                txtIDPOMaxValue.Text = "0"
                lnkAddIDTask.Visible = False


                If params.PONumberGenerate Then
                    Dim curSeqNum As Integer = 0
                    Dim formattedNum As String
                    Dim poNumFormat As String = "{0}"

                    poNumFormat = params.PONumberFormat

                    Integer.TryParse(params.PONumberSequence, curSeqNum)
                    formattedNum = String.Format(poNumFormat, curSeqNum)
                    If formattedNum = "" Then
                        formattedNum = "1"
                    End If

                    txtIDPONumber.Text = formattedNum & " (provisional)"
                    txtIDPONumber.ToolTip = "Provisional assignment only. May change if another PO generated before record saved"
                End If
            Else
                lnkAddIDTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)

                Dim newIdx As Integer
                Dim tmpStr As String

                db.FWDb("R", "invoices", "InvoiceId", InvId, "", "", "", "", "", "", "", "", "", "")

                If db.FWDbFlag = True Then
                    txtInvoiceNumber.Text = db.FWDbFindVal("invoiceNumber", 1)

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                        txtIDAmount.Text = db.FWDbFindVal("totalAmount", 1)
                    End If

                    tmpStr = db.FWDbFindVal("receivedDate", 1)
                    If tmpStr <> "" Then
                        txtReceivedDate.Text = CDate(tmpStr).ToShortDateString
                    Else
                        txtReceivedDate.Text = ""
                    End If

                    tmpStr = db.FWDbFindVal("dueDate", 1)
                    If tmpStr <> "" Then
                        txtDueDate.Text = CDate(tmpStr).ToShortDateString
                    Else
                        txtDueDate.Text = ""
                    End If

                    tmpStr = db.FWDbFindVal("paidDate", 1)
                    If tmpStr <> "" Then
                        txtInvoicePaidDate.Text = CDate(tmpStr).ToShortDateString
                    Else
                        txtInvoicePaidDate.Text = ""
                    End If

                    tmpStr = db.FWDbFindVal("coverPeriodEnd", 1)
                    If tmpStr <> "" Then
                        txtIDCoverEnd.Text = CDate(tmpStr).ToShortDateString
                    Else
                        txtIDCoverEnd.Text = ""
                    End If

                    txtInvComment.Text = db.FWDbFindVal("comment", 1)

                    'oldIdx = lstInvSalesTax.SelectedItem.Value
                    newIdx = SMRoutines.CheckListIndex(db.FWDbFindVal("salesTaxRate", 1))
                    lstInvSalesTax.ClearSelection()
                    lstInvSalesTax.Items.FindByValue(newIdx).Selected = True
                    'If oldIdx <> newIdx Then
                    '    lstInvSalesTax.Items(oldIdx).Selected = False
                    'End If

                    'oldIdx = lstStatus.SelectedItem.Value
                    newIdx = SMRoutines.CheckListIndex(db.FWDbFindVal("invoiceStatus", 1))

                    lstStatus.ClearSelection()
                    lstStatus.Items.FindByValue(newIdx).Selected = True

                    'If oldIdx <> newIdx Then
                    '    lstStatus.Items(oldIdx).Selected = False
                    'End If

                    txtPaymentRef.Text = db.FWDbFindVal("paymentReference", 1)
                    txtIDPONumber.Text = db.FWDbFindVal("poNumber", 1)
                    If db.FWDbFindVal("poStartDate", 1) <> "" Then
                        dateIDPOStart.Text = Format(CDate(db.FWDbFindVal("poStartDate", 1)), cDef.DATE_FORMAT)
                    Else
                        dateIDPOStart.Text = ""
                    End If
                    If db.FWDbFindVal("poExpiryDate", 1) <> "" Then
                        dateIDPOExpiry.Text = Format(CDate(db.FWDbFindVal("poExpiryDate", 1)), cDef.DATE_FORMAT)
                    Else
                        dateIDPOExpiry.Text = ""
                    End If

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                        txtIDPOMaxValue.Text = db.FWDbFindVal("poMaxValue", 1)
                    End If
                End If
            End If

            If params.PONumberGenerate Then
                txtIDPONumber.Enabled = False
            End If
            db.DBClose()
        End Sub

        Private Sub UpdateInvEntry(ByVal curUser As CurrentUser)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)
            Dim ARec As New cAuditRecord
            Dim firstChange, statusChanged As Boolean
            Dim tmpVal, tmpVal2 As Object
            Dim tmpStr, tmpStr2 As String
            Dim refreshURL As String = ""
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Invoices, curUser.CurrentSubAccountId)
            Dim ARecContrID As String = String.Empty

            ' update existing or add new invoice
            db.FWDb("R3", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")

            Select Case Session("IDAction")
                Case "add", "move"
                    db.SetFieldValue("contractId", Session("ActiveContract"), "N", True)
                    db.SetFieldValue("invoiceNumber", txtInvoiceNumber.Text, "S", False)

                    If IsDate(dateIDPOStart.Text) = True Then
                        db.SetFieldValue("poStartDate", dateIDPOStart.Text, "D", False)
                    End If
                    If IsDate(dateIDPOExpiry.Text) = True Then
                        db.SetFieldValue("poExpiryDate", dateIDPOExpiry.Text, "D", False)
                    End If

                    'Only save the amount if the user is allowed to view the financials
                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                        db.SetFieldValue("poMaxValue", txtIDPOMaxValue.Text, "N", False)
                        db.SetFieldValue("totalAmount", txtIDAmount.Text, "N", False)
                    End If
                    db.SetFieldValue("salesTaxRate", lstInvSalesTax.SelectedItem.Value, "N", False)
                    If lstStatus.SelectedItem.Value <> "0" Then
                        db.SetFieldValue("invoiceStatus", lstStatus.SelectedItem.Value, "N", False)
                    End If
                    db.SetFieldValue("comment", txtInvComment.Text, "S", False)
                    If IsDate(txtDueDate.Text) = True Then
                        db.SetFieldValue("dueDate", txtDueDate.Text, "D", False)
                    End If
                    If IsDate(txtReceivedDate.Text) = True Then
                        db.SetFieldValue("receivedDate", txtReceivedDate.Text, "D", False)
                    End If
                    If IsDate(txtInvoicePaidDate.Text) = True Then
                        db.SetFieldValue("paidDate", txtInvoicePaidDate.Text, "D", False)
                    End If
                    If IsDate(txtIDCoverEnd.Text) = True Then
                        db.SetFieldValue("coverPeriodEnd", txtIDCoverEnd.Text, "D", False)
                    End If
                    db.SetFieldValue("paymentReference", txtPaymentRef.Text, "S", False)

                    ' if auto-generating contract id, then get next available contract id and insert into contract number field
                    If Session("IDAction") <> "move" Then
                        Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
                        If params.PONumberGenerate Then
                            Dim curSeqNum As Integer = 0
                            Dim formattedNum As String
                            Dim poNumFormat As String = params.PONumberFormat
                            If poNumFormat = "" Then
                                poNumFormat = "{0}"
                            End If

                            Integer.TryParse(params.PONumberSequence, curSeqNum)
                            formattedNum = String.Format(poNumFormat, curSeqNum)
                            subaccs.IncrementPONumber(curUser.CurrentSubAccountId, curUser.EmployeeID)

                            db.SetFieldValue("poNumber", formattedNum, "S", False)
                        Else
                            db.SetFieldValue("poNumber", txtIDPONumber.Text, "S", False)
                        End If
                    Else
                        db.SetFieldValue("poNumber", txtIDPONumber.Text, "S", False)
                    End If

                    db.SetFieldValue("createdBy", curUser.EmployeeID, "N", False)
                    db.SetFieldValue("createdOn", Date.UtcNow, "D", False)
                    db.FWDb("W", "invoices", "", "", "", "", "", "", "", "", "", "", "", "")

                    Dim tmpInvId As Integer
                    If db.glIdentity <> 0 Then
                        tmpInvId = db.glIdentity

                        ' store initial invoice status
                        db.SetFieldValue("invoiceId", db.glIdentity, "N", True)
                        db.SetFieldValue("dateChanged", Now(), "D", False)
                        db.SetFieldValue("userId", curUser.EmployeeID, "N", False)
                        If lstStatus.SelectedItem.Value <> "0" Then
                            db.SetFieldValue("invoiceStatus", lstStatus.SelectedItem.Value, "N", False)
                        End If
                        db.SetFieldValue("Comment", "First entry", "S", False)
                        db.FWDb("W", "invoiceLog", "", "", "", "", "", "", "", "", "", "", "", "")
                    End If

                    If db.FWDb3Flag Then
                        ARec.ContractNumber = db.FWDbFindVal("contractKey", 3)
                    End If
                    ARec.Action = cFWAuditLog.AUDIT_ADD
                    ARec.DataElementDesc = "CONTRACT FINANCIALS"
                    ARec.ElementDesc = "INV:" & txtInvoiceNumber.Text.Trim
                    ARec.PostVal = ""
                    ARec.PreVal = ""
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, tmpInvId)

                    ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.InvoiceDetail, Session("ActiveContract"), ARec)
                    invEditFieldsPanel.Visible = False

                    If Session("IDAction") = "move" Then
                        ' copy across any product breakdown information
                        Dim sql As String
                        Dim drow As DataRow

                        sql = "SELECT * FROM [contract_forecastproducts] WHERE [forecastId] = " & invID.Text
                        db.RunSQL(sql, db.glDBWorkB, False, "", False)

                        For Each drow In db.glDBWorkB.Tables(0).Rows
                            db.SetFieldValue("invoiceId", tmpInvId, "N", True)
                            db.SetFieldValue("contractId", Session("ActiveContract"), "N", False)
                            db.SetFieldValue("productId", drow.Item("productId"), "N", False)
                            db.SetFieldValue("productInvoiceAmount", drow.Item("productAmount"), "N", False)
                            db.FWDb("W", "invoiceProductDetails", "", "", "", "", "", "", "", "", "", "", "", "")
                        Next

                        ' delete the forecast now it has been moved, only if KEEPFORECAST=0
                        Dim app As cFWSettings
                        app = fws
                        If app.glKeepForecast = 0 Then
                            ' delete the original forecast record
                            db.FWDb("D", "contract_forecastdetails", "contractForecastId", Val(invID.Text), "", "", "", "", "", "", "", "", "", "")
                            db.FWDb("D", "contract_forecastproducts", "forecastId", Integer.Parse(invID.Text), "", "", "", "", "", "", "", "", "", "")
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_DEL
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            If db.FWDb3Flag Then
                                ARec.ContractNumber = db.FWDbFindVal("contractKey", 3)
                            End If
                            ARec.ElementDesc = "FORECAST->INVOICE"
                            ARec.PostVal = "INV:" & txtInvoiceNumber.Text.Trim
                            ARec.PreVal = ""
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, tmpInvId)
                        End If
                        refreshURL = "ContractSummary.aspx?id=" & Trim(Session("ActiveContract")) & "&tab=" & SummaryTabs.InvoiceDetail
                    End If

                Case "edit"
                    ' get the original record for comparison
                    Dim arrayAuditRecs As New List(Of cAuditRecord)

                    db.FWDb("R2", "invoices", "invoiceId", invID.Text, "", "", "", "", "", "", "", "", "", "")

                    If db.FWDb2Flag = True Then
                        firstChange = True
                        statusChanged = False

                        ARec = New cAuditRecord
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        If db.FWDb3Flag Then
                            ARec.ContractNumber = db.FWDbFindVal("Contract Key", 3)
                            ARecContrID = db.FWDbFindVal("Contract Key", 3)
                        End If
                        ARec.DataElementDesc = "CONTRACT FINANCIALS"

                        tmpVal = db.FWDbFindVal("invoiceNumber", 2).Trim
                        If tmpVal <> txtInvoiceNumber.Text.Trim Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "INVOICE NUMBER"
                            ARec.PreVal = tmpVal
                            ARec.PostVal = txtInvoiceNumber.Text.Trim
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("invoiceNumber", Trim(txtInvoiceNumber.Text), "S", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpStr = db.FWDbFindVal("poNumber", 2).Trim
                        If tmpStr <> txtIDPONumber.Text.Trim Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "PO Number"
                            ARec.PreVal = tmpStr
                            ARec.PostVal = txtIDPONumber.Text.Trim
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("poNumber", Trim(txtIDPONumber.Text), "S", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpStr = Trim(db.FWDbFindVal("coverPeriodEnd", 2))
                        tmpStr2 = Trim(txtIDCoverEnd.Text)
                        If tmpStr <> "" Then
                            tmpVal = CStr(Format(Date.Parse(tmpStr), cDef.DATE_FORMAT))
                        Else
                            tmpVal = ""
                        End If

                        If tmpStr2 <> "" Then
                            tmpVal2 = CStr(Format(Date.Parse(tmpStr2), cDef.DATE_FORMAT))
                        Else
                            tmpVal2 = ""
                        End If

                        If tmpVal <> tmpVal2 Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "COVER PERIOD END"
                            ARec.PostVal = tmpVal2
                            ARec.PreVal = tmpVal
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("coverPeriodEnd", tmpVal2, "D", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpStr = Trim(db.FWDbFindVal("poStartDate", 2))
                        tmpStr2 = Trim(dateIDPOStart.Text)
                        If tmpStr <> "" Then
                            tmpVal = CStr(Format(Date.Parse(tmpStr), cDef.DATE_FORMAT))
                        Else
                            tmpVal = ""
                        End If

                        If tmpStr2 <> "" Then
                            tmpVal2 = CStr(Format(Date.Parse(tmpStr2), cDef.DATE_FORMAT))
                        Else
                            tmpVal2 = ""
                        End If

                        If tmpVal <> tmpVal2 Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "PO START DATE"
                            ARec.PostVal = tmpVal2
                            ARec.PreVal = tmpVal
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("poStartDate", tmpVal2, "D", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpStr = Trim(db.FWDbFindVal("poExpiryDate", 2))
                        tmpStr2 = Trim(dateIDPOExpiry.Text)
                        If tmpStr <> "" Then
                            tmpVal = CStr(Format(Date.Parse(tmpStr), cDef.DATE_FORMAT))
                        Else
                            tmpVal = ""
                        End If

                        If tmpStr2 <> "" Then
                            tmpVal2 = CStr(Format(Date.Parse(tmpStr2), cDef.DATE_FORMAT))
                        Else
                            tmpVal2 = ""
                        End If

                        If tmpVal <> tmpVal2 Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "PO EXPIRY DATE"
                            ARec.PostVal = tmpVal2
                            ARec.PreVal = tmpVal
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("poExpiryDate", tmpVal2, "D", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        'Only save the amount if the user is allowed to view the financials
                        If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                            tmpVal = db.FWDbFindVal("poMaxValue", 2)
                            If tmpVal <> txtIDPOMaxValue.Text Then
                                ARec = New cAuditRecord
                                ARec.Action = cFWAuditLog.AUDIT_UPDATE
                                ARec.ContractNumber = ARecContrID
                                ARec.DataElementDesc = "CONTRACT FINANCIALS"
                                ARec.ElementDesc = "PO MAX VALUE"
                                ARec.PreVal = tmpVal
                                ARec.PostVal = txtIDAmount.Text
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, invID.Text)

                                db.SetFieldValue("poMaxValue", txtIDPOMaxValue.Text, "N", firstChange)
                                arrayAuditRecs.Add(ARec)
                                firstChange = False
                            End If

                            tmpVal = db.FWDbFindVal("totalAmount", 2)
                            If tmpVal <> txtIDAmount.Text Then
                                ARec = New cAuditRecord
                                ARec.Action = cFWAuditLog.AUDIT_UPDATE
                                ARec.ContractNumber = ARecContrID
                                ARec.DataElementDesc = "CONTRACT FINANCIALS"
                                ARec.ElementDesc = "INVOICE AMOUNT"
                                ARec.PreVal = tmpVal
                                ARec.PostVal = txtIDAmount.Text
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, invID.Text)

                                db.SetFieldValue("totalAmount", txtIDAmount.Text, "N", firstChange)
                                arrayAuditRecs.Add(ARec)
                                firstChange = False
                            End If
                        End If

                        tmpStr = Trim(db.FWDbFindVal("receivedDate", 2))
                        tmpStr2 = Trim(txtReceivedDate.Text)
                        If tmpStr <> "" Then
                            tmpVal = CStr(Format(Date.Parse(tmpStr), cDef.DATE_FORMAT))
                        Else
                            tmpVal = ""
                        End If

                        If tmpStr2 <> "" Then
                            tmpVal2 = CStr(Format(Date.Parse(tmpStr2), cDef.DATE_FORMAT))
                        Else
                            tmpVal2 = ""
                        End If

                        If tmpVal <> tmpVal2 Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "INV.RCVD DATE"
                            ARec.PostVal = tmpVal2
                            ARec.PreVal = tmpVal
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("receivedDate", tmpVal2, "D", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpStr = Trim(db.FWDbFindVal("dueDate", 2))
                        If tmpStr <> "" Then
                            If IsDate(tmpStr) = True Then
                                tmpVal = CStr(Format(Date.Parse(tmpStr), cDef.DATE_FORMAT))
                            Else
                                tmpVal = ""
                            End If
                        Else
                            tmpVal = ""
                        End If

                        tmpStr2 = Trim(txtDueDate.Text)
                        If tmpStr2 <> "" Then
                            If IsDate(tmpStr2) = True Then
                                tmpVal2 = CStr(Format(Date.Parse(tmpStr2), cDef.DATE_FORMAT))
                            Else
                                tmpVal2 = ""
                            End If
                        Else
                            tmpVal2 = ""
                        End If

                        If tmpVal <> tmpVal2 Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "INV.DUE DATE"
                            ARec.PreVal = tmpVal
                            ARec.PostVal = tmpVal2
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("dueDate", tmpVal2, "D", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpStr = Trim(db.FWDbFindVal("paidDate", 2))
                        If tmpStr <> "" Then
                            tmpVal = CStr(Format(Date.Parse(tmpStr), cDef.DATE_FORMAT))
                        Else
                            tmpVal = ""
                        End If

                        tmpStr2 = Trim(txtInvoicePaidDate.Text)
                        If tmpStr2 <> "" Then
                            tmpVal2 = CStr(Format(CDate(tmpStr2), cDef.DATE_FORMAT))
                        Else
                            tmpVal2 = ""
                        End If

                        If tmpVal <> tmpVal2 Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "INV.PAID DATE"
                            ARec.PreVal = tmpVal
                            ARec.PostVal = tmpVal2
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("paidDate", tmpVal2, "D", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpVal = db.FWDbFindVal("salesTaxRate", 2)
                        If tmpVal <> lstInvSalesTax.SelectedItem.Value Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "TAX RATE"
                            ARec.PreVal = lstInvSalesTax.Items.FindByValue(SMRoutines.CheckListIndex(tmpVal)).Text
                            ARec.PostVal = lstInvSalesTax.SelectedItem.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("salesTaxRate", lstInvSalesTax.SelectedItem.Value, "N", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpVal = db.FWDbFindVal("invoiceStatus", 2)
                        If tmpVal <> lstStatus.SelectedItem.Value Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "INV.STATUS"
                            ARec.PreVal = lstStatus.Items.FindByValue(SMRoutines.CheckListIndex(tmpVal)).Text
                            ARec.PostVal = lstStatus.SelectedItem.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            If lstStatus.SelectedItem.Value = "0" Then
                                db.SetFieldValue("invoiceStatus", DBNull.Value, "#", firstChange)
                            Else
                                db.SetFieldValue("invoiceStatus", lstStatus.SelectedItem.Value, "N", firstChange)
                            End If

                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                            statusChanged = True
                        End If

                        tmpVal = db.FWDbFindVal("Comment", 2)
                        If tmpVal <> txtInvComment.Text Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "INV.COMMENT"
                            ARec.PreVal = ""
                            ARec.PostVal = Left(txtInvComment.Text, 60)
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("Comment", txtInvComment.Text, "S", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        tmpVal = db.FWDbFindVal("paymentReference", 2)
                        If tmpVal <> txtPaymentRef.Text Then
                            ARec = New cAuditRecord
                            ARec.Action = cFWAuditLog.AUDIT_UPDATE
                            ARec.ContractNumber = ARecContrID
                            ARec.DataElementDesc = "CONTRACT FINANCIALS"
                            ARec.ElementDesc = "PAYMENT REF."
                            ARec.PreVal = tmpVal
                            ARec.PostVal = txtPaymentRef.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, invID.Text)

                            db.SetFieldValue("paymentReference", txtPaymentRef.Text, "S", firstChange)
                            arrayAuditRecs.Add(ARec)
                            firstChange = False
                        End If

                        If firstChange = False Then
                            db.FWDb("A", "invoices", "invoiceId", invID.Text, "", "", "", "", "", "", "", "", "", "")
                            ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.InvoiceDetail, Session("ActiveContract"), arrayAuditRecs)
                        End If

                        If statusChanged = True Then
                            ' store change to the invoice status in the invoice log
                            db.SetFieldValue("invoiceId", db.FWDbFindVal("invoiceId", 2), "N", True)
                            db.SetFieldValue("dateChanged", Now(), "D", False)
                            db.SetFieldValue("UserId", curUser.EmployeeID, "N", False)
                            db.SetFieldValue("invoiceStatus", lstStatus.SelectedItem.Value, "N", False)
                            db.SetFieldValue("Comment", txtInvComment.Text, "S", False)
                            db.FWDb("W", "invoiceLog", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If

                Case Else

            End Select

            Session("IDAction") = Nothing

            If refreshURL <> "" Then
                db.DBClose()
                db = Nothing

                Response.Redirect(refreshURL, True)
            End If
        End Sub

        Private Function GetInvoiceTable(ByVal curUser As CurrentUser, ByVal dset As DataSet, ByVal currencyId As Integer) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim hasdata As Boolean = False
            Dim rowalt As Boolean = False
            Dim rowClass As String = "row1"
            Dim tmpDate As String = ""
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim baseCurrency As cCurrency
            If currencyId = 0 Then
                baseCurrency = currency.getCurrencyById(params.BaseCurrency.Value)
            Else
                baseCurrency = currency.getCurrencyById(currencyId)
            End If

            With strHTML
                .Append("<table class=""datatbl"">" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                .Append("<th><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
                .Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
                .Append("<th><img src=""./icons/16/plain/document_attachment.gif"" /></th>" & vbNewLine)
                .Append("<th><img src=""./icons/16/plain/history.png"" /></th>" & vbNewLine)
                .Append("<th>Invoice Number</th>" & vbNewLine)
                .Append("<th>Invoice Amount</th>" & vbNewLine)
                .Append("<th>Due Date</th>" & vbNewLine)
                .Append("<th>Received Date</th>" & vbNewLine)
                .Append("<th>Paid Date</th>" & vbNewLine)
                .Append("<th>Invoice Status</th>" & vbNewLine)
                .Append("<th>Comment</th>" & vbNewLine)

                .Append("</tr>" & vbNewLine)
                For Each drow In dset.Tables(0).Rows
                    hasdata = True
                    rowalt = (rowalt Xor True)

                    If rowalt Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    .Append("<tr>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>")
                    If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Invoices, False) Then
                        .Append("<a href=""ContractSummary.aspx?idaction=edit&tab=" & SummaryTabs.InvoiceDetail & "&invid=" & CStr(drow.Item("InvoiceId")).Trim & """ onmouseover=""window.status='Edit this invoice';return true;"" onmouseout=""window.status='Done';"" title=""Edit this invoice""><img src=""./icons/edit.gif"" /></a>")
                    End If
                    .Append("</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>")
                    If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Invoices, False) Then
                        .Append("<a href=""javascript:DeleteInvoice(" & CStr(drow.Item("InvoiceId")).Trim & "," & Session("ActiveContract") & ");"" onmouseover=""window.status='Delete this invoice';return true;"" onmouseout=""window.status='Done';"" title=""Delete this invoice"" style=""cursor: hand;""><img src=""./icons/delete2.gif"" /></a>")
                    End If
                    .Append("</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>")
                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceNotes, False) Then
                        .Append("<a href=""ContractSummary.aspx?idaction=notes&tab=" & SummaryTabs.InvoiceDetail & "&invid=" & drow.Item("invoiceId") & "&invnum=" & drow.Item("invoiceNumber") & """ onmouseover=""window.status='Open free text notes for this invoice';return true;"" onmouseout=""window.status='Done';"" title=""Open free text notes for this invoice""><img src=""./icons/16/plain/document_attachment.png"" /></a>")
                    End If
                    .Append("</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """><a href=""ViewStatusHistory.aspx?id=" & drow.Item("invoiceId") & "&desc=" & drow.Item("invoiceNumber") & """ onmouseover=""window.status='View the status history for this invoice';return true;"" onmouseout=""window.status='Done';"" title=""View the status history for this invoice""><img src=""./icons/16/plain/history.png"" /></a></td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & drow.Item("invoiceNumber") & "</td>" & vbNewLine)

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                        .Append("<td class=""" & rowClass & """>" & currency.FormatCurrency(drow.Item("totalAmount"), baseCurrency, False) & "</td>" & vbNewLine)
                    Else
                        .Append("<td class=""" & rowClass & """>N/A</td>" & vbNewLine)
                    End If

                    .Append("<td class=""" & rowClass & """>")
                    If IsDBNull(drow.Item("dueDate")) = False And IsDate(drow.Item("dueDate")) = True Then
                        .Append(Format(CDate(drow.Item("dueDate")), cDef.DATE_FORMAT))
                    End If
                    .Append("</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>")
                    If IsDBNull(drow.Item("receivedDate")) = False And IsDate(drow.Item("receivedDate")) = True Then
                        .Append(Format(CDate(drow.Item("receivedDate")), cDef.DATE_FORMAT))
                    End If
                    .Append("</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>")
                    If IsDBNull(drow.Item("paidDate")) = False And IsDate(drow.Item("paidDate")) = True Then
                        .Append(Format(CDate(drow.Item("paidDate")), cDef.DATE_FORMAT))
                    End If
                    .Append("</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """>" & drow.Item("description") & "</td>" & vbNewLine)
                    .Append("<td class=""" & rowClass & """><textarea readonly style=""width: 150px;"" rows=""2"">" & drow.Item("Comment") & "</textarea></td>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                Next

                If hasdata = False Then
                    .Append("<tr>" & vbNewLine)
                    .Append("<td class=""row1"" colspan=""11"" align=""center"">No Invoices returned</td>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                End If
                .Append("</table>" & vbNewLine)
            End With

            Return strHTML.ToString
        End Function

        Private Sub PopulateFromForecast(ByVal curuser As CurrentUser, ByVal fId As Integer)
            Dim tmpStr As String
            Dim subaccs As New cAccountSubAccounts(curuser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curuser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curuser.Account, subaccs.getSubAccountsCollection, curuser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            db.FWDb("R2", "contract_forecastdetails", "contractForecastId", fId, "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag = True Then
                invEditFieldsPanel.Visible = True

                txtInvoiceNumber.Text = ""
                lstInvSalesTax.SelectedIndex = 0
                txtInvComment.Text = db.FWDbFindVal("Comment", 2)

                'Only save the amount if the user is allowed to view the financials
                If curuser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                    txtIDAmount.Text = db.FWDbFindVal("ForecastAmount", 2)
                    txtIDPOMaxValue.Text = db.FWDbFindVal("poMaxValue", 2)
                End If

                txtIDPONumber.Text = db.FWDbFindVal("poNumber", 2)
                If IsDate(db.FWDbFindVal("poStartDate", 2)) Then
                    dateIDPOStart.Text = CDate(db.FWDbFindVal("poStartDate", 2)).ToShortDateString
                Else
                    dateIDPOStart.Text = ""
                End If

                If IsDate(db.FWDbFindVal("poExpiryDate", 2)) Then
                    dateIDPOExpiry.Text = CDate(db.FWDbFindVal("poExpiryDate", 2)).ToShortDateString
                Else
                    dateIDPOExpiry.Text = ""
                End If

                If IsDate(db.FWDbFindVal("coverPeriodEnd", 2)) Then
                    txtIDCoverEnd.Text = CDate(db.FWDbFindVal("coverPeriodEnd", 2)).ToShortDateString
                Else
                    txtIDCoverEnd.Text = ""
                End If

                txtPaymentRef.Text = ""
                lstStatus.SelectedIndex = 0
                txtReceivedDate.Text = ""
                tmpStr = db.FWDbFindVal("paymentDate", 2)
                If tmpStr <> "" And IsDate(tmpStr) = True Then
                    txtDueDate.Text = CDate(tmpStr)
                Else
                    txtDueDate.Text = ""
                End If
                txtInvoicePaidDate.Text = ""
            Else
                invEditFieldsPanel.Visible = False
            End If


            If params.PONumberGenerate Then
                txtIDPONumber.Enabled = False
            End If

            db.DBClose()
        End Sub

        Protected Sub lstInvArchived_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)
            Session("InvArchiveFlag") = lstInvArchived.SelectedItem.Value

            Dim currencyId As Integer = 0

            db.RunSQL(GetInvSQL(Session("InvArchiveFlag")), db.glDBWorkA, False, "", False)

            db.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag Then
                If db.FWDbFindVal("Contract Currency", 2) = "" Then
                    currencyId = 0
                Else
                    currencyId = db.FWDbFindVal("Contract Currency", 2)
                End If
            End If

            Dim litData As New Literal

            litInvoiceData.Text = GetInvoiceTable(curuser, db.glDBWorkA, currencyId)

            db.DBClose()
            db = Nothing
            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub lnkAddIDTask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddIDTask.Click
            Dim varURL As String
            varURL = "tid=0&rid=" & invID.Text & "&rtid=" & AppAreas.INVOICE_DETAILS
            Session("TaskRetURL") = Server.UrlEncode("~/ContractSummary.aspx?idaction=edit&tab=" & SummaryTabs.InvoiceDetail & "&invid=" & invID.Text)
            Response.Redirect("~/shared/tasks/ViewTask.aspx?" & varURL, True)
        End Sub
#End Region

#Region "Invoice Forecast Code"
        Private Sub InvoiceForecast_Load()
            Dim tmpStr As String
            Dim action As String = ""
            Dim forecastId As Integer
            Dim displayData As Boolean = True
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            'action = Request.QueryString("ifaction")
            If Me.IsPostBack = False Then
                action = Session("IFAction")
                If action Is Nothing Then
                    action = Request.QueryString("ifaction")
                    If action Is Nothing Then
                        action = ""
                    End If
                End If
            End If

            lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.InvoiceForecasts, False)
            lnkNew.ToolTip = "Insert a new forecast entry"
            lnkNew.Attributes.Add("onmouseover", "window.status='Insert a new forecast entry';return true;")
            lnkNew.Attributes.Add("onmouseout", "window.status='Done';")

            lnkBulkUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False)
            If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False) Then
                lnkBulkUpdate.ToolTip = "Update all checked Forecasts with common properties"
                lnkBulkUpdate.Attributes.Add("onmouseover", "window.status='Update all checked Forecasts with common properties';return true;")
                lnkBulkUpdate.Attributes.Add("onmouseout", "window.status='Done';")
            End If

            If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False) Then
                lnkAddIFTask.ToolTip = "Create a task associated with the current forecasted invoice"
                lnkAddIFTask.Attributes.Add("onmouseover", "window.status='Create a task associated with the current forecasted invoice';return true;")
                lnkAddIFTask.Attributes.Add("onmouseout", "window.status='Done';")
            End If

            lnkGenerate.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.InvoiceForecasts, False)
            lnkGenerate.ToolTip = "Automatically generate forecast payments"
            lnkGenerate.Attributes.Add("onmouseover", "window.status='Automatically generate forecast payments';return true;")
            lnkGenerate.Attributes.Add("onmouseout", "window.status='Done';")

            cmdIFUpdate.ToolTip = "Amend the selected forecast entry"
            cmdIFUpdate.AlternateText = "Update"
            cmdIFUpdate.Attributes.Add("onmouseover", "window.status='Amend the selected forecast entry';return true;")
            cmdIFUpdate.Attributes.Add("onmouseout", "window.status='Done';")

            cmdIFClose.Attributes.Add("onclick", "javascript:GoHome();")

            tmpStr = "ERROR!"

            Select Case action.ToLower
                Case "callback"
                    Response.Write(GetBreakdown(curUser, Integer.Parse(Request.QueryString("cb_fid")), True))
                    Response.Flush()
                    Response.End()
                    Exit Sub

                Case "add"
                    AddNewForecast()
                    cmdIFUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.InvoiceForecasts, False)
                    displayData = False

                Case "edit"
                    forecastId = Request.QueryString("ifid")
                    Session("IFAction") = "update"
                    Session("ForecastId") = forecastId
                    ShowForecastDetail(curUser, forecastId)
                    cmdIFUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False)
                    displayData = False

                Case "bulkedit"
                    Dim BUitems As Integer
                    BUitems = Integer.Parse(Request.QueryString("buitems"))

                    If BUitems > 0 Then
                        ForecastEditFieldsPanel.Visible = True
                        IFBreakdownPanel.Visible = False
                        cmdIFClose.Visible = False

                        lblStatusMsg.Text = "*** Only update the field to be modified. " & BUitems.ToString & " products tagged for update."
                        displayData = False

                        'litBreakdownTable.Text = GetBreakdown(db, Integer.Parse(Request.QueryString("fid1")))
                        lblProductAmount.Visible = False

                        cmdBUUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False)

                        If params.PONumberGenerate Then
                            txtIFPONumber.Text = "n/a"
                            txtIFPONumber.Enabled = False
                        End If

                    Else
                        lblStatusMsg.Text = "*** No Forecast Items tagged for Bulk Update"
                    End If

                Case "generate"
                    GenerateForecasts()

                Case Else

            End Select

            If displayData = True Then
                'Master.enablenavigation = False
                Master.useCloseNavigationMsg = True
                Master.RefreshBreadcrumbInfo()

                GetForecastTable(curUser)
            Else
                litForecastData.Text = ""
                cmdIFClose.Visible = False
            End If

            db.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag Then
                lblIFTitle.Text = " - " & db.FWDbFindVal("contractDescription", 2)
                'Session("contractDescription") = lblIFTitle.Text
            Else
                lblIFTitle.Text = ""
            End If

            If Session("IFAction") = "bulk" Or Session("IFAction") = "update" Or Session("IFAction") = "add" Then
                cmdIFCancel.AlternateText = "Cancel"
                cmdIFCancel.CausesValidation = False
                cmdIFCancel.ToolTip = "Exit add / edit and return to forecasts list"
                cmdIFCancel.Attributes.Add("onmouseover", "window.status='Exit add / edit and return to forecasts list';return true;")
                cmdIFCancel.Attributes.Add("onmouseout", "window.status='Done';")

                cmdIFClose.Visible = False
            End If
        End Sub

        Private Sub GetForecastTable(ByVal curUser As CurrentUser)
            Dim currencyId As Integer = 0
            Dim sql As New System.Text.StringBuilder
            Dim hasdata As Boolean = False
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fwparams As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            sql.Append("SELECT [contract_forecastdetails].*,[contract_details].[contractCurrency] FROM [contract_forecastdetails] ")
            sql.Append("LEFT OUTER JOIN [contract_details] ON [contract_forecastdetails].[contractId] = [contract_details].[contractId] ")
            sql.Append("WHERE [contract_forecastdetails].[contractId] = @conId")
            db.AddDBParam("conId", Session("ActiveContract"), True)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            If db.glNumRowsReturned > 0 Then
                hasdata = True
                If Not db.GetFieldValue(db.glDBWorkA, "contractCurrency", 0, 0) Is DBNull.Value Then
                    currencyId = db.GetFieldValue(db.glDBWorkA, "contractCurrency", 0, 0)
                Else
                    currencyId = -1
                End If
                lnkBulkUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False)
            Else
                currencyId = -1
                lnkBulkUpdate.Visible = False
            End If

            cLocks.UpdateCacheExpireItem(Cache, "IFData", Integer.Parse(fwparams.CacheTimeout), GetBasicForecasts(curUser, db.glDBWorkA, Session("ActiveContract"), hasdata, currencyId))
            litForecastData.Text = Cache("IFData")
        End Sub

        Private Sub AddNewForecast()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            Master.enablenavigation = False
            Master.useCloseNavigationMsg = False
            Master.RefreshBreadcrumbInfo()

            txtForecastDate.Text = Today()
            txtForecastAmount.Text = "0"
            txtIFPOMaxValue.Text = "0"
            txtIFComment.Text = ""

            If params.PONumberGenerate Then
                Dim curSeqNum As Integer = 0
                Dim formattedNum As String
                Dim poNumFormat As String = "{0}"

                If params.PONumberFormat <> "" Then
                    poNumFormat = params.PONumberFormat
                End If

                curSeqNum = params.PONumberSequence
                formattedNum = String.Format(poNumFormat, curSeqNum)

                txtIFPONumber.Text = formattedNum & " (provisional)"
                txtIFPONumber.ToolTip = "Provisional assignment only. May change if another PO generated before record saved."
                txtIFPONumber.Enabled = False
            End If

            ForecastEditFieldsPanel.Visible = True
            lnkBulkUpdate.Visible = False
            lnkNew.Visible = False
        End Sub

        Private Sub AddIFRec(ByVal curUser As CurrentUser)
            Dim ARec As New cAuditRecord
            Dim ALog As New cAuditLog(curUser.Account.accountid, curUser.Employee.employeeid)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim FWDb As New cFWDBConnection
            FWDb.DBOpen(fws, False)

            FWDb.FWDb("R2", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            ARec.Action = cFWAuditLog.AUDIT_ADD
            If FWDb.FWDb2Flag = True Then
                ARec.ContractNumber = FWDb.FWDbFindVal("contractKey", 2)
            End If

            ARec.DataElementDesc = "INV.FORECAST"
            ARec.PostVal = Trim(txtForecastDate.Text) & ":" & Trim(txtForecastAmount.Text)
            ARec.PreVal = ""
            ALog.addRecord(SpendManagementElement.InvoiceForecasts, ARec.DataElementDesc, 0)

            ' if auto-generating contract id, then get next available contract id and insert into contract number field
            If params.PONumberGenerate Then
                Dim curSeqNum As Integer = 0
                Dim formattedNum As String
                Dim poNumFormat As String = "{0}"

                If params.PONumberFormat <> "" Then
                    poNumFormat = params.PONumberFormat
                End If

                curSeqNum = params.PONumberSequence
                formattedNum = String.Format(poNumFormat, curSeqNum)

                subaccs.IncrementPONumber(curUser.CurrentSubAccountId, curUser.EmployeeID)

                FWDb.SetFieldValue("poNumber", formattedNum, "S", True)
            Else
                FWDb.SetFieldValue("poNumber", txtIFPONumber.Text, "S", True)
            End If

            If IsDate(txtForecastDate.Text) = True Then
                FWDb.SetFieldValue("paymentDate", txtForecastDate.Text, "D", False)
            End If
            If IsDate(dateIFPOStart.Text) = True Then
                FWDb.SetFieldValue("poStartDate", dateIFPOStart.Text, "D", False)
            End If
            If IsDate(dateIFPOExpiry.Text) = True Then
                FWDb.SetFieldValue("poExpiryDate", dateIFPOExpiry.Text, "D", False)
            End If
            If IsDate(txtIFCoverEnd.Text) = True Then
                FWDb.SetFieldValue("coverPeriodEnd", txtIFCoverEnd.Text, "D", False)
            End If
            'Only save the amount if the user is allowed to view the financials
            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                FWDb.SetFieldValue("poMaxValue", txtIFPOMaxValue.Text, "N", False)
                FWDb.SetFieldValue("forecastAmount", txtForecastAmount.Text, "N", False)
            End If
            FWDb.SetFieldValue("contractId", Session("ActiveContract"), "N", False)
            FWDb.SetFieldValue("Comment", txtIFComment.Text, "S", False)
            FWDb.FWDb("W", "contract_forecastdetails", "", "", "", "", "", "", "", "", "", "", "", "")

            ContractRoutines.AddContractHistory(curUser.Account.accountid, cAccounts.getConnectionString(curUser.Account.accountid), curUser.Employee, SummaryTabs.InvoiceForecast, Session("ActiveContract"), ARec)

            Session("IFAction") = Nothing
            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
        End Sub

        Private Sub UpdateIFRecord(ByVal curUser As CurrentUser, ByVal tmpF_Id As Integer, Optional ByVal isBulkUpdate As Boolean = False)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            Dim ARec As New cAuditRecord
            Dim ALog As New cAuditLog(curUser.Account.accountid, curUser.Employee.employeeid)
            Dim firstchange As Boolean
            Dim tmpDate As String
            Dim arrayAuditRecs As New List(Of cAuditRecord)
            Dim ARecContrID As String

            ARecContrID = String.Empty
            ARec.Action = cFWAuditLog.AUDIT_UPDATE

            firstchange = True

            db.FWDb("R2", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag = True Then
                ARec.ContractNumber = db.FWDbFindVal("contractKey", 2)
                ARecContrID = db.FWDbFindVal("contractKey", 2)
            End If

            ARec.DataElementDesc = "INV.FORECASTS"

            db.FWDb("R2", "contract_forecastdetails", "contractForecastId", tmpF_Id, "", "", "", "", "", "", "", "", "", "")

            If db.FWDb2Flag = True Then
                If Trim(txtForecastDate.Text) <> Format(CDate(db.FWDbFindVal("paymentDate", 2)), cDef.DATE_FORMAT) Then
                    ARec.ElementDesc = "FORECAST PAYMENT DATE"
                    ARec.PostVal = txtForecastDate.Text
                    ARec.PreVal = db.FWDbFindVal("paymentDate", 2)
                    'ALog.editRecord("Contracts", ARec.ElementDesc, ARec.DataElementDesc, ARec.PreVal, ARec.PostVal)

                    db.SetFieldValue("paymentDate", txtForecastDate.Text, "D", firstchange)
                    arrayAuditRecs.Add(ARec)
                    firstchange = False
                End If

                'Only save the amount if the user is allowed to view the financials
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                    If Trim(txtForecastAmount.Text) <> Trim(db.FWDbFindVal("forecastAmount", 2)) Then
                        ARec = New cAuditRecord
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ARecContrID
                        ARec.DataElementDesc = "INV.FORECASTS"
                        ARec.ElementDesc = "FORECAST AMOUNT"
                        ARec.PostVal = txtForecastAmount.Text
                        ARec.PreVal = db.FWDbFindVal("forecastAmount", 2)
                        'ALog.editRecord("Contracts", ARec.ElementDesc, ARec.DataElementDesc, ARec.PreVal, ARec.PostVal)

                        db.SetFieldValue("forecastAmount", txtForecastAmount.Text, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If
                End If

                tmpDate = db.FWDbFindVal("coverPeriodEnd", 2)
                If tmpDate <> "" And IsDate(tmpDate) = True Then
                    tmpDate = Format(CDate(tmpDate), cDef.DATE_FORMAT)
                End If

                If txtIFCoverEnd.Text.Trim <> tmpDate Then
                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "INV.FORECASTS"
                    ARec.ElementDesc = "COVER PERIOD END"
                    ARec.PostVal = txtIFCoverEnd.Text
                    ARec.PreVal = tmpDate
                    'ALog.editRecord("Contracts", ARec.ElementDesc, ARec.DataElementDesc, ARec.PreVal, ARec.PostVal)

                    db.SetFieldValue("coverPeriodEnd", txtIFCoverEnd.Text, "D", firstchange)
                    arrayAuditRecs.Add(ARec)
                    firstchange = False
                End If

                If Trim(txtIFPONumber.Text) <> Trim(db.FWDbFindVal("poNumber", 2)) Then
                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "INV.FORECASTS"
                    ARec.ElementDesc = "poNumber"
                    ARec.PostVal = txtIFPONumber.Text
                    ARec.PreVal = db.FWDbFindVal("poNumber", 2)
                    'ALog.editRecord("Contracts", ARec.ElementDesc, ARec.DataElementDesc, ARec.PreVal, ARec.PostVal)

                    db.SetFieldValue("poNumber", txtIFPONumber.Text, "S", firstchange)
                    arrayAuditRecs.Add(ARec)
                    firstchange = False
                End If

                'Only save the amount if the user is allowed to view the financials
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                    If Trim(txtIFPOMaxValue.Text) <> Trim(db.FWDbFindVal("poMaxValue", 2)) Then
                        ARec = New cAuditRecord
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE
                        ARec.ContractNumber = ARecContrID
                        ARec.DataElementDesc = "INV.FORECASTS"
                        ARec.ElementDesc = "PO MAX VALUE"
                        ARec.PostVal = txtIFPOMaxValue.Text
                        ARec.PreVal = db.FWDbFindVal("poMaxValue", 2)
                        'ALog.editRecord("Contracts", ARec.ElementDesc, ARec.DataElementDesc, ARec.PreVal, ARec.PostVal)

                        db.SetFieldValue("poMaxValue", txtIFPOMaxValue.Text, "N", firstchange)
                        arrayAuditRecs.Add(ARec)
                        firstchange = False
                    End If
                End If

                tmpDate = Trim(db.FWDbFindVal("poStartDate", 2))
                If tmpDate <> "" And IsDate(tmpDate) = True Then
                    tmpDate = Format(CDate(tmpDate), cDef.DATE_FORMAT)
                End If

                If Trim(dateIFPOStart.Text) <> tmpDate Then
                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "INV.FORECASTS"
                    ARec.ElementDesc = "PO START DATE"
                    ARec.PostVal = dateIFPOStart.Text
                    ARec.PreVal = db.FWDbFindVal("poStartDate", 2)
                    'ALog.editRecord("Contracts", ARec.ElementDesc, ARec.DataElementDesc, ARec.PreVal, ARec.PostVal)

                    db.SetFieldValue("poStartDate", dateIFPOStart.Text, "D", firstchange)
                    arrayAuditRecs.Add(ARec)
                    firstchange = False
                End If

                tmpDate = Trim(db.FWDbFindVal("poExpiryDate", 2))
                If tmpDate <> "" And IsDate(tmpDate) = True Then
                    tmpDate = Format(CDate(tmpDate), cDef.DATE_FORMAT)
                End If

                If Trim(dateIFPOExpiry.Text) <> tmpDate Then
                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "INV.FORECASTS"
                    ARec.ElementDesc = "PO EXPIRY DATE"
                    ARec.PostVal = dateIFPOExpiry.Text
                    ARec.PreVal = db.FWDbFindVal("poExpiryDate", 2)
                    'ALog.editRecord("Contracts", ARec.ElementDesc, ARec.DataElementDesc, ARec.PreVal, ARec.PostVal)

                    db.SetFieldValue("poExpiryDate", dateIFPOExpiry.Text, "D", firstchange)
                    arrayAuditRecs.Add(ARec)
                    firstchange = False
                End If

                If Trim(txtIFComment.Text) <> Trim(db.FWDbFindVal("Comment", 2)) Then
                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "INV.FORECASTS"
                    ARec.ElementDesc = "FORECAST COMMENT"
                    ARec.PreVal = Left(db.FWDbFindVal("Comment", 2), 50)
                    ARec.PostVal = Left(txtIFComment.Text, 50)
                    'ALog.editRecord("Contracts", ARec.ElementDesc, ARec.DataElementDesc, ARec.PreVal, ARec.PostVal)

                    db.SetFieldValue("Comment", txtIFComment.Text, "S", False)
                    arrayAuditRecs.Add(ARec)
                    firstchange = False
                End If

                If firstchange = False Then
                    db.FWDb("A", "contract_forecastdetails", "contractForecastId", tmpF_Id, "", "", "", "", "", "", "", "", "", "")
                    ContractRoutines.AddContractHistory(curUser.Account.accountid, cAccounts.getConnectionString(curUser.Account.accountid), curUser.Employee, SummaryTabs.InvoiceForecast, Session("ActiveContract"), arrayAuditRecs)
                End If

                ' repeat the amendments for all ticked forecasts
                Dim sql As String
                Dim drow As DataRow

                sql = "SELECT [contractForecastId] FROM [contract_forecastdetails] WHERE [ContractId] = @conId"
                db.AddDBParam("conId", Session("ActiveContract"), True)
                db.RunSQL(sql, db.glDBWorkL, False, "", False)

                For Each drow In db.glDBWorkL.Tables(0).Rows
                    If Request.Form("IF" & Trim(drow.Item("contractForecastId"))) = "1" Then
                        ' item is ticked for bulk update
                        db.FWDb("A", "contract_forecastdetails", "contractForecastId", drow.Item("contractForecastId"), "", "", "", "", "", "", "", "", "", "")
                    End If
                Next
            End If

            Session("IFAction") = Nothing
            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
        End Sub

        Private Function GetBasicForecasts(ByVal curUser As CurrentUser, ByVal dset As DataSet, ByVal conId As Integer, ByVal hasdata As Boolean, Optional ByVal currencyId As Integer = -1) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim tmpstr As String
            Dim firstrow As Boolean
            Dim rowClass As String = "row1"
            Dim rowalt As Boolean = False

            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim baseCurrency As cCurrency = currency.getCurrencyById(params.BaseCurrency.Value)

            strHTML.Append("<table class=""datatbl"">" & vbNewLine)

            strHTML.Append("<tr>" & vbNewLine)
            'strHTML = strHTML & "<th class=""" & rowclass & """><b>Active Forecast</b></th>" & vbNewLine
            strHTML.Append("<th>BU?</th>" & vbNewLine)
            strHTML.Append("<th>Move?</th>" & vbNewLine)
            strHTML.Append("<th><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
            strHTML.Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
            strHTML.Append("<th>PO Number</th>" & vbNewLine)
            strHTML.Append("<th>PO Start</th>" & vbNewLine)
            strHTML.Append("<th>PO Expiry</th>" & vbNewLine)
            strHTML.Append("<th>PO Max Value</th>" & vbNewLine)
            strHTML.Append("<th>Payment Date</th>" & vbNewLine)
            strHTML.Append("<th>Forecast Amount</th>" & vbNewLine)
            strHTML.Append("<th>Product Breakdown</th>" & vbNewLine)
            strHTML.Append("<th>Comment</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            Dim BUitemlist As String = ""
            Dim comma As String = ""

            If hasdata = False Then
                strHTML.Append("<tr><td class=""row1"" colspan=""12"" align=""center"">No Invoice Forecasts to display</td></tr>" & vbNewLine)
            Else
                firstrow = True
                For Each drow In dset.Tables(0).Rows
                    rowalt = (rowalt Xor True)
                    If rowalt Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    strHTML.Append("<tr>" & vbNewLine)
                    'strHTML = strHTML & "<td class=""" & rowclass & """><input type=""radio"" name=""InvForSel"" value=""" & Trim(drow.Item("contractForecastId")) & """" & IIf(firstrow = True, " checked ", "") & "/></td>" & vbNewLine
                    BUitemlist = BUitemlist & comma & CStr(drow.Item("contractForecastId"))
                    comma = ","
                    strHTML.Append("<td class=""" & rowClass & """><input type=""checkbox"" name=""IFBU"" value=""" & drow.Item("contractForecastId") & """ /></td>" & vbNewLine)
                    If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False) Then
                        strHTML.Append("<td class=""" & rowClass & """><a title=""Transfer forecast into an active invoice"" href=""ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & Trim(Session("ActiveContract")) & "&idaction=move&fid=" & Trim(drow.Item("contractForecastId")) & """>Move</a></td>" & vbNewLine)
                        strHTML.Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Edit this Forecast Item';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&ifaction=edit&ifid=" & Trim(drow.Item("contractForecastId")) & """><img src=""./icons/edit.gif"" /></a></td>" & vbNewLine)
                    Else
                        strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                        strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                    End If
                    If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.InvoiceForecasts, False) Then
                        strHTML.Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Delete this Forecast Item';return true;"" onmouseout=""window.status='Done';"" href=""javascript:DelInvForecastRec(" & Trim(drow.Item("contractForecastId")) & "," & Session("ActiveContract") & ");""><img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                    Else
                        strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                    End If

                    tmpstr = IIf(IsDBNull(drow.Item("poNumber")) = True, "&nbsp;", drow.Item("poNumber"))
                    strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)

                    If IsDBNull(drow.Item("poStartDate")) = True Then
                        tmpstr = "&nbsp;"
                    Else
                        tmpstr = Format(drow.Item("poStartDate"), cDef.DATE_FORMAT)
                    End If

                    strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)

                    If IsDBNull(drow.Item("poExpiryDate")) = True Then
                        tmpstr = "&nbsp;"
                    Else
                        tmpstr = Format(drow.Item("poExpiryDate"), cDef.DATE_FORMAT)
                    End If

                    strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False) Then
                        tmpstr = IIf(IsDBNull(drow.Item("poMaxValue")) = True, "&nbsp;", currency.FormatCurrency(drow.Item("poMaxValue"), baseCurrency, False))
                    Else
                        tmpstr = "n/a"
                    End If
                    strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)

                    tmpstr = IIf(IsDBNull(drow.Item("paymentDate")) = True, "&nbsp;", Format(drow.Item("paymentDate"), cDef.DATE_FORMAT))
                    strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False) Then
                        tmpstr = IIf(IsDBNull(drow.Item("ForecastAmount")) = True, "&nbsp;", currency.FormatCurrency(drow.Item("ForecastAmount"), baseCurrency, False))
                    Else
                        tmpstr = "n/a"
                    End If
                    strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)

                    ' get product breakdown information
                    tmpstr = GetProductBreakdown(curUser, drow.Item("contractForecastId"))
                    If tmpstr = "" Then
                        strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                    Else
                        strHTML.Append("<td class=""" & rowClass & """><textarea cols=""30"" rows=""2"" readonly>" & tmpstr & "</textarea></td>" & vbNewLine)
                    End If

                    tmpstr = IIf(IsDBNull(drow.Item("Comment")) = True, "", drow.Item("Comment"))
                    If tmpstr = "" Then
                        strHTML.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                    Else
                        strHTML.Append("<td class=""" & rowClass & """><textarea cols=""40"" rows=""2"" readonly>" & tmpstr & "</textarea></td>" & vbNewLine)
                    End If

                    strHTML.Append("</tr>" & vbNewLine)
                    firstrow = False
                Next
            End If
            strHTML.Append("</table>" & vbNewLine)

            Return strHTML.ToString
        End Function

        Private Sub ShowForecastDetail(ByVal curUser As CurrentUser, ByVal f_id As Integer)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            db.FWDb("R", "contract_forecastdetails", "contractForecastId", f_id, "", "", "", "", "", "", "", "", "", "")
            If db.FWDbFlag = True Then
                Master.enablenavigation = False
                Master.useCloseNavigationMsg = False
                Master.RefreshBreadcrumbInfo()

                ForecastEditFieldsPanel.Visible = True
                If db.FWDbFindVal("Paymentdate", 1) = "" Then
                    txtForecastDate.Text = ""
                Else
                    txtForecastDate.Text = CDate(db.FWDbFindVal("PaymentDate", 1)).ToShortDateString
                End If

                txtForecastAmount.Text = db.FWDbFindVal("ForecastAmount", 1)

                If db.FWDbFindVal("CoverPeriodEnd", 1) = "" Then
                    txtIFCoverEnd.Text = ""
                Else
                    txtIFCoverEnd.Text = CDate(db.FWDbFindVal("CoverPeriodEnd", 1)).ToShortDateString
                End If

                txtIFPONumber.Text = db.FWDbFindVal("poNumber", 1)
                txtIFPOMaxValue.Text = db.FWDbFindVal("poMaxValue", 1)

                If db.FWDbFindVal("poStartDate", 1) = "" Then
                    dateIFPOStart.Text = ""
                Else
                    dateIFPOStart.Text = CDate(db.FWDbFindVal("poStartDate", 1)).ToShortDateString
                End If

                If db.FWDbFindVal("poEndDate", 1) = "" Then
                    dateIFPOExpiry.Text = ""
                Else
                    dateIFPOExpiry.Text = CDate(db.FWDbFindVal("poEndDate", 1)).ToShortDateString
                End If

                txtIFComment.Text = db.FWDbFindVal("Comment", 1)
                lnkBulkUpdate.Visible = False
                lnkNew.Visible = False
                lnkAddIFTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)
                cmdIFClose.Visible = False
            End If

            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            If params.PONumberGenerate Then
                txtIFPONumber.Enabled = False
            End If
        End Sub

        <WebMethod()> _
        Public Shared Function DeleteForecast(ByVal f_id As Integer, ByVal contractId As Integer) As String
            Try
                Dim ARec As New cAuditRecord
                Dim appinfo As HttpApplication = CType(HttpContext.Current.ApplicationInstance, HttpApplication)
                Dim curUser As CurrentUser = cMisc.GetCurrentUser()
                Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim db As New cFWDBConnection
                Dim ALog As New cAuditLog(curUser.AccountID, curUser.EmployeeID)

                db.DBOpen(fws, False)
                db.FWDb("R", "contract_forecastdetails", "contractForecastId", f_id, "", "", "", "", "", "", "", "", "", "")
                If db.FWDbFlag = True Then
                    db.FWDb("R2", "contract_details", "ContractId", db.FWDbFindVal("ContractId", 2), "", "", "", "", "", "", "", "", "", "")
                    ARec.Action = cFWAuditLog.AUDIT_DEL
                    ARec.ContractNumber = IIf(db.FWDb2Flag = True, db.FWDbFindVal("ContractKey", 2), "Unknown")
                    ARec.DataElementDesc = "INV. FORECAST"
                    ARec.ElementDesc = Format(CDate(db.FWDbFindVal("PaymentDate", 1)), cDef.DATE_FORMAT) & ":" & Trim(db.FWDbFindVal("ForecastAmount", 1))
                    ALog.deleteRecord(SpendManagementElement.InvoiceForecasts, f_id, ARec.DataElementDesc + " : " + ARec.ElementDesc)

                    db.FWDb("D", "contract_forecastdetails", "contractForecastId", f_id, "", "", "", "", "", "", "", "", "", "")

                    ContractRoutines.AddContractHistory(curUser.Account.accountid, cAccounts.getConnectionString(curUser.Account.accountid), curUser.Employee, SummaryTabs.InvoiceForecast, contractId, ARec)
                End If
                db.DBClose()
                db = Nothing
                Return "OK"

            Catch ex As Exception
                Return "ERROR! " & ex.Message
            End Try
        End Function

        Private Sub GenerateForecasts()
            ' must check to establish that required pre-requisite information exists
            Dim tmpVal As Integer
            Dim tmpFloat As Double
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            db.FWDb("R", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDbFlag = True Then
                If db.FWDbFindVal("invoiceFrequencyTypeId", 1).Trim = "" Then
                    tmpVal = 0
                Else
                    tmpVal = Integer.Parse(db.FWDbFindVal("invoiceFrequencyTypeId", 1).Trim)
                End If

                If tmpVal <= 0 Then
                    ' no invoice frequency specified
                    lblStatusMsg.Text = "ERROR! Cannot generate forecasts : [No Invoice Freq. for contract]"
                    db.DBClose()
                    Session("IFAction") = Nothing
                    Exit Sub
                End If

                If db.FWDbFindVal("ContractValue", 1).Trim = "" Then
                    tmpFloat = 0
                Else
                    tmpFloat = Double.Parse(db.FWDbFindVal("ContractValue", 1).Trim)
                End If

                If tmpFloat <= 0 Then
                    ' no contract value to calculate from
                    lblStatusMsg.Text = "ERROR! Cannot generate forecasts : [No Contract Value specified]"
                    db.DBClose()
                    db = Nothing
                    Exit Sub
                End If
            End If

            db.DBClose()
            db = Nothing
            Response.Redirect("InvoiceForecastGenerate.aspx", True)
        End Sub

        Private Sub CallBulkUpdates()
            Dim BUitemlist As String = Request.Form("IFBU")
            If Not BUitemlist Is Nothing Then
                If BUitemlist.Length > 0 Then
                    Dim BU_Items() As String = BUitemlist.Split(",")

                    ' call only if items have been ticked for bulk update
                    Dim BU_URL As New System.Text.StringBuilder

                    If BU_Items.Length > 0 Then
                        Dim x As Integer

                        'ReDim BU_Items(BUCount)

                        BU_URL.Append("buitems=" & BU_Items.Length)

                        For x = 0 To BU_Items.Length - 1
                            BU_URL.Append("&fid" & CStr(x + 1).Trim & "=" & BU_Items(x).Trim)
                        Next
                    Else
                        BU_URL.Append("buitems=0")
                    End If

                    Session("IFAction") = "bulkedit"
                    Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&ifaction=bulkedit&" & BU_URL.ToString, True)
                End If
            Else
                lblErrorString.Text = "ERROR! Cannot bulk edit as no selections were made."
            End If
        End Sub

        Private Function GetProductBreakdown(ByVal curUser As CurrentUser, ByVal fId As Integer) As String
            Dim sql As New System.Text.StringBuilder
            Dim tmpStr As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim firstProduct As Boolean
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            sql.Append("SELECT [productDetails].[ProductName], [ProductAmount] FROM [contract_forecastproducts] ")
            sql.Append("LEFT OUTER JOIN [productDetails] ON [contract_forecastproducts].[ProductId] = [productDetails].[ProductId] ")
            sql.Append(" WHERE [ForecastId] = @fId")
            db.AddDBParam("fId", fId, True)
            db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)
            firstProduct = True

            For Each drow In db.glDBWorkD.Tables(0).Rows
                If IsDBNull(drow.Item("ProductName")) = False Then
                    If firstProduct = False Then
                        tmpStr.Append(vbNewLine)
                    End If

                    tmpStr.Append(drow.Item("ProductName") & " : " & CStr(drow.Item("ProductAmount")).Trim)
                    firstProduct = False
                End If
            Next

            Return tmpStr.ToString
        End Function

        Private Sub UpdateBulkRecs(ByVal curUser As CurrentUser)
            Try
                Dim sql As System.Text.StringBuilder
                Dim firstchange As Boolean = True
                Dim ARec As New cAuditRecord
                Dim ARecContrID As String = String.Empty
                Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
                Dim db As New cFWDBConnection
                db.DBOpen(fws, False)

                Dim ALog As New cAuditLog(curUser.AccountID, curUser.EmployeeID)
                ' compile a list of the forecast ids to be updated
                Dim BUitems As Integer
                Dim BUlist As New System.Text.StringBuilder
                Dim comma As String = ""

                BUitems = Integer.Parse(Request.QueryString("buitems"))
                Dim x As Integer
                For x = 1 To BUitems
                    BUlist.Append(comma)
                    BUlist.Append(Request.QueryString("fid" & x.ToString.Trim))
                    comma = ","
                Next

                ' Check if breakdown exists for first item in the list
                db.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                If db.FWDb2Flag = True Then
                    If db.FWDbFindVal("ContractKey", 2) <> "" Then
                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                        ARecContrID = db.FWDbFindVal("ContractKey", 2)
                    ElseIf db.FWDbFindVal("ContractNumber", 2) <> "" Then
                        ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 2)
                        ARecContrID = db.FWDbFindVal("ContractNumber", 2)
                    End If
                End If

                ARec.Action = cFWAuditLog.AUDIT_UPDATE
                comma = ""
                sql = New System.Text.StringBuilder
                sql.Append("UPDATE [contract_forecastdetails] SET ")

                If txtIFCoverEnd.Text <> "" Then
                    'db.SetFieldValue("PO Start Date", datePOStart.Text, "D", firstchange)
                    sql.Append(comma)
                    sql.Append("[CoverPeriodEnd] = CONVERT(datetime,@Cover_Period_End,120)")
                    db.AddDBParam("CoverPeriodEnd", Format(CDate(txtIFCoverEnd.Text), "yyyy-MM-dd"), firstchange)
                    comma = ","
                    firstchange = False

                    ARec.DataElementDesc = "FORECAST"
                    ARec.ElementDesc = "COVER PERIOD END"
                    ARec.PreVal = db.FWDbFindVal("CoverPeriodEnd", 2)
                    ARec.PostVal = txtIFCoverEnd.Text
                    'ALog.editRecord("Contracts", ARec.DataElementDesc, ARec.ElementDesc, ARec.PreVal, ARec.PostVal)
                End If

                If txtIFPONumber.Text.Trim <> "" Then
                    ' poNumber selected for BU
                    'db.SetFieldValue("poNumber", txtPONumber.Text, "S", firstchange)
                    sql.Append(comma)
                    sql.Append("[poNumber] = @PO_Number")
                    db.AddDBParam("poNumber", txtIFPONumber.Text, firstchange)
                    comma = ","
                    firstchange = False

                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "FORECAST"
                    ARec.ElementDesc = "poNumber"
                    ARec.PreVal = db.FWDbFindVal("poNumber", 2)
                    ARec.PostVal = txtIFPONumber.Text
                    'ALog.editRecord("Contracts", ARec.DataElementDesc, ARec.ElementDesc, ARec.PreVal, ARec.PostVal)
                End If

                If dateIFPOStart.Text <> "" Then
                    'db.SetFieldValue("PO Start Date", datePOStart.Text, "D", firstchange)
                    sql.Append(comma)
                    sql.Append("[poStartDate] = CONVERT(datetime,@PO_Start_Date,120)")
                    db.AddDBParam("PO Start Date", Format(CDate(dateIFPOStart.Text), "yyyy-MM-dd"), firstchange)
                    comma = ","
                    firstchange = False

                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "FORECAST"
                    ARec.ElementDesc = "PO START DATE"
                    ARec.PreVal = db.FWDbFindVal("poStartDate", 2)
                    ARec.PostVal = dateIFPOStart.Text
                    'ALog.editRecord("Contracts", ARec.DataElementDesc, ARec.ElementDesc, ARec.PreVal, ARec.PostVal)
                End If

                If dateIFPOExpiry.Text <> "" Then
                    'db.SetFieldValue("PO Expiry Date", datePOExpiry.Text, "D", firstchange)
                    sql.Append(comma)
                    sql.Append("[poExpiryDate] = CONVERT(datetime,@PO_Expiry_Date,120)")
                    db.AddDBParam("PO Expiry Date", Format(CDate(dateIFPOExpiry.Text), "yyyy-MM-dd"), firstchange)
                    comma = ","
                    firstchange = False

                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "FORECAST"
                    ARec.ElementDesc = "PO EXPIRY DATE"
                    ARec.PreVal = db.FWDbFindVal("poExpiryDate", 2)
                    ARec.PostVal = dateIFPOExpiry.Text
                    'ALog.editRecord("Contracts", ARec.DataElementDesc, ARec.ElementDesc, ARec.PreVal, ARec.PostVal)
                End If

                If txtIFPOMaxValue.Text <> "" Then
                    'db.SetFieldValue("PO Max Value", txtPOMaxValue.Text, "N", firstchange)
                    sql.Append(comma)
                    sql.Append("[poMaxValue] = @PO_Max_Value")
                    db.AddDBParam("PO Max Value", txtIFPOMaxValue.Text, firstchange)
                    comma = ","
                    firstchange = False

                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "FORECAST"
                    ARec.ElementDesc = "PO MAX VALUE"
                    ARec.PreVal = db.FWDbFindVal("poMaxValue", 2)
                    ARec.PostVal = txtIFPOMaxValue.Text
                    'ALog.editRecord("Contracts", ARec.DataElementDesc, ARec.ElementDesc, ARec.PreVal, ARec.PostVal)
                End If

                If txtForecastAmount.Text <> "" Then
                    'db.SetFieldValue("Forecast Amount", txtForecastAmount.Text, "N", firstchange)
                    sql.Append(comma)
                    sql.Append("[ForecastAmount] = @Forecast_Amount")
                    db.AddDBParam("ForecastAmount", txtForecastAmount.Text, firstchange)
                    comma = ","
                    firstchange = False

                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "FORECAST"
                    ARec.ElementDesc = "AMOUNT"
                    ARec.PreVal = db.FWDbFindVal("ForecastAmount", 2)
                    ARec.PostVal = txtForecastAmount.Text
                    'ALog.editRecord("Contracts", ARec.DataElementDesc, ARec.ElementDesc, ARec.PreVal, ARec.PostVal)
                End If

                If txtIFComment.Text <> "" Then
                    'db.SetFieldValue("Comment", txtComment.Text, "S", firstchange)
                    sql.Append(comma)
                    sql.Append("[Comment] = @Comment")
                    db.AddDBParam("Comment", txtIFComment.Text, firstchange)
                    comma = ","
                    firstchange = False

                    ARec = New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ContractNumber = ARecContrID
                    ARec.DataElementDesc = "FORECAST"
                    ARec.ElementDesc = "COMMENT"
                    Dim tmpstr As String
                    tmpstr = db.FWDbFindVal("Comment", 2)
                    If tmpstr.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                        ARec.PreVal = tmpstr.Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                    Else
                        ARec.PreVal = tmpstr
                    End If
                    If txtIFComment.Text.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                        ARec.PostVal = txtIFComment.Text.Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                    Else
                        ARec.PostVal = txtIFComment.Text
                    End If
                    'ALog.editRecord("Contracts", ARec.DataElementDesc, ARec.ElementDesc, ARec.PreVal, ARec.PostVal)
                End If

                Dim arrBUItems() As String

                arrBUItems.CreateInstance(GetType(String), BUitems)
                arrBUItems = Split(BUlist.ToString, ",")

                If firstchange = False Then
                    sql.Append(" WHERE [contractForecastId] IN (")
                    sql.Append(BUlist.ToString)
                    sql.Append(")")
                    db.ExecuteSQL(sql.ToString)
                End If

                ' update any breakdown changes

                ' "safe" copy of the breakdown is in a protected version of the collection
                Dim safe_collFBU As ArrayList = ArrayList.Synchronized(Cache("collFBU_protected"))
                Dim collFBU As ArrayList = ArrayList.Synchronized(Cache("collFBU"))

                'safe_collFBU = Cache("collFBU_protected")
                'collFBU = Cache("collFBU")

                Dim forecastItem As ForecastBreakdownItem
                Dim safe_forecastItem As ForecastBreakdownItem
                Dim subtotal As Double = 0
                Dim collEnum As System.Collections.IEnumerator = collFBU.GetEnumerator
                Dim removalItems As New System.Text.StringBuilder
                Dim removalComma As String = ""

                'For idx = 0 To collFBU.Count - 1
                While collEnum.MoveNext
                    '  forecastItem = collFBU(x)
                    forecastItem = collEnum.Current

                    If forecastItem.ForecastProductId = 0 Then
                        ' adding an extra breakdown element
                        subtotal += forecastItem.ProductAmount
                        For x = 0 To BUitems - 1
                            ' add the new item for all tagged breakdowns
                            db.SetFieldValue("ForecastId", arrBUItems(x), "N", True)
                            db.SetFieldValue("ProductId", forecastItem.ProductId, "N", False)
                            db.SetFieldValue("ProductAmount", forecastItem.ProductAmount, "N", False)
                            db.FWDb("W", "contract_forecastproducts", "", "", "", "", "", "", "", "", "", "", "", "")

                            If db.glIdentity > 0 Then
                                forecastItem.ForecastProductId = db.glIdentity
                            End If

                            'collFBU() = forecastItem
                        Next
                    ElseIf forecastItem.ProductId = -1 Then
                        ' breakdown item must have been deleted, so add to removal id list
                        removalItems.Append(removalComma)
                        removalItems.Append(forecastItem.ForecastProductId.ToString)
                        removalComma = ","
                    Else
                        ' check for update from previous
                        firstchange = True

                        Dim safeEnum As System.Collections.IEnumerator = safe_collFBU.GetEnumerator
                        Dim strSQL As New System.Text.StringBuilder

                        comma = ""
                        strSQL.Append("UPDATE [contract_forecastproducts] SET ")

                        While safeEnum.MoveNext
                            safe_forecastItem = safeEnum.Current

                            If forecastItem.ForecastProductId = safe_forecastItem.ForecastProductId Then
                                ' found current instance, so check if changed
                                If forecastItem.ProductId <> safe_forecastItem.ProductId Then
                                    strSQL.Append(comma)
                                    strSQL.Append("[ProductId] = @Product_Id")
                                    db.AddDBParam("Product Id", forecastItem.ProductId.ToString, firstchange)

                                    comma = ","
                                    'db.SetFieldValue("Product Id", forecastItem.ProductId, "N", firstchange)
                                    firstchange = False
                                End If

                                If forecastItem.ProductAmount <> safe_forecastItem.ProductAmount Then
                                    strSQL.Append(comma)
                                    strSQL.Append("[ProductAmount] = @Product_Amount")
                                    db.AddDBParam("Product Amount", forecastItem.ProductAmount.ToString, firstchange)
                                    comma = ","
                                    'db.SetFieldValue("Product Amount", forecastItem.ProductAmount, "N", firstchange)
                                    firstchange = False
                                End If

                                If firstchange = False Then
                                    'Dim tmpLoop As Integer
                                    'For tmpLoop = 0 To BUitems - 1
                                    '    db.FWDb("A", "Contract - Forecast Products", "Forecast Id", arrBUItems(tmpLoop))
                                    'Next
                                    strSQL.Append(" WHERE [ForecastId] IN (")
                                    strSQL.Append(BUlist.ToString)
                                    strSQL.Append(") AND [ProductId] = ")
                                    strSQL.Append(forecastItem.ProductId.ToString)
                                    db.ExecuteSQL(strSQL.ToString)
                                End If
                                ' no need to go any further, match has been made
                                subtotal += forecastItem.ProductAmount
                                Exit While
                            End If
                        End While
                    End If
                    'Next
                End While

                If removalItems.ToString.Length > 0 Then
                    Dim completeRemovalList As New System.Text.StringBuilder
                    removalComma = ""

                    sql = New System.Text.StringBuilder
                    ' get all details for the items tagged for deletion
                    sql.Append("SELECT * FROM [contract_forecastproducts] WHERE [ForecastProductId] IN (")

                    sql.Append(removalItems.ToString & ")")
                    db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

                    Dim FullList_drow As DataRow
                    Dim Tagged_drow As DataRow

                    sql = New System.Text.StringBuilder
                    sql.Append("SELECT * FROM [contract_forecastproducts] WHERE [ForecastId] IN (" & BUlist.ToString & ")")
                    db.RunSQL(sql.ToString, db.glDBWorkB, False, "", False)

                    For Each FullList_drow In db.glDBWorkB.Tables(0).Rows
                        For Each Tagged_drow In db.glDBWorkA.Tables(0).Rows
                            If FullList_drow.Item("ProductId") = Tagged_drow("ProductId") Then
                                If FullList_drow.Item("ProductAmount") = Tagged_drow.Item("ProductAmount") Then
                                    ' must be identical line item, so permit deletion. This prevent duplicate product lines from also being deleted.
                                    'It is possible for Product A to appear > once with different amount
                                    completeRemovalList.Append(removalComma)
                                    completeRemovalList.Append(FullList_drow.Item("ForecastProductId"))
                                    removalComma = ","
                                End If
                            End If
                        Next
                    Next

                    If completeRemovalList.ToString.Length > 0 Then
                        sql.Append("DELETE FROM [contract_forecastproducts] WHERE [ForecastProductId] IN (")
                        sql.Append(completeRemovalList.ToString)
                        sql.Append(")")
                        db.ExecuteSQL(sql.ToString)
                    End If
                End If

                ' update breakdown subtotal to all tagged forecasts
                sql = New System.Text.StringBuilder
                sql.Append("UPDATE [contract_forecastdetails] SET [ForecastAmount] = @ForecastAmount WHERE [contractForecastId] IN (" & BUlist.ToString & ") ")
                db.AddDBParam("ForecastAmount", subtotal.ToString, True)
                db.ExecuteSQL(sql.ToString)

                Cache("collFBU_protected") = Nothing
                Cache("collFBU") = Nothing

                db.DBClose()
            Catch ex As Exception

            End Try

            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
        End Sub

        Private Function GetBreakdown(ByVal curUser As CurrentUser, ByVal forecastId As Integer, Optional ByVal buildFromCollection As Boolean = False) As String
            Dim strhtml As System.Text.StringBuilder
            Dim ForecastItem As ForecastBreakdownItem
            Dim rowClass As String = "row1"
            Dim rowalt As Boolean = False
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fwparams As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            db.DBOpen(fws, False)

            Try
                Dim sql As New System.Text.StringBuilder

                strhtml = New System.Text.StringBuilder

                strhtml.Append("<table class=""datatbl"">" & vbNewLine)
                strhtml.Append("<tr>" & vbNewLine)
                strhtml.Append("<th>Product Name</th>" & vbNewLine)
                strhtml.Append("<th>Forecast Amount</th>" & vbNewLine)
                strhtml.Append("</tr>" & vbNewLine)

                If buildFromCollection = False Then
                    Dim collFBU As New ArrayList

                    sql.Append("SELECT [ForecastProductId],[contract_forecastproducts].[ProductId],[productDetails].[ProductName], [ProductAmount] FROM [contract_forecastproducts] ")
                    sql.Append("INNER JOIN [productDetails] ON [contract_forecastproducts].[ProductId] = [productDetails].[ProductId] ")
                    sql.Append("WHERE [ForecastId] = @Forecast_Id")
                    db.AddDBParam("Forecast Id", forecastId, True)

                    db.RunSQL(sql.ToString, db.glDBWorkI, False, "", False)

                    Dim drow As DataRow

                    If db.glNumRowsReturned > 0 Then
                        For Each drow In db.glDBWorkI.Tables(0).Rows
                            rowalt = (rowalt Xor True)
                            If rowalt Then
                                rowClass = "row1"
                            Else
                                rowClass = "row2"
                            End If

                            With ForecastItem
                                .ForecastProductId = drow.Item("ForecastProductId")
                                .ProductId = drow.Item("ProductId")
                                .ProductName = drow.Item("ProductName")
                                If IsDBNull(drow.Item("ProductAmount")) = True Then
                                    .ProductAmount = 0
                                Else
                                    .ProductAmount = drow.Item("ProductAmount")
                                End If

                                collFBU.Add(ForecastItem)

                                strhtml.Append("<tr>" & vbNewLine)
                                strhtml.Append("<td class=""" & rowClass & """>" & .ProductName & "</td>" & vbNewLine)
                                strhtml.Append("<td class=""" & rowClass & """ align=""right"">" & Format(.ProductAmount, "#,###,##0.00") & "</td>" & vbNewLine)
                                strhtml.Append("</tr>" & vbNewLine)
                            End With
                        Next

                        If Cache("collFBU_protected") Is Nothing Then
                            Dim tmpColl As New System.Collections.ArrayList
                            Dim fcast As ForecastBreakdownItem
                            Dim idx As Integer
                            For idx = 0 To collFBU.Count - 1
                                fcast = collFBU(idx)
                                tmpColl.Add(fcast)
                            Next
                            cLocks.UpdateCacheExpireItem(Cache, "collFBU_protected", fwparams.CacheTimeout, tmpColl)
                        End If

                        If collFBU.IsSynchronized = False Then
                            Dim tmpAL As ArrayList = ArrayList.Synchronized(collFBU)
                            cLocks.UpdateCacheExpireItem(Cache, "collFBU", fwparams.CacheTimeout, tmpAL)
                        Else
                            cLocks.UpdateCacheExpireItem(Cache, "collFBU", fwparams.CacheTimeout, collFBU)
                        End If

                        ' put in window hyperlink to open for edit
                        rowalt = (rowalt Xor True)
                        If rowalt Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        strhtml.Append("<tr>" & vbNewLine)
                        strhtml.Append("<td class=""" & rowClass & """ align=""center"" colspan=""2""><a onclick=""OpenBreakdown('" & forecastId.ToString & "');"">Amend Breakdown</a></td>")
                        strhtml.Append("</tr>" & vbNewLine)
                    Else
                        strhtml.Append("<tr>" & vbNewLine)
                        strhtml.Append("<td class=""row1"" align=""center"" colspan=""2"">No breakdown detail</td>" & vbNewLine)
                        strhtml.Append("</tr>" & vbNewLine)
                    End If
                Else
                    Dim tmpFBU As ArrayList = ArrayList.Synchronized(Cache("collFBU"))
                    'tmpFBU = Cache("collFBU")

                    Dim x As Integer
                    For x = 0 To tmpFBU.Count - 1
                        rowalt = (rowalt Xor True)
                        If rowalt Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        ForecastItem = tmpFBU(x)

                        If ForecastItem.ProductId <> -1 Then
                            ' if -1, then it has been tagged for deletion upon update
                            With ForecastItem
                                strhtml.Append("<tr>" & vbNewLine)
                                strhtml.Append("<td class=""" & rowClass & """>" & .ProductName & "</td>" & vbNewLine)
                                strhtml.Append("<td class=""" & rowClass & """ align=""right"">" & Format(.ProductAmount, "#,###,##0.00") & "</td>" & vbNewLine)
                                strhtml.Append("</tr>" & vbNewLine)
                            End With
                        End If
                    Next

                    ' put in window hyperlink to open for edit
                    strhtml.Append("<tr>" & vbNewLine)
                    strhtml.Append("<td class=""" & rowClass & """ align=""center"" colspan=""2""><a onclick=""OpenBreakdown('" & forecastId.ToString & "');"">Amend Breakdown</a></td>")
                    strhtml.Append("</tr>" & vbNewLine)
                End If

                strhtml.Append("</table>" & vbNewLine)

            Catch ex As Exception
                strhtml = New System.Text.StringBuilder
                strhtml.Append("<table class=""datatbl"">" & vbNewLine)
                strhtml.Append("<tr>" & vbNewLine)
                strhtml.Append("<th>Product Name</th>" & vbNewLine)
                strhtml.Append("<th>Forecast Amount</th>" & vbNewLine)
                strhtml.Append("</tr>" & vbNewLine)
                strhtml.Append("<tr>" & vbNewLine)
                strhtml.Append("<td class=""row1"" align=""center"" colspan=""2""><b>ERROR:</b>" & ex.Message & "</td>" & vbNewLine)
                strhtml.Append("</tr>" & vbNewLine)
                strhtml.Append("</table>" & vbNewLine)
            End Try

            db.DBClose()
            Return strhtml.ToString
        End Function

        Protected Sub cmdIFUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            cmdIFUpdate.Enabled = False

            doIFUpdate(curUser)

            ForecastEditFieldsPanel.Visible = False
            cmdIFClose.Visible = True
            lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.InvoiceForecasts, False)

            GetForecastTable(curUser)
            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
        End Sub

        Private Sub doIFUpdate(ByVal curUser As CurrentUser)
            Select Case Session("IFAction")
                Case "add"
                    AddIFRec(curUser)

                Case "update"
                    If Not Session("ForecastId") Is Nothing And Session("ForecastId") <> 0 Then
                        UpdateIFRecord(curUser, Session("ForecastId"))
                    End If

                Case "bulkedit"
                    UpdateBulkRecs(curUser)
                Case Else

            End Select

            Session("IFAction") = Nothing
        End Sub

        Protected Sub cmdIFCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            ForecastEditFieldsPanel.Visible = False
            Session("IFAction") = Nothing
            litForecastData.Text = Cache("IFData")
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.AccountID))
            ' only display the bulk update if permission held and records exist
            Dim sql As String = "select count(contractForecastId) from contract_forecastdetails where contractId = @conId"
            db.sqlexecute.Parameters.AddWithValue("@conId", Session("ActiveContract"))
            Dim count As Integer = db.getcount(sql)
            lnkBulkUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.InvoiceForecasts, False) And count > 0
            lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.InvoiceForecasts, False)
            lnkAddIFTask.Visible = False
            cmdIFClose.Visible = True
            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub cmdBUCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Session("IFAction") = Nothing
            Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast, True)
        End Sub

        Protected Sub cmdBUUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            doIFUpdate(curUser)
        End Sub

        Protected Sub cmdIFClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Response.Redirect("Home.aspx", True)
        End Sub

        Protected Sub lnkAddIFTask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddIFTask.Click
            Dim varURL As String
            varURL = "tid=0&rid=" & Session("ForecastId") & "&rtid=" & AppAreas.INVOICE_FORECASTS
            Session("TaskRetURL") = Server.UrlEncode("~/ContractSummary.aspx?ifaction=edit&tab=" & SummaryTabs.InvoiceForecast & "&ifid=" & Session("ForecastId"))
            Response.Redirect("~/shared/tasks/ViewTask.aspx?" & varURL, True)
        End Sub
#End Region

#Region "Note Summary Code"
        Private Sub NoteSummary_Load()
            Dim strNoteSection As New System.Text.StringBuilder
            Dim action As String
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection

            db.DBOpen(fws, False)
            'Master.enablenavigation = False
            Master.useCloseNavigationMsg = True
            Master.RefreshBreadcrumbInfo()

            action = Request.QueryString("nsaction")

            db.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag = True Then
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractNotes, False) Then
                    strNoteSection.Append(CreateNotesTable(db, "contract", Session("ActiveContract")))
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierNotes, False) Then
                    strNoteSection.Append(CreateNotesTable(db, "suppliers", Val(db.FWDbFindVal("supplierId", 2))))
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierContactNotes, False) Then
                    strNoteSection.Append(CreateNotesTable(db, "suppliercontact", Val(db.FWDbFindVal("supplierId", 2))))
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductNotes, False) Then
                    strNoteSection.Append(CreateNotesTable(db, "product", Session("ActiveContract")))
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Invoices, False) Then
                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceNotes, False) Then
                        strNoteSection.Append(CreateNotesTable(db, "invoice", Session("ActiveContract")))
                    End If
                End If
                lblNSTitle.Text = " - " & db.FWDbFindVal("ContractDescription", 2)
            Else
                lblNSTitle.Text = ""
            End If

            litNoteSummaryData.Text = strNoteSection.ToString

            cmdNSClose.Attributes.Add("onclick", "javascript:GoHome();")
            cmdNSClose.Attributes.Add("onmouseover", "window.status='Exit contract and return to home page';return true;")
            cmdNSClose.Attributes.Add("onmouseout", "window.status='Done';")
            cmdNSClose.ToolTip = "Exit contract and return to home page"

            db.DBClose()
        End Sub

        Private Function CreateNotesTable(ByVal db As cFWDBConnection, ByVal notetype As String, ByVal RecId As Integer) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim sql As New System.Text.StringBuilder
            Dim tmpStr As String
            Dim IDField As String = ""
            Dim NoteTable As String = ""
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim drow As DataRow

            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            Select Case LCase(notetype)
                Case "suppliers"
                    strHTML.Append("<div class=""inputpanel formpanel_padding"">" & vbNewLine)
                    strHTML.Append("<div class=""inputpaneltitle"">" & params.SupplierPrimaryTitle & " Notes</div>" & vbNewLine)
                    IDField = "[supplierId]"
                    NoteTable = "[supplierNotes]"

                Case "contract"
                    strHTML.Append("<div class=""inputpanel formpanel_padding"">" & vbNewLine)
                    strHTML.Append("<div class=""inputpaneltitle"">Contract Notes</div>" & vbNewLine)
                    IDField = "[contractID]"
                    NoteTable = "[contractNotes]"

                Case "invoice"
                    strHTML.Append("<div class=""inputpanel formpanel_padding"">" & vbNewLine)
                    strHTML.Append("<div class=""inputpaneltitle"">Invoice Notes</div>" & vbNewLine)
                    IDField = "[invoiceId]"
                    NoteTable = "[invoiceNotes]"

                Case "product"
                    strHTML.Append("<div class=""inputpanel formpanel_padding"">" & vbNewLine)
                    strHTML.Append("<div class=""inputpaneltitle"">Product Notes</div>" & vbNewLine)
                    IDField = "[productId]"
                    NoteTable = "[productNotes]"

                Case "suppliercontact"
                    strHTML.Append("<div class=""inputpanel formpanel_padding"">" & vbNewLine)
                    strHTML.Append("<div class=""inputpaneltitle"">" & params.SupplierPrimaryTitle & " Contacts Notes</div>" & vbNewLine)
                    IDField = "[contactId]"
                    NoteTable = "[supplierContactNotes]"

                Case Else

            End Select

            Select Case LCase(notetype)
                Case "contract"
                    Dim tmpNote As String
                    tmpNote = GetNoteData(db, notetype, NoteTable, IDField, RecId)
                    If tmpNote.Trim <> "" Then
                        strHTML.Append(tmpNote)
                    Else
                        strHTML.Append("<div>No Contract Notes to display</div>" & vbNewLine)
                    End If

                Case "suppliers"
                    Dim tmpNote As String
                    tmpNote = GetNoteData(db, notetype, NoteTable, IDField, RecId)
                    If tmpNote.Trim <> "" Then
                        strHTML.Append(tmpNote)
                    Else
                        strHTML.Append("<div>No " & params.SupplierPrimaryTitle & " Notes to display</div>" & vbNewLine)
                    End If

                Case "product"
                    ' can be multiple products on a contract
                    ' RecId passed in is contract id, not product id!!
                    sql.Append("SELECT [contract_productdetails].[ProductId],[productDetails].[ProductName] FROM [contract_productdetails] ")
                    sql.Append("INNER JOIN [productDetails] ON [contract_productdetails].[ProductId] = [productDetails].[ProductId] ")
                    sql.Append("WHERE [ContractId] = @recId ORDER BY [ProductName]")
                    db.AddDBParam("recId", RecId, True)
                    db.RunSQL(sql.ToString, db.glDBWorkC, False, "", False)

                    Dim noteCount As Integer = 0

                    If db.GetRowCount(db.glDBWorkC, 0) > 0 Then

                        For Each drow In db.glDBWorkC.Tables(0).Rows
                            Dim tmpNote As String
                            tmpNote = GetNoteData(db, notetype, NoteTable, IDField, drow.Item("ProductId"), Session("ActiveContract"))
                            If tmpNote.Trim <> "" Then
                                noteCount += 1
                                strHTML.Append("<div><b><u>" & drow.Item("ProductName") & "</u></b></div>" & vbNewLine)
                                strHTML.Append("<div style=""height: 5px;"">&nbsp;</div>" & vbNewLine)
                                strHTML.Append(tmpNote)
                                strHTML.Append("<div style=""height: 5px;"">&nbsp;</div>" & vbNewLine)
                            End If
                        Next
                    End If

                    If noteCount = 0 Then
                        strHTML.Append("<div>No Product Notes to display</div>" & vbNewLine)
                    End If

                Case "invoice"
                    ' can have multiple invoices to a contract
                    ' RecId is passed in as contract id, not invoice id
                    sql.Append("SELECT [InvoiceId],[InvoiceNumber],[invoiceStatusType].[isArchive] FROM [invoices] ")
                    sql.Append("INNER JOIN [invoiceStatusType] ON [invoiceStatusType].[InvoiceStatusTypeId] = [invoices].[InvoiceStatus] ")
                    sql.Append("WHERE [ContractId] = @recId AND [isArchive] = 0")
                    db.AddDBParam("recId", RecId, True)
                    db.RunSQL(sql.ToString, db.glDBWorkC, False, "", False)

                    Dim noteCount As Integer = 0

                    If db.GetRowCount(db.glDBWorkC, 0) > 0 Then
                        For Each drow In db.glDBWorkC.Tables(0).Rows
                            Dim tmpNote As String
                            tmpNote = GetNoteData(db, notetype, NoteTable, IDField, drow.Item("InvoiceId"), Session("ActiveContract"))
                            If tmpNote.Trim <> "" Then
                                noteCount += 1
                                strHTML.Append("<div><b><u>" & vbNewLine)
                                strHTML.Append(drow.Item("InvoiceNumber") & "</u></b></div>" & vbNewLine)
                                strHTML.Append("<div style=""height: 5px;"">&nbsp;</div>" & vbNewLine)
                                strHTML.Append(tmpNote)
                                strHTML.Append("<div style=""height: 5px;"">&nbsp;</div>" & vbNewLine)
                            End If
                        Next
                    End If

                    If noteCount = 0 Then
                        strHTML.Append("<div>No Invoice Notes to display</div>" & vbNewLine)
                    End If

                Case "suppliercontact"
                    ' can have multiple contacts to a vendor
                    ' RecId passed in will be Vendor Id not contact id
                    sql.Append("SELECT * FROM [supplier_contacts] WHERE [supplierid] = @recId")
                    db.AddDBParam("recId", RecId, True)
                    db.RunSQL(sql.ToString, db.glDBWorkC, False, "", False)

                    Dim notecount As Integer = 0
                    If db.GetRowCount(db.glDBWorkC, 0) > 0 Then
                        For Each drow In db.glDBWorkC.Tables(0).Rows
                            tmpStr = GetNoteData(db, notetype, NoteTable, IDField, drow.Item("contactId"), RecId)
                            If tmpStr.Trim <> "" Then
                                notecount += 1
                                strHTML.Append("<div><b><u>" & vbNewLine)
                                strHTML.Append(drow.Item("contactName") & "</u></b></div>" & vbNewLine)
                                strHTML.Append("<div style=""height: 5px;"">&nbsp;</div>" & vbNewLine)
                                strHTML.Append(tmpStr)
                                strHTML.Append("<div style=""height: 5px;"">&nbsp;</div>" & vbNewLine)
                            End If
                        Next
                    End If

                    If notecount = 0 Then
                        strHTML.Append("<div>No " & params.SupplierPrimaryTitle & " Contact Notes to display</div>" & vbNewLine)
                    End If
            End Select

            strHTML.Append("</div>" & vbNewLine)

            Return strHTML.ToString
        End Function

        Private Function GetNoteData(ByVal db As cFWDBConnection, ByVal NoteType As String, ByVal NoteTable As String, ByVal IDField As String, ByVal RecId As Integer, Optional ByVal OwnerId As Integer = 0) As String
            Dim strNoteTable As New System.Text.StringBuilder
            Dim sql As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim tmpStr As String
            Dim tmpOwnerId As Integer
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            sql.Append("SELECT " & NoteTable & ".*,[primarynotes].[fullDescription] AS [primarydesc],[secondarynotes].[fullDescription] AS [secondarydesc], employees.firstname + ' ' + employees.surname AS CreatedByName,")
            sql.Append("(SELECT COUNT(*) FROM [attachments] WHERE [ReferenceNumber] = " & NoteTable & ".[noteId] AND [AttachmentArea] = @attArea AND dbo.CheckAttachmentAccess([AttachmentId],@userId) > 0) AS [Num Attachments] ")
            sql.Append("FROM " & NoteTable & " ")
            sql.Append("INNER JOIN [codes_notecategory] AS [primarynotes] ON [primarynotes].[noteCatId] = " & NoteTable & ".[noteType] ")
            sql.Append("INNER JOIN [codes_notecategory] AS [secondarynotes] ON [secondarynotes].[noteCatId] = " & NoteTable & ".[noteCatId] ")
            sql.Append("LEFT JOIN employees ON employees.employeeId = " & NoteTable & ".CreatedBy ")
            sql.Append("WHERE " & IDField & " = @refId ")
            sql.Append("ORDER BY [Date] DESC")
            db.AddDBParam("refId", RecId, True)
            db.AddDBParam("userId", curUser.EmployeeID, False)
            Select Case NoteType
                Case "contract"
                    db.AddDBParam("attArea", AttachmentArea.CONTRACT_NOTES, False)
                Case "suppliers"
                    db.AddDBParam("attArea", AttachmentArea.VENDOR_NOTES, False)
                Case "product"
                    db.AddDBParam("attArea", AttachmentArea.PRODUCT_NOTES, False)
                Case "invoice"
                    db.AddDBParam("attArea", AttachmentArea.INVOICE_NOTES, False)
                Case "supplierrcontact"
                    db.AddDBParam("attArea", AttachmentArea.CONTACT_NOTES, False)
                Case Else
                    db.AddDBParam("attArea", 0, False)
            End Select
            db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)

            If db.glNumRowsReturned = 0 Then
                Return ""
                Exit Function
            End If

            strNoteTable.Append("<table class=""datatbl"">" & vbNewLine)
            strNoteTable.Append("<tr>" & vbNewLine)
            strNoteTable.Append("<th><img src=""./icons/16/plain/zoom_in.png"" /></th>" & vbNewLine)
            strNoteTable.Append("<th>Date Created</th>" & vbNewLine)
            strNoteTable.Append("<th>Created By</th>" & vbNewLine)
            strNoteTable.Append("<th>Primary Note Category</th>" & vbNewLine)
            strNoteTable.Append("<th>Secondary Note Category</th>" & vbNewLine)
            strNoteTable.Append("<th>Note Sample</th>" & vbNewLine)
            strNoteTable.Append("<th>Attachments?</th>" & vbNewLine)
            strNoteTable.Append("</tr>" & vbNewLine)

            Dim rowalt As Boolean = False
            Dim rowClass As String = "row1"

            For Each drow In db.glDBWorkD.Tables(0).Rows
                rowalt = (rowalt Xor True)
                If rowalt Then
                    rowClass = "row1"
                Else
                    rowClass = "row2"
                End If

                strNoteTable.Append("<tr>" & vbNewLine)
                If OwnerId <> 0 Then
                    tmpOwnerId = OwnerId
                Else
                    tmpOwnerId = RecId
                End If

                strNoteTable.Append("<td class=""" & rowClass & """style=""text-align:center""><a href=""ViewSingleNote.aspx?id=" & Trim(drow.Item("NoteId")) & "&notetype=" & Trim(NoteType) & "&ownerid=" & RecId.ToString & """><img src=""./icons/16/plain/zoom_in.png"" /></a></td>" & vbNewLine)

                tmpStr = IIf(IsDBNull(drow.Item("Date")) = True, "&nbsp;", Format(CDate(drow.Item("Date")), cDef.DATE_FORMAT))
                strNoteTable.Append("<td class=""" & rowClass & """>" & tmpStr.Trim & "</td>" & vbNewLine)

                tmpStr = IIf(IsDBNull(drow.Item("CreatedByName")) = True, "&nbsp;", drow.Item("CreatedByName"))
                strNoteTable.Append("<td class=""" & rowClass & """>" & tmpStr.Trim & "</td>" & vbNewLine)

                tmpStr = IIf(IsDBNull(drow.Item("primarydesc")) = True, "&nbsp;", drow.Item("primarydesc"))
                strNoteTable.Append("<td class=""" & rowClass & """>" & tmpStr.Trim & "</td>" & vbNewLine)

                tmpStr = IIf(IsDBNull(drow.Item("secondarydesc")) = True, "&nbsp;", drow.Item("secondarydesc"))
                strNoteTable.Append("<td class=""" & rowClass & """>" & tmpStr.Trim & "</td>" & vbNewLine)

                tmpStr = IIf(IsDBNull(drow.Item("Note")) = True, "&nbsp;", drow.Item("Note"))
                strNoteTable.Append("<td class=""" & rowClass & """><textarea style=""width: 300px;"" rows=""3"" readonly>" & Trim(tmpStr) & "</textarea></td>" & vbNewLine)

                strNoteTable.Append("<td class=""" & rowClass & """ align=""center"">")
                If drow.Item("Num Attachments") > 0 Then
                    strNoteTable.Append("<img src=""./icons/16/plain/paperclip.gif"" />")
                End If
                strNoteTable.Append("</td>" & vbNewLine)
                strNoteTable.Append("</tr>" & vbNewLine)
            Next

            strNoteTable.Append("</table>" & vbNewLine)

            Return strNoteTable.ToString
        End Function

#End Region

#Region "Linked Contracts Code"
        Private Sub LinkedContracts_Load()
            Dim contractId As Integer = 0
            Dim action As String = ""
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection

            db.DBOpen(fws, False)

            action = Request.QueryString("action")

            contractId = Session("ActiveContract")

            Title = "Linked Contracts"
            Master.title = Title
            'Master.enablenavigation = False
            Master.useCloseNavigationMsg = True
            Master.RefreshBreadcrumbInfo()

            If contractId > 0 Then
                litLinkData.Text = GetContractLinks(db, contractId)
            Else
                litLinkData.Text = "<div class=""inputpanel formpanel_padding"" style=""height: 10px;"">Currently no contract links in place</div>"
            End If

            cmdLCCancel.ToolTip = "Exit contract summary and return to home page"
            cmdLCCancel.Attributes.Add("onmouseover", "window.status='Exit contract summary and return to home page';return true;")
            cmdLCCancel.Attributes.Add("onmouseout", "window.status='Done';")
            cmdLCCancel.Attributes.Add("onclick", "javascript:GoHome();")
            db.DBClose()
        End Sub

        Private Function GetContractLinks(ByVal db As cFWDBConnection, ByVal cId As Integer) As String
            Dim rowClass As String
            Dim sql As New System.Text.StringBuilder
            Dim headerHTML As New System.Text.StringBuilder
            Dim tmpData As New System.Text.StringBuilder
            Dim linkDef As String
            Dim tmpStr As New System.Text.StringBuilder
            Dim drow, drow_link As DataRow
            Dim hasRows As Boolean
            Dim rowalt As Boolean = False
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim baseCurrency As cCurrency = currency.getCurrencyById(params.BaseCurrency.Value)

            sql.Append("SELECT [link_matrix].[LinkId],[link_definitions].[linkDefinition] FROM [link_matrix] ")
            sql.Append("LEFT JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] ")
            sql.Append("WHERE [link_matrix].[ContractId] = @cId AND [IsScheduleLink] = 0 ")
            sql.Append("ORDER BY [LinkDefinition]")
            db.AddDBParam("cId", cId, True)
            db.RunSQL(sql.ToString, db.glDBWorkB, False, "", False)

            ' header row
            headerHTML.Append("<table class=""datatbl"">" & vbNewLine)
            headerHTML.Append("<tr>" & vbNewLine)
            headerHTML.Append("<th style=""width:30px"">&nbsp;</th>" & vbNewLine)
            headerHTML.Append("<th>Contract Key</th>" & vbNewLine)
            headerHTML.Append("<th>Contract Description</th>" & vbNewLine)
            headerHTML.Append("<th>Contract Value</th>" & vbNewLine)
            headerHTML.Append("<th>Contract " & params.SupplierPrimaryTitle & "</th>" & vbNewLine)
            headerHTML.Append("<th>Is Contract Archived?</th>" & vbNewLine)
            headerHTML.Append("</tr>" & vbNewLine)

            rowClass = "row1"
            hasRows = False

            For Each drow_link In db.glDBWorkB.Tables(0).Rows
                rowalt = False

                tmpStr.Append("<div class=""inputpanel formpanel_padding"">" & vbNewLine)
                tmpStr.Append("<div class=""inputpaneltitle"">Link Definition:&nbsp;" & drow_link.Item("linkDefinition") & "</div>" & vbNewLine)
                tmpStr.Append(headerHTML)
                linkDef = drow_link.Item("linkDefinition")

                sql = New System.Text.StringBuilder
                sql.Append("SELECT [contract_details].[ContractId],[ContractKey],[ContractDescription],[ContractValue],[Archived],[ContractCurrency],[contract_details].[supplierId],[supplier_details].[suppliername] ")
                sql.Append("FROM [contract_details] ")
                sql.Append("LEFT JOIN [link_matrix] ON [link_matrix].[ContractId] = [contract_details].[ContractId] ")
                sql.Append("LEFT JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] ")
                sql.Append("LEFT JOIN [supplier_details] ON [contract_details].[supplierId] = [supplier_details].[supplierid] ")
                sql.Append("WHERE [link_matrix].[LinkId] = @linkId")
                sql.Append(" AND [contract_details].[subAccountId] = @locId")
                sql.Append(" ORDER BY [ContractDescription]")
                db.AddDBParam("locId", curUser.CurrentSubAccountId, True)
                db.AddDBParam("linkId", drow_link.Item("LinkId"), False)
                db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    rowalt = (rowalt Xor True)

                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    tmpStr.Append("<tr>" & vbNewLine)

                    If Val(drow.Item("ContractId")) = cId Then
                        tmpStr.Append("<td class=""" & rowClass & """><img src=""./images/arrow_right.png"" /></td>" & vbNewLine)
                    Else
                        tmpStr.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
                    End If

                    If IsDBNull(drow.Item("ContractKey")) = True Then
                        tmpData.Append("")
                    Else
                        tmpData.Append("<a onmouseover=""window.status='Open contract';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Trim(Str(drow.Item("ContractId"))) & """>")
                        tmpData.Append(Trim(drow.Item("ContractKey")))
                        tmpData.Append("</a>")
                    End If
                    tmpStr.Append("<td class=""" & rowClass & """>")
                    tmpStr.Append(tmpData)
                    tmpData = New System.Text.StringBuilder
                    tmpStr.Append("</td>" & vbNewLine)

                    If IsDBNull(drow.Item("ContractDescription")) = True Then
                        tmpData.Append("")
                    Else
                        tmpData.Append("<a onmouseover=""window.status='Open contract';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Trim(Str(drow.Item("ContractId"))) & """>")
                        tmpData.Append(Trim(drow.Item("ContractDescription")))
                        tmpData.Append("</a>")
                    End If
                    tmpStr.Append("<td class=""" & rowClass & """>")
                    tmpStr.Append(tmpData)
                    tmpData = New System.Text.StringBuilder
                    tmpStr.Append("</td>" & vbNewLine)

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False) Then
                        If IsDBNull(drow.Item("ContractValue")) = True Then
                            tmpData.Append("")
                        Else
                            If IsDBNull(drow.Item("ContractCurrency")) = True Then
                                tmpData.Append(currency.FormatCurrency(drow.Item("ContractValue"), baseCurrency, False))
                            Else
                                tmpData.Append(currency.FormatCurrency(drow.Item("ContractValue"), currency.getCurrencyById(drow.Item("ContractCurrency")), False))
                            End If
                        End If
                    Else
                        tmpData.Append("n/a")
                    End If
                    tmpStr.Append("<td class=""" & rowClass & """>")
                    tmpStr.Append(tmpData)
                    tmpData = New System.Text.StringBuilder
                    tmpStr.Append("</td>" & vbNewLine)

                    If IsDBNull(drow.Item("suppliername")) = True Then
                        tmpData.Append("")
                    Else
                        tmpData.Append("<a onmouseover=""window.status='Open " & params.SupplierPrimaryTitle & " record';return true;"" onmouseout=""window.status='Done';"" href=""supplier_details.aspx?sid=" & Trim(Str(drow.Item("supplierId"))) & """>")
                        tmpData.Append(Trim(drow.Item("suppliername")))
                        tmpData.Append("</a>")
                    End If
                    tmpStr.Append("<td class=""" & rowClass & """>")
                    tmpStr.Append(tmpData)
                    tmpData = New System.Text.StringBuilder
                    tmpStr.Append("</td>" & vbNewLine)
                    tmpStr.Append("<td class=""" & rowClass & """>" & IIf(drow.Item("Archived") = "N", "No", "Yes") & "</td>" & vbNewLine)
                    tmpStr.Append("</tr>" & vbNewLine)
                    hasRows = True
                Next

                tmpStr.Append("</table>" & vbNewLine)
                tmpStr.Append("</div>" & vbNewLine)
                'If hasRows = False Then
                '    tmpStr += "<tr><td class=""data"" align=""center"" colspan=""6"">No contract links returned</td></tr>" & vbNewLine
                'End If
            Next

            Return tmpStr.ToString
        End Function
#End Region
    End Class
End Namespace
