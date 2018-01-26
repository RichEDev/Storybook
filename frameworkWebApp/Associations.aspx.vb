Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class Associations
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
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim fwparams As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)
            Dim FWDb As New cFWDBConnection
            Dim helpID As String = ""
            Dim suppStr As String = "Supplier"

            If Me.IsPostBack = False Then
                ViewState("FromPage") = UCase(Request.QueryString("frompage"))
                FWDb.DBOpen(fws, False)

                Dim activeContract = 0
                If Request.QueryString("contractId") > "" Then
                    activeContract = Request.QueryString("contractId")
                    ViewState("ActiveContract") = activeContract
                End If
                

                ' set titles
                Select Case ViewState("FromPage")
                    Case "PRODUCTDETAILS", "VENDORDETAILS"
                        suppStr = fwparams.SupplierPrimaryTitle
                        lblTitle.Text = "Product-" & suppStr & " Association"
                        LoadProductVendorAssociations(FWDb)
                        helpID = "#1066"

                    Case "CONTRACTDETAILS"
                        lblTitle.Text = "Notifications"
                        LoadNotifications(FWDb)
                        helpID = ""
                    Case Else

                End Select

                Title = lblTitle.Text
                Master.title = Title

                lblAvailable.Text = "Available"
                lblSelected.Text = "Selected"

                Master.enablenavigation = False
                Master.useCloseNavigationMsg = True
                Master.RefreshBreadcrumbInfo()

                FWDb.DBClose()
                FWDb = Nothing
            End If

            cmdDeselect.AlternateText = "Deselect"
            cmdDeselect.ToolTip = "Move the highlighted elements to the Available Items"
            cmdDeselect.Attributes.Add("onmouseover", "window.status='Move the highlighted elements to the Available Items';return true;")
            cmdDeselect.Attributes.Add("onmouseout", "window.status='Done';")

            cmdSelect.ToolTip = "Move the highlighted elements to the Selected Items"
            cmdSelect.AlternateText = "Select"
            cmdSelect.Attributes.Add("onmouseover", "window.status='Move the highlighted elements to the Selected Items';return true;")
            cmdSelect.Attributes.Add("onmouseout", "window.status='Done';")

            cmdOK.AlternateText = "Close"
            cmdOK.ToolTip = "Exit to previous screen."
            cmdOK.Attributes.Add("onmouseover", "window.status='Exit to previous screen.';return true;")
            cmdOK.Attributes.Add("onmouseout", "window.status='Done';")
        End Sub

        Private Sub ReturnToWhenceICame()
            Dim redir As String

            Select Case CStr(ViewState("FromPage"))
                Case "PRODUCTDETAILS"
                    redir = "ProductDetails.aspx?item=" & Trim(Session("ProductText")) & "&id=" & Session("ProductId") & "&action=edit"

                Case "VENDORDETAILS"
                    redir = "Home.aspx"

                Case "CONTRACTDETAILS"
                    redir = "ContractSummary.aspx?id=" & Trim(ViewState("ActiveContract"))

                Case Else
                    redir = "Home.aspx"
            End Select

            Response.Redirect(redir, True)
        End Sub

        Private Sub LoadProductVendorAssociations(ByRef db As cFWDBConnection)
            Dim curID As Integer
            Dim sql, sql2 As String
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            curID = Session("ProductId")
            Select Case ViewState("FromPage")
                Case "PRODUCTDETAILS"
                    sql = "SELECT [product_suppliers].[supplierId],[supplier_details].[suppliername] FROM [product_suppliers]" & vbNewLine & _
     "INNER JOIN [supplier_details] ON [product_suppliers].[supplierId] = [supplier_details].[supplierid]" & vbNewLine & _
     "WHERE [ProductId] = @prodID ORDER BY [suppliername]"
                    db.AddDBParam("prodID", curID, True)
                    db.RunSQL(sql, db.glDBWorkA, False, "", False)

                    lstSelected.DataSource = db.glDBWorkA
                    lstSelected.DataTextField = "suppliername"
                    lstSelected.DataValueField = "supplierid"
                    lstSelected.DataBind()

                    sql2 = "SELECT [supplierid],[suppliername] FROM [supplier_details] WHERE [subAccountId] = @subaccID AND " & vbNewLine & _
                 "[supplierid] NOT IN " & vbNewLine & _
                 "(SELECT [supplierId] FROM [product_suppliers] WHERE [ProductId] = @productID) ORDER BY [suppliername]"
                    db.AddDBParam("subAccID", curUser.CurrentSubAccountId, True)
                    db.AddDBParam("productID", curID, False)
                    db.RunSQL(sql2, db.glDBWorkB, False, "", False)

                    lstAvailable.DataSource = db.glDBWorkB
                    lstAvailable.DataTextField = "suppliername"
                    lstAvailable.DataValueField = "supplierid"
                    lstAvailable.DataBind()

                Case "VENDORDETAILS"

                Case Else
            End Select
        End Sub

        Private Sub SelectItem()
            Dim x, reccount As Integer
            Dim db As New cFWDBConnection
            Dim ARec As New cAuditRecord
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ALog As cFWAuditLog

            With lstAvailable
                If Not .SelectedItem Is Nothing Then
                    db.DBOpen(fws, False)

                    Select Case CStr(ViewState("FromPage"))
                        Case "PRODUCTDETAILS", "VENDORDETAILS"
                            reccount = .Items.Count - 1
                            x = 0
                            Do While x <= reccount
                                If .Items(x).Selected = True Then
                                    .Items(x).Selected = False
                                    lstSelected.Items.Add(.Items(x))
                                    db.SetFieldValue("ProductId", Session("ProductId"), "N", True)
                                    db.SetFieldValue("supplierId", .Items(x).Value, "N", False)
                                    db.FWDb("W", "product_suppliers", "", "", "", "", "", "", "", "", "", "", "", "")
                                    ALog = New cFWAuditLog(fws, SpendManagementElement.Products, curUser.CurrentSubAccountId)
                                    ARec.Action = cFWAuditLog.AUDIT_ADD
                                    ARec.DataElementDesc = "PRODUCT/SUPPLIER ASSOC"
                                    ARec.ElementDesc = .Items(x).Text
                                    ARec.PreVal = "DESELECTED"
                                    ARec.PostVal = "SELECTED"
                                    ALog.AddAuditRec(ARec, True)
                                    ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))

                                    .Items.Remove(.Items(x))
                                    reccount = reccount - 1
                                Else
                                    x = x + 1
                                End If
                            Loop

                        Case "CONTRACTDETAILS"
                            reccount = .Items.Count - 1
                            x = 0
                            Do While x <= reccount
                                If .Items(x).Selected = True Then
                                    .Items(x).Selected = False
                                    lstSelected.Items.Add(.Items(x))
                                    db.SetFieldValue("ContractId", ViewState("ActiveContract"), "N", True)
                                    db.SetFieldValue("employeeId", .Items(x).Value.Replace("T", ""), "N", False)
                                    db.SetFieldValue("subAccountId", curUser.CurrentSubAccountId, "N", False)
                                    If .Items(x).Text.Substring(0, 2) = "**" Then
                                        ' must be a team
                                        db.SetFieldValue("IsTeam", AudienceType.Team, "N", False)
                                    Else
                                        db.SetFieldValue("IsTeam", AudienceType.Individual, "N", False)
                                    End If
                                    db.FWDb("W", "contract_notification", "", "", "", "", "", "", "", "", "", "", "", "")

                                    db.FWDb("R2", "contract_details", "ContractId", ViewState("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                                    If db.FWDb2Flag = True Then
                                        Dim tmpId As String
                                        tmpId = Trim(db.FWDbFindVal("ContractKey", 2))
                                        If tmpId = "" Then
                                            tmpId = Trim(db.FWDbFindVal("ContractNumber", 2))
                                        End If
                                        ARec.ContractNumber = tmpId
                                    End If

                                    ALog = New cFWAuditLog(fws, SpendManagementElement.ContractDetails, curUser.CurrentSubAccountId)
                                    ARec.Action = cFWAuditLog.AUDIT_ADD
                                    ARec.DataElementDesc = "CONTRACT NOTIFY"
                                    ARec.ElementDesc = .Items(x).Text
                                    ARec.PreVal = "DESELECTED"
                                    ARec.PostVal = "SELECTED"
                                    ALog.AddAuditRec(ARec, True)
                                    ALog.CommitAuditLog(curUser.Employee, ViewState("ActiveContract"))

                                    ARec.ElementDesc = "CONTRACT NOTIFY"
                                    ARec.PreVal = ""
                                    ARec.PostVal = .Items(x).Text
                                    ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.ContractDetail, ViewState("ActiveContract"), ARec)

                                    .Items.Remove(.Items(x))
                                    reccount = reccount - 1
                                Else
                                    x = x + 1
                                End If
                            Loop
                        Case Else
                            Exit Sub
                    End Select

                    db.DBClose()
                End If
            End With

            db = Nothing
        End Sub

        Private Sub DeselectItem()
            Dim x, reccount As Integer
            Dim db As New cFWDBConnection
            Dim ARec As New cAuditRecord
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ALog As cFWAuditLog

            With lstSelected
                If Not .SelectedItem Is Nothing Then
                    db.DBOpen(fws, False)

                    Select Case ViewState("FromPage")
                        Case "PRODUCTDETAILS", "VENDORDETAILS"
                            reccount = .Items.Count - 1
                            x = 0
                            Do While x <= reccount
                                If .Items(x).Selected = True Then
                                    .Items(x).Selected = False
                                    lstAvailable.Items.Add(.Items(x))
                                    db.FWDb("D", "product_suppliers", "ProductId", Session("ProductId"), "supplierId", .Items(x).Value, "", "", "", "", "", "", "", "")
                                    ALog = New cFWAuditLog(fws, SpendManagementElement.Products, curUser.CurrentSubAccountId)
                                    ARec.Action = cFWAuditLog.AUDIT_DEL
                                    ARec.DataElementDesc = "PRODUCT/SUPPLIER ASSOC"
                                    ARec.ElementDesc = .Items(x).Text
                                    ARec.PreVal = "SELECTED"
                                    ARec.PostVal = "DESELECTED"
                                    ALog.AddAuditRec(ARec, True)
                                    ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))

                                    .Items.Remove(.Items(x))
                                    reccount = reccount - 1
                                Else
                                    x = x + 1
                                End If
                            Loop

                        Case "CONTRACTDETAILS"
                            reccount = .Items.Count - 1
                            x = 0
                            Do While x <= reccount
                                If .Items(x).Selected = True Then
                                    .Items(x).Selected = False
                                    lstAvailable.Items.Add(.Items(x))

                                    db.FWDb("R2", "contract_details", "ContractId", ViewState("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                                    If db.FWDb2Flag = True Then
                                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
                                    End If

                                    Dim IsTeam As AudienceType
                                    If .Items(x).Text.Substring(0, 2) = "**" Then
                                        IsTeam = AudienceType.Team
                                    Else
                                        IsTeam = AudienceType.Individual
                                    End If
                                    db.FWDb("D", "contract_notification", "contractId", ViewState("ActiveContract"), "employeeId", .Items(x).Value.Replace("T", ""), "IsTeam", IsTeam, "", "", "", "", "", "")

                                    ALog = New cFWAuditLog(fws, SpendManagementElement.ContractDetails, curUser.CurrentSubAccountId)
                                    ARec.Action = cFWAuditLog.AUDIT_DEL
                                    ARec.DataElementDesc = "CONTRACT NOTIFY"
                                    ARec.ElementDesc = .Items(x).Text
                                    ARec.PreVal = "SELECTED"
                                    ARec.PostVal = "DESELECTED"
                                    ALog.AddAuditRec(ARec, True)
                                    ALog.CommitAuditLog(curUser.Employee, ViewState("ActiveContract"))

                                    ARec.ElementDesc = "CONTRACT NOTIFY"
                                    ARec.PreVal = .Items(x).Text
                                    ARec.PostVal = ""
                                    ContractRoutines.AddContractHistory(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), curUser.Employee, SummaryTabs.ContractDetail, ViewState("ActiveContract"), ARec)

                                    .Items.Remove(.Items(x))
                                    reccount = reccount - 1
                                Else
                                    x = x + 1
                                End If
                            Loop

                        Case Else
                            Exit Sub
                    End Select

                    db.DBClose()
                End If
            End With

            db = Nothing
        End Sub

        Private Sub LoadNotifications(ByVal db As cFWDBConnection)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim SelectedSQL As New System.Text.StringBuilder
            Dim curID As Integer
            Dim notifySQL As New System.Text.StringBuilder
            Dim drow As DataRow

            curID = ViewState("ActiveContract")

            notifySQL.Append("SELECT [contract_notification].[employeeId] AS [NotifyId], [employees].[firstname] + ' ' + employees.surname AS [NotifyName],[IsTeam] " & vbNewLine)
            notifySQL.Append("FROM [contract_notification] " & vbNewLine)
            notifySQL.Append("INNER JOIN [employees] ON [contract_notification].[employeeId] = [employees].[employeeId] " & vbNewLine)
            notifySQL.Append("WHERE [ContractId] = @conId AND IsTeam = @NoTeam " & vbNewLine)
            notifySQL.Append("UNION " & vbNewLine)
            notifySQL.Append("SELECT [teamid] AS [NotifyId] ,[teamname] AS [NotifyName],[IsTeam] FROM [contract_notification] " & vbNewLine)
            notifySQL.Append("INNER JOIN [teams] ON [teams].[teamid] = [contract_notification].[employeeId] " & vbNewLine)
            notifySQL.Append("WHERE [ContractId] = @conId AND [IsTeam] = @YesTeam " & vbNewLine)
            notifySQL.Append("ORDER BY [NotifyName] " & vbNewLine)
            db.AddDBParam("conId", curID, True)
            db.AddDBParam("YesTeam", AudienceType.Team, False)
            db.AddDBParam("NoTeam", AudienceType.Individual, False)

            'sql = "SELECT [contract_notification].[Staff Id],[staff_details].[Staff Name] FROM [contract_notification]" & vbNewLine & _
            '                "INNER JOIN [staff_details] ON [contract_notification].[Staff Id] = [staff_details].[Staff Id]" & vbNewLine & _
            '                "WHERE [Contract Id] = @conId ORDER BY [Staff Name]"
            'db.AddDBParam("conId", curID.ToString.Trim, True)
            db.RunSQL(notifySQL.ToString, db.glDBWorkA, False, "", False)

            Dim tmpDesc As String
            Dim prefixId As String

            For Each drow In db.glDBWorkA.Tables(0).Rows
                tmpDesc = ""
                prefixId = ""

                If drow.Item("IsTeam") = 1 Then
                    tmpDesc = "**"
                    prefixId = "T"
                End If
                tmpDesc += drow.Item("NotifyName")
                lstSelected.Items.Add(New ListItem(tmpDesc, prefixId & drow.Item("NotifyId")))
            Next

            SelectedSQL.Append("SELECT [employeeId] AS [NotifyId], firstname + ' ' + surname AS [NotifyName], 0 AS [IsTeam] FROM [employees] ")
            SelectedSQL.Append("WHERE [defaultSubAccountId] = @locId AND [employeeId] NOT IN " & vbNewLine)
            SelectedSQL.Append("(SELECT [employeeId] FROM [contract_notification] WHERE [ContractId] = @conId AND [IsTeam] = @NoTeam) " & vbNewLine)
            SelectedSQL.Append("UNION " & vbNewLine)
            SelectedSQL.Append("SELECT [teamid] AS [NotifyId], [teamname] AS [NotifyName], 1 AS [IsTeam] FROM [teams] WHERE [subaccountid] = @locId AND [teamid] NOT IN " & vbNewLine)
            SelectedSQL.Append("(SELECT [employeeId] FROM [contract_notification] WHERE [ContractId] = @conId AND [IsTeam] = @YesTeam) " & vbNewLine)
            SelectedSQL.Append("ORDER BY [NotifyName]" & vbNewLine)
            db.AddDBParam("locId", curUser.CurrentSubAccountId, True)
            db.AddDBParam("YesTeam", AudienceType.Team, False)
            db.AddDBParam("NoTeam", AudienceType.Individual, False)
            db.AddDBParam("staffTeam", TeamType.Employee, False)
            db.AddDBParam("conId", curID, False)

            db.RunSQL(SelectedSQL.ToString, db.glDBWorkB, False, "", False)

            For Each drow In db.glDBWorkB.Tables(0).Rows
                tmpDesc = ""
                prefixId = ""

                If drow.Item("IsTeam") = 1 Then
                    tmpDesc = "**"
                    prefixId = "T"
                End If
                tmpDesc += drow.Item("NotifyName")
                lstAvailable.Items.Add(New ListItem(tmpDesc, prefixId & drow.Item("NotifyId")))
            Next
        End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            ReturnToWhenceICame()
        End Sub

        Protected Sub cmdSelect_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            SelectItem()
        End Sub

        Protected Sub cmdDeselect_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            DeselectItem()
        End Sub
    End Class

End Namespace
