Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class LinkedContracts
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
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection
            Dim contractId As Integer
			Dim action, tmpStr As String

			action = Request.QueryString("action")
			If action = "print" Then
				Session("HideMenu") = 1
			Else
				Session("HideMenu") = Nothing
			End If

			contractId = CInt(Request.QueryString("cid"))

            If Me.IsPostBack = False Then
                curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractLinks, False, True)

                db.DBOpen(fws, False)

                db.FWDb("R2", "contract_details", "ContractId", contractId, "", "", "", "", "", "", "", "", "", "")
                If db.FWDb2Flag = True Then
                    lblTitle.Text = "Contracts Linked to " & db.FWDbFindVal("ContractDescription", 2).Trim
                Else
                    lblTitle.Text = "Contracts Links"
                End If

                Title = lblTitle.Text
                Master.title = Title

                If contractId > 0 Then
                    litLinkData.Text = GetContractLinks(db, contractId)
                Else
                    litLinkData.Text = ""
                End If

                db.DBClose()
            End If

			'If action <> "print" Then

			lnkPrint.ToolTip = "Open this window in a printer friendly format"
			lnkPrint.Attributes.Add("onmouseover", "window.status='Open this window in a printer friendly format';return true;")
			lnkPrint.Attributes.Add("onmouseout", "window.status='Done';")
			'tmpStr = "window.open('LinkedContracts.aspx?action=print&cid=" & Trim(Str(contractId)) & "');"
			tmpStr = "doPrint();"
			lnkPrint.Attributes.Add("onclick", tmpStr)
			'Else
			'lnkPrint.Visible = False

			'End If

			db = Nothing
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
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim baseCurrency As cCurrency = Nothing
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            If params.BaseCurrency.HasValue Then
                baseCurrency = currency.getCurrencyById(params.BaseCurrency.Value)
            End If

            sql.Append("SELECT [link_matrix].[LinkId],[link_definitions].[LinkDefinition] FROM [link_matrix] ")
            sql.Append("LEFT JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] ")
            sql.Append("WHERE [link_matrix].[ContractId] = @conId AND [IsScheduleLink] = 0 ")
            sql.Append("ORDER BY [LinkDefinition]")
            db.AddDBParam("conId", cId, True)
            db.RunSQL(sql.ToString, db.glDBWorkB, False, "", False)

            tmpStr.Append("<table class=""datatbl"">" & vbNewLine)

            ' header row
            headerHTML.Append("<tr>" & vbNewLine)
            headerHTML.Append("<td width=""50"">&nbsp;</td>" & vbNewLine)
            headerHTML.Append("<th class=""datatbl"" width=""100"">Contract Key</th>" & vbNewLine)
            headerHTML.Append("<th class=""datatbl"" width=""180"">Contract Description</th>" & vbNewLine)
            headerHTML.Append("<th class=""datatbl"" width=""100"">Contract Value</th>" & vbNewLine)
            headerHTML.Append("<th class=""datatbl"" width=""180"">Contract Supplier</th>" & vbNewLine)
            headerHTML.Append("<th class=""datatbl"" width=""100"">Is Contract Archived?</th>" & vbNewLine)
            headerHTML.Append("</tr>" & vbNewLine)

            rowClass = "row1"
            hasRows = False

            For Each drow_link In db.glDBWorkB.Tables(0).Rows
                tmpStr.Append("<tr><td colspan=""6"">&nbsp;</td></tr>" & vbNewLine)
                tmpStr.Append("<tr><td colspan=""6""><h2><b>Link Definition: </b>" & drow_link.Item("LinkDefinition") & "</h2></td></tr>" & vbNewLine)
                'tmpStr += "<tr><td class=""main"" colspan=""6"">&nbsp;</td></tr>" & vbNewLine
                tmpStr.Append(headerHTML)
                linkDef = drow_link.Item("Link Definition")

                sql = New System.Text.StringBuilder
                sql.Append("SELECT [contract_details].[ContractId],[ContractKey],[ContractDescription],[ContractValue],[Archived],[ContractCurrency],[contract_details].[supplierId],[supplier_details].[supplierName]")
                sql.Append(" FROM [contract_details] ")
                sql.Append("LEFT JOIN [link_matrix] ON [link_matrix].[contractId] = [contract_details].[contractId] ")
                sql.Append("LEFT JOIN [link_definitions] ON [link_definitions].[LinkId] = [link_matrix].[LinkId] ")
                sql.Append("LEFT JOIN [supplier_details] ON [contract_details].[supplierId] = [supplier_details].[supplierId] ")
                sql.Append("WHERE [link_matrix].[LinkId] = @lnkId")
                sql.Append(" AND [contract_details].[subAccountId] = @subAccId")
                sql.Append(" ORDER BY [ContractDescription]")
                db.AddDBParam("lnkId", drow_link.Item("linkId"), True)
                db.AddDBParam("subAccId", curUser.CurrentSubAccountId, False)
                db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    rowalt = (rowalt Xor True)

                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    tmpStr.Append("<tr height=""30"">" & vbNewLine)

                    If Val(drow.Item("contractId")) = cId Then
                        tmpStr.Append("<td align=""center""><img src=""./images/arrow_right.gif"" /></td>" & vbNewLine)
                    Else
                        tmpStr.Append("<td>&nbsp;</td>" & vbNewLine)
                    End If

                    If IsDBNull(drow.Item("Contract Key")) = True Then
                        tmpData.Append("")
                    Else
                        tmpData.Append("<a onmouseover=""window.status='Open contract';return true;"" onmouseout=""window.status='Done';"" href=""redirect.htm?ContractSummary.aspx?id=" & Trim(Str(drow.Item("contractId"))) & """>")
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
                        tmpData.Append("<a onmouseover=""window.status='Open contract';return true;"" onmouseout=""window.status='Done';"" href=""redirect.htm?ContractSummary.aspx?id=" & Trim(Str(drow.Item("contractId"))) & """>")
                        tmpData.Append(Trim(drow.Item("ContractDescription")))
                        tmpData.Append("</a>")
                    End If
                    tmpStr.Append("<td class=""" & rowClass & """>")
                    tmpStr.Append(tmpData)
                    tmpData = New System.Text.StringBuilder
                    tmpStr.Append("</td>" & vbNewLine)

                    If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) = True Then
                        If IsDBNull(drow.Item("ContractValue")) = True Then
                            tmpData.Append("")
                        Else
                            If IsDBNull(drow.Item("ContractCurrency")) = True Then
                                If baseCurrency Is Nothing Then
                                    tmpData.Append(CStr(drow.Item("ContractValue")))
                                Else
                                    tmpData.Append(currency.FormatCurrency(drow.Item("ContractValue"), baseCurrency, False))
                                End If

                            Else
                                Dim curCurrency As cCurrency = currency.getCurrencyById(drow.Item("ContractCurrency"))
                                tmpData.Append(currency.FormatCurrency(drow.Item("ContractValue"), curCurrency, False))
                            End If
                        End If
                    Else
                        tmpData.Append("n/a")
                    End If
                    tmpStr.Append("<td class=""" & rowClass & """>")
                    tmpStr.Append(tmpData)
                    tmpData = New System.Text.StringBuilder
                    tmpStr.Append("</td>" & vbNewLine)

                    If IsDBNull(drow.Item("supplierName")) = True Then
                        tmpData.Append("")
                    Else
                        tmpData.Append("<a onmouseover=""window.status='Open supplier record';return true;"" onmouseout=""window.status='Done';"" href=""/shared/supplier_details.aspx?sid=" & Trim(Str(drow.Item("supplierId"))) & "&tab=" & Trim(Left(drow.Item("supplierName"), 1)) & """>")
                        tmpData.Append(Trim(drow.Item("supplierName")))
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

                'If hasRows = False Then
                '    tmpStr += "<tr><td class=""data"" align=""center"" colspan=""6"">No contract links returned</td></tr>" & vbNewLine
                'End If
            Next

            tmpStr.Append("</table>")

            GetContractLinks = tmpStr.ToString
        End Function
    End Class
End Namespace
