Imports System.Collections
Imports System.Collections.Generic
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management
Imports AjaxControlToolkit

Namespace Framework2006
    Partial Class SummaryPortal
        Inherits System.Web.UI.Page
        Private SearchCriteria As String

        Private Enum SearchType
            Equals = 0
            Wildcard = 1
            IsNULL = 2
        End Enum

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim clsBaseDefs As cBaseDefinitions
            Dim sql As String
            Dim action As String
            Dim Err As Integer

            Dim colParams As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            SetPanels(curUser, colParams)

            If Me.IsPostBack = False Then
                Title = "Advanced Search"
                Master.title = Title

                db.DBOpen(fws, False)

                action = Request.QueryString("action")
                If action = "dunsexport" Then
                    ExportDUNS(db)
                End If

                Err = Val(Request.QueryString("error"))
                Select Case Err
                    Case 1
                        lblErrorStatus.Text = "ERROR! Redirected due no active contract at the current time."

                    Case 2
                        lblErrorStatus.Text = "ERROR! Access denied to requested repository location."

                    Case Else

                End Select

                'Populate Contract Categories
                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractCategories)
                lstContractCategory.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                Dim tmpTitle As String = IIf(String.IsNullOrEmpty(colParams.ContractCategoryTitle), "Contract Category", colParams.ContractCategoryTitle)

                lblContractCategory.Text = tmpTitle

                'Populate Contract Types
                clsBaseDefs = New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ContractTypes)
                lstContractType.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                sql = "SELECT [LinkId],[LinkDefinition] FROM [link_definitions] WHERE [IsScheduleLink] = 0 AND [subAccountId] = @subAccId ORDER BY [LinkDefinition]"
                db.AddDBParam("subAccId", curUser.CurrentSubAccountId, True)
                db.RunSQL(sql, db.glDBWorkA, False, "", False)
                lstLinkDefs.DataSource = db.glDBWorkA
                lstLinkDefs.DataTextField = "LinkDefinition"
                lstLinkDefs.DataValueField = "LinkId"
                lstLinkDefs.DataBind()
                lstLinkDefs.Items.Insert(0, New ListItem("Select a definition", "0"))

                lstContractCategory.ToolTip = "Search for contracts held that are associated with selected " & tmpTitle
                lstContractType.ToolTip = "Search for contracts held that are associated with selected type"
                lstLinkDefs.ToolTip = "Displays all contracts linked to the selected definition"
                txtPONumber.ToolTip = "Search for Invoices or Forecasted Invoices with matching PO Number"
                txtInvoiceNo.ToolTip = "Search for Invoice number within Invoices or Invoice Forecasts"

                db.DBClose()

                SetPermissions()
            End If

            ' set contract status return to last value
            If Session("SP_Status") = Nothing Then
                Session("SP_Status") = "N"
            End If

            rdoContractStatus.Items.FindByValue(Session("SP_Status")).Selected = True

            ' output meaningful status bar messages for the Go buttons
            Dim moTxt, doneTxt, ov, ou, ttip As String
            moTxt = "window.status='Run %1 search on the database';return true;"
            ttip = "Run %1 search on the database"
            doneTxt = "window.status='Done';"
            ov = "onmouseover"
            ou = "onmouseout"

            cmdInvNo.AlternateText = "Go"
            cmdInvNo.ToolTip = Replace(ttip, "%1", "Invoice Number")
            cmdInvNo.Attributes.Add(ov, Replace(moTxt, "%1", "Invoice Number"))
            cmdInvNo.Attributes.Add(ou, doneTxt)

            cmdPONumber.AlternateText = "Go"
            cmdPONumber.ToolTip = Replace(ttip, "%1", "PO Number")
            cmdPONumber.Attributes.Add(ov, Replace(moTxt, "%1", "PO Number"))
            cmdPONumber.Attributes.Add(ou, doneTxt)

            lstLinkDefs.Attributes.Add(ov, Replace(moTxt, "%1", "Link Definition"))
            lstLinkDefs.Attributes.Add(ou, doneTxt)
            lstLinkDefs.ToolTip = Replace(ttip, "%1", "Link Definition")

            cmdSearch.AlternateText = "Search Again"
            cmdSearch.ToolTip = "Perform another search"
            cmdSearch.Attributes.Add(ov, "window.status='Perform another search';return true;")
            cmdSearch.Attributes.Add(ou, doneTxt)
            db = Nothing
        End Sub

        Private Sub ExportDUNS(ByVal db As cFWDBConnection)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim path, strOut As String
            Dim sql As New StringBuilder
            Dim drow As DataRow
            Dim x As Integer

            db.DBOpen(fws, False)

            sql.Append("SELECT [supplier_details].[supplierid],[suppliername],[supplier_addresses].[addr_line1],[supplier_addresses].[addr_line2],")
            sql.Append("[supplier_addresses].[town] as addr_line3,[supplier_addresses].[county] as addr_line4,[supplier_addresses].[postcode] as addr_line5 FROM [supplier_details]")
            sql.Append("LEFT OUTER JOIN [supplier_addresses] ON [supplier_details].[primary_addressid] = [supplier_addresses].[addressid]")
            sql.Append("WHERE [subAccountId] = @curLoc ORDER BY [supplier_details].[supplierid]")
            db.AddDBParam("curLoc", curUser.CurrentSubAccountId, True)
            db.RunSQL(sql.ToString(), db.glDBWorkA, False, "", False)

            path = Server.MapPath("./temp/tmpDUNSExport.csv")

            Dim file As New System.IO.StreamWriter(path, False)

            For Each drow In db.glDBWorkA.Tables(0).Rows
                strOut = drow.Item("supplierId") & "," & drow.Item("suppliername")
                For x = 1 To 5
                    If IsDBNull(drow.Item("addr_line " & Trim(Str(x)))) = True Then
                        strOut = strOut & ","
                    Else
                        strOut = strOut & "," & drow.Item("Addr_Line " & Trim(Str(x)))
                    End If
                Next
                'strOut = strOut & vbNewLine

                file.WriteLine(strOut)
            Next

            file.Close()

            litResults.Text = "<a onclick=""window.open('temp/tmpDUNSExport.csv');"">Click here to open DUNS Export file</a>"
        End Sub

        Private Sub InvNumberSearch()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim smData As New DBConnection(cAccounts.getConnectionString(curUser.Account.accountid))
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim sql As String
            Dim tmpStr As New System.Text.StringBuilder
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)

            If Trim(txtInvoiceNo.Text) <> "" Then
                sql = "SELECT contract_details.contractId, contract_details.contractKey, contract_details.contractDescription, contract_details.contractCurrency, invoiceStatusType.[description], invoices.poNumber, invoices.invoiceNumber,invoices.totalAmount FROM [invoices] INNER JOIN [contract_details] ON [contract_details].[ContractId] = [invoices].[ContractId] LEFT OUTER JOIN [invoiceStatusType] ON [invoiceStatusType].[invoiceStatusTypeId] = [invoices].[InvoiceStatus] WHERE LOWER([invoices].[InvoiceNumber]) LIKE LOWER(@invNo) AND [contract_details].[subAccountId] = @locId"
                smData.sqlexecute.Parameters.AddWithValue("@locId", curUser.CurrentSubAccountId)
                smData.sqlexecute.Parameters.AddWithValue("@invNo", "%" & txtInvoiceNo.Text & "%")

                Dim reader As System.Data.SqlClient.SqlDataReader
                reader = smData.GetReader(sql)

                Dim rowalt As Boolean = False
                Dim rowClass As String = "row1"

                If reader.HasRows = True Then
                    tmpStr.Append("<table class=""datatbl"">" & vbNewLine)
                    tmpStr.Append("<tr>" & vbNewLine)
                    tmpStr.Append("<th>Contract Key</th>" & vbNewLine)
                    tmpStr.Append("<th>Description</th>" & vbNewLine)
                    tmpStr.Append("<th>Invoice Number</th>" & vbNewLine)
                    tmpStr.Append("<th>PO Number</th>" & vbNewLine)
                    tmpStr.Append("<th>Invoice Amount</th>" & vbNewLine)
                    tmpStr.Append("<th>Invoice Status</th>" & vbNewLine)
                    tmpStr.Append("</tr>" & vbNewLine)

                    Dim contractID As Integer
                    Dim contractKey As String
                    Dim contractDescription As String
                    Dim invoiceStatusDesciption As String
                    Dim contractCurrency As Integer?
                    Dim poNumber As String
                    Dim invoiceNumber As String
                    Dim totalAmount As Decimal

                    Dim canViewFinancials As Boolean
                    canViewFinancials = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, False)

                    While reader.Read()
                        contractID = reader.GetInt32(reader.GetOrdinal("contractId"))
                        If reader.IsDBNull(reader.GetOrdinal("contractKey")) Then
                            contractKey = ""
                        Else
                            contractKey = reader.GetString(reader.GetOrdinal("contractKey"))
                        End If

                        If reader.IsDBNull(reader.GetOrdinal("contractDescription")) Then
                            contractDescription = ""
                        Else
                            contractDescription = reader.GetString(reader.GetOrdinal("contractDescription"))
                        End If

                        If reader.IsDBNull(reader.GetOrdinal("description")) Then
                            invoiceStatusDesciption = ""
                        Else
                            invoiceStatusDesciption = reader.GetString(reader.GetOrdinal("description"))
                        End If

                        If reader.IsDBNull(reader.GetOrdinal("contractCurrency")) Then
                            contractCurrency = Nothing
                        Else
                            contractCurrency = reader.GetInt32(reader.GetOrdinal("contractCurrency"))
                        End If

                        If reader.IsDBNull(reader.GetOrdinal("poNumber")) Then
                            poNumber = ""
                        Else
                            poNumber = reader.GetString(reader.GetOrdinal("poNumber"))
                        End If

                        If reader.IsDBNull(reader.GetOrdinal("invoiceNumber")) Then
                            invoiceNumber = ""
                        Else
                            invoiceNumber = reader.GetString(reader.GetOrdinal("invoiceNumber"))
                        End If

                        If reader.IsDBNull(reader.GetOrdinal("totalAmount")) Then
                            totalAmount = 0
                        Else
                            totalAmount = reader.GetDouble(reader.GetOrdinal("totalAmount"))
                        End If

                        rowalt = (rowalt Xor True)
                        If rowalt = True Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        tmpStr.Append("<tr>" & vbNewLine)

                        If contractKey <> "" Then
                            tmpStr.Append("<td class='" & rowClass & "'><a href='ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & contractID & "'>" & contractKey & "</a></td>")
                        Else
                            tmpStr.Append("<td class='" & rowClass & "'>&nbsp;</td>")
                        End If

                        If contractDescription <> "" Then
                            tmpStr.Append("<td class='" & rowClass & "'>" & contractDescription & "</td>")
                        Else
                            tmpStr.Append("<td class='" & rowClass & "'>Unknown</td>")
                        End If

                        If invoiceNumber <> "" Then
                            tmpStr.Append("<td class='" & rowClass & "'><a href='ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & contractID & "'>" & invoiceNumber & "</a></td>")
                        Else
                            tmpStr.Append("<td class='" & rowClass & "'>&nbsp;</td>")
                        End If

                        If poNumber <> "" Then
                            tmpStr.Append("<td class='" & rowClass & "'>" & poNumber & "</td>")
                        Else
                            tmpStr.Append("<td class='" & rowClass & "'>&nbsp;</td>")
                        End If

                        If canViewFinancials = True And Not contractCurrency Is Nothing Then
                            tmpStr.Append("<td class='" & rowClass & "'>" & currency.FormatCurrency(totalAmount, currency.getCurrencyById(contractCurrency), False) & "</td>")
                        Else
                            tmpStr.Append("<td class='" & rowClass & "'>n/a</td>")
                        End If

                        If invoiceStatusDesciption <> "" Then
                            tmpStr.Append("<td class='" & rowClass & "'>" & invoiceStatusDesciption & "</td>")
                        Else
                            tmpStr.Append("<td class='" & rowClass & "'>&nbsp;</td>")
                        End If

                        tmpStr.Append("</tr>" & vbNewLine)
                    End While

                    reader.Close()

                    tmpStr.Append("</table>")
                Else
                    tmpStr.Append("<table class=""datatbl""><tr><td class=""row1"">No matching Invoice Number found</td></tr></table>")
                End If

                litResults.Text = tmpStr.ToString
            End If

            SearchFieldsPanel.Visible = False
            ResultsPanel.Visible = True
        End Sub

        Private Sub PONumberSearch()
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim sql, dataStr As String
            Dim drow As DataRow
            Dim tmpStr As New System.Text.StringBuilder

            db.DBOpen(fws, False)
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)

            If txtPONumber.Text.Trim <> "" Then
                ' find PO Number in either invoices or invoice forecasts
                tmpStr.Append("<div class=""formpanel formpanel_padding"">" & vbNewLine)

                ' firstly, search the invoices
                sql = "SELECT [invoices].*,[invoiceStatusType].[description],[contract_details].[ContractCurrency],[contract_details].[ContractDescription],[contract_details].[ContractKey] FROM [invoices] "
                sql += "INNER JOIN [contract_details] ON [contract_details].[ContractId] = [invoices].[ContractId] "
                sql += "LEFT OUTER JOIN [invoiceStatusType] ON [invoiceStatusType].[InvoiceStatusTypeId] = [invoices].[InvoiceStatus] "
                sql += "WHERE LOWER([invoices].[poNumber]) LIKE LOWER(@PO) "
                sql += "AND [contract_details].[subAccountId] = @locId"
                db.AddDBParam("locId", curUser.CurrentSubAccountId, True)
                db.AddDBParam("PO", "%" & txtPONumber.Text.Trim & "%", False)
                db.RunSQL(sql, db.glDBWorkA, False, "", False)

                If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                    ' construct invoice information
                    tmpStr.Append("<div class=""inputpaneltitle"">Contract Invoices</div>" & vbNewLine)

                    tmpStr.Append("<table class=""datatbl"">" & vbNewLine)
                    tmpStr.Append("<tr>" & vbNewLine)
                    tmpStr.Append("<th>Contract Key</th>" & vbNewLine)
                    tmpStr.Append("<th>Description</th>" & vbNewLine)
                    tmpStr.Append("<th>Invoice Number</th>" & vbNewLine)
                    tmpStr.Append("<th>PO Number</th>" & vbNewLine)
                    tmpStr.Append("<th>Invoice Amount</th>" & vbNewLine)
                    tmpStr.Append("<th>Invoice Status</th>" & vbNewLine)
                    tmpStr.Append("</tr>" & vbNewLine)

                    Dim rowalt As Boolean = False
                    Dim rowClass As String = "row1"

                    For Each drow In db.glDBWorkA.Tables(0).Rows
                        rowalt = (rowalt Xor True)
                        If rowalt = True Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        tmpStr.Append("<tr>" & vbNewLine)
                        If IsDBNull(drow.Item("ContractKey")) = True Then
                            dataStr = "&nbsp;"
                        Else
                            dataStr = "<a href=""ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & drow.Item("ContractId") & """>" & Trim(drow.Item("ContractKey")) & "</a>"
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        If IsDBNull(drow.Item("ContractDescription")) = True Then
                            dataStr = "Unknown"
                        Else
                            dataStr = Trim(drow.Item("ContractDescription"))
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        ' construct the hyperlink to the required contract
                        If IsDBNull(drow.Item("InvoiceNumber")) = True Then
                            dataStr = "&nbsp;"
                        Else
                            dataStr = Trim(drow.Item("InvoiceNumber"))
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        dataStr = "<a href=""ContractSummary.aspx?tab=" & SummaryTabs.InvoiceDetail & "&id=" & Trim(drow.Item("ContractId")) & """>" & Trim(drow.Item("poNumber")) & "</a>"
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) = False Then
                            dataStr = "n/a"
                        Else
                            If IsDBNull(drow.Item("TotalAmount")) = True Then
                                dataStr = "0.00"
                            Else
                                If IsDBNull(drow.Item("ContractCurrency")) Then
                                    dataStr = drow.Item("TotalAmount")
                                Else
                                    dataStr = currency.FormatCurrency(drow.Item("TotalAmount"), currency.getCurrencyById(drow.Item("ContractCurrency")), False)
                                End If
                            End If
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        If IsDBNull(drow.Item("description")) = True Then
                            dataStr = "&nbsp;"
                        Else
                            dataStr = Trim(drow.Item("description"))
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)
                        tmpStr.Append("</tr>" & vbNewLine)
                    Next
                    tmpStr.Append("</table>")

                Else
                    tmpStr.Append("<table class=""datatbl""><tr><td class=""row1"" align=""center"">No matching PO Number found in Contract Invoices</td></tr></table>")
                End If

                tmpStr.Append("</div>" & vbNewLine)
                ' close the cell of the outer table and insert spacer and new title

                tmpStr.Append("<div class=""inputpanel"">" & vbNewLine)

                ' now check for entries in the Invoice Forecasts
                sql = "SELECT [contract_forecastdetails].*,[contract_details].[ContractCurrency],[contract_details].[ContractDescription],[contract_details].[ContractKey] FROM [contract_forecastdetails] "
                sql = sql & "INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_forecastdetails].[ContractId] "
                sql = sql & "WHERE [poNumber] LIKE @poNumber "
                sql = sql & "AND [contract_details].[subAccountId] = @subAccountId"
                db.AddDBParam("poNumber", "%" & txtPONumber.Text.Trim & "%", True)
                db.AddDBParam("subAccountId", curUser.CurrentSubAccountId, False)
                db.RunSQL(sql, db.glDBWorkB, False, "", False)

                If db.GetRowCount(db.glDBWorkB, 0) > 0 Then
                    tmpStr.Append("<div class=""inputpaneltitle"">Contract Forecasts</div>" & vbNewLine)

                    tmpStr.Append("<table class=""datatbl"">" & vbNewLine)
                    tmpStr.Append("<tr>" & vbNewLine)
                    tmpStr.Append("<th>Contract Key</th>" & vbNewLine)
                    tmpStr.Append("<th>Description</th>" & vbNewLine)
                    tmpStr.Append("<th>Payment Date</th>" & vbNewLine)
                    tmpStr.Append("<th>PO Number</th>" & vbNewLine)
                    tmpStr.Append("<th>Forecast Amount</th>" & vbNewLine)
                    tmpStr.Append("</tr>" & vbNewLine)

                    Dim rowalt As Boolean = False
                    Dim rowClass As String = "row1"

                    For Each drow In db.glDBWorkB.Tables(0).Rows
                        rowalt = (rowalt Xor True)
                        If rowalt = True Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        tmpStr.Append("<tr>" & vbNewLine)

                        If IsDBNull(drow.Item("ContractKey")) = True Then
                            dataStr = "&nbsp;"
                        Else
                            dataStr = "<a href=""ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & drow.Item("ContractId") & """>" & Trim(drow.Item("ContractKey")) & "</a>"
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        If IsDBNull(drow.Item("ContractDescription")) = True Then
                            dataStr = "&nbsp;"
                        Else
                            dataStr = Trim(drow.Item("ContractDescription"))
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        If IsDBNull(drow.Item("PaymentDate")) = True Then
                            dataStr = "&nbsp;"
                        Else
                            dataStr = Format(drow.Item("PaymentDate"), cDef.DATE_FORMAT)
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        ' hyperlink PO Number field
                        dataStr = "<a href=""ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Trim(drow.Item("ContractId")) & """>" & Trim(drow.Item("PONumber")) & "</a>"
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) = True Then
                            If IsDBNull(drow.Item("ForecastAmount")) = True Then
                                dataStr = "0.00"
                            Else
                                If IsDBNull(drow.Item("ContractCurrency")) Then
                                    dataStr = drow.Item("ForecastAmount")
                                Else
                                    dataStr = currency.FormatCurrency(drow.Item("ForecastAmount"), currency.getCurrencyById(drow.Item("ContractCurrency")), False)
                                End If
                            End If
                        Else
                            dataStr = "n/a"
                        End If
                        tmpStr.Append("<td class=""" & rowClass & """>" & dataStr & "</td>" & vbNewLine)

                        tmpStr.Append("</tr>" & vbNewLine)
                    Next
                    tmpStr.Append("</table>" & vbNewLine)
                Else
                    tmpStr.Append("<table class=""datatbl""><tr><td class=""row1"">No matching PO Number found in Contract Forecasts</td></tr></table>")
                End If

                tmpStr.Append("</div>" & vbNewLine)

                litResults.Text = tmpStr.ToString

                SearchFieldsPanel.Visible = False
                ResultsPanel.Visible = True
            End If

            db.DBClose()
            db = Nothing
        End Sub

        Private Sub lstLinkDefs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstLinkDefs.SelectedIndexChanged
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim rowClass As String
            Dim sql As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim hasRows As Boolean
            Dim strHTML As New System.Text.StringBuilder
            Dim tmpStr As System.Text.StringBuilder

            If lstLinkDefs.SelectedItem.Value = "0" Then
                litResults.Text = ""
                Exit Sub
            End If

            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim supplierStr As String = params.SupplierPrimaryTitle

            db.DBOpen(fws, False)

            sql.Append("SELECT [link_matrix].[ContractId],[contract_details].[ContractKey],[contract_details].[ContractDescription],[supplier_details].[suppliername],[contract_details].[Archived],[supplier_details].[supplierid] FROM [link_matrix] ")
            sql.Append("LEFT OUTER JOIN [contract_details] ON [contract_details].[ContractId] = [link_matrix].[ContractId] ")
            sql.Append("LEFT OUTER JOIN [supplier_details] ON [supplier_details].[supplierid] = [contract_details].[supplierId] ")
            sql.Append("WHERE [LinkId] = @lnkId ")
            sql.Append("ORDER BY [ContractDescription]")
            db.AddDBParam("lnkId", lstLinkDefs.SelectedValue, True)
            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

            hasRows = False
            rowClass = "row1"

            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th>Contract Key</th>" & vbNewLine)
            strHTML.Append("<th>Contract Description</th>" & vbNewLine)
            strHTML.Append("<th>" & supplierStr & "</th>" & vbNewLine)
            strHTML.Append("<th>Is Contract Archived?</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            Dim rowalt As Boolean = False

            For Each drow In db.glDBWorkA.Tables(0).Rows
                rowalt = (rowalt Xor True)
                If rowalt = True Then
                    rowClass = "row1"
                Else
                    rowClass = "row2"
                End If

                strHTML.Append("<tr>" & vbNewLine)

                tmpStr = Nothing
                tmpStr = New System.Text.StringBuilder

                If IsDBNull(drow.Item("ContractKey")) = True Then
                    tmpStr.Append("")
                Else
                    tmpStr.Append("<a onmouseover=""window.status='Open contract';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Trim(Str(drow.Item("ContractId"))) & """>")
                    tmpStr.Append(Trim(drow.Item("ContractKey")))
                    tmpStr.Append("</a>")
                End If
                strHTML.Append("<td class=""" & rowClass & """>")
                strHTML.Append(tmpStr)
                strHTML.Append("</td>" & vbNewLine)

                tmpStr = Nothing
                tmpStr = New System.Text.StringBuilder

                If IsDBNull(drow.Item("ContractDescription")) = True Then
                    tmpStr.Append("")
                Else
                    tmpStr.Append("<a onmouseover=""window.status='Open contract';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Trim(Str(drow.Item("ContractId"))) & """>")
                    tmpStr.Append(drow.Item("ContractDescription"))
                    tmpStr.Append("</a>")
                End If
                strHTML.Append("<td class=""" & rowClass & """>")
                strHTML.Append(tmpStr)
                strHTML.Append("</td>" & vbNewLine)

                tmpStr = Nothing
                tmpStr = New System.Text.StringBuilder

                If IsDBNull(drow.Item("suppliername")) = True Then
                    tmpStr.Append("")
                Else
                    tmpStr.Append("<a onmouseover=""window.status='Open supplier record';return true;"" onmouseout=""window.status='Done';"" href=""shared/supplier_details.aspx?sid=" & Trim(Str(drow.Item("supplierid"))) & """>")
                    tmpStr.Append(drow.Item("suppliername"))
                    tmpStr.Append("</a>")
                End If
                strHTML.Append("<td class=""" & rowClass & """>")
                strHTML.Append(tmpStr)
                strHTML.Append("</td>" & vbNewLine)

                strHTML.Append("<td class=""" & rowClass & """>" & IIf(drow.Item("Archived") = "N", "No", "Yes") & "</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
                hasRows = True
            Next

            If hasRows = False Then
                strHTML.Append("<tr><td class=""row1"" colspan=""4"">No contracts returned for the selected definition</td></tr>" & vbNewLine)
            End If

            strHTML.Append("</table>" & vbNewLine)
            litResults.Text = strHTML.ToString

            db.DBClose()
            db = Nothing
            SearchFieldsPanel.Visible = False
            ResultsPanel.Visible = True

        End Sub

        Private Sub cmdPONumber_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdPONumber.Click
            PONumberSearch()
        End Sub

        Private Sub cmdInvNo_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdInvNo.Click
            InvNumberSearch()
        End Sub

        Protected Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            ResultsPanel.Visible = False
            SearchFieldsPanel.Visible = True


        End Sub

        Private Sub SetPanels(ByVal curuser As CurrentUser, ByVal params As cAccountProperties)
            Dim xSize As Double

            'xSize = (CInt(Session("XRes")) / 100) * 75
            xSize = Unit.Percentage(85).Value

            igContractPanel.Width = xSize
            igLinkPanel.Width = xSize
            igInvPanel.Width = xSize
            igUFSearchPanel.Width = xSize

            ' if inv tabs are not visible, then remove from search portal
            'igInvPanel.Visible = uinfo.TabAccessList(CInt(SummaryTabs.InvoiceDetail)) -- NEEDS IMPLEMENTING

            Dim userfields As New cUserdefinedFields(curuser.AccountID)

            If userfields.getSearchableFields.Count > 0 Then
                igUFSearchPanel.Visible = True
                DisplayUFSearchFields(userfields)
            End If
        End Sub

        Private Sub DisplayUFSearchFields(ByVal userfields As cUserdefinedFields)
            Dim searchfields As SortedList(Of Integer, cUserDefinedField) = userfields.getSearchableFields
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim clsTables As New cTables(curUser.AccountID)
            Dim table As New Table

            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim valGroup As String = ""

            Dim trow As TableRow
            Dim tcell As TableCell
            'Dim employees As New cFWEmployees(fws, uinfo)

            Dim sField As cUserDefinedField
            Dim relatedTable As cTable
            Dim element As SpendManagementElement
            For Each sItem As KeyValuePair(Of Integer, cUserDefinedField) In searchfields
                sField = sItem.Value

                relatedTable = clsTables.getTableByUserdefineTableID(sField.table.tableid)

                If Not relatedTable.ElementID.HasValue Then
                    Continue For
                End If

                element = CType(relatedTable.ElementID.Value, SpendManagementElement)

                'If sField.table.tablename = "productDetails" And curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, False) Then
                If curUser.CheckAccessRole(AccessRoleType.View, element, False) Then
                    'Dim UFgrp As New cUserFieldGroupingCollection(fws, uinfo, sField.AppArea, 0, employees)

                    trow = New TableRow
                    tcell = New TableCell
                    tcell.CssClass = "labeltd"
                    tcell.Text = sField.attribute.displayname
                    trow.Cells.Add(tcell)

                    tcell = New TableCell
                    tcell.CssClass = "inputtd"

                    Select Case sField.fieldtype
                        Case FieldType.Relationship
                            Dim att As cManyToOneRelationship = CType(sField.attribute, cManyToOneRelationship)
                            Dim cntlID As String = "txtUF" + att.attributeid.ToString
                            Dim cntls As List(Of WebControl) = AutoComplete.createAutoCompleteControls(cntlID, att.displayname, "")
                            For Each cntl As WebControl In cntls
                                tcell.Controls.Add(cntl)
                            Next
                            tcell.Controls.Add(AutoComplete.getAutoCompleteInvalidEntryValidator(cntlID, att.displayname, valGroup))

                            Dim bindStr As New List(Of String)
                            bindStr.Add(AutoComplete.createAutoCompleteBindString(cntlID, att.AutoCompleteMatchRows, att.relatedtable.TableID, att.AutoCompleteDisplayField, att.AutoCompleteMatchFieldIDList))

                            ClientScript.RegisterStartupScript(Me.GetType(), "relSearch" + att.attributeid.ToString, AutoComplete.generateScriptRegisterBlock(bindStr), True)

                            '    Dim relTxt As New relationshipTextbox
                            '    Dim relTxtAtt As cRelationshipTextBoxAttribute = CType(sField.attribute, cRelationshipTextBoxAttribute)
                            '    relTxt.ID = "rtbUF" & sField.attribute.attributeid.ToString
                            '    valGroup = "rtbUF" & sField.attribute.attributeid.ToString
                            '    relTxt.FieldID = relTxtAtt.relatedtable.KeyFieldID
                            '    tcell.Controls.Add(relTxt)
                            '    'Dim reqtxt As New RequiredFieldValidator
                            '    'reqtxt.ID = "reqexUF" & sField.attribute.attributeid.ToString
                            '    'reqtxt.ControlToValidate = relTxt.ID & "_relationship_text_1"
                            '    'reqtxt.ValidationGroup = valGroup
                            '    'reqtxt.Text = "**"
                            '    'reqtxt.ErrorMessage = "A search parameter must be specified"
                            '    'tcell.Controls.Add(reqtxt)
                            '    'Dim reqextxt As New AjaxControlToolkit.ValidatorCalloutExtender
                            '    'reqextxt.ID = "reqextxtUF" & sField.attribute.attributeid.ToString
                            '    'reqextxt.TargetControlID = "reqexUF" & sField.attribute.attributeid.ToString
                            '    'tcell.Controls.Add(reqextxt)

                        Case FieldType.Text, FieldType.LargeText, FieldType.Hyperlink, FieldType.DynamicHyperlink
                            Dim txt As New TextBox
                            txt.ID = "txtUF" & sField.attribute.attributeid.ToString
                            valGroup = "txtUF" & sField.attribute.attributeid.ToString
                            txt.ValidationGroup = valGroup
                            tcell.Controls.Add(txt)
                            Dim reqtxt As New RequiredFieldValidator
                            reqtxt.ID = "reqexUF" & sField.attribute.attributeid.ToString
                            reqtxt.ControlToValidate = "txtUF" & sField.attribute.attributeid.ToString
                            reqtxt.ValidationGroup = valGroup
                            reqtxt.Text = "*"
                            reqtxt.ErrorMessage = "A search parameter must be specified"
                            tcell.Controls.Add(reqtxt)
                            Dim reqextxt As New AjaxControlToolkit.ValidatorCalloutExtender
                            reqextxt.ID = "reqextxtUF" & sField.attribute.attributeid.ToString
                            reqextxt.TargetControlID = "reqexUF" & sField.attribute.attributeid.ToString
                            tcell.Controls.Add(reqextxt)

                        Case FieldType.TickBox
                            Dim chk As New CheckBox
                            chk.ID = "chkUF" & sField.attribute.attributeid.ToString
                            valGroup = "chkUF" & sField.attribute.attributeid.ToString
                            chk.ValidationGroup = valGroup
                            tcell.Controls.Add(chk)

                        Case FieldType.DateTime
                            Dim dt As New TextBox
                            Dim dtAttribute As cDateTimeAttribute = CType(sField.attribute, cDateTimeAttribute)

                            dt.ID = "dateUF" & dtAttribute.attributeid.ToString
                            valGroup = "dateUF" & dtAttribute.attributeid.ToString
                            dt.ValidationGroup = valGroup
                            tcell.Controls.Add(dt)

                            Select Case CType(dtAttribute.format, AttributeFormat)
                                Case AttributeFormat.DateTime
                                    Dim maskededit As New MaskedEditExtender()
                                    maskededit.TargetControlID = dt.ID
                                    maskededit.Mask = "99/99/9999 99:99"
                                    maskededit.UserDateFormat = MaskedEditUserDateFormat.DayMonthYear
                                    maskededit.UserTimeFormat = MaskedEditUserTimeFormat.TwentyFourHour
                                    maskededit.MaskType = MaskedEditType.DateTime
                                    maskededit.ID = "mskUF" & dtAttribute.attributeid.ToString
                                    maskededit.CultureName = "en-GB"
                                    tcell.Controls.Add(maskededit)

                                    Dim regexvaldt = New RegularExpressionValidator()
                                    regexvaldt.ValidationExpression = "^((((31\/(0?[13578]|1[02]))|((29|30)\/(0?[1,3-9]|1[0-2])))\/(1[6-9]|[2-9]\d)?\d{2})|(29\/0?2\/(((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))|(0?[1-9]|1\d|2[0-8])\/((0?[1-9])|(1[0-2]))\/((1[6-9]|[2-9]\d)?\d{2})) (20|21|22|23|[0-1]?\d):[0-5]?\d$"
                                    regexvaldt.ID = "compregdatetime" & dtAttribute.attributeid.ToString
                                    regexvaldt.ControlToValidate = dt.ID
                                    regexvaldt.Text = "*"
                                    regexvaldt.ErrorMessage = "The value you have entered for " & dtAttribute.displayname & " is invalid."
                                    regexvaldt.ValidationGroup = valGroup

                                    Dim regexdttxt As New ValidatorCalloutExtender
                                    regexdttxt.ID = "regexdatetimeUF" & dtAttribute.attributeid.ToString
                                    regexdttxt.TargetControlID = regexvaldt.ID

                                    tcell.Controls.Add(regexvaldt)
                                    tcell.Controls.Add(regexdttxt)

                                Case AttributeFormat.TimeOnly
                                    Dim maskededit As New MaskedEditExtender()
                                    maskededit.TargetControlID = dt.ID
                                    maskededit.Mask = "99:99"
                                    maskededit.UserTimeFormat = MaskedEditUserTimeFormat.TwentyFourHour
                                    maskededit.MaskType = MaskedEditType.Time
                                    maskededit.ID = "mskUF" & dtAttribute.attributeid.ToString

                                    tcell.Controls.Add(maskededit)
                                    Dim regexval As New RegularExpressionValidator()
                                    regexval.ID = "regextime" & dtAttribute.attributeid.ToString
                                    regexval.ControlToValidate = dt.ID
                                    regexval.ValidationExpression = "([0-1]\d|2[0-3]):([0-5]\d)"
                                    regexval.ErrorMessage = "The value you have entered for " & dtAttribute.displayname & " is invalid. Valid time format is 00:00 to 23:59"
                                    regexval.ValidationGroup = valGroup
                                    regexval.Text = "*"

                                    Dim regextxt As New ValidatorCalloutExtender
                                    regextxt.ID = "reqextimeUF" & dtAttribute.attributeid.ToString
                                    regextxt.TargetControlID = regexval.ID

                                    tcell.Controls.Add(regexval)
                                    tcell.Controls.Add(regextxt)

                                Case AttributeFormat.DateOnly
                                    Dim ajaxcal As New AjaxControlToolkit.CalendarExtender
                                    ajaxcal.TargetControlID = dt.ID
                                    ajaxcal.Format = "dd/MM/yyyy"
                                    ajaxcal.PopupPosition = AjaxControlToolkit.CalendarPosition.BottomRight
                                    ajaxcal.ID = "calexUF" & dtAttribute.attributeid.ToString

                                    Dim valdt As New CompareValidator
                                    With valdt
                                        .ID = "valdateUF" & dtAttribute.attributeid.ToString
                                        .ValidationGroup = valGroup
                                        .Operator = ValidationCompareOperator.DataTypeCheck
                                        .Type = ValidationDataType.Date
                                        .ControlToValidate = dt.ID
                                        .Text = "*"
                                        .ErrorMessage = "Invalid date format entered"
                                        .SetFocusOnError = True
                                    End With

                                    Dim valexdt As New AjaxControlToolkit.ValidatorCalloutExtender
                                    With valexdt
                                        .ID = "valexdateUF" & dtAttribute.attributeid.ToString
                                        .TargetControlID = valdt.ID
                                    End With

                                    tcell.Controls.Add(ajaxcal)
                                    tcell.Controls.Add(valdt)
                                    tcell.Controls.Add(valexdt)

                            End Select

                            Dim reqtxt As New RequiredFieldValidator
                            reqtxt.ID = "reqexUF" & dtAttribute.attributeid.ToString
                            reqtxt.ControlToValidate = dt.ID
                            reqtxt.Text = "*"
                            reqtxt.ValidationGroup = valGroup
                            reqtxt.ErrorMessage = "A search parameter must be specified"
                            tcell.Controls.Add(reqtxt)
                            Dim reqextxt As New AjaxControlToolkit.ValidatorCalloutExtender
                            reqextxt.ID = "reqexdateUF" & dtAttribute.attributeid.ToString
                            reqextxt.TargetControlID = reqtxt.ID
                            tcell.Controls.Add(reqextxt)

                        Case FieldType.List
                            Dim ddl As New DropDownList
                            ddl.ID = "ddlUF" & sField.attribute.attributeid.ToString
                            valGroup = "ddlUF" & sField.attribute.attributeid.ToString
                            ddl.ValidationGroup = valGroup
                            ddl.Items.AddRange(CreateListItemArray(sField.items))
                            tcell.Controls.Add(ddl)
                            Dim cmp As New CompareValidator
                            cmp.ID = "cmpUF" & sField.attribute.attributeid.ToString
                            cmp.ControlToValidate = ddl.ID
                            cmp.ValidationGroup = valGroup
                            cmp.Type = ValidationDataType.Integer
                            cmp.Operator = ValidationCompareOperator.GreaterThan
                            cmp.ValueToCompare = 0
                            cmp.SetFocusOnError = True
                            cmp.Text = "*"
                            cmp.ErrorMessage = "A search selection must be made"
                            tcell.Controls.Add(cmp)
                            Dim cmpex As New AjaxControlToolkit.ValidatorCalloutExtender
                            cmpex.ID = "cmpexUF" & sField.attribute.attributeid.ToString
                            cmpex.TargetControlID = cmp.ID
                            tcell.Controls.Add(cmpex)

                        Case FieldType.Currency, FieldType.Number
                            Dim num As New TextBox
                            num.ID = "numUF" & sField.attribute.attributeid.ToString
                            valGroup = "numUF" & sField.attribute.attributeid.ToString
                            num.ValidationGroup = valGroup
                            Dim cmpnum As New CompareValidator
                            With cmpnum
                                .ID = "cmpnumUF" & sField.attribute.attributeid.ToString
                                .ValidationGroup = valGroup
                                .ControlToValidate = num.ID
                                .Text = "*"
                                .ErrorMessage = "Invalid decimal value entered"
                                .SetFocusOnError = True
                                .Operator = ValidationCompareOperator.DataTypeCheck
                                .Type = ValidationDataType.Double
                            End With
                            Dim cmpexnum As New AjaxControlToolkit.ValidatorCalloutExtender
                            cmpexnum.ID = "cmpexnumUF" & sField.attribute.attributeid.ToString
                            cmpexnum.TargetControlID = cmpnum.ID
                            tcell.Controls.Add(num)
                            tcell.Controls.Add(cmpnum)
                            tcell.Controls.Add(cmpexnum)

                        Case FieldType.Integer
                            Dim num As New TextBox
                            num.ID = "numUF" & sField.attribute.attributeid.ToString
                            valGroup = "numUF" & sField.attribute.attributeid.ToString
                            num.ValidationGroup = valGroup
                            Dim cmpnum As New CompareValidator
                            With cmpnum
                                .ID = "cmpnumUF" & sField.attribute.attributeid.ToString
                                .ValidationGroup = valGroup
                                .ControlToValidate = num.ID
                                .Text = "*"
                                .ErrorMessage = "Invalid integer value entered"
                                .SetFocusOnError = True
                                .Operator = ValidationCompareOperator.DataTypeCheck
                                .Type = ValidationDataType.Integer
                            End With
                            Dim cmpexnum As New AjaxControlToolkit.ValidatorCalloutExtender
                            cmpexnum.ID = "cmpexnumUF" & sField.attribute.attributeid.ToString
                            cmpexnum.TargetControlID = cmpnum.ID
                            tcell.Controls.Add(num)
                            tcell.Controls.Add(cmpnum)
                            tcell.Controls.Add(cmpexnum)

                            'Case UserFieldType.RechargeAcc_Code
                            '    Dim txt As New TextBox
                            '    txt.ID = "txtUF" & sField.attribute.attributeid.ToString
                            '    valGroup = "txtUF" & sField.attribute.attributeid.ToString
                            '    txt.ValidationGroup = valGroup
                            '    txt.Text = "Sorry - currently unavailable"
                            '    txt.ReadOnly = True
                            '    tcell.Controls.Add(txt)

                            'Case UserFieldType.RechargeClient_Ref
                            '    Dim clients As New cRechargeClientList(uinfo, fws)
                            '    Dim ddl As New DropDownList
                            '    ddl.ID = "ddlUF" & sField.attribute.attributeid.ToString
                            '    valGroup = "ddlUF" & sField.attribute.attributeid.ToString
                            '    ddl.ValidationGroup = valGroup
                            '    ddl.Items.AddRange(clients.GetListControlItems(True, True))
                            '    tcell.Controls.Add(ddl)

                            '    Dim cmp As New CompareValidator
                            '    With cmp
                            '        .ID = "cmpUF" & sField.attribute.attributeid.ToString
                            '        .ControlToValidate = ddl.ID
                            '        .Operator = ValidationCompareOperator.GreaterThan
                            '        .ValueToCompare = 0
                            '        .Text = "**"
                            '        .ErrorMessage = "A valid selection must be made for searching"
                            '        .SetFocusOnError = True
                            '        .Type = ValidationDataType.Integer
                            '    End With
                            '    tcell.Controls.Add(cmp)
                            '    Dim cmpex As New AjaxControlToolkit.ValidatorCalloutExtender
                            '    cmpex.ID = "cmpexUF" & sField.attribute.attributeid.ToString
                            '    cmpex.TargetControlID = cmp.ID
                            '    tcell.Controls.Add(cmpex)

                            'Case UserFieldType.Site_Ref
                            '    Dim sites As New cSites(fws, uinfo)
                            '    Dim ddl As New DropDownList
                            '    ddl.ID = "ddlUF" & sField.attribute.attributeid.ToString
                            '    valGroup = "ddlUF" & sField.attribute.attributeid.ToString
                            '    ddl.ValidationGroup = valGroup
                            '    ddl.Items.AddRange(sites.GetListItems(True))
                            '    tcell.Controls.Add(ddl)

                            '    Dim cmp As New CompareValidator
                            '    With cmp
                            '        .ID = "cmp" & sField.attribute.attributeid.ToString
                            '        .ControlToValidate = ddl.ID
                            '        .ValidationGroup = valGroup
                            '        .Operator = ValidationCompareOperator.GreaterThan
                            '        .ValueToCompare = 0
                            '        .Text = "**"
                            '        .ErrorMessage = "A valid selection must be made for searching"
                            '        .SetFocusOnError = True
                            '        .Type = ValidationDataType.Integer
                            '    End With
                            '    tcell.Controls.Add(cmp)
                            '    Dim cmpex As New AjaxControlToolkit.ValidatorCalloutExtender
                            '    cmpex.ID = "cmpexUF" & sField.attribute.attributeid.ToString
                            '    cmpex.TargetControlID = cmp.ID
                            '    tcell.Controls.Add(cmpex)

                            'Case UserFieldType.StaffName_Ref
                            '    Dim ddl As New DropDownList
                            '    ddl.ID = "ddlUF" & sField.attribute.attributeid.ToString
                            '    valGroup = "ddlUF" & sField.attribute.attributeid.ToString
                            '    ddl.ValidationGroup = valGroup
                            '    ddl.Items.AddRange(employees.GetListControlItems(True, True))
                            '    tcell.Controls.Add(ddl)

                            '    Dim cmp As New CompareValidator
                            '    With cmp
                            '        .ID = "cmpUF" & sField.attribute.attributeid.ToString
                            '        .ControlToValidate = ddl.ID
                            '        .ValidationGroup = valGroup
                            '        .Operator = ValidationCompareOperator.GreaterThan
                            '        .ValueToCompare = 0
                            '        .Text = "**"
                            '        .ErrorMessage = "A valid selection must be made for searching"
                            '        .SetFocusOnError = True
                            '        .Type = ValidationDataType.Integer
                            '    End With
                            '    tcell.Controls.Add(cmp)
                            '    Dim cmpex As New AjaxControlToolkit.ValidatorCalloutExtender
                            '    cmpex.ID = "cmpexUF" & sField.attribute.attributeid.ToString
                            '    cmpex.TargetControlID = cmp.ID
                            '    tcell.Controls.Add(cmpex)

                            'Case UserFieldType.Site_Ref
                            '    Dim sites As New cSites(fws, uinfo)
                            '    Dim ddl As New DropDownList
                            '    ddl.ID = "ddlUF" & sField.attribute.attributeid.ToString
                            '    valGroup = "ddlUF" & sField.attribute.attributeid.ToString
                            '    ddl.ValidationGroup = valGroup
                            '    ddl.Items.AddRange(sites.GetListItems(True))
                            '    tcell.Controls.Add(ddl)

                            '    Dim cmp As New CompareValidator
                            '    With cmp
                            '        .ID = "cmp" & sField.attribute.attributeid.ToString
                            '        .ControlToValidate = ddl.ID
                            '        .ValidationGroup = valGroup
                            '        .Operator = ValidationCompareOperator.GreaterThan
                            '        .ValueToCompare = 0
                            '        .Text = "**"
                            '        .ErrorMessage = "A valid selection must be made for searching"
                            '        .SetFocusOnError = True
                            '        .Type = ValidationDataType.Integer
                            '    End With
                            '    tcell.Controls.Add(cmp)
                            '    Dim cmpex As New AjaxControlToolkit.ValidatorCalloutExtender
                            '    cmpex.ID = "cmpexUF" & sField.attribute.attributeid.ToString
                            '    cmpex.TargetControlID = cmp.ID
                            '    tcell.Controls.Add(cmpex)

                            'Case UserFieldType.Text
                            '    Dim txt As New TextBox
                            '    txt.ID = "txtUF" & sField.attribute.attributeid.ToString
                            '    valGroup = "txtUF" & sField.attribute.attributeid.ToString
                            '    txt.ValidationGroup = valGroup
                            '    txt.Rows = 3
                            '    txt.TextMode = TextBoxMode.MultiLine
                            '    tcell.Controls.Add(txt)
                            '    Dim reqtxt As New RequiredFieldValidator
                            '    reqtxt.ID = "reqexUF" & sField.attribute.attributeid.ToString
                            '    reqtxt.ControlToValidate = txt.ID
                            '    reqtxt.ValidationGroup = valGroup
                            '    reqtxt.Text = "**"
                            '    reqtxt.ErrorMessage = "A search parameter must be specified"
                            '    tcell.Controls.Add(reqtxt)
                            '    Dim reqextxt As New AjaxControlToolkit.ValidatorCalloutExtender
                            '    reqextxt.ID = "reqextxtUF" & sField.attribute.attributeid.ToString
                            '    reqextxt.TargetControlID = reqtxt.ID
                            '    tcell.Controls.Add(reqextxt)
                        Case Else

                    End Select
                    trow.Cells.Add(tcell)

                    tcell = New TableCell
                    tcell.CssClass = ""
                    Dim imgbtn As New ImageButton
                    With imgbtn
                        .ID = "cmdUF" & sField.attribute.attributeid.ToString
                        .CommandName = "String"
                        .CommandArgument = sField.attribute.attributeid.ToString
                        .ToolTip = "Perform search"
                        .ValidationGroup = valGroup
                        .CssClass = "advance_search_icon"
                        .Attributes.Add("onmouseover", "window.status='Perform search for " & sField.attribute.displayname.Replace("'", "`") & "';return true;")
                        .Attributes.Add("onmouseout", "window.status='Done';")
                        .ImageUrl = "~/icons/16/plain/find.png"
                    End With
                    AddHandler imgbtn.Click, AddressOf SearchStr_Click
                    tcell.Controls.Add(imgbtn)
                    trow.Cells.Add(tcell)

                    tcell = New TableCell
                    tcell.CssClass = ""

                    'Select Case sField.table.tablename
                    Select Case relatedTable.tablename
                        Case "productDetails"
                            If Not sField.Grouping Is Nothing Then
                                tcell.Text = "Contract Product Grouping : " & sField.Grouping.GroupName
                            Else
                                tcell.Text = "Product Details"
                            End If
                        Case "productLicences"
                            If Not sField.Grouping Is Nothing Then
                                tcell.Text = "Product Licences Grouping : " & sField.Grouping.GroupName
                            Else
                                tcell.Text = "Product Licences"
                            End If
                        Case "contract_details"
                            If Not sField.Grouping Is Nothing Then
                                tcell.Text = "Contract Additional : " & sField.Grouping.GroupName
                            Else
                                tcell.Text = "Contract Details"
                            End If
                        Case "contract_productdetails"
                            If Not sField.Grouping Is Nothing Then
                                tcell.Text = "Contract Product Grouping : " & sField.Grouping.GroupName
                            Else
                                tcell.Text = "Contract Products"
                            End If
                            'Case AppAreas.RECHARGE_GROUPING
                            '    Dim curgrp As cUserFieldGrouping = UFgrp.GetGroupingById(sField.GroupId)
                            '    tcell.Text = "Recharge Grouping : " & curgrp.GroupingDescription
                        Case "employees"
                            tcell.Text = "Employee Details"
                        Case "supplier_details"
                            If Not sField.Grouping Is Nothing Then
                                tcell.Text = params.SupplierPrimaryTitle & " Additional : " & sField.Grouping.GroupName
                            Else
                                tcell.Text = params.SupplierPrimaryTitle & " Details"
                            End If
                        Case "supplier_contacts"
                            If Not sField.Grouping Is Nothing Then
                                tcell.Text = params.SupplierPrimaryTitle & " Contacts : " & sField.Grouping.GroupName
                            Else
                                tcell.Text = params.SupplierPrimaryTitle & " Contacts"
                            End If
                        Case "invoices"
                            If Not sField.Grouping Is Nothing Then
                                tcell.Text = " Invoices : " & sField.Grouping.GroupName
                            Else
                                tcell.Text = " Invoices"
                            End If
                        Case Else
                            tcell.Text = "Unknown Application Area"
                    End Select

                    trow.Cells.Add(tcell)
                    table.Rows.Add(trow)
                End If
            Next
            igUFSearchPanel.Controls.Add(table)
        End Sub

        Private Function CreateListItemArray(ByVal items As SortedList(Of Integer, cListAttributeElement)) As ListItem()
            Dim listItems As New List(Of ListItem)
            Dim sorted As New SortedList(Of String, cListAttributeElement)

            For Each i As KeyValuePair(Of Integer, cListAttributeElement) In items
                Dim item As cListAttributeElement = i.Value

                sorted.Add(item.elementText, item)
            Next

            For Each s As KeyValuePair(Of String, cListAttributeElement) In sorted
                Dim item As cListAttributeElement = s.Value

                listItems.Add(New ListItem(item.elementText, item.elementValue))
            Next

            listItems.Insert(0, New ListItem("[None]", "0"))

            Return listItems.ToArray
        End Function

        Protected Sub SearchStr_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
            Dim imgbutton As ImageButton = CType(sender, ImageButton)
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim emps As New cEmployees(curUser.AccountID)
            Dim ufields As New cUserdefinedFields(curUser.AccountID)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            Dim cmpType As SearchType
            Dim searchVal As String = ""

            If Not imgbutton Is Nothing Then
                Dim ufield As cUserDefinedField = ufields.getUserDefinedById(Integer.Parse(imgbutton.CommandArgument))

                Select Case ufield.fieldtype
                    Case FieldType.Relationship
                        Dim txt As TextBox = igUFSearchPanel.FindControl("txtUF" & ufield.attribute.attributeid.ToString & "_ID")
                        cmpType = SearchType.Equals
                        If Not txt Is Nothing Then
                            searchVal = txt.Text
                        End If

                    Case FieldType.Hyperlink, FieldType.Text, FieldType.LargeText, FieldType.DynamicHyperlink
                        Dim txt As TextBox = igUFSearchPanel.FindControl("txtUF" & ufield.attribute.attributeid.ToString)
                        cmpType = SearchType.Wildcard
                        If Not txt Is Nothing Then
                            searchVal = txt.Text
                        End If

                    Case FieldType.TickBox
                        Dim chk As CheckBox = igUFSearchPanel.FindControl("chkUF" & ufield.attribute.attributeid.ToString)
                        cmpType = SearchType.Equals
                        If Not chk Is Nothing Then
                            If chk.Checked Then
                                searchVal = "1"
                            Else
                                searchVal = "0"
                            End If
                        End If

                    Case FieldType.DateTime
                        Dim dt As TextBox = igUFSearchPanel.FindControl("dateUF" & ufield.attribute.attributeid.ToString)
                        cmpType = SearchType.Equals
                        If Not dt Is Nothing Then
                            searchVal = dt.Text
                        End If

                    Case FieldType.List
                        Dim lst As DropDownList = igUFSearchPanel.FindControl("ddlUF" & ufield.attribute.attributeid.ToString)
                        cmpType = SearchType.Equals
                        If Not lst Is Nothing Then
                            searchVal = lst.SelectedItem.Value
                        End If

                        If searchVal = "0" Then
                            cmpType = SearchType.IsNULL
                        End If

                    Case FieldType.Currency, FieldType.Integer, FieldType.Number
                        Dim num As TextBox = igUFSearchPanel.FindControl("numUF" & ufield.attribute.attributeid.ToString)
                        cmpType = SearchType.Equals
                        If Not num Is Nothing Then
                            searchVal = num.Text
                        End If

                        'Case UserFieldType.RechargeAcc_Code

                        'Case UserFieldType.RechargeClient_Ref, UserFieldType.StaffName_Ref, UserFieldType.Site_Ref
                        '    Dim ref As DropDownList = igUFSearchPanel.FindControl("ddlUF" & ufield.attribute.attributeid.ToString)
                        '    cmpType = SearchType.Equals
                        '    If Not ref Is Nothing Then
                        '        searchVal = ref.SelectedItem.Value
                        '    End If
                        'Case FieldType.RelationshipTextbox
                        '    Dim tmpCntlID As String = "rtbUF" & ufield.attribute.attributeid.ToString
                        '    Dim relTxt As relationshipTextbox = CType(igUFSearchPanel.FindControl(tmpCntlID), relationshipTextbox)
                        '    cmpType = SearchType.Equals

                        '    If Not relTxt Is Nothing Then
                        '        Dim relTextBoxAtt As cRelationshipTextBoxAttribute = CType(ufield.attribute, cRelationshipTextBoxAttribute)
                        '        Dim relTxtQuery As New cQueryBuilder(curUser.AccountID, cAccounts.getConnectionString(curUser.AccountID), ConfigurationManager.ConnectionStrings("metabase").ConnectionString, relTextBoxAtt.relatedtable, New cTables(curUser.AccountID), New cFields(curUser.AccountID))

                        '        relTxtQuery.addColumn(relTextBoxAtt.relatedtable.PrimaryKey)
                        '        relTxtQuery.addFilter(relTextBoxAtt.relatedtable.KeyField, ConditionType.Equals, New Object() {relTxt.TextBox.Text}, Nothing, ConditionJoiner.None)

                        '        Using reader As System.Data.SqlClient.SqlDataReader = relTxtQuery.getReader
                        '            While (reader.Read())
                        '                If Not reader.IsDBNull(0) Then
                        '                    searchVal = reader.GetInt32(0)
                        '                End If
                        '            End While
                        '            reader.Close()
                        '        End Using
                        '    End If
                    Case Else

                End Select

                Dim tables As New cTables(curUser.AccountID)
                Dim baseTable As String = ufield.table.tablename
                Dim parentTable As String = ""
                Dim sortField As String = ""
                Dim joinString As New StringBuilder
                Dim joinFields As New StringBuilder
                Dim fieldAlias As String = ""
                Dim subAccountFilter As String = ""

                Select Case ufield.table.tablename.ToLower
                    Case "userdefinedcontractdetails"
                        sortField = "contractDescription"
                        fieldAlias = "Contract Description"
                        parentTable = "contract_details"
                        joinString.Append(" INNER JOIN contract_details ON contract_details.[contractId] = userdefinedcontractdetails.[contractId]")
                        joinString.Append(" INNER JOIN supplier_details ON contract_details.[supplierId] = supplier_details.[supplierId]")
                        joinFields.Append(", supplier_details.[supplierName] ")
                        subAccountFilter = " AND contract_details.subAccountId = @subAccountId "

                    Case "userdefinedcontractproductdetails"
                        sortField = "productName"
                        fieldAlias = "Product Name"
                        parentTable = "contract_productdetails"
                        joinString.Append(" INNER JOIN contract_productdetails ON contract_productdetails.[contractProductId] = userdefinedcontractproductDetails.[contractProductId] ")
                        joinString.Append(" INNER JOIN productDetails ON contract_productdetails.[ProductId] = productDetails.[ProductId] ")
                        joinFields.Append(", productDetails.[ProductName] ")
                        subAccountFilter = " AND productDetails.subAccountId = @subAccountId "

                    Case "userdefinedproductlicences"
                        sortField = "productName"
                        fieldAlias = "Product Name"
                        parentTable = "productLicences"
                        joinString.Append(" INNER JOIN [productLicences] ON productLicences.licenceID = userdefinedProductLicences.licenceID")
                        joinString.Append(" INNER JOIN productDetails ON productDetails.productID = productLicences.productID")
                        joinFields.Append(", productDetails.productId, productDetails.[ProductName] ")
                        subAccountFilter = " AND productDetails.subAccountId = @subAccountId "

                    Case "userdefinedproductdetails"
                        sortField = "ProductName"
                        fieldAlias = "Product Name"
                        parentTable = "productDetails"
                        joinString.Append(" INNER JOIN productDetails ON userdefinedProductDetails.[ProductId] = productDetails.[ProductId] ")
                        subAccountFilter = " AND productDetails.subAccountId = @subAccountId "

                    Case "userdefined_employees"
                        sortField = "fullName"
                        fieldAlias = "Full Name"
                        parentTable = "employees"
                        joinString.Append(" INNER JOIN employees ON employees.employeeId = userdefined_employees.employeeId ")
                        joinFields.Append(", dbo.getEmployeeFullName(employees.employeeId) as [fullName] ")

                    Case "userdefinedsupplierdetails"
                        sortField = "supplierName"
                        fieldAlias = params.SupplierPrimaryTitle & " Name"
                        parentTable = "supplier_details"
                        joinString.Append(" INNER JOIN supplier_details ON supplier_details.supplierId = userdefinedsupplierdetails.supplierId ")
                        subAccountFilter = " AND supplier_details.subAccountId = @subAccountId "

                    Case "userdefinedsuppliercontacts"
                        sortField = "contactName"
                        fieldAlias = "Contact Name"
                        parentTable = "supplier_contacts"
                        joinString.Append(" INNER JOIN supplier_contacts ON supplier_contacts.[contactId] = userdefinedSupplierContacts.[contactId]")
                        joinString.Append(" INNER JOIN supplier_details ON supplier_contacts.[supplierId] = supplier_details.[supplierId]")
                        joinFields.Append(", supplier_details.[supplierName] ")
                        subAccountFilter = " AND supplier_details.subAccountId = @subAccountId "

                        'Case "userdefinedrechargeassociations"
                        '    sortField = "Name"
                        '    joinString.Append("INNER JOIN codes_rechargeentity ON recharge_associations.[RechargeEntityId] = codes_rechargeentity.[EntityId]")
                        '    joinFields.Append(", recharge_associations.Name ")
                    Case Else

                End Select

                Dim sql As New StringBuilder
                sql.Append("SELECT ")
                sql.Append(parentTable & ".*, ")
                sql.Append(baseTable & ".* ")
                sql.Append(joinFields)
                sql.Append("FROM ")
                sql.Append(baseTable)
                sql.Append(joinString)

                Select Case ufield.attribute.fieldtype
                    Case FieldType.DateTime
                        Dim dt As cDateTimeAttribute = CType(ufield.attribute, cDateTimeAttribute)
                        Select Case dt.format
                            Case AttributeFormat.TimeOnly
                                sql.Append(" WHERE CONVERT(time,[udf" & ufield.userdefineid.ToString & "],120) ")
                            Case AttributeFormat.DateOnly
                                sql.Append(" WHERE CONVERT(date,[udf" & ufield.userdefineid.ToString & "],120) ")
                            Case AttributeFormat.DateTime
                                sql.Append(" WHERE CONVERT(datetime,[udf" & ufield.userdefineid.ToString & "],120) ")
                            Case Else
                                sql.Append(" WHERE [udf" & ufield.userdefineid.ToString & "] ")
                        End Select

                    Case Else
                        sql.Append(" WHERE [udf" & ufield.userdefineid.ToString & "] ")
                End Select

                Select Case cmpType
                    Case SearchType.IsNULL
                        sql.Append(" IS NULL ")
                    Case SearchType.Equals
                        sql.Append(" = ")
                    Case SearchType.Wildcard
                        sql.Append(" LIKE ")
                End Select

                Select Case CType(ufield.fieldtype, FieldType)
                    Case FieldType.DateTime
                        Dim dtAttribute As cDateTimeAttribute = CType(ufield.attribute, cDateTimeAttribute)
                        Dim dOrigDate As DateTime = Convert.ToDateTime(searchVal)

                        Select Case dtAttribute.format
                            Case AttributeFormat.DateTime
                                sql.Append("CONVERT(datetime, @searchval, 120) ")

                            Case AttributeFormat.TimeOnly
                                sql.Append("CONVERT(time, @searchval, 120) ")
                                searchVal = dOrigDate.Hour.ToString & ":" & dOrigDate.Minute.ToString

                            Case AttributeFormat.DateOnly
                                sql.Append("CONVERT(date, @searchval, 120) ")
                                searchVal = dOrigDate.Year.ToString & "-" & dOrigDate.Month.ToString("00") & "-" & dOrigDate.Day.ToString("00")

                            Case Else
                        End Select
                    Case FieldType.List
                        If searchVal <> "0" Then
                            sql.Append("@searchval ")
                        End If
                    Case Else
                        sql.Append("@searchval ")
                End Select

                If subAccountFilter <> "" Then
                    sql.Append(subAccountFilter)
                End If

                sql.Append("ORDER BY ")
                sql.Append("[" & sortField & "]")

                Dim db As New DBConnection(cAccounts.getConnectionString(curUser.AccountID))

                Select Case cmpType
                    Case SearchType.Equals
                        db.sqlexecute.Parameters.AddWithValue("@searchval", searchVal)
                    Case SearchType.Wildcard
                        db.sqlexecute.Parameters.AddWithValue("@searchval", "%" & searchVal & "%")
                    Case Else
                End Select

                If subAccountFilter <> "" Then
                    db.sqlexecute.Parameters.AddWithValue("@subAccountId", curUser.CurrentSubAccountId)
                End If

                Dim dset As DataSet = db.GetDataSet(sql.ToString)

                ResultsPanel.Visible = True
                SearchFieldsPanel.Visible = False

                litResults.Text = SearchUF(dset, ufield, sortField, fieldAlias, parentTable, params)
            End If
        End Sub

        Private Function SearchUF(ByVal dset As DataSet, ByVal ufield As cUserDefinedField, ByVal sortField As String, ByVal fieldAlias As String, ByVal parentTable As String, ByVal params As cAccountProperties) As String
            Dim displayfields As New ArrayList
            Dim displayfieldtype As New ArrayList
            Dim displayfieldalias As New Dictionary(Of Guid, String)
            Dim idfield As String = ""
            Dim strHTML As New StringBuilder
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New DBConnection(cAccounts.getConnectionString(curUser.AccountID))
            Dim fields As New cFields(curUser.AccountID)
            Dim tables As New cTables(curUser.AccountID)
            Dim baseTableId As Guid = tables.getTableByName(parentTable).tableid
            Dim productDetails As cTable = tables.GetTableByID(new Guid("676DF92B-386A-4E39-BE7D-A54AB7D6D168"))
            Dim productLicences As cTable = tables.GetTableByID(new Guid("C952BFB5-B3F9-48E8-834F-6C609E31108B"))


            Select Case ufield.table.tablename.ToLower
                Case "userdefinedcontractdetails"
                    idfield = "ContractId"
                    displayfields.Add(idfield)
                    displayfieldtype.Add("I")
                    displayfields.Add(fields.GetBy(baseTableId, "contractKey").FieldID.ToString)
                    displayfieldtype.Add("S")
                    displayfields.Add(fields.GetBy(baseTableId, sortField).FieldID.ToString)
                    displayfieldtype.Add("S")
                    displayfields.Add("udf" & ufield.userdefineid.ToString)
                    displayfieldtype.Add("U")
                    displayfields.Add("supplierName")
                    displayfieldtype.Add("S")
                    displayfields.Add(fields.GetBy(baseTableId, "contractValue").FieldID.ToString)
                    displayfieldtype.Add("C")
                    displayfields.Add(fields.GetBy(baseTableId, "Archived").FieldID.ToString)
                    displayfieldtype.Add("X")

                Case "userdefinedcontractproductdetails"
                    idfield = "contractId"
                    displayfields.Add(idfield)
                    displayfieldtype.Add("I")
                    displayfields.Add(fields.GetBy(productDetails.TableID, sortField).FieldID.ToString)
                    displayfieldtype.Add("S")
                    displayfields.Add("udf" & ufield.userdefineid.ToString)
                    displayfieldtype.Add("U")

                Case "userdefinedproductlicences"
                    idfield = "productId"
                    displayfields.Add(idfield)
                    displayfieldtype.Add("I")
                    displayfields.Add(fields.GetBy(productDetails.TableID, sortField).FieldID.ToString)
                    displayfieldtype.Add("S")
                    displayfields.Add(fields.GetBy(productLicences.TableID, "expiry").FieldID.ToString)
                    displayfieldtype.Add("D")
                    displayfields.Add("udf" & ufield.userdefineid.ToString)
                    displayfieldtype.Add("U")

                Case "userdefinedproductdetails"
                    idfield = "productId"
                    displayfields.Add(idfield)
                    displayfieldtype.Add("I")
                    displayfields.Add(fields.GetBy(baseTableId, sortField).FieldID.ToString)
                    displayfieldtype.Add("S")
                    displayfields.Add(fields.GetBy(baseTableId, "description").FieldID.ToString)
                    displayfieldtype.Add("T")
                    displayfields.Add("udf" & ufield.userdefineid.ToString)
                    displayfieldtype.Add("U")

                Case "userdefinedrechargeassociation"
                    idfield = "contractId"
                    displayfields.Add(idfield)
                    displayfieldtype.Add("I")
                    displayfields.Add(fields.GetBy(baseTableId, sortField).FieldID.ToString)
                    displayfieldtype.Add("S")
                    displayfields.Add("udf" & ufield.userdefineid.ToString)
                    displayfieldtype.Add("U")
                Case "userdefined_employees"
                    idfield = "employeeId"
                    displayfields.Add(idfield)
                    displayfieldtype.Add("I")
                    Dim fsfield As Guid = fields.GetBy(baseTableId, "dbo.getEmployeeFullname(employees.employeeid)").FieldID
                    displayfields.Add(fsfield.ToString)
                    displayfieldtype.Add("FS")
                    displayfieldalias.Add(fsfield, "fullName")
                    displayfields.Add("udf" & ufield.userdefineid.ToString)
                    displayfieldtype.Add("U")
                Case "userdefinedsupplierdetails"
                    idfield = "supplierId"
                    displayfields.Add(idfield)
                    displayfieldtype.Add("I")
                    displayfields.Add(fields.GetBy(baseTableId, sortField).FieldID.ToString)
                    displayfieldtype.Add("S")
                    displayfields.Add("udf" & ufield.userdefineid.ToString)
                    displayfieldtype.Add("U")
                Case "userdefinedsuppliercontacts"
                    idfield = "supplierId" ' because its the supplier record we are loading and not the contactId
                    displayfields.Add(idfield)
                    displayfieldtype.Add("I")
                    displayfields.Add(fields.GetBy(baseTableId, sortField).FieldID.ToString)
                    displayfieldtype.Add("S")
                    displayfields.Add("supplierName")
                    displayfieldtype.Add("S")
                    displayfields.Add("udf" & ufield.userdefineid.ToString)
                    displayfieldtype.Add("U")
                Case Else
            End Select

            With strHTML
                .Append("<table class=""datatbl"">" & vbNewLine)
                .Append("<tr>" & vbNewLine)
                For i As Integer = 0 To displayfields.Count - 1
                    If displayfields(i) = idfield Then
                        ' add view contract / product icon
                        .Append("<th style=""width:21px;""><img src=""./icons/16/plain/view.png"" alt=""View"" /></th>" & vbNewLine)
                    Else
                        If displayfields(i) = "supplierName" Then
                            .Append("<th>" & params.SupplierPrimaryTitle & " Name</th>" & vbNewLine)
                        Else
                            If displayfieldtype(i) = "U" Then
                                .Append("<th>" & ufield.attribute.attributename & "</th>" & vbNewLine)
                            Else
                                Dim field As cField = fields.getFieldById(New Guid(CStr(displayfields(i))))
                                .Append("<th>" & field.description & "</th>" & vbNewLine)
                            End If
                        End If
                    End If
                Next

                Dim rowalt As Boolean = False
                Dim rowclass As String = "row1"

                .Append("</tr>" & vbNewLine)
                If dset.Tables(0).Rows.Count > 0 Then
                    For Each drow As DataRow In dset.Tables(0).Rows
                        rowalt = rowalt Xor True
                        If rowalt Then
                            rowclass = "row1"
                        Else
                            rowclass = "row2"
                        End If
                        .Append("<tr>" & vbNewLine)
                        For i As Integer = 0 To displayfields.Count - 1
                            .Append("<td class=""" & rowclass & """>")

                            If displayfields(i) = idfield Then
                                ' add view contract / product icon
                                Select Case ufield.table.tablename.ToLower
                                    Case "userdefinedcontractdetails"
                                        .Append("<a href=""ContractSummary.aspx?tab=" & IIf(ufield.Grouping Is Nothing, SummaryTabs.ContractDetail, SummaryTabs.ContractAdditional) & "&id=" & CStr(drow(idfield)) & """ onmouseover=""window.status='View Record';return true;"" onmouseout=""window.status='Done';"">")
                                    Case "userdefinedcontractproductdetails"
                                        .Append("<a href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractProduct & "&id=" & CStr(drow(idfield)) & """ onmouseover=""window.status='View Record';return true;"" onmouseout=""window.status='Done';"">")
                                    Case "userdefinedproductlicences"
                                        .Append("<a href=""contracts/ProductLicences.aspx?pid=" & CStr(drow(idfield)) & """ onmouseover=""window.status='View Record';return true;"" onmouseout=""window.status='Done';"">")
                                    Case "userdefinedproductdetails"
                                        .Append("<a href=""ProductDetails.aspx?ret=2&action=edit&id=" & CStr(drow(idfield)) & """ onmouseover=""window.status='View Record';return true;"" onmouseout=""window.status='Done';"">")
                                    Case "userdefinedrechargeassociations"
                                        .Append("<a href=""SummaryPortal.aspx"" onmouseover=""window.status='Option not currently available';return true;"" onmouseout=""window.status='Done';"">")
                                    Case "userdefined_employees"
                                        .Append("<a href=""shared/admin/aeemployee.aspx?employeeid=" & CStr(drow(idfield)) & """ onmouseover=""window.status='View Record';return true;"" onmouseout=""window.status='Done';"">")
                                    Case "userdefinedsupplierdetails", "userdefinedsuppliercontacts"
                                        .Append("<a href=""shared/supplier_details.aspx?redir=2&sid=" & CStr(drow(idfield)) & IIf(ufield.table.TableName.ToLower = "userdefinedsuppliercontacts", "&t=1", "&t=0") & """ onmouseover=""window.status='View Record';return true;"" onmouseout=""window.status='Done';"">")
                                End Select
                                .Append("<img src=""./icons/16/plain/view.png"" alt=""View"" />")
                                .Append("</a>" & vbNewLine)
                            Else
                                Dim field As cField
                                If CStr(displayfieldtype(i)) <> "U" Then
                                    If CStr(displayfields(i)) = "supplierName" Then
                                        field = fields.getFieldById(New Guid("3F834EE1-C272-4C42-902B-F37B1FC95DD9"))
                                    Else
                                        field = fields.getFieldById(New Guid(CStr(displayfields(i))))
                                    End If
                                Else
                                    field = Nothing
                                End If

                                Select Case CStr(displayfieldtype(i))
                                    Case "S", "F", "N"
                                        .Append(drow(field.FieldName))
                                    Case "FS"
                                        If displayfieldalias.ContainsKey(field.fieldid) Then
                                            .Append(drow(displayfieldalias(field.fieldid)))
                                        Else
                                            .Append("unknown field")
                                        End If
                                    Case "D"
                                        Dim dateStr As String = IIf(IsDBNull(drow(field.FieldName)), "", drow(field.FieldName))
                                        If IsDate(dateStr) Then
                                            .Append(Format(CDate(dateStr), cDef.DATE_FORMAT))
                                        End If

                                    Case "T"
                                        .Append("<textbox>" & vbNewLine)
                                        .Append(drow(field.FieldName))
                                        .Append("</textbox>" & vbNewLine)
                                    Case "X"
                                        .Append("<input type=""checkbox"" disabled ")
                                        If drow(field.FieldName) = "1" Then
                                            .Append("checked ")
                                        End If
                                        .Append(" />" & vbNewLine)
                                    Case "C"
                                        If Not IsDBNull(drow(field.FieldName)) And IsNumeric(drow(field.FieldName)) Then
                                            .Append(Format(drow(field.FieldName), "#,###,##0.00"))
                                        End If

                                    Case "U"
                                        ' the user defined field so need to process accordingly
                                        Select Case ufield.fieldtype
                                            Case FieldType.Text, FieldType.Currency, FieldType.Number, FieldType.Integer, FieldType.DynamicHyperlink, FieldType.Hyperlink
                                                .Append(drow(displayfields(i)))
                                            Case FieldType.List
                                                Dim lstVal As String = ufield.items(drow(displayfields(i))).elementText
                                                .Append(lstVal)
                                            Case FieldType.DateTime
                                                Dim dateStr As String = drow(displayfields(i))
                                                If IsDate(dateStr) Then
                                                    .Append(Format(CDate(dateStr), cDef.DATE_FORMAT))
                                                End If
                                            Case FieldType.TickBox
                                                .Append("<input type=""checkbox"" disabled ")
                                                If drow(displayfields(i)) = "1" Then
                                                    .Append("checked ")
                                                End If
                                                .Append(" />" & vbNewLine)
                                                'Case UserFieldType.RechargeAcc_Code
                                                '    Dim racs As New cRechargeAccountCodes(uinfo, fws)
                                                '    Dim rac As cRechargeAccountCode = racs.GetCodeById(drow(displayfields(i)))
                                                '    If Not rac Is Nothing Then
                                                '        .Append(rac.AccountCode)
                                                '    End If
                                                'Case UserFieldType.RechargeClient_Ref
                                                '    Dim rc As New cRechargeClientList(uinfo, fws)
                                                '    Dim rclient As cRechargeClient = rc.GetClientById(drow(displayfields(i)))
                                                '    If Not rclient Is Nothing Then
                                                '        .Append(rclient.ClientName)
                                                '    End If
                                                'Case UserFieldType.Site_Ref
                                                '    Dim sites As New cSites(fws, uinfo)
                                                '    Dim site As cSite = sites.GetSiteById(drow(displayfields(i)))
                                                '    If Not site Is Nothing Then
                                                '        .Append(site.SiteCode & ":" & site.SiteDescription)
                                                '    End If
                                                'Case UserFieldType.StaffName_Ref
                                                '    Dim emps As New cFWEmployees(fws, uinfo)
                                                '    Dim emp As cFWEmployee = emps.GetEmployeeById(drow(displayfields(i)))
                                                '    If Not emp Is Nothing Then
                                                '        .Append(emp.EmployeeName)
                                                '    End If
                                            Case FieldType.LargeText
                                                .Append("<textbox>" & vbNewLine)
                                                .Append(drow(displayfields(i)))
                                                .Append("</textbox>" & vbNewLine)
                                                'Case FieldType.RelationshipTextbox
                                                '    Dim tmpCntlID As String = "rtbUF" & ufield.attribute.attributeid.ToString
                                                '    Dim relTxt As relationshipTextbox = CType(igUFSearchPanel.FindControl(tmpCntlID), relationshipTextbox)
                                                '    If Not relTxt Is Nothing Then
                                                '        .Append(relTxt.TextBox.Text)
                                                '    End If
                                        End Select
                                    Case Else
                                        .Append("Unknown field type")
                                End Select
                            End If
                            .Append("</td>" & vbNewLine)
                        Next
                        .Append("</tr>" & vbNewLine)
                    Next
                Else
                    .Append("<tr>" & vbNewLine)
                    .Append("<td class=""row1"" colspan=""" & displayfields.Count.ToString & """ align=""center"">")
                    .Append("No results returned for the requested search")
                    .Append("</td>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                End If
                .Append("</table>" & vbNewLine)
            End With

            Return strHTML.ToString
        End Function

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            Response.Redirect("Home.aspx", True)
        End Sub

        Protected Sub cmdSearchContractType_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdSearchContractType.Click
            If lstContractType.SelectedItem.Value <> "0" Then
                SearchCriteria = "[contract_details].[ContractTypeId] = " & Trim(lstContractType.SelectedItem.Value)

                If Not rdoContractStatus.SelectedItem Is Nothing Then
                    If rdoContractStatus.SelectedItem.Value <> "B" Then
                        SearchCriteria += " AND [contract_details].[Archived] = '" & Trim(rdoContractStatus.SelectedItem.Value) & "'"
                    End If

                    If lstContractCategory.SelectedItem.Value <> 0 Then
                        SearchCriteria += " AND [contract_details].[CategoryId] = " & lstContractCategory.SelectedItem.Value.ToString
                    End If
                End If

                Dim db As New cFWDBConnection
                Dim curUser As CurrentUser = cMisc.GetCurrentUser()
                Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

                db.DBOpen(fws, False)
                litResults.Text = GetContracts(curUser, New Dictionary(Of String, Object), chkIncludeVariations.Checked, SearchCriteria)
                db.DBClose()
                db = Nothing
                SearchFieldsPanel.Visible = False
                ResultsPanel.Visible = True
            Else
                litResults.Text = ""
            End If
        End Sub

        Protected Sub cmdSearchContractCategory_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdSearchContractCategory.Click
            Dim retStr As String = ""
            If lstContractCategory.SelectedItem.Value <> "0" Then
                SearchCriteria = "[contract_details].[CategoryId] = " & Trim(lstContractCategory.SelectedItem.Value)
                If Not rdoContractStatus.SelectedItem Is Nothing Then
                    If rdoContractStatus.SelectedItem.Value <> "B" Then
                        SearchCriteria += " AND [contract_details].[Archived] = '" & Trim(rdoContractStatus.SelectedItem.Value) & "'"
                    End If
                End If

                If lstContractType.SelectedItem.Value <> 0 Then
                    SearchCriteria += " AND [contract_details].[ContractTypeId] = " & lstContractType.SelectedItem.Value.ToString
                End If

                Dim db As New cFWDBConnection
                Dim curUser As CurrentUser = cMisc.GetCurrentUser()
                Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

                db.DBOpen(fws, False)

                litResults.Text = GetContracts(curUser, New Dictionary(Of String, Object), chkIncludeVariations.Checked, SearchCriteria.ToString)

                SearchFieldsPanel.Visible = False
                ResultsPanel.Visible = True

                db.DBClose()
                db = Nothing
            Else
                litResults.Text = ""
            End If
        End Sub

        Private Sub SetPermissions()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, False) = False Then
                lstContractCategory.Enabled = False
                cmdSearchContractCategory.Visible = False
                lstContractType.Enabled = False
                cmdSearchContractType.Visible = False
            End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, False) = False Then
                txtInvoiceNo.Enabled = False
                cmdInvNo.Visible = False
                txtPONumber.Enabled = False
                cmdPONumber.Visible = False
            End If

            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LinkedContracts, False) = False Then
                lstLinkDefs.Enabled = False
            End If
        End Sub
    End Class

End Namespace
