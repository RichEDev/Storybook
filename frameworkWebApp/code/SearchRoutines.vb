Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports SpendManagementLibrary
Imports FWBase
Imports FWClasses
Imports System.Data
Imports Spend_Management

Namespace Framework2006
    Public Module SearchRoutines
        Public Function GetContracts(ByVal curUser As CurrentUser, ByVal params As Dictionary(Of String, Object), ByVal showVariations As Boolean, Optional ByVal SearchCriteria As String = "") As String
            Dim sql As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim tmpStr, rowClass As String
            Dim strHTML As New System.Text.StringBuilder
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim accProperties As cAccountProperties = IIf(curUser.CurrentSubAccountId, subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties, subaccs.getFirstSubAccount().SubAccountProperties)

            Dim supplierStr As String = accProperties.SupplierPrimaryTitle

            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)

            sql.Append("SELECT [contract_details].[ContractId],[ContractKey],[ContractDescription],[ContractNumber],[suppliername],[EndDate],[contract_details].[Archived],ISNULL([ContractCurrency],0) AS [ContractCurrency]")
            If accProperties.ShowProductInSearch Then
                sql.Append(",[ProductName],ISNULL([contract_productdetails].[MaintenanceValue],0) AS [MaintenanceValue]")
                If curUser.Account.companyname = "Software Europe" Then
                    ' software europe only (for Rita!!)
                    sql.Append(",[contract_productdetails].[Quantity],[codes_units].[description] ")
                End If
            End If

            If showVariations Then
                sql.Append(",dbo.IsVariation([contract_details].[ContractId]) AS [IsVariation]")
            End If

            sql.Append(" FROM [contract_details] ")
            sql.Append("LEFT JOIN [supplier_details] ON [supplier_details].[supplierid] = [contract_details].[supplierId] " & vbNewLine)
            If accProperties.ShowProductInSearch Then
                sql.Append("LEFT JOIN [contract_productdetails] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId] " & vbNewLine)
                sql.Append("LEFT JOIN [productDetails] ON [contract_productdetails].[ProductId] = [productDetails].[ProductId] " & vbNewLine)
                If curUser.Account.companyname = "Software Europe" Then
                    sql.Append("LEFT JOIN [codes_units] ON [codes_units].[unitId] = [contract_productdetails].[UnitId] " & vbNewLine)
                End If
            End If

            sql.Append(" WHERE [contract_details].[subAccountId] = @locId " & vbNewLine)
            sql.Append("AND dbo.CheckContractAccess(@userId,[contract_details].[ContractId], @locId) > 0 ")
            If showVariations = False Then
                sql.Append("AND dbo.IsVariation([contract_details].[ContractId]) = 0 ")
            End If

            If SearchCriteria <> "" Then
                sql.Append(" AND " & SearchCriteria)
            End If
            sql.Append(" ORDER BY [suppliername],[ContractDescription]")

            Dim db As New DBConnection(SpendManagementLibrary.cAccounts.getConnectionString(curUser.Account.accountid))

            db.sqlexecute.Parameters.AddWithValue("@locId", curUser.CurrentSubAccountId)
            db.sqlexecute.Parameters.AddWithValue("@userId", curUser.Employee.EmployeeID)

            For Each p As KeyValuePair(Of String, Object) In params
                db.sqlexecute.Parameters.AddWithValue(p.Key, p.Value)
            Next

            Dim dset As New DataSet
            dset = db.GetDataSet(sql.ToString)

            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th>Contract Key</th>" & vbNewLine)
            strHTML.Append("<th>Contract Number</th>" & vbNewLine)
            strHTML.Append("<th>Contract Description</th>" & vbNewLine)
            strHTML.Append("<th>" & supplierStr & "</th>" & vbNewLine)
            If accProperties.ShowProductInSearch Then
                strHTML.Append("<th>Product Name</th>" & vbNewLine)
                strHTML.Append("<th>Annual Cost</th>" & vbNewLine)

                If curUser.Account.companyname = "Software Europe" Then
                    strHTML.Append("<th>Quantity</th>" & vbNewLine)
                    strHTML.Append("<th>Units</th>" & vbNewLine)
                End If
            End If
            strHTML.Append("<th>End / Renewal Date</th>" & vbNewLine)
            strHTML.Append("<th>Archived?</th>" & vbNewLine)
            If showVariations Then
                strHTML.Append("<th>Variation?</th>" & vbNewLine)
            End If
            strHTML.Append("</tr>" & vbNewLine)

            Dim archivedVal As String = ""
            Dim rowalt As Boolean = False

            If dset.Tables(0).Rows.Count > 0 Then
                For Each drow In dset.Tables(0).Rows
                    rowalt = (rowalt Xor True)

                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    If drow.Item("Archived") = "Y" Then
                        archivedVal = " checked "
                    Else
                        archivedVal = ""
                    End If

                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """><a href=""ContractSummary.aspx?id=" & drow.Item("ContractId") & """ onmouseover=""window.status='Access contract record';return true;"" onmouseout=""window.status='Done';"">" & drow.Item("ContractKey") & "</a></td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("ContractNumber") & "</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """><a href=""ContractSummary.aspx?id=" & drow.Item("ContractId") & """ onmouseover=""window.status='Access contract record';return true;"" onmouseout=""window.status='Done';"">" & drow.Item("ContractDescription") & "</a></td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("suppliername") & "</td>" & vbNewLine)
                    If accProperties.ShowProductInSearch Then
                        strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("ProductName") & "</td>" & vbNewLine)

                        If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                            strHTML.Append("<td class=""" & rowClass & """>" & currency.FormatCurrency(drow.Item("MaintenanceValue"), currency.getCurrencyById(drow.Item("ContractCurrency")), False) & "</td>" & vbNewLine)
                        Else
                            strHTML.Append("<td class=""" & rowClass & """>n/a</td>" & vbNewLine)
                        End If

                        If curUser.Account.companyname = "Software Europe" Then
                            strHTML.Append("<td class=""" & rowClass & """>")
                            If IsDBNull(drow.Item("Quantity")) = False Then
                                strHTML.Append(drow.Item("Quantity"))
                            End If
                            strHTML.Append("</td>" & vbNewLine)
                            strHTML.Append("<td class=""" & rowClass & """>")
                            If IsDBNull(drow.Item("description")) = False Then
                                strHTML.Append(drow.Item("description"))
                            End If
                            strHTML.Append("</td>" & vbNewLine)
                        End If
                    End If

                    If IsDBNull(drow.Item("EndDate")) = True Then
                        tmpStr = "&nbsp;"
                    Else
                        tmpStr = Format(CDate(drow.Item("EndDate")), cDef.DATE_FORMAT)
                    End If

                    strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=""checkbox"" disabled " & archivedVal & " /></td>" & vbNewLine)
                    If showVariations Then
                        strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=""checkbox"" disabled " & IIf(drow.Item("IsVariation") = "1", "checked ", "") & " /></td>" & vbNewLine)
                    End If
                    strHTML.Append("</tr>")
                Next
            Else
                Dim tmpColSpan As String
                Dim showVariationsOffset = IIf(showVariations = True, 1, 0)

                If accProperties.ShowProductInSearch Then
                    If curUser.Account.companyname = "Software Europe" Then
                        tmpColSpan = "10" + showVariationsOffset
                    Else
                        tmpColSpan = "8" + showVariationsOffset
                    End If
                Else
                    tmpColSpan = "6" + showVariationsOffset
                End If
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""row1"" colspan=""" & tmpColSpan & """>No results returned</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
            End If
            strHTML.Append("</table>")

            Return strHTML.ToString
        End Function

        Public Function DescriptionSearch(ByVal curUser As CurrentUser, ByVal searchVal As String, ByVal searchStatus As String, ByVal includeVariations As String) As String
            Dim searchCriteria As New System.Text.StringBuilder
            Dim retStr As String
            Dim showVariations As Boolean = False
            Dim params As New Dictionary(Of String, Object)

            searchCriteria.Append("LOWER([ContractDescription]) LIKE LOWER('%' + @searchval + '%')")
            If searchStatus.Trim <> "B" Then
                searchCriteria.Append(" AND [contract_details].[Archived] = '" & searchStatus.Trim & "'")
            End If

            params.Add("@searchval", searchVal)
            If includeVariations <> "1" Then
                searchCriteria.Append(" AND dbo.IsVariation([contract_details].[ContractId]) = @isVariation")
                params.Add("@isVariation", 0)
            Else
                showVariations = True
            End If

            retStr = GetContracts(curUser, params, showVariations, searchCriteria.ToString)

            Return retStr
        End Function

        Public Function VendorDefSearch(ByVal curUser As CurrentUser, ByVal searchVal As String, ByVal searchStatus As String) As String
            Dim sql As New System.Text.StringBuilder
            Dim db As New cFWDBConnection
            Dim strHTML As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties
            If curUser.CurrentSubAccountId >= 0 Then
                params = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Else
                params = subaccs.getFirstSubAccount().SubAccountProperties
            End If

            Dim catTitle As String = "Supplier Category"
            Dim supplierStr As String = "Supplier"

            db.DBOpen(fws, False)


            catTitle = params.SupplierCatTitle
            supplierStr = params.SupplierPrimaryTitle

            sql.Append("SELECT [supplierid],[suppliername],ISNULL([supplier_categories].[description],'') AS [catTitle] FROM [supplier_details] ")
            sql.Append("LEFT JOIN [supplier_categories] ON [supplier_categories].[categoryid] = [supplier_details].[categoryid] ")
            sql.Append("WHERE LOWER([suppliername]) LIKE LOWER(@searchval) AND [supplier_details].[subAccountId] = @locId ORDER BY [suppliername]")
            db.AddDBParam("searchval", "%" & searchVal.Trim & "%", True)
            db.AddDBParam("locId", curUser.CurrentSubAccountId, False)
            db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)

            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th>" & supplierStr & " Name</th>" & vbNewLine)
            strHTML.Append("<th>" & catTitle & "</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            Dim rowalt As Boolean = False
            Dim rowClass As String

            If db.GetRowCount(db.glDBWorkD, 0) > 0 Then
                For Each drow In db.glDBWorkD.Tables(0).Rows
                    rowalt = (rowalt Xor True)

                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """><a onmouseout=""window.status='Done';"" onmouseover=""window.status='Access " & supplierStr & " record';return true;"" href=""" & cMisc.path & "/shared/supplier_details.aspx?sid=" & drow.Item("supplierId") & """>" & drow.Item("supplierName") & "</a></td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("catTitle") & "</td>" & vbNewLine)
                    strHTML.Append("</tr>")
                Next
            Else
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""row1"" align=""center"" colspan=""2"">No results returned</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
            End If
            strHTML.Append("</table>")

            db.DBClose()
            db = Nothing

            Return strHTML.ToString
        End Function

        Public Function ContractCategorySearch(ByVal curUser As CurrentUser, ByVal searchVal As String, ByVal searchStatus As String, ByVal includeVariations As String) As String
            Dim searchCriteria As New System.Text.StringBuilder
            Dim retStr As String = ""
            Dim showVariations As Boolean = False
            Dim params As New Dictionary(Of String, Object)

            If searchVal <> "0" Then
                searchCriteria.Append("[contract_details].[CategoryId] = " & searchVal.Trim)
                If searchStatus <> "B" Then
                    searchCriteria.Append(" AND [contract_details].[Archived] = @archived")
                    params.Add("@archived", searchStatus)
                End If

                If includeVariations <> "1" Then
                    searchCriteria.Append(" AND dbo.IsVariation([contract_details].[ContractId]) = @isVariation")
                    params.Add("@isVariation", 0)
                Else
                    showVariations = True
                End If

                retStr = GetContracts(curUser, params, showVariations, searchCriteria.ToString)
            Else
                retStr = ""
            End If

            Return retStr
        End Function

        Public Function ContractNoSearch(ByVal curUser As CurrentUser, ByVal searchVal As String, ByVal searchStatus As String, ByVal includeVariations As String) As String
            Dim searchCriteria As New System.Text.StringBuilder
            Dim retStr As String
            Dim showVariations As Boolean = False
            Dim params As New Dictionary(Of String, Object)

            searchCriteria.Append("LOWER([ContractNumber]) LIKE LOWER(@searchval)")
            If searchStatus.Trim <> "B" Then
                searchCriteria.Append(" AND [contract_details].[Archived] = @archived")
                params.Add("@archived", searchStatus)
            End If

            params.Add("@searchval", "%" & searchVal.Trim & "%")
            If includeVariations <> "1" Then
                searchCriteria.Append(" AND dbo.IsVariation([contract_details].[ContractId]) = @isVariation")
                params.Add("@isVariation", 0)
            Else
                showVariations = True
            End If

            retStr = GetContracts(curUser, params, showVariations, searchCriteria.ToString)

            Return retStr
        End Function

        Public Function ProductSearch(ByVal curUser As CurrentUser, ByVal searchVal As String, ByVal searchStatus As String, ByVal includeVariations As String) As String
            Dim sql As System.Text.StringBuilder
            Dim tmpstr, rowClass As String
            Dim drow_product, drow_contracts As DataRow
            Dim strHTML As New System.Text.StringBuilder
            Dim showVariations As Boolean = False
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim properties As cAccountProperties = IIf(curUser.CurrentSubAccountId, subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties, subaccs.getFirstSubAccount().SubAccountProperties)
            Dim supplierStr As String = properties.SupplierPrimaryTitle

            Dim currency As New cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim db As New DBConnection(SpendManagementLibrary.cAccounts.getConnectionString(curUser.Account.accountid))

            sql = New System.Text.StringBuilder
            sql.Append("SELECT [ProductId],[ProductName] FROM [productDetails] WHERE [subAccountId] = @subAccountId  AND LOWER([ProductName]) LIKE LOWER(@Product_Name) ORDER BY [ProductName]")
            db.sqlexecute.Parameters.AddWithValue("@subAccountId", curUser.CurrentSubAccountId)
            db.sqlexecute.Parameters.AddWithValue("@Product_Name", "%" & searchVal.Trim & "%")
            Dim dset As DataSet = db.GetDataSet(sql.ToString)

            strHTML.Append("<table>" & vbNewLine)
            If dset.Tables(0).Rows.Count > 0 Then
                For Each drow_product In dset.Tables(0).Rows
                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<td align=""center""><a onmouseover=""window.status='Access product record';return true;"" onmouseout=""window.status='Done';"" onclick=""javascript:toggle('" & Trim(drow_product("ProductId")) & "');""><img id=""img" & Trim(drow_product("ProductId")) & """ src=""./buttons/open.gif"" /></a></td>")
                    strHTML.Append("<td align=""left""><b>" & Trim(drow_product.Item("ProductName")) & "</b></td>" & vbNewLine)
                    strHTML.Append("</tr><tr><td colspan=""2"">" & vbNewLine)

                    sql = New System.Text.StringBuilder
                    sql.Append("SELECT [contract_details].[ContractId],[ContractKey],[ContractNumber],[ContractDescription],[ContractValue],[supplier_details].[supplierName],ISNULL([contract_details].[ContractCurrency],0) AS [CurrencyId],[contract_details].[Archived]")
                    If includeVariations = "1" Then
                        sql.Append(",dbo.IsVariation([contract_details].[ContractId]) AS [IsVariation]")
                    End If
                    'sql.Append(",[Contract - Product Details].[Product Id], 
                    sql.Append(" FROM [contract_details] " & vbNewLine)
                    sql.Append("LEFT OUTER JOIN [contract_productdetails] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId] " & vbNewLine)
                    sql.Append("INNER JOIN [supplier_details] ON [contract_details].[supplierId] = [supplier_details].[supplierid] " & vbNewLine)
                    sql.Append("WHERE ")
                    ' removed now we have live, archived, both radio buttons
                    ' [Archived] = 'N' AND
                    Select Case searchStatus
                        Case "N"
                            sql.Append("[contract_details].[Archived] = 'N' AND ")
                        Case "Y"
                            sql.Append("[contract_details].[Archived] = 'Y' AND ")
                        Case Else

                    End Select
                    sql.Append("[contract_details].[subAccountId] = @subAccountId AND [contract_productdetails].[ProductId] = @Product_Id")
                    sql.Append(" AND dbo.CheckContractAccess(@userId,[contract_details].[ContractId], @subAccountId) > 0 ")

                    db.sqlexecute.Parameters.Clear()
                    db.sqlexecute.Parameters.AddWithValue("@Product_Id", CStr(drow_product.Item("ProductId")).Trim)
                    db.sqlexecute.Parameters.AddWithValue("@subAccountId", curUser.CurrentSubAccountId)
                    db.sqlexecute.Parameters.AddWithValue("@userId", curUser.Employee.employeeid)
                    If includeVariations <> "1" Then
                        sql.Append(" AND dbo.IsVariation([contract_details].[ContractId]) = @isVariation")
                        db.sqlexecute.Parameters.AddWithValue("@isVariation", 0)
                    Else
                        showVariations = True
                    End If
                    Dim dset_contracts = db.GetDataSet(sql.ToString)

                    strHTML.Append("<div id=""" & Trim(drow_product("ProductId")) & """ style=""display: none; cursor: hand;"">" & vbNewLine)
                    strHTML.Append("<table class=""datatbl"">" & vbNewLine)

                    ' heading
                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<th>Contract Key</th>" & vbNewLine)
                    strHTML.Append("<th>Contract Number</th>" & vbNewLine)
                    strHTML.Append("<th>Contract Desciption</th>" & vbNewLine)
                    strHTML.Append("<th>Contract Value</th>" & vbNewLine)
                    strHTML.Append("<th>" & supplierStr & " Name</th>" & vbNewLine)
                    strHTML.Append("<th>Archived?</th>" & vbNewLine)
                    If includeVariations = "1" Then
                        strHTML.Append("<th>Variation?</th>" & vbNewLine)
                    End If
                    strHTML.Append("</tr>" & vbNewLine)

                    Dim rowalt As Boolean = False
                    Dim archivedVal As String = ""

                    If dset_contracts.Tables(0).Rows.Count > 0 Then
                        For Each drow_contracts In dset_contracts.Tables(0).Rows
                            rowalt = (rowalt Xor True)

                            If rowalt = True Then
                                rowClass = "row1"
                            Else
                                rowClass = "row2"
                            End If

                            If drow_contracts.Item("Archived") = "Y" Then
                                archivedVal = " checked "
                            Else
                                archivedVal = ""
                            End If

                            strHTML.Append("<tr>" & vbNewLine)
                            strHTML.Append("<td class=""" & rowClass & """><a onmouseout=""window.status='Done';"" onmouseover=""window.status='Access contract record';return true;"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & drow_contracts("ContractId") & """>" & drow_contracts.Item("ContractKey") & "</a></td>" & vbNewLine)
                            If IsDBNull(drow_contracts.Item("ContractNumber")) = True Then
                                tmpstr = "&nbsp;"
                            Else
                                tmpstr = Trim(drow_contracts.Item("ContractNumber"))
                            End If
                            strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)
                            strHTML.Append("<td class=""" & rowClass & """><a onmouseover=""window.status='Access contract record';return true;"" onmouseout=""window.status='Done';"" href=""ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & Trim(drow_contracts("ContractId")) & """>" & Trim(drow_contracts.Item("ContractDescription")) & "</a></td>" & vbNewLine)
                            If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, True) Then
                                If IsDBNull(drow_contracts.Item("ContractValue")) = True Then
                                    tmpstr = "nbsp;"
                                Else
                                    If drow_contracts("CurrencyId") = "0" Then
                                        tmpstr = drow_contracts("ContractValue")
                                    Else
                                        Dim reqCurrency As cCurrency
                                        reqCurrency = currency.getCurrencyById(drow_contracts.Item("CurrencyId"))
                                        If reqCurrency Is Nothing Then
                                            tmpstr = "&nbsp;"
                                        Else
                                            tmpstr = currency.FormatCurrency(drow_contracts.Item("ContractValue"), reqCurrency, False)
                                        End If


                                    End If
                                End If
                            Else
                                tmpstr = "n/a"
                            End If
                            strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)
                            If IsDBNull(drow_contracts.Item("suppliername")) = True Then
                                tmpstr = "nbsp;"
                            Else
                                tmpstr = Trim(drow_contracts.Item("suppliername"))
                            End If
                            strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)
                            strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=""checkbox"" disabled " & archivedVal & " /></td>" & vbNewLine)
                            If showVariations Then
                                strHTML.Append("<td class=""" & rowClass & """ align=""center""><input type=""checkbox"" disabled " & IIf(drow_contracts.Item("IsVariation") = "1", "checked ", "") & " /></td>" & vbNewLine)
                            End If
                            strHTML.Append("</tr>" & vbNewLine)
                        Next
                    Else
                        strHTML.Append("<tr><td class=""row1"" align=""center"" colspan=""6"">No contracts found to match product</td></tr>" & vbNewLine)
                    End If

                    strHTML.Append("</table>" & vbNewLine)
                    strHTML.Append("</div></td></tr>" & vbNewLine)
                Next
            Else
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""row1"" align=""center"">No matching products returned</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
            End If

            strHTML.Append("</table>" & vbNewLine)

            Return strHTML.ToString
        End Function

        Public Function ContractKeySearch(ByVal curUser As CurrentUser, ByVal searchVal As String, ByVal searchStatus As String) As String
            Dim tmpstr As String = ""
            Dim retStr As String = ""
            Dim db As New DBConnection(SpendManagementLibrary.cAccounts.getConnectionString(curUser.Account.accountid))

            If searchVal.Trim <> "" Then
                Dim sql As String = "select [contractId] from contract_details where [contractKey] = @key"
                db.sqlexecute.Parameters.AddWithValue("@key", searchVal)
                Dim conId As Integer = db.getcount(sql)

                If conId = 0 Then
                    retStr = "ContractSummary.aspx?id=" & conId.ToString
                Else
                    tmpstr = "<table class=""datatbl""><tr><td class=""row1""><b>No matching contract key found</b></td></tr></table>"
                    retStr = tmpstr
                End If
            End If

            Return retStr
        End Function

        Public Function createBroadcastMessage(ByVal sbContent As System.Text.StringBuilder, Optional ByVal bmTitle As String = "Broadcast Message", Optional ByVal bDDL_VisibleOnClose As Boolean = False) As String
            Dim output As New System.Text.StringBuilder

            If bmTitle.Trim = "CK Found" Then
                output.Append(sbContent.ToString)
            Else
                output.Append("<div id=""broadcastheader"">")
                output.Append("<span style=""margin-left: 5px; margin-top: 6px; font-size: 24px;"">" & bmTitle & "</span>")
                output.Append("<img style=""curor:pointer; cursor:hand;"" onclick=""" & IIf(bDDL_VisibleOnClose = True, "document.getElementById('CCSearch').style.visibility = 'visible';", "") & "document.getElementById('broadcastmsg').style.display = 'none';"" id=""broadcastclose"" src=""./icons/24/plain/delete2.png"" onmouseover=""document.getElementById('broadcastclose').src = './icons/24/plain/delete2.png';"" onmouseout=""document.getElementById('broadcastclose').src = './icons/24/plain/delete2.png';"">")
                output.Append("</div>")
                output.Append("<div id=""broadcasttxt"">")
                output.Append(sbContent)
                output.Append("</div>")
            End If

            Return output.ToString
        End Function

        Public Function GotoCK(ByVal curUser As CurrentUser, ByVal CKey As String) As String
            Dim db As New DBConnection(SpendManagementLibrary.cAccounts.getConnectionString(curUser.AccountID))
            Dim sql As New System.Text.StringBuilder
            sql.Append("if exists (select [contractId] from contract_details where [contractKey] = @key)")
            sql.Append(" begin ")
            sql.Append("select [contractId] from contract_details where [contractKey] = @key")
            sql.Append(" End ")
            sql.Append("else")
            sql.Append(" begin ")
            sql.Append(" Select 0")
            sql.Append(" end")

            db.sqlexecute.Parameters.AddWithValue("@key", CKey)
            Dim conId As Integer = db.getcount(sql.ToString)
            Dim retStr As String

            If conId > 0 Then
                retStr = "ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & conId.ToString
            Else
                retStr = "ERROR"
            End If

            Return retStr
        End Function
    End Module
End Namespace
